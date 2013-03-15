<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output encoding="Shift_JIS" />
	<xsl:template match="/">
		<xsl:processing-instruction name="xml-stylesheet">
			type="text/xsl" href="populationTable.xsl"
		</xsl:processing-instruction>
		<root>
			<xsl:for-each select="//contribution[@type='land' or @type='commercial' or @type='varHeightBuilding']">
				<item type="{@type}" id="{@id}">
					<name>
						<xsl:choose>
							<xsl:when test="name"><xsl:value-of select="name"/></xsl:when>
							<xsl:when test="group"><xsl:value-of select="group"/></xsl:when>
							<xsl:otherwise>unknown</xsl:otherwise>
						</xsl:choose>
					</name>
					<xsl:copy-of select="population"/>
				</item>
			</xsl:for-each>
		</root>
	</xsl:template>
</xsl:stylesheet>
