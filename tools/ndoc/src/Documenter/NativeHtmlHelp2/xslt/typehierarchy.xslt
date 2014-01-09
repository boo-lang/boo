<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:MSHelp="http://msdn.microsoft.com/mshelp"
	xmlns:NUtil="urn:ndoc-sourceforge-net:documenters.NativeHtmlHelp2.xsltUtilities" 
	exclude-result-prefixes="NUtil">
	<!-- -->
	<xsl:output method="xml" indent="yes" encoding="utf-8" omit-xml-declaration="yes" />
	<!-- -->
	<xsl:include href="common.xslt" />
	<!-- -->
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
				<xsl:with-param name="link-text" select="substring-after( $list[$last]/@id, ':' )" />
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
	<xsl:template name="type">
		<xsl:param name="type" />
		<html dir="LTR">
			<xsl:call-template name="html-head">
				<xsl:with-param name="title" select="concat(@name, ' Hierarchy')" />
				<xsl:with-param name="page-type" select="'TypeHierarchy'" />
			</xsl:call-template>
			<body topmargin="0" id="bodyID" class="dtBODY">
				<object id="obj_cook" classid="clsid:59CC0C20-679B-11D2-88BD-0800361A1803" style="display:none;"></object>
				<xsl:call-template name="title-row">
					<xsl:with-param name="type-name" select="concat(@name, ' Hierarchy')" />
				</xsl:call-template>
				<div id="nstext" valign="bottom">
					<p>
						<xsl:choose>
							<xsl:when test="self::interface">
								<xsl:if test="derivedBy">
									<b>
										<xsl:value-of select="substring-after( @id, ':' )" />
									</b>
									<xsl:for-each select="derivedBy">
										<br />
										<xsl:call-template name="indent">
											<xsl:with-param name="count" select="1" />
										</xsl:call-template>
										<xsl:call-template name="get-link-for-type">
											<xsl:with-param name="type" select="@id" />
											<xsl:with-param name="link-text" select="substring-after(@id, ':' )" />
										</xsl:call-template>
									</xsl:for-each>
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
									<xsl:value-of select="substring-after( @id, ':' )" />
								</b>
								<xsl:if test="derivedBy">
									<xsl:variable name="derivedTypeIndent" select="$typeIndent+2" />
									<xsl:for-each select="derivedBy">
										<br />
										<xsl:call-template name="indent">
											<xsl:with-param name="count" select="$derivedTypeIndent" />
										</xsl:call-template>
										<xsl:call-template name="get-link-for-type">
											<xsl:with-param name="type" select="@id" />
											<xsl:with-param name="link-text" select="substring-after(@id, ':' )" />
										</xsl:call-template>
									</xsl:for-each>
								</xsl:if>
							</xsl:otherwise>
						</xsl:choose>
					</p>
					<xsl:call-template name="seealso-section">
						<xsl:with-param name="page" select="'typehierarchy'" />
					</xsl:call-template>
					<xsl:call-template name="footer-row">
						<xsl:with-param name="type-name" select="concat(@name, ' Hierarchy')" />
					</xsl:call-template>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
