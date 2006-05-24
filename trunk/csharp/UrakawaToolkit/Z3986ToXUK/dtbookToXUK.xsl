<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
 <xsl:param name="dtbookDir" select="''"/>
 <xsl:output method="xml" encoding="utf-8" indent="yes" />
<!--
Match root element - generates the header
-->
 <xsl:template match="/">
  <XUK xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="xuk.xsd">
   <metaData>
    <!-- TODO: Extract metadata from dtbook document-->
   </metaData>
   <Presentation>
    <ChannelsManager>
     <Channel id="audioChannel">AUDIO</Channel>
     <Channel id="textChannel">TEXT</Channel>
    </ChannelsManager>
    <xsl:for-each select="dtbook/book">
     <xsl:call-template name="elementnode"/>
    </xsl:for-each>
   </Presentation>
  </XUK>
 </xsl:template>
<!--
Inserts AudioObject elements for the audio elements 
in the supplied SMIL time container
-->
 <xsl:template name="audioMediaObject">
  <xsl:param name="SMILTimeContainers" select="/.."/>
  <xsl:param name="SMILaudioCount" select="count($SMILTimeContainers//audio)"/>
  <xsl:choose>
   <xsl:when test="$SMILaudioCount=0"/>
   <xsl:when test="$SMILaudioCount=1">
    <ChannelMapping channel="audioChannel">
     <Media type="AUDIO" src="{$SMILTimeContainers//audio/@src}" clipBegin="{$SMILTimeContainers//audio/@clipBegin}" clipEnd="{$SMILTimeContainers//audio/@clipEnd}">
      <xsl:if test="$SMILTimeContainers/@id">
       <xsl:attribute name="id"><xsl:value-of select="$SMILTimeContainers//audio/@id"/></xsl:attribute>
      </xsl:if>
     </Media>
    </ChannelMapping>
   </xsl:when>
   <xsl:otherwise>
    <ChannelMapping channel="audioChannel">
     <SequenceMedia type="AUDIO">
      <xsl:for-each select="$SMILTimeContainers">
       <xsl:for-each select=".//audio">
        <Media type="AUDIO" src="{@src}" clipBegin="{@clipBegin}" clipEnd="{@clipEnd}">
         <xsl:if test="./ancestor::par/@id">
          <xsl:attribute name="id"><xsl:value-of select="@id"/></xsl:attribute>
         </xsl:if>
        </Media>
       </xsl:for-each>
      </xsl:for-each>
     </SequenceMedia>
    </ChannelMapping>
   </xsl:otherwise>
   </xsl:choose>
 </xsl:template>
<!--
Inserts a text node
-->
 <xsl:template name="textnode">
  <xsl:param name="SMILTimeContainers" select="/.."/>
  <CoreNode>
   <mProperties>
    <ChannelsProperty>
     <ChannelMapping channel="textChannel">
      <Media type="TEXT"><xsl:value-of select="."/></Media>
     </ChannelMapping>
     <xsl:call-template name="audioMediaObject">
      <xsl:with-param name="SMILTimeContainers" select="$SMILTimeContainers"/>
     </xsl:call-template>
    </ChannelsProperty>
   </mProperties>
  </CoreNode>
 </xsl:template>
<!--
Inserts an element node 
-->
 <xsl:template name="elementnode">
  <xsl:param name="smilAnchor" select="substring-after(@smilref, '#')"/>
  <xsl:param name="smil" select="document(substring-before(@smilref, '#'))"/>
  <xsl:param name="textLinkBack" select="$smil//*[@id=$smilAnchor]/descendant-or-self::text[substring-after(@src, '#')=current()/@id]/@src"/>
  <xsl:param name="SMILTimeContainers" select="$smil//par[text[@src=$textLinkBack]] | $smil//par[(preceding::text[@src=$textLinkBack]) and (following::text[@src=$textLinkBack])]"/>
  <CoreNode>
   <mProperties>
    <XmlProperty name="{local-name()}">
     <xsl:if test="not (namespace-uri()='')">
      <xsl:attribute name="namespace"><xsl:value-of select="namespace-uri()=''"/></xsl:attribute>
     </xsl:if>
     <xsl:for-each select="@*[local-name()!='smilref']">
      <xsl:if test="not (namespace-uri()='')">
       <xsl:attribute name="namespace"><xsl:value-of select="namespace-uri()=''"/></xsl:attribute>
      </xsl:if>
      <XmlAttribute name="{local-name()}">
       <xsl:if test="not (namespace-uri()='')">
        <xsl:attribute name="namespace"><xsl:value-of select="namespace-uri()=''"/></xsl:attribute>
       </xsl:if>
       <xsl:value-of select="."/>
      </XmlAttribute>
     </xsl:for-each>
    </XmlProperty>
    <ChannelsProperty>
     <xsl:if test="text() and not(*)">
      <ChannelMapping channel="textChannel">
       <Media type="TEXT">
        <xsl:value-of select="."/>
       </Media>
      </ChannelMapping>
     </xsl:if>
     <xsl:if test="(@smilref) and not (./*[@smilref])">
      <xsl:call-template name="audioMediaObject">
       <xsl:with-param name="SMILTimeContainers" select="$SMILTimeContainers"/>
      </xsl:call-template>
     </xsl:if>
    </ChannelsProperty>
   </mProperties>
   <xsl:if test="*">
    <xsl:if test="(@smilref) and (.//*[@smilref])">
     <NonDistributedAudios>     
      <xsl:attribute name="smilref"><xsl:value-of select="@smilref"/></xsl:attribute>
      <xsl:call-template name="audioMediaObject">
       <xsl:with-param name="SMILTimeContainers" select="$SMILTimeContainers"/>
      </xsl:call-template>
     </NonDistributedAudios>
    </xsl:if>
    <xsl:for-each select="*|text()">
     <xsl:choose>
      <xsl:when test="self::text()">
       <xsl:call-template name="textnode"/>
      </xsl:when>
      <xsl:otherwise>
       <xsl:call-template name="elementnode"/>
      </xsl:otherwise>
     </xsl:choose>
    </xsl:for-each>
   </xsl:if>
  </CoreNode>
 </xsl:template>
</xsl:stylesheet>