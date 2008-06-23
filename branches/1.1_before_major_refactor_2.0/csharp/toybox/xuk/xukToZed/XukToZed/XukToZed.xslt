<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xuk="http://www.daisy.org/urakawa/xuk/0.5" xmlns:obi="http://www.daisy.org/urakawa/obi" xmlns:opf="http://openebook.org/namespaces/oeb-package/1.0/">
  <xsl:output method="xml" indent="yes"/>
  <xsl:include href="X2Z_smil.xslt"/>
  <xsl:include href="X2Z_navmap.xslt"/>
  <xsl:include href="X2Z_pageList.xslt"/>
  <xsl:include href="X2Z_package.xslt"/>
  <xsl:param name="dcDate" >UNSPECIFIED</xsl:param>

  <xsl:template match="/">
    <wrapper>
      <xsl:variable name="maxDepth">
        <xsl:variable name="depths">
          <xsl:for-each select="(//obi:section)">
            <xsl:sort data-type="number" order="descending" select="count(ancestor-or-self::obi:section)"/>
              <xsl:value-of select="count(ancestor-or-self::obi:section)"/><xsl:text>:</xsl:text>
          </xsl:for-each>
        </xsl:variable>
        <xsl:value-of select="substring-before($depths,':')"/>
      </xsl:variable>
      <ncx version="2005-1" xmlns="http://www.daisy.org/z3986/2005/ncx/">
        <head>
          <meta name="dtb:uid" content="{$dcId}"/>
          <meta name="dtb:depth" content="{$maxDepth}"/>
          <meta name="dtb:generator" content="XukToZed for Obi 0.7"/>
          <meta name="dtb:maxPageNumber" content="0" />
          <meta name="dc:Date" content="{$dcDate}" />
        </head>
        <!-- Does the head, doctitle and docAuthor-->
        <xsl:apply-templates />
        <navMap>
          <xsl:apply-templates mode="NAVMAP" />
        </navMap>
        <pageList>
          <xsl:apply-templates mode="PAGELIST" />
        </pageList>
      </ncx>
      <smil>
        <xsl:apply-templates mode="SMIL" />
      </smil>
      <filenames>
        <xsl:apply-templates mode="MEDIAFILES" />
      </filenames>
      <xsl:apply-templates mode="PACKAGE" />
    </wrapper>
  </xsl:template>


  <!-- Building the MEDIAFILES -->

  <xsl:param name="mediaFileLocation">
    <xsl:choose>
      <xsl:when test="/xuk:XUK/xuk:ProjectMetadata/xuk:Metadata[@name='obi:assetsdir']/@content"><xsl:value-of select="/xuk:XUK/xuk:ProjectMetadata/xuk:Metadata[@name='obi:assetsdir']/@content" /></xsl:when>
      <xsl:otherwise>.</xsl:otherwise>
    </xsl:choose>
  </xsl:param>

  <xsl:template match="*" mode="MEDIAFILES" >
    <xsl:choose>
      <xsl:when test="ancestor-or-self::obi:*[@used='False']">
        <xsl:comment>
          Not using <xsl:value-of select="generate-id(.)"/>
        </xsl:comment>
      </xsl:when>
      <xsl:otherwise>
        <xsl:message terminate="no" >
          Processing <xsl:value-of select="name()"/> on MEDIAFILES
        </xsl:message>
        <xsl:apply-templates mode="MEDIAFILES" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="xuk:AudioMedia" mode="MEDIAFILES">
    <xsl:if test="@src != (following::xuk:AudioMedia[count(ancestor-or-self::obi:*[@used='False'])=0]/@src)[1] or (count((following::xuk:AudioMedia[count(ancestor-or-self::obi:*[@used='False'])=0]/@src)[1])=0)">
      <!-- if the file name after this one is different
           OR
           this is the last audio reference
       -->
      <file><xsl:value-of select="$mediaFileLocation"/>/<xsl:value-of select ="@src"/></file>
    </xsl:if>
  </xsl:template>
  


  <!-- simple forwarding -->

  <xsl:template match="*" >
    <xsl:message terminate="no" >Processing <xsl:value-of select="name()"/> on *</xsl:message>
    <xsl:apply-templates />
  </xsl:template>

  <!-- geting rid of default text handling -->
  <xsl:template match="text()" mode="MEDIAFILES">
    <xsl:apply-templates mode="MEDIAFILES"/>
  </xsl:template>

  <xsl:template match="text()">
    <xsl:apply-templates/>
  </xsl:template>


</xsl:stylesheet>