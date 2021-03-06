using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
    public static class StreamUtils
    {
        /// <summary>
        /// Tests the byte-to-byte equality of the raw PCM datas contained in two sequences of wav streams.
        /// The wav header of each stream is skipped before the comparison. 
        /// </summary>
        /// <param name="seq1">a squence of wav streams</param>
        /// <param name="seq2">another squence of wav streams</param>
        /// <returns>true if the two wav sequences are byte-to-byte equal</returns>
        public static bool CompareWavSeq(List<Stream> seq1, List<Stream> seq2)
        {
            int i1 = 0;
            int i2 = 0;
            Stream s1 = seq1[i1];
            Stream s2 = seq2[i2];
            s1.Seek(44, SeekOrigin.Begin);
            s2.Seek(44, SeekOrigin.Begin);
            while (true)
            {
                int b1 = s1.ReadByte();
                int b2 = s2.ReadByte();
                if (b1 == -1 && i1 < seq1.Count - 1)
                {
                    s1 = seq1[++i1];
                    s1.Seek(44, SeekOrigin.Begin);
                    b1 = s1.ReadByte();
                }
                if (b2 == -1 && i2 < seq2.Count - 1)
                {
                    s2 = seq2[++i2];
                    s2.Seek(44, SeekOrigin.Begin);
                    b2 = s2.ReadByte();
                }
                if (b1 != b2)
                {
                    return false;
                }
                if (b1 == -1)
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Tests the byte-to-byte equality of two byte streams. 
        /// </summary>
        /// <param name="seq1">a stream</param>
        /// <param name="seq2">another stream</param>
        /// <returns>true if the two streams are byte-to-byte equal</returns>
        public static bool CompareStreams(Stream s1, Stream s2)
        {
            while (true)
            {
                int b1 = s1.ReadByte();
                int b2 = s2.ReadByte();
                if (b1 != b2)
                {
                    return false;
                }
                if (b1 == -1 || b2 == -1)
                {
                    return true;
                }
            }
        }

        public const int BYTE_BUFFER_LENGTH = 1024 * 10; // 10 KB 

        public static void CopyData(Stream source, Stream dest)
        {
            byte[] buf = new byte[BYTE_BUFFER_LENGTH];
            while (source.Position < source.Length)
            {
                int byteCount = source.Read(buf, 0, buf.Length);
                if (byteCount > 0)
                {
                    dest.Write(buf, 0, byteCount);
                }
            }
        }

        public static MemoryStream GetRandomMemoryStream(int length)
        {
            MemoryStream memStm = new MemoryStream();
            System.Security.Cryptography.RandomNumberGenerator rndGen =
                System.Security.Cryptography.RandomNumberGenerator.Create();
            byte[] buf = new byte[BYTE_BUFFER_LENGTH];
            int count = 0;
            while (count < length)
            {
                int l = length - count;
                if (l > buf.Length) l = buf.Length;
                rndGen.GetBytes(buf);
                memStm.Write(buf, 0, l);
                count += l;
            }
            memStm.Seek(0, SeekOrigin.Begin);
            return memStm;
        }
    }
}