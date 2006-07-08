#!/bin/sh

# http://www.webpagepublicity.com/free-fonts-a4.html
# arial.ttf arialbd.ttf arialbi.ttf
#sudo cp /Users/danielweck/Desktop/*.ttf /usr/X11R6/lib/X11/fonts/TTF/

javadoc -docletpath UMLGraph-4.3/lib/UmlGraph.jar -doclet gr.spinellis.umlgraph.doclet.UmlGraph -private -verbose -outputencoding UTF-8 -sourcepath ./src/ -subpackages org.daisy.urakawa > log.txt

sed -f dot.sed ViewMedia.dot | cat > ViewMedia_.dot
sed -f dot.sed ViewStickyNote.dot | cat > ViewStickyNote_.dot
sed -f dot.sed ViewCoreDataModel.dot | cat > ViewCoreDataModel_.dot
sed -f dot.sed ViewFullUML.dot | cat > ViewFullUML_.dot
sed -f dot.sed ViewExceptions.dot | cat > ViewExceptions_.dot
sed -f dot.sed ViewVisitors.dot | cat > ViewVisitors_.dot
sed -f dot.sed ViewCommands.dot | cat > ViewCommands_.dot

sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' ViewStickyNote_.dot | cat > ViewStickyNote__.dot
sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' ViewCoreDataModel_.dot | cat > ViewCoreDataModel__.dot
sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' ViewExceptions_.dot | cat > ViewExceptions__.dot
sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' ViewMedia_.dot | cat > ViewMedia__.dot
sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' ViewFullUML_.dot | cat > ViewFullUML__.dot
sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' ViewCommands_.dot | cat > ViewCommands__.dot
sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' ViewVisitors_.dot | cat > ViewVisitors__.dot

# Page A4: 8.3,11.7 MARGIN: 0.5

PARAMS=" -v -Tpng"
EXT="png"
PAGESTR="_A4_Print"
ROTATE=" -Grotate=90 "
SIZE=" -Gsize=7.3,10.7! "

dot $PARAMS -oViewVisitors$PAGESTR.$EXT ViewVisitors__.dot
dot $PARAMS $SIZE -oViewStickyNote$PAGESTR.$EXT ViewStickyNote__.dot
dot $PARAMS $SIZE $ROTATE -oViewFullUML$PAGESTR.$EXT ViewFullUML__.dot
dot $PARAMS $SIZE $ROTATE -oViewCoreDataModel$PAGESTR.$EXT ViewCoreDataModel__.dot
dot $PARAMS $SIZE $ROTATE -oViewExceptions$PAGESTR.$EXT ViewExceptions__.dot
dot $PARAMS $SIZE $ROTATE -oViewMedia$PAGESTR.$EXT ViewMedia__.dot
dot $PARAMS $SIZE $ROTATE -oViewCommands$PAGESTR.$EXT ViewCommands__.dot

dot $PARAMS -oViewVisitors.$EXT ViewVisitors__.dot
dot $PARAMS -oViewStickyNote.$EXT ViewStickyNote__.dot
dot $PARAMS -oViewFullUML.$EXT ViewFullUML__.dot
dot $PARAMS -oViewCoreDataModel.$EXT ViewCoreDataModel__.dot
dot $PARAMS -oViewExceptions.$EXT ViewExceptions__.dot
dot $PARAMS -oViewMedia.$EXT ViewMedia__.dot
dot $PARAMS -oViewCommands.$EXT ViewCommands__.dot
