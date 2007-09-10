The following is a description of how to build the documentation for the urakawa SDK:

Prerequisites
* Install Microsoft Sandcastle version 2.0.2459.30850
* Add the html Presentation to sandcastle:
  - Unzip the file PresentationHtml.zip to the sandcastle progran directory (normally ("%ProgramFiles%\Sandcastle")

Build process (If you want to build documentation for the release configuration, replace debug with release in the following):
* Build the debug configuration of the UrakawaToolkitAllProjects solution
* Open a command window and navigate to the implementation\documentation directory
* Run CopyDllsEct.bat debug
* Run sandcastleBuild urakawa html > sandcastleBuild.log

This will create the documentation in directory implementation\documentation\Output (open index.htm). A zipped version will be in file implementation\documentation\doc.zip and corresponding dlls will be in file implementation\documentation\dll.zip

/OHA 2007-09-10