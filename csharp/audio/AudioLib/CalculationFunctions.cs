using System;
using System.IO;

namespace AudioLib
{
    public static class StreamUtils
    {
        public static uint Copy(Stream from, ulong fromNumberOfBytes, Stream to, uint buffer_size)
        {
            if (fromNumberOfBytes == 0)
            {
                if (from.CanSeek)
                {
                    fromNumberOfBytes = (ulong) from.Length;
                }
            }

            if (fromNumberOfBytes > 0 && fromNumberOfBytes <= buffer_size)
            {
                byte[] buffer = new byte[fromNumberOfBytes];
                int bytesRead = from.Read(buffer, 0, (int)fromNumberOfBytes);
                if (bytesRead > 0)
                {
                    to.Write(buffer, 0, bytesRead);
                    return (uint)bytesRead;
                }

                return 0;
            }
            else
            {
                int bytesRead = 0;
                int totalBytesWritten = 0;
                byte[] buffer = new byte[buffer_size];

                while ((bytesRead = from.Read(buffer, 0, (int)buffer_size)) > 0)
                {
                    if (fromNumberOfBytes > 0 && (ulong)(totalBytesWritten + bytesRead) > fromNumberOfBytes)
                    {
                        int bytesToWrite = (int)(fromNumberOfBytes - (ulong)totalBytesWritten);
                        if (bytesToWrite > 0)
                        {
                            to.Write(buffer, 0, bytesToWrite);
                            totalBytesWritten += bytesToWrite;
                        }
                        break;
                    }

                    to.Write(buffer, 0, bytesRead);
                    totalBytesWritten += bytesRead;
                }

                return (uint)totalBytesWritten;
            }
        }
    }

    public class CalculationFunctions
    {
        public static long ConvertToDecimal(int[] Ar)
        {
            //convert from mod 256 to mod 10
            return Ar[0] + (Ar[1] * 256) + (Ar[2] * 256 * 256) + (Ar[3] * 256 * 256 * 256);
        }

        public static int[] ConvertFromDecimal(long lVal)
        {
            // convert  mod 10 to 4 byte array each of mod 256
            int[] Result = new int[4];
            Result[0] = Result[1] = Result[2] = Result[3] = 0;
            for (int i = 0; i < 4; i++)
            {
                Result[i] = Convert.ToInt32(lVal % 256);
                lVal = lVal / 256;
            }
            return Result;
        }
    }
}
