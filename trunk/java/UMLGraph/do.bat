echo -----------------------------------

javadoc -docletpath UMLGraph-4.8/lib/UmlGraph.jar -doclet gr.spinellis.umlgraph.doclet.UmlGraph -private -verbose -outputencoding UTF-8 -sourcepath ./src/ -subpackages org.daisy.urakawa > log.txt

sed -f dot.sed ViewMedia.dot | cat > ViewMedia_.dot
sed -f dot.sed ViewStickyNote.dot | cat > ViewStickyNote_.dot
sed -f dot.sed ViewCoreDataModel.dot | cat > ViewCoreDataModel_.dot
sed -f dot.sed ViewFullUML.dot | cat > ViewFullUML_.dot
sed -f dot.sed ViewExceptions.dot | cat > ViewExceptions_.dot
sed -f dot.sed ViewVisitors.dot | cat > ViewVisitors_.dot
sed -f dot.sed ViewCommands.dot | cat > ViewCommands_.dot

pause 


REM Page A4: 8.3,11.7 MARGIN: 0.5

dot -v -Tpng -oViewVisitors_A4_Print.png ViewVisitors_.dot
dot -v -Tpng -Gsize=7.3,10.7! -oViewStickyNote_A4_Print.png ViewStickyNote_.dot
dot -v -Tpng -Gsize=7.3,10.7! -Grotate=90 -oViewExceptions_A4_Print.png ViewExceptions_.dot
dot -v -Tpng -Gsize=7.3,10.7! -Grotate=90 -oViewCoreDataModel_A4_Print.png ViewCoreDataModel_.dot
dot -v -Tpng -Gsize=7.3,10.7! -Grotate=90 -oViewMedia_A4_Print.png ViewMedia_.dot
dot -v -Tpng -Gsize=7.3,10.7! -Grotate=90 -oViewFullUML_A4_Print.png ViewFullUML_.dot
dot -v -Tpng -Gsize=7.3,10.7! -Grotate=90 -oViewCommands_A4_Print.png ViewCommands_.dot

dot -v -Tpng -oViewStickyNote.png ViewStickyNote_.dot
dot -v -Tpng -oViewExceptions.png ViewExceptions_.dot
dot -v -Tpng -oViewCoreDataModel.png ViewCoreDataModel_.dot
dot -v -Tpng -oViewMedia.png ViewMedia_.dot
dot -v -Tpng -oViewFullUML.png ViewFullUML_.dot
dot -v -Tpng -oViewVisitors.png ViewVisitors_.dot
dot -v -Tpng -oViewCommands.png ViewCommands_.dot
