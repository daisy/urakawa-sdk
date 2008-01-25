package org.daisy.urakawa;


/*
VERY IMPORTANT:
Search for "@tagvalue Exceptions[^ ]" with Regular Expressions: if any, must remove the line break ! 

GraphViz DOT:
http://www.ryandesign.com/graphviz/

DOT HTML format:
http://www.graphviz.org/doc/info/shapes.html#html

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
 * @opt !horizontal
 * @hidden
 */
class UMLOptions {
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
 * @xopt inferdep
 * @xopt inferrel
 * @match class org.daisy.urakawa.*Impl
 * @opt nodefontcolor black
 */
abstract class ViewBase {
}


/**
 * @view
 * @opt hide
 * @comment Un-hiding the presentation and factories (one by one):
 * @match class org.daisy.urakawa.Presentation
 * @opt !hide
 * @match class org.daisy.urakawa.media.data.MediaDataFactory
 * @opt !hide
 * @match class org.daisy.urakawa.media.data.DataProviderFactory
 * @opt !hide
 * @match class org.daisy.urakawa.media.data.FileDataProviderFactory
 * @opt !hide
 * @match class org.daisy.urakawa.property.xml.XmlPropertyFactory
 * @opt !hide
 * @match class org.daisy.urakawa.property.channel.ChannelFactory
 * @opt !hide
 * @match class org.daisy.urakawa.property.channel.ChannelsPropertyFactory
 * @opt !hide
 * @match class org.daisy.urakawa.property.GenericPropertyFactory
 * @opt !hide
 * @match class org.daisy.urakawa.property.PropertyFactory
 * @opt !hide
 * @match class org.daisy.urakawa.core.TreeNodeFactory
 * @opt !hide
 * @match class org.daisy.urakawa.media.MediaFactory
 * @opt !hide
 * @match class org.daisy.urakawa.undo.CommandFactory
 * @opt !hide
 * @match class org.daisy.urakawa.metadata.MetadataFactory
 * @opt !hide
 * @comment Setting the special colors:
 * @match class org.daisy.urakawa.Presentation
 * @opt nodefillcolor darkolivegreen1
 */
class UML_PresentationAndFactories extends ViewBase {
}

/**
 * @view
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 */
class UML_PresentationAndFactories_Minimal extends UML_PresentationAndFactories {
}
