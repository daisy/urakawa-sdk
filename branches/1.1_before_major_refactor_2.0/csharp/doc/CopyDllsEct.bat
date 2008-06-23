set conf=%1
if a%conf%==a set conf=debug
copy ..\core\bin\%conf%\urakawa.dll dll /y
copy ..\core\bin\%conf%\urakawa.xml dll /y
copy ..\examples\bin\%conf%\urakawa.examples.dll dll /y
copy ..\examples\bin\%conf%\urakawa.examples.xml dll /y
if exist bin.zip del bin.zip
"%programfiles%\WinZip\wzzip.exe" bin.zip dll\*.*
pause
