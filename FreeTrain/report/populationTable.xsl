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
			<h1>�����l���ꗗ</h1>
			<table border="1">
				<thead>
					<tr><td>
						������
					</td><td>
						ID
					</td><td>
						�l���ݒ�
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
		�Z��n(<xsl:value-of select="base"/>�l)
	</xsl:template>
	
	<xsl:template match="population[class/@name='freetrain.contributions.population.OfficePopulation']">
		�I�t�B�X�n(<xsl:value-of select="base"/>�l)
	</xsl:template>
	
	<xsl:template match="population[class/@name='freetrain.contributions.population.AgriculturalPopulation']">
		�_�ƌn(<xsl:value-of select="base"/>�l)
	</xsl:template>
	
	<xsl:template match="population[class/@name='freetrain.contributions.population.ShopperPopulation']">
		�������q�n(<xsl:value-of select="base"/>�l)
	</xsl:template>
	
	<xsl:template match="population[class/@name='freetrain.contributions.population.RestaurantPopulation']">
		���H�X�n(<xsl:value-of select="base"/>�l)
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
		���m
	</xsl:template>
</xsl:stylesheet>
