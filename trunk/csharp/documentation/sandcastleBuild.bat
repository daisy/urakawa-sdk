set pres=%2
if a%pres%==a set pres=vs2005
MRefBuilder "dll\*.dll" /out:reflection.org
if exist Output rmdir Output /s /q
mkdir Output
mkdir Output\html
mkdir Output\icons
mkdir Output\scripts
mkdir Output\styles
mkdir Output\media
if exist index.htm copy index.htm Output /y
if exist root.htm copy root.htm Output\html /y
copy "%DXROOT%\Presentation\%pres%\icons\*" Output\icons
copy "%DXROOT%\Presentation\%pres%\scripts\*" Output\scripts
copy "%DXROOT%\Presentation\%pres%\styles\*" Output\styles
rem XslTransform "/xsl:%DXROOT%\ProductionTransforms\AddOverloads.xsl" reflection.org "/xsl:%DXROOT%\ProductionTransforms\AddGuidFilenames.xsl" /out:reflection.xml
XslTransform "/xsl:%DXROOT%\ProductionTransforms\ApplyVSDocModel.xsl" reflection.org "/xsl:%DXROOT%\ProductionTransforms\AddGuidFilenames.xsl" /out:reflection.xml
XslTransform "/xsl:%DXROOT%\ProductionTransforms\ReflectionToManifest.xsl" reflection.xml /out:manifest.xml
if not exist Intellisense mkdir Intellisense
BuildAssembler "/config:%DXROOT%\Presentation\%pres%\Configuration\sandcastle.config" manifest.xml
XslTransform "/xsl:%DXROOT%\ProductionTransforms\ReflectionToChmProject.xsl" reflection.xml /arg:project=%1 /out:Output\%1.hhp
XslTransform "/xsl:%DXROOT%\ProductionTransforms\ReflectionToChmContents.xsl" reflection.xml /arg:html=Output\html /out:Output\%1.hhc
XslTransform "/xsl:%DXROOT%\ProductionTransforms\ReflectionToHtmContents.xsl" reflection.xml /arg:html=Output\html /arg:linktarget=main /out:Output\html\toc.htm
XslTransform "/xsl:%DXROOT%\ProductionTransforms\ReflectionToChmIndex.xsl" reflection.xml /out:Output\%1.hhk
XslTransform "/xsl:%DXROOT%\ProductionTransforms\ReflectionToHxSContents.xsl" reflection.xml /out:Output\%1.HxS
cd Output
"%programfiles%\Html Help Workshop\hhc.exe" %1.hhp
cd ..
if exist doc.zip del doc.zip
"%programfiles%\WinZip\wzzip.exe" doc.zip -p -r Output\*.*
