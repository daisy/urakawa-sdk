<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="2.0">

<xsl:output encoding="utf-8" method="xml" indent="yes" version="1.0" omit-xml-declaration="no"/>
<xsl:output doctype-public="DOCTYPE_PUBLIC-URAKAWA" doctype-system="DOCTYPE_SYSTEM-URAKAWA"/>

<xsl:decimal-format name="comma" decimal-separator="," grouping-separator="."/>
<xsl:strip-space elements="*"/>

<!-- Date and Time of generation -->
<xsl:variable name="dateTimeNow" select="current-dateTime()"/>

<!-- Debug trace ('true' == TRUE, anything else is FALSE) -->
<xsl:variable name="trace" select="'true'"/>

<!-- Include original SMIL and DTBook IDs ('true' == TRUE, anything else is FALSE) -->
<xsl:variable name="includeIDs" select="'true'"/>

<!--
	This XSLT stylesheet transforms a Daisy3 book (DTBook and SMIL files) to a single flat XML file, by merging the content into a structure-centric tree (as opposed, for example, to a SMIL-centric tree).
	
### Example of use:
	
1) Download SAXON (XSLT 2.0 processor):
http://prdownloads.sourceforge.net/saxon/saxonb8-6-1.zip
or
http://saxon.sourceforge.net/

2) extract saxon8.jar in your local repository 'urakawa/architecture/sandbox/'

3) extract SampleDTB.zip in 'urakawa/architecture/sandbox/SampleDTB/'

4) Command line:
cd urakawa/architecture/sandbox/
java -jar saxon8.jar -o Urakawa_DaisyToXML_OUTPUT_TEST.xml SampleDTB/SampleDTB.xml Urakawa_DaisyToXML.xsl
-->

<!-- =================================== -->
<!-- UTILITIES -->

<!-- Function that displays a message on the standard output (usually the console), unless the global variable [$trace] is set to anything else than 'true' (the variable is at the top of this file) -->
<xsl:template name="trace">
	<xsl:param name="msg" select="'UH, No Message !!'"/>
	<xsl:if test="$trace='true'">
		<xsl:message terminate="no">TRACE: <xsl:value-of select="$msg"/>.</xsl:message>
	</xsl:if>
</xsl:template>

<!-- Inserts a line break in the result tree -->
<xsl:template name="lineBreak">
<xsl:text>
</xsl:text>
</xsl:template>

<!-- =================================== -->
<!-- GENERAL TEMPLATES -->

<!-- This root template will be called by both SMIL and DTBook parsing...so it's not generating anything, it just forwards to the root element -->
<xsl:template match="/">
	<xsl:call-template name="trace">
		<xsl:with-param name="msg" select="'XML root'"/>
	</xsl:call-template>
	<xsl:apply-templates/>
</xsl:template>

<!-- nodes of type NODE_TEXT must be swallowed , as they are treated separately depending of their context -->
<xsl:template match="text()">
</xsl:template>

<!-- =================================== -->
<!-- DTBOOK TEMPLATES -->

<!-- The root element of the DTBook -->
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

<!-- The root of the actual book structure -->
<xsl:template match="book">
	<xsl:call-template name="trace">
		<xsl:with-param name="msg" select="'book'"/>
	</xsl:call-template>
	<coreTree>
		<xsl:apply-templates/>
	</coreTree>
</xsl:template>

<!-- The head element is bypassed, as it is processed manually by the 'metaData' template. We do this to make sure that the meta-data appears at the top of the resulting XML -->
<xsl:template match="head">
</xsl:template>

<!-- This template mainly looks at the 'head' element to extract meta-data, but also uses information coming from the 'book' element (like language) -->
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

<!-- This function should be used inside a &lt;xsl:variable/&gt; variable declaration, as it calculates a string which represents if the given DTBook element has mixed content or not. An empty string means there's no mixed content, and a sequence of numbers like "1-3-5-" means that child nodes at position 1, 3, and 5 are of type NODE_TEXT. -->
<xsl:template name="calculateMixedContentCode">
	<xsl:variable name="nodeCount" select="count(child::node())"/>
	<xsl:choose>
		<xsl:when test="$nodeCount > 1">
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
</xsl:template>

<!-- This templates generates the Structure node which contains structural properties extracted from the DTBook. The is a flag (see global variables at the top of this file) to include/exclude ID attributes (like 'id' and 'smilref') -->
<xsl:template name="generateStructureProperties">
	<structure name="{name(.)}">
	<xsl:for-each select="@*">		
		<xsl:if test="$includeIDs = 'true' or (not (name() = 'id') and not (name() = 'smilref'))">
			<xsl:attribute name="{name()}"><xsl:value-of select="."/></xsl:attribute>
		</xsl:if>
	</xsl:for-each>
	</structure>
</xsl:template>

