using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


namespace  Eloi
{




    /// <summary>
    ///  // https://www.browserling.com/tools/base58-decode
    //   // https://www.browserling.com/tools/base58-encode
    /// Bit4096 because I only need to sign from time to time, so I can afford to have a big key
    /// Pkcs1 because it is the most common
    /// Sha256 because it is the most common
    /// B58 because I need to be able to copy past in url. 
    /// Fromat: 123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz
    /// </summary>
    /// 
    ///https://raskeyconverter.azurewebsites.net/XmlToPem?handler=ConvertPEM
    ///



    public static class Bit4096B58Pkcs1SHA256
    {
        /// <summary>
        /// Allows to identify if a text is a public key in base58 format
        /// </summary>
        public const string START_PUBLIC_KEY = "pBit4096B58Pkcs1SHA256";
        /// <summary>
        /// Allows to identify if a text is a  private key in base58 format
        /// </summary>
        public const string START_PRIVATE_KEY = "PBit4096B58Pkcs1SHA256";

        /// <summary>
        /// The aim of this tool is to provide handshake to encrypted data
        /// So the key size is big, just to be used from time to time
        /// Reddit said:
        /// - that 512 is hacked
        /// - 1024 is not safe
        /// - 2048 is safe but ai could break it soon
        /// - 4096 is safe but quantum computer could break it soon
        /// If it happens in the future, this tool need to be achived.
        /// And a new encryption tool need to be created.
        /// </summary>
        public const int KEY_SIZE = 4096;


        public static void CreateXmlRsa4096PrivateKey(out string privateKeyXmlRsa4096)
        {
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = KEY_SIZE;
                privateKeyXmlRsa4096 = rsa.ToXmlString(true);
            }
        }

        public static void IsTextStartWithBase58Tag(string text, 
            out bool isB58, 
            out bool isPublic)
        {
            if (string.IsNullOrEmpty(text))
            {
                isB58 = false;
                isPublic = false;
                return;
            }
            isB58 = text.StartsWith(START_PUBLIC_KEY) || text.StartsWith(START_PRIVATE_KEY);
            isPublic = text.StartsWith("p");
        }

