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
 * @comment Un-hiding the whole channel package:
 * @match class org.daisy.urakawa.property.channel.*
 * @opt !hide
 * @opt nodefillcolor darkolivegreen1
 * @comment Hiding the factories, exceptions and implementations:
 * @match class org.daisy.urakawa.*XukAble
 * @opt hide
 * @match class org.daisy.urakawa.*Exception
 * @opt hide
 * @match class org.daisy.urakawa.*Factory
 * @opt hide
 * @match class org.daisy.urakawa.*Impl
 * @opt hide
 * @match class org.daisy.urakawa.*Visitor
 * @opt hide
 * @comment Hiding specific With* entities:
 * @match class org.daisy.urakawa.property.channel.WithMedia
 * @opt hide
 * @match class org.daisy.urakawa.property.channel.WithChannelsManager
 * @opt hide
 * @match class org.daisy.urakawa.property.channel.WithChannelFactory
 * @opt hide
 * @match class org.daisy.urakawa.property.channel.WithChannelsPropertyFactory
 * @opt hide
 * @match class org.daisy.urakawa.property.channel.AudioChannel
 * @opt hide
 * @match class org.daisy.urakawa.property.channel.ManagedAudioChannel
 * @opt hide
 * @match class org.daisy.urakawa.property.channel.TextChannel
 * @opt hide
 * @comment Un-hiding external entities:
 * @match class org.daisy.urakawa.property.Property
 * @opt !hide
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @match class org.daisy.urakawa.media.Media
 * @opt !hide
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 */
class UML_ChannelsProperty extends ViewBase {
}

/**
 * @view
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @match class org.daisy.urakawa.property.channel.WithName
 * @opt hide
 */
class UML_ChannelsProperty_Minimal extends UML_ChannelsProperty {
}

/**
 * @view
 * @match class org.daisy.urakawa.property.channel.ChannelFactory
 * @opt !hide
 * @match class org.daisy.urakawa.property.channel.ChannelsPropertyFactory
 * @opt !hide
 */
class UML_ChannelsPropertyAndFactories_Minimal extends
		UML_ChannelsProperty_Minimal {
}



/**
 * @view
 * @opt hide
 * @comment Un-hiding the whole event package:
 * @match class org.daisy.urakawa.event.*
 * @opt !hide
 * @comment Setting the special colors:
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.event.ChangeNotifier<T extends DataModelChangedEvent>
 * @opt hide
 * @match class org.daisy.urakawa.event.ChangeNotifierImpl
 * @opt hide
 * @match class org.daisy.urakawa.event.Change*Event>
 * @opt hide
 */
class UML_Events extends ViewBase {
}

/**
 * @view
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 */
class UML_Events_Minimal extends UML_Events {
}
/**
 * @view
 * @opt hide
 * @match class org.daisy.urakawa.event.*
 * @opt !hide
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.event.*Event
 * @opt hide
 */
class UML_Events_Core extends ViewBase {
}




/**
 * @view
 * @opt hide
 * @comment Un-hiding the whole undo package and setting the special colors:
 * @match class org.daisy.urakawa.undo.*
 * @opt !hide
 * @opt nodefillcolor darkolivegreen1
 * @comment Hiding the exceptions and implementations:
 * @match class org.daisy.urakawa.*Exception
 * @opt hide
 * @match class org.daisy.urakawa.*Impl
 * @opt hide
 * @comment Hiding the With* interfaces:
 * @match class org.daisy.urakawa.undo.WithUndoRedoManager
 * @opt hide
 * @match class org.daisy.urakawa.undo.WithCommandFactory
 * @opt hide
 */
class UML_UndoRedo extends ViewBase {
}

/**
 * @view
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 */
class UML_UndoRedo_Minimal extends UML_UndoRedo {
}

/**
 * @view
 * @opt hide
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @comment Un-hiding the whole undo package and setting the special colors:
 * @match class org.daisy.urakawa.undo.*
 * @opt !hide
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.event.undo.*
 * @opt !hide
 * @opt nodefillcolor plum1
 * @comment Hiding the factories:
 * @match class org.daisy.urakawa.*Factory
 * @opt hide
 * @comment Hiding the exceptions and implementations:
 * @match class org.daisy.urakawa.*Exception
 * @opt hide
 * @match class org.daisy.urakawa.*Impl
 * @opt hide
 * @comment Hiding the With* interfaces:
 * @match class org.daisy.urakawa.undo.WithUndoRedoManager
 * @opt hide
 * @match class org.daisy.urakawa.undo.WithShortLongDescription
 * @opt hide
 * @match class org.daisy.urakawa.undo.WithCommandFactory
 * @opt hide
 */
class UML_UndoRedo_Events extends ViewBase {
}


/**
 * @view
 * @opt hide
 * @comment Un-hiding the media-related presentations and managers (one by one):
 * @match class org.daisy.urakawa.Presentation
 * @opt !hide
 * @match class org.daisy.urakawa.media.data.MediaDataManager
 * @opt !hide
 * @match class org.daisy.urakawa.media.data.DataProviderManager
 * @opt !hide
 * @match class org.daisy.urakawa.media.data.FileDataProviderManager
 * @opt !hide
 * @match class org.daisy.urakawa.media.MediaPresentation
 * @opt !hide
 * @match class org.daisy.urakawa.media.Media
 * @opt !hide
 * @comment Setting the special colors:
 * @match class org.daisy.urakawa.Presentation
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.Presentation
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 */
class UML_PresentationAndMedia extends ViewBase {
}