<!-- This templates generates the Media properties node for non-mixed-content. It actually relays some SMIL parsing work to the template that matches the "par" element -->
<xsl:template name="generateMediaProperties_NotMixedContent">
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
</xsl:template>

<!-- This templates generates the intermediate Nodes for mixed-content, by calling the template "generateIntermediateMixedContent" on NODE_TEXT XML nodes-->
<xsl:template name="generateMediaProperties_MixedContent">
	<xsl:param name="mixedContentCode" select="''"/>

	<xsl:call-template name="trace">
		<xsl:with-param name="msg">+++++ MIXED CONTENT [<xsl:value-of select="$mixedContentCode"/>]</xsl:with-param>
	</xsl:call-template>

	<xsl:variable name="smilRef" select="@smilref"/>
	<xsl:variable name="parentID" select="@id"/>

	<xsl:for-each select="child::node()">
		<xsl:call-template name="trace">
			<xsl:with-param name="msg">pos: [<xsl:value-of select="position()"/>]</xsl:with-param>
		</xsl:call-template>
		<xsl:choose>
			<xsl:when test="self::text()">
				<xsl:call-template name="generateIntermediateMixedContent">
					<xsl:with-param name="parentID" select="$parentID"/>
					<xsl:with-param name="textPositions" select="$mixedContentCode"/>
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
</xsl:template>

<!-- The "catch all" template takes care of all possible other elements in the DTBook. Basically, it replicates the structure and merges it with the SMIL content, in order to obtain a fully synchronized audio/text book represented by a root node of generic type 'node' -->
<xsl:template match="*">
	<xsl:call-template name="trace">
		<xsl:with-param name="msg">catch ALL [<xsl:value-of select="name()"/>]</xsl:with-param>
	</xsl:call-template>
	<node>
		<xsl:variable name="mixedContentCode">
			<xsl:call-template name="calculateMixedContentCode"/>
		</xsl:variable>
		<properties>
			<xsl:call-template name="generateStructureProperties"/>
			<xsl:choose>
				<xsl:when test="$mixedContentCode=''">
					<xsl:call-template name="generateMediaProperties_NotMixedContent"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="trace">
						<xsl:with-param name="msg">##### MIXED CONTENT [<xsl:value-of select="$mixedContentCode"/>]</xsl:with-param>
					</xsl:call-template>
				</xsl:otherwise>
	    	</xsl:choose>
		</properties>
		<xsl:choose>
			<xsl:when test="$mixedContentCode=''">
				<xsl:apply-templates/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="generateMediaProperties_MixedContent">
					<xsl:with-param name="mixedContentCode" select="$mixedContentCode"/>
				</xsl:call-template>
			</xsl:otherwise>
    	</xsl:choose>
	</node>
</xsl:template>

<!-- This template applies to a node of type NODE_TEXT. The parsing algorithm will look at the SMIL content and systematically deduct what audio is synchronized with the text. -->
<xsl:template name="generateIntermediateMixedContent">
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
	
	<xsl:variable name="smilFile" select="substring-before($smilRef, '#')"/>
	<xsl:variable name="smilAnchor" select="substring-after($smilRef, '#')"/>
	
	<xsl:call-template name="trace">
		<xsl:with-param name="msg">@@1@ [<xsl:value-of select="$smilFile"/>]</xsl:with-param>
	</xsl:call-template>
	<xsl:call-template name="trace">
		<xsl:with-param name="msg">@@2@ [<xsl:value-of select="$smilAnchor"/>]</xsl:with-param>
	</xsl:call-template>
	<xsl:for-each select="document($smilFile, $smilRef)">					
		<xsl:for-each select="id($smilAnchor)">
			<xsl:call-template name="trace">
				<xsl:with-param name="msg">==============</xsl:with-param>
			</xsl:call-template>
		</xsl:for-each>
		
		<xsl:variable name="pattern">.*#<xsl:value-of select="$parentID"/></xsl:variable>
		<xsl:call-template name="trace">
			<xsl:with-param name="msg">%%%%%%%%%%% <xsl:value-of select="$pattern"/></xsl:with-param>
		</xsl:call-template>

		<xsl:choose>
			<xsl:when test="$posTranslated > count(/smil/body//text[matches(@src, $pattern)]/following-sibling::seq/audio)">
				<xsl:for-each select="/smil/body//text[matches(@src, $pattern)]/following-sibling::seq/audio">
					<xsl:if test="position()=last()">
						<xsl:call-template name="trace">
							<xsl:with-param name="msg">}}}}}}}1} <xsl:value-of select="position()"/></xsl:with-param>
						</xsl:call-template>
						<xsl:call-template name="audioWrapper">
							<xsl:with-param name="text" select="$text"/>
						</xsl:call-template>
					</xsl:if>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<xsl:for-each select="/smil/body//text[matches(@src, $pattern)]/following-sibling::seq/audio">
					<xsl:if test="position()=$posTranslated">
						<xsl:call-template name="trace">
							<xsl:with-param name="msg">}}}}}}}2} <xsl:value-of select="position()"/></xsl:with-param>
						</xsl:call-template>
						<xsl:call-template name="audioWrapper">
							<xsl:with-param name="text" select="$text"/>
						</xsl:call-template>
					</xsl:if>
				</xsl:for-each>
			</xsl:otherwise>
    	</xsl:choose>
		
		<!-- xsl:for-each select="/smil/body/seq/par[child::text[matches(@src, $pattern)]]">
			<xsl:if test="position()=$posTranslated">
				<xsl:call-template name="trace">
					<xsl:with-param name="msg">}}}}}}}} <xsl:value-of select="position()"/></xsl:with-param>
				</xsl:call-template>
				<xsl:message terminate="no" select="."/>
				<xsl:copy-of select="."/>
			</xsl:if>
		</xsl:for-each -->
	</xsl:for-each>
	
	<!-- xsl:analyze-string select="$smilRef" regex="(.*\.smil)#(.*)">
		<xsl:matching-substring>
			<xsl:call-template name="trace">
				<xsl:with-param name="msg">@@1@ [<xsl:value-of select="regex-group(1)"/>]</xsl:with-param>
			</xsl:call-template>
			<xsl:call-template name="trace">
				<xsl:with-param name="msg">@@2@ [<xsl:value-of select="regex-group(2)"/>]</xsl:with-param>
			</xsl:call-template>
		</xsl:matching-substring>
		<xsl:non-matching-substring>
		</xsl:non-matching-substring>
	</xsl:analyze-string -->
	
