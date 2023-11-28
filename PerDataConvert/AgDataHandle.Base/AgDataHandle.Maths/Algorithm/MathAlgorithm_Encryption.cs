using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AgDataHandle.Maths
{
    /// <summary>
    /// 本文件负责加解密的算法
    /// </summary>
    public partial class MathAlgorithm
    {
        public static byte[] ComputeMD5(string str)
        {
            var byts = Encoding.UTF8.GetBytes(str);
            var md5 = new MD5CryptoServiceProvider();
            var bts = md5.ComputeHash(byts);
            return bts;
        }

        #region RSA
        /// <summary>
        /// 生成公钥和密钥
        /// </summary>
        /// <param name="keySize">密钥的大小，必须以 8 为增量从 384 位到 16384 位</param>
        /// <returns></returns>
        public static RSASecretKey GenerateRSASecretKey(int keySize = -1)
        {
            RSASecretKey rsaKey = new RSASecretKey();
            if (keySize == -1)
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsaKey.PrivateKey = rsa.ToXmlString(true);
                    rsaKey.PublicKey = rsa.ToXmlString(false);
                    rsaKey.pemPrivateKey = RsaKeysFormatConverter.XmlPrivateKeyToPem(rsaKey.PrivateKey);
                    rsaKey.pemPublicKey = RsaKeysFormatConverter.XmlPublicKeyToPem(rsaKey.PublicKey);
                }
            }
            else
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize))
                {
                    rsaKey.PrivateKey = rsa.ToXmlString(true);
                    rsaKey.PublicKey = rsa.ToXmlString(false);
                    rsaKey.pemPrivateKey = RsaKeysFormatConverter.XmlPrivateKeyToPem(rsaKey.PrivateKey);
                    rsaKey.pemPublicKey = RsaKeysFormatConverter.XmlPublicKeyToPem(rsaKey.PublicKey);
                }
            }
            return rsaKey;
        }

        /// <summary>
        /// 使用公钥加密
        /// </summary>
        /// <param name="xmlPublicKey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string xmlPublicKey, string content)
        {
            string encryptedContent = string.Empty;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlPublicKey);
                byte[] encryptedData = rsa.Encrypt(Encoding.Default.GetBytes(content), false);
                encryptedContent = Convert.ToBase64String(encryptedData);
            }
            return encryptedContent;
        }
        /// <summary>
        /// 使用私钥解密
        /// </summary>
        /// <param name="xmlPrivateKey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSADecrypt(string privateKey, string encryptedInput)
        {
            ///string xmlPrivateKey, string content
            //string decryptedContent = string.Empty;
            //using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            //{
            //    rsa.FromXmlString(xmlPrivateKey);
            //    byte[] decryptedData = rsa.Decrypt(Convert.FromBase64String(content), false);
            //    decryptedContent = Encoding.Default.GetString(decryptedData);
            //}
            //return decryptedContent;
            //using (TextReader reader2 = new StringReader(privateKey))
            //{
            //    dynamic key = new Org.BouncyCastle.OpenSsl.PemReader(reader2).ReadObject();
            //    var rsa2 = new RsaEngine();
            //    if (key is AsymmetricKeyParameter)
            //    {
            //        key = (AsymmetricKeyParameter)key;
            //    }
            //    else if (key is AsymmetricCipherKeyPair)
            //    {
            //        key = ((AsymmetricCipherKeyPair)key).Private;
            //    }
            //    rsa2.Init(false, key);  //加密true 解密 false

            //    //entData 待加密解密的 串 
            //    //byte[] entData = Convert.FromBase64String(encryptedInput);
            //    byte[] entData = Encoding.UTF8.GetBytes(encryptedInput);
            //    entData = rsa2.ProcessBlock(entData, 0, entData.Length);
            //    return Encoding.UTF8.GetString(entData);
            //}
            byte[] toDecrypt = Convert.FromBase64String(encryptedInput);
            //AsymmetricCipherKeyPair keyPair;

            var engine = new RsaEngine();
            using (TextReader reader2 = new StringReader(privateKey))
            {
                dynamic key = new Org.BouncyCastle.OpenSsl.PemReader(reader2).ReadObject();
                var rsa2 = new RsaEngine();
                if (key is AsymmetricKeyParameter)
                {
                    key = (AsymmetricKeyParameter)key;
                }
                else if (key is AsymmetricCipherKeyPair)
                {
                    key = ((AsymmetricCipherKeyPair)key).Private;
                }
                engine.Init(false, key);
            }

            return Encoding.UTF8.GetString(engine.ProcessBlock(toDecrypt, 0, toDecrypt.Length));
        }

        public static String PEMRSADecrypt(String pemPrivateKey, string content)
        {
            return "";
        } 
        #endregion
    }

    public struct RSASecretKey
    {
        public RSASecretKey(string privateKey, string publicKey, string _pemPublicKey = null, string _pemPrivateKey = null)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;
            pemPublicKey = _pemPublicKey;
            pemPrivateKey = _pemPrivateKey;
        }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string pemPublicKey { get; set; }
        public string pemPrivateKey { get; set; }
        public override string ToString()
        {
            return string.Format(
                "PrivateKey: {0}\r\nPublicKey: {1}", PrivateKey, PublicKey);
        }
    } 

    public static class RsaKeysFormatConverter
    {
        /// <summary>
        /// XML公钥转成Pem公钥
        /// </summary>
        /// <param name="xmlPublicKey"></param>
        /// <returns></returns>
        public static string XmlPublicKeyToPem(string xmlPublicKey)
        {
            RSAParameters rsaParam;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlPublicKey);
                rsaParam = rsa.ExportParameters(false);
            }
            RsaKeyParameters param = new RsaKeyParameters(false, new BigInteger(1, rsaParam.Modulus), new BigInteger(1, rsaParam.Exponent));

            string pemPublicKeyStr = null;
            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    var pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(sw);
                    pemWriter.WriteObject(param);
                    sw.Flush();

                    byte[] buffer = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(buffer, 0, (int)ms.Length);
                    pemPublicKeyStr = Encoding.UTF8.GetString(buffer);
                }
            }

            return pemPublicKeyStr;
        }

        /// <summary>
        /// Pem公钥转成XML公钥
        /// </summary>
        /// <param name="pemPublicKeyStr"></param>
        /// <returns></returns>
        public static string PemPublicKeyToXml(string pemPublicKeyStr)
        {
            RsaKeyParameters pemPublicKey;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(pemPublicKeyStr)))
            {
                using (var sr = new StreamReader(ms))
                {
                    var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(sr);
                    pemPublicKey = (RsaKeyParameters)pemReader.ReadObject();
                }
            }

            var p = new RSAParameters
            {
                Modulus = pemPublicKey.Modulus.ToByteArrayUnsigned(),
                Exponent = pemPublicKey.Exponent.ToByteArrayUnsigned()
            };

            string xmlPublicKeyStr;
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(p);
                xmlPublicKeyStr = rsa.ToXmlString(false);
            }

            return xmlPublicKeyStr;
        }

        /// <summary>
        /// XML私钥转成PEM私钥
        /// </summary>
        /// <param name="xmlPrivateKey"></param>
        /// <returns></returns>
        public static string XmlPrivateKeyToPem(string xmlPrivateKey)
        {
            RSAParameters rsaParam;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlPrivateKey);
                rsaParam = rsa.ExportParameters(true);
            }

            var param = new RsaPrivateCrtKeyParameters(
              new BigInteger(1, rsaParam.Modulus), new BigInteger(1, rsaParam.Exponent), new BigInteger(1, rsaParam.D),
              new BigInteger(1, rsaParam.P), new BigInteger(1, rsaParam.Q), new BigInteger(1, rsaParam.DP), new BigInteger(1, rsaParam.DQ),
              new BigInteger(1, rsaParam.InverseQ));

            string pemPrivateKeyStr = null;
            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    var pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(sw);
                    pemWriter.WriteObject(param);
                    sw.Flush();

                    byte[] buffer = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(buffer, 0, (int)ms.Length);
                    pemPrivateKeyStr = Encoding.UTF8.GetString(buffer);
                }
            }

            return pemPrivateKeyStr;
        }

        /// <summary>
        /// Pem私钥转成XML私钥
        /// </summary>
        /// <param name="pemPrivateKeyStr"></param>
        /// <returns></returns>
        public static string PemPrivateKeyToXml(string privateKey)
        {
            RsaPrivateCrtKeyParameters pemPrivateKey;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(privateKey)))
            {
                using (var sr = new StreamReader(ms))
                {
                    var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(sr);
                    var keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
                    pemPrivateKey = (RsaPrivateCrtKeyParameters)keyPair.Private;
                }
            }

            var p = new RSAParameters
            {
                Modulus = pemPrivateKey.Modulus.ToByteArrayUnsigned(),
                Exponent = pemPrivateKey.PublicExponent.ToByteArrayUnsigned(),
                D = pemPrivateKey.Exponent.ToByteArrayUnsigned(),
                P = pemPrivateKey.P.ToByteArrayUnsigned(),
                Q = pemPrivateKey.Q.ToByteArrayUnsigned(),
                DP = pemPrivateKey.DP.ToByteArrayUnsigned(),
                DQ = pemPrivateKey.DQ.ToByteArrayUnsigned(),
                InverseQ = pemPrivateKey.QInv.ToByteArrayUnsigned(),
            };

            string xmlPrivateKeyStr;
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(p);
                xmlPrivateKeyStr = rsa.ToXmlString(true);
            }

            return xmlPrivateKeyStr;
            //RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));

            //return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
            //    Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
            //    Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
            //    Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
            //    Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
            //    Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
            //    Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
            //    Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
            //    Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
        }

    }

    public class JsEncryptHelper
    {
        private readonly RSACryptoServiceProvider _privateKeyRsaProvider;
        private readonly RSACryptoServiceProvider _publicKeyRsaProvider;

        public JsEncryptHelper(string privateKey = null, string publicKey = null)
        {
            if (!string.IsNullOrEmpty(privateKey))
            {
                _privateKeyRsaProvider = CreateRsaProviderFromPrivateKey(privateKey);
            }

            if (!string.IsNullOrEmpty(publicKey))
            {
                _publicKeyRsaProvider = CreateRsaProviderFromPublicKey(publicKey);
            }
        }

        public string Decrypt(string cipherText)
        {
            if (_privateKeyRsaProvider == null)
            {
                throw new Exception("_privateKeyRsaProvider is null");
            }

            return Encoding.UTF8.GetString(_privateKeyRsaProvider.Decrypt(Convert.FromBase64String(cipherText), false));
        }

        public string Encrypt(string text)
        {
            if (_publicKeyRsaProvider == null)
            {
                throw new Exception("_publicKeyRsaProvider is null");
            }

            return Convert.ToBase64String(_publicKeyRsaProvider.Encrypt(Encoding.UTF8.GetBytes(text), false));
        }

        private RSACryptoServiceProvider CreateRsaProviderFromPrivateKey(string privateKey)
        {
            byte[] privateKeyBits = Convert.FromBase64String(privateKey);

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSAParameters RSAparams = new RSAParameters();

            using (BinaryReader binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                {
                    binr.ReadByte();
                }
                else if (twobytes == 0x8230)
                {
                    binr.ReadInt16();
                }
                else
                {
                    throw new Exception("Unexpected value read binr.ReadUInt16()");
                }

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                {
                    throw new Exception("Unexpected version");
                }

                bt = binr.ReadByte();
                if (bt != 0x00)
                {
                    throw new Exception("Unexpected value read binr.ReadByte()");
                }

                RSAparams.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.D = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.P = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Q = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DP = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DQ = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }

            RSA.ImportParameters(RSAparams);
            return RSA;
        }

        private int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
            {
                return 0;
            }

            bt = binr.ReadByte();

            if (bt == 0x81)
            {
                count = binr.ReadByte();
            }
            else if (bt == 0x82)
            {
                highbyte = binr.ReadByte();
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }

            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        private RSACryptoServiceProvider CreateRsaProviderFromPublicKey(string publicKeyString)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] x509key;
            byte[] seq = new byte[15];
            int x509size;

            x509key = Convert.FromBase64String(publicKeyString);
            x509size = x509key.Length;

            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            using (MemoryStream mem = new MemoryStream(x509key))
            {
                using (BinaryReader binr = new BinaryReader(mem)) //wrap Memory Stream with BinaryReader for easy reading
                {
                    byte bt = 0;
                    ushort twobytes = 0;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    {
                        binr.ReadByte(); //advance 1 byte
                    }
                    else if (twobytes == 0x8230)
                    {
                        binr.ReadInt16(); //advance 2 bytes
                    }
                    else
                    {
                        return null;
                    }

                    seq = binr.ReadBytes(15); //read the Sequence OID
                    if (!CompareBytearrays(seq, SeqOID)) //make sure Sequence for OID is correct
                    {
                        return null;
                    }

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                    {
                        binr.ReadByte(); //advance 1 byte
                    }
                    else if (twobytes == 0x8203)
                    {
                        binr.ReadInt16(); //advance 2 bytes
                    }
                    else
                    {
                        return null;
                    }

                    bt = binr.ReadByte();
                    if (bt != 0x00) //expect null byte next
                    {
                        return null;
                    }

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    {
                        binr.ReadByte(); //advance 1 byte
                    }
                    else if (twobytes == 0x8230)
                    {
                        binr.ReadInt16(); //advance 2 bytes
                    }
                    else
                    {
                        return null;
                    }

                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                    {
                        lowbyte = binr.ReadByte(); // read next bytes which is bytes in modulus
                    }
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte(); //advance 2 bytes
                        lowbyte = binr.ReadByte();
                    }
                    else
                    {
                        return null;
                    }

                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 }; //reverse byte order since asn.1 key uses big endian order
                    int modsize = BitConverter.ToInt32(modint, 0);

                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {
                        //if first byte (highest order) of modulus is zero, don't include it
                        binr.ReadByte(); //skip this null byte
                        modsize -= 1; //reduce modulus buffer size by 1
                    }

                    byte[] modulus = binr.ReadBytes(modsize); //read the modulus bytes

                    if (binr.ReadByte() != 0x02) //expect an Integer for the exponent data
                    {
                        return null;
                    }

                    int expbytes = binr.ReadByte(); // should only need one byte for actual exponent data (for all useful values)
                    byte[] exponent = binr.ReadBytes(expbytes);

                    // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                    RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                    RSAParameters RSAKeyInfo = new RSAParameters();
                    RSAKeyInfo.Modulus = modulus;
                    RSAKeyInfo.Exponent = exponent;
                    RSA.ImportParameters(RSAKeyInfo);

                    return RSA;
                }
            }
        }

        private bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                {
                    return false;
                }

                i++;
            }

            return true;
        }
    }
}
