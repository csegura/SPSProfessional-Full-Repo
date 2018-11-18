<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT" 
                exclude-result-prefixes="msxsl sps">
  
  <xsl:output method="xml" indent="yes" />
    
  <!-- First Level -->
  <xsl:key name="KGroup" match="Row" use="Columna3" /> 
  
  <!-- Select distinct for categories -->
  <xsl:key name="KSubGroupDistinct" match="Row" use="Title" />

  <!-- Graph Template -->
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
      
      <!-- Select distinct for Continents -->
      <categories font="Arial" fontSize="11" fontColor="000000">
      <xsl:for-each select="Row[count(. | key('KSubGroupDistinct', Title)[1]) = 1]">
        <xsl:sort select="Title" />
        <category name="{Title}" hoverText="{Title}" />
      </xsl:for-each>
      </categories>

      
      <!-- Select distinct for Parts -->
      <xsl:for-each select="Row[count(. | key('KGroup', Columna3)[1]) = 1]">
        <xsl:sort select="Columna3" />
        <!-- Generate Graph DataSet -->
        <dataset seriesname="{Columna3}" color="{sps:GetFcColor()}">
          <!-- For each part get the data in each Continent -->
          <xsl:call-template name="SecondGroup">
            <xsl:with-param name="Part" select="Columna3" />
          </xsl:call-template>
        </dataset>
      </xsl:for-each>           
    </graph>
  </xsl:template>

  <!-- Search Parts in a Continent -->
  <xsl:template name="SecondGroup">
    <xsl:param name="Part" />
      <!-- Again select distinct Continent -->
      <xsl:for-each select="/Rows/Row[count(. | key('KSubGroupDistinct', Title)[1]) = 1]">
        <xsl:sort select="Title"/>
        <xsl:variable name="ContinentName" select="Title" />
        <!-- Prepare the search Part & Continent -->
        <xsl:variable name="GS" select="/Rows/Row[Columna3=$Part][Title=$ContinentName]" />
        <xsl:choose>
          <!-- If no data set data to 0 -->
          <xsl:when test="count($GS[1]) = 0">
            <set value="0" />
          </xsl:when>
          <!-- If data sum -->
          <xsl:otherwise>
            <set value="{sum($GS/Columna2)}" />
          </xsl:otherwise>
        </xsl:choose>           
      </xsl:for-each>
  </xsl:template>
  
</xsl:stylesheet>