</xsl:template>

<!-- =================================== -->
<!-- SMIL TEMPLATES -->

<!-- This template generates the actual audio media elements for the Media properties of a Node (text media channel was deducted from DTBook, this is a SMIL template). It parses a SMIL 'par' element, and flattens 'audio' elements (video support for later) -->
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
	<xsl:variable name="parID">
		<xsl:value-of select="@id"/>
	</xsl:variable>
	<xsl:for-each select="seq">
		<xsl:choose>
			<xsl:when test="count(current()/*) > 1">
				<sequence>
					<xsl:for-each select="audio">
						<xsl:apply-templates select=".">
							<xsl:with-param name="parID" select="$parID"/>
						</xsl:apply-templates>
					</xsl:for-each>
				</sequence>
			</xsl:when>
			<xsl:otherwise>
				<xsl:for-each select="audio">
					<xsl:apply-templates select=".">
						<xsl:with-param name="parID" select="$parID"/>
					</xsl:apply-templates>
				</xsl:for-each>
			</xsl:otherwise>
    	</xsl:choose>
	</xsl:for-each>
</xsl:template>

<!-- This template replicates the audio element -->
<xsl:template match="audio">
	<xsl:param name="parID" select="'parID'"/>
	<audio channel="audioFromSMIL" src="{@src}" clipBegin="{@clipBegin}" clipEnd="{@clipEnd}">
		<xsl:if test="$includeIDs = 'true' and not ($parID = '')">
			<xsl:attribute name="smilRefID" select="$parID"/>
		</xsl:if>
	</audio>
</xsl:template>

<!-- This template wraps the audio element -->
<xsl:template name="audioWrapper">
	<xsl:param name="text" select="'xxx'"/>
	<xsl:message terminate="no" select="."/>

	<xsl:variable name="parID" select="parent::seq/parent::par/@id"/>
	<xsl:message terminate="no" select="$parID"/>

	<node>
		<properties>
			<structure name="textMixedContent"/>
			<medias>
				<text channel="textFromDTBook" src="{$text}"/>
				<xsl:call-template name="trace">
					<xsl:with-param name="msg" select="'yyyyy'"/>
				</xsl:call-template>
				<xsl:message terminate="no" select="."/>
				<xsl:apply-templates select=".">
					<xsl:with-param name="parID" select="$parID"/>
				</xsl:apply-templates>
			</medias>
		</properties>
	</node>
</xsl:template>

<!-- xsl:template name="audio_">
	<xsl:param name="parID" select="'parID'"/>
	<xsl:param name="audioElement" select="."/>

	<xsl:call-template name="trace">
		<xsl:with-param name="msg" select="'zzzzz'"/>
	</xsl:call-template>
	<xsl:message terminate="no" select="$audioElement"/>

	<xsl:for-each select="$audioElement">
			<xsl:variable name="src"><xsl:value-of select="@src"/></xsl:variable>
	<audio channel="audioFromSMIL" src="{$src}" clipBegin="{@clipBegin}" clipEnd="{@clipEnd}">
		<xsl:if test="$includeIDs = 'true' and not ($parID = '')">
			<xsl:attribute name="smilRefID" select="$parID"/>
		</xsl:if>
	</audio>
	</xsl:for-each>
</xsl:template -->

</xsl:transform>