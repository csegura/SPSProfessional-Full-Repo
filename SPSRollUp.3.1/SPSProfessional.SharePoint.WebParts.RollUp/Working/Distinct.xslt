<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT"
                exclude-result-prefixes="msxsl sps">
  <xsl:output method="html" indent="yes"/>

  <!-- Select distinct Sites and Lists -->
  <xsl:key name="sites-lists" match="Row" use="concat(_SiteTitle,_ListTitle)" />

  <xsl:template match="Rows">
    <table>
      <!-- Show Lists and Sites -->
      <xsl:for-each select="Row[count(. | key('sites-lists', concat(_SiteTitle,_ListTitle))[1]) = 1]">
        <xsl:sort select="_SiteTitle" />
        <xsl:sort select="_ListTitle" />
        <tr>
          <td>
            <a href="{sps:Event('Select',_RowNumber)}">
              <img src="/_layouts/images/exptitem.gif" border="0" />
            </a>
          </td>
          <td>
            <xsl:value-of select="_SiteTitle" /> ( <xsl:value-of select="_ListTitle" /> ) <br/>
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

</xsl:stylesheet>
