<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="2.0">

<xsl:output encoding="utf-8" method="xml" indent="yes" version="1.0" omit-xml-declaration="no" doctype-public="DOCTYPE_PUBLIC-URAKAWA" doctype-system="DOCTYPE_SYSTEM-URAKAWA"/>
<xsl:decimal-format name="comma" decimal-separator="," grouping-separator="."/>
<xsl:strip-space elements="*"/>
<xsl:variable name="dateTimeNow" select="current-dateTime()"/>
<xsl:variable name="trace" select="'true'"/>

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

<xsl:template name="trace">
	<xsl:param name="msg" select="'UH, No Message !!'"/>
	<xsl:if test="$trace='true'">
		<xsl:message terminate="no">TRACE: <xsl:value-of select="$msg"/>.</xsl:message>
	</xsl:if>
</xsl:template>

<xsl:template name="lineBreak">
<xsl:text>
</xsl:text>
</xsl:template>

<xsl:template match="/">
	<xsl:call-template name="trace">
		<xsl:with-param name="msg" select="'XML root'"/>
	</xsl:call-template>
	<xsl:apply-templates/>
</xsl:template>

<xsl:template match="text()">
	<!-- This is to eliminate XML text() from the output -->
</xsl:template>

<xsl:template match="head">
	<!-- must be swallowed, as template is called manually -->
</xsl:template>

<xsl:template match="dtbook">
	<xsl:call-template name="trace">
		<xsl:with-param name="msg" select="'dtbook'"/>
	</xsl:call-template>
	<xsl:call-template name="lineBreak"/>
	<urakawaProject>
		<xsl:call-template name="lineBreak"/>
		<xsl:comment>Daisy 3 (z3986) to flat XML, by Daniel WECK</xsl:comment>
		<xsl:call-template name="lineBreak"/>
		<xsl:comment>Generated: <xsl:value-of select="$dateTimeNow"/></xsl:comment>
		<xsl:call-template name="lineBreak"/>
		<xsl:call-template name="metaData"/>
		<xsl:apply-templates/>
	</urakawaProject>
</xsl:template>

<xsl:template name="metaData">
	<xsl:call-template name="trace">
		<xsl:with-param name="msg" select="'metaData'"/>
	</xsl:call-template>
	<metaData>
		<xsl:choose>
			<xsl:when test="head">
				<xsl:choose>
					<xsl:when test="head/title">
						<key name="bookTitle" value="{head/title}"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="trace">
							<xsl:with-param name="msg" select="'Warning, this book has no title'"/>
						</xsl:call-template>
					</xsl:otherwise>
		    	</xsl:choose>
				<xsl:for-each select="head/meta">
					<key name="{@name}" value="{@content}"/>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="trace">
					<xsl:with-param name="msg" select="'Warning, this book has no metaData'"/>
				</xsl:call-template>
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
	<xsl:call-template name="trace">
		<xsl:with-param name="msg" select="'book'"/>
	</xsl:call-template>
	<coreTree>
		<xsl:apply-templates/>
	</coreTree>
</xsl:template>

<xsl:template match="par">
	<xsl:call-template name="trace">
		<xsl:with-param name="msg" select="'par'"/>
	</xsl:call-template>
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
	<xsl:call-template name="trace">
		<xsl:with-param name="msg">catch ALL [<xsl:value-of select="name()"/>]</xsl:with-param>
	</xsl:call-template>
	<node>
		<xsl:variable name="nodeCount" select="count(child::node())"/>
		<xsl:variable name="isMixedContent">
			<xsl:choose>
				<xsl:when test="$nodeCount &gt; 1">
					<xsl:call-template name="trace">
						<xsl:with-param name="msg">=== nodeCount: [<xsl:value-of select="$nodeCount"/>]</xsl:with-param>
					</xsl:call-template>
					<xsl:for-each select="child::node()">
						<xsl:if test="self::text()">
							<xsl:value-of select="position()"/><xsl:value-of select="'-'"/>
						</xsl:if>
					</xsl:for-each>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="''"/>
				</xsl:otherwise>
	    	</xsl:choose>
		</xsl:variable>
		<properties>
			<structure name="{name(.)}">
			<xsl:for-each select="@*">
				<xsl:if test="not (name() = 'id') and not (name() = 'smilref')">
					<xsl:attribute name="{name()}">
						<xsl:value-of select="."/>
					</xsl:attribute>
				</xsl:if>
			</xsl:for-each>
			</structure>
			<xsl:choose>
				<xsl:when test="$isMixedContent=''">
					<medias>
						<xsl:if test="text()">
							<text channel="textFromDTBook" src="{text()}"/>
						</xsl:if>
						<xsl:if test="@smilref">
							<xsl:variable name="smilREF"><xsl:value-of select="@smilref"/></xsl:variable>
							<xsl:for-each select="document($smilREF, @smilref)">
								<xsl:if test="name() = 'par'">
									<xsl:apply-templates select="."/>
								</xsl:if>
							</xsl:for-each>
						</xsl:if>
					</medias>
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="trace">
						<xsl:with-param name="msg">##### MIXED CONTENT [<xsl:value-of select="$isMixedContent"/>]</xsl:with-param>
					</xsl:call-template>
				</xsl:otherwise>
	    	</xsl:choose>
		</properties>
		<xsl:choose>
			<xsl:when test="$isMixedContent=''">
				<xsl:apply-templates/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="trace">
					<xsl:with-param name="msg">+++++ MIXED CONTENT [<xsl:value-of select="$isMixedContent"/>]</xsl:with-param>
				</xsl:call-template>

				<xsl:variable name="smilRef" select="@smilref"/>
				<xsl:variable name="parentID" select="@id"/>
				
				<xsl:for-each select="child::node()">
					<xsl:call-template name="trace">
						<xsl:with-param name="msg">pos: [<xsl:value-of select="position()"/>]</xsl:with-param>
					</xsl:call-template>
					<xsl:choose>
						<xsl:when test="self::text()">
							<xsl:call-template name="textMixedContent">
								<xsl:with-param name="parentID" select="$parentID"/>
								<xsl:with-param name="textPositions" select="$isMixedContent"/>
								<xsl:with-param name="position" select="position()"/>
								<xsl:with-param name="text"><xsl:value-of select="self::text()"/></xsl:with-param>
								<xsl:with-param name="smilRef" select="$smilRef"/>
							</xsl:call-template>
						</xsl:when>
						<xsl:otherwise>
