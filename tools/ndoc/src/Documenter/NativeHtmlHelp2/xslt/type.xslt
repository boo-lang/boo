<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:MSHelp="http://msdn.microsoft.com/mshelp"
	xmlns:NUtil="urn:ndoc-sourceforge-net:documenters.NativeHtmlHelp2.xsltUtilities" exclude-result-prefixes="NUtil">
	<!-- -->
	<xsl:output method="xml" indent="yes" encoding="utf-8" omit-xml-declaration="yes" />
	<!-- -->
	<xsl:include href="common.xslt" />
	<xsl:include href="syntax.xslt" />
	<!-- -->
	<xsl:param name='type-id' />
	<!-- -->
	<xsl:template match="/">
		<xsl:apply-templates select="ndoc/assembly/module/namespace/*[@id=$type-id]" />
	</xsl:template>
	<!-- -->
	<xsl:template name="indent">
		<xsl:param name="count" />
		<xsl:if test="$count &gt; 0">
			<xsl:text>&#160;&#160;&#160;</xsl:text>
			<xsl:call-template name="indent">
				<xsl:with-param name="count" select="$count - 1" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="draw-hierarchy">
		<xsl:param name="list" />
		<xsl:param name="level" />
		<xsl:if test="count($list) &gt; 0">
			<xsl:variable name="last" select="count($list)" />
			<xsl:call-template name="indent">
				<xsl:with-param name="count" select="$level" />
			</xsl:call-template>
			<xsl:call-template name="get-link-for-type">
				<xsl:with-param name="type" select="$list[$last]/@id" />
				<xsl:with-param name="link-text">
                    <xsl:value-of select="$list[$last]/@namespace" />.<xsl:value-of select="$list[$last]/@displayName" />
                </xsl:with-param>
			</xsl:call-template>
			<br />
			<xsl:call-template name="draw-hierarchy">
				<xsl:with-param name="list" select="$list[position()!=$last]" />
				<xsl:with-param name="level" select="$level + 1" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="class">
		<xsl:call-template name="type">
			<xsl:with-param name="type">Class</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="interface">
		<xsl:call-template name="type">
			<xsl:with-param name="type">Interface</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="structure">
		<xsl:call-template name="type">
			<xsl:with-param name="type">Structure</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="delegate">
		<xsl:call-template name="type">
			<xsl:with-param name="type">Delegate</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="enumeration">
		<xsl:call-template name="type">
			<xsl:with-param name="type">Enumeration</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template name="type">
		<xsl:param name="type" />
		<html dir="LTR">
			<xsl:call-template name="html-head">
				<xsl:with-param name="title" select="concat(@displayName, ' ', $type)" />
				<xsl:with-param name="page-type" select="'type'" />
			</xsl:call-template>
			<body topmargin="0" id="bodyID" class="dtBODY">
				<object id="obj_cook" classid="clsid:59CC0C20-679B-11D2-88BD-0800361A1803" style="display:none;"></object>
				<xsl:call-template name="title-row">
					<xsl:with-param name="type-name" select="concat(@displayName, ' ', $type)" />
				</xsl:call-template>
				<div id="nstext" valign="bottom">
					<xsl:call-template name="summary-section" />
					<xsl:if test="local-name()!='delegate' and local-name()!='enumeration'">
						<xsl:variable name="members-href" select="NUtil:GetOverviewHRef( string( @id ), 'Members' )" />
						<xsl:if test="constructor|field|property|method|operator|event">
							<p>
								<xsl:text>For a list of all members of this type, see </xsl:text>
								<a>
									<xsl:attribute name="href">
										<xsl:value-of select="$members-href" />
									</xsl:attribute>
									<xsl:value-of select="@displayName" />
									<xsl:text> Members</xsl:text>
								</a>
								<xsl:text>.</xsl:text>
							</p>
						</xsl:if>
					</xsl:if>
					<xsl:if test="local-name()='enumeration' and @flags">
						<p>This enumeration has a 
						<xsl:call-template name="get-a-href">
								<xsl:with-param name="cref" select="'T:System.FlagsAttribute'" />
								<xsl:with-param name="ignore-text" select="true()"/>
						</xsl:call-template>
						attribute that allows a bitwise combination of its member values.</p>
					</xsl:if>
					<xsl:if test="local-name() != 'delegate' and local-name() != 'enumeration'">
						<p>
							<xsl:choose>
								<xsl:when test="self::interface">
									<xsl:if test="derivedBy">
										<b>
                                            <xsl:value-of select="@namespace" />.<xsl:value-of select="@displayName" />
										</b>
										<xsl:choose>
											<xsl:when test="count(derivedBy) &lt; 6">
												<xsl:for-each select="derivedBy">
													<br />
													<xsl:call-template name="indent">
														<xsl:with-param name="count" select="1" />
													</xsl:call-template>
													<xsl:call-template name="get-link-for-type">
														<xsl:with-param name="type" select="@id" />
														<xsl:with-param name="link-text">
                                                            <xsl:value-of select="@namespace" />.<xsl:value-of select="@displayName" />
                                                        </xsl:with-param>
													</xsl:call-template>
												</xsl:for-each>
											</xsl:when>
											<xsl:otherwise>
												<br />
												<xsl:call-template name="indent">
													<xsl:with-param name="count" select="1" />
												</xsl:call-template>
												<a>
													<xsl:attribute name="href">
														<xsl:value-of select="NUtil:GetTypeHierarchyHRef( string( @id ))" />
													</xsl:attribute>
													<xsl:text>Derived interfaces</xsl:text>
												</a>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:if>
								</xsl:when>
								<xsl:otherwise>
									<xsl:call-template name="get-xlink-for-foreign-type">
										<xsl:with-param name="type" select="'T:System.Object'" />
									</xsl:call-template>
									<br />
									<xsl:call-template name="draw-hierarchy">
										<xsl:with-param name="list" select="descendant::base" />
										<xsl:with-param name="level" select="1" />
									</xsl:call-template>
									<xsl:variable name="typeIndent" select="count(descendant::base)" />
									<xsl:call-template name="indent">
										<xsl:with-param name="count" select="$typeIndent+1" />
									</xsl:call-template>
									<b>
                                        <xsl:value-of select="@namespace" />.<xsl:value-of select="@displayName" />
									</b>
									<xsl:if test="derivedBy">
										<xsl:variable name="derivedTypeIndent" select="$typeIndent+2" />
										<xsl:choose>
											<xsl:when test="count(derivedBy) &lt; 6">
												<xsl:for-each select="derivedBy">
													<br />
													<xsl:call-template name="indent">
														<xsl:with-param name="count" select="$derivedTypeIndent" />
													</xsl:call-template>
													<xsl:call-template name="get-link-for-type">
														<xsl:with-param name="type" select="@id" />
														<xsl:with-param name="link-text">
                                                            <xsl:value-of select="@namespace" />.<xsl:value-of select="@displayName" />
                                                        </xsl:with-param>
													</xsl:call-template>
												</xsl:for-each>
											</xsl:when>
											<xsl:otherwise>
												<br />
												<xsl:call-template name="indent">
													<xsl:with-param name="count" select="$derivedTypeIndent" />
												</xsl:call-template>
												<a>
													<xsl:attribute name="href">
														<xsl:value-of select="NUtil:GetTypeHierarchyHRef( string( @id ))" />
													</xsl:attribute>
													<xsl:text>Derived types</xsl:text>
												</a>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:if>
								</xsl:otherwise>
							</xsl:choose>
						</p>
					</xsl:if>
					<xsl:call-template name="syntax-section" />
					<xsl:if test="local-name() = 'interface'">
						<xsl:call-template name="interface-implementing-types-section" />
					</xsl:if>
					<xsl:if test="local-name() = 'delegate'">
						<xsl:call-template name="parameter-section" />
						<xsl:call-template name="returnvalue-section" />
					</xsl:if>
					<!-- only classes and structures get a thread safety section -->
					<xsl:if test="local-name() = 'class' or local-name() = 'structure'">
						<xsl:call-template name="thread-safety-section" />
					</xsl:if>
					<xsl:call-template name="remarks-section" />
					<xsl:apply-templates select="documentation/node()" mode="after-remarks-section" />
					<xsl:call-template name="example-section" />
					<xsl:if test="local-name() = 'enumeration'">
						<xsl:call-template name="enumeration-members-section" />
					</xsl:if>
					<xsl:call-template name="type-requirements-section" />
					<xsl:variable name="page">
						<xsl:choose>
							<xsl:when test="local-name() = 'enumeration'">enumeration</xsl:when>
							<xsl:when test="local-name() = 'delegate'">delegate</xsl:when>
							<xsl:otherwise>type</xsl:otherwise>
						</xsl:choose>
					</xsl:variable>
					<xsl:call-template name="seealso-section">
						<xsl:with-param name="page" select="$page" />
					</xsl:call-template>
					<xsl:call-template name="footer-row">
						<xsl:with-param name="type-name" select="concat(@name, ' ', $type)" />
					</xsl:call-template>
				</div>
			</body>
		</html>
	</xsl:template>
	<!-- -->
	<xsl:template name="interface-implementing-types-section">
		<xsl:if test="implementedBy">
			<h4 class="dtH4">Types that implement <xsl:value-of select="@name" /></h4>
			<div class="tablediv">
				<table class="dtTABLE" cellspacing="0">
					<tr valign="top">
						<th width="50%">Type</th>
						<th width="50%">Description</th>
					</tr>
					<xsl:for-each select="implementedBy">
						<xsl:variable name="typeID" select="@id" />
						<xsl:apply-templates select="ancestor::ndoc/assembly/module/namespace/*[@id=$typeID]" mode="implementingType" />
					</xsl:for-each>
				</table>
			</div>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="structure | class" mode="implementingType">
		<tr valign="top">
			<td>
				<a>
					<xsl:attribute name="href">
						<xsl:value-of select="NUtil:GetLocalCRef( string( @id ) )" />
					</xsl:attribute>
					<xsl:value-of select="@displayName" />
				</a>
			</td>
			<td>
				<xsl:call-template name="obsolete-inline" />
				<xsl:apply-templates select="(documentation/summary)[1]/node()" mode="slashdoc" />
				<xsl:if test="not((documentation/summary)[1]/node())">&#160;</xsl:if>
			</td>
		</tr>
	</xsl:template>
</xsl:stylesheet>
