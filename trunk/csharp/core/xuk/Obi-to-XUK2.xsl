<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns="http://www.daisy.org/urakawa/xuk/2.0"
    xmlns:oldXuk="http://www.daisy.org/urakawa/xuk/1.0" xmlns:obi="http://www.daisy.org/urakawa/obi"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
    exclude-result-prefixes="xs" version="2.0">
    <xsl:output method="xml" indent="yes" omit-xml-declaration="no" version="1.0"/>
    <!-- xsl:strip-space elements="*"/ -->

    <!-- xsl:template match="text()[not(normalize-space())]"/ -->
    <xsl:template match="text()"/>

    <xsl:template match="/">
        <xsl:apply-templates/>
    </xsl:template>

    <xsl:template match="oldXuk:Xuk">
        <xsl:element name="Xuk">
            <xsl:namespace name="xsi">
                <xsl:text>http://www.w3.org/2001/XMLSchema-instance</xsl:text>
            </xsl:namespace>
            <xsl:attribute name="noNamespaceSchemaLocation"
                namespace="http://www.w3.org/2001/XMLSchema-instance">
                <xsl:text>http://www.daisy.org/urakawa/xuk/2.0/xuk.xsd</xsl:text>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xsl:template match="oldXuk:Project">
        <xsl:element name="Project">
            <xsl:element name="PresentationFactory">
                <xsl:element name="RegisteredTypes">
                    <xsl:element name="Type">
                        <xsl:attribute name="XukLocalName">
                            <xsl:text>Presentation</xsl:text>
                        </xsl:attribute>
                        <xsl:attribute name="XukNamespaceUri">
                            <xsl:text>http://www.daisy.org/urakawa/xuk/2.0</xsl:text>
                        </xsl:attribute>
                        <xsl:attribute name="AssemblyName">
                            <xsl:text>UrakawaSDK.core</xsl:text>
                        </xsl:attribute>
                        <xsl:attribute name="AssemblyVersion">
                            <xsl:text>2.0.0.0</xsl:text>
                        </xsl:attribute>
                        <xsl:attribute name="FullName">
                            <xsl:text>urakawa.Presentation</xsl:text>
                        </xsl:attribute>
                    </xsl:element>
                </xsl:element>
            </xsl:element>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>
    <xsl:template match="oldXuk:mPresentations">
        <xsl:element name="Presentations">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>
    <xsl:template match="obi:Presentation">
        <xsl:element name="Presentation">
            <xsl:attribute name="RootUri">
                <xsl:text>./</xsl:text>
            </xsl:attribute>
            <TreeNodeFactory>
                <RegisteredTypes>
                    <Type
                        XukLocalName="TreeNode"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.core.TreeNode" />
                </RegisteredTypes>
            </TreeNodeFactory>
            <PropertyFactory
                DefaultXmlNamespaceUri="http://www.daisy.org/z3986/2005/dtbook/">
                <RegisteredTypes>
                    <Type
                        XukLocalName="ChannelsProperty"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.property.channel.ChannelsProperty" />
                    <Type
                        XukLocalName="XmlProperty"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.property.xml.XmlProperty" />
                </RegisteredTypes>
            </PropertyFactory>
            <ChannelFactory>
                <RegisteredTypes>
                    <Type
                        XukLocalName="Channel"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.property.channel.Channel" />
                    <Type
                        XukLocalName="TextChannel"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.property.channel.TextChannel" />
                    <Type
                        XukLocalName="ImageChannel"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.property.channel.ImageChannel" />
                    <Type
                        XukLocalName="AudioChannel"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.property.channel.AudioChannel" />
                </RegisteredTypes>
            </ChannelFactory>
            <MediaFactory>
                <RegisteredTypes>
                    <Type
                        XukLocalName="ManagedImageMedia"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.media.data.image.ManagedImageMedia" />
                    <Type
                        XukLocalName="ManagedAudioMedia"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.media.data.audio.ManagedAudioMedia" />
                    <Type
                        XukLocalName="TextMedia"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.media.TextMedia" />
                    <Type
                        XukLocalName="ExternalImageMedia"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.media.ExternalImageMedia" />
                    <Type
                        XukLocalName="ExternalVideoMedia"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.media.ExternalVideoMedia" />
                    <Type
                        XukLocalName="ExternalTextMedia"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.media.ExternalTextMedia" />
                    <Type
                        XukLocalName="ExternalAudioMedia"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.media.ExternalAudioMedia" />
                    <Type
                        XukLocalName="SequenceMedia"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.media.SequenceMedia" />
                </RegisteredTypes>
            </MediaFactory>
            <DataProviderFactory>
                <RegisteredTypes>
                    <Type
                        XukLocalName="FileDataProvider"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.data.FileDataProvider" />
                </RegisteredTypes>
            </DataProviderFactory>
            <MediaDataFactory>
                <RegisteredTypes>
                    <Type
                        XukLocalName="JpgImageMediaData"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.media.data.image.codec.JpgImageMediaData" />
                    <Type
                        XukLocalName="WavAudioMediaData"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.media.data.audio.codec.WavAudioMediaData" />
                </RegisteredTypes>
            </MediaDataFactory>
            <CommandFactory>
                <RegisteredTypes>
                    <Type
                        XukLocalName="CompositeCommand"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.command.CompositeCommand" />
                    <Type
                        XukLocalName="TreeNodeSetManagedAudioMediaCommand"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.commands.TreeNodeSetManagedAudioMediaCommand" />
                    <Type
                        XukLocalName="ManagedAudioMediaInsertDataCommand"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.commands.ManagedAudioMediaInsertDataCommand" />
                    <Type
                        XukLocalName="TreeNodeAudioStreamDeleteCommand"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.commands.TreeNodeAudioStreamDeleteCommand" />
                    <Type
                        XukLocalName="TreeNodeSetIsMarkedCommand"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.commands.TreeNodeSetIsMarkedCommand" />
                    <Type
                        XukLocalName="TreeNodeChangeTextCommand"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.commands.TreeNodeChangeTextCommand" />
                    <Type
                        XukLocalName="MetadataAddCommand"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.commands.MetadataAddCommand" />
                    <Type
                        XukLocalName="MetadataRemoveCommand"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.commands.MetadataRemoveCommand" />
                    <Type
                        XukLocalName="MetadataSetContentCommand"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.commands.MetadataSetContentCommand" />
                    <Type
                        XukLocalName="MetadataSetNameCommand"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.commands.MetadataSetNameCommand" />
                    <Type
                        XukLocalName="MetadataSetIdCommand"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.commands.MetadataSetIdCommand" />
                </RegisteredTypes>
            </CommandFactory>
            <MetadataFactory>
                <RegisteredTypes>
                    <Type
                        XukLocalName="Metadata"
                        XukNamespaceUri="http://www.daisy.org/urakawa/xuk/2.0"
                        AssemblyName="UrakawaSDK.core"
                        AssemblyVersion="2.0.0.0"
                        FullName="urakawa.metadata.Metadata" />
                </RegisteredTypes>
            </MetadataFactory>
            <ExternalFileDataFactory>
                <RegisteredTypes />
            </ExternalFileDataFactory>
            <ChannelsManager>
                <Channels>
                    <TextChannel
                        Uid="CHID0001"
                        Name="The Text Channel" />
                    <AudioChannel
                        Uid="CHID0000"
                        Name="The Audio Channel" />
                    <ImageChannel
                        Uid="CHID0002"
                        Name="The Image Channel" />
                </Channels>
            </ChannelsManager>
            <DataProviderManager
                DataFileDirectoryPath="___Data">
                <DataProviders />
            </DataProviderManager>
            <MediaDataManager
                enforceSinglePCMFormat="true">
                <DefaultPCMFormat>
                    <PCMFormatInfo
                        NumberOfChannels="1"
                        SampleRate="44100"
                        BitDepth="16" />
                </DefaultPCMFormat>
                <MediaDatas />
            </MediaDataManager>
            <ExternalFileDataManager>
                <ExternalFileDatas />
            </ExternalFileDataManager>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>
    <xsl:template match="oldXuk:mMetadata">
        <xsl:element name="Metadatas">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>
    <xsl:template match="obi:Metadata">
        <xsl:element name="Metadata">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>
</xsl:stylesheet>
