<?xml version='1.0' encoding='utf-8'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:utils="urn:script-items">
  <msxsl:script language="C#" implements-prefix="utils">      <![CDATA[               public string FormatDate(string sDate)              {                                 if (sDate.Length > 0)                               {                                   DateTime d = DateTime.Parse(sDate);        return d.ToShortDateString();                   }       else          return "";      }      ]]>   </msxsl:script>
  <xsl:output method="html" encoding="UTF-8" indent="yes" />
  <xsl:param name="CurrentPage" />
  <xsl:param name="RecordCount" />
  <xsl:variable name="RecordsPage" select="5" />
  <xsl:template match="/">
    <html>
      <head>         </head>
      <body>
        <p>
          <table width="100%">
            <tbody>
              <xsl:for-each select="Rows/Row">
                <xsl:sort select="Fecha" order="descending" />
                <xsl:if test="position() &gt; $RecordsPage * number($CurrentPage) and position() &lt;= number($RecordsPage * number($CurrentPage) + $RecordsPage)">
                  <tr>
                    <td class="ms-gb" width="50%">
                      <xsl:value-of select="utils:FormatDate(Fecha)" />                                 <![CDATA[ ]]>                                 <a href="{_SiteUrl}/Lists/Seguimiento%20de%20Actividades/Todos%20los%20Seguimientos.aspx">
                        <xsl:value-of select="_SiteTitle" />
                      </a>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <b>
                        <xsl:value-of select="Actividad" />
                      </b>                                 <![CDATA[  ]]>                                 <xsl:value-of select="SubActividad" />                                 <![CDATA[ - ]]>                                 <xsl:value-of select="Proceso" />
                    </td>
                  </tr>
                  <tr>
                    <td colspan="2">
                      <xsl:value-of select="Seguimiento" disable-output-escaping="yes" />
                    </td>
                  </tr>
                </xsl:if>
              </xsl:for-each>
            </tbody>
          </table>
        </p>
        <p>
          <table width="100%">
            <tbody>
              <tr>
                <td width="50%">
                  <xsl:if test="$CurrentPage&gt;0">
                    <xsl:element name="a">
                      <xsl:attribute name="href">
                        ?page=                               <xsl:value-of select="number($CurrentPage)-1" />
                      </xsl:attribute>                              &lt;&lt; Anterior
                    </xsl:element>
                  </xsl:if>
                  <!-- Next page, do not show when at end() of listing -->
                </td>
                <td width="50%">
                  <p align="right">
                    <xsl:if test="($RecordCount - ((1+number($CurrentPage)) * $RecordsPage))&gt;0">
                      <xsl:element name="a">
                        <xsl:attribute name="href">
                          ?page=                                  <xsl:value-of select="number($CurrentPage)+1" />
                        </xsl:attribute>                                 Siguiente &gt;&gt;
                      </xsl:element>
                    </xsl:if>
                  </p>
                </td>
              </tr>
            </tbody>
          </table>
        </p>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
