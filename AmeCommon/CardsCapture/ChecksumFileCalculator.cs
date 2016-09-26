using System.Security.Cryptography;
using System.Text;

namespace AmeCommon.CardsCapture
{
    public class ChecksumFileCalculator
    {
        private readonly MD5 md5 = MD5.Create();

        public ChecksumFileCalculator()
        {
            md5.Initialize();
        }

        public void AppendBuffer(byte[] buffer, int? bufferSize = null)
        {
            int offset = 0;
            int size = bufferSize ?? buffer.Length;
            while (offset < size)
            {
                offset += md5.TransformBlock(buffer, offset, size, buffer, offset);
            }
        }

        public string GetChecksum()
        {
            md5.TransformFinalBlock(new byte[0], 0, 0);
            var sb = new StringBuilder();
            foreach (var b in md5.Hash)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString();
        }
    }
}
