This is the main directory for the toolkit. This file is a set of somewhat random notes taken by Jack while looking at this stuff, changed by Ole somewhat due to 

coreAndTests.sln is the main solution, containing:
* the main project core
* the unit test project tests
* the examples project, illustrating the extensibility features of the project

It needs VS 2003.

For unit testing, install NUnit, get it from <http://www.nunit.org/>.

For unit testing from within VS 2003, install TestDriven plug-in, from <http://www.testdriven.net/>.

You cannot run the main project of the solution, but you can run the tests in UnitTests with the "Run Tests" command in the contextual menu (if you installed the TestDriven plug-in). 

You can also inspect the tests by firing up the NUnit application and opening urakawa\tests\bin\Debug\urakawa.tests.dll.
