<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xuk="http://www.daisy.org/urakawa/xuk/0.5">
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
      <filenames>
        <xsl:apply-templates mode="MEDIAFILES" />
      </filenames>
    </wrapper>
  </xsl:template>

  <xsl:template name="newline">
    <xsl:text >
  </xsl:text>
  </xsl:template>

  <!-- Building the NAVMAP-->

  <xsl:template match="xuk:CoreNode" mode="NAVMAP">
    <xsl:choose>
      <xsl:when test="xuk:mProperties/xuk:ChannelsProperty/xuk:ChannelMapping[@channel='CHID0001']/xuk:TextMedia/text()" >
        <navTarget>
          <xsl:for-each select="xuk:mProperties/xuk:ChannelsProperty/xuk:ChannelMapping[@channel='CHID0001']/xuk:TextMedia/text()">
            <navLabel>
              <text>
                <xsl:value-of select="."/>
              </text>
              <!-- Do something for Audio(?), even if current impl hasn't anything in direct sync -->
            </navLabel>
            <content>
              <xsl:attribute name="src">everything.smil#<xsl:value-of select ="generate-id(ancestor::xuk:CoreNode[1])"/></xsl:attribute>
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
  <xsl:template match="xuk:CoreNode" mode="SMIL" >
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
        <xsl:comment>Not including <xsl:value-of select="generate-id(.)"/> in SMIL</xsl:comment>
        <xsl:apply-templates mode="SMIL"/>
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
      </xsl:attribute><xsl:comment>Not really needed, since nothing will be referring this seq directly </xsl:comment>
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
    <xsl:message terminate="no" >
      Processing <xsl:value-of select="name()"/> on *
    </xsl:message>
    <xsl:apply-templates />
  </xsl:template>

  <!-- geting rid of default text handling -->
  <xsl:template match="text()" mode="NAVMAP">
    <xsl:apply-templates mode="NAVMAP"/>
  </xsl:template>

  <xsl:template match="text()" mode="SMIL">
    <xsl:apply-templates mode="SMIL"/>
  </xsl:template>

  <xsl:template match="text()" mode="MEDIAFILES">
    <xsl:apply-templates mode="MEDIAFILES"/>
  </xsl:template>

  <xsl:template match="text()">
    <xsl:apply-templates/>
  </xsl:template>


</xsl:stylesheet>