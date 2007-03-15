<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xuk="http://www.daisy.org/urakawa/xuk/0.5" xmlns:obi="http://www.daisy.org/urakawa/obi" >

  <xsl:template name="MakeSmilCoreNode">
    <xsl:choose >
      <xsl:when test="xuk:mProperties/xuk:ChannelsProperty/xuk:ChannelMapping" >
        <seq xmlns="http://www.w3.org/2001/SMIL20/Language">
          <xsl:attribute name="id">
            <xsl:value-of select="generate-id(.)"/>
          </xsl:attribute>
          <xsl:apply-templates mode="SMIL" />
        </seq>
      </xsl:when>
      <xsl:otherwise>
        <xsl:comment>
          Not including <xsl:value-of select="generate-id(.)"/> in SMIL
        </xsl:comment>
        <xsl:apply-templates mode="SMIL"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="obi:*" mode="SMIL" >
    <xsl:choose>
      <xsl:when test="@used='false'">
        <xsl:comment>Not using <xsl:value-of select="generate-id(.)"/></xsl:comment>
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="(self::obi:section | preceding-sibling::obi:section[1])" >
            <xsl:comment>started a newfile tag here</xsl:comment>
            <smil xmlns="http://www.w3.org/2001/SMIL20/Language" filename="{generate-id(.)}">
              <head>
                <meta name="dtb:uid" content="{$dcId}" />
                <meta name="dtb:generator" content="XukToZed for Obi 0.7" />
                <meta name="dtb:totalElapsedTime" content="."/>
              </head>
              <body>
                <xsl:call-template name="MakeSmilCoreNode" />
              </body>
            </smil>
            <xsl:comment>ended a newfile tag here</xsl:comment>
          </xsl:when>
          <xsl:otherwise>
            <xsl:call-template name="MakeSmilCoreNode" />
          </xsl:otherwise>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="xuk:SequenceMedia" mode="SMIL" >
    <seq xmlns="http://www.w3.org/2001/SMIL20/Language">
      <xsl:attribute name="id">
        <xsl:value-of select="generate-id(.)"/>
      </xsl:attribute>
      <xsl:apply-templates mode="SMIL" />
    </seq>
  </xsl:template>

  <xsl:template match="xuk:AudioMedia" mode="SMIL" >
    <audio xmlns="http://www.w3.org/2001/SMIL20/Language">
      <xsl:attribute name="id">
        <xsl:value-of select="generate-id(.)"/>
      </xsl:attribute>
      <xsl:copy-of select="@*"/>
    </audio>
  </xsl:template>


  <xsl:template match="*" mode="SMIL" >
    <xsl:message terminate="no" >Processing <xsl:value-of select="name()"/> on SMIL</xsl:message>
    <xsl:apply-templates mode="SMIL" />
  </xsl:template>

  <xsl:template match="text()" mode="SMIL">
    <xsl:apply-templates mode="SMIL"/>
  </xsl:template>

</xsl:stylesheet>