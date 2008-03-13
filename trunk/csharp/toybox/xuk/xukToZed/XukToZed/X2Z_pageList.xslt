<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xuk="http://www.daisy.org/urakawa/xuk/0.5" xmlns:obi="http://www.daisy.org/urakawa/obi" >
  <!-- Building the PAGELIST-->

  <xsl:template match="obi:page" mode="PAGELIST">
    <pageTarget xmlns="http://www.daisy.org/z3986/2005/ncx/">
      <xsl:attribute name="playOrder">
        <xsl:value-of select="count(preceding::obi:section[count(ancestor-or-self::obi:*[@used='False'])=0]|preceding::obi:page[count(ancestor-or-self::obi:*[@used='False'])=0]|ancestor-or-self::obi:section[count(ancestor-or-self::obi:*[@used='False'])=0]|ancestor-or-self::obi:page[count(ancestor-or-self::obi:*[@used='False'])=0])"/>
      </xsl:attribute>
      <navLabel>
        <text>
          <xsl:value-of select="@num"/>
        </text>
        <audio>
          <xsl:for-each select="preceding::xuk:AudioMedia[1]">
            <xsl:attribute name="src" >
              <xsl:value-of select="@src" />
            </xsl:attribute>
            <xsl:attribute name="clipBegin" >
              <xsl:value-of select="@clipBegin" />
            </xsl:attribute>
            <xsl:attribute name="clipEnd" >
              <xsl:value-of select="@clipEnd" />
            </xsl:attribute>
          </xsl:for-each>
        </audio>
      </navLabel>
      <content>
        <xsl:attribute name="src">
          <xsl:value-of select="generate-id((ancestor-or-self::obi:*[self::obi:section or preceding-sibling::obi:section][1]))"/>.smil#<xsl:value-of select ="generate-id(preceding::xuk:AudioMedia[1]/ancestor::obi:phrase[1])"/>
        </xsl:attribute>
      </content>
    </pageTarget>
  </xsl:template>

  <xsl:template match="*" mode="PAGELIST" >
    <xsl:message terminate="no" >
      Processing <xsl:value-of select="name()"/> on PAGELIST
    </xsl:message>
    <xsl:choose>
      <xsl:when test="@used='False'">
        <xsl:comment>
          Not using <xsl:value-of select="generate-id(.)"/>
        </xsl:comment>
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates mode="PAGELIST"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="text()" mode="PAGELIST">
    <xsl:apply-templates mode="PAGELIST"/>
  </xsl:template>

</xsl:stylesheet>