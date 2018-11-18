<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT">
  <xsl:output method="html" encoding="UTF-8" indent="yes" />
  <xsl:param name="CurrentPage" />
  <!-- Main Template -->
  <xsl:template match="/Rows">

    <!-- Main table -->
    <table width="100%" class="ms-listviewtable" cellspacing="0" cellpadding="1" border="0" style="border-style: none; width: 100%; border-collapse: collapse;">
      <tbody>
        <xsl:for-each select="/Rows/Row">
          <xsl:call-template name="DrawRows" />
        </xsl:for-each>
      </tbody>
    </table>
  </xsl:template>
  
  <!-- TableRow Template -->
  <xsl:template name="DrawRows">
    <xsl:for-each select="node()">
      <xsl:choose>
        <xsl:when test="starts-with(name(), '_')">
        </xsl:when>
        <xsl:otherwise>
          <tr class="">
            <!-- Icon to send the row -->
            <td width="165" valign="top" class="ms-formlabel">
              <xsl:value-of select="sps:XmlDecode(name())" />
            </td>
            <td valign="top" class="ms-formbody">
              <xsl:value-of select="." disable-output-escaping="yes" />
            </td>
          </tr>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>