<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:sps="http://schemas.spsprofessional.net/WebParts/SPSXSLT">
  <xsl:output method="html" encoding="UTF-8" indent="yes" />
  <xsl:param name="CurrentPage" />
  <xsl:param name="CurrentOrder" />
  <xsl:variable name="RecordCount" select="count(Row)" />
  <xsl:variable name="PageSize" select="5" />
  <xsl:variable name="MaxPagesLinks" select="3" />
  <!-- Main Template -->
  <xsl:template match="/Rows">
    <!--
    <br/>Current Page: <xsl:value-of select="$CurrentPage" />
    <br/>Record Count: <xsl:value-of select="$RecordCount" />
    <br/>Page size: <xsl:value-of select="$PageSize" />   
    <br/>
    -->
    <xsl:call-template name="ContextMenuScript" />
    <xsl:call-template name="ToolBar" />
    <table width="100%" class="ms-listviewtable" cellspacing="0" cellpadding="0" border="0" style="border-style: none; width: 100%; border-collapse: collapse;">
      <tbody>
        <xsl:call-template name="TableHeader" />
        <xsl:for-each select="/Rows/Row">
          <xsl:sort select="*[local-name()=$CurrentOrder]" data-type="text" order="ascending" />
          <xsl:call-template name="TableRow" />
        </xsl:for-each>
      </tbody>
    </table>
  </xsl:template>
  <!-- TableHeader Template -->
  <xsl:template name="TableHeader">
    <tr class="ms-viewheadertr" valign="top">
      <xsl:for-each select="Row[1]/*">        
          <xsl:if test="position()!=1">
          <th class="ms-vh2-nofilter ms-vh2-gridview" nowrap="">
            <!-- Using sps namespace -->
            <a href="{sps:Event('Order',name())}">
              <xsl:value-of select="name()" />
            </a>
          </th>
        </xsl:if>
      </xsl:for-each>
    </tr>
  </xsl:template>
  <!-- TableRow Template -->
  <xsl:template name="TableRow">
    <xsl:choose>
      <xsl:when test="(position() &gt;= 1 + ($CurrentPage - 1) * $PageSize) and (position() &lt; (1 + $CurrentPage * $PageSize))">
        <tr class="">        
          <xsl:for-each select="node()">
            <xsl:if test="position()!=1">
              <xsl:choose>
                <xsl:when test="position()=2">
                  <td class="ms-vb-title">
                    <xsl:call-template name="ContextMenu">
                      <xsl:with-param name="Row" select="../_RowNumber" />
                      <xsl:with-param name="Data" select="." />
                    </xsl:call-template>
                  </td>
                </xsl:when>
                <xsl:otherwise>
                  <td class="ms-vb2">
                    <xsl:value-of select="." disable-output-escaping="yes" />
                  </td>
                </xsl:otherwise>
              </xsl:choose>              
            </xsl:if>
          </xsl:for-each>
        </tr>
      </xsl:when>
    </xsl:choose>
  </xsl:template>
  <!-- ToolBar Template -->
  <xsl:template name="ToolBar">
    <table cellSpacing="0" class="ms-menutoolbar" cellPadding="0" border="0" width="100%">
      <tr>
        <xsl:call-template name="PageButtons" />
        <td width="99%" nowrap="">
          <img width="1" height="18" alt="" src="/_layouts/images/blank.gif" />
        </td>
      </tr>
    </table>
  </xsl:template>
  <!-- PageButtons Template -->
  <xsl:template name="PageButtons">
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
  </xsl:template>
  <!-- Context Menu -->
  <xsl:template name="ContextMenu">
    <xsl:param name="Row" />
    <xsl:param name="Data" />
    <xsl:if test="string-length($Data)">
    <table height="100%" cellspacing="0" 
           class="ms-unselectedtitle" 
           onmouseover="SPSOnItem(this,OptionsMenu1)" 
           CTXName="RollUpCtx" 
           ItemId="{$Row}" 
           MsoPnlId="data">
      <tr>
        <td width="100%" class="ms-vb">
          <xsl:value-of select="$Data" />
        </td>
        <td class="">
          <img src="/_layouts/images/blank.gif" width="13" style="visibility: hidden" alt="" />
          <xsl:element name ="div">
            <xsl:attribute name="style">
              <xsl:variable name="width" select="PercentComplete*100" />
              <xsl:value-of select="PercentComplete" />
            </xsl:attribute>
          </xsl:element>
        </td>
      </tr>
    </table>
    </xsl:if>
  </xsl:template>
  <!-- Context Menu Script template -->
  <xsl:template name="ContextMenuScript">
    <![CDATA[
    <script type="text/javascript">
    function OptionsMenu1(m,ctx)
    {
       var selectRow = 
        "]]><xsl:value-of select="sps:EventJS('Select','currentItemID')" /><![CDATA[";
       CAMOpt(m, "Select this Row", selectRow, "/_layouts/images/exptitem.gif");
        
       // Add custom options here
       // CAMOpt(m,"Sample","alert('Sample')","");
    }
    </script>
    ]]>
  </xsl:template>
  <xsl:template name="DropDownFields">
    <xsl:param name="dropdown_name" select="combo" />
    <select name="{$dropdown_name}" onchange="javascript:combo_onchange(this)" class="ms-imput">
      <xsl:for-each select="Row[1]/*">
        <option value="name()">
          <xsl:value-of select="name()" />
        </option>
      </xsl:for-each>
    </select>
  </xsl:template>
  <xsl:template name="DropDownScript">
    <![CDATA[
     <script>
      function combo_onchange(oSelect) {
        if(oSelect.selectedIndex != -1)
        __doPostBack('',oSelect.options[oSelect.selectedIndex].text);
        }
      </script>
    ]]>
  </xsl:template>
</xsl:stylesheet>