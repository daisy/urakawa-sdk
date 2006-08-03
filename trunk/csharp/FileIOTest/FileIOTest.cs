using System;
using System.Diagnostics;

namespace urakawa.test
{
	/// <summary>
	/// FileIOTest is a console application which can read in and then output a XUK file
	/// </summary>
	public class FileIOTest
	{
		string mDefaultInFile = "../../../UnitTests/XukWorks/simplesample.xuk";
		string mDefaultOutFile = "../../../UnitTests/XukWorks/testOutput.xuk";

		public FileIOTest()
		{
		}

		static void Main(string[] args)
		{
			TextWriterTraceListener myWriter = new TextWriterTraceListener();
			myWriter.Writer = System.Console.Out;
			Trace.Listeners.Add(myWriter);
			Console.WriteLine("Starting Urakawa ConsoleTest appliation ... beep beep beep...");
			Console.WriteLine(
				"Current Directory:\n\t{0}\n",
				System.IO.Directory.GetCurrentDirectory());
			

			Console.WriteLine("Welcome to the Urakawa Toolkit.\n Please select an option:");
			Console.WriteLine("\t1. Read in a XUK file");
			Console.WriteLine("\t2. Output to a XUK file");
			Console.WriteLine("\t3. Do both in sequence");
        
			string strChoice = Console.ReadLine();

			int choice;

			if (strChoice != "")
				choice = int.Parse(strChoice);
			else
				choice = 1;

			FileIOTest tester = new FileIOTest();
      urakawa.project.Project project = new urakawa.project.Project();
			if (choice == 1)
			{
				tester.readXukFile(project);
			}
			else if (choice == 2)
			{
				tester.writeXukFile(project);
			}
			else if (choice == 3)
			{
				tester.readXukFile(project);
				tester.writeXukFile(project);
			}
		}

		private void readXukFile(urakawa.project.Project project)
		{
			Console.WriteLine("\nOpen a XUK file\n------------");
			Console.WriteLine
				("Please enter a filepath for the input file:");
			Console.WriteLine("\t or leave blank to use the default file {0}:", mDefaultInFile);

			string filepath = Console.ReadLine();
      if (filepath.StartsWith("\"") && filepath.EndsWith("\""))
      {
        filepath = filepath.Substring(1, filepath.Length-2);
      }
      Uri fileUri = null;

			//if no filepath was entered
			if (filepath == "") filepath = mDefaultInFile;
			filepath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(),
				filepath);

			fileUri = new Uri(filepath);

			
			if (project.openXUK(fileUri))
			{
				Console.WriteLine("Yay!  Successfully read the XUK file.\n\n");
			}
			else
			{
				Console.WriteLine("Oh NO!!  Failed to read the XUK file.\n\n");
			}

		}

		private void writeXukFile(urakawa.project.Project project)
		{
			Console.WriteLine("Enter a file name for saving the project:");
			Console.WriteLine("\t or leave blank to use the default file {0}:", 
				mDefaultOutFile);


			string filepath = Console.ReadLine();
      if (filepath.StartsWith("\"") && filepath.EndsWith("\""))
      {
        filepath = filepath.Substring(1, filepath.Length-2);
      }

			Uri fileUri = null;

			//if no filepath was entered
			if (filepath == "") filepath = mDefaultOutFile;
			filepath = System.IO.Path.Combine(
        System.IO.Directory.GetCurrentDirectory(),
				filepath);

			fileUri = new Uri(filepath);

			if (project.saveXUK(fileUri))
			{
				Console.WriteLine("Yay!  Successfully wrote the XUK file.\n\n");
			}
			else
			{
				Console.WriteLine("Oh NO!!  Failed to write the XUK file.\n\n");
			}
		}
	}
}
