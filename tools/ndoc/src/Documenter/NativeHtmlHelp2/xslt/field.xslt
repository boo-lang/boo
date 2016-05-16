<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:MSHelp="http://msdn.microsoft.com/mshelp">
	<!-- -->
	<xsl:output method="xml" indent="yes" encoding="utf-8" omit-xml-declaration="yes" />
	<!-- -->
	<xsl:include href="common.xslt" />
	<xsl:include href="syntax.xslt" />
	<!-- -->
	<xsl:param name='field-id' />
	<!-- -->
	<xsl:template match="/">
		<xsl:apply-templates select="ndoc/assembly/module/namespace/*/field[@id=$field-id]" />
	</xsl:template>
	<!-- -->
	<xsl:template match="field">
		<html dir="LTR">
			<xsl:call-template name="html-head">
				<xsl:with-param name="title" select="concat( @name, ' Field' )" />
				<xsl:with-param name="page-type" select="'Field'" />
			</xsl:call-template>
			<body topmargin="0" id="bodyID" class="dtBODY">
				<object id="obj_cook" classid="clsid:59CC0C20-679B-11D2-88BD-0800361A1803" style="display:none;"></object>
				<xsl:call-template name="title-row">
					<xsl:with-param name="type-name">
						<xsl:value-of select="../@displayName" />.<xsl:value-of select="@name" /> Field
					</xsl:with-param>
				</xsl:call-template>
				<div id="nstext" valign="bottom">
					<xsl:call-template name="summary-section" />
					<xsl:call-template name="syntax-section" />
					<xsl:call-template name="remarks-section" />
					<xsl:apply-templates select="documentation/node()" mode="after-remarks-section" />
					<xsl:call-template name="example-section" />
					<xsl:call-template name="member-requirements-section" />
					<xsl:call-template name="seealso-section">
						<xsl:with-param name="page">field</xsl:with-param>
					</xsl:call-template>
					<xsl:call-template name="footer-row">
						<xsl:with-param name="type-name">
							<xsl:value-of select="../@displayName" />.<xsl:value-of select="@name" /> Field
						</xsl:with-param>
					</xsl:call-template>
				</div>
			</body>
		</html>
	</xsl:template>
	<!-- -->
</xsl:stylesheet>
