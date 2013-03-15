<?xml version="1.0" encoding="Shift_JIS" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output encoding="Shift_JIS" method="html" />
	<xsl:template match="/">
		<html>
			<head>
				<meta http-equiv="Content-type" content="text/html; charset=shift_jis" />
			</head>
			<body>
				<table border="1">
					<tr><td>
						���O
					</td><td>
						���i/�~�n
					</td><td>
						�~�n�T�C�Y
					</td><td>
						�t���A�����艿�i
					</td></tr>
						
					<xsl:for-each select="root/item">
						<xsl:sort select="@pricePerArea" data-type="number" order="descending"/>
						
						<tr><td>
							<xsl:value-of select="@name" />
						</td><td>
							<xsl:value-of select="@pricePerArea" />
						</td><td>
							<xsl:value-of select="@size" />
						</td><td style="text-align:right;">
							<xsl:value-of select="@price" />
						</td></tr>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
