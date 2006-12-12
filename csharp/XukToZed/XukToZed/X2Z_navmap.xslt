<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xuk="http://www.daisy.org/urakawa/xuk/0.5" xmlns:obi="http://www.daisy.org/urakawa/obi" >
  <!-- Building the NAVMAP-->

  <xsl:template match="obi:section" mode="NAVMAP">
    <xsl:choose>
      <xsl:when test="@used='false'">
        <xsl:comment>Not using <xsl:value-of select="generate-id(.)"/>
      </xsl:comment>
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="xuk:mProperties/xuk:ChannelsProperty/xuk:ChannelMapping[@channel='CHID0001']/xuk:TextMedia/text()" >
            <navTarget xmlns="http://www.daisy.org/z3986/2005/ncx/">
              <xsl:for-each select="xuk:mProperties/xuk:ChannelsProperty/xuk:ChannelMapping[@channel='CHID0001']/xuk:TextMedia/text()">
                <navLabel>
                  <text>
                    <xsl:value-of select="."/>
                  </text>
                  <audio>
                    <xsl:for-each select="following::xuk:AudioMedia[1]">
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
                  <xsl:attribute name="src"><xsl:value-of select="generate-id((ancestor-or-self::obi:*[self::obi:section or preceding-sibling::obi:section][1]))"/>.smil#<xsl:value-of select ="generate-id(ancestor-or-self::obi:*[1])"/></xsl:attribute>
                </content>
              </xsl:for-each>
              <xsl:apply-templates mode="NAVMAP"/>
            </navTarget>
          </xsl:when>
          <xsl:otherwise>
            <xsl:message terminate="no">Skipping <xsl:value-of select="name()"/></xsl:message>
            <xsl:apply-templates mode="NAVMAP" />
          </xsl:otherwise>
        </xsl:choose>
        
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="*" mode="NAVMAP" >
    <xsl:message terminate="no" >Processing <xsl:value-of select="name()"/> on NAVMAP</xsl:message>
    <xsl:apply-templates mode="NAVMAP"/>
  </xsl:template>

  <xsl:template match="text()" mode="NAVMAP">
    <xsl:apply-templates mode="NAVMAP"/>
  </xsl:template>

</xsl:stylesheet>