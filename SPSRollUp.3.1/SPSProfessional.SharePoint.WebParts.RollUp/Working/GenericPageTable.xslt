<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT">
  
  <xsl:output method="html" encoding="UTF-8" indent="yes" />
  
  <xsl:param name="CurrentPage" />
  
  <!-- Customize PageSize & MaxPageLinks -->
  <xsl:variable name="RecordCount" select="count(Row)" />
  <xsl:variable name="PageSize" select="10" />
  <xsl:variable name="MaxPagesLinks" select="3" />
  
  <!-- Main Template -->
  <xsl:template match="/Rows">
    
    <!-- Debug Use
    <br/>Current Page: <xsl:value-of select="$CurrentPage" />
    <br/>Record Count: <xsl:value-of select="$RecordCount" />
    <br/>Page size: <xsl:value-of select="$PageSize" />   
    <br/>
    -->
    
    <!-- Pages Toolbar -->
    <xsl:call-template name="DrawPagesToolbar" />
    
    <!-- Main table -->
    <table width="100%" class="ms-listviewtable" cellspacing="0" cellpadding="1" border="0" style="border-style: none; width: 100%; border-collapse: collapse;">
      <tbody>
        <xsl:call-template name="DrawHeader" />
        <xsl:for-each select="/Rows/Row">
          <xsl:call-template name="DrawRow" />
        </xsl:for-each>
      </tbody>
    </table>
  </xsl:template>

  <!-- TableHeader Template -->
  <xsl:template name="DrawHeader">
    <tr class="ms-viewheadertr" valign="top">
      <th class="ms-vh2-nofilter ms-vh2-gridview" nowrap="">
        Row
      </th>
      <xsl:for-each select="Row[1]/*">
        <xsl:if test="name()!='_RowNumber'">
          <th class="ms-vh2-nofilter ms-vh2-gridview" nowrap="">        
            <xsl:value-of select="sps:XmlDecode(name())" />        
          </th>
        </xsl:if>
      </xsl:for-each>
    </tr>
  </xsl:template>

  <!-- TableRow Template -->
  <xsl:template name="DrawRow">
    <xsl:choose>
      <xsl:when test="(position() &gt;= 1 + ($CurrentPage - 1) * $PageSize) and (position() &lt; (1 + $CurrentPage * $PageSize))">
        <tr class="" >
          <!-- Icon to send the row -->
          <td class="ms-vb-title">            
            <a href="{sps:Event('Select',_RowNumber)}">
              <img src="/_layouts/images/exptitem.gif" border="0" />
            </a>
          </td>
          <!-- All columns except _RowNumber -->
          <xsl:for-each select="node()">
            <xsl:if test="name()!='_RowNumber'">
              <td class="ms-vb2">
                <xsl:value-of select="." disable-output-escaping="yes" />
              </td>
            </xsl:if>
          </xsl:for-each>
        </tr>
      </xsl:when>
    </xsl:choose>
  </xsl:template>
  
  <!-- Pages Template -->
  <xsl:template name="DrawPagesToolbar">
    <table cellSpacing="0" class="ms-menutoolbar" cellPadding="0" border="0" width="100%">
      <tr>    
        <!-- Draw buttons for pages -->
        <xsl:for-each select="Row[((position() - 1) mod $PageSize = 0)]">
          <td class="ms-toolbar" nowrap="true" style="padding: 3px;">          
            <xsl:choose>
              <xsl:when test="(position() &gt; ($CurrentPage - ceiling($MaxPagesLinks div 2)) or position() &gt; (last() - $MaxPagesLinks)) and ((position() &lt; $CurrentPage + $MaxPagesLinks div 2) or (position() &lt; 1 + $MaxPagesLinks))">
                <xsl:if test="position()=$CurrentPage">
                  <xsl:value-of select="position()" />
                </xsl:if>
                <xsl:if test="position()!=$CurrentPage">
                  <b>
                    <a href="{sps:Event('Page',position())}">
                      <xsl:value-of select="position()" />
                    </a>
                  </b>
                </xsl:if>
              </xsl:when>
              <xsl:when test="position()=1">
                [ <a href="{sps:Event('Page',position())}">
                  First
                </a> ]
              </xsl:when>
              <xsl:when test="position()=last()">
                [ <a href="{sps:Event('Page',position())}">
                  Last
                </a> ]
              </xsl:when>
              <xsl:when test="(position() &gt; ($CurrentPage - ceiling($MaxPagesLinks div 2) - 1) or position() &gt; (last() - $MaxPagesLinks) - 1 ) and ((position() &lt; $CurrentPage + $MaxPagesLinks div 2 + 1) or (position() &lt; 2 + $MaxPagesLinks))">
                [ <a href="{sps:Event('Page',position())}">
                  ...
                </a> ]
              </xsl:when>
            </xsl:choose>
          </td>
        </xsl:for-each>
        <!-- Right Fill -->
        <td width="99%" nowrap="">
          <img width="1" height="18" alt="" src="/_layouts/images/blank.gif" />
        </td>
      </tr>
    </table>
  </xsl:template>
</xsl:stylesheet>
