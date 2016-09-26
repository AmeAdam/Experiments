using System.Linq;
using AmeCommon.CardsCapture;
using NUnit.Framework;

namespace AmeTests
{
    [TestFixture]
    public class ChecksumTest
    {
        private byte[] buff;

        [SetUp]
        public void SetUp()
        {
            buff = new byte[4 * 1024 * 1024];
            for (int i = 0; i < buff.Length; i++)
                buff[i] = (byte)(i % 100);
        }

        [Test]
        public void CalculateChecksumOneStep()
        {
            var calc = new ChecksumFileCalculator();
            calc.AppendBuffer(buff);
            Assert.AreEqual("ea9935f794b52c389b0fde30f4e0d837", calc.GetChecksum());
        }

        [Test]
        public void CalculateChecksumMultipleSteps()
        {
            var calc = new ChecksumFileCalculator();
            for (int i = 0; i < 4; i++)
            {
                var smallBuff = buff.Skip(i*1024*1024).Take(1024*1024).ToArray();
                calc.AppendBuffer(smallBuff);
            }
            Assert.AreEqual("ea9935f794b52c389b0fde30f4e0d837", calc.GetChecksum());
        }

        [Test]
        public void CalculateChecksumMultipleVariableSteps()
        {
            var calc = new ChecksumFileCalculator();
            calc.AppendBuffer(buff.Take(100).ToArray());
            calc.AppendBuffer(buff.Skip(100).Take(buff.Length-150).ToArray());
            calc.AppendBuffer(buff.Skip(buff.Length-50).Take(50).ToArray());
            Assert.AreEqual("ea9935f794b52c389b0fde30f4e0d837", calc.GetChecksum());
        }

    }
}
