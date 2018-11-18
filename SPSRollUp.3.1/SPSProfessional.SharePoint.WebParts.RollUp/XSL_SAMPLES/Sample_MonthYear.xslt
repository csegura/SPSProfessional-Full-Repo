<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:sps="http://schemas.spsprofessional.net/WebParts/SPSXSLT">
 
  <xsl:output method="html" indent="yes"/>

  <!-- Main Template -->
  <xsl:template match="Rows">

    <script>
      <![CDATA[
    // Month Year Array
    var MY = new Array();
    // Month Names
    var monthname=new Array("January",
                             "February",
                             "March",
                             "April",
                             "May",
                             "June",
                             "July",
                             "August",
                             "September",
                             "October",
                             "November",
                             "December");
    var sYear;
    var sMonth;
    ]]>
      <xsl:for-each select="Row">
        MYAdd(<xsl:value-of select="sps:YearNumber(Created)"/>,<xsl:value-of select="sps:MonthNumber(Created)"/>);
      </xsl:for-each>
      <![CDATA[    
    // Sort the array
    MY.sort(sortYear);

    // Render
    for(var y=0;y<MY.length;y++)
    {
      sYear = MY[y][0]; 
      for(var m=11;m>=0;m--)
      {
        sMonth = monthname[m];
        if (MY[y][1][m]!=null)
          document.writeln(sMonth+" "+sYear+" ("+MY[y][1][m]+")<br/>");
      }
    }

    // MYAdd<
    // MY - Array of years
    //    - each element contains the year and 
    //      an array of Months (counters)
    function MYAdd(year, month)
    {
      var iY = searchYear(year);
      
      if (iY >= 0)
      {
         if (MY[iY][1][month-1] != null)
         {
             MY[iY][1][month-1] += 1;
         }
         else
         {
             MY[iY][1][month-1] = 1;
         }      
      }
      else
      {
        MY.push(new Array(year,new Array(12)));   
        MY[MY.length-1][1][month-1] = 1;    
      }
    }

    // Search a year in the array
    function searchYear(year)
    {
       for(var y=0;y<MY.length;y++)
       {
          if (MY[y][0]==year)
          {
            return y;
          }
       }
       return -1;
    }

    // Sort the years in array 
    // M > m 
    function sortYear(a, b)
    {   
       return b[0]-a[0];
    }
    </script>
    ]]>
    </script>
  </xsl:template>
  
</xsl:stylesheet>

