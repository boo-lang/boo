<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- -->
	<xsl:output method="xml" indent="yes"  encoding="utf-8" omit-xml-declaration="yes" standalone="no"/>
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
				<xsl:with-param name="title" select="concat(@name, ' Members')" />
			</xsl:call-template>
			<body id="bodyID" class="dtBODY">
				<xsl:call-template name="title-row">
					<xsl:with-param name="type-name">
						<xsl:value-of select="@name" /> Members
					</xsl:with-param>
				</xsl:call-template>
				<div id="nstext">
					<p>
						<a>
							<xsl:attribute name="href">
								<xsl:call-template name="get-filename-for-type">
									<xsl:with-param name="id" select="@id" />
								</xsl:call-template>
							</xsl:attribute>
							<xsl:value-of select="@name" />
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
					
					<xsl:if test="not($ndoc-omit-object-tags)">
						<object type="application/x-oleobject" classid="clsid:1e2a7bd0-dab9-11d0-b93a-00c04fc99f9e" viewastext="true" style="display: none;">
							<xsl:element name="param">
								<xsl:attribute name="name">Keyword</xsl:attribute>
								<xsl:attribute name="value"><xsl:value-of select="concat(@name, ' ', local-name())" /></xsl:attribute>
							</xsl:element>
							<xsl:element name="param">
								<xsl:attribute name="name">Keyword</xsl:attribute>
								<xsl:attribute name="value"><xsl:value-of select="concat(substring-after(@id, ':'), ' ', local-name())" /></xsl:attribute>
							</xsl:element>
							<xsl:element name="param">
								<xsl:attribute name="name">Keyword</xsl:attribute>
								<xsl:attribute name="value"><xsl:value-of select="concat(@name, ' ', local-name(), ', all members')" /></xsl:attribute>
							</xsl:element>
						</object>
					</xsl:if>
										
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
	<xsl:template match="constructor">
		<xsl:variable name="access" select="@access" />
		<xsl:if test="not(preceding-sibling::constructor[@access=$access])">
			<xsl:variable name="contract" select="@contract" />
			<tr VALIGN="top">
				<xsl:choose>
					<xsl:when test="(count(../constructor[@contract!='Static']) &gt; 1) and ($contract!='Static')">
						<td width="50%">
						  <xsl:choose>
							<xsl:when test="@access='Public'">
								<img src="pubmethod.gif" />
							</xsl:when>
							<xsl:when test="@access='Family'">
								<img src="protmethod.gif" />
							</xsl:when>
							<xsl:when test="@access='Private'">
								<img src="privmethod.gif" />
							</xsl:when>
							<xsl:when test="@access='Assembly' or @access='FamilyOrAssembly'">
								<img src="intmethod.gif" />
							</xsl:when>
						  </xsl:choose>
							<a>
								<xsl:attribute name="href">
									<xsl:call-template name="get-filename-for-current-constructor-overloads" />
								</xsl:attribute>
								<xsl:value-of select="../@name" />
							</a>
						</td>
						<td width="50%">
							<xsl:text>Overloaded. </xsl:text>
							<xsl:choose>
								<xsl:when test="../constructor/documentation/overloads">
									<xsl:call-template name="overloads-summary-with-no-paragraph">
										<xsl:with-param name="overloads" select="../constructor" />
									</xsl:call-template>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text>Initializes a new instance of the </xsl:text>
									<xsl:value-of select="../@name" />
									<xsl:text> class.</xsl:text>
								</xsl:otherwise>
							</xsl:choose>
						</td>
					</xsl:when>
					<xsl:otherwise>
						<td width="50%">
						  <xsl:choose>
							<xsl:when test="@access='Public'">
								<img src="pubmethod.gif" />
							</xsl:when>
							<xsl:when test="@access='Family'">
								<img src="protmethod.gif" />
							</xsl:when>
							<xsl:when test="@access='Private'">
								<img src="privmethod.gif" />
							</xsl:when>
							<xsl:when test="@access='Assembly' or @access='FamilyOrAssembly'">
								<img src="intmethod.gif" />
							</xsl:when>
						  </xsl:choose>
						  <xsl:if test="$contract='Static'">
							 <img src="static.gif" />
						  </xsl:if>
							<a>
								<xsl:attribute name="href">
									<xsl:call-template name="get-filename-for-current-constructor" />
								</xsl:attribute>
								<xsl:value-of select="../@name" />
								<xsl:text> Constructor</xsl:text>
							</a>
						</td>
						<td width="50%">
							<xsl:apply-templates select="documentation/summary/node()" mode="slashdoc" />
						</td>
					</xsl:otherwise>
				</xsl:choose>
			</tr>
		</xsl:if>
	</xsl:template>
	<!-- -->
</xsl:stylesheet>
