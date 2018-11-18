<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT"
                exclude-result-prefixes="msxsl sps">
  
  <xsl:output method="html" />

  <xsl:key name="group-by-sites" match="Row" use="_SiteTitle" />
  <xsl:key name="group-by-lists" match="Row" use="concat(_SiteTitle,_ListTitle)" />


  <xsl:template match="Rows">
    <xsl:for-each select="Row[count(. | key('group-by-sites', _SiteTitle)[1]) = 1]">
      <xsl:sort select="_SiteTitle" />
      <b>
        <xsl:value-of select="_SiteTitle" />
      </b><br />
      <xsl:call-template name="ListsInSite"></xsl:call-template>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="ListsInSite">
    <xsl:variable name="Site" select="key('group-by-sites', _SiteTitle)" />
    <xsl:for-each
        select="$Site[generate-id() =
                             generate-id(
                               key('group-by-lists',
                                   concat(_SiteTitle,_ListTitle))[1])]">
      - <xsl:value-of select="_ListTitle" />
      <br/>
      <xsl:call-template name="Details">
        <xsl:with-param name="Site" select="_SiteTitle" />
        <xsl:with-param name="List" select="_ListTitle" />
      </xsl:call-template>
    </xsl:for-each>

  </xsl:template>

  <xsl:template name="Details">
    <xsl:param name="Site" />
    <xsl:param name="List" />
    <xsl:for-each select="key('group-by-lists',concat($Site,$List))">
      <xsl:value-of select="Title" />
      <br/>
    </xsl:for-each>
  </xsl:template>
  
</xsl:stylesheet>

<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT"
                exclude-result-prefixes="msxsl sps">

  <xsl:output method="html" />

  <xsl:key name="lists-by-sites" match="Row" use="concat(_SiteTitle,_ListTitle)" />
  <xsl:key name="lists" match="Row" use="_SiteTitle" />

  <xsl:template match="Rows">
    <xsl:for-each select="Row[count(. | key('lists-by-sites', concat(_SiteTitle,_ListTitle))[1]) = 1]">
      <xsl:sort select="_SiteTitle" />
      <b>
        <xsl:value-of select="_SiteTitle" />
      </b>
      <br />
      <xsl:for-each select="key('lists-by-sites', concat(_SiteTitle,_ListTitle))">
        <xsl:sort select="_ListTitle" />
        <blocquote>
          * <xsl:value-of select="Title" /> (<xsl:value-of select="_ListTitle" />)<br />
        </blocquote>
      </xsl:for-each>
      <xsl:for-each select="key('lists', _SiteTitle)">
        <xsl:sort select="_ListTitle" />
        <blocquote>
          <xsl:value-of select="Title" /> (<xsl:value-of select="_ListTitle" />)<br />
        </blocquote>
      </xsl:for-each>
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>