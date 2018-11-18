<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT">
  <xsl:output method="xml" indent="yes" />
  <xsl:template match="@* | node()">
    <!-- Graph -->
    <SPSCalendar ViewType="month">
      <xsl:apply-templates />
    </SPSCalendar>
  </xsl:template>
  <!-- Each row -->
  <xsl:template match="Row">
    <!-- Put here your fields -->
    <xsl:element name="SPSCalendarItem">
      
      <xsl:if test="string(StartDate)">
        <xsl:attribute name="StartDate">
          <xsl:value-of select="sps:FormatDateTime(StartDate,'u')"/>
        </xsl:attribute>
      </xsl:if>
      
      <xsl:if test="string(DueDate)">
        <xsl:attribute name="EndDate">
          <xsl:value-of select="sps:FormatDateTime(DueDate,'u')"/>
        </xsl:attribute>
      </xsl:if>
      
      <xsl:attribute name="Title">
        <xsl:value-of select="Title"/>ç
        <xsl:text>SPSCal_Red</xsl:text>
      </xsl:attribute>
    </xsl:element>
    <!-- 
    <SPSCalendarItem StartDate="{StartDate}" EndDate="{DueDate}" Title="{Title}" />
    -->
  </xsl:template>
</xsl:stylesheet>

<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT">
  <xsl:output method="xml" indent="yes" />
  <xsl:template match="@* | node()">
    <!-- Our Calendar View Calendar -->
    <SPSCalendar ViewType="week">
      <xsl:apply-templates />
    </SPSCalendar>
  </xsl:template>
  <!-- Each row -->
  <xsl:template match="Row">
    <!-- Put here your fields -->
    <xsl:element name="SPSCalendarItem">
      
      <!-- StartDate -->
      <xsl:if test="string(StartDate)">
        <xsl:attribute name="StartDate">
          <xsl:value-of select="sps:FormatDateTime(StartDate,'s')" />
        </xsl:attribute>
      </xsl:if>

      <!-- EndDate -->
      <xsl:choose>
        <xsl:when test="string(DueDate)">
          <xsl:attribute name="EndDate">
            <xsl:value-of select="sps:FormatDateTime(DueDate,'s')" />
          </xsl:attribute>
        </xsl:when>        
        <xsl:otherwise>
          <!-- If no DueDate specified use StartDate -->
          <xsl:if test="string(StartDate)">
            <xsl:attribute name="EndDate">
              <xsl:value-of select="sps:FormatDateTime(StartDate,'s')" />
            </xsl:attribute>
          </xsl:if>
        </xsl:otherwise>        
      </xsl:choose>

      <!-- Title -->
      <xsl:attribute name="Title">
        <xsl:value-of select="Title" />
      </xsl:attribute>
      
      <!-- DisplayFormUrl -->
      <xsl:attribute name="DisplayFormUrl">
        <xsl:value-of select="substring-before (_ItemUrl, '?' )" />
      </xsl:attribute>
      
      <!-- ItemID -->
      <xsl:attribute name="ItemID">
        <xsl:value-of select="_ItemId" />
      </xsl:attribute>
      
      <!-- BackgroundClassName -->
      <xsl:attribute name="BackgroundColorClassName">
        <xsl:value-of select="sps:GetCalColor()" />
      </xsl:attribute>
    </xsl:element>

  </xsl:template>
</xsl:stylesheet>