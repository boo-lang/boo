<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:MSHelp="http://msdn.microsoft.com/mshelp">
	<!-- -->
	<xsl:output method="xml" indent="yes" encoding="utf-8" omit-xml-declaration="yes" />
	<!-- -->
	<xsl:include href="common.xslt" />
	<xsl:include href="syntax.xslt" />
	<!-- -->
	<xsl:param name='member-id' />
	<!-- -->
	<xsl:template match="/">
		<xsl:apply-templates select="ndoc/assembly/module/namespace/*/*[@id=$member-id][1]" />
	</xsl:template>
	<!-- -->
	<xsl:template match="method | constructor | operator">
		<xsl:variable name="type">
			<xsl:choose>
				<xsl:when test="local-name(..)='interface'">Interface</xsl:when>
				<xsl:when test="local-name(..)='structure'">Structure</xsl:when>
				<xsl:otherwise>Class</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="childType">
			<xsl:choose>
				<xsl:when test="local-name()='method'">Method</xsl:when>
				<xsl:when test="local-name()='operator'"></xsl:when>
				<xsl:when test="@contract='Static'">Static Constructor</xsl:when>
				<xsl:otherwise>Constructor</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="memberName" select="@name" />
		<html dir="LTR">
			<xsl:call-template name="html-head">
				<xsl:with-param name="title">
					<xsl:if test="local-name()='constructor'">
						<xsl:value-of select="../@displayName" />
					</xsl:if>
					<xsl:if test="local-name()='method'">
						<xsl:value-of select="@name" />
					</xsl:if>
					<xsl:if test="local-name()='operator'">
						<xsl:call-template name="operator-name">
							<xsl:with-param name="name" select="@name" />
							<xsl:with-param name="from" select="parameter/@type" />
							<xsl:with-param name="to" select="@returnType" />
						</xsl:call-template>
					</xsl:if>
					<xsl:if test="local-name()!='operator'">
						<xsl:text>&#32;</xsl:text>
						<xsl:value-of select="$childType" />
						<xsl:if test="count(parent::node()/*[@name=$memberName]) &gt; 1">
							<xsl:text>&#32;</xsl:text>
							<xsl:call-template name="get-param-list" />
						</xsl:if>
					</xsl:if>
				</xsl:with-param>
				<xsl:with-param name="page-type" select="$childType" />
			</xsl:call-template>
			<body topmargin="0" id="bodyID" class="dtBODY">
				<object id="obj_cook" classid="clsid:59CC0C20-679B-11D2-88BD-0800361A1803" style="display:none;"></object>
				<xsl:call-template name="title-row">
					<xsl:with-param name="type-name">
						<xsl:if test="local-name()='constructor'">
							<xsl:value-of select="../@displayName" />
						</xsl:if>
						<xsl:if test="local-name()='method'">
							<xsl:value-of select="../@displayName" />
							<xsl:text>.</xsl:text>
							<xsl:value-of select="@name" />
						</xsl:if>
						<xsl:if test="local-name()='operator'">
							<xsl:call-template name="operator-name">
								<xsl:with-param name="name" select="@name" />
								<xsl:with-param name="from" select="parameter/@type" />
								<xsl:with-param name="to" select="@returnType" />
							</xsl:call-template>
						</xsl:if>
						<xsl:if test="local-name()!='operator'">
							<xsl:text>&#160;</xsl:text>
							<xsl:value-of select="$childType" />
							<xsl:if test="count(parent::node()/*[@name=$memberName]) &gt; 1">
								<xsl:text>&#160;</xsl:text>
								<xsl:call-template name="get-param-list" />
							</xsl:if>
						</xsl:if>
					</xsl:with-param>
				</xsl:call-template>
				<div id="nstext" valign="bottom">
					<xsl:call-template name="summary-section" />
					<xsl:call-template name="syntax-section" />
					<xsl:call-template name="parameter-section" />
					<xsl:call-template name="returnvalue-section" />
					<xsl:call-template name="implements-section" />
					<xsl:call-template name="remarks-section" />
					<xsl:apply-templates select="documentation/node()" mode="after-remarks-section" />
					<xsl:call-template name="events-section" />
					<xsl:call-template name="exceptions-section" />
					<xsl:call-template name="example-section" />
					<xsl:call-template name="member-requirements-section" />
					<xsl:call-template name="seealso-section">
						<xsl:with-param name="page">member</xsl:with-param>
					</xsl:call-template>
					<xsl:call-template name="footer-row">
						<xsl:with-param name="type-name">
							<xsl:value-of select="../@name" />
							<xsl:if test="local-name()='method'">
								<xsl:text>.</xsl:text>
								<xsl:value-of select="@name" />
							</xsl:if>
							<xsl:text>&#160;</xsl:text>
							<xsl:value-of select="$childType" />
							<xsl:text>&#160;</xsl:text>
							<xsl:if test="local-name()!='operator'">
								<xsl:if test="count(parent::node()/*[@name=$memberName]) &gt; 1">
									<xsl:call-template name="get-param-list" />
								</xsl:if>
							</xsl:if>
						</xsl:with-param>
					</xsl:call-template>
				</div>
			</body>
		</html>
	</xsl:template>
	<!-- -->
</xsl:stylesheet>
