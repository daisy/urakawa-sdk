using System;
using System.Diagnostics;
using urakawa;
using urakawa.core;
using urakawa.properties.channel;
using urakawa.examples;


namespace urakawa.test
{
	/// <summary>
	/// FileIOTest is a console application which can open and the optionally save a urakawa toolkit Xuk file
	/// </summary>
	public class FileIOTest
	{
		/// <summary>
		/// Description of the usage of the command line tool
		/// </summary>
		static string USAGE
			= "FileIOTest -i:<input_xuk> [-o:<output_xuk>]";

		/// <summary>
		/// The path of the Xuk input file to open
		/// </summary>
		static string inputXuk;
		/// <summary>
		/// The path of the Xuk output file to save
		/// </summary>
		static string outputXuk;

		/// <summary>
		/// Parses an command line argument (having form -localName:value)
		/// </summary>
		/// <param name="arg">The command line argument to parse</param>
		/// <param name="localName">A <see cref="string"/> in which to output the localName part of the argument</param>
		/// <param name="val">A <see cref="string"/> in which to return the value part of the argument</param>
		/// <returns>A <see cref="bool"/> indicating if the command line argument was succesfully parsed</returns>
		static bool ParseArgument(string arg, out string name, out string val)
		{
			if (arg.StartsWith("-"))
			{
				string[] parts = arg.Substring(1).Split(new char[] { ':' });
				if (parts.Length > 1)
				{
					name = parts[0];
					val = parts[1];
					for (int i = 2; i < parts.Length; i++)
					{
						val += ":" + parts[i];
					}
					return true;
				}
			}
			name = null;
			val = null;
			return false;
		}

		/// <summary>
		/// Parses an array of command line arguments
		/// </summary>
		/// <param name="args">The command line arguments to parse</param>
		/// <returns>A <see cref="bool"/> indicating if the command line arguments were succesfully parsed</returns>
		static bool ParseCommandLineArguments(string[] args)
		{
			string name, val;
			foreach (string arg in args)
			{
				if (ParseArgument(arg, out name, out val))
				{
					switch (name.ToLower())
					{
						case "i":
							inputXuk = val;
							break;
						case "o":
							outputXuk = val;
							break;
						default:
							Console.WriteLine("Invalid argument {0}", arg);
							return false;
					}
				}
				else
				{
					Console.WriteLine("Invalid argument {0}", arg);
					return false;
				}
			}
			if (inputXuk == null || inputXuk == String.Empty)
			{
				Console.WriteLine("No input Xuk file was given");
				return false;
			}
			return true;
		}

		/// <summary>
		/// Application entry point
		/// </summary>
		/// <param name="args">The command line arguments</param>
		/// <returns>If the application runs succesfulle then <c>0</c> if returned else a non-zero error code is returned</returns>
		[STAThread]
		static int Main(string[] args)
		{
			Uri testUri = new Uri(@"C:\Documents and settings\oha\Dokumenter\Urakawa\XukProjects\myproj.xuk");
			Uri baseUri = new Uri(@"C:\Documents and settings\oha\Dokumenter\");
			Uri relUri = baseUri.MakeRelativeUri(testUri);
			if (!ParseCommandLineArguments(args))
			{
				Console.WriteLine(USAGE);
				return -1;
			}
			Project proj = new Project();
			Uri inputUri = new Uri(System.IO.Directory.GetCurrentDirectory() + "\\");
			inputUri = new Uri(inputUri, inputXuk);
			if (!proj.openXUK(inputUri))
			{
				Console.WriteLine("Could not open Xuk file {0}", inputXuk);
				return -1;
			}
			Console.WriteLine("Succesfully opened Xuk file {0}", inputXuk);
			if (outputXuk != null && outputXuk != String.Empty)
			{
				Uri outputUri = new Uri(System.IO.Directory.GetCurrentDirectory() + "\\");
				outputUri = new Uri(outputUri, outputXuk);
				if (!proj.saveXUK(outputUri))
				{
					Console.WriteLine("Could not save project to Xuk file {0}", outputXuk);
					return -1;
				}
				Console.WriteLine("Succesfully saved project to Xuk file", outputXuk);
			}
			return 0;
		}

	}
}
