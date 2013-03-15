<?xml version="1.0" encoding="iso-8859-1" ?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
	xmlns:usr="http://www.kohsuke.org/freetrain/system/mountainPalette/"
	>
	
	<xsl:template match="/">
		<html>
			<head>
				<title>Mountain Color Palette</title>
			</head>
			<style>
				td {
					text-align: center;
					font-size: 8pt;
				}
			</style>
			<body>
				<xsl:for-each select="/*/*">
					<h1><xsl:value-of select="local-name(.)"/></h1>
					<table><tr>
						<xsl:for-each select="color">
							<xsl:variable name="rgb">
								<xsl:call-template name="get-color"/>
							</xsl:variable>
							<td style="background-color:rgb({$rgb}); width:100px; height:100px">
								(<xsl:value-of select="$rgb"/>)
							</td>
						</xsl:for-each>
					</tr></table>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
	
	
	<!-- HSV to RGB conversion -->
	<msxsl:script language="JScript" implements-prefix="usr">
		function toRGB(hsvStr) {
			h = new Number(hsvStr.substring(0,hsvStr.indexOf(",")))/40.0;
			s = new Number(hsvStr.substring(hsvStr.indexOf(",")+1,hsvStr.lastIndexOf(",")))/240.0;
			v = new Number(hsvStr.substring(hsvStr.lastIndexOf(",")+1))/240.0;
			
			
			i = Math.floor(h);
			f = h-i;
			if( i==0 || i==2 || i==4 || i==6 )	f=1.0-f;
			
			m = 256.0*v*(1.0-s);
			n = 256.0*v*(1.0-s*f);
			o = 256.0*v;
			
			if(i==0)	return combine(o,n,m);
			if(i==1)	return combine(n,o,m); //+":"+v*240;
			if(i==2)	return combine(m,o,n);
			if(i==3)	return combine(m,n,o);
			if(i==4)	return combine(n,m,o);
			if(i==5)	return combine(o,m,n);
			if(i==6)	return combine(o,n,m);
			return "255,0,0";	// error
		}
		
		function combine(r,g,b) {
			return Math.floor(r)+","+Math.floor(g)+","+Math.floor(b);
		}
	</msxsl:script>
	
	<xsl:template name="get-color">
		<xsl:choose>
			<xsl:when test="@type='rgb'">
				<xsl:value-of select="text()"/>
			</xsl:when>
			<xsl:when test="@type='hsv'">
				<xsl:value-of select="usr:toRGB(string(text()))"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:message terminate="yes">
					unreconigzed type attribute "<xsl:value-of select="@type"/>"
				</xsl:message>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>
