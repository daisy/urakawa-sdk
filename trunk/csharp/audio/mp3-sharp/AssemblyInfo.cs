using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("Mp3Sharp")]
[assembly: AssemblyDescription("Mp3 Decoder for the .NET Framework")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Robert Burke (rob@mle.ie)")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("100% Freeware")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion("1.4.0.0")]


[assembly: ComVisibleAttribute(false)]

#if !NET_3_5 // NET_4_0 || BOOTSTRAP_NET_4_0
[assembly: System.Security.SecurityRules(System.Security.SecurityRuleSet.Level1)]
#endif