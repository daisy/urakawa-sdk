echo -----------------------------------

javadoc -docletpath UMLGraph-4.3/lib/UmlGraph.jar -doclet gr.spinellis.umlgraph.doclet.UmlGraph -private -verbose -outputencoding UTF-8 -sourcepath ./src/ -subpackages org.daisy.urakawa > log.txt

sed -f dot.sed ViewMedia.dot | cat > ViewMedia_.dot
sed -f dot.sed ViewCoreDataModel.dot | cat > ViewCoreDataModel_.dot
sed -f dot.sed ViewFullUML.dot | cat > ViewFullUML_.dot
sed -f dot.sed ViewExceptions.dot | cat > ViewExceptions_.dot

pause 

dot -v -Tpng -oViewExceptions.png ViewExceptions_.dot
dot -v -Tpng -oViewCoreDataModel.png ViewCoreDataModel_.dot
dot -v -Tpng -oViewMedia.png ViewMedia_.dot
dot -v -Tpng -oViewFullUML.png ViewFullUML_.dot
