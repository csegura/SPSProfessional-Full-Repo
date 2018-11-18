<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="html" indent="yes"/>

  <!-- Main Template -->
  <xsl:template match="Rows">
    <table>
      <xsl:for-each select="Row">
        <xsl:call-template name="TableRow"/>
      </xsl:for-each>
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
  
</xsl:stylesheet>

