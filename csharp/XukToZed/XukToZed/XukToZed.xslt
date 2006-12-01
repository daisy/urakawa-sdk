<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xuk="http://www.daisy.org/urakawa/xuk/0.5" xmlns:obi="http://www.daisy.org/urakawa/obi/0.5" xmlns:opf="http://openebook.org/namespaces/oeb-package/1.0/">
  <xsl:output method="xml" indent="yes"/>
  <xsl:include href="X2Z_smil.xslt"/>
  <xsl:include href="X2Z_navmap.xslt"/>
  <xsl:include href="X2Z_package.xslt"/>



  <xsl:template match="/">
    <wrapper>
      <ncx version="2005-1" xmlns="http://www.daisy.org/z3986/2005/ncx/">
        <!-- Does the head, doctitle and docAuthor-->
        <xsl:apply-templates />
        <navMap>
          <xsl:apply-templates mode="NAVMAP" />
        </navMap>
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
    <xsl:message terminate="no" >Processing <xsl:value-of select="name()"/> on MEDIAFILES</xsl:message>
    <xsl:apply-templates mode="MEDIAFILES" />
  </xsl:template>

  <xsl:template match="xuk:AudioMedia" mode="MEDIAFILES">
    <xsl:if test="@src != (following::xuk:AudioMedia/@src)[1] or ((boolean((following::xuk:AudioMedia/@src)[1])=false) and (@src != (preceding::xuk:AudioMedia/@src)[1]))">
      <!-- if the file name after this one is different
           OR
           (there is no filename after this one AND the preceeding is different)
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