<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- -->
	<xsl:output method="html" indent="no" />
	<!-- -->
	<xsl:include href="common.xslt" />
	<!-- -->
	<xsl:param name='namespace' />
	<!-- -->
	<xsl:template match="/">
		<xsl:variable name="ns" select="ndoc/assembly/module/namespace[@name=$namespace]" />
		<html dir="LTR">
			<xsl:call-template name="html-head">
				<xsl:with-param name="title" select="concat($ns/@name, 'Hierarchy')" />
			</xsl:call-template>
			<body id="bodyID" class="dtBODY">
				<xsl:call-template name="title-row">
					<xsl:with-param name="type-name" select="concat($ns/@name, ' Hierarchy')" />
				</xsl:call-template>
				<div id="nstext">
					<a>
						<xsl:attribute name="href">
							<xsl:call-template name="get-filename-for-system-type">
								<xsl:with-param name="type-name" select="'System.Object'" />
							</xsl:call-template>
						</xsl:attribute>
						<xsl:text>System.Object</xsl:text>
					</a>
					<br />
					<xsl:variable name="context" select="$ns//*[local-name()='class' or local-name()='structure' or local-name()='base']" />
					<xsl:variable name="roots" select="$ns//*[(local-name()='class' and not(base)) or (local-name()='structure' and not(base)) or (local-name()='base' and not(base))]" />
					<xsl:call-template name="call-draw">
						<xsl:with-param name="context" select="$context" />
						<xsl:with-param name="nodes" select="$roots" />
						<xsl:with-param name="level" select="1" />
					</xsl:call-template>
					<br />
					<xsl:if test="$ns/interface">
						<h3 class="dtH3">Interfaces</h3>
						<p>
							<xsl:apply-templates select="$ns/interface">
								<xsl:sort select="@name" />
							</xsl:apply-templates>
						</p>
					</xsl:if>
					<xsl:call-template name="footer-row">
						<xsl:with-param name="type-name" select="concat($ns/@name, ' Hierarchy')" />
					</xsl:call-template>
				</div>
			</body>
		</html>
	</xsl:template>
	<!-- -->
	<xsl:template name="call-draw">
		<xsl:param name="context" />
		<xsl:param name="nodes" />
		<xsl:param name="level" />
		<xsl:for-each select="$nodes">
			<xsl:sort select="@name" />
			<xsl:if test="position() = 1">
				<xsl:variable name="head" select="." />
				<xsl:call-template name="draw">
					<xsl:with-param name="context" select="$context" />
					<xsl:with-param name="head" select="$head" />
					<xsl:with-param name="tail" select="$nodes[@name != $head/@name]" />
					<xsl:with-param name="level" select="$level" />
				</xsl:call-template>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>
	<!-- -->
	<xsl:template name="draw">
		<xsl:param name="context" />
		<xsl:param name="head" />
		<xsl:param name="tail" />
		<xsl:param name="level" />
		<xsl:call-template name="indent">
			<xsl:with-param name="count" select="$level" />
		</xsl:call-template>
		<xsl:text>-</xsl:text>
		<a>
			<xsl:attribute name="href">
				<xsl:choose>
					<xsl:when test="starts-with($head/@id, 'T:System.')">
						<xsl:call-template name="get-filename-for-system-type">
							<xsl:with-param name="type-name" select="substring-after($head/@id, 'T:')" />
						</xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="get-filename-for-type">
							<xsl:with-param name="id" select="$head/@id" />
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<xsl:call-template name="get-datatype">
				<xsl:with-param name="datatype" select="substring-after($head/@id, 'T:')" />
			</xsl:call-template>
		</a>
		<br />
		<xsl:variable name="derivative-classes" select="/ndoc/assembly/module/namespace/class[base/@id = $head/@id and @id = $context/@id] | /ndoc/assembly/module/namespace/class/descendant::base[base[@id = $head/@id] and @id = context/@id]" />
		<xsl:variable name="derivative-structures" select="/ndoc/assembly/module/namespace/structure[base/@id = $head/@id and @id = $context/@id] | /ndoc/assembly/module/namespace/structure/descendant::base[base[@id = $head/@id] and @id = context/@id]" />
		<xsl:variable name="derivatives" select="$derivative-classes | $derivative-structures" />
		<xsl:if test="$derivatives">
			<xsl:call-template name="call-draw">
				<xsl:with-param name="context" select="$context" />
				<xsl:with-param name="nodes" select="$derivatives" />
				<xsl:with-param name="level" select="$level + 1" />
			</xsl:call-template>
		</xsl:if>
		<xsl:if test="$tail">
			<xsl:call-template name="call-draw">
				<xsl:with-param name="context" select="$context" />
				<xsl:with-param name="nodes" select="$tail" />
				<xsl:with-param name="level" select="$level" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="indent">
		<xsl:param name="count" />
		<xsl:if test="$count &gt; 0">
			<xsl:text>&#160;&#160;</xsl:text>
			<xsl:call-template name="indent">
				<xsl:with-param name="count" select="$count - 1" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="interface">
		<a>
			<xsl:attribute name="href">
				<xsl:call-template name="get-filename-for-type">
					<xsl:with-param name="id" select="@id" />
				</xsl:call-template>
			</xsl:attribute>
			<xsl:value-of select="@name" />
		</a>
		<br />
	</xsl:template>
	<!-- -->
</xsl:stylesheet>
