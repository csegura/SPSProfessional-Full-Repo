<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
                xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT">
  <xsl:output method="html" />
  <xsl:template match="/">
    <table class="ms-summarycustombody">
      <tr>
        <th>Task</th>
        <th>Due date</th>
        <th>Status</th>
      </tr>
      <xsl:for-each select="Rows/Row">
        <tr>
          <td>
            <!-- Link to task -->
            <a href="{_ItemUrl}">
              <xsl:value-of select="Title" />
            </a>
          </td>
          <td>
            <!-- Show the DueDate -->
            <p style="color:#ff0000;text-align:right">
              <xsl:value-of select="substring-before(DueDate,' ')" />
            </p>
          </td>
          <td>
            <!-- Current status -->
            <xsl:value-of select="Status" />
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>
</xsl:stylesheet>

<!-- My Tasks -->
<Where>
  <Or>
    <Eq>
      <FieldRef Name="AssignedTo" LookupId="TRUE"/>
      <Value Type="int">
        <UserID />
      </Value>
    </Eq>
    <Membership Type="CurrentUserGroups">
      <FieldRef Name="AssignedTo"/>
    </Membership>
  </Or>
</Where>

<Where>
  <Neq>
    <FieldRef Name="Status"/>
    <Value Type="Lookup">Completed</Value>
  </Neq>
</Where>

<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:sps="http://schemas.spsprofessional.com/WebParts/SPSXSLT">
  <xsl:output method="html" />
  <xsl:template match="/">
    <table class="ms-summarycustombody">
      <tr>
        <th>Task</th>
        <th>Due date</th>
        <th>Status</th>
      </tr>
      <xsl:for-each select="Rows/Row">
        <tr>
          <td>
            <a href="{_ItemUrl}">
              <xsl:value-of select="Title" />
            </a>
          </td>
          <td>
            <!-- Show the DueDate -->
            <p style="color:#ff0000;text-align:right">
              <xsl:value-of select="substring-before(DueDate,' ')" />
            </p>
          </td>
          <td class="">
            <p>
              <xsl:choose>
                <xsl:when test="Status='Completed'">
                  <img src="_layouts/images/flag_green.png" />
                </xsl:when>
                <xsl:when test="Status='In Progress'">
                  <img src="_layouts/images/flag_orange.png" />
                </xsl:when>
                <xsl:when test="Status='Deferred'">
                  <img src="_layouts/images/flag_blue.png" />
                </xsl:when>
                <xsl:when test="Status='Not Started'">
                  <img src="_layouts/images/flag_red.png" />
                </xsl:when>
                <xsl:otherwise>
                  <img src="_layouts/images/flag_black.PNG" />
                </xsl:otherwise>
              </xsl:choose>
            </p>
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>
</xsl:stylesheet>