using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("NAudio")]
[assembly: AssemblyDescription("NAudio .NET Audio Library")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Mark Heath")]
[assembly: AssemblyProduct("NAudio")]
[assembly: AssemblyCopyright("© 2001-2008 Mark Heath")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.3.5.0")]
[assembly: AssemblyFileVersion("1.3.5.0")]

[assembly: ComVisibleAttribute(false)]

#if !NET_3_5 // NET_4_0 || BOOTSTRAP_NET_4_0
[assembly: System.Security.SecurityRules(System.Security.SecurityRuleSet.Level1)]
#endif