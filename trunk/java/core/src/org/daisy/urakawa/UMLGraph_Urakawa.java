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
 * @tagvalue Notes "{The arrows in blue-ish color are Dependency relationships,
 *           <br/> whereas other arrows in gray denote generalizations. <br/>
 *           <br/> }Notes"
 * @tagvalue Notes "{The blue annotations on the Dependency arrows (Name and
 *           Multiplicity) <br/> provide additional specification. For example,
 *           they can represent Associations <br/> with the specified
 *           Navigability and Multiplicity, and the given Role-Name is either
 *           <br/> 'Aggregation' or 'Composition'. When Multiplicity is
 *           indicated on the arrow <br/> start side, the uni-directional
 *           Navigability becomes bi-directional. <br/> Another case is when
 *           using the 'Create' Role-Name: this provides <br/> additional
 *           information as to what Instance types the Entity can create. <br/>
 *           This representation system is not UML-standard, and has been
 *           introduced <br/> in this design representation in order to address
 *           the shortcomings of <br/> Interfaces, in praticular the inability
 *           to have outward Associations. <br/> Abstract classes would be more
 *           structurally expressive than Interfaces, <br/> and therefore would
 *           have not required such workaround. But to avoid <br/> clutter of
 *           the UML diagram we only show the corresponding Interfaces, <br/>
 *           thus requiring this sort of extra information. <br/> <br/> }Notes"
 * @tagvalue Notes "{The Entities with a white background color are not specific
 *           to this Class Diagram <br/> and may be used in other Class
 *           Diagrams. This is why they are marked as such. <br/> <br/> }Notes"
 * @tagvalue Notes "{The Entities with a bright green background color are
 *           'Interface Lollipops': <br/> they refer to another part of the
 *           Model outside of this Class Diagram. <br/> The description of this
 *           Interface (Operations) is therefore ommited. <br/> <br/> }Notes"
 * @tagvalue Notes "{The Class names in red are just for highlighting purposes,
 *           <br/> for a reader to visually locate actual implementations in the
 *           Diagram. <br/> <br/> }Notes"
 * @tagvalue Notes "{Some Operations may have an '{Exceptions AnException,
 *           AnotherException}' annotation. <br/> This is used to show the full
 *           method signature including thrown Exceptions. <br/> These
 *           Exceptions are mostly used for assertion and they should be
 *           implemented <br/> and raised according to the full specification
 *           available in the design comments <br/> (not shown in the Class
 *           Diagram, please see the Java source code) <br/> Implementations of
 *           this error checking paradigm may vary, <br/> depending on language
 *           and performance considerations. <br/> <br/> }Notes"
 * @tagvalue Notes "{Some operations are decorated with an 'Initialize'
 *           stereotype. <br/> This means that they should *only* be called at
 *           construction/initialization time, <br/> usually by the IXukAble. It
 *           has the same effect as having a 'package' visibility, <br/>
 *           assuming the IXukAble is in the same package of course (an end-user
 *           from another package <br/> could not call the method). <br/> <br/>
 *           }Notes"
 * @tagvalue Notes "{ The Entities with a dark-red font color are dedicated to
 *           validation. <br/> Like most colors used in the diagram, this is
 *           just a visual hint to help the reader.<br/> <br/> }Notes"
 * @opt nodefillcolor Yellow
 * @opt nodefonttagname arial
 * @opt nodefonttagsize 10
 */
class StickyNotes {
	/**
	 * The arrows in blue-ish color are Dependency relationships, whereas other
	 * arrows in gray denote generalizations. The blue annotations on the
	 * Dependency arrows (Name and Multiplicity) provide additional
	 * specification. For example, they can represent Associations with the
	 * specified Navigability and Multiplicity, and the given Role-Name is
	 * either 'Aggregation' or 'Composition'. When Multiplicity is indicated on
	 * the arrow start side, the uni-directional Navigability becomes
	 * bi-directional. Another case is when using the 'Create' Role-Name: this
	 * provides additional information as to what Instance types the Entity can
	 * create. This representation system is not UML-standard, and has been
	 * introduced in this design representation in order to address the
	 * shortcomings of Interfaces, in praticular the inability to have outward
	 * Associations. Abstract classes would be more structurally expressive than
	 * Interfaces, and therefore would have not required such workaround. But to
	 * avoid clutter of the UML diagram we only show the corresponding
	 * Interfaces, thus requiring this sort of extra information. The Entities
	 * with a white background color are not specific to this Class Diagram and
	 * may be used in other Class Diagrams. This is why they are marked as such.
	 * The Entities with a bright green background color are 'Interface
	 * Lollipops': they refer to another part of the Model outside of this Class
	 * Diagram. The description of this Interface (Operations) is therefore
	 * ommited. Class names in red are just for highlighting purposes, for a
	 * reader to visually locate actual implementations in the Diagram. Some
	 * Operations may have an '{Exceptions AnException, AnotherException}'
	 * annotation. This is used to show the full method signature including
	 * thrown Exceptions. These Exceptions are mostly used for assertion and
	 * they should be implemented and raised according to the full specification
	 * available in the design comments (not shown in the Class Diagram, please
	 * see the Java source code) Implementations of this error checking paradigm
	 * may vary, depending on language and performance considerations. Some
	 * operations are decorated with an 'Initialize' stereotype. This means that
	 * they should *only* be called at construction/initialization time, usually
	 * by the IXukAble. It has the same effect as having a 'package' visibility,
	 * assuming the IXukAble is in the same package of course (an end-user from
	 * another package could not call the method). The Entities with a dark-red
	 * font color are dedicated to validation. Like most colors used in the
	 * diagram, this is just a visual hint to help the reader.
	 */
}


