<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:MSHelp="http://msdn.microsoft.com/mshelp"
	xmlns:NUtil="urn:ndoc-sourceforge-net:documenters.NativeHtmlHelp2.xsltUtilities" exclude-result-prefixes="NUtil">
	<!-- -->
	<xsl:output method="xml" indent="yes" encoding="utf-8" omit-xml-declaration="yes" />
	<!-- -->
	<xsl:include href="common.xslt" />
	<xsl:include href="memberscommon.xslt" />
	<!-- -->
	<xsl:param name='id' />
	<xsl:param name='member-type' />
	<!-- -->
	<xsl:template name="type-members">
		<xsl:param name="type" />
		<xsl:variable name="Members">
			<xsl:call-template name="get-big-member-plural">
				<xsl:with-param name="member" select="$member-type" />
			</xsl:call-template>
		</xsl:variable>
		<xsl:variable name="members">
			<xsl:call-template name="get-small-member-plural">
				<xsl:with-param name="member" select="$member-type" />
			</xsl:call-template>
		</xsl:variable>
		<html dir="LTR">
			<xsl:call-template name="html-head">
				<xsl:with-param name="title" select="concat(@name, ' ', $Members)" />
				<xsl:with-param name="page-type" select="$Members" />
			</xsl:call-template>
			<body topmargin="0" id="bodyID" class="dtBODY">
				<object id="obj_cook" classid="clsid:59CC0C20-679B-11D2-88BD-0800361A1803" style="display:none;"></object>
				<xsl:call-template name="title-row">
					<xsl:with-param name="type-name">
						<xsl:value-of select="@displayName" />&#160;<xsl:value-of select="$Members" />
					</xsl:with-param>
				</xsl:call-template>
				<div id="nstext" valign="bottom">
					<div id="allHistory" class="saveHistory" onsave="saveAll()" onload="loadAll()"></div>
					<p>
						<xsl:text>The </xsl:text>
						<xsl:value-of select="$members" />
						<xsl:text> of the </xsl:text>
						<b>
							<xsl:value-of select="@displayName" />
						</b>
						<xsl:text> </xsl:text>
						<xsl:value-of select="local-name()" />
						<xsl:text> are listed below. For a complete list of </xsl:text>
						<b>
							<xsl:value-of select="@displayName" />
						</b>
						<xsl:text> </xsl:text>
						<xsl:value-of select="local-name()" />
						<xsl:text> members, see the </xsl:text>
						<a>
							<xsl:attribute name="href">
								<xsl:value-of select="NUtil:GetOverviewHRef( string( @id ), 'Members' )" />
							</xsl:attribute>
							<xsl:value-of select="@displayName" />
							<xsl:text> Members</xsl:text>
						</a>
						<xsl:text> topic.</xsl:text>
					</p>
					<!-- static members -->
					<xsl:call-template name="public-static-section">
						<xsl:with-param name="member" select="$member-type" />
					</xsl:call-template>
					<xsl:call-template name="protected-static-section">
						<xsl:with-param name="member" select="$member-type" />
					</xsl:call-template>
					<xsl:call-template name="protected-internal-static-section">
						<xsl:with-param name="member" select="$member-type" />
					</xsl:call-template>
					<xsl:call-template name="internal-static-section">
						<xsl:with-param name="member" select="$member-type" />
					</xsl:call-template>
					<xsl:call-template name="private-static-section">
						<xsl:with-param name="member" select="$member-type" />
					</xsl:call-template>
					<!-- instance members -->
					<xsl:call-template name="public-instance-section">
						<xsl:with-param name="member" select="$member-type" />
					</xsl:call-template>
					<xsl:call-template name="protected-instance-section">
						<xsl:with-param name="member" select="$member-type" />
					</xsl:call-template>
					<xsl:call-template name="protected-internal-instance-section">
						<xsl:with-param name="member" select="$member-type" />
					</xsl:call-template>
					<xsl:call-template name="internal-instance-section">
						<xsl:with-param name="member" select="$member-type" />
					</xsl:call-template>
					<xsl:call-template name="private-instance-section">
						<xsl:with-param name="member" select="$member-type" />
					</xsl:call-template>
					<xsl:call-template name="explicit-interface-implementations">
						<xsl:with-param name="member" select="$member-type" />
					</xsl:call-template>
					<xsl:call-template name="seealso-section">
						<xsl:with-param name="page">
							<xsl:value-of select="$members" />
						</xsl:with-param>
					</xsl:call-template>
					<xsl:call-template name="footer-row">
						<xsl:with-param name="type-name">
							<xsl:value-of select="@name" />&#160;<xsl:value-of select="$Members" />
						</xsl:with-param>
					</xsl:call-template>
				</div>
			</body>
		</html>
	</xsl:template>
	<!-- -->
</xsl:stylesheet>
