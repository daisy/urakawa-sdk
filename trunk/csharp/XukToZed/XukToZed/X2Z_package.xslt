<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"  xmlns:xuk="http://www.daisy.org/urakawa/xuk/0.5" xmlns:obi="http://www.daisy.org/urakawa/obi" xmlns:opf="http://openebook.org/namespaces/oeb-package/1.0/">
  <xsl:include href="X2Z_manifest.xslt"/>

  <xsl:param name="packageFilename">package.opf</xsl:param>
  <xsl:param name="ncxFilename">navigation.ncx</xsl:param>
  <xsl:param name="unique-identifier">PackageUID</xsl:param>
  <xsl:param name="dcId">
    <xsl:choose>
      <xsl:when test="/xuk:XUK/xuk:ProjectMetadata/xuk:Metadata[@name='dc:Identifier']/@content">
        <xsl:value-of select="/xuk:XUK/xuk:ProjectMetadata/xuk:Metadata[@name='dc:Identifier']/@content"/>
      </xsl:when>
      <xsl:otherwise>NO_ID_SPECIFIED_IN_XUK</xsl:otherwise>
    </xsl:choose>
  </xsl:param>
  
  <xsl:template match="xuk:XUK" mode="PACKAGE">
    <package xmlns="http://openebook.org/namespaces/oeb-package/1.0/" unique-identifier="{$unique-identifier}">
      <metadata>
        <dc-metadata xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:oebpackage="http://openebook.org/namespaces/oeb-package/1.0/">
          <xsl:for-each select="xuk:ProjectMetadata/xuk:Metadata[contains(@name,'dc:') and @name!='dc:Identifier']">
            <xsl:element name="{@name}"><xsl:value-of select="@content"/></xsl:element>
          </xsl:for-each>
          <dc:Identifier id="{$unique-identifier}"><xsl:value-of select="$dcId"/></dc:Identifier>
        </dc-metadata>
        <xsl:if test="xuk:ProjectMetadata/xuk:Metadata[not(contains(@name,'dc:'))]">
          <x-metadata>
            <xsl:for-each select="xuk:ProjectMetadata/xuk:Metadata[not(contains(@name,'dc:'))]">
              <meta>
                <xsl:copy-of select="@*"  />
              </meta>
            </xsl:for-each>
            <meta name="dtb:multimediaType" content="audioNCX" />
            <meta name="dtb:multimediaContent" content="audio" />
          </x-metadata>
        </xsl:if>
      </metadata>
      <manifest>
        <item id="NCX" href="{$ncxFilename}" media-type="application/x-dtbncx+xml" />
        <item id="PACKAGE" href="{$packageFilename}" media-type="text/xml" />
        <xsl:apply-templates mode="MANIFEST" />
      </manifest>
      <spine>
        <!-- xsl:for-each select="//xuk:CoreNode[xuk:mProperties/obi:info[@type='Section'] | preceding-sibling::xuk:mProperties/obi:info[@type='Section'][1]]" -->
        <xsl:for-each select="//obi:*[self::obi:section | preceding-sibling::obi:section[1]]" >
          <itemref  >
            <xsl:attribute name="idref">SMIL_<xsl:value-of select="generate-id(.)"/></xsl:attribute>
          </itemref>
        </xsl:for-each>
      </spine>
    </package>
  </xsl:template>

  
  
  <xsl:template match="*" mode="PACKAGE" >
    <xsl:message terminate="no" >Processing <xsl:value-of select="name()"/> on PACKAGE</xsl:message>
    <xsl:apply-templates mode="PACKAGE"/>
  </xsl:template>

  <xsl:template match="text()" mode="PACKAGE">
    <xsl:apply-templates mode="PACKAGE"/>
  </xsl:template>
</xsl:stylesheet>