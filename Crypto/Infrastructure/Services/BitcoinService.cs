using AutoMapper.Configuration;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class BitcoinService : IBitcoinService, IConfigurationProvider<BitcoinService.Configuration>, ITransient<IBitcoinService>
    {
        public async Task<string> GenerateKeys()
        {
            var passphraseCode = new BitcoinPassphraseCode("my secret", Network.Main, null);
            EncryptedKeyResult encryptedKeyResult = passphraseCode.GenerateEncryptedSecret();
            var generatedAddress = encryptedKeyResult.GeneratedAddress;
            var encryptedKey = encryptedKeyResult.EncryptedKey;
            var confirmationCode = encryptedKeyResult.ConfirmationCode;
            var bitcoinPrivateKey = encryptedKey.GetSecret("my secret");

            throw new NotImplementedException();
        }

        public class Configuration
        {

        }
    }
}
