<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:csproj="http://schemas.microsoft.com/developer/msbuild/2003">
 <xsl:output method="text" encoding="utf-8" />
 <xsl:template match="/">
  <xsl:text>gmcs</xsl:text>
  <xsl:text> -target:</xsl:text><xsl:value-of select="translate(string(csproj:Project/csproj:PropertyGroup/csproj:OutputType), 'ABCDEFGHIJKLMNOPQRSTUVXYZÆØÅ', 'abcdefghijklmnopqrstuvxyzæøå')"/>
  <xsl:text> -out:mono/</xsl:text><xsl:value-of select="csproj:Project/csproj:PropertyGroup/csproj:AssemblyName"/>
  <xsl:choose>
  	<xsl:when test="string(csproj:Project/csproj:PropertyGroup/csproj:OutputType)='Library'"><xsl:text>.dll</xsl:text></xsl:when>
  	<xsl:when test="string(csproj:Project/csproj:PropertyGroup/csproj:OutputType)='Exe'"><xsl:text>.exe</xsl:text></xsl:when>
  	<xsl:when test="string(csproj:Project/csproj:PropertyGroup/csproj:OutputType)='WinExe'"><xsl:text>.exe</xsl:text></xsl:when>
  </xsl:choose>
  <xsl:for-each select="csproj:Project/csproj:ItemGroup/csproj:Reference[contains(@Include, ', Version=')]">
   <xsl:text> -reference:mono/</xsl:text><xsl:value-of select="substring-before(@Include, ',')"/><xsl:text>.dll</xsl:text>
  </xsl:for-each>
  <xsl:for-each select="csproj:Project/csproj:ItemGroup/csproj:ProjectReference">
  	<xsl:text> -reference:mono/</xsl:text><xsl:value-of select="document(@Include)/csproj:Project/csproj:PropertyGroup/csproj:AssemblyName"/><xsl:text>.dll</xsl:text>
  </xsl:for-each>
  <xsl:for-each select="csproj:Project/csproj:ItemGroup/csproj:Compile[(csproj:SubType[text()='Code']) or not (*)]">
  	<xsl:value-of select="' '"/><xsl:value-of select="translate(@Include, '\', '/')"/>
  </xsl:for-each>
  <xsl:text>
pause
</xsl:text>
 </xsl:template>
</xsl:stylesheet>