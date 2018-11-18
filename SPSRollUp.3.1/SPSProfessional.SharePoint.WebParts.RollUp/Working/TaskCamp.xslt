<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT">
  
  <xsl:output method="html" indent="yes" />

  <!-- Main Template -->
  <xsl:template match="Rows">
    <h1 style="font-size:1em;background-color:#CC0000;border-bottom:1px solid #CC0000;color:#FFFFFF;margin:0pt 0pt 4pt">Late</h1>
    <!-- Draw Rows -->
    <xsl:for-each select="Row">
      <xsl:sort select="DueDate" order="descending" />
      <xsl:call-template name="DrawRow" />
    </xsl:for-each>
  </xsl:template>
  
  <!-- Row Table Template -->
  <xsl:template name="DrawRow">
    <div style="border-bottom:1px solid #E0E0E0;padding:4px 0pt 4px 3px;">
      <div style="color:#CC0000;font-family:verdana;font-size:10px;padding:0pt 0pt 3px;">
        <strong>
          <!-- calc days ago -->
          <xsl:value-of select="sps:DateDiff('d',DueDate,sps:TodayIso())" /> days ago
        </strong> (<xsl:value-of select="sps:FormatDateTime(DueDate,'dddd, dd MMM')" />)
      </div>
      <div style="font-weight:bold;padding:0pt 5pt 6px;">
        <xsl:value-of select="Title" />
      </div>
    </div>
  </xsl:template>

</xsl:stylesheet>

<Where>
  <Geq>
    <FieldRef Name="DueDate" />
    <Value Type="DateTime">
      <Today />
    </Value>
  </Geq>
</Where>