/**
 * @view
 * @opt hide
 * @comment Un-hiding the whole channel package:
 * @match class org.daisy.urakawa.property.channel.*
 * @opt !hide
 * @opt nodefillcolor darkolivegreen1
 * @comment Hiding the factories, exceptions and implementations:
 * @match class org.daisy.urakawa.*IXukAble
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
 * @match class org.daisy.urakawa.property.channel.IWithMedia
 * @opt hide
 * @match class org.daisy.urakawa.property.channel.IWithChannelsManager
 * @opt hide
 * @match class org.daisy.urakawa.property.channel.IWithChannelFactory
 * @opt hide
 * @match class org.daisy.urakawa.property.channel.IWithChannelsPropertyFactory
 * @opt hide
 * @match class org.daisy.urakawa.property.channel.AudioChannel
 * @opt hide
 * @match class org.daisy.urakawa.property.channel.ManagedAudioChannel
 * @opt hide
 * @match class org.daisy.urakawa.property.channel.TextChannel
 * @opt hide
 * @comment Un-hiding external entities:
 * @match class org.daisy.urakawa.property.IProperty
 * @opt !hide
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @match class org.daisy.urakawa.media.IMedia
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
 * @match class org.daisy.urakawa.property.channel.IWithName
 * @opt hide
 */
class UML_ChannelsProperty_Minimal extends UML_ChannelsProperty {
}

/**
 * @view
 * @match class org.daisy.urakawa.property.channel.IChannelFactory
 * @opt !hide
 * @match class org.daisy.urakawa.property.channel.IChannelsPropertyFactory
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
 * @match class org.daisy.urakawa.event.IEventHandler<T extends DataModelChangedEvent>
 * @opt hide
 * @match class org.daisy.urakawa.event.EventHandlerImpl
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
 * @match class org.daisy.urakawa.undo.IWithUndoRedoManager
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
 * @match class org.daisy.urakawa.undo.IWithUndoRedoManager
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
 * @match class org.daisy.urakawa.IPresentation
 * @opt !hide
 * @match class org.daisy.urakawa.media.data.IMediaDataManager
 * @opt !hide
 * @match class org.daisy.urakawa.media.data.IDataProviderManager
 * @opt !hide
 * @match class org.daisy.urakawa.media.data.IFileDataProviderManager
 * @opt !hide
 * @match class org.daisy.urakawa.media.IMediaPresentation
 * @opt !hide
 * @match class org.daisy.urakawa.media.IMedia
 * @opt !hide
 * @comment Setting the special colors:
 * @match class org.daisy.urakawa.IPresentation
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.IPresentation
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
 * @match class org.daisy.urakawa.media.IMediaPresentation
 * @opt hide
 * @match class org.daisy.urakawa.media.IMedia
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
 * @match class org.daisy.urakawa.metadata.IWithMetadataFactory
 * @opt hide
 * @match class org.daisy.urakawa.metadata.IWithMetadata
 * @opt !hide
 * @comment Hiding the implementations:
 * @match class org.daisy.urakawa.*Impl
 * @opt hide
 * @comment Hiding the factories:
 * @match class org.daisy.urakawa.*Factory
 * @opt hide
 * @comment Un-hiding the presentation
 * @match class org.daisy.urakawa.IPresentation
 * @opt !hide
 * @comment Setting the special colors:
 * @match class org.daisy.urakawa.IProject
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.metadata.IMetadata
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.IPresentation
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.IPresentation
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
 * @match class org.daisy.urakawa.metadata.IWithMetadata
 * @opt hide
 * @match class org.daisy.urakawa.IProject
 * @opt !hide
 * @match class org.daisy.urakawa.metadata.IMetadataFactory
 * @opt !hide
 */
