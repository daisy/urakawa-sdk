#!/bin/sh

# http://www.webpagepublicity.com/free-fonts-a4.html
# arial.ttf arialbd.ttf arialbi.ttf
#sudo cp /Users/danielweck/Desktop/*.ttf /usr/X11R6/lib/X11/fonts/TTF/

javadoc -docletpath UMLGraph-4.7/lib/UmlGraph.jar -doclet gr.spinellis.umlgraph.doclet.UmlGraph -private -verbose -outputencoding UTF-8 -sourcepath ./src/ -subpackages org.daisy.urakawa > log.txt

sed -f dot.sed ViewMedia.dot | cat > ViewMedia_.dot
sed -f dot.sed ViewStickyNote.dot | cat > ViewStickyNote_.dot
sed -f dot.sed ViewCoreDataModel.dot | cat > ViewCoreDataModel_.dot
sed -f dot.sed ViewFullUML.dot | cat > ViewFullUML_.dot
sed -f dot.sed ViewExceptions.dot | cat > ViewExceptions_.dot
sed -f dot.sed ViewVisitors.dot | cat > ViewVisitors_.dot
sed -f dot.sed ViewCommands.dot | cat > ViewCommands_.dot

# Page A4: 8.3,11.7 MARGIN: 0.5

PARAMS=" -v -Tpng"
EXT="png"
PAGESTR="_A4_Print"
ROTATE=" -Grotate=90 "
SIZE=" -Gsize=7.3,10.7! "

dot $PARAMS -oViewVisitors$PAGESTR.$EXT ViewVisitors_.dot
dot $PARAMS $SIZE -oViewStickyNote$PAGESTR.$EXT ViewStickyNote_.dot
dot $PARAMS $SIZE $ROTATE -oViewFullUML$PAGESTR.$EXT ViewFullUML_.dot
dot $PARAMS $SIZE $ROTATE -oViewCoreDataModel$PAGESTR.$EXT ViewCoreDataModel_.dot
dot $PARAMS $SIZE $ROTATE -oViewExceptions$PAGESTR.$EXT ViewExceptions_.dot
dot $PARAMS $SIZE $ROTATE -oViewMedia$PAGESTR.$EXT ViewMedia_.dot
dot $PARAMS $SIZE $ROTATE -oViewCommands$PAGESTR.$EXT ViewCommands_.dot

dot $PARAMS -oViewVisitors.$EXT ViewVisitors_.dot
dot $PARAMS -oViewStickyNote.$EXT ViewStickyNote_.dot
dot $PARAMS -oViewFullUML.$EXT ViewFullUML_.dot
dot $PARAMS -oViewCoreDataModel.$EXT ViewCoreDataModel_.dot
dot $PARAMS -oViewExceptions.$EXT ViewExceptions_.dot
dot $PARAMS -oViewMedia.$EXT ViewMedia_.dot
dot $PARAMS -oViewCommands.$EXT ViewCommands_.dot
