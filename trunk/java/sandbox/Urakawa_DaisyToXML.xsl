<?xml version="1.0" encoding="UTF-8"?>

<!--
1) Download SAXON:
http://prdownloads.sourceforge.net/saxon/saxonb8-6-1.zip
or
http://saxon.sourceforge.net/

2) extract saxon8.jar in your local repository 'urakawa/architecture/sandbox/'

3) extract SampleDTB.zip in 'urakawa/architecture/sandbox/SampleDTB/'

4) Command line:
cd urakawa/architecture/sandbox/
java -jar saxon8.jar -o Urakawa_DaisyToXML_OUTPUT_TEST.xml SampleDTB/SampleDTB.xml Urakawa_DaisyToXML.xsl
-->

<xsl:transform xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

<xsl:output encoding="utf-8" method="xml" indent="no" version="1.0" omit-xml-declaration="no" doctype-public="PUBLIC-URAKAWA" doctype-system="SYSTEM-URAKAWA"/>

<xsl:decimal-format name="comma" decimal-separator="," grouping-separator="."/>

<xsl:variable name="now" select="current-dateTime()"/>

<xsl:strip-space elements="*"/>

<xsl:template match="/">
	<xsl:message terminate="no">TRACE: root.</xsl:message>
	<urakawaProject>
		<xsl:comment>Daisy 3 (z3986) to flat XML, by Daniel WECK</xsl:comment>
		<xsl:comment>Generated: <xsl:value-of select="$now"/></xsl:comment>
		<xsl:apply-templates/>
	</urakawaProject>
</xsl:template>

<xsl:template match="text()">
	<!-- This is to eliminate XML text() from the output -->
</xsl:template>

<xsl:template match="head">
	<!-- must be swallowed, as template is called manually -->
</xsl:template>

<xsl:template match="dtbook">
	<xsl:message terminate="no">TRACE: dtbook.</xsl:message>
	<xsl:call-template name="metaData"/>
	<xsl:apply-templates/>
</xsl:template>

<xsl:template name="metaData">
	<xsl:message terminate="no">TRACE: metaData.</xsl:message>
	<metaData>
		<xsl:choose>
			<xsl:when test="head">
				<xsl:choose>
					<xsl:when test="head/title">
						<key name="bookTitle" value="{head/title}"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:message terminate="no">WARNING: This book has no title.</xsl:message>
					</xsl:otherwise>
		    	</xsl:choose>
				<xsl:for-each select="head/meta">
					<key name="{@name}" value="{@content}"/>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<xsl:message terminate="no">WARNING: This book has no meta-data.</xsl:message>
			</xsl:otherwise>
		</xsl:choose>
		<xsl:if test="book">
			<xsl:if test="book/@lang">
				<key name="bookLanguage" value="{book/@lang}"/>
			</xsl:if>
		</xsl:if>
	</metaData>
</xsl:template>

<xsl:template match="book">
	<xsl:message terminate="no">TRACE: book.</xsl:message>
<xsl:text>
</xsl:text>
<xsl:text>
</xsl:text>
	<coreTree>
		<xsl:apply-templates/>
	</coreTree>
<xsl:text>
</xsl:text>
<xsl:text>
</xsl:text>
</xsl:template>

<xsl:template name="par">
	<xsl:message terminate="no">TRACE: par.</xsl:message>
	<!-- xsl:for-each select="text">
		<xsl:if test="@src">
			<xsl:for-each select="document(@src)">
				<text channel="textFromSMIL" src="{text()}"/>
			</xsl:for-each>
		</xsl:if>
	</xsl:for-each -->
	<xsl:for-each select="seq">
		<xsl:choose>
			<xsl:when test="count(current()/*) > 1">
				<sequence>
					<xsl:for-each select="audio">
						<xsl:if test="@src">
							<audio channel="audioFromSMIL" src="{@src}" clipBegin="{@clipBegin}" clipEnd="{@clipEnd}"/>
						</xsl:if>
					</xsl:for-each>
				</sequence>
			</xsl:when>
			<xsl:otherwise>
				<xsl:for-each select="audio">
					<xsl:if test="@src">
						<audio channel="audioFromSMIL" src="{@src}" clipBegin="{@clipBegin}" clipEnd="{@clipEnd}"/>
					</xsl:if>
				</xsl:for-each>
			</xsl:otherwise>
    	</xsl:choose>
	</xsl:for-each>
</xsl:template>

<xsl:template match="*">
	<xsl:message terminate="no">TRACE: catch ALL [<xsl:value-of select="name()"/>].</xsl:message>
	<node>
		<properties>
			<structure type="{name(.)}">
			<xsl:for-each select="@*">
				<xsl:if test="not (name() = 'id') and not (name() = 'smilref')">
					<xsl:attribute name="{name()}">
						<xsl:value-of select="."/>
					</xsl:attribute>
				</xsl:if>
			</xsl:for-each>
			</structure>
			<medias>
				<xsl:if test="text()">
					<text channel="textFromDTBook" src="{text()}"/>
				</xsl:if>
				<xsl:if test="@smilref">
					<xsl:for-each select="document(@smilref)">
						<xsl:if test="name() = 'par'">
							<xsl:call-template name="par"/>
						</xsl:if>
					</xsl:for-each>
				</xsl:if>
			</medias>
		</properties>
		<xsl:apply-templates/>
	</node>
</xsl:template>

</xsl:transform>