<xsl:call-template name="trace">
<xsl:with-param name="msg">||||| [<xsl:value-of select="name()"/>]</xsl:with-param>
</xsl:call-template>
							<xsl:apply-templates select="."/>
						</xsl:otherwise>
			    	</xsl:choose>
				</xsl:for-each>
			</xsl:otherwise>
    	</xsl:choose>
	</node>
</xsl:template>

<xsl:template name="textMixedContent">
	<xsl:param name="text"/>
	<xsl:param name="parentID"/>
	<xsl:param name="smilRef"/>
	<xsl:param name="textPositions"/>
	<xsl:param name="position"/>
	<xsl:call-template name="trace">
		<xsl:with-param name="msg">*** [<xsl:value-of select="$text"/>]</xsl:with-param>
	</xsl:call-template>
	<xsl:call-template name="trace">
		<xsl:with-param name="msg">*** [<xsl:value-of select="$smilRef"/>]</xsl:with-param>
	</xsl:call-template>
	
	<xsl:variable name="posTranslated">
		<xsl:for-each select="tokenize($textPositions, '-')">
			<xsl:variable name="pos"><xsl:value-of select="."/></xsl:variable>
			<xsl:call-template name="trace">
				<xsl:with-param name="msg">TOKEN : <xsl:value-of select="$pos"/></xsl:with-param>
			</xsl:call-template>

			<xsl:if test="not($pos='')">
				<xsl:if test="$pos=$position">
					<xsl:call-template name="trace">
						<xsl:with-param name="msg">TOKEN POSITION : <xsl:value-of select="position()"/></xsl:with-param>
					</xsl:call-template>
					<xsl:value-of select="position()"/>
				</xsl:if>
			</xsl:if>
		</xsl:for-each>
	</xsl:variable>

	<xsl:call-template name="trace">
		<xsl:with-param name="msg">POS TRANSLATED [<xsl:value-of select="$posTranslated"/>]</xsl:with-param>
	</xsl:call-template>

	<xsl:variable name="parElement">
		<xsl:analyze-string select="$smilRef" regex="(.*\.smil)#(.*)">
			<xsl:matching-substring>
				<xsl:call-template name="trace">
					<xsl:with-param name="msg">@@@ [<xsl:value-of select="regex-group(1)"/>]</xsl:with-param>
				</xsl:call-template>
				<xsl:call-template name="trace">
					<xsl:with-param name="msg">@@@ [<xsl:value-of select="regex-group(2)"/>]</xsl:with-param>
				</xsl:call-template>
				<xsl:for-each select="document(regex-group(1), $smilRef)">					
					<xsl:for-each select="id(regex-group(2))">
						<xsl:call-template name="trace">
							<xsl:with-param name="msg">==============</xsl:with-param>
						</xsl:call-template>
						<xsl:message terminate="no" select="."/>
					</xsl:for-each>
					
					<xsl:variable name="pattern">.*#<xsl:value-of select="$parentID"/></xsl:variable>
					<xsl:call-template name="trace">
						<xsl:with-param name="msg">%%%%%%%%%%% <xsl:value-of select="$pattern"/></xsl:with-param>
					</xsl:call-template>

					<xsl:for-each select="/smil/body/seq/par[child::text[matches(@src, $pattern)]]">
						<xsl:if test="position()=$posTranslated">
							<xsl:call-template name="trace">
								<xsl:with-param name="msg">}}}}}}}} <xsl:value-of select="position()"/></xsl:with-param>
							</xsl:call-template>
							<xsl:message terminate="no" select="."/>
							<xsl:copy-of select="."/>
						</xsl:if>
					</xsl:for-each>
				</xsl:for-each>
			</xsl:matching-substring>
			<xsl:non-matching-substring>
				<!-- xsl:value-of select="."/ -->
			</xsl:non-matching-substring>
		</xsl:analyze-string>
	</xsl:variable>
	
	<node>
		<properties>
			<structure name="textMixedContent"/>
			<medias>
				<text channel="textFromDTBook" src="{$text}"/>
				<!-- audio channel="audioFromSMIL"/ -->
				<xsl:apply-templates select="$parElement"/>
			</medias>
		</properties>
	</node>
</xsl:template>

</xsl:transform>