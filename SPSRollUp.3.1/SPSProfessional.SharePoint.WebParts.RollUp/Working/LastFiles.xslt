<?xml version='1.0' encoding='utf-8'?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT">
  
  <xsl:output method="html" />    
  
  <xsl:template match="/">
    <table class="ms-summarycustombody">
      <tbody>
        <xsl:for-each select="Rows/Row">
          <xsl:sort select="Modified" order="descending"/>
          <tr>
            <td width="24px">
              <!-- Use MapToIcon to show the file icon -->
              <img src="_layouts/images/{sps:MapToIcon(FileRef,'')}" alt="{FileRef}"/>
            </td>
            <td width="100%">
              <xsl:value-of select="LinkFilename" />
              <!-- Use IfNew to show the new file icon -->
              <xsl:if test="sps:IfNew(substring-before(Created,' '))">
                <img src="_layouts/1033/images/new.gif" alt="New" />
              </xsl:if>
                <br/>
              <!-- Show the modified date -->
              <font color="#999999">
                (<xsl:value-of select="substring-before(Modified,' ')" />)
              </font>
            </td>
            <!-- Go to Folder -->
            <td width="24px">
              <a href="{substring-before(FileRef,LinkFilename)}">
                <img border="0" src="/_layouts/images/folder.gif" alt="Go to Folder"/>
              </a>
            </td>
          </tr>
        </xsl:for-each>
      </tbody>
    </table>
  </xsl:template>
</xsl:stylesheet>

<!--<Where>
  <Eq>
    <FieldRef Name="Author" />
    <Value Type='Text'>[UserName]</Value>
  </Eq>
</Where>-->
