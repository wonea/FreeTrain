<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:saxon="http://icl.com/saxon"
	extension-element-prefixes="saxon">
	
	<xsl:output method="html" encoding="UTF-8" />
	
	<xsl:template match="/">
		<saxon:output href="toc.html" method="html">
			<html>
				<head>
					<xsl:call-template name="css"/>
				</head>
				<body>
					<ul>
						<xsl:for-each select="//companies/company">
							<xsl:sort select="."/>
							
							<xsl:variable name="trains" select="//train[company=current()/text()]"/>
							<li>
								<a href="{generate-id(.)}.html" target="body">
									<xsl:value-of select="."/>
								</a>
								<small>
									<xsl:text> (</xsl:text>
									<xsl:value-of select="count($trains)"/>
									<xsl:text> trains)</xsl:text>
									<!--!<xsl:text>コ)</xsl:text>-->
								</small>
							</li>
							<saxon:output href="{generate-id(.)}.html" method="html">
								<html>
									<head>
										<title><xsl:value-of select="." />Train list</title>
										<!--!<title><xsl:value-of select="." />車両リスト</title>-->
										<xsl:call-template name="css"/>
									</head>
									<body>
										<h1><xsl:value-of select="." />Train list</h1>
										<!--!<h1><xsl:value-of select="." />車両リスト</h1>-->
										<xsl:apply-templates select="$trains">
											<xsl:sort select="name"/>
										</xsl:apply-templates>
									</body>
								</html>
							</saxon:output>
						</xsl:for-each>
					</ul>
					<p>Total number of trains: <xsl:value-of select="count(//train)"/></p>
					<!--!<p>全<xsl:value-of select="count(//train)"/>個</p>-->
				</body>
			</html>
		</saxon:output>
		<saxon:output href="index.html" method="html">
			<html>
				<head>
					<title>FreeTrain train list</title>
					<!--!<title>FreeTrain車両リスト</title>-->
				</head>
				<frameset cols="250,*">
					<frame src="toc.html" />
					<frame src="first.html" name="body" />
				</frameset>
			</html>
		</saxon:output>
		<saxon:output href="first.html" method="html">
			<html>
				<head>
					<xsl:call-template name="css"/>
				</head>
				<body>
					<h1>FreeTrain train list</h1>
					<!--!<h1>FreeTrain車両リスト</h1>-->
					<p>
						A collection of all published FreeTrain trains. Feel free to drop me a line if you are interested in the program that created this HTML file.
						<!--!私の知る範囲で公開されているFreeTrain用の車両を集めてみました。このHTMLを作るプログラム自体に興味のある人は私までどうぞ。-->
					</p>
				</body>
			</html>
		</saxon:output>
	</xsl:template>
	
	<xsl:template match="train">
		<h2><xsl:value-of select="name"/></h2>
		<table border="0">
			<tr>
				<td rowspan="3">
					<img src="{@id}.png"/>
				</td>
				<td><nobr>Author:</nobr></td>
				<!--!<td><nobr>作者：</nobr></td>-->
				<td>
					<xsl:value-of select="author"/>
				</td>
			</tr>
			<tr>
				<td><nobr>Speed:</nobr></td>
				<!--!<td><nobr>速度：</nobr></td>-->
				<td>
					<xsl:value-of select="speed"/>
				</td>
			</tr>
			<tr>
				<td><nobr>Description:</nobr></td>
				<!--!<td><nobr>説明：</nobr></td>-->
				<td>
					<xsl:value-of select="description"/>
				</td>
			</tr>
		</table>
	</xsl:template>
	
	
	<xsl:template name="css">
		<style>
			h2 {
				background-color: lightblue;
			}
		</style>
	</xsl:template>
</xsl:stylesheet>
