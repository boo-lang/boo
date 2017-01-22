<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:MSHelp="http://msdn.microsoft.com/mshelp"
	xmlns:NUtil="urn:ndoc-sourceforge-net:documenters.NativeHtmlHelp2.xsltUtilities" exclude-result-prefixes="NUtil">
	<!-- -->
	<xsl:output method="xml" indent="yes" encoding="utf-8" omit-xml-declaration="yes" />
	<!-- -->
	<xsl:include href="common.xslt" />
	<!-- -->
	<xsl:param name='namespace' />
	<!-- -->
	<xsl:template match="/">
		<xsl:variable name="ns" select="ndoc/namespaceHierarchies/namespaceHierarchy[@name=$namespace][1]" />
		<html dir="LTR">
			<xsl:call-template name="html-head">
				<xsl:with-param name="title" select="concat($ns/@name, 'Hierarchy')" />
				<xsl:with-param name="page-type" select="'hierarchy'" />
			</xsl:call-template>
			<body topmargin="0" id="bodyID" class="dtBODY">
				<object id="obj_cook" classid="clsid:59CC0C20-679B-11D2-88BD-0800361A1803" style="display:none;"></object>
				<xsl:call-template name="title-row">
					<xsl:with-param name="type-name" select="concat($ns/@name, ' Hierarchy')" />
				</xsl:call-template>
				<div id="nstext" valign="bottom">
					<xsl:apply-templates select="$ns" />
					<h4 class="dtH4">See Also</h4>
					<p>
						<a>
							<xsl:attribute name="href">
								<xsl:value-of select="NUtil:GetNamespaceHRef( string( $namespace ) )" />
							</xsl:attribute>
							<xsl:value-of select="$namespace" /> Namespace
						</a>
					</p>
					<xsl:call-template name="footer-row">
						<xsl:with-param name="type-name" select="concat($ns/@name, ' Hierarchy')" />
					</xsl:call-template>
				</div>
			</body>
		</html>
	</xsl:template>
	<!-- -->
	<xsl:template match="namespaceHierarchy">
		<xsl:for-each select="hierarchyType">
			<div>
				<xsl:call-template name="get-link-for-type">
					<xsl:with-param name="type" select="@id" />
					<xsl:with-param name="link-text" select="substring-after(@id, ':' )" />
				</xsl:call-template>
				<xsl:apply-templates mode="hierarchy" />
			</div>
		</xsl:for-each>
	</xsl:template>
	<!-- -->
	<xsl:template match="hierarchyType" mode="hierarchy">
		<div class="hier">
			<xsl:choose>
				<xsl:when test="hierarchyInterfaces">
					<table class="hier">
						<tr>
							<td width="1%">
								<nobr>
									<xsl:call-template name="get-link-for-type">
										<xsl:with-param name="type" select="@id" />
										<xsl:with-param name="link-text">
                                            <xsl:value-of select="@namespace" />.<xsl:value-of select="@displayName" />
                                        </xsl:with-param>
									</xsl:call-template>
									<xsl:text>&#160;----&#160;</xsl:text>
								</nobr>
							</td>
							<td width="99%">
								<xsl:apply-templates select="./hierarchyInterfaces/hierarchyInterface" mode="baseInterfaces" />
							</td>
						</tr>
					</table>
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="get-link-for-type">
						<xsl:with-param name="type" select="@id" />
						<xsl:with-param name="link-text">
                            <xsl:value-of select="@namespace" />.<xsl:value-of select="@displayName" />
                        </xsl:with-param>
					</xsl:call-template>
				</xsl:otherwise>
			</xsl:choose>
			<xsl:apply-templates select="hierarchyType" mode="hierarchy" />
		</div>
	</xsl:template>
	<!-- -->
	<xsl:template match="hierarchyInterface" mode="baseInterfaces">
		<xsl:call-template name="get-link-for-type">
			<xsl:with-param name="type" select="@id" />
			<xsl:with-param name="link-text">
                <xsl:value-of select="@namespace" />.<xsl:value-of select="@displayName" />
        </xsl:with-param>
		</xsl:call-template>
		<xsl:if test="position() != last()">
			<xsl:text>, </xsl:text>
		</xsl:if>
	</xsl:template>
	<!-- -->
</xsl:stylesheet>
