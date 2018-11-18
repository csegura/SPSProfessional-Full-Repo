<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT" 
                exclude-result-prefixes="msxsl sps">
  
  <xsl:output method="xml" indent="yes" />
  
  <!-- First Level -->
  <xsl:key name="KGroup" match="Row" use="Columna3" />
  
  <!-- Second Level / First Level + Second Level -->
  <xsl:key name="KSubGroup" match="Row" use="concat(Columna3,'-',Title)" />
  
  <!-- Second Level - Distinct names -->
  <xsl:key name="KSubGroupDistinct" match="Row" use="Title" />
  
  <xsl:template name="MS" match="/Rows">
    <graph xaxisname="Title" 
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
           subcaption="Sales by Groups in Titles">
      
      <!-- Select distinct for categories -->
      <categories font="Arial" fontSize="11" fontColor="000000">
      <xsl:for-each select="Row[count(. | key('KSubGroupDistinct', Title)[1]) = 1]">
        <xsl:sort select="Title" />
        <category name="{Title}" hoverText="{Title}" />
      </xsl:for-each>
      </categories>
      
      <!-- First Level Group (DataSets) -->
      <xsl:for-each select="Row[count(. | key('KGroup', Columna3)[1]) = 1]">
        <xsl:sort select="Columna3" />
        <dataset seriesname="{Columna3}" color="#000000">                   
          <xsl:call-template name="SecondLevel" />
        </dataset>
      </xsl:for-each>
    </graph>
  </xsl:template>

  <!-- Second Level -->
  <xsl:template name="SecondLevel">
    <xsl:variable name="VSubGroup" select="key('KGroup', Columna3)" />




    <xsl:variable name="NSubGroup" select="$VSubGroup[generate-id() =
                             generate-id(
                               key('KSubGroup',
                                   concat(Columna3, '-', Title))[1])]" />
   
    <debug a="{$VSubGroup}" d="{generate-id(key('KSubGroup',concat(Columna3, '-', Title)))}" />

    <xsl:for-each select="$NSubGroup">
      <xsl:sort select="Title" />
            
      <!-- Set -->
      <set name="{Title}" value="{sum(key('KSubGroup',concat(Columna3, '-', Title))/Columna2)}" />      
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>
