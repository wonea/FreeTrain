<?xml version="1.0" encoding="Shift_JIS" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output encoding="Shift_JIS" />
	<xsl:template match="/">
		<html><head>
			<style>
				thead tr td {
					background-color: black;
					color: white;
					font-weight: bold;
				}
			</style>
		</head><body>
			<h1>建物人口一覧</h1>
			<table border="1">
				<thead>
					<tr><td>
						建物名
					</td><td>
						ID
					</td><td>
						人口設定
					</td></tr>
				</thead>
				<xsl:for-each select="/root/item">
					<xsl:sort select="population/base" data-type="number" order="descending"/>
					<tr><td>
						<xsl:value-of select="name"/>
					</td><td>
						<span style="font-size: 0.5em">
							<xsl:value-of select="@id"/>
						</span>
					</td><td>
						<xsl:apply-templates select="population"/>
					</td></tr>
				</xsl:for-each>
			</table>
		</body></html>
	</xsl:template>
	
	<xsl:template match="population[class/@name='freetrain.contributions.population.ResidentialPopulation']">
		住宅系(<xsl:value-of select="base"/>人)
	</xsl:template>
	
	<xsl:template match="population[class/@name='freetrain.contributions.population.OfficePopulation']">
		オフィス系(<xsl:value-of select="base"/>人)
	</xsl:template>
	
	<xsl:template match="population[class/@name='freetrain.contributions.population.AgriculturalPopulation']">
		農業系(<xsl:value-of select="base"/>人)
	</xsl:template>
	
	<xsl:template match="population[class/@name='freetrain.contributions.population.ShopperPopulation']">
		買い物客系(<xsl:value-of select="base"/>人)
	</xsl:template>
	
	<xsl:template match="population[class/@name='freetrain.contributions.population.RestaurantPopulation']">
		飲食店系(<xsl:value-of select="base"/>人)
	</xsl:template>
	
	<xsl:template match="population[class/@name='freetrain.contributions.population.CombinationPopulation']">
		<xsl:for-each select="population">
			<xsl:if test="position()!=1">
				<xsl:text>+</xsl:text>
			</xsl:if>
			<xsl:apply-templates select="."/>
		</xsl:for-each>
	</xsl:template>

	<xsl:template match="population">
		未知
	</xsl:template>
</xsl:stylesheet>