class UML_ProjectPresentationMetadata_Minimal extends
		UML_ProjectPresentationMetadata {
}


/**
 * @view
 * @opt hide
 * @comment Un-hiding the presentation and factories (one by one):
 * @match class org.daisy.urakawa.IPresentation
 * @opt !hide
 * @match class org.daisy.urakawa.media.data.IMediaDataFactory
 * @opt !hide
 * @match class org.daisy.urakawa.media.data.IDataProviderFactory
 * @opt !hide
 * @match class org.daisy.urakawa.media.data.IFileDataProviderFactory
 * @opt !hide
 * @match class org.daisy.urakawa.property.xml.IXmlPropertyFactory
 * @opt !hide
 * @match class org.daisy.urakawa.property.channel.IChannelFactory
 * @opt !hide
 * @match class org.daisy.urakawa.property.channel.IChannelsPropertyFactory
 * @opt !hide
 * @match class org.daisy.urakawa.property.IGenericPropertyFactory
 * @opt !hide
 * @match class org.daisy.urakawa.property.IPropertyFactory
 * @opt !hide
 * @match class org.daisy.urakawa.core.ITreeNodeFactory
 * @opt !hide
 * @match class org.daisy.urakawa.media.IMediaFactory
 * @opt !hide
 * @match class org.daisy.urakawa.undo.CommandFactory
 * @opt !hide
 * @match class org.daisy.urakawa.metadata.IMetadataFactory
 * @opt !hide
 * @comment Setting the special colors:
 * @match class org.daisy.urakawa.IPresentation
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.IPresentation
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
 * @match class org.daisy.urakawa.IProject
 * @opt !hide
 * @match class org.daisy.urakawa.IDataModelFactory
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
 * @match class org.daisy.urakawa.core.visitor.IVisitableTreeNode
 * @opt !hide
 * @comment Un-hiding a specific entity from the property sub-package:
 * @match class org.daisy.urakawa.property.IProperty
 * @opt !hide
 * @comment Hiding the factories, exceptions and implementations:
 * @match class org.daisy.urakawa.*Factory
 * @opt hide
 * @match class org.daisy.urakawa.*Exception
 * @opt hide
 * @match class org.daisy.urakawa.*Impl
 * @opt hide
 * @comment Hiding specific With* entities:
 * @match class org.daisy.urakawa.core.IWithTreeNode
 * @opt hide
 * @match class org.daisy.urakawa.core.IWithTreeNodeOwner
 * @opt hide
 * @match class org.daisy.urakawa.core.IWithTreeNodeFactory
 * @opt hide
 * @match class org.daisy.urakawa.property.IWithProperties
 * @opt !hide
 * @comment Setting the special colors:
 * @match class org.daisy.urakawa.core.ITreeNode
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.property.IProperty
 * @opt nodefillcolor darkolivegreen1
 */
class UML_CoreTree extends ViewBase {
}

/**
 * @view
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @match class org.daisy.urakawa.property.IWithProperties
 * @opt hide
 */
class UML_CoreTree_Minimal extends UML_CoreTree {
}

/**
 * @view
 * @match class org.daisy.urakawa.IPresentation
 * @opt !hide
 * @match class org.daisy.urakawa.core.ITreeNodeFactory
 * @opt !hide
 * @match class org.daisy.urakawa.property.IPropertyFactory
 * @opt !hide
 * @match class org.daisy.urakawa.core.ITreeNodeWriteOnlyMethods
 * @opt hide
 * @match class org.daisy.urakawa.core.ITreeNodeReadOnlyMethods
 * @opt hide
 * @match class org.daisy.urakawa.core.visitor.IVisitableTreeNode
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
 * @match class org.daisy.urakawa.core.ITreeNode
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



/**
 * @view
 * @opt hide
 * @comment Un-hiding the whole xml package:
 * @match class org.daisy.urakawa.property.xml.*
 * @opt !hide
 * @opt nodefillcolor darkolivegreen1
 * @comment Hiding the factories, exceptions and implementations:
 * @match class org.daisy.urakawa.*Exception
 * @opt hide
 * @match class org.daisy.urakawa.*Impl
 * @opt hide
 * @match class org.daisy.urakawa.*IXukAble
 * @opt hide
 * @comment Un-hiding external entities
 * @match class org.daisy.urakawa.property.IProperty
 * @opt !hide
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @comment Hiding specific With* entities
 * @match class org.daisy.urakawa.property.xml.IWithXmlProperty
 * @opt hide
 * @match class org.daisy.urakawa.property.xml.IWithXmlPropertyFactory
 * @opt hide
 * @match class org.daisy.urakawa.property.xml.IXmlPropertyFactory
 * @opt hide
 */
