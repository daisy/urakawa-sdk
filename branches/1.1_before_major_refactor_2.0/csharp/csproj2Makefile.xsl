<?xml version="1.0" encoding="utf-8"?>

<!-- Create a Makefile for gmcs on Unix machines -->
<!-- From csproj2gmcsCmd.xsl -->

<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:csproj="http://schemas.microsoft.com/developer/msbuild/2003">

 <xsl:output method="text" encoding="utf-8" />
 <xsl:param name="include-dir">mono</xsl:param>
 <xsl:param name="output-dir">mono</xsl:param>

 <xsl:template match="/">

   <xsl:text>MCS = gmcs&#xa;</xsl:text>

   <xsl:text>INCLUDE_DIR = </xsl:text>
   <xsl:value-of select="$include-dir"/>
   <xsl:text>&#xa;</xsl:text>

   <xsl:text>OUTPUT_DIR = </xsl:text>
   <xsl:value-of select="$output-dir"/>
   <xsl:text>&#xa;</xsl:text>

   <xsl:text>MCS_FLAGS = -target:</xsl:text>
   <xsl:value-of select="translate(csproj:Project/csproj:PropertyGroup/csproj:OutputType,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')"/>
   <xsl:for-each select="csproj:Project/csproj:ItemGroup/csproj:Reference[contains(@Include,', Version=')]">
     <xsl:text> \&#xa;&#x9;    -reference:$(INCLUDE_DIR)/</xsl:text>
     <xsl:value-of select="substring-before(@Include, ',')"/>
     <xsl:text>.dll</xsl:text>
   </xsl:for-each>
   <xsl:for-each
     select="csproj:Project/csproj:ItemGroup/csproj:ProjectReference">
     <xsl:text> \&#xa;&#x9;    -reference:$(INCLUDE_DIR)/</xsl:text>
     <xsl:value-of select="document(@Include)/csproj:Project/csproj:PropertyGroup/csproj:AssemblyName"/>
     <xsl:text>.dll</xsl:text>
   </xsl:for-each>
   <xsl:text>&#xa;</xsl:text>

   <xsl:text>ASSEMBLY_NAME = $(OUTPUT_DIR)/</xsl:text>
   <xsl:value-of
     select="csproj:Project/csproj:PropertyGroup/csproj:AssemblyName"/>
    <xsl:choose>
      <xsl:when
        test="csproj:Project/csproj:PropertyGroup/csproj:OutputType='Library'">
        <xsl:text>.dll</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text>.exe</xsl:text>
      </xsl:otherwise>
    </xsl:choose>
   <xsl:text>&#xa;</xsl:text>

   <xsl:text>CS_FILES = </xsl:text>
   <xsl:for-each select="csproj:Project/csproj:ItemGroup/csproj:Compile[(csproj:SubType[text()='Code']) or not (*)]">
     <xsl:if test="position()>1">
       <xsl:text>&#x9;   </xsl:text>
     </xsl:if>
     <xsl:value-of select="translate(@Include, '\', '/')"/>
     <xsl:if test="position()!=last()">
       <xsl:text> \</xsl:text>
     </xsl:if>
    <xsl:text>&#xa;</xsl:text>
   </xsl:for-each>
   <xsl:text>&#xa;</xsl:text>

   <xsl:text>all:&#x9;$(ASSEMBLY_NAME)&#xa;</xsl:text>
   <xsl:text>&#xa;</xsl:text>

   <xsl:text>$(ASSEMBLY_NAME):&#x9;$(CS_FILES)&#xa;</xsl:text>
   <xsl:text>&#x9;@if [ ! -d $(OUTPUT_DIR) ]; then mkdir -p $(OUTPUT_DIR); fi&#xa;</xsl:text>
   <xsl:text>&#x9;$(MCS) $(MCS_FLAGS) -out:$@ $(CS_FILES)&#xa;</xsl:text>
   <xsl:text>&#xa;</xsl:text>

   <xsl:text>clean:&#xa;</xsl:text>
   <xsl:text>&#x9;$(RM) $(ASSEMBLY_NAME)&#xa;</xsl:text>

 </xsl:template>

</xsl:stylesheet>
