using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.CryptoService.Application;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Binebase.Exchange.CryptoService.Domain.Entities;
using Binebase.Exchange.CryptoService.Domain.Enums;
using Microsoft.Extensions.Options;
using NBitcoin;
using QBitNinja.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
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

        public async Task<string> GenerateAddress(uint index)
        {
            var mnemo = new Mnemonic(_configuration.Mnemonic, Wordlist.English);
            var key = mnemo.DeriveExtKey(_configuration.Password);
            var address = key.Derive(index).ScriptPubKey.GetDestinationAddress(Network);
            return await Task.FromResult(address.ToString());
        }

        public async Task<decimal> GetBalance(string address)
        {
            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            var client = new QBitNinjaClient(Network);
            var balance = await client.GetBalance(BitcoinAddress.Create(address, Network));
            return new Money(balance.Operations.Sum(x => x.Amount)).ToDecimal(MoneyUnit.BTC);
        }

        public async Task<Domain.Entities.Transaction[]> GetTransactions(string address)
        {
            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            var client = new QBitNinjaClient(Network);
            var balance = await client.GetBalance(BitcoinAddress.Create(address, Network));

            return balance.Operations
                .Where(x => (ulong)x.Confirmations >= _configuration.ConfirmationsCount)
                .Select(x => new Domain.Entities.Transaction
                {
                    Direction = TransactionDirection.Inbound,
                    Confirmations = (ulong)x.Confirmations,
                    Confirmed = x.FirstSeen.DateTime,
                    Status = TransactionStatus.Confirmed,
                    Hash = x.TransactionId.ToString(),
                    RawAmount = (ulong)x.Amount.Satoshi,
                    Amount = x.Amount.ToDecimal(MoneyUnit.BTC)
                })
                .Where(x => x.Amount > 0)
                .ToArray();
        }

        public async Task<Domain.Entities.Transaction> GetTransaction(string hash)
        {
            if (hash is null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

            var client = new QBitNinjaClient(Network);
            var response = await client.GetTransaction(new uint256(hash));

            if (response == null)
            {
                return new Domain.Entities.Transaction
                {
                    Status = TransactionStatus.Failed
                };
            }

            var amount = response.SpentCoins.Select(x => x.Amount).Aggregate((x, y) => x.Add(y)) as Money;

            return new Domain.Entities.Transaction
            {
                Direction = TransactionDirection.Outbound,
                Confirmations = (ulong)(response.Block?.Confirmations ?? 0),
                Confirmed = response.FirstSeen.DateTime,
                Status = response.Block == null || (ulong)response.Block.Confirmations < _configuration.ConfirmationsCount
                    ? TransactionStatus.Published
                    : TransactionStatus.Confirmed,
                Hash = response.TransactionId.ToString(),
                RawAmount = (ulong)amount.Satoshi,
                Amount = amount.ToDecimal(MoneyUnit.BTC)
            };
        }

        public async Task<(string Hash, ulong Amount)> PublishTransaction(decimal amount, string address)
        {
            var key = new Mnemonic(_configuration.Mnemonic, Wordlist.English)
                .DeriveExtKey(_configuration.Password)
                .Derive((uint)_configuration.WithdrawAccountIndex);

            var client = new QBitNinjaClient(Network);
            var balance = await client.GetBalance(key.ScriptPubKey, true);

            var value = Money.Coins(amount);
            var collected = Money.Zero;
            var received = balance.Operations.SelectMany(x => x.ReceivedCoins).ToArray();

            var tx = NBitcoin.Transaction.Create(Network);
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

        public async Task<Domain.Entities.Transaction[]> TransferAssets(Address[] addresses, string address)
        {
            if (addresses is null)
            {
                throw new ArgumentNullException(nameof(addresses));
            }

            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            var list = new List<Domain.Entities.Transaction>();
            var client = new QBitNinjaClient(Network);
            var root = new Mnemonic(_configuration.Mnemonic, Wordlist.English).DeriveExtKey(_configuration.Password);

            var transaction = NBitcoin.Transaction.Create(Network);
            var coins = new List<ICoin>();
            var secrets = new List<ISecret>();

            var total = Money.Zero;

            foreach (var addr in addresses)
            {
                var key = root.Derive(addr.Index.Value);
                var balance = await client.GetBalance(key.ScriptPubKey, true);
                var received = balance.Operations.SelectMany(x => x.ReceivedCoins).ToArray();

                coins.AddRange(received);
                secrets.Add(key.PrivateKey.GetBitcoinSecret(Network));
                var value = Money.Zero;

                foreach (var coin in received)
                {
                    transaction.Inputs.Add(new TxIn(coin.Outpoint, key.ScriptPubKey));
                    value += coin.Amount as Money;
                }

                var tx = new Domain.Entities.Transaction
                {
                    Id = Guid.NewGuid(),
                    AddressId = addr.Id,
                    Direction = TransactionDirection.Transfer,
                    Status = TransactionStatus.Published,
                    RawAmount = value,
                    Amount = value.ToDecimal(MoneyUnit.BTC)
                };

                total += value;
                list.Add(tx);
            }

            var response = await _httpClient.Get<EarnResponse>(_configuration.EarnAddress);
            total -= Money.Satoshis(response.FastestFee * transaction.GetVirtualSize());

            transaction.Outputs.Add(new TxOut(total, BitcoinAddress.Create(address, Network)));
            transaction.Sign(secrets.ToArray(), coins.ToArray());
            var result = await client.Broadcast(transaction);

            if (!result.Success)
            {
                throw new InvalidOperationException(result.Error.ErrorCode.ToString());
            }

            var hash = transaction.GetHash().ToString();
            list.ForEach(x => x.Hash = hash);
            return list.ToArray();
        }

        private class EarnResponse
        {
            public int FastestFee { get; set; }
            public int HalfHourFee { get; set; }
            public int HourFee { get; set; }
        }
    }
}
