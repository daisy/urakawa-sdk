echo -----------------------------------

javadoc -docletpath UMLGraph-4.3/lib/UmlGraph.jar -doclet gr.spinellis.umlgraph.doclet.UmlGraph -private -verbose -outputencoding UTF-8 -sourcepath ./src/ -subpackages org.daisy.urakawa > log.txt

REM xxxx DIRTY HACK...but we have to, because UMLGraph does not generate the right color for all edges.
sed -e 's/Exceptions = /throws /g' -e 's/align="right"/align="center"/g' -e 's/}Notes}//g' -e 's/{Notes = {//g' -e 's/node \[/node \[color=\"Gray\",/g' -e 's/edge \[/edge \[color=\"Gray\",/g' ViewMedia.dot | cat > ViewMedia_.dot
sed -e 's/Exceptions = /throws /g' -e 's/align="right"/align="center"/g' -e 's/}Notes}//g' -e 's/{Notes = {//g' -e 's/node \[/node \[color=\"Gray\",/g' -e 's/edge \[/edge \[color=\"Gray\",/g' ViewCoreDataModel.dot | cat > ViewCoreDataModel_.dot
sed -e 's/Exceptions = /throws /g' -e 's/align="right"/align="center"/g' -e 's/}Notes}//g' -e 's/{Notes = {//g' -e 's/node \[/node \[color=\"Gray\",/g' -e 's/edge \[/edge \[color=\"Gray\",/g' ViewFullUML.dot | cat > ViewFullUML_.dot
sed -e 's/Exceptions = /throws /g' -e 's/align="right"/align="center"/g' -e 's/}Notes}//g' -e 's/{Notes = {//g' -e 's/node \[/node \[color=\"Gray\",/g' -e 's/edge \[/edge \[color=\"Gray\",/g' ViewExceptions.dot | cat > ViewExceptions_.dot

rem  -e 's/__/ /g'

pause 

rem dot -v -Tpng *.dot

dot -v -Tpng -oViewExceptions.png ViewExceptions_.dot
dot -v -Tpng -oViewCoreDataModel.png ViewCoreDataModel_.dot
dot -v -Tpng -oViewMedia.png ViewMedia_.dot
dot -v -Tpng -oViewFullUML.png ViewFullUML_.dot



rem dot -v -Tps -oViewExceptions.ps ViewExceptions.dot
rem dot -v -Tps -oViewCoreDataModel.ps ViewCoreDataModel.dot
rem dot -v -Tps -oViewMedia.ps ViewMedia.dot
rem dot -v -Tps -oViewFullUML.ps ViewFullUML.dot

rem dot -v -Tplain-ext -oViewExceptions.txt ViewExceptions.dot
rem dot -v -Tplain-ext -oViewCoreDataModel.txt ViewCoreDataModel.dot
rem dot -v -Tplain-ext -oViewMedia.txt ViewMedia.dot
rem dot -v -Tplain-ext -oViewFullUML.txt ViewFullUML.dot