<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:user="http://mycompany.com/mynamespace"
	version="1.0">
	
	<xsl:output encoding="Shift_JIS" />
	
  <msxsl:script language="JScript" implements-prefix="user">
    function area(s) {
      idx = s.indexOf(',');
      return new Number( s.substring(0,idx)) * new Number( s.substring(idx+1,s.length) );
    }
    function ppa(a,b) {
      return Math.floor(a/b);
    }
  </msxsl:script>
	
	<xsl:template match="/">
		<xsl:processing-instruction name="xml-stylesheet">type="text/xsl" href="priceDisplay.xsl"</xsl:processing-instruction>
		<root>
			<xsl:for-each select="//contribution[@type='commercial' or @type='varHeightBuilding']">
			  <xsl:sort select="price"/>
			  
			  <xsl:variable name="area" select="user:area(string(size))"/>
				<item price="{price}" type="{@type}" size="{size}"
					area="{$area}" pricePerArea="{user:ppa(number(price),number($area))}">
					
					<xsl:attribute name="name">
						<xsl:choose>
							<xsl:when test="name">
								<xsl:value-of select="name"/>
							</xsl:when>
							<xsl:when test="group">
								<xsl:value-of select="group"/>
							</xsl:when>
						</xsl:choose>
					</xsl:attribute>
				</item>
			</xsl:for-each>
		</root>
	</xsl:template>
</xsl:stylesheet>
