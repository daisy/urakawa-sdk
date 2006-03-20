/*
UMLGraph Documentation:
http://www.spinellis.gr/sw/umlgraph/doc/indexw.html

JavaDoc command line:
javadoc -docletpath UMLGraph-4.3/lib/UmlGraph.jar -doclet gr.spinellis.umlgraph.doclet.UmlGraph -private -verbose -outputencoding UTF-8 *.java

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
class UMLOptions {}

/**
 * @view
 * @opt nodefillcolor LightGray
 */
abstract class ViewBase {}


/**
 * @view
 *
 * @match class exceptions.*
 * @opt nodefillcolor LightGray
 *
 * @match class mediaObject.*
 * @opt nodefillcolor LemonChiffon
 *
 * @match class coreDataModel.*
 * @opt nodefillcolor PaleGreen
 */
class ViewFullUML extends ViewBase {}


/**
 * @opt hide
 */
class unsigned_int {}

/**
 * @opt hide
 */
class unsigned_long {}

/**
 * @opt hide
 */
class string {}