<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xuk="http://www.daisy.org/urakawa/xuk/0.5" xmlns:obi="http://www.daisy.org/urakawa/obi" >
  <!-- Building the MANIFEST-->

  <xsl:template match="*[@src]" mode="MANIFEST">
    <item xmlns="http://openebook.org/namespaces/oeb-package/1.0/">
      <xsl:attribute name="id">
        <xsl:choose>
          <xsl:when test="contains(@src,'.wav') or contains(@src,'.mp3')">aud_<xsl:value-of select="translate(@src,'.','_')"/></xsl:when>
          <xsl:otherwise>TROUBLE_<xsl:value-of select="@src"/></xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <xsl:attribute name="href">
        <xsl:choose>
          <xsl:when test="@src"><xsl:value-of select="@src"/></xsl:when>
          <xsl:otherwise>TROUBLE_<xsl:value-of select="@src"/></xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <xsl:attribute name="media-type">
        <xsl:choose>
          <xsl:when test="contains(@src,'.wav')">audio/x-wav</xsl:when>
          <xsl:when test="contains(@src,'.mp3')">audio/mpeg</xsl:when>
          <xsl:otherwise>TROUBLE_<xsl:value-of select="@src"/></xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
    </item>
    <xsl:apply-templates mode="MANIFEST" />
  </xsl:template>

  <!-- xsl:template match="xuk:CoreNode[xuk:mProperties/obi:info[@type='Section'] | preceding-sibling::xuk:mProperties/obi:info[@type='Section'][1]]" mode="MANIFEST" -->
  <xsl:template match="obi:*[self::obi:section | preceding-sibling::obi:section[1]]" mode="MANIFEST">
    <item xmlns="http://openebook.org/namespaces/oeb-package/1.0/">
      <!-- xsl:attribute name="id">
        SMIL_<xsl:value-of select="generate-id((ancestor-or-self::xuk:CoreNode[xuk:mProperties/obi:info[@type='Section'] or preceding-sibling::xuk:CoreNode[xuk:mProperties/obi:info[@type='Section']]][1]))"/>
      </xsl:attribute -->
      <xsl:attribute name="id">SMIL_<xsl:value-of select="generate-id((ancestor-or-self::obi:*[self::obi:section or preceding-sibling::obi:section][1]))"/></xsl:attribute>

      <!-- xsl:attribute name="href"><xsl:value-of select="generate-id((ancestor-or-self::xuk:CoreNode[xuk:mProperties/obi:info[@type='Section'] or preceding-sibling::xuk:CoreNode[xuk:mProperties/obi:info[@type='Section']]][1]))"/>.smil</xsl:attribute -->
      <xsl:attribute name="href"><xsl:value-of select="generate-id((ancestor-or-self::obi:*[self::obi:section or preceding-sibling::obi:section][1]))"/>.smil</xsl:attribute>

      <xsl:attribute name="media-type">application/smil</xsl:attribute>
    </item>
    <xsl:apply-templates mode="MANIFEST" />
  </xsl:template>

  <xsl:template match="*" mode="MANIFEST" >
    <xsl:message terminate="no" >Processing <xsl:value-of select="name()"/> on MANIFEST</xsl:message>
    <xsl:apply-templates mode="MANIFEST"/>
  </xsl:template>

  <xsl:template match="text()" mode="MANIFEST">
    <xsl:apply-templates mode="MANIFEST"/>
  </xsl:template>

</xsl:stylesheet>