/**
 * @view
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @match class org.daisy.urakawa.media.MediaPresentation
 * @opt hide
 * @match class org.daisy.urakawa.media.Media
 * @opt hide
 */
class UML_PresentationAndMedia_Minimal extends UML_PresentationAndMedia {
}


/**
 * @view
 * @opt hide
 * @comment Un-hiding the whole metadata package:
 * @match class org.daisy.urakawa.metadata.*
 * @opt !hide
 * @comment Hiding a specific With* entity:
 * @match class org.daisy.urakawa.metadata.WithMetadataFactory
 * @opt hide
 * @match class org.daisy.urakawa.metadata.WithMetadata
 * @opt !hide
 * @comment Hiding the implementations:
 * @match class org.daisy.urakawa.*Impl
 * @opt hide
 * @comment Hiding the factories:
 * @match class org.daisy.urakawa.*Factory
 * @opt hide
 * @comment Un-hiding the presentation
 * @match class org.daisy.urakawa.Presentation
 * @opt !hide
 * @comment Setting the special colors:
 * @match class org.daisy.urakawa.Project
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.metadata.Metadata
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.Presentation
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.Presentation
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 */
class UML_ProjectPresentationMetadata extends ViewBase {
}

/**
 * @view
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @match class org.daisy.urakawa.metadata.WithMetadata
 * @opt hide
 * @match class org.daisy.urakawa.Project
 * @opt !hide
 * @match class org.daisy.urakawa.metadata.MetadataFactory
 * @opt !hide
 */
class UML_ProjectPresentationMetadata_Minimal extends
		UML_ProjectPresentationMetadata {
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
 * @match class org.daisy.urakawa.Presentation
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 */
class UML_PresentationAndFactories extends ViewBase {
}

/**
 * @view
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @match class org.daisy.urakawa.Project
 * @opt !hide
 * @match class org.daisy.urakawa.DataModelFactory
 * @opt !hide
 */
class UML_PresentationAndFactories_Minimal extends UML_PresentationAndFactories {
}




/**
 * @view
 * @opt hide
 * @comment Un-hiding the whole core package:
 * @match class org.daisy.urakawa.core.*
 * @opt !hide
 * @comment Hiding the whole visitor sub-package:
 * @match class org.daisy.urakawa.core.visitor.*
 * @opt hide
 * @comment Un-hiding a specific entity from the visitor sub-package:
 * @match class org.daisy.urakawa.core.visitor.VisitableTreeNode
 * @opt !hide
 * @comment Un-hiding a specific entity from the property sub-package:
 * @match class org.daisy.urakawa.property.Property
 * @opt !hide
 * @comment Hiding the factories, exceptions and implementations:
 * @match class org.daisy.urakawa.*Factory
 * @opt hide
 * @match class org.daisy.urakawa.*Exception
 * @opt hide
 * @match class org.daisy.urakawa.*Impl
 * @opt hide
 * @comment Hiding specific With* entities:
 * @match class org.daisy.urakawa.core.WithTreeNode
 * @opt hide
 * @match class org.daisy.urakawa.core.WithTreeNodeOwner
 * @opt hide
 * @match class org.daisy.urakawa.core.WithTreeNodeFactory
 * @opt hide
 * @match class org.daisy.urakawa.property.WithProperties
 * @opt !hide
 * @comment Setting the special colors:
 * @match class org.daisy.urakawa.core.TreeNode
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.property.Property
 * @opt nodefillcolor darkolivegreen1
 */
class UML_CoreTree extends ViewBase {
}

/**
 * @view
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @match class org.daisy.urakawa.property.WithProperties
 * @opt hide
 */
class UML_CoreTree_Minimal extends UML_CoreTree {
}

/**
 * @view
 * @match class org.daisy.urakawa.Presentation
 * @opt !hide
 * @match class org.daisy.urakawa.core.TreeNodeFactory
 * @opt !hide
 * @match class org.daisy.urakawa.property.PropertyFactory
 * @opt !hide
 * @match class org.daisy.urakawa.core.TreeNodeWriteOnlyMethods
 * @opt hide
 * @match class org.daisy.urakawa.core.TreeNodeReadOnlyMethods
 * @opt hide
 * @match class org.daisy.urakawa.core.visitor.VisitableTreeNode
 * @opt hide
 */
class UML_CoreTreeAndFactories_Minimal extends UML_CoreTree_Minimal {
}



/**
 * @view
 * @opt hide
 * @comment Un-hiding the whole visitor package and setting the special colors:
 * @match class org.daisy.urakawa.core.visitor.*
 * @opt !hide
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.core.visitor.examples.*
 * @opt hide
 * @comment Un-hiding a specific entity:
 * @match class org.daisy.urakawa.core.TreeNode
 * @opt !hide
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 */
class UML_TreeVisitor extends ViewBase {
}


/**
 * @view
 * @opt hide
 * @comment Un-hiding the whole navigator package:
 * @match class org.daisy.urakawa.navigator.*
 * @opt !hide
 * @opt nodefillcolor darkolivegreen1
 * @comment Hiding the exceptions:
 * @match class org.daisy.urakawa.*Exception
 * @opt hide
 * @match class org.daisy.urakawa.navigator.IntWrapper
 * @opt hide
 */
class UML_TreeNavigator extends ViewBase {
}

/**
 * @view
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 */
class UML_TreeNavigator_Minimal extends UML_TreeNavigator {
}
