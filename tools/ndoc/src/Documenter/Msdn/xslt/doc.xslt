<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:doc="http://ndoc.sf.net/doc"
	exclude-result-prefixes="doc"
>
	<xsl:key name="docs" match="doc:template" use="following-sibling::xsl:template[1]/@match" />

	<xsl:template match="/">
		<html>
			<head>
				<title>NDoc Supported Tags for Documentation Comments</title>
			</head>
			<body>
				<xsl:apply-templates select="*/xsl:template[@doc:group][not(@doc:group = following-sibling::xsl:template/@doc:group)]/@doc:group" />
			</body>
		</html>
	</xsl:template>

	<xsl:template match="@doc:group">
		<h3>
			<xsl:value-of select="." />
			<xsl:text> tags</xsl:text>
		</h3>
		<table width="100%" border="1">
			<xsl:apply-templates select="/*/xsl:template[@doc:group = current()]">
				<xsl:sort select="@match" />
			</xsl:apply-templates>
		</table>
	</xsl:template>

	<xsl:template match="xsl:template">
		<tr bgcolor="#EEEEEE">
			<td>
				<xsl:value-of select="@match" />
			</td>
			<td>
				<xsl:choose>
					<xsl:when test="@doc:msdn">
						<a href="{@doc:msdn}">MSDN</a>
					</xsl:when>
					<xsl:otherwise>&#xA0;</xsl:otherwise>
				</xsl:choose>
			</td>
		</tr>
		<tr>
			<td colspan="2">
				<xsl:apply-templates select="key('docs', @match)/summary/node()" mode="doc" />
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="node()|@*" mode="doc">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()" mode="doc" />
		</xsl:copy>
	</xsl:template>

</xsl:stylesheet>
