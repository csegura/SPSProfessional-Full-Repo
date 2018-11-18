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
      <!-- First column Thumbnail -->
      <td>        
        <a href="{sps:Event('Send',_RowNumber)}">
          <img src="{EncodedAbsThumbnailUrl}" />          
        </a>
      </td>      
          <td>
            <xsl:value-of select="FileRef" />
          </td>      
    </tr>
  </xsl:template>

  <!-- Header Table Template -->
  <xsl:template name="DrawHeader">
    <tr class="ms-viewheadertr" valign="top">
      <th class="ms-vh2-nofilter ms-vh2-gridview" nowrap="">
        Thumbnail
      </th>
      <th class="ms-vh2-nofilter ms-vh2-gridview" nowrap="">
        Image
      </th>
    </tr>
  </xsl:template>

</xsl:stylesheet>

