<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT" 
                exclude-result-prefixes="msxsl sps">
  
  <xsl:output method="xml" indent="yes" />
  
  <!-- First Level -->
  <xsl:key name="KGroup" match="Row" use="Nivel_x0020_de_x0020_Interés" />
  
  <!-- Second Level / First Level + Second Level -->
  <xsl:key name="KSubGroup" match="Row" use="concat(Nivel_x0020_de_x0020_Interés,' ',sps:FormatDateTime(Fecha_x0020_de_x0020_Contacto,'MMM/yy'))" />
  
  <!-- Second Level - Distinct names -->
  <xsl:key name="KSubGruopDistinct" match="Row" use="sps:FormatDateTime(Fecha_x0020_de_x0020_Contacto,'MMM/yy')" />
  
  <xsl:template name="MS" match="/Rows">
    <graph xaxisname="sps:FormatDateTime(Fecha_x0020_de_x0020_Contacto,'MMM/yy')" 
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
           subcaption="Sales by Groups in sps:FormatDateTime(Fecha_x0020_de_x0020_Contacto,'MMM/yy')s">
      
      <!-- Select distinct for categories -->
      <categories font="Arial" fontSize="11" fontColor="000000">
      <xsl:for-each select="Row[count(. | key('KSubGruopDistinct', sps:FormatDateTime(Fecha_x0020_de_x0020_Contacto,'MMM/yy'))[1]) = 1]">
        <xsl:sort select="sps:FormatDateTime(Fecha_x0020_de_x0020_Contacto,'MMM/yy')" />
        <category name="{sps:FormatDateTime(Fecha_x0020_de_x0020_Contacto,'MMM/yy')}" hoverText="{sps:FormatDateTime(Fecha_x0020_de_x0020_Contacto,'MMM/yy')}" />
      </xsl:for-each>
      </categories>
      
      <!-- First Level Group (DataSets) -->
      <xsl:for-each select="Row[count(. | key('KGroup', Nivel_x0020_de_x0020_Interés)[1]) = 1]">
        <xsl:sort select="Nivel_x0020_de_x0020_Interés" />
        <dataset seriesname="{Nivel_x0020_de_x0020_Interés}" color="{sps:GetFcColor()}">
          <xsl:call-template name="SecondLevel" />
        </dataset>
      </xsl:for-each>
    </graph>
  </xsl:template>

  <!-- Second Level -->
  <xsl:template name="SecondLevel">
    <xsl:variable name="VSubGroup" select="key('KGroup', Nivel_x0020_de_x0020_Interés)" />
    <xsl:for-each
        select="$VSubGroup[generate-id() =
                             generate-id(
                               key('KSubGroup',
                                   concat(Nivel_x0020_de_x0020_Interés, ' ', sps:FormatDateTime(Fecha_x0020_de_x0020_Contacto,'MMM/yy')))[1])]">
      <xsl:sort select="sps:FormatDateTime(Fecha_x0020_de_x0020_Contacto,'MMM/yy')" />
      <!-- Set -->
      <set value="{sum(Columna2)}" />      
    </xsl:for-each>
                                                  
  </xsl:template>
</xsl:stylesheet>
