<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
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
								<xsl:call-template name="get-filename-for-namespace">
									<xsl:with-param name="name" select="$namespace" />
								</xsl:call-template>
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
				<xsl:call-template name="get-type-link">
					<xsl:with-param name="id" select="@id" />
				</xsl:call-template>
				<xsl:apply-templates mode="hierarchy" />
			</div>
		</xsl:for-each>
	</xsl:template>
	<!-- -->
	<xsl:template match="hierarchyType" mode="hierarchy">
		<div class="Hierarchy">
			<xsl:call-template name="get-type-link">
				<xsl:with-param name="id" select="@id" />
			</xsl:call-template>
			<xsl:if test="hierarchyInterfaces">
				<xsl:text>&#160;---- </xsl:text>
				<xsl:apply-templates select="./hierarchyInterfaces/hierarchyInterface" mode="baseInterfaces" />
			</xsl:if>
			<xsl:apply-templates select="hierarchyType" mode="hierarchy" />
		</div>
	</xsl:template>
	<!-- -->
	<xsl:template match="hierarchyInterface" mode="baseInterfaces">
		<xsl:call-template name="get-type-link">
			<xsl:with-param name="id" select="@id" />
		</xsl:call-template>
		<xsl:if test="position() != last()">
			<xsl:text>, </xsl:text>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="get-type-link">
		<xsl:param name="id" />
			<xsl:choose>
				<xsl:when test="starts-with($id, 'T:System.') or starts-with($id, 'T:Microsoft.')">
               <!--
					<xsl:attribute name="href">
						<xsl:call-template name="get-filename-for-system-type">
							<xsl:with-param name="type-name" select="substring-after($id, ':')" />
						</xsl:call-template>
					</xsl:attribute>
            -->
               <b><xsl:value-of select="substring-after($id, ':')" />
               </b>
            </xsl:when>
				<xsl:otherwise>
               <a>
                  <xsl:attribute name="href">
						<xsl:call-template name="get-filename-for-type">
							<xsl:with-param name="id" select="$id" />
						</xsl:call-template>
					</xsl:attribute>
                  <xsl:value-of select="substring-after(@id, ':')" />
               </a>
            </xsl:otherwise>
			</xsl:choose>
	</xsl:template>
	<!-- -->
</xsl:stylesheet>
