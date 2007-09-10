set conf=%1
if a%conf%==a set conf=debug
copy ..\UrakawaToolkit\bin\%conf%\urakawa.dll dll /y
copy ..\UrakawaToolkit\bin\%conf%\urakawa.xml dll /y
copy ..\UrakawaToolkitExamples\bin\%conf%\urakawa.examples.dll dll /y
copy ..\UrakawaToolkitExamples\bin\%conf%\urakawa.examples.xml dll /y
if exist bin.zip del bin.zip
"%programfiles%\WinZip\wzzip.exe" bin.zip dll\*.*
pause
