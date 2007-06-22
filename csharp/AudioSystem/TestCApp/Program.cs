using System;
using System.Collections.Generic;
using System.Text;

namespace TestCApp
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				Console.WriteLine("Inside try before return statement");
				return;
			}
			finally
			{
				Console.WriteLine("Inside finally");
			}
		}
	}
}
