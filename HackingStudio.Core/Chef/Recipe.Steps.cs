using BosonWare.Cryptography;
using BosonWare.Encoding;
using Realtin.Xdsl;
using System.Security.Cryptography;
using System.Text;

namespace HackingStudio.Core.Chef;

public sealed partial class Recipe
{
    public abstract class Step
    {
        public class Parameter
        {
            public required Type Type { get; set; }

            public required string Name { get; set; }

            public required string Description { get; set; }
        }

        public int InstanceId { get; private set; }

        public virtual string Description => "";

        public virtual Parameter[] Parameters { get; } = [];

        public virtual void Initialize() { }

        public abstract byte[] Perform(byte[] data, XdslElement definedStep);

        public Step Clone()
        {
            var copy = (Step)MemberwiseClone();

            copy.InstanceId = Random.Shared.Next(short.MinValue, int.MaxValue);

            return copy;
        }
    }

    [Step("Base64Encode")]
    public sealed class Base64EncodeStep : Step
    {
        public override string Description => """
            Uses base64 to encode the input data.
            Usage: <Base64Encode />
            """;

        public override byte[] Perform(byte[] data, XdslElement definedStep)
        {
            return Encoding.UTF8.GetBytes(Convert.ToBase64String(data));
        }
    }

    [Step("Base64Decode")]
    public sealed class Base64DecodeStep : Step
    {
        public override string Description => """
            Uses base64 to decode the input data.
            Usage: <Base64Decode />
            """;

        public override byte[] Perform(byte[] data, XdslElement definedStep)
        {
            return Convert.FromBase64String(Encoding.UTF8.GetString(data));
        }
    }

    [Step("Base58Encode")]
    public sealed class Base58EncodeStep : Step
    {
        public override string Description => """
            Uses base58 to encode the input data.
            Usage: <Base58Encode />
            """;

        public override byte[] Perform(byte[] data, XdslElement definedStep)
        {
            byte[] output = Encoding.UTF8.GetBytes(Base58.EncodeData(data));

            return output;
        }
    }

    [Step("Base58Decode")]
    public sealed class Base58DecodeStep : Step
    {
        public override string Description => """
            Uses base58 to decode the input data.
            Usage: <Base58Decode />
            """;

        public override byte[] Perform(byte[] data, XdslElement definedStep)
        {
            var dataStr = Encoding.UTF8.GetString(data);

            var output = Base58.DecodeData(dataStr);

            return output;
        }
    }

    [Step("AesEncrypt")]
    public sealed class AesEncryptStep : Step
    {
        public override string Description => """
            Uses AES to encrypt the input data.
            Note: The key is derived using SHA-256.
            Usage: <AesEncrypt key="your_key"/>
            """;

        public override Parameter[] Parameters => [new Parameter() {
            Type = typeof(string),
            Name = "key",
            Description = "The encryption key."
        }];

        public override byte[] Perform(byte[] data, XdslElement definedStep)
        {
            var keyText = definedStep.GetAttribute("key")?.Value
                ?? throw new ArgumentNullException(nameof(definedStep),
                $"Attribute 'key' is required for AesEncrypt.");

            var key = SHA256.HashData(Encoding.UTF8.GetBytes(keyText));

            using var service = new AesEncryptionService(key);

            return service.Encrypt(data);
        }
    }

    [Step("AesDecrypt")]
    public sealed class AesDecryptStep : Step
    {
        public override string Description => """
            Uses AES to decrypt the input data.
            Note: The key is derived using SHA-256.
            Usage: <AesDecrypt key="your_key"/>
            """;

        public override Parameter[] Parameters => [new Parameter() {
            Type = typeof(string),
            Name = "key",
            Description = "The decryption key."
        }];

        public override byte[] Perform(byte[] data, XdslElement definedStep)
        {
            var keyText = definedStep.GetAttribute("key")?.Value 
                ?? throw new ArgumentNullException(nameof(definedStep), 
                $"Attribute 'key' is required for AesDecrypt.");

            var key = SHA256.HashData(Encoding.UTF8.GetBytes(keyText));

            using var service = new AesEncryptionService(key);

            return service.Decrypt(data);
        }
    }
}