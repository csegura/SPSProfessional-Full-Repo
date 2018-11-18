<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                exclude-result-prefixes="msxsl">
  
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="@* | node()">
    <!-- Graph -->
    <graph caption="Title" 
           xAxisName="Tasks"
           yAxisName="Completed"
           decimalPrecision="0" 
           formatNumberScale="0">
      <xsl:apply-templates />
    </graph>
  </xsl:template>

  <!-- Each row -->
  <xsl:template match="Row">    
    <!-- Put here your fields -->
    <set name="{Title}" value="{PercentComplete}" />
  </xsl:template>
  
</xsl:stylesheet>
