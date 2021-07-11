using System;
using System.Security.Cryptography;
using System.Text;

namespace Parto.Helper.Extensions
{
    public static class ByteArrayExtension
    {
        public static byte[] GetByteArray(this int data) => BitConverter.GetBytes(data);

        public static byte[] GetByteArray(this uint data) => BitConverter.GetBytes(data);

        public static byte[] GetByteArray(this short data) => BitConverter.GetBytes(data);

        public static byte[] GetByteArray(this ushort data) => BitConverter.GetBytes(data);

        public static byte[] GetByteArray(this long data) => BitConverter.GetBytes(data);

        public static byte[] GetByteArray(this ulong data) => BitConverter.GetBytes(data);

        public static byte[] GetByteArray(this double data) => BitConverter.GetBytes(data);

        public static byte[] GetByteArray(this float data) => BitConverter.GetBytes(data);

        public static byte[] GetByteArray(this bool data) => BitConverter.GetBytes(data);

        public static byte[] GetByteArray(this char data) => BitConverter.GetBytes(data);

        public static byte[] GetByteArray(this string data, Encoding? encoding = null) =>
            (encoding ?? Encoding.UTF8).GetBytes(data);

        public static byte[] GetHash(this byte[] data) => data.GetHash(HashAlgorithmName.SHA512);

        // ReSharper disable once MemberCanBePrivate.Global
        public static byte[] GetHash(this byte[] data, HashAlgorithmName hashAlgorithmName)
        {
            using var hashAlgorithm = HashAlgorithm.Create((hashAlgorithmName.Name ?? HashAlgorithmName.SHA512.Name) ??
                throw new InvalidOperationException());
            return hashAlgorithm?.ComputeHash(data) ?? Array.Empty<byte>();
        }
    }
}