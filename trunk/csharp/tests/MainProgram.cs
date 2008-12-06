using System;
using System.Reflection;
using System.Windows.Forms;

/**
 * 
 * modify the .csproj.user file to include this snippet
 * 
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
<PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
<StartAction>Program</StartAction>
<StartProgram>C:\Program Files\NUnit-Net-2.0 2.2.8\bin\nunit-gui.exe</StartProgram>
<StartWorkingDirectory>C:\Programme\Microsoft\Image Cup 2006\</StartWorkingDirectory>
</PropertyGroup>
</Project>
under the PropertyGroup element. 
 */
namespace urakawa
{
    /**
     * http://www.hanselman.com/blog/GivingAMappedNetworkDriveFullTrustWithNETCodeAccessSecurity.aspx
     * http://www.sellsbrothers.com/news/showTopic.aspx?ixTopic=1519
     * c:\>caspol -q -machine -addgroup 1 -url file://z:/* FullTrust -name "Z Drive" 
     **/
    class MainProgram
    {
        [STAThread]
        static void Main(string[] args)
        {
            //string[] argz = new string[1] { "../../UrakawaTests.nunit" };
            //NUnit.ConsoleRunner.Runner.Main(new string[] { Assembly.GetExecutingAssembly().Location });
            DialogResult dialogResult = MessageBox.Show("Use GUI ?", "NUnit tests", MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Question);
            if (dialogResult.Equals(DialogResult.Yes))
            {
                NUnit.Gui.AppEntry.Main(args);
            }
            else
            {
                NUnit.ConsoleRunner.Runner.Main(args);
            }

            /*
            AppDomain.CurrentDomain.ExecuteAssembly(
        @"C:\Program Files\NUnit\bin\NUnit-console.exe",
        null,
        new string[] { Assembly.GetExecutingAssembly().Location });
             * */





            /* 
             * 
            // Create a new AppDomain PolicyLevel.

            PolicyLevel domainPolicy = PolicyLevel.CreateAppDomainLevel();



            // Create a 'Membership Condition' to be assigned to a new code group

            AllMembershipCondition allCodeMC = new AllMembershipCondition();



            // Create a new permission set with the same permissions as the

            // "LocalIntranet" permission set.

            PermissionSet CustomPS = new PermissionSet(domainPolicy.GetNamedPermissionSet("LocalIntranet"));



            // Add the permission needed to read from the registry.

            CustomPS.AddPermission(new RegistryPermission(PermissionState.Unrestricted));



            PolicyStatement polState = new PolicyStatement(CustomPS);



            //Create a new code group which will serve as the root code group for the

            //AppDomain policy level

            CodeGroup allCodeCG = new UnionCodeGroup(allCodeMC, polState);

            domainPolicy.RootCodeGroup = allCodeCG;



            // Create a new application domain.

            AppDomain domain = System.AppDomain.CreateDomain("CustomDomain");

            domain.SetAppDomainPolicy(domainPolicy);



            // Load and execute the assembly.

            try
            {

                string FQ_UNC_Path = @"\\MyServer\MyShare\ConsoleReadRegistry.exe";

                domain.ExecuteAssembly(FQ_UNC_Path);

            }

            catch (PolicyException e)
            {

                Console.WriteLine("PolicyException: {0}", e.Message);

            }

            catch (Exception e)
            {

                Console.WriteLine("Unexpected Exception: {0}", e.Message);

            }



            AppDomain.Unload(domain);
            */
        }
    }
}
