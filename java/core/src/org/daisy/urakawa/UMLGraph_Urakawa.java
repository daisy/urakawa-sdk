package org.daisy.urakawa;


/**
 * VERY IMPORTANT: Search for " Expressions: if any, must remove the line break
 * ! GraphViz DOT: http://www.ryandesign.com/graphviz/ DOT HTML format:
 * http://www.graphviz.org/doc/info/shapes.html#html UMLGraph Documentation:
 * http://www.spinellis.gr/sw/umlgraph/doc/indexw.html JavaDoc command line:
 * javadoc -docletpath UMLGraph-4.3/lib/UmlGraph.jar -doclet
 * gr.spinellis.umlgraph.doclet.UmlGraph -private -verbose -outputencoding UTF-8
 * -sourcepath ./src/ -subpackages org.daisy.urakawa.java DOT command line: dot
 * -Tps -oUrakawa_Graph.ps Urakawa_Graph.dot SED command line for MacOSX (to use
 * for the MacOSX GraphViz viewer): (@opt noguillemot does not work either) sed
 * -e 's/laquo/lt/g' -e 's/raquo/gt/g' Urakawa_Graph.dot | cat >
 * Urakawa_Graph_.dot
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
 * @opt hide java.
 * @opt collpackages java.util.
 * @opt visibility
 * @opt vertical
 * @opt !horizontal
 * @hidden
 */
class UMLOptions
{
    /**
     * Empty body.
     */
}

/**
 * For examples of valid named colors:
 * http://www.graphviz.org/doc/info/colors.html
 * http://www.w3schools.com/html/html_colornames.asp
 * http://www.webdevelopersnotes.com/design/list_of_HTML_color_names.php3
 * http://www.scriptingmaster.com/html/HTML-extended-color-names.asp
 * @view
 * @opt nodefillcolor lavender
 * @opt nodefontcolor Black
 * @opt nodefontname Helvetica
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
 * @opt edgefontcolor firebrick3
 * @opt edgecolor dimgray
 * @opt bgcolor white
 * @opt !attributes
 * @comment the JavaDoc tags below are escaped so we can leave them in for
 * future reference.
 * @xopt inferdep
 * @xopt inferrel
 * @xopt nodefillcolor darkolivegreen1
 * @xopt nodefillcolor plum1
 * @xmatch class org.daisy.urakawa.
 * @xmatch class org.daisy.urakawa.WHATEVER
 * @xmatch class org.daisy.urakawa.media.[^.]+
 * @xmatch class org.daisy.urakawa.media.data.With.Manager
 * @xmatch class org.daisy.urakawa.event.ChangeEvent>
 * @xmatch class org.daisy.urakawa.event.IEventHandler<T extends Event>
 * @xopt !hide
 * @xopt !operations
 * @xopt !constructors
 * @xopt !attributes
 */
abstract class UML_Defaults
{
    /**
     * Empty body.
     */
}

// --------------------------------------------------
/**
 * @view
 * @comment HIDE ALL
 * @match class org.daisy.urakawa.*
 * @opt hide
 * @comment UNHIDE PACKAGE
 * @match class org.daisy.urakawa.[^.]+
 * @opt !hide
 * @comment UNHIDE OUT-PACKAGE
 * @match class org.daisy.urakawa.metadata.IWithMetadata
 * @opt !hide
 * @match class org.daisy.urakawa.xuk.IXukAble
 * @opt !hide
 * @match class org.daisy.urakawa.media.IMediaPresentation
 * @opt !hide
 * @match class org.daisy.urakawa.events.IEventHandler.*
 * @opt !hide
 * @comment HIDE SPECIFICS
 * @match class org.daisy.urakawa.IValueEquatable<T>
 * @opt hide
 * @match class org.daisy.urakawa.*Exception
 * @opt hide
 * @match class org.daisy.urakawa.DesignConvenienceInterface
 * @opt hide
 * @match class org.daisy.urakawa.IWithPresentation
 * @opt hide
 * @match class org.daisy.urakawa.LeafInterface
 * @opt hide
 * @match class org.daisy.urakawa.AbstractXukAbleWithPresentation
 * @opt hide
 * @comment COLORIZE FACTORY
 * @match class org.daisy.urakawa.*Factory
 * @opt nodefillcolor plum1
 * @comment COLORIZE MAIN
 * @match class org.daisy.urakawa.Presentation
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.Project
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.xuk.IXukAble
 * @opt !operations !constructors !attributes
 */
class UML_Base_Full extends UML_Defaults
{
    /**
     * Empty body.
     */
}
/**
 * @view
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @comment HIDE SPECIFICS
 * @match class org.daisy.urakawa.IWithProject
 * @opt hide
 * @match class org.daisy.urakawa.IWithPresentations
 * @opt hide
 */
class UML_Base_Summary extends UML_Base_Full
{
    /**
     * Empty body.
     */
}