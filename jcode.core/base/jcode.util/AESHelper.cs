using System;
using System.Security.Cryptography;
using System.Text;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2022.04.19
    /// 描 述：加密、解密帮助类
    /// </summary>
    public class AESHelper
    {
        #region AES加密解密 
        /// <summary>
        /// 32位处理key 
        /// </summary>
        /// <param name="key">原字节</param>
        /// <returns></returns>
        private static string GetAesKey(string key)
        {
            var len = key.Length;
            if (len < 32)
            {
                for (var i = 0; i < 32 - len; i++)
                {
                    key += "0";
                }
                return key;
            }
            else
            {
                return key[..32];
            }
        }
        /// <summary>
        /// 使用AES加密字符串
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <param name="key">秘钥</param>
        /// <param name="vi">偏移量</param>
        /// <returns>Base64字符串结果</returns>
        public static string AesEncrypt(string content, string key,string vi = "1234567890000000")
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(GetAesKey(key));
            byte[] ivArray = Encoding.UTF8.GetBytes(vi);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(content);

            SymmetricAlgorithm des = Aes.Create();
            des.IV = ivArray;
            des.Key = keyArray;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = des.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray);
        }
        /// <summary>
        /// 使用AES解密字符串
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="key">秘钥</param>
        /// <param name="vi">偏移量</param>
        /// <returns>UTF8解密结果</returns>
        public static string AesDecrypt(string content, string key,string vi = "1234567890000000")
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(GetAesKey(key));
            byte[] ivArray = Encoding.UTF8.GetBytes(vi);
            byte[] toEncryptArray = Convert.FromBase64String(content);

            SymmetricAlgorithm des = Aes.Create();
            des.IV = ivArray;
            des.Key = keyArray;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = des.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
        #endregion
    }
}
