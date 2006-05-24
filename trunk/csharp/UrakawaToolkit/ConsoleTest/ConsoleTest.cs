using System;

namespace urakawa.test
{
	/// <summary>
	/// Summary description for ConsoleTest.
	/// </summary>
	public class ConsoleTest
	{
		public ConsoleTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static void Main(string[] args)
		{
      Console.WriteLine(
        "Current Directory: {0}",
        System.IO.Directory.GetCurrentDirectory());

			urakawa.xuk.Project project = new urakawa.xuk.Project();

			Console.WriteLine
				("Welcome to the Urakawa Toolkit.\nPlease enter a filepath, or leave blank to use the default file:");

			string filepath = Console.ReadLine();
			Uri fileUri = null;

			//if no filepath was entered
			if (filepath == "")
			{
				//find the executable's directory
				filepath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location );
				fileUri = new Uri(filepath);
	
				fileUri = new Uri(fileUri, "../../../XukWorks/simplesample.xuk");
			}
			else
			{
        filepath = System.IO.Path.Combine(
          System.IO.Directory.GetCurrentDirectory(),
          filepath);

				fileUri = new Uri(filepath);
			}

			
			if (project.openXUK(fileUri))
			{
				Console.WriteLine("Yay!");
			}
			else
			{
				Console.WriteLine("Oh NO!!");
			}

		}
	}
}
