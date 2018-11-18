<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT">

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
      <!-- Draw header -->
      <xsl:call-template name="DrawHeader"/>      
      <!-- Draw Rows -->
      <xsl:for-each select="Row">
        <xsl:call-template name="DrawRow"/>
      </xsl:for-each>
      </tbody>
    </table>
  </xsl:template>

  <!-- Row Table Template -->
  <xsl:template name="DrawRow">
    <tr>
      <!-- First column link, fired RowSelect -->
      <td>
        <a href="{sps:Event('Send',_RowNumber)}">
          <xsl:value-of select="_RowNumber" />
        </a>
      </td>
      <!-- Draw other columns -->
      <xsl:for-each select="node()">
        <xsl:if test="name()!='_RowNumber'">
          <td>
            <xsl:value-of select="." disable-output-escaping="yes"/>
          </td>
        </xsl:if>  
      </xsl:for-each>
    </tr>
  </xsl:template>

  <!-- Header Table Template -->
  <xsl:template name="DrawHeader">
    <tr class="ms-viewheadertr" valign="top">
      <th class="ms-vh2-nofilter ms-vh2-gridview" nowrap="">
        Num
      </th>
      <xsl:for-each select="Row[1]/*">
        <xsl:if test="name()!='_RowNumber'">
          <th class="ms-vh2-nofilter ms-vh2-gridview" nowrap="">
            <xsl:value-of select="sps:XmlDecode(name())"/>
          </th>
        </xsl:if>
      </xsl:for-each >
    </tr>
  </xsl:template>

</xsl:stylesheet>

