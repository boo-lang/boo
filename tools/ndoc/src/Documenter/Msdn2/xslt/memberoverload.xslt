<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
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
	<xsl:template match="method | constructor | property | operator">
		<xsl:variable name="type">
			<xsl:choose>
				<xsl:when test="local-name(..)='interface'">Interface</xsl:when>
				<xsl:otherwise>Class</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="childType">
			<xsl:choose>
				<xsl:when test="local-name()='method'">Method</xsl:when>
				<xsl:when test="local-name()='constructor'">Constructor</xsl:when>
				<xsl:when test="local-name()='operator'">
					<xsl:call-template name="operator-name">
						<xsl:with-param name="name">
							<xsl:value-of select="@name" />
						</xsl:with-param>
					</xsl:call-template>
				</xsl:when>
				<xsl:otherwise>Property</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="memberName" select="@name" />
		<html dir="LTR">
			<xsl:call-template name="html-head">
				<xsl:with-param name="title">
					<xsl:choose>
						<xsl:when test="local-name()='constructor' or local-name()='operator'">
							<xsl:value-of select="../@name" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="@name" />
						</xsl:otherwise>
					</xsl:choose>
					<xsl:text>&#32;</xsl:text>
					<xsl:value-of select="$childType" />
				</xsl:with-param>
			</xsl:call-template>
			<body id="bodyID" class="dtBODY">
				<INPUT class="userDataStyle" id="userDataCache" type="hidden" />
				<xsl:call-template name="title-row">
					<xsl:with-param name="type-name">
						<xsl:value-of select="../@name" />
						<xsl:if test="local-name()='method' or local-name()='property' ">
							<xsl:text>.</xsl:text>
							<xsl:value-of select="@name" />
						</xsl:if>
						<xsl:text>&#160;</xsl:text>
						<xsl:value-of select="$childType" />
					</xsl:with-param>
				</xsl:call-template>
				<div id="nstext">
					<xsl:call-template name="overloads-summary-section" />
					<h4 class="dtH4">Overload List</h4>
					<xsl:for-each select="parent::node()/*[@name=$memberName]">
						<xsl:sort order="ascending" select="@id" />
						<xsl:choose>
							<xsl:when test="@declaringType and starts-with(@declaringType, 'System.')">
								<p>
									<xsl:call-template name="obsolete-inline" />
									<xsl:call-template name="summary-with-no-paragraph" />
										<xsl:if test="./documentation/summary or ./documentation/obsolete">
											<br />
										</xsl:if>
										<i>
											<xsl:text>Inherited from </xsl:text>
										</i>
										<a>
											<xsl:attribute name="href">
												<xsl:call-template name="get-filename-for-type-name">
													<xsl:with-param name="type-name" select="@declaringType" />
												</xsl:call-template>
											</xsl:attribute>
											<xsl:call-template name="strip-namespace">
												<xsl:with-param name="name" select="@declaringType" />
											</xsl:call-template>
										</a>
										<xsl:text>.</xsl:text>
								</p>
								<xsl:apply-templates select="self::node()" mode="syntax" />
							</xsl:when>
							<xsl:when test="@declaringType">
								<p>
									<xsl:call-template name="obsolete-inline" />
									<xsl:call-template name="summary-with-no-paragraph" />
										<xsl:if test="./documentation/summary or ./documentation/obsolete">
											<br />
										</xsl:if>
										<i>
											<xsl:text>Inherited from </xsl:text>
										</i>
										<a>
											<xsl:attribute name="href">
												<xsl:call-template name="get-filename-for-type-name">
													<xsl:with-param name="type-name" select="@declaringType" />
												</xsl:call-template>
											</xsl:attribute>
											<xsl:call-template name="strip-namespace">
												<xsl:with-param name="name" select="@declaringType" />
											</xsl:call-template>
										</a>
										<xsl:text>.</xsl:text>
								</p>
								<xsl:apply-templates select="self::node()" mode="syntax" />
							</xsl:when>
							<xsl:otherwise>
								<p>
									<xsl:call-template name="obsolete-inline" />
									<xsl:call-template name="summary-with-no-paragraph">
										<xsl:with-param name="member" select="." />
									</xsl:call-template>
								</p>
								<xsl:apply-templates select="self::node()" mode="syntax" />
							</xsl:otherwise>
						</xsl:choose>
					</xsl:for-each>
					<xsl:call-template name="overloads-remarks-section" />
					<xsl:call-template name="overloads-example-section" />
					<xsl:call-template name="seealso-section">
						<xsl:with-param name="page">memberoverload</xsl:with-param>
					</xsl:call-template>
					<xsl:call-template name="footer-row">
						<xsl:with-param name="type-name">
							<xsl:value-of select="../@name" />
							<xsl:if test="local-name()='method' or local-name()='property' ">
								<xsl:text>.</xsl:text>
								<xsl:value-of select="@name" />
							</xsl:if>
							<xsl:text>&#160;</xsl:text>
							<xsl:value-of select="$childType" />
						</xsl:with-param>
					</xsl:call-template>
				</div>
			</body>
		</html>
	</xsl:template>
	<!-- -->
	<xsl:template match="constructor | method | property | operator" mode="syntax">
		<xsl:if test="not( @unsafe | parameter[@unsafe] )">
			<blockquote class="dtBlock">
				<xsl:apply-templates select="." mode="inline-syntax">
					<xsl:with-param name="lang" select="'Visual Basic'" />
				</xsl:apply-templates>
			</blockquote>
		</xsl:if>
		<blockquote class="dtBlock">
			<xsl:apply-templates select="." mode="inline-syntax">
				<xsl:with-param name="lang" select="'C#'" />
			</xsl:apply-templates>
		</blockquote>
		<blockquote class="dtBlock">
			<xsl:apply-templates select="." mode="inline-syntax">
				<xsl:with-param name="lang" select="'C++'" />
			</xsl:apply-templates>
		</blockquote>
		<xsl:if test="not( @unsafe | parameter[@unsafe] )">
			<blockquote class="dtBlock">
				<xsl:apply-templates select="." mode="inline-syntax">
					<xsl:with-param name="lang" select="'JScript'" />
				</xsl:apply-templates>
			</blockquote>
		</xsl:if>
		<br />
	</xsl:template>
	<!-- -->
</xsl:stylesheet>
