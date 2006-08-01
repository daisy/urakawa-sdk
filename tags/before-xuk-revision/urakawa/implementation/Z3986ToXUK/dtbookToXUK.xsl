<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
 <xsl:output method="xml" encoding="utf-8" indent="yes" />
<!--
Match root element - generates the header
-->
 <xsl:template match="/">
  <XUK xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
   <ProjectMetadata>
    <!-- TODO: Extract metadata from dtbook document-->
   </ProjectMetadata>
   <Presentation>
    <ChannelsManager>
     <Channel id="textChannel">TEXT</Channel>
     <Channel id="audioChannel">AUDIO</Channel>
    </ChannelsManager>
    <xsl:for-each select="/*[local-name()='dtbook']">
			<CoreNode>
			<mProperties>
				<XmlProperty name="dtbook">
				<xsl:if test="not (namespace-uri()='')">
					<xsl:attribute name="namespace"><xsl:value-of select="namespace-uri()"/></xsl:attribute>
				</xsl:if>
				</XmlProperty>
			</mProperties>
 			<xsl:for-each select="*[local-name()='book']">
 				<xsl:call-template name="elementnode"/>
			</xsl:for-each>
			</CoreNode>
    </xsl:for-each>
   </Presentation>
  </XUK>
 </xsl:template>
<!--
Inserts a text node
-->
 <xsl:template name="textnode">
  <CoreNode>
   <mProperties>
    <ChannelsProperty>
     <ChannelMapping channel="textChannel">
      <Media type="TEXT"><xsl:value-of select="."/></Media>
     </ChannelMapping>
    </ChannelsProperty>
   </mProperties>
  </CoreNode>
 </xsl:template>
<!--
Inserts an element node 
-->
 <xsl:template name="elementnode">
  <CoreNode>
   <mProperties>
    <XmlProperty name="{local-name()}">
     <xsl:if test="not (namespace-uri()='')">
      <xsl:attribute name="namespace"><xsl:value-of select="namespace-uri()"/></xsl:attribute>
     </xsl:if>
     <xsl:for-each select="@*">
      <XmlAttribute name="{local-name()}">
       <xsl:if test="not (namespace-uri()='')">
        <xsl:attribute name="namespace"><xsl:value-of select="namespace-uri()"/></xsl:attribute>
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
    </ChannelsProperty>
   </mProperties>
   <xsl:if test="*">
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