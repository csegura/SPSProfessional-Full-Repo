<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="html" indent="yes"/>

  <!-- Main Template -->
  <xsl:template match="Rows">
    <table width="100%" 
           class="ms-listviewtable" 
           cellspacing="0" 
           cellpadding="1" 
           border="0" 
           style="border-style: none; width: 100%; border-collapse: collapse;">
      <tbody>
      <xsl:call-template name="TableHeader"/>      
        <xsl:for-each select="Row">
          <xsl:call-template name="TableRow"/>
        </xsl:for-each>
      </tbody>
    </table>
  </xsl:template>

  <!-- TableRow Template -->
  <xsl:template name="TableRow">
    <tr>
      <td>
        <xsl:value-of select="Title"/>
      </td>
      <td>
        <xsl:value-of select="StartDate"/>
      </td>
      <td>
        <xsl:value-of select="PercentComplete"/>
      </td>
    </tr>
  </xsl:template>

  <!-- TableHeader Template -->
  <xsl:template name="TableHeader">
    <tr class="ms-viewheadertr" valign="top">
      <th class="ms-vh2-nofilter ms-vh2-gridview" nowrap="">
        Title
      </th>
      <th class="ms-vh2-nofilter ms-vh2-gridview" nowrap="">
        Start
      </th>
      <th class="ms-vh2-nofilter ms-vh2-gridview" nowrap="">
        Completed
      </th>
    </tr>
  </xsl:template>
  
</xsl:stylesheet>

