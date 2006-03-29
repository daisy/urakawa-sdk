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
 * For examples of valid named colors:
 * http://www.w3schools.com/html/html_colornames.asp
 * http://www.webdevelopersnotes.com/design/list_of_HTML_color_names.php3
 * http://www.scriptingmaster.com/html/HTML-extended-color-names.asp
 * 
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
 * @match class org.daisy.urakawa.ClassDiagramNotes_Media
 * @opt !hide
 * @opt nodefillcolor Yellow
 * @opt nodefonttagname arial
 * @opt nodefonttagsize 10
 * @match class org.daisy.urakawa.IdentifiableInterface
 * @opt !hide
 * @opt nodefillcolor LightBlue
 * @match class org.daisy.urakawa.InterfaceID
 * @opt !hide
 * @opt nodefillcolor LightBlue
 */
class ViewMedia extends ViewBase {
}

/**
 * @tagvalue Notes "{-__The__blue__annotations__on__the__Dependency__arrows__(Name__and__Multiplicity)<br/>provide__additional__specification.__For__example,__they__can__represent__Associations<br/>with__the__specified__Navigability__and__Multiplicity,__and__the__given__Role-Name__is__either<br/>'Aggregation'__or__'Composition'.__When__Multiplicity__is__indicated__on__the__arrow<br/>start__side,__the__uni-directional__Navigability__becomes__bi-directional.<br/>Another__case__is__when__using__the__'Create'__Role-Name:__this__provides<br/>additional__information__as__to__what__Instance__types__the__Entity__can__create.<br/>This__representation__system__is__not__UML-standard,__and__has__been__introduced<br/>in__this__design__representation__in__order__to__address__the__shortcomings__of<br/>Interfaces,__in__praticular__the__inability__to__have__outward__Associations.<br/>Abstract__classes__would__be__more__structurally__expressive__than__Interfaces,<br/>and__therefore__would__have__not__required__such__workaround.__But__to__avoid<br/>clutter__of__the__UML__diagram__we__only__show__the__corresponding__Interfaces,<br/>thus__requiring__this__sort__of__extra__information.<br/>__<br/>-__The__Entities__with__a__blue-ish__background__color__are__not__specific__to__this__Class__Diagram<br/>and__may__be__used__in__other__Class__Diagrams.__This__is__why__they__are__marked__as__such.<br/>__<br/>-__Some__Operations__may__have__an__'Exception__=__'__annotation.__This__is__used__to<br/>show__the__full__method__signature__including__thrown__Exceptions__when__appropriate.<br/>These__Exceptions__are__mostly__used__for__assertion__and__they__should__be__implemented<br/>and__raised__according__to__the__full__specification__available__in__the__design__comments<br/>(not__shown__in__the__Class__Diagram)}Notes"
 */
class ClassDiagramNotes_Media {

/**
- The blue annotations on the Dependency arrows (Name and Multiplicity)
provide additional specification. For example, they can represent Associations
with the specified Navigability and Multiplicity, and the given Role-Name is either
'Aggregation' or 'Composition'. When Multiplicity is indicated on the arrow
start side, the uni-directional Navigability becomes bi-directional.
Another case is when using the 'Create' Role-Name: this provides
additional information as to what Instance types the Entity can create.
This representation system is not UML-standard, and has been introduced
in this design representation in order to address the shortcomings of
Interfaces, in praticular the inability to have outward Associations.
Abstract classes would be more structurally expressive than Interfaces,
and therefore would have not required such workaround. But to avoid
clutter of the UML diagram we only show the corresponding Interfaces,
thus requiring this sort of extra information.

- The Entities with a blue-ish background color are not specific to this Class Diagram
and may be used in other Class Diagrams. This is why they are marked as such.

- Some Operations may have an 'Exception = ' annotation. This is used to
show the full method signature including thrown Exceptions when appropriate.
These Exceptions are mostly used for assertion and they should be implemented
and raised according to the full specification available in the design comments
(not shown in the Class Diagram)
*/
}