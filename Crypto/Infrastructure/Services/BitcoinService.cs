using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.CryptoService.Application;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Binebase.Exchange.CryptoService.Domain.Enums;
using Microsoft.Extensions.Options;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class BitcoinService : IBlockchainService
    {
        private readonly Configuration _configuration;
        private readonly HttpClient _httpClient;

        private Network Network => _configuration.IsTestNet ? Network.TestNet : Network.Main;

        public Currency Currency => Currency.BTC;

        public BitcoinService(IOptions<Configuration> options, HttpClient httpClient)
            => (_configuration, _httpClient) = (options.Value, httpClient);

        public async Task<ulong> CurrentIndex()
        {
            var client = new QBitNinjaClient(Network);
            var response = await client.GetBlock(new BlockFeature { Special = SpecialFeature.Last });
            if (response == null)
            {
                return 0;
            }

            return (ulong)response.Block.GetCoinbaseHeight().Value;
        }

        public async Task<string> GenerateAddress(uint index)
        {
            var mnemo = new Mnemonic(_configuration.Mnemonic, Wordlist.English);
            var key = mnemo.DeriveExtKey(_configuration.Password);
            var address = key.Derive(index).ScriptPubKey.GetDestinationAddress(Network);
            return await Task.FromResult(address.ToString());
        }

        public async Task<decimal> GetBalance(string address)
        {
            var client = new QBitNinjaClient(Network);
            var balance = await client.GetBalance(BitcoinAddress.Create(address, Network));
            return new Money(balance.Operations.Sum(x => x.Amount)).ToDecimal(MoneyUnit.BTC);
        }

        public async Task<Domain.Entities.Transaction[]> GetTransactions(string address)
        {
            var client = new QBitNinjaClient(Network);
            var balance = await client.GetBalance(BitcoinAddress.Create(address, Network));
            return balance.Operations.Select(x => new Domain.Entities.Transaction
            {
                Direction = TransactionDirection.Inbound,
                Confirmed = x.FirstSeen.DateTime,
                Status = TransactionStatus.Confirmed,
                Hash = x.TransactionId.ToString(),
                Block = (ulong)x.Height,
                RawAmount = (ulong)x.Amount.Satoshi,
                Amount = x.Amount.ToDecimal(MoneyUnit.BTC)
            }).ToArray();
        }

        public async Task<(string Hash, ulong Amount)> PublishTransaction(decimal amount, string address)
        {
            var key = new Mnemonic(_configuration.Mnemonic, Wordlist.English)
                .DeriveExtKey(_configuration.Password)
                .Derive((uint)_configuration.WithdrawAccountIndex);

            var client = new QBitNinjaClient(Network);
            var balance = await client.GetBalance(key.ScriptPubKey);

            var value = Money.Coins(amount);
            var collected = Money.Zero;
            var received = balance.Operations.SelectMany(x => x.ReceivedCoins).ToArray();

            var tx = Transaction.Create(Network);
            var coins = new List<ICoin>();

            var response = await _httpClient.Get<EarnResponse>(_configuration.EarnAddress);
            var fee = Money.Zero;

            for (var index = 0; collected < value + fee && index < received.Length; index++)
            {
                coins.Add(received[index]);
                tx.Inputs.Add(new TxIn(received[index].Outpoint, key.ScriptPubKey));
                fee = Money.Satoshis(response.FastestFee * tx.GetVirtualSize());
                collected += received[index].TxOut.Value;
            }

            if (collected < value + fee)
            {
                throw new InvalidOperationException(ErrorCode.InsufficientBalance);
            }

            var change = collected - fee - value;
            if (change > Money.Zero)
            {
                tx.Outputs.Add(new TxOut(change, key.ScriptPubKey.GetDestinationAddress(Network)));
            }

            tx.Outputs.Add(new TxOut(value, BitcoinAddress.Create(address, Network)));
            tx.Sign(key.PrivateKey, coins.ToArray());

            var result = await client.Broadcast(tx);

            if (!result.Success)
            {
                throw new InvalidOperationException(result.Error.ErrorCode.ToString());
            }

            return (tx.GetHash().ToString(), (ulong)value.Satoshi);
        }

        public async Task<bool> ValidateAddress(string address)
        {
            var result = false;
            try
            {
                BitcoinAddress.Create(address, Network);
                result = true;
            }
            catch { }

            return await Task.FromResult(result);
        }

        private class EarnResponse
        {
            public int FastestFee { get; set; }
            public int HalfHourFee { get; set; }
            public int HourFee { get; set; }
        }
    }
}
