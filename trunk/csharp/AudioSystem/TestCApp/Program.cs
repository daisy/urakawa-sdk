using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TestCApp
{
	class Program
	{
		static void Main(string[] args)
		{
			short val16 = 0;
			Console.WriteLine("Int16 {0:0}: {1}", val16, WriteByteArrayRep(BitConverter.GetBytes(val16)));
			val16 = Int16.MinValue;
			Console.WriteLine("Int16 {0:0}: {1}", val16, WriteByteArrayRep(BitConverter.GetBytes(val16)));
			val16 = Int16.MaxValue;
			Console.WriteLine("Int16 {0:0}: {1}", val16, WriteByteArrayRep(BitConverter.GetBytes(val16)));
			int val32 = 0;
			Console.WriteLine("Int32 {0:0}: {1}", val32, WriteByteArrayRep(BitConverter.GetBytes(val32)));
			val32 = Int32.MinValue;
			Console.WriteLine("Int32 {0:0}: {1}", val32, WriteByteArrayRep(BitConverter.GetBytes(val32)));
			val32 = Int32.MaxValue;
			Console.WriteLine("Int32 {0:0}: {1}", val32, WriteByteArrayRep(BitConverter.GetBytes(val32)));
			long val64 = 0;
			Console.WriteLine("Int64 {0:0}: {1}", val64, WriteByteArrayRep(BitConverter.GetBytes(val64)));
			val64 = Int64.MinValue;
			Console.WriteLine("Int64 {0:0}: {1}", val64,WriteByteArrayRep( BitConverter.GetBytes(val64)));
			val64 = Int64.MaxValue;
			Console.WriteLine("Int64 {0:0}: {1}", val64, WriteByteArrayRep(BitConverter.GetBytes(val64)));

		}

		static string WriteByteArrayRep(byte[] data)
		{
			string rep = "";
			foreach (byte b in data)
			{
				rep += b.ToString("X2");
			}
			return rep;
		}
	}
}
