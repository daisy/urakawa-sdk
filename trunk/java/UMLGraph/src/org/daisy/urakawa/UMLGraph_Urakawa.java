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
 * @opt nodefontclassname arialbd
 * @opt nodefontclassabstractname arialbi
 * @opt nodefontclasssize 10
 * @opt nodefonttagname arial
 * @opt nodefonttagsize 8
 * @opt nodefontpackagename arial
 * @opt nodefontpackagesize 8
 * @opt edgefontname arialbd
 * @opt edgefontsize 10
 * @opt edgefontcolor Blue
 * @opt edgecolor Gray
 * @opt bgcolor white
 * @match class org.daisy.urakawa.exceptions.*
 * @opt nodefillcolor LightGray
 * @match class org.daisy.urakawa.media.*
 * @opt nodefillcolor LemonChiffon
 * @match class org.daisy.urakawa.coreDataModel.*
 * @opt nodefillcolor PaleGreen
 * @match class org.daisy.urakawa.*.*Impl
 * @opt nodefontcolor Red
 * @match class org.daisy.urakawa.IdentifiableInterface
 * @opt !hide
 * @opt nodefillcolor LightBlue
 * @match class org.daisy.urakawa.InterfaceID
 * @opt !hide
 * @opt nodefillcolor LightBlue
 * @match class org.daisy.urakawa.media.Time
 * @opt nodefillcolor GreenYellow
 * @match class org.daisy.urakawa.media.TimeDelta
 * @opt nodefillcolor GreenYellow
 * @match class org.daisy.urakawa.media.MediaLocation
 * @opt nodefillcolor GreenYellow
 */
abstract class ViewBase {
}

/**
 * @view
 * @match class org.daisy.urakawa.impl.*
 * @opt hide
 */
class ViewFullUML extends ViewBase {
}

/**
 * @view
 * @opt hide
 * @match class org.daisy.urakawa.exceptions.*
 * @opt !hide
 * @match class org.daisy.urakawa.IdentifiableInterface
 * @opt hide
 * @match class org.daisy.urakawa.InterfaceID
 * @opt hide
 */
class ViewExceptions extends ViewBase {
}

/**
 * @view
 * @opt inferdep
 * @opt inferrel
 * @opt hide
 * @match class org.daisy.urakawa.coreDataModel.*
 * @opt !hide
 * @match class org.daisy.urakawa.Notes
 * @opt !hide
 */
class ViewCoreDataModel extends ViewBase {
}

/**
 * @view
 * @opt hide
 * @match class org.daisy.urakawa.media.*
 * @opt !hide
 * @match class org.daisy.urakawa.Notes
 * @opt !hide
 */
class ViewMedia extends ViewBase {
}

/**
 * @tagvalue Notes "{###  The blue annotations on the Dependency arrows (Name and Multiplicity)<br/>provide additional specification. For example, they can represent Associations<br/>with the specified Navigability and Multiplicity, and the given Role-Name is either<br/>'Aggregation' or 'Composition'. When Multiplicity is indicated on the arrow<br/>start side, the uni-directional Navigability becomes bi-directional.<br/>Another case is when using the 'Create' Role-Name: this provides<br/>additional information as to what Instance types the Entity can create.<br/>This representation system is not UML-standard, and has been introduced<br/>in this design representation in order to address the shortcomings of<br/>Interfaces, in praticular the inability to have outward Associations.<br/>Abstract classes would be more structurally expressive than Interfaces,<br/>and therefore would have not required such workaround. But to avoid<br/>clutter of the UML diagram we only show the corresponding Interfaces,<br/>thus requiring this sort of extra information.<br/> <br/>}Notes"
 * @tagvalue Notes "{###  The Entities with a blue-ish background color are not specific to this Class Diagram<br/>and may be used in other Class Diagrams. This is why they are marked as such.<br/> <br/>}Notes"
 * @tagvalue Notes "{###  The Entities with a green-ish background color are 'Interface Lollipops':<br/> they refer to another part of the Model outside of this Class Diagram.<br/> The description of this Interface (Operations) is therefore ommited.<br/> <br/>}Notes"
 * @tagvalue Notes "{###  The Class names in red are just for highlighting purposes,<br/>for a reader to visually find implementations in the Diagram.<br/> <br/>}Notes"
 * @tagvalue Notes "{###  Some Operations may have an 'Exception = ' annotation. This is used to<br/>show the full method signature including thrown Exceptions when appropriate.<br/>These Exceptions are mostly used for assertion and they should be implemented<br/>and raised according to the full specification available in the design comments<br/>(not shown in the Class Diagram)<br/> <br/>}Notes"
 * 
 * @opt nodefillcolor Yellow
 * @opt nodefonttagname arial
 * @opt nodefonttagsize 10
 */
class Notes {
/**
 ### The blue annotations on the Dependency arrows (Name and Multiplicity)
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

 ### The Entities with a blue-ish background color are not specific to this Class Diagram
 and may be used in other Class Diagrams. This is why they are marked as such.

 ### The Entities with a green-ish background color are "Interface Lollipops":
 they refer to another part of the Model outside of this Class Diagram.
 The description of this Interface (Operations) is therefore ommited.
 
 ### Class names in red are just for highlighting purposes,
 for a reader to visually find implementations in the Diagram.

 ### Some Operations may have an 'Exception = ' annotation. This is used to
 show the full method signature including thrown Exceptions when appropriate.
 These Exceptions are mostly used for assertion and they should be implemented
 and raised according to the full specification available in the design comments
 (not shown in the Class Diagram)
 */
}