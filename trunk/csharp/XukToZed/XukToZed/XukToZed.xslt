<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.daisy.org/urakawa/xuk/0.5">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/">
    <wrapper>
      <ncx>
        <!-- Does the head, doctitle and docAuthor-->
        <xsl:apply-templates />
        <navMap>
          <xsl:apply-templates mode="NAVMAP" />
        </navMap>
      </ncx>
      <smil>
        <xsl:apply-templates mode="SMIL" />
      </smil>
    </wrapper>
  </xsl:template>

  <xsl:template name="newline">
    <xsl:text >
  </xsl:text>
  </xsl:template>

  <!-- Building the NAVMAP-->

  <xsl:template match="CoreNode" mode="NAVMAP">
    <xsl:choose>
      <xsl:when test="mProperties/ChannelsProperty/ChannelMapping/TextMedia" >
        <navTarget>
          <xsl:for-each select="mProperties/ChannelsProperty/ChannelMapping/TextMedia">
            <navLabel>
              <text>
                <xsl:value-of select="."/>
              </text>
              <!-- Do something for Audio(?), even if current impl hasn't anything in direct sync -->
            </navLabel>
            <content>
              <xsl:attribute name="src">everything.smil#<xsl:value-of select ="generate-id(ancestor::CoreNode[1])"/></xsl:attribute>
            </content>

          </xsl:for-each>
          <xsl:apply-templates mode="NAVMAP"/>
        </navTarget>
      </xsl:when>
      <xsl:otherwise>
        <xsl:message terminate="no">
          Skipping <xsl:value-of select="name()"/>
        </xsl:message>
        <xsl:apply-templates mode="NAVMAP" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="*" mode="NAVMAP" >
    <xsl:message terminate="no" >
      Processing <xsl:value-of select="name()"/> on NAVMAP
    </xsl:message>
    <xsl:apply-templates mode="NAVMAP"/>
  </xsl:template>


  <!-- Building the SMIL-->
  <xsl:template match="CoreNode" mode="SMIL" >
    <seq>
      <xsl:attribute name="id">
        <xsl:value-of select="./id"/>
      </xsl:attribute>
      <xsl:apply-templates mode="SMIL" />
    </seq>
  </xsl:template>

  <xsl:template match="ChannelsProperty" mode="SMIL" >
    <seq>
      <xsl:attribute name="id">
        <xsl:value-of select="generate-id(.)"/>
      </xsl:attribute>
      <xsl:apply-templates mode="SMIL" />
    </seq>
  </xsl:template>

  <xsl:template match="SequenceMedia" mode="SMIL" >
    <seq>
      <xsl:attribute name="id">
        <xsl:value-of select="generate-id(.)"/>
        <!-- not really needed, since nothing will be referring this seq directly -->
      </xsl:attribute>
      <xsl:apply-templates mode="SMIL" />
    </seq>
  </xsl:template>

  <xsl:template match="AudioMedia" mode="SMIL" >
    <audio>
      <xsl:copy-of select="@*"/>
    </audio>
  </xsl:template>

  <xsl:template match="ChannelMapping" mode="SMIL" >
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

  <xsl:template match="*" >
    <xsl:message terminate="no" >
      Processing <xsl:value-of select="name()"/> on *
    </xsl:message>
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="text()" mode="NAVMAP">
    <xsl:apply-templates mode="NAVMAP"/>
  </xsl:template>

  <xsl:template match="text()" mode="SMIL">
    <xsl:apply-templates mode="SMIL"/>
  </xsl:template>

  <xsl:template match="text()">
    <xsl:apply-templates/>
  </xsl:template>


</xsl:stylesheet>