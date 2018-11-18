<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT">
  <xsl:output method="html" encoding="UTF-8" indent="yes" />
  
  <!-- Main Template -->
  <xsl:template match="/Rows">
    <xsl:call-template name="Toolbar" />
    <!-- Main table -->
    <table width="100%" class="ms-listviewtable" cellspacing="0" cellpadding="1" border="0" style="border-style: none; width: 100%; border-collapse: collapse;">
      <tbody>
        <xsl:for-each select="/Rows/Row">
          <xsl:call-template name="DrawRows" />
        </xsl:for-each>
      </tbody>
    </table>
  </xsl:template>
  
  <!-- Toolbar -->
  <xsl:template name="Toolbar">
    <table cellSpacing="0" class="ms-menutoolbar" cellPadding="0" border="0" width="100%">
      <tr>

        <xsl:call-template name="Button">
          <xsl:with-param name="url" select="Row/_ItemUrl" />
          <xsl:with-param name="option">Display</xsl:with-param>
          <xsl:with-param name="image">/_layouts/images/detail.gif</xsl:with-param>
        </xsl:call-template>

        <xsl:call-template name="Separator" />

        <xsl:call-template name="Button">
          <xsl:with-param name="url" select="Row/_ItemEdit" />
          <xsl:with-param name="option">Edit</xsl:with-param>
          <xsl:with-param name="image">/_layouts/images/edit.gif</xsl:with-param>
        </xsl:call-template>

        <xsl:call-template name="Separator" />

        <xsl:call-template name="Button">
          <xsl:with-param name="url" select="Row/_ListUrl" />
          <xsl:with-param name="option">List</xsl:with-param>
          <xsl:with-param name="image">/_layouts/images/list.gif</xsl:with-param>
        </xsl:call-template>
       
        <xsl:call-template name="RightFill" />
      </tr>
    </table>
  </xsl:template>
  
  <!-- Toolbar Button -->
  <xsl:template name="Button">
    <xsl:param name="url" />
    <xsl:param name="option" />
    <xsl:param name="image" />
    <td nowrap="true" class="ms-toolbar">
      <table cellspacing="0" cellpadding="1" border="0">
        <tbody>
          <tr>
            <td nowrap="" style="padding: 3px;" class="ms-toolbar">
              <a href="{$url}{sps:SourceParam()}">
                <img src="{$image}" border="0" />
              </a>
            </td>
            <td nowrap="" style="padding: 3px;" class="ms-toolbar">
              <a href="{$url}{sps:SourceParam()}">
                <xsl:value-of select="$option" />
              </a>
            </td>
          </tr>
        </tbody>
      </table>
    </td>
  </xsl:template>
  
  <!-- Toolbar Separator -->
  <xsl:template name="Separator">
    <td>|</td>
  </xsl:template>

  <!-- Toolbar RightFill -->
  <xsl:template name="RightFill">
    <td width="99%" nowrap="">
      <img width="1" height="18" alt="" src="/_layouts/images/blank.gif" />
    </td>
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
