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
	<!-- -->
	<xsl:template name="type-members">
		<xsl:param name="type" />
		<html dir="LTR">
			<xsl:call-template name="html-head">
				<xsl:with-param name="title" select="concat(@displayName, ' Members')" />
				<xsl:with-param name="page-type" select="'Members'" />
			</xsl:call-template>
			<body topmargin="0" id="bodyID" class="dtBODY">
				<xsl:call-template name="title-row">
					<xsl:with-param name="type-name">
						<xsl:value-of select="@displayName" /> Members
					</xsl:with-param>
				</xsl:call-template>
				<div id="nstext" valign="bottom">
					<div id="allHistory" class="saveHistory" onsave="saveAll()" onload="loadAll()"></div>
					<p>
						<a>
							<xsl:attribute name="href">
								<xsl:value-of select="NUtil:GetLocalCRef( string( @id ) )" />
							</xsl:attribute>
							<xsl:value-of select="@displayName" />
							<xsl:text> overview</xsl:text>
						</a>
					</p>
					<!-- public static members -->
					<xsl:call-template name="public-static-section">
						<xsl:with-param name="member" select="'field'" />
					</xsl:call-template>
					<xsl:call-template name="public-static-section">
						<xsl:with-param name="member" select="'property'" />
					</xsl:call-template>
					<xsl:call-template name="public-static-section">
						<xsl:with-param name="member" select="'method'" />
					</xsl:call-template>
					<xsl:call-template name="public-static-section">
						<xsl:with-param name="member" select="'operator'" />
					</xsl:call-template>
					<xsl:call-template name="public-static-section">
						<xsl:with-param name="member" select="'event'" />
					</xsl:call-template>
					<!-- protected static members -->
					<xsl:call-template name="protected-static-section">
						<xsl:with-param name="member" select="'field'" />
					</xsl:call-template>
					<xsl:call-template name="protected-static-section">
						<xsl:with-param name="member" select="'property'" />
					</xsl:call-template>
					<xsl:call-template name="protected-static-section">
						<xsl:with-param name="member" select="'method'" />
					</xsl:call-template>
					<xsl:call-template name="protected-static-section">
						<xsl:with-param name="member" select="'event'" />
					</xsl:call-template>
					<!-- protected internal static members -->
					<xsl:call-template name="protected-internal-static-section">
						<xsl:with-param name="member" select="'field'" />
					</xsl:call-template>
					<xsl:call-template name="protected-internal-static-section">
						<xsl:with-param name="member" select="'property'" />
					</xsl:call-template>
					<xsl:call-template name="protected-internal-static-section">
						<xsl:with-param name="member" select="'method'" />
					</xsl:call-template>
					<xsl:call-template name="protected-internal-static-section">
						<xsl:with-param name="member" select="'event'" />
					</xsl:call-template>
					<!-- internal static members -->
					<xsl:call-template name="internal-static-section">
						<xsl:with-param name="member" select="'field'" />
					</xsl:call-template>
					<xsl:call-template name="internal-static-section">
						<xsl:with-param name="member" select="'property'" />
					</xsl:call-template>
					<xsl:call-template name="internal-static-section">
						<xsl:with-param name="member" select="'method'" />
					</xsl:call-template>
					<xsl:call-template name="internal-static-section">
						<xsl:with-param name="member" select="'event'" />
					</xsl:call-template>
					<!-- private static members -->
					<xsl:if test="constructor[@access='Private' and @contract='Static']">
						<h4 class="dtH4">Private Static Constructor</h4>
						<div class="tablediv">
							<table class="dtTABLE" cellspacing="0">
								<xsl:apply-templates select="constructor[@access='Private' and @contract='Static']" />
							</table>
						</div>
					</xsl:if>
					<xsl:call-template name="private-static-section">
						<xsl:with-param name="member" select="'field'" />
					</xsl:call-template>
					<xsl:call-template name="private-static-section">
						<xsl:with-param name="member" select="'property'" />
					</xsl:call-template>
					<xsl:call-template name="private-static-section">
						<xsl:with-param name="member" select="'method'" />
					</xsl:call-template>
					<xsl:call-template name="private-static-section">
						<xsl:with-param name="member" select="'event'" />
					</xsl:call-template>
					<!-- public instance members -->
					<xsl:if test="constructor[@access='Public' and @contract='Normal']">
						<h4 class="dtH4">Public Instance Constructors</h4>
						<div class="tablediv">
							<table class="dtTABLE" cellspacing="0">
								<xsl:apply-templates select="constructor[@access='Public' and @contract='Normal']" />
							</table>
						</div>
					</xsl:if>
					<xsl:call-template name="public-instance-section">
						<xsl:with-param name="member" select="'field'" />
					</xsl:call-template>
					<xsl:call-template name="public-instance-section">
						<xsl:with-param name="member" select="'property'" />
					</xsl:call-template>
					<xsl:call-template name="public-instance-section">
						<xsl:with-param name="member" select="'method'" />
					</xsl:call-template>
					<xsl:call-template name="public-instance-section">
						<xsl:with-param name="member" select="'event'" />
					</xsl:call-template>
					<!-- protected instance members -->
					<xsl:if test="constructor[@access='Family' and @contract='Normal']">
						<h4 class="dtH4">Protected Instance Constructors</h4>
						<div class="tablediv">
							<table class="dtTABLE" cellspacing="0">
								<xsl:apply-templates select="constructor[@access='Family' and @contract='Normal']" />
							</table>
						</div>
					</xsl:if>
					<xsl:call-template name="protected-instance-section">
						<xsl:with-param name="member" select="'field'" />
					</xsl:call-template>
					<xsl:call-template name="protected-instance-section">
						<xsl:with-param name="member" select="'property'" />
					</xsl:call-template>
					<xsl:call-template name="protected-instance-section">
						<xsl:with-param name="member" select="'method'" />
					</xsl:call-template>
					<xsl:call-template name="protected-instance-section">
						<xsl:with-param name="member" select="'event'" />
					</xsl:call-template>
					<!-- protected internal instance members -->
					<xsl:if test="constructor[@access='FamilyOrAssembly' and @contract='Normal']">
						<h4 class="dtH4">Protected Internal Instance Constructors</h4>
						<div class="tablediv">
							<table class="dtTABLE" cellspacing="0">
								<xsl:apply-templates select="constructor[@access='FamilyOrAssembly' and @contract='Normal']" />
							</table>
						</div>
					</xsl:if>
					<xsl:call-template name="protected-internal-instance-section">
						<xsl:with-param name="member" select="'field'" />
					</xsl:call-template>
					<xsl:call-template name="protected-internal-instance-section">
						<xsl:with-param name="member" select="'property'" />
					</xsl:call-template>
					<xsl:call-template name="protected-internal-instance-section">
						<xsl:with-param name="member" select="'method'" />
					</xsl:call-template>
					<xsl:call-template name="protected-internal-instance-section">
						<xsl:with-param name="member" select="'event'" />
					</xsl:call-template>
					<!-- internal instance members -->
					<xsl:if test="constructor[@access='Assembly' and @contract='Normal']">
						<h4 class="dtH4">Internal Instance Constructors</h4>
						<div class="tablediv">
							<table class="dtTABLE" cellspacing="0">
								<xsl:apply-templates select="constructor[@access='Assembly' and @contract='Normal']" />
							</table>
						</div>
					</xsl:if>
					<xsl:call-template name="internal-instance-section">
						<xsl:with-param name="member" select="'field'" />
					</xsl:call-template>
					<xsl:call-template name="internal-instance-section">
						<xsl:with-param name="member" select="'property'" />
					</xsl:call-template>
					<xsl:call-template name="internal-instance-section">
						<xsl:with-param name="member" select="'method'" />
					</xsl:call-template>
					<xsl:call-template name="internal-instance-section">
						<xsl:with-param name="member" select="'event'" />
					</xsl:call-template>
					<!-- private instance members -->
					<xsl:if test="constructor[@access='Private' and @contract='Normal']">
						<h4 class="dtH4">Private Instance Constructors</h4>
						<div class="tablediv">
							<table class="dtTABLE" cellspacing="0">
								<xsl:apply-templates select="constructor[@access='Private' and @contract='Normal']" />
							</table>
						</div>
					</xsl:if>
					<xsl:call-template name="private-instance-section">
						<xsl:with-param name="member" select="'field'" />
					</xsl:call-template>
					<xsl:call-template name="private-instance-section">
						<xsl:with-param name="member" select="'property'" />
					</xsl:call-template>
					<xsl:call-template name="private-instance-section">
						<xsl:with-param name="member" select="'method'" />
					</xsl:call-template>
					<xsl:call-template name="private-instance-section">
						<xsl:with-param name="member" select="'event'" />
					</xsl:call-template>
					<xsl:call-template name="explicit-interface-implementations">
						<xsl:with-param name="member" select="'method'" />
					</xsl:call-template>
					<xsl:call-template name="seealso-section">
						<xsl:with-param name="page">members</xsl:with-param>
					</xsl:call-template>
					<xsl:call-template name="footer-row">
						<xsl:with-param name="type-name">
							<xsl:value-of select="@name" /> Members
						</xsl:with-param>
					</xsl:call-template>
				</div>
			</body>
		</html>
	</xsl:template>
	<!-- -->
</xsl:stylesheet>
