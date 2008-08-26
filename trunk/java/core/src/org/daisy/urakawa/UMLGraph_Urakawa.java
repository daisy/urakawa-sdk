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
 * 
 * @view
 * @opt nodefillcolor lavender
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
 * @opt edgefontsize 8
 * @opt edgefontcolor firebrick3
 * @opt edgecolor dimgray
 * @opt bgcolor white
 * @comment the JavaDoc tags below are escaped so we can leave them in for
 *          future reference.
 * @xopt inferdep
 * @xopt inferrel
 * @xopt nodefillcolor darkolivegreen1
 * @xopt nodefillcolor plum1
 * @xmatch class org.daisy.urakawa.media.[^.]+
 * @xmatch class org.daisy.urakawa.media.data.With.Manager
 * @xmatch class org.daisy.urakawa.event.ChangeEvent>
 * @xmatch class org.daisy.urakawa.event.IEventHandler<T extends Event>
 * @xmatch class org.daisy.urakawa.*\.I.*Manager
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
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @comment -------------------
 * @comment HIDE ALL
 * @match class org.daisy.urakawa.*
 * @opt hide
 * @comment -------------------
 * @comment UNHIDE SOME
 * @match class org.daisy.urakawa.Presentation
 * @opt !hide
 * @opt shape node
 * @match class org.daisy.urakawa.Project
 * @opt !hide
 * @opt shape activeclass
 * @match class org.daisy.urakawa.*Factory
 * @opt !hide
 * @opt operations
 * @match class org.daisy.urakawa.property.channel.Channel
 * @opt !hide
 * @match class org.daisy.urakawa.property.channel.AudioChannel
 * @opt !hide
 * @match class org.daisy.urakawa.property.channel.ManagedAudioChannel
 * @opt !hide
 * @match class org.daisy.urakawa.property.channel.TextChannel
 * @opt !hide
 * @match class org.daisy.urakawa.command.CompositeCommand
 * @opt !hide
 * @match class org.daisy.urakawa.media.data.FileDataProvider
 * @opt !hide
 * @match class org.daisy.urakawa.media.data.audio.codec.WavAudioMediaData
 * @opt !hide
 * @match class org.daisy.urakawa.media.ExternalAudioMedia
 * @opt !hide
 * @match class org.daisy.urakawa.metadata.Metadata
 * @opt !hide
 * @match class org.daisy.urakawa.property.Property
 * @opt !hide
 * @match class org.daisy.urakawa.property.xml.XmlProperty
 * @opt !hide
 * @match class org.daisy.urakawa.property.channel.ChannelsProperty
 * @opt !hide
 * @match class org.daisy.urakawa.core.TreeNode
 * @opt !hide
 * @comment -------------------
 * @comment COLORIZE FACTORY
 * @match class org.daisy.urakawa.*Factory
 * @opt nodefillcolor plum1
 * @comment COLORIZE MAIN
 * @match class org.daisy.urakawa.Presentation
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.Project
 * @opt nodefillcolor darkolivegreen1
 * @comment -------------------
 */
class UML_Project_Presentation_Factories extends UML_Defaults
{
    /**
     * Empty body.
     */
}

// --------------------------------------------------
/**
 * @view
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @comment -------------------
 * @comment HIDE ALL
 * @match class org.daisy.urakawa.*
 * @opt hide
 * @comment -------------------
 * @comment UNHIDE SOME
 * @match class org.daisy.urakawa.GenericFactory.*
 * @opt !hide
 * @opt shape activeclass
 * @opt operations
 * @match class org.daisy.urakawa.*Factory
 * @opt !hide
 * @opt operations
 * @comment -------------------
 * @comment COLORIZE FACTORY
 * @match class org.daisy.urakawa.*Factory
 * @opt nodefillcolor plum1
 * @comment COLORIZE MAIN
 * @match class org.daisy.urakawa.GenericFactory.*
 * @opt nodefillcolor darkolivegreen1
 * @comment -------------------
 */
class UML_Factory extends UML_Defaults
{
    /**
     * Empty body.
     */
}
