<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT" exclude-result-prefixes="msxsl sps">
  <xsl:output method="xml" indent="yes" />
  
  <!-- Process Graph -->
  <xsl:template match="@* | node()">    
    <xsl:apply-templates />
  </xsl:template>
  
  <!-- Pie Chart -->
  <xsl:template match="Rows">
    <graph caption="Pie Chart" decimalPrecision="0" showPercentageValues="1" showNames="1" showValues="1" showPercentageInLabel="1" pieYScale="45" pieBorderAlpha="100" pieRadius="100" animation="0" shadowXShift="4" shadowYShift="4" shadowAlpha="40" pieFillAlpha="95" pieBorderColor="FFFFFF">
      <xsl:if test="count(//Status[text()='In Progress'])&gt;0">
       <set name="In Progress" value="{count(//Status[text()='In Progress'])}" color="{sps:GetFcColor()}" />
      </xsl:if>
      <xsl:if test="count(//Status[text()='Completed'])&gt;0">
        <set name="Coompleted" value="{count(//Status[text()='Completed'])}" color="{sps:GetFcColor()}" />
      </xsl:if>
      <xsl:if test="count(//Status[text()='Not Started'])&gt;0">
        <set name="Not Started" value="{count(//Status[text()='Not Started'])}" color="{sps:GetFcColor()}" />
      </xsl:if>
    </graph>
  </xsl:template>
  
</xsl:stylesheet>