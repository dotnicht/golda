using Binebase.Exchange.Common.Domain;
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
            return (ulong)response.Block.GetCoinbaseHeight().Value;
        }

        public Task<string> GenerateAddress(uint index)
        {
            var mnemo = new Mnemonic(_configuration.Mnemonic, Wordlist.English);
            var key = mnemo.DeriveExtKey(_configuration.Password);
            var address = key.Derive(index).ScriptPubKey.GetDestinationAddress(Network);
            return Task.FromResult(address.ToString());
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
            var change = Money.Zero;
            var received = balance.Operations.SelectMany(x => x.ReceivedCoins).ToArray();

            var tx = Transaction.Create(Network);
            var coins = new List<ICoin>();

            for (var index = 0; collected < value && index < received.Length; index++)
            {
                tx.Inputs.Add(new TxIn(received[index].Outpoint, key.ScriptPubKey));
                collected += received[index].TxOut.Value;
            }

            // TODO: get fee.

            if (collected < value)
            {
                throw new InvalidOperationException(ErrorCode.InsufficientBalance);
            }

            // TODO send tx and change;

            tx.Outputs.Add(new TxOut(collected, BitcoinAddress.Create(address, Network)));

            if (change > 0)
            {
                tx.Outputs.Add(new TxOut(change, key.ScriptPubKey.GetDestinationAddress(Network)));
            }

            return (string.Empty, (ulong)value.Satoshi);
        }

        public Task<bool> ValidateAddress(string address)
        {
            var result = false;
            try
            {
                BitcoinAddress.Create(address, Network);
                result = true;
            }
            catch
            {
            }

            return Task.FromResult(result);
        }

        private class EarnResponse
        {
            public int FastestFee { get; set; }
            public int HalfHourFee { get; set; }
            public int HourFee { get; set; }
        }
    }
}