        /// <summary>
        ///  Algorithm is Pkcs1 and HashAlgorithmName is SHA256
        /// </summary>
        /// <param name="bytesDataToSign"></param>
        /// <param name="privateKeyAsXmlFormat"></param>
        /// <param name="constructSignatureInBytes"></param>
        public static  void SignDataWithRsaXmlPrivateKey(
            in byte[] bytesDataToSign, 
            in string privateKeyAsXmlFormat,
            out byte[] constructSignatureInBytes)
        {
            constructSignatureInBytes = null;
            using (RSA rsa = RSA.Create())
            {
                rsa.KeySize = KEY_SIZE;
                rsa.FromXmlString(privateKeyAsXmlFormat);
                constructSignatureInBytes= rsa.SignData(bytesDataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        public static void SignTextUtf8ToTextBase58(
            in string textInUTF8, 
            in string xmlPrivate, 
            out string signatureAsBase58)
        {
            UTF8ToBytes(textInUTF8, out byte[] data);
            SignDataWithRsaXmlPrivateKey(data, xmlPrivate, out byte[] signatureAsBytes);
            BytesToBase58(signatureAsBytes, out signatureAsBase58);
        }

        public static void SignBytesDataInTextBase58(
            in byte[] data, 
            in string xmlPrivate, out string signatureAsBase58)
        {
            SignDataWithRsaXmlPrivateKey(data, xmlPrivate, out byte[] signatureAsBytes);
            BytesToBase58(signatureAsBytes, out signatureAsBase58);
        }


        public static void CreateFromPrivateKeyAsBase58(
            string xmlPrivateKey4096
            , out string resultingPublicKey
            , out string privateKeyAsTextBase58
            , out string publicKeyAsTextBase58
            )
        {
            using (var rsa = RSA.Create())
            {
                rsa.FromXmlString(xmlPrivateKey4096);
                resultingPublicKey = rsa.ToXmlString(false);
                StringToBaseBase58(xmlPrivateKey4096
                    , out string b58PrivateKey);
                StringToBaseBase58(resultingPublicKey
                    , out string b58PublicAddress);
                privateKeyAsTextBase58 = START_PRIVATE_KEY + b58PrivateKey;
                publicKeyAsTextBase58 = START_PUBLIC_KEY + b58PublicAddress;

            }
        }

        public static void CreatePrivateKeyAsBase58(
            out string privateKey,
            out string publicKey,
            out string privateKeyBase58,
            out string publicKeyBase58
            ) 
        {
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = KEY_SIZE;
                privateKey = rsa.ToXmlString(true);
                publicKey = rsa.ToXmlString(false);
                StringToBaseBase58(privateKey
                    , out string b58PrivateKey);
                StringToBaseBase58(publicKey
                    , out string b58PublicAddress);
                privateKeyBase58= START_PRIVATE_KEY + b58PrivateKey;
                publicKeyBase58=START_PUBLIC_KEY + b58PublicAddress;
        
            }
        }

        public static void TextToSHA256(string text, out string textHashed)
        {
            SHA256Encoder.TextToSHA256(text, out textHashed);
        }



        public static bool VerifySignatureFromB58Signature(
            in string messageAsUTF8, 
            in string signatureAsBase58,
            in string publicXmlKey
            
            )
        {

            byte[] dataByteFromUtf = System.Text.Encoding.UTF8.GetBytes(messageAsUTF8);
            Base58ToBytes(signatureAsBase58, out byte[] signature);
            return VerifySignatureFromRawBytes(dataByteFromUtf, signature, publicXmlKey);

        }

        public static bool VerifySignatureFromRawBytes(in byte[] dataUtf8, in byte[] signatureAsBytes, in string publicKeyXml)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.KeySize = KEY_SIZE;
                rsa.FromXmlString(publicKeyXml);
                return rsa.VerifyData(dataUtf8, signatureAsBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        /// <summary>
        ///  Static page provided by eloistree to sign a message with metamask
        ///  As a github page with Javascript in HTML page.
        /// </summary>
        public static string SIGN_WITH_META_MASK_URL_GITHUB = "https://eloistree.github.io/SignMetaMaskTextHere/index.html?q=";
        public static void OpenUrlToSignTextWithMetaMask(string textToSignCompatibleWithUrl)
        {
            Application.OpenURL(SIGN_WITH_META_MASK_URL_GITHUB + textToSignCompatibleWithUrl);
        }
        public static void OpenUrlToSignTextAsBase58WithMetaMask(string textToSignCompatibleWithUrl)
        {
            StringToBaseBase58(textToSignCompatibleWithUrl, out string b58Text);
            Application.OpenURL(SIGN_WITH_META_MASK_URL_GITHUB + b58Text);
        }




        public static void BytesToUTF8(byte[] bytes, out string utf8)
        {
            utf8 = System.Text.Encoding.UTF8.GetString(bytes);
        }
        public static void UTF8ToBytes(string utf8, out byte[] bytes)
        {
            bytes = System.Text.Encoding.UTF8.GetBytes(utf8);
        }

        public static void StringToBaseBase58(string str, out string b58)
        {
            b58 = Base58Encoder.Base58EncodeToUTF8(str);
        }
        public static void Base58ToString(string b58, out string str)
        {
            str = Base58Encoder.Base58DecodeFromUTF8(b58);
        }

        public static void BytesToBase58(byte[] bytes, out string b58)
        {
            b58 = Base58Encoder.Base58Encode(bytes);
        }
        public static void Base58ToBytes(string b58, out byte[] bytes)
        {
            bytes = Base58Encoder.Base58Decode(b58);
        }



    }
}