class UML_XmlProperty extends ViewBase {
}

/**
 * @view
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @match class org.daisy.urakawa.property.xml.IWithXmlAttributes
 * @opt hide
 * @match class org.daisy.urakawa.property.xml.IWithValue
 * @opt hide
 * @match class org.daisy.urakawa.property.xml.IWithQualifiedName
 * @opt hide
 * @match class org.daisy.urakawa.property.xml.IXmlPropertyFactory
 * @opt !hide
 */
class UML_XmlProperty_Minimal extends UML_XmlProperty {
}




/**
 * @view
 * @opt hide
 * @comment Un-hiding the whole media package:
 * @match class org.daisy.urakawa.media.*
 * @opt !hide
 * @comment Hiding the whole timing package:
 * @match class org.daisy.urakawa.media.timing.*
 * @opt hide
 * @comment Hiding the whole media data package:
 * @match class org.daisy.urakawa.media.data.*
 * @opt hide
 * @comment Hiding the factories, exceptions and implementations:
 * @match class org.daisy.urakawa.*Exception
 * @opt hide
 * @match class org.daisy.urakawa.*Impl
 * @opt hide
 * @match class org.daisy.urakawa.*Factory
 * @opt hide
 * @match class org.daisy.urakawa.*IXukAble
 * @opt hide
 * @comment Hiding the presentation:
 * @match class org.daisy.urakawa.media.IMediaPresentation
 * @opt hide
 * @comment Un-hiding some media data entities:
 * @match class org.daisy.urakawa.media.data.audio.IManagedAudioMedia
 * @opt !hide
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @match class org.daisy.urakawa.media.data.IManagedMedia
 * @opt !hide
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @comment Setting the special colors:
 * @match class org.daisy.urakawa.media.[^.]+
 * @opt nodefillcolor darkolivegreen1
 */
class UML_Media extends ViewBase {
}

/**
 * @view
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 */
class UML_Media_Minimal extends UML_Media {
}

/**
 * @view
 * @match class org.daisy.urakawa.media.IMediaFactory
 * @opt !hide
 */
class UML_MediaAndFactories_Minimal extends UML_Media_Minimal {
}


/**
 * @view
 * @opt hide
 * @comment Un-hiding the whole media data package:
 * @match class org.daisy.urakawa.media.data.*
 * @opt !hide
 * @comment Hiding the factories (includign With*), other With* entities, exceptions and implementations:
 * @match class org.daisy.urakawa.*Exception
 * @opt hide
 * @match class org.daisy.urakawa.*Impl
 * @opt hide
 * @match class org.daisy.urakawa.*Factory
 * @opt hide
 * @match class org.daisy.urakawa.media.data.*With.*Manager
 * @opt hide
 * @match class org.daisy.urakawa.media.data.*With.*Data
 * @opt hide
 * @comment Hiding the presentation:
 * @match class org.daisy.urakawa.media.data.MediaDataPresentation
 * @opt hide
 * @comment Un-hiding the media base type.
 * @match class org.daisy.urakawa.media.IAudioMedia
 * @opt !hide
 * @comment Un-hiding the abstract impl.
 * @match class org.daisy.urakawa.media.data.*AbstractImpl
 * @opt !hide
 * @comment Setting the special colors:
 * @match class org.daisy.urakawa.media.data.*
 * @opt nodefillcolor darkolivegreen1
 * @match class org.daisy.urakawa.media.data.audio.*
 * @opt nodefillcolor plum1
 * @match class org.daisy.urakawa.media.data.IManagedMedia
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 */
class UML_MediaData extends ViewBase {
}

/**
 * @view
 * @opt !operations
 * @opt !constructors
 * @opt !attributes
 * @match class org.daisy.urakawa.media.data.IWithName
 * @opt hide
 */
class UML_MediaData_Minimal extends UML_MediaData {
}

/**
 * @view
 * @match class org.daisy.urakawa.media.*Factory
 * @opt !hide
 * @match class org.daisy.urakawa.media.IWithMediaFactory
 * @opt hide
 * @match class org.daisy.urakawa.media.data.IWithMediaDataFactory
 * @opt hide
 * @match class org.daisy.urakawa.media.data.IWithDataProviderFactory
 * @opt hide
 */
class UML_MediaDataAndFactories_Minimal extends UML_MediaData_Minimal {
}