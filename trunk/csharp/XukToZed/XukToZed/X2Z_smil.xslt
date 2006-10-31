<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xuk="http://www.daisy.org/urakawa/xuk/0.5" xmlns:obi="http://www.daisy.org/urakawa/obi/0.5" >
  <!-- Building the SMIL-->
  <xsl:template match="xuk:CoreNode" mode="SMIL_DISABLED" >
    <xsl:choose >
      <xsl:when test="xuk:mProperties/xuk:ChannelsProperty/xuk:ChannelMapping" >
        <seq>
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


  <xsl:template name="MakeSmilCoreNode">
    <xsl:choose >
      <xsl:when test="xuk:mProperties/xuk:ChannelsProperty/xuk:ChannelMapping" >
        <seq>
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

  <xsl:template match="xuk:CoreNode" mode="SMIL" >
    <xsl:choose>
      <xsl:when test="(xuk:mProperties/obi:info[@type='Section'] | preceding-sibling::xuk:mProperties/obi:info[@type='Section'][1])">
        <xsl:comment>started a newfile tag here</xsl:comment>
        <smil xmlns="http://www.w3.org/2001/SMIL20/Language" filename="{generate-id(.)}">
          <xsl:call-template name="MakeSmilCoreNode" />
        </smil>
        <xsl:comment>ended a newfile tag here</xsl:comment>
      </xsl:when>
      <xsl:otherwise>
        <xsl:call-template name="MakeSmilCoreNode" />
      </xsl:otherwise>
    </xsl:choose>

  </xsl:template>

  <xsl:template match="xuk:ChannelsProperty" mode="SMIL" >
    <seq>
      <xsl:attribute name="id">
        <xsl:value-of select="generate-id(.)"/>
      </xsl:attribute>
      <xsl:apply-templates mode="SMIL" />
    </seq>
  </xsl:template>

  <xsl:template match="xuk:SequenceMedia" mode="SMIL" >
    <seq>
      <xsl:attribute name="id">
        <xsl:value-of select="generate-id(.)"/>
      </xsl:attribute>
      <xsl:comment>Not really needed, since nothing will be referring this seq directly </xsl:comment>
      <xsl:apply-templates mode="SMIL" />
    </seq>
  </xsl:template>

  <xsl:template match="xuk:AudioMedia" mode="SMIL" >
    <audio>
      <xsl:attribute name="id">
        <xsl:value-of select="generate-id(.)"/>
      </xsl:attribute>
      <xsl:copy-of select="@*"/>
    </audio>
  </xsl:template>

  <xsl:template match="xuk:ChannelMapping" mode="SMIL" >
    <seq>
      <xsl:attribute name="id">
        <xsl:value-of select="generate-id(.)"/>
      </xsl:attribute>
      <xsl:apply-templates mode="SMIL" />
    </seq>
  </xsl:template>

  <!-- Currently Obi does not produce fulltext, so there is little reason to include references to such a file
  
  <xsl:template match="TextMedia" mode="SMIL">
    <text>
      <xsl:attribute name="src">
        <xsl:value-of select="GetTheNameOfTheFulltextDoc"/>#<xsl:value-of select="generate-id(ancestor::CoreNode[1])"/>
      </xsl:attribute>
    </text>
  </xsl:template>
  -->

  <xsl:template match="*" mode="SMIL" >
    <xsl:message terminate="no" >
      Processing <xsl:value-of select="name()"/> on SMIL
    </xsl:message>
    <xsl:apply-templates mode="SMIL" />
  </xsl:template>

</xsl:stylesheet>