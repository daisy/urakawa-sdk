echo -----------------------------------

javadoc -docletpath UMLGraph-4.3/lib/UmlGraph.jar -doclet gr.spinellis.umlgraph.doclet.UmlGraph -private -verbose -outputencoding UTF-8 -sourcepath ./src/ -subpackages org.daisy.urakawa *.java > log.txt

pause 

rem dot -v -Tpng *.dot

dot -v -Tpng -oViewExceptions.png ViewExceptions.dot
dot -v -Tpng -oViewCoreDataModel.png ViewCoreDataModel.dot
dot -v -Tpng -oViewMediaObject.png ViewMediaObject.dot
dot -v -Tpng -oViewFullUML.png ViewFullUML.dot

rem dot -v -Tps -oViewExceptions.ps ViewExceptions.dot
rem dot -v -Tps -oViewCoreDataModel.ps ViewCoreDataModel.dot
rem dot -v -Tps -oViewMediaObject.ps ViewMediaObject.dot
rem dot -v -Tps -oViewFullUML.ps ViewFullUML.dot

rem dot -v -Tplain-ext -oViewExceptions.txt ViewExceptions.dot
rem dot -v -Tplain-ext -oViewCoreDataModel.txt ViewCoreDataModel.dot
rem dot -v -Tplain-ext -oViewMediaObject.txt ViewMediaObject.dot
rem dot -v -Tplain-ext -oViewFullUML.txt ViewFullUML.dot