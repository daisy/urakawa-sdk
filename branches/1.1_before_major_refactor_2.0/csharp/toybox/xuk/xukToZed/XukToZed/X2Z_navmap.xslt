<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xuk="http://www.daisy.org/urakawa/xuk/0.5" xmlns:obi="http://www.daisy.org/urakawa/obi" >
  <!-- Building the NAVMAP-->

  <xsl:template match="obi:section" mode="NAVMAP">
    <xsl:choose>
      <xsl:when test="@used='False'">
        <xsl:comment>Not using <xsl:value-of select="generate-id(.)"/>
      </xsl:comment>
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="xuk:mProperties/xuk:ChannelsProperty/xuk:ChannelMapping[@channel='CHID0001']/xuk:TextMedia/text()" >
            <navPoint xmlns="http://www.daisy.org/z3986/2005/ncx/">
              <xsl:attribute name="playOrder">
                <xsl:value-of select="count(preceding::obi:section[count(ancestor-or-self::obi:*[@used='False'])=0]|preceding::obi:page[count(ancestor-or-self::obi:*[@used='False'])=0]|ancestor-or-self::obi:section[count(ancestor-or-self::obi:*[@used='False'])=0]|ancestor-or-self::obi:page[count(ancestor-or-self::obi:*[@used='False'])=0])"/>
              </xsl:attribute>
              <xsl:for-each select="xuk:mProperties/xuk:ChannelsProperty/xuk:ChannelMapping[@channel='CHID0001']/xuk:TextMedia/text()">
                <navLabel>
                  <text>
                    <xsl:value-of select="."/>
                  </text>
                  <audio>
                    <xsl:choose>
                      <xsl:when test="ancestor::obi:section/xuk:mChildren/obi:phrase[@heading='True']">
                        <xsl:apply-templates mode="get-audio"
                          select="ancestor::obi:section/xuk:mChildren/obi:phrase[@heading='True'][1]//xuk:AudioMedia[1]"/>
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:apply-templates mode="get-audio" select="following::xuk:AudioMedia[1]"/>
                      </xsl:otherwise>
                    </xsl:choose>
                  </audio>
                </navLabel>
                <content>
                  <xsl:attribute name="src"><xsl:value-of select="generate-id((ancestor-or-self::obi:*[self::obi:section or preceding-sibling::obi:section][1]))"/>.smil#<xsl:value-of select ="generate-id(ancestor-or-self::obi:*[1])"/></xsl:attribute>
                </content>
              </xsl:for-each>
              <xsl:apply-templates mode="NAVMAP"/>
            </navPoint>
          </xsl:when>
          <xsl:otherwise>
            <xsl:message terminate="no">Skipping <xsl:value-of select="name()"/></xsl:message>
            <xsl:apply-templates mode="NAVMAP" />
          </xsl:otherwise>
        </xsl:choose>
        
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="xuk:AudioMedia" mode="get-audio">
    <xsl:copy-of select="@src"/>
    <xsl:copy-of select="@clipBegin"/>
    <xsl:copy-of select="@clipEnd"/>
  </xsl:template>
  
  <xsl:template match="*" mode="NAVMAP" >
    <xsl:message terminate="no" >Processing <xsl:value-of select="name()"/> on NAVMAP</xsl:message>
    <xsl:apply-templates mode="NAVMAP"/>
  </xsl:template>

  <xsl:template match="text()" mode="NAVMAP">
    <xsl:apply-templates mode="NAVMAP"/>
  </xsl:template>

</xsl:stylesheet>