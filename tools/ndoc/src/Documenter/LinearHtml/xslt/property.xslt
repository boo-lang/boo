<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- -->
	<xsl:output method="html" indent="no" />
	<!-- -->
	<xsl:include href="common.xslt" />
	<!-- -->
	<xsl:param name='property-id' />
	<!-- -->
	<xsl:template match="/">
		<xsl:apply-templates select="ndoc/assembly/module/namespace/*/property[@id=$property-id]" />
	</xsl:template>
	<!-- -->
	<xsl:template match="property">
		<xsl:variable name="type">
			<xsl:choose>
				<xsl:when test="local-name(..)='interface'">Interface</xsl:when>
				<xsl:otherwise>Class</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="propertyName" select="@name" />
		<html dir="LTR">
			<xsl:call-template name="html-head">
				<xsl:with-param name="title" select="concat(../@name, '.', @name, ' Property')" />
			</xsl:call-template>
			<body id="bodyID" class="dtBODY">
				<xsl:call-template name="title-row">
					<xsl:with-param name="type-name">
						<xsl:value-of select="../@name" />.<xsl:value-of select="@name" /> Property
					</xsl:with-param>
				</xsl:call-template>
				<div id="nstext">
					<xsl:call-template name="summary-section" />
					<xsl:if test="$ndoc-vb-syntax">
						<div class="syntax">
							<span class="lang">[Visual&#160;Basic]</span>
							<br />
							<xsl:call-template name="vb-property-syntax" />
						</div>
					</xsl:if>
					<div class="syntax">
						<xsl:if test="$ndoc-vb-syntax">
							<span class="lang">[C#]</span>
							<br />
						</xsl:if>
						<xsl:call-template name="cs-property-syntax" />
					</div>
					<p></p>
					<xsl:call-template name="parameter-section" />
					<xsl:call-template name="value-section" />
					<xsl:call-template name="implements-section" />
					<xsl:call-template name="remarks-section" />
					<xsl:call-template name="events-section" />
					<xsl:call-template name="exceptions-section" />
					<xsl:call-template name="example-section" />
					<xsl:call-template name="requirements-section" />
					<xsl:call-template name="seealso-section">
						<xsl:with-param name="page">property</xsl:with-param>
					</xsl:call-template>
					<xsl:if test="not($ndoc-omit-object-tags)">
						<object type="application/x-oleobject" classid="clsid:1e2a7bd0-dab9-11d0-b93a-00c04fc99f9e" viewastext="true" style="display: none;">
							<xsl:element name="param">
								<xsl:attribute name="name">Keyword</xsl:attribute>
								<xsl:attribute name="value"><xsl:value-of select='@name' /> property</xsl:attribute>
							</xsl:element>
							<xsl:element name="param">
								<xsl:attribute name="name">Keyword</xsl:attribute>
								<xsl:attribute name="value"><xsl:value-of select='@name' /> property, <xsl:value-of select='../@name' /> class</xsl:attribute>
							</xsl:element>
							<xsl:element name="param">
								<xsl:attribute name="name">Keyword</xsl:attribute>
								<xsl:attribute name="value"><xsl:value-of select='../@name' />.<xsl:value-of select='@name' /> property</xsl:attribute>
							</xsl:element>
						</object>
					</xsl:if>
					<xsl:call-template name="footer-row">
						<xsl:with-param name="type-name">
							<xsl:value-of select="../@name" />.<xsl:value-of select="@name" /> Property
						</xsl:with-param>
					</xsl:call-template>
				</div>
			</body>
		</html>
	</xsl:template>
	<!-- -->
</xsl:stylesheet>
