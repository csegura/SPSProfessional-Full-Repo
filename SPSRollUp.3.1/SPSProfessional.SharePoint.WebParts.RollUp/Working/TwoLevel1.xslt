<?xml version='1.0' encoding='utf-8'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT"
                exclude-result-prefixes="msxsl sps">
  <xsl:output method="html" />

  <!-- Group by Lists -->
  <xsl:key name="KGroup" match="Row" use="_ListTitle" />
 
  <xsl:template match="Rows">

    <table width="100%">
      <!-- Select distinct for Lists -->
      <xsl:for-each select="Row[count(. | key('KGroup', _ListTitle)[1]) = 1]">
        <xsl:sort select="_SiteTitle" />

        <!-- Collapsable Header -->
        <xsl:call-template name="ExpandableHeader">
          <xsl:with-param name="Title" select="_ListTitle" />
        </xsl:call-template>
        
        <!-- Details -->
        <xsl:for-each select="key('KGroup', _ListTitle)">
          <tr style="display:none;">
            <td> 
              <!-- Empty Column -->
            </td>
            <td>
              <xsl:value-of select="Title"/>
            </td>
            <td>
              <xsl:value-of select="sps:FormatDateTime(StartDate,'m')" />
            </td>
            <td>
              <xsl:value-of select="sps:FormatDateTime(DueDate,'m')" />
            </td>
            <td>
              <xsl:choose>
                <xsl:when test="DueDate!=''">
                  <xsl:value-of select="sps:DateDiff('D',DueDate,StartDate)" />
                </xsl:when>
              </xsl:choose>
            </td>
          </tr>                
        </xsl:for-each>
      </xsl:for-each>

    </table>
  </xsl:template> 

  <xsl:template name="Collapsable">
    <xsl:param name="Title" />
    <tr id="group0" >
      <td class="ms-gb" colspan="2">
        <span class="ms-announcementtitle">
          <a href="javascript:" onclick="javascript:ExpGroupBy(this);return false;">
            <xsl:element name ="img">
              <xsl:attribute name ="src">/_layouts/images/minus.gif</xsl:attribute>
              <xsl:attribute name ="border">0</xsl:attribute>
            </xsl:element >
          </a>
          <![CDATA[ ]]>
          <xsl:value-of select="$Title" />
        </span>
      </td>
      <td class="ms-gb">StartDate</td>
      <td class="ms-gb">DueDate</td>
      <td class="ms-gb">Diff</td>
    </tr>
  </xsl:template>

  <xsl:template name="ExpandableHeader">
    <xsl:param name="Title" />
    <tr id="group0" >
      <td class="ms-gb" colspan="2">
        <span class="ms-announcementtitle">
          <a href="javascript:" onclick="javascript:ExpGroupBy(this);return false;">
            <xsl:element name ="img">
              <xsl:attribute name ="src">/_layouts/images/plus.gif</xsl:attribute>
              <xsl:attribute name ="border">0</xsl:attribute>
            </xsl:element >
          </a>
          <![CDATA[ ]]>
          <xsl:value-of select="$Title" />
        </span>
      </td>
      <td class="ms-gb">StartDate</td>
      <td class="ms-gb">DueDate</td>
      <td class="ms-gb">Diff</td>
    </tr>
  </xsl:template>

</xsl:stylesheet>

<?xml version='1.0' encoding='utf-8'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT"
                exclude-result-prefixes="msxsl sps">
  <xsl:output method="html" />
  
  <xsl:key name="group-by-siteTitle" match="Row" use="_SiteTitle" />

  <!-- First Level -->
  <xsl:key name="KGroup" match="Row" use="_SiteTitle" />

  <!-- Select distinct for categories -->
  <xsl:key name="KSubGroup" match="Row" use="concat(_SiteTitle,'-',_ListTitle)" />

  <xsl:template match="Rows">

    <table width="100%">
      <!-- Select distinct for Parts -->
      <xsl:for-each select="Row[count(. | key('KGroup', _SiteTitle)[1]) = 1]">
        <xsl:sort select="_SiteTitle" />

        <xsl:call-template name="Collapsable">
          <xsl:with-param name="Title" select="_SiteTitle" />
        </xsl:call-template>
        <tr>
          <td>
            <table width="100%">
              <xsl:call-template name="SecondGroup">
                <xsl:with-param name="Part" select="_SiteTitle" />
              </xsl:call-template>
            </table>
          </td>
        </tr>
      </xsl:for-each>

    </table>
  </xsl:template>

  <!-- Search Parts in a Continent -->
  <xsl:template name="SecondGroup">        
    <xsl:param name="Part" />

    <xsl:variable name="VSubGroup" select="key('KGroup', _SiteTitle)" />
    <xsl:variable name="NSubGroup" select="$VSubGroup[generate-id() =
                             generate-id(
                               key('KSubGroup',
                                   concat(_SiteTitle, '-', _ListTitle))[1])]" />    

    <!-- Again select distinct Continent -->
    <!--<xsl:for-each select="$NSubGroup">
      <xsl:sort select="_ListTitle"/>-->
      
      <xsl:choose>
        <!-- If no data do nothing -->
        <xsl:when test="count($NSubGroup) = 0">
        </xsl:when>
        <!-- Else show header and data -->
        <xsl:otherwise>
          <!-- Collapsable Header -->
          <xsl:call-template name="Collapsable">
            <xsl:with-param name="Title" select="concat(_SiteTitle, '-', _ListTitle)" />
          </xsl:call-template>               
          <!-- Show the data -->
          <xsl:for-each select="$NSubGroup">
            <tr>
            <td>
              <![CDATA[ ]]>
            </td>
              <td>
                <xsl:value-of select="($NSubGroup)/Title"/>
              </td>
            </tr>
          </xsl:for-each>
        </xsl:otherwise>
      </xsl:choose>
    <!--</xsl:for-each>-->
  </xsl:template>
  
  <xsl:template name="Collapsable">
    <xsl:param name="Title" />
    <tr id="group0" >
      <td class="ms-gb" colspan="2">
        <span class="ms-announcementtitle">
          <a href="javascript:" onclick="javascript:ExpGroupBy(this);return false;">
            <xsl:element name ="img">
              <xsl:attribute name ="src">/_layouts/images/minus.gif</xsl:attribute>             
              <xsl:attribute name ="border">0</xsl:attribute>
            </xsl:element >
          </a>                        
          <![CDATA[ ]]>                        
          <xsl:value-of select="$Title" />
        </span>
      </td>
    </tr>
  </xsl:template>

  <xsl:template name="Expandable">
    <xsl:param name="Title" />
    <tr id="group0" >
      <td class="ms-gb" colspan="2">
        <span class="ms-announcementtitle">
          <a href="javascript:" onclick="javascript:ExpGroupBy(this);return false;">
            <xsl:element name ="img">
              <xsl:attribute name ="src">/_layouts/images/plus.gif</xsl:attribute>
              <xsl:attribute name ="border">0</xsl:attribute>
            </xsl:element >
          </a>
          <![CDATA[ ]]>
          <xsl:value-of select="$Title" />
        </span>
      </td>
    </tr>
  </xsl:template>
  
</xsl:stylesheet>