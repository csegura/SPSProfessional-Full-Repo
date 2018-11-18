<?xml version='1.0' encoding='utf-8'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" />
  <xsl:key name="group-by-estado" match="Row" use="Estado" />
  <xsl:template match="Rows">
    <table width="100%">
      <tbody>
        <xsl:for-each select="Row[count(. | key('group-by-estado', Estado)[1]) = 1]">
          <xsl:sort select="Estado" />
          <tr id="group0" >
            <td class="ms-gb" colspan="4">
              <span class="ms-announcementtitle">
                <a href="javascript:" onclick="javascript:ExpGroupBy(this);return false;">
                  <xsl:element name ="img">
                    <xsl:choose>
                      <xsl:when test="substring-before(Estado,'.')='03'">
                        <xsl:attribute name ="src">/_layouts/images/plus.gif</xsl:attribute>
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:attribute name ="src">/_layouts/images/minus.gif</xsl:attribute>
                      </xsl:otherwise>
                    </xsl:choose>
                    <xsl:attribute name ="border">0</xsl:attribute>
                  </xsl:element >
                </a>                        <![CDATA[ ]]>                        <xsl:value-of select="substring-after(Estado,'-')" />
              </span>
            </td>
          </tr>
          <xsl:for-each select="key('group-by-estado', Estado)">
            <xsl:element name ="tr">
              <xsl:choose>
                <xsl:when test="substring-before(Estado,'.')='03'">
                  <xsl:attribute name ="style">display:none</xsl:attribute>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:attribute name ="style">display:auto</xsl:attribute>
                </xsl:otherwise>
              </xsl:choose>
              <td class="ms-vb" width="5%" style="padding-bottom: 3px">
                <span class="ms-announcementtitle">
                  <xsl:choose>
                    <xsl:when test="substring-before(Estado_x0020_Documentación,'.')='01'">
                      <img src="/_layouts/3082/images/avanco/sblack.gif" border="0" />
                    </xsl:when>
                    <xsl:when test="substring-before(Estado_x0020_Documentación,'.')='04'">
                      <img src="/_layouts/3082/images/avanco/sred.gif" border="0" />
                    </xsl:when>
                    <xsl:when test="substring-before(Estado_x0020_Documentación,'.')='05'">
                      <img src="/_layouts/3082/images/avanco/sorange.gif" border="0" />
                    </xsl:when>
                    <xsl:when test="substring-before(Estado_x0020_Documentación,'.')='06'">
                      <img src="/_layouts/3082/images/avanco/sgreen.gif" border="0" />
                    </xsl:when>
                  </xsl:choose>
                </span>
              </td>
              <td class="ms-vb" width="85%" style="padding-bottom: 3px">
                <span>
                  <a href="{substring-before(Dirección,',')}">
                    <xsl:value-of select="Title" />
                  </a>
                </span>
              </td>
              <td class="ms-vb" width="5%" style="padding-bottom: 3px">
                <span>
                  <a href="{substring-before(Dirección,',')}/Lists/Seguimiento%20de%20Actividades/NewForm.aspx">
                    <img src="/_layouts/3082/images/avanco/seguimiento.gif" alt="Nuevo seguimiento" border="0" />
                  </a>
                </span>
              </td>
              <td class="ms-vb" width="5%" style="padding-bottom: 3px">
                <span>
                  <a href="{substring-before(Dirección,',')}/Archivo%20D/Forms/AllItems.aspx">
                    <img src="/_layouts/3082/images/avanco/DocuDocu.jpg" alt="Ir a DocuDocu" border="0" />
                  </a>
                </span>
              </td>
            </xsl:element>
          </xsl:for-each>
        </xsl:for-each>
      </tbody>
    </table>
  </xsl:template>
</xsl:stylesheet>