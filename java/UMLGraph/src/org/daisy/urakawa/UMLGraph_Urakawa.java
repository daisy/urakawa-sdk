package org.daisy.urakawa;

/*
UMLGraph Documentation:
http://www.spinellis.gr/sw/umlgraph/doc/indexw.html

JavaDoc command line:
javadoc -docletpath UMLGraph-4.3/lib/UmlGraph.jar -doclet gr.spinellis.umlgraph.doclet.UmlGraph -private -verbose -outputencoding UTF-8 -sourcepath ./src/ -subpackages org.daisy.urakawa *.java

DOT command line:
dot -Tps -oUrakawa_Graph.ps Urakawa_Graph.dot

SED command line for MacOSX (to use for the MacOSX GraphViz viewer):
(@opt noguillemot does not work either)
sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' Urakawa_Graph.dot | cat > Urakawa_Graph_.dot
*/

/**
 * @opt verbose2
 * @opt compact
 * @opt attributes
 * @opt operations
 * @opt enumerations
 * @opt enumconstants
 * @opt views
 * @opt types
 * @opt hide java.*
 * @opt inferrel
 * @opt inferdep
 * @opt collpackages java.util.*
 * @opt visibility
 * @opt vertical
 * @hidden
 */
class UMLOptions {
}

/**
 * @view
 * @opt nodefillcolor LightGray
 * @opt nodefontcolor Black
 * @opt nodefontname arial
 * @opt nodefontabstractname arial
 * @opt nodefontsize 10
 *
 * @opt nodefontclassname arialbd
 * @opt nodefontclassabstractname arialbi
 * @opt nodefontclasssize 10
 *
 * @opt nodefonttagname arial
 * @opt nodefonttagsize 8
 *
 * @opt nodefontpackagename arial
 * @opt nodefontpackagesize 8
 *
 * @opt edgefontname arialbd
 * @opt edgefontsize 10
 * @opt edgefontcolor Blue
 * @opt edgecolor Gray
 *
 * @opt bgcolor white
 */
abstract class ViewBase {
}

/**
 * @view
 * @match class org.daisy.urakawa.exceptions.*
 * @opt nodefillcolor LightGray
 * @match class org.daisy.urakawa.media.*
 * @opt nodefillcolor LemonChiffon
 * @match class org.daisy.urakawa.coreDataModel.*
 * @opt nodefillcolor PaleGreen
 */
class ViewFullUML extends ViewBase {
}

/**
 * @view
 * @opt nodefillcolor LightGray
 * @opt hide
 * @match class org.daisy.urakawa.exceptions.*
 * @opt !hide
 */
class ViewExceptions extends ViewBase {
}

/**
 * @view
 * @opt nodefillcolor PaleGreen
 * @opt hide
 * @match class org.daisy.urakawa.coreDataModel.*
 * @opt !hide
 */
class ViewCoreDataModel extends ViewBase {
}

/**
 * @view
 * @opt nodefillcolor LemonChiffon
 * @opt hide
 * @match class org.daisy.urakawa.media.*
 * @opt !hide
 */
class ViewMedia extends ViewBase {
}
