<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.daisy.org/urakawa/xuk/0.5" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
 <xsl:output method="xml" encoding="utf-8" indent="yes"/>
 <xsl:template match="/XUK">
  <xsl:element name="XUK">
   <xsl:if test="@xsi:noNamespaceSchemaLocation">
    <xsl:attribute name="xsi:schemaLocation">
     <xsl:text>http://www.daisy.org/urakawa/xuk/0.5 </xsl:text><xsl:value-of select="@xsi:noNamespaceSchemaLocation"/>
    </xsl:attribute>
   </xsl:if>
  	<xsl:apply-templates/>
  </xsl:element>
 </xsl:template>
 <xsl:template match="*">
  <xsl:choose>
  	<xsl:when test="namespace-uri()=''">
  	 <xsl:element name="{local-name()}">
  	  <xsl:call-template name="AllAttrs"/>
  	  <xsl:apply-templates/>
  	 </xsl:element>
  	</xsl:when>
  	<xsl:otherwise>
  	 <xsl:copy>
  	 	<xsl:call-template name="AllAttrs"/>
  	  <xsl:apply-templates/>
  	 </xsl:copy>
  	</xsl:otherwise>
  </xsl:choose>
 </xsl:template>
 <xsl:template match="SequenceMedia">
  <xsl:element name="SequenceMedia">
   <xsl:apply-templates select="*"/>
  </xsl:element>
 </xsl:template>
 <xsl:template match="Media">
 	<xsl:choose>
 	 <xsl:when test="@type='AUDIO'">
 	  <xsl:element name="AudioMedia">
  	  <xsl:call-template name="AttrsExceptType"/>
 	  </xsl:element>
 	 </xsl:when>
 	 <xsl:when test="@type='TEXT'">
 	  <xsl:element name="TextMedia">
  	  <xsl:call-template name="AttrsExceptType"/>
  	  <xsl:value-of select="."/>
 	  </xsl:element>
 	 </xsl:when>
 	 <xsl:when test="@type='IMAGE'">
 	  <xsl:element name="ImageMedia">
  	  <xsl:call-template name="AttrsExceptType"/>
 	  </xsl:element>
 	 </xsl:when>
 	 <xsl:when test="@type='VIDEO'">
 	  <xsl:element name="VideoMedia">
  	  <xsl:call-template name="AttrsExceptType"/>
 	  </xsl:element>
 	 </xsl:when>
 	 <xsl:otherwise>
 	  <xsl:copy>
 	   <xsl:apply-templates/>
 	  </xsl:copy>
 	 </xsl:otherwise>
 	</xsl:choose>
 </xsl:template>
 <xsl:template name="AllAttrs">
		<xsl:for-each select="@*">
			<xsl:attribute name="{local-name()}" namespace="{namespace-uri()}"><xsl:value-of select="."/></xsl:attribute>
		</xsl:for-each>
 </xsl:template>
 <xsl:template name="AttrsExceptType">
		<xsl:for-each select="@*[local-name()!='type']">
			<xsl:attribute name="{local-name()}" namespace="{namespace-uri()}"><xsl:value-of select="."/></xsl:attribute>
		</xsl:for-each>
 </xsl:template>
</xsl:stylesheet>