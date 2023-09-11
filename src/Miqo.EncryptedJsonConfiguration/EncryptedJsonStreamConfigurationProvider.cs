using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using CryptHash.Net;
using CryptHash.Net.Encryption.AES.AEAD;
using Microsoft.Extensions.Configuration.Json;
using RegExtract;

namespace Miqo.EncryptedJsonConfiguration
{
    /// <summary>
    /// Loads configuration key/values from a JSON stream into a provider.
    /// </summary>
    public class EncryptedJsonStreamConfigurationProvider : StreamConfigurationProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="source">The <see cref="JsonStreamConfigurationSource"/>.</param>
        public EncryptedJsonStreamConfigurationProvider(EncryptedJsonStreamConfigurationSource source) : base(source) { }

        /// <summary>
        /// Loads JSON configuration key/values from a stream into a provider.
        /// </summary>
        /// <param name="stream">The encrypted JSON <see cref="Stream"/> to load configuration data from.</param>
        public override void Load(Stream stream)
        {
            var source = (EncryptedJsonStreamConfigurationSource)Source;

            try
            {
                var encryptedSettings = stream.ToBytes();
                var aes = new AEAD_AES_256_GCM();
                var decryptionResult = aes.DecryptString(encryptedSettings, source.Key);

                if (!decryptionResult.Success) {
                    var message = decryptionResult.Message.Split(Environment.NewLine);
                    var (exceptionName, exceptionMessage) = 
                        decryptionResult.Message.Extract<(string exceptionName, string exceptionMessage)>(@"([\w\.]+Exception\:)([\w\s]+\.)");
                    exceptionName = exceptionName.TrimEnd(':');

                    if (exceptionName == typeof(CryptographicException).FullName) {
                        throw new CryptographicException(exceptionMessage.Trim() + 
                        $"{Environment.NewLine}NOTE: This exception is a newly allocated one without the original stack trace as the CryptHash library swallows the original and only forwards the message.");
                    }
                }

                var decryptedStream = new MemoryStream(decryptionResult.DecryptedDataBytes);
                Data = EncryptedJsonConfigurationFileParser.Parse(decryptedStream);
            }
            catch (Exception e)
            {
                throw new FormatException("Could not parse the encrypted JSON stream", e);
            }
        }
    }
}