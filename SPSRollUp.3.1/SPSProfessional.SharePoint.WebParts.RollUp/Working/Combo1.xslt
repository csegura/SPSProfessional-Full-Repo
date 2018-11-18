<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT" 
                exclude-result-prefixes="sps">
  <xsl:output method="html" />

  <xsl:param name="CurrentRow" />
  
  <xsl:template match="/">
    <![CDATA[
    <script type="text/javascript">
      function combo_onchange(oSelect)
      {
      if(oSelect.selectedIndex != -1)
      {
      var selected = oSelect.options[oSelect.selectedIndex].value;
      ]]><xsl:value-of select="sps:EventJS('Select','selected')" /><![CDATA[
      }
      }
    </script>
     ]]>
    Seleccione el proveedor
    <select id="a1" name="combo" onchange="javascript:combo_onchange(this);" class="ms-input">
      <option value="" />
      <xsl:for-each select="Rows/Row">
        <xsl:sort select="Title" />
        <xsl:element name="option">
          <xsl:attribute name="value">
            <xsl:value-of select="_RowNumber"/>
          </xsl:attribute>
          <xsl:if test="_RowNumber=$CurrentRow">
            <xsl:attribute name ="selected" />
          </xsl:if>
          <xsl:value-of select="Title" />
        </xsl:element>
      </xsl:for-each>
    </select>
  </xsl:template>
</xsl:stylesheet>
