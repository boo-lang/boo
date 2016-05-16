<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- -->
	<xsl:output method="xml" indent="yes"  encoding="utf-8" omit-xml-declaration="yes"/>
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
			</xsl:call-template>
			<body id="bodyID" class="dtBODY">
				<xsl:call-template name="title-row">
					<xsl:with-param name="type-name">
						<xsl:value-of select="@name" />&#160;<xsl:value-of select="$Members" />
					</xsl:with-param>
				</xsl:call-template>
				<div id="nstext">
					<p>
						<xsl:text>The </xsl:text>
						<xsl:value-of select="$members" />
						<xsl:text> of the </xsl:text>
						<b>
							<xsl:value-of select="@name" />
						</b>
						<xsl:text> </xsl:text>
						<xsl:value-of select="local-name()" />
						<xsl:text> are listed below. For a complete list of </xsl:text>
						<b>
							<xsl:value-of select="@name" />
						</b>
						<xsl:text> </xsl:text>
						<xsl:value-of select="local-name()" />
						<xsl:text> members, see the </xsl:text>
						<a>
							<xsl:attribute name="href">
								<xsl:call-template name="get-filename-for-type-members">
									<xsl:with-param name="id" select="@id" />
								</xsl:call-template>
							</xsl:attribute>
							<xsl:value-of select="@name" />
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
					
					<xsl:if test="not($ndoc-omit-object-tags)">
						<object type="application/x-oleobject" classid="clsid:1e2a7bd0-dab9-11d0-b93a-00c04fc99f9e" viewastext="true" style="display: none;">
							<xsl:element name="param">
								<xsl:attribute name="name">Keyword</xsl:attribute>
								<xsl:attribute name="value"><xsl:value-of select="concat(@name, ' ', local-name(), ', ', $members)" /></xsl:attribute>
							</xsl:element>
						</object>
					</xsl:if>
					
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
