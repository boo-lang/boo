<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- -->
	<xsl:output method="xml" indent="yes"  encoding="utf-8" omit-xml-declaration="yes"/>
	<!-- -->
	<xsl:include href="common.xslt" />
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
						<xsl:sort order="ascending" select="@id"/>
						<xsl:choose>
							<xsl:when test="@declaringType and starts-with(@declaringType, 'System.')">
								<p>
									<xsl:text>Inherited from </xsl:text>
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
								<blockquote class="dtBlock">
									<a>
										<xsl:attribute name="href">
											<xsl:call-template name="get-filename-for-system-method" />
										</xsl:attribute>
										<xsl:apply-templates select="self::node()" mode="syntax" />
									</a>
								</blockquote>
							</xsl:when>
							<xsl:when test="@declaringType">
								<p>
									<xsl:text>Inherited from </xsl:text>
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
								<blockquote class="dtBlock">
									<a>
										<xsl:attribute name="href">
											<xsl:call-template name="get-filename-for-inherited-method-overloads">
												<xsl:with-param name="declaring-type" select="@declaringType" />
												<xsl:with-param name="method-name" select="@name" />
											</xsl:call-template>
										</xsl:attribute>
										<xsl:apply-templates select="self::node()" mode="syntax" />
									</a>
								</blockquote>
							</xsl:when>
							<xsl:otherwise>
								<p>
									<xsl:call-template name="obsolete-inline"/>
									<xsl:call-template name="summary-with-no-paragraph">
										<xsl:with-param name="member" select="." />
									</xsl:call-template>
								</p>
								<blockquote class="dtBlock">
									<a>
										<xsl:attribute name="href">
											<xsl:call-template name="get-filename-for-cref-overload">
												<xsl:with-param name="cref" select="@id" />
												<xsl:with-param name="overload" select="@overload" />
											</xsl:call-template>
										</xsl:attribute>
										<xsl:apply-templates select="self::node()" mode="syntax" />
									</a>
								</blockquote>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:for-each>
					<xsl:call-template name="overloads-remarks-section" />
					<xsl:call-template name="overloads-example-section" />
					<xsl:call-template name="seealso-section">
						<xsl:with-param name="page">memberoverload</xsl:with-param>
					</xsl:call-template>
					<xsl:if test="local-name()='constructor'">
						<xsl:if test="not($ndoc-omit-object-tags)">
							<object type="application/x-oleobject" classid="clsid:1e2a7bd0-dab9-11d0-b93a-00c04fc99f9e" viewastext="true" style="display: none;">
								<xsl:element name="param">
									<xsl:attribute name="name">Keyword</xsl:attribute>
									<xsl:attribute name="value"><xsl:value-of select='../@name' /> class, constructors</xsl:attribute>
								</xsl:element>
							</object>
						</xsl:if>
					</xsl:if>
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
	<xsl:template match="constructor | method | operator" mode="syntax">
		<xsl:call-template name="member-syntax2" />
	</xsl:template>
	<!-- -->
	<xsl:template match="property" mode="syntax">
		<xsl:call-template name="cs-property-syntax">
			<xsl:with-param name="indent" select="false()" />
			<xsl:with-param name="display-names" select="false()" />
			<xsl:with-param name="link-types" select="false()" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
</xsl:stylesheet>
