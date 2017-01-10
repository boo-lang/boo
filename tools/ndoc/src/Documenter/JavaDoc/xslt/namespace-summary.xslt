<?xml version="1.0" encoding="utf-8" ?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- -->
	<xsl:include href="javadoc.xslt" />
	<!-- -->
	<xsl:param name="global-path-to-root" />
	<xsl:param name="global-namespace-name" />
	<!-- -->
	<xsl:template match="/">
		<xsl:variable name="namespace" select="ndoc/assembly/module/namespace[@name=$global-namespace-name]" />
		<html>
			<xsl:call-template name="output-head" />
			<body>
				<xsl:variable name="navigation-bar">
					<xsl:call-template name="output-navigation-bar">
						<xsl:with-param name="select" select="'Namespace'" />
						<xsl:with-param name="prev-next-what" select="'NAMESPACE'" />
					</xsl:call-template>
				</xsl:variable>
				<xsl:copy-of select="$navigation-bar" />
				<hr />
				<h2 class="title">
					<xsl:value-of select="$namespace/@name" />
					<xsl:text> Namespace Summary</xsl:text>
				</h2>
				<xsl:if test="$namespace/documentation/summary">
					<p>
						<xsl:apply-templates select="$namespace/documentation/summary" mode="doc" />
					</p>
				</xsl:if>
				<xsl:variable name="interfaces" select="$namespace/interface" />
				<xsl:if test="$interfaces">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th colspan="2">Interface Summary</th>
							</tr>
						</thead>
						<xsl:apply-templates select="$interfaces">
							<xsl:sort select="@name" />
						</xsl:apply-templates>
					</table>
					<br />
				</xsl:if>
				<xsl:variable name="classes" select="$namespace/class" />
				<xsl:if test="$classes">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th colspan="2">Class Summary</th>
							</tr>
						</thead>
						<xsl:apply-templates select="$classes">
							<xsl:sort select="@name" />
						</xsl:apply-templates>
					</table>
					<br />
				</xsl:if>
				<xsl:variable name="structures" select="$namespace/structure" />
				<xsl:if test="$structures">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th colspan="2">Structure Summary</th>
							</tr>
						</thead>
						<xsl:apply-templates select="$structures">
							<xsl:sort select="@name" />
						</xsl:apply-templates>
					</table>
					<br />
				</xsl:if>
				<xsl:variable name="delegates" select="$namespace/delegate" />
				<xsl:if test="$delegates">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th colspan="2">Delegate Summary</th>
							</tr>
						</thead>
						<xsl:apply-templates select="$delegates">
							<xsl:sort select="@name" />
						</xsl:apply-templates>
					</table>
					<br />
				</xsl:if>
				<xsl:variable name="enumerations" select="$namespace/enumeration" />
				<xsl:if test="$enumerations">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th colspan="2">Enumeration Summary</th>
							</tr>
						</thead>
						<xsl:apply-templates select="$enumerations">
							<xsl:sort select="@name" />
						</xsl:apply-templates>
					</table>
					<br />
				</xsl:if>
				<hr />
				<xsl:copy-of select="$navigation-bar" />
			</body>
		</html>
	</xsl:template>
	<xsl:template match="interface|class|structure|delegate|enumeration">
		<tr>
			<td class="name">
				<a>
					<xsl:attribute name="href">
						<xsl:call-template name="get-href-to-type">
							<xsl:with-param name="type-name" select="@name" />
						</xsl:call-template>
					</xsl:attribute>
					<xsl:value-of select="@name" />
				</a>
			</td>
			<td class="type">
				<xsl:choose>
					<xsl:when test="documentation/summary">
						<xsl:value-of select="documentation/summary" />
					</xsl:when>
					<xsl:otherwise>&#160;</xsl:otherwise>
				</xsl:choose>
			</td>
		</tr>
	</xsl:template>
	<!-- -->
</xsl:transform>
