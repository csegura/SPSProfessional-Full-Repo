<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT" 
                exclude-result-prefixes="msxsl sps">
  
  <xsl:output method="xml" indent="yes" />
  
  <!-- First Level -->
  <xsl:key name="KGroup" match="Row" use="Columna3" />
  
  <!-- Second Level / First Level + Second Level -->
  <xsl:key name="KSubGroup" match="Row" use="concat(Columna3,' ',Continent)" />
  
  <!-- Second Level - Distinct names -->
  <xsl:key name="KSubGruopDistinct" match="Row" use="Continent" />
  
  <xsl:template name="MS" match="/Rows">
    <graph xaxisname="Continent" 
           yaxisname="Export" 
           hovercapbg="DEDEBE" 
           hovercapborder="889E6D" 
           rotateNames="0" 
           yAxisMaxValue="100"
           numdivlines="9" 
           divLineColor="CCCCCC" 
           divLineAlpha="80" 
           decimalPrecision="0"
           showAlternateHGridColor="1"
           AlternateHGridAlpha="30" 
           AlternateHGridColor="CCCCCC" 
           caption="Global Export" 
           subcaption="Sales by Groups in Continents">
      
      <!-- Select distinct for categories -->
      <categories font="Arial" fontSize="11" fontColor="000000">
      <xsl:for-each select="Row[count(. | key('KSubGruopDistinct', Continent)[1]) = 1]">
        <xsl:sort select="Continent" />
        <category name="{Continent}" hoverText="{Continent}" />
      </xsl:for-each>
      </categories>
      
      <!-- First Level Group (DataSets) -->
      <xsl:for-each select="Row[count(. | key('KGroup', Columna3)[1]) = 1]">
        <xsl:sort select="Columna3" />
        <dataset seriesname="{Columna3}" color="{sps:GetFcColor()}">
          <xsl:call-template name="SecondLevel" />
        </dataset>
      </xsl:for-each>
    </graph>
  </xsl:template>

  <!-- Second Level -->
  <xsl:template name="SecondLevel">
    <xsl:variable name="VSubGroup" select="key('KGroup', Columna3)" />
    <xsl:for-each
        select="$VSubGroup[generate-id() =
                             generate-id(
                               key('KSubGroup',
                                   concat(Columna3, ' ', Continent))[1])]">
      <xsl:sort select="Continent" />
      <!-- Set -->
      <set value="{sum(Columna2)}" />      
    </xsl:for-each>
                                                  
  </xsl:template>
</xsl:stylesheet>
