using System;

namespace MultiProcessCommunicator.Spec
{
    /// <summary>
    /// Large object implementation for test reasons
    /// </summary>
    public class LargeObject
    {
        public byte[] Data { get; set; }

        public LargeObject Concatenate(LargeObject largeObject)
        {
            var resultData = new byte[Data.Length + largeObject.Data.Length];

            Buffer.BlockCopy(Data, 0, resultData, 0, Data.Length);
            Buffer.BlockCopy(largeObject.Data, 0, resultData, Data.Length, largeObject.Data.Length);

            return new LargeObject { Data = resultData };
        }

        public static LargeObject GenerateRandom(int size)
        {
            var buffer = new byte[size];
            Random randNum = new Random();
            for (int i = 0; i < size; i++)
                buffer[i] = (byte)randNum.Next(byte.MinValue, byte.MaxValue);


            return new LargeObject { Data = buffer };
        }
    }
}
