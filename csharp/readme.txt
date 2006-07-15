This is the main directory for the toolkit. This file
is a set of somewhat random notes taken by Jack while
looking at this stuff.

UrakawaToolkitAllProjects.sln is the main solution, containing:
* the main project UrakawaToolkit
* the unit test project UnitTests
* the DAISY Z39.86 to XUK converter project Z3986ToXUK

It needs VS 2003.

For unit testing, install NUnit, get it from <http://www.nunit.org/>.

For unit testing from within VS 2003, install TestDriven plug-in, from <http://www.testdriven.net/>.

You also need MSXML4. Google for it, download from MS.
Note that the SDK isn't installed by default, you should
select it manually when running the installer.

Build the solution. 

You cannot run the main project of the solution, but you can run the
tests in UnitTests with the "Run Tests" command in the
contextual menu. You can also inspect the tests by firing up
the NUnit application and opening
urakawa\UnitTests\bin\Debug\UnitTests.dll.
Additionally you can run the Z3986ToXUK tool to generate XUK files from Z3986 books.
Remark that the Z3986ToXUK tool does not utilize the urakawa core model.

