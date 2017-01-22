<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:NUtil="urn:ndoc-sourceforge-net:documenters.NativeHtmlHelp2.xsltUtilities"
	xmlns:MSHelp="http://msdn.microsoft.com/mshelp" 
	exclude-result-prefixes="NUtil">
	<!-- -->
	<xsl:include href="syntax-map.xslt" />
	<!-- -->
	<xsl:param name="ndoc-document-attributes" />
	<!-- -->
	<xsl:template name="syntax-section">
		<xsl:apply-templates select="." mode="pre-syntax" />
		<xsl:text>&#10;</xsl:text>
		<PRE class="syntax">
			<xsl:if test="not( @unsafe | parameter[@unsafe] )">
				<xsl:apply-templates select="." mode="syntax">
					<xsl:with-param name="lang" select="'Visual Basic'" />
				</xsl:apply-templates>
			</xsl:if>
			<xsl:apply-templates select="." mode="syntax">
				<xsl:with-param name="lang" select="'C#'" />
			</xsl:apply-templates>
			<xsl:apply-templates select="." mode="syntax">
				<xsl:with-param name="lang" select="'C++'" />
			</xsl:apply-templates>
			<xsl:if test="not( @unsafe | parameter[@unsafe] )">
				<xsl:apply-templates select="." mode="syntax">
					<xsl:with-param name="lang" select="'JScript'" />
				</xsl:apply-templates>
			</xsl:if>
		</PRE>
		<xsl:apply-templates select="." mode="post-syntax" />
	</xsl:template>
	<!-- -->
	<xsl:template name="syntax-header">
		<xsl:param name="lang" />
		<SPAN class="lang">[<xsl:value-of select="$lang" />]<xsl:text>&#10;</xsl:text></SPAN>
		<!-- JScript declares attributes after the visibility -->
		<xsl:if test="$lang != 'JScript'">
			<xsl:call-template name="attributes">
				<xsl:with-param name="lang" select="$lang" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="@* | node() | text()" mode="syntax" />
	<!-- -->
	<xsl:template match="enumeration" mode="syntax">
		<xsl:param name="lang" />
		<xsl:call-template name="syntax-header">
			<xsl:with-param name="lang" select="$lang" />
		</xsl:call-template>
		<b>
			<xsl:apply-templates select="." mode="access">
				<xsl:with-param name="lang" select="$lang" />
			</xsl:apply-templates>
			<xsl:apply-templates select="." mode="gc-type">
				<xsl:with-param name="lang" select="$lang" />
			</xsl:apply-templates>
			<xsl:apply-templates select="." mode="keyword">
				<xsl:with-param name="lang" select="$lang" />
			</xsl:apply-templates>
			<xsl:value-of select="@name" />
			<xsl:if test="@baseType!=''">
				<xsl:apply-templates select="." mode="enum-type">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:apply-templates>
				<xsl:variable name="link-type">
					<xsl:call-template name="get-datatype">
						<xsl:with-param name="datatype" select="@baseType" />
						<xsl:with-param name="lang" select="$lang" />
					</xsl:call-template>
				</xsl:variable>
				<xsl:call-template name="get-link-for-type-name">
					<xsl:with-param name="type-name" select="@baseType" />
					<xsl:with-param name="link-text" select="$link-type" />
				</xsl:call-template>
			</xsl:if>
			<xsl:text>&#10;</xsl:text>
		</b>
	</xsl:template>
	<!-- -->
	<xsl:template match="structure | interface | class" mode="syntax">
		<xsl:param name="lang" />
		<xsl:if test="not(local-name()='structure' and $lang='JScript')">
			<xsl:call-template name="syntax-header">
				<xsl:with-param name="lang" select="$lang" />
			</xsl:call-template>
			<b>
				<xsl:choose>
					<xsl:when test="$lang='Visual Basic'">
						<xsl:if test="@hiding">Shadows&#160;</xsl:if>
						<xsl:apply-templates select="." mode="abstract">
							<xsl:with-param name="lang" select="$lang" />
						</xsl:apply-templates>
						<xsl:apply-templates select="." mode="sealed">
							<xsl:with-param name="lang" select="$lang" />
						</xsl:apply-templates>
						<xsl:apply-templates select="." mode="access">
							<xsl:with-param name="lang" select="$lang" />
						</xsl:apply-templates>
					</xsl:when>
					<xsl:otherwise>
						<xsl:if test="$lang = 'C#' and @hiding">new&#160;</xsl:if>
						<xsl:apply-templates select="." mode="access">
							<xsl:with-param name="lang" select="$lang" />
						</xsl:apply-templates>
						<xsl:if test="$lang = 'JScript'">
							<xsl:call-template name="attributes">
								<xsl:with-param name="lang" select="$lang" />
							</xsl:call-template>
						</xsl:if>
						<xsl:apply-templates select="." mode="gc-type">
							<xsl:with-param name="lang" select="$lang" />
						</xsl:apply-templates>
						<xsl:apply-templates select="." mode="abstract">
							<xsl:with-param name="lang" select="$lang" />
						</xsl:apply-templates>
						<xsl:apply-templates select="." mode="sealed">
							<xsl:with-param name="lang" select="$lang" />
						</xsl:apply-templates>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:apply-templates select="." mode="keyword">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:apply-templates>
				<xsl:value-of select="@name" />
				<xsl:apply-templates select="." mode="derivation">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:apply-templates>
				<xsl:text>&#10;</xsl:text>
			</b>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="delegate" mode="syntax">
		<xsl:param name="lang" />
		<!-- delegates can't be defined in JScript -->
		<xsl:if test="$lang != 'JScript'">
			<xsl:call-template name="syntax-header">
				<xsl:with-param name="lang" select="$lang" />
			</xsl:call-template>
			<b>
				<xsl:apply-templates select="." mode="access">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:apply-templates>
				<xsl:apply-templates select="." mode="gc-type">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:apply-templates>
				<xsl:apply-templates select="." mode="keyword">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:apply-templates>
				<xsl:variable name="link-type">
					<xsl:call-template name="get-datatype">
						<xsl:with-param name="datatype" select="@returnType" />
						<xsl:with-param name="lang" select="$lang" />
					</xsl:call-template>
				</xsl:variable>
				<xsl:if test="$lang != 'Visual Basic'">
					<xsl:call-template name="get-link-for-type-name">
						<xsl:with-param name="type-name" select="@returnType" />
						<xsl:with-param name="link-text" select="$link-type" />
					</xsl:call-template>
				</xsl:if>
				<xsl:text>&#160;</xsl:text>
				<xsl:value-of select="@name" />
				<xsl:call-template name="parameters">
					<xsl:with-param name="include-type-links" select="true()" />
					<xsl:with-param name="lang" select="$lang" />
					<xsl:with-param name="namespace-name" select="../@name" />
				</xsl:call-template>
				<xsl:if test=" $lang = 'C++' and contains(@returnType, '[')">
					<xsl:text>&#160;[]</xsl:text>
				</xsl:if>
				<xsl:if test="$lang='Visual Basic'">
					<xsl:call-template name="return-type">
						<xsl:with-param name="lang" select="$lang" />
						<xsl:with-param name="include-type-links" select="true()" />
						<xsl:with-param name="type" select="@returnType" />
					</xsl:call-template>
				</xsl:if>
				<xsl:text>&#10;</xsl:text>
			</b>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="constructor" mode="inline-syntax">
		<xsl:param name="lang" />
		<xsl:param name="href" />
		<xsl:call-template name="syntax-header">
			<xsl:with-param name="lang" select="$lang" />
		</xsl:call-template>
		<xsl:variable name="link-text">
			<xsl:call-template name="constructor-syntax">
				<xsl:with-param name="lang" select="$lang" />
				<xsl:with-param name="include-type-links" select="false()" />
			</xsl:call-template>
		</xsl:variable>
		<xsl:call-template name="get-link-for-member-overload">
			<xsl:with-param name="link-text" select="$link-text" />
			<xsl:with-param name="member" select="." />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="constructor" mode="syntax">
		<xsl:param name="lang" />
		<xsl:call-template name="syntax-header">
			<xsl:with-param name="lang" select="$lang" />
		</xsl:call-template>
		<b>
			<xsl:call-template name="constructor-syntax">
				<xsl:with-param name="lang" select="$lang" />
				<xsl:with-param name="include-type-links" select="true()" />
			</xsl:call-template>
			<xsl:text>&#10;</xsl:text>
		</b>
	</xsl:template>
	<!-- -->
	<xsl:template name="constructor-syntax">
		<xsl:param name="lang" />
		<xsl:param name="include-type-links" />
		<xsl:call-template name="member-syntax-prolog">
			<xsl:with-param name="lang" select="$lang" />
		</xsl:call-template>
		<xsl:call-template name="constructor-keyword">
			<xsl:with-param name="lang" select="$lang" />
		</xsl:call-template>
		<xsl:if test="$lang!='Visual Basic'">
			<xsl:value-of select="../@name" />
		</xsl:if>
		<xsl:call-template name="parameters">
			<xsl:with-param name="include-type-links" select="$include-type-links" />
			<xsl:with-param name="lang" select="$lang" />
			<xsl:with-param name="namespace-name" select="../../@name" />
		</xsl:call-template>
		<xsl:call-template name="statement-end">
			<xsl:with-param name="lang" select="$lang" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="method" mode="inline-syntax">
		<xsl:param name="lang" />
		<xsl:call-template name="syntax-header">
			<xsl:with-param name="lang" select="$lang" />
		</xsl:call-template>
		<xsl:variable name="link-text">
			<xsl:call-template name="method-syntax">
				<xsl:with-param name="lang" select="$lang" />
				<xsl:with-param name="include-type-links" select="false()" />
			</xsl:call-template>
		</xsl:variable>
		<xsl:call-template name="get-link-for-member-overload">
			<xsl:with-param name="link-text" select="$link-text" />
			<xsl:with-param name="member" select="." />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="method" mode="syntax">
		<xsl:param name="lang" />
		<xsl:call-template name="syntax-header">
			<xsl:with-param name="lang" select="$lang" />
		</xsl:call-template>
		<b>
			<xsl:call-template name="method-syntax">
				<xsl:with-param name="lang" select="$lang" />
				<xsl:with-param name="include-type-links" select="true()" />
			</xsl:call-template>
			<xsl:text>&#10;</xsl:text>
		</b>
	</xsl:template>
	<!-- -->
	<xsl:template name="method-syntax">
		<xsl:param name="include-type-links" />
		<xsl:param name="lang" />
		<xsl:choose>
			<!-- special 'destructor' syntax for Finalize() in c# and c++ -->
			<xsl:when test="@name='Finalize' and not(Parameters) and ($lang = 'C#' or $lang='C++')">
				<xsl:text>~</xsl:text>
				<xsl:value-of select="../@name" />
				<xsl:text>();</xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="member-syntax-prolog">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:call-template>
				<xsl:call-template name="method-start">
					<xsl:with-param name="include-type-links" select="$include-type-links" />
					<xsl:with-param name="lang" select="$lang" />
				</xsl:call-template>
				<xsl:value-of select="@name" />
				<xsl:call-template name="parameters">
					<xsl:with-param name="include-type-links" select="$include-type-links" />
					<xsl:with-param name="lang" select="$lang" />
					<xsl:with-param name="namespace-name" select="../../@name" />
				</xsl:call-template>
				<xsl:if test="$lang = 'Visual Basic'">
					<xsl:call-template name="member-implements">
						<xsl:with-param name="lang" select="$lang" />
					</xsl:call-template>
				</xsl:if>
				<xsl:call-template name="method-end">
					<xsl:with-param name="include-type-links" select="$include-type-links" />
					<xsl:with-param name="lang" select="$lang" />
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="member-syntax-prolog">
		<xsl:param name="lang" />
		<xsl:if test="not(parent::interface or @interface)">
			<xsl:choose>
				<xsl:when test="$lang='Visual Basic'">
					<xsl:if test="@contract and @contract!='Normal' and @contract!='Final'">
						<xsl:apply-templates select="." mode="contract">
							<xsl:with-param name="lang" select="$lang" />
						</xsl:apply-templates>
					</xsl:if>
					<xsl:if test="(local-name()!='constructor') or (@contract!='Static')">
						<xsl:apply-templates select="." mode="access">
							<xsl:with-param name="lang" select="$lang" />
						</xsl:apply-templates>
					</xsl:if>
				</xsl:when>
				<xsl:otherwise>
					<xsl:if test="(local-name()!='constructor') or (@contract!='Static')">
						<xsl:apply-templates select="." mode="access">
							<xsl:with-param name="lang" select="$lang" />
						</xsl:apply-templates>
						<xsl:if test="$lang = 'JScript'">
							<xsl:call-template name="attributes">
								<xsl:with-param name="lang" select="$lang" />
							</xsl:call-template>
						</xsl:if>
					</xsl:if>
					<xsl:if test="@contract and @contract!='Normal' and @contract!='Final'">
						<xsl:apply-templates select="." mode="contract">
							<xsl:with-param name="lang" select="$lang" />
						</xsl:apply-templates>
					</xsl:if>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="method-start">
		<xsl:param name="include-type-links" />
		<xsl:param name="lang" />
		<xsl:apply-templates select="." mode="method-open">
			<xsl:with-param name="lang" select="$lang" />
		</xsl:apply-templates>
		<!-- VB and JScript declare the return type at the end of the declaration -->
		<xsl:if test="$lang != 'Visual Basic' and $lang != 'JScript'">
			<xsl:call-template name="return-type">
				<xsl:with-param name="include-type-links" select="$include-type-links" />
				<xsl:with-param name="lang" select="$lang" />
				<xsl:with-param name="type" select="@returnType" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="method-end">
		<xsl:param name="include-type-links" />
		<xsl:param name="lang" />
		<xsl:if test="$lang = 'Visual Basic' or $lang = 'JScript'">
			<xsl:call-template name="return-type">
				<xsl:with-param name="include-type-links" select="$include-type-links" />
				<xsl:with-param name="lang" select="$lang" />
				<xsl:with-param name="type" select="@returnType" />
			</xsl:call-template>
		</xsl:if>
		<xsl:if test="$lang='C++' and contains(@returnType, '[')">
			<xsl:text>&#160;&#160;__gc[]</xsl:text>
		</xsl:if>
		<xsl:call-template name="statement-end">
			<xsl:with-param name="lang" select="$lang" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template name="member-implements">
		<xsl:if test="implements[not(@inherited)]">
			<xsl:text>&#160;_&#10;&#160;&#160;&#160;&#160;Implements&#160;</xsl:text>
			<xsl:for-each select="implements[not(@inherited)]">
				<xsl:call-template name="get-link-for-type-name">
					<xsl:with-param name="type-name" select="substring-after(@id,':')" />
					<xsl:with-param name="link-text" select="concat(@interface,'.',@name)" />
				</xsl:call-template>
				<xsl:if test="position()!=last()">
					<xsl:text>, </xsl:text>
				</xsl:if>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="return-type">
		<xsl:param name="lang" />
		<xsl:param name="include-type-links" />
		<xsl:param name="type" />
		<xsl:variable name="link-type">
			<xsl:call-template name="get-datatype">
				<xsl:with-param name="datatype" select="$type" />
				<xsl:with-param name="lang" select="$lang" />
			</xsl:call-template>
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="$lang = 'Visual Basic' or $lang = 'JScript'">
				<xsl:if test="$type != 'System.Void'">
					<xsl:call-template name="param-seperator">
						<xsl:with-param name="lang" select="$lang" />
					</xsl:call-template>
					<xsl:choose>
						<xsl:when test="$include-type-links = true()">
							<xsl:call-template name="get-link-for-type-name">
								<xsl:with-param name="type-name" select="$type" />
								<xsl:with-param name="link-text" select="$link-type" />
							</xsl:call-template>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="$link-type" />
						</xsl:otherwise>
					</xsl:choose>
				</xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<xsl:choose>
					<xsl:when test="$include-type-links = true()">
						<xsl:call-template name="get-link-for-type-name">
							<xsl:with-param name="type-name" select="$type" />
							<xsl:with-param name="link-text" select="$link-type" />
						</xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="$link-type" />
					</xsl:otherwise>
				</xsl:choose>
				<xsl:text>&#160;</xsl:text>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template match="property" mode="syntax">
		<xsl:param name="lang" />
		<xsl:call-template name="syntax-header">
			<xsl:with-param name="lang" select="$lang" />
		</xsl:call-template>
		<xsl:call-template name="property-syntax">
			<xsl:with-param name="lang" select="$lang" />
			<xsl:with-param name="include-type-links" select="true()" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="property" mode="inline-syntax">
		<xsl:param name="href" />
		<xsl:param name="lang" />
		<xsl:if test="($lang != 'JScript' and parameter) or not(parameter)">
			<xsl:call-template name="syntax-header">
				<xsl:with-param name="lang" select="$lang" />
			</xsl:call-template>
			<xsl:variable name="link-text">
				<xsl:call-template name="property-syntax">
					<xsl:with-param name="lang" select="$lang" />
					<xsl:with-param name="include-type-links" select="false()" />
				</xsl:call-template>
			</xsl:variable>
			<xsl:call-template name="get-link-for-member-overload">
				<xsl:with-param name="link-text" select="$link-text" />
				<xsl:with-param name="member" select="." />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="field" mode="syntax">
		<xsl:param name="lang" />
		<xsl:call-template name="syntax-header">
			<xsl:with-param name="lang" select="$lang" />
		</xsl:call-template>
		<b>
			<xsl:if test="not(parent::interface)">
				<xsl:apply-templates select="." mode="access">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:apply-templates>
				<xsl:if test="$lang = 'JScript'">
					<xsl:call-template name="attributes">
						<xsl:with-param name="lang" select="$lang" />
					</xsl:call-template>
				</xsl:if>
			</xsl:if>
			<xsl:if test="@contract='Static'">
				<xsl:choose>
					<xsl:when test="@literal='true'">
						<xsl:text>const&#160;</xsl:text>
					</xsl:when>
					<xsl:otherwise>
						<xsl:apply-templates select="." mode="contract">
							<xsl:with-param name="lang" select="$lang" />
						</xsl:apply-templates>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:if>
			<xsl:apply-templates select="." mode="keyword">
				<xsl:with-param name="lang" select="$lang" />
			</xsl:apply-templates>
			<xsl:variable name="link-type">
				<xsl:call-template name="get-datatype">
					<xsl:with-param name="datatype" select="@type" />
					<xsl:with-param name="lang" select="$lang" />
				</xsl:call-template>
			</xsl:variable>
			<xsl:if test="$lang != 'Visual Basic' and $lang != 'JScript'">
				<xsl:call-template name="get-link-for-type-name">
					<xsl:with-param name="type-name" select="@type" />
					<xsl:with-param name="link-text" select="$link-type" />
				</xsl:call-template>
				<xsl:text>&#160;</xsl:text>
			</xsl:if>
			<xsl:value-of select="@name" />
			<xsl:if test="$lang = 'Visual Basic' or $lang = 'JScript'">
				<xsl:call-template name="return-type">
					<xsl:with-param name="lang" select="$lang" />
					<xsl:with-param name="include-type-links" select="true()" />
					<xsl:with-param name="type" select="@type" />
				</xsl:call-template>
			</xsl:if>
			<xsl:if test="$lang='C++' and contains(@type, '[')">
				<xsl:text>&#160;__gc[]</xsl:text>
			</xsl:if>
			<xsl:if test="@literal='true' and @value">
				<xsl:text> = </xsl:text>
				<xsl:if test="@type='System.String'">
					<xsl:text>"</xsl:text>
				</xsl:if>
				<xsl:value-of select="@value" />
				<xsl:if test="@type='System.String'">
					<xsl:text>"</xsl:text>
				</xsl:if>
			</xsl:if>
			<xsl:call-template name="statement-end">
				<xsl:with-param name="lang" select="$lang" />
			</xsl:call-template>
			<xsl:text>&#10;</xsl:text>
		</b>
	</xsl:template>
	<!-- -->
	<xsl:template match="event" mode="syntax">
		<xsl:param name="lang" />
		<!-- events can't be defined in JScript -->
		<xsl:if test="$lang != 'JScript'">
			<xsl:call-template name="syntax-header">
				<xsl:with-param name="lang" select="$lang" />
			</xsl:call-template>
			<b>
				<xsl:if test="not(parent::interface)">
					<xsl:apply-templates select="." mode="access">
						<xsl:with-param name="lang" select="$lang" />
					</xsl:apply-templates>
				</xsl:if>
				<xsl:if test="$lang = 'JScript'">
					<xsl:call-template name="attributes">
						<xsl:with-param name="lang" select="$lang" />
					</xsl:call-template>
				</xsl:if>
				<xsl:apply-templates select="." mode="contract">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:apply-templates>
				<xsl:apply-templates select="." mode="keyword">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:apply-templates>
				<xsl:variable name="link-type">
					<xsl:call-template name="get-datatype">
						<xsl:with-param name="datatype" select="@type" />
						<xsl:with-param name="lang" select="$lang" />
					</xsl:call-template>
				</xsl:variable>
				<xsl:if test="$lang != 'Visual Basic'">
					<xsl:call-template name="get-link-for-type-name">
						<xsl:with-param name="type-name" select="@type" />
						<xsl:with-param name="link-text" select="$link-type" />
					</xsl:call-template>
					<xsl:text>&#160;</xsl:text>
				</xsl:if>
				<xsl:value-of select="@name" />
				<xsl:if test="$lang = 'Visual Basic'">
					<xsl:call-template name="return-type">
						<xsl:with-param name="lang" select="$lang" />
						<xsl:with-param name="include-type-links" select="true()" />
						<xsl:with-param name="type" select="@type" />
					</xsl:call-template>
					<xsl:text>&#160;</xsl:text>
				</xsl:if>
				<xsl:if test="$lang='C++' and contains(@type, '[')">
					<xsl:text>__gc[]&#160;</xsl:text>
				</xsl:if>
				<xsl:call-template name="statement-end">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:call-template>
				<xsl:text>&#10;</xsl:text>
			</b>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="operator" mode="inline-syntax">
		<xsl:param name="lang" />
		<xsl:choose>
			<xsl:when test="$lang='Visual Basic'">
				<xsl:call-template name="vb-operator-inline-syntax">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$lang='JScript'">
				<xsl:call-template name="vb-operator-inline-syntax">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="syntax-header">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:call-template>
				<xsl:variable name="link-text">
					<xsl:call-template name="operator-syntax">
						<xsl:with-param name="lang" select="$lang" />
						<xsl:with-param name="include-type-links" select="false()" />
					</xsl:call-template>
				</xsl:variable>
				<xsl:call-template name="get-link-for-member-overload">
					<xsl:with-param name="link-text" select="$link-text" />
					<xsl:with-param name="member" select="." />
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="vb-operator-inline-syntax">
		<xsl:param name="lang" />
		<xsl:call-template name="syntax-header">
			<xsl:with-param name="lang" select="$lang" />
		</xsl:call-template>
		<xsl:variable name="link-text">
			<xsl:call-template name="operator-name">
				<xsl:with-param name="name">
					<xsl:value-of select="@name" />
				</xsl:with-param>
			</xsl:call-template>
			<xsl:text>&#160;</xsl:text>
			<!--HACK -->
			<xsl:call-template name="parameters">
				<xsl:with-param name="include-type-links" select="false()" />
				<xsl:with-param name="lang" select="'C#'" />
				<xsl:with-param name="namespace-name" select="../../@name" />
			</xsl:call-template>
			<!--HACK END -->
		</xsl:variable>
		<xsl:call-template name="get-link-for-member-overload">
			<xsl:with-param name="link-text" select="$link-text" />
			<xsl:with-param name="member" select="." />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="operator" mode="syntax">
		<xsl:param name="lang" />
		<!-- true and false operators cannot be used by JScript? -->
		<xsl:if test="not((@name='op_True' or @name='op_False') and $lang='JScript')">
			<xsl:call-template name="syntax-header">
				<xsl:with-param name="lang" select="$lang" />
			</xsl:call-template>
			<xsl:call-template name="operator-syntax">
				<xsl:with-param name="lang" select="$lang" />
				<xsl:with-param name="include-type-links" select="true()" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="operator-syntax">
		<xsl:param name="include-type-links" />
		<xsl:param name="lang" />
		<xsl:choose>
			<xsl:when test="$lang = 'C#' or $lang='C++'">
				<b>
					<xsl:call-template name="member-syntax-prolog">
						<xsl:with-param name="lang" select="$lang" />
					</xsl:call-template>
					<xsl:variable name="link-type">
						<xsl:call-template name="get-datatype">
							<xsl:with-param name="datatype" select="@returnType" />
							<xsl:with-param name="lang" select="$lang" />
						</xsl:call-template>
					</xsl:variable>
					<xsl:apply-templates select="." mode="cast-type">
						<xsl:with-param name="lang" select="$lang" />
					</xsl:apply-templates>
					<xsl:choose>
						<xsl:when test="$include-type-links = true()">
							<xsl:call-template name="get-link-for-type-name">
								<xsl:with-param name="type-name" select="@returnType" />
								<xsl:with-param name="link-text" select="$link-type" />
							</xsl:call-template>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="link-type" />
						</xsl:otherwise>
					</xsl:choose>
					<xsl:text>&#160;</xsl:text>
					<xsl:choose>
						<xsl:when test="$lang = 'C#'">
							<xsl:call-template name="csharp-operator-name">
								<xsl:with-param name="name" select="@name" />
							</xsl:call-template>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="@name" />
						</xsl:otherwise>
					</xsl:choose>
					<xsl:call-template name="parameters">
						<xsl:with-param name="include-type-links" select="$include-type-links" />
						<xsl:with-param name="lang" select="$lang" />
						<xsl:with-param name="namespace-name" select="../../@name" />
					</xsl:call-template>
					<xsl:if test="$lang='C++' and contains(@returnType, '[')">
						<xsl:text>&#160;&#160;__gc[]</xsl:text>
					</xsl:if>
					<xsl:text>&#10;</xsl:text>
				</b>
			</xsl:when>
			<xsl:when test="$lang = 'Visual Basic'">
				<I>returnValue</I> = <B><xsl:value-of select="../@name" />.<xsl:value-of select="@name" />
				<xsl:text>(</xsl:text></B>
				<xsl:for-each select="parameter">
					<i>
						<xsl:value-of select="@name" />
					</i>
					<xsl:if test="position()!= last()">
						<xsl:text>,&#160;</xsl:text>
					</xsl:if>
				</xsl:for-each>
				<B>
					<xsl:text>)</xsl:text>
				</B>
			</xsl:when>
			<xsl:when test="$lang = 'JScript'">
				<xsl:call-template name="jscript-operator-syntax" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates select="." mode="keyword">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:apply-templates>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="jscript-operator-syntax">
		<xsl:variable name="operatorsymbol">
			<xsl:call-template name="operator-symbol">
				<xsl:with-param name="name" select="@name" />
			</xsl:call-template>
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="count(./parameter)=2">
				<I>returnValue</I>
				<B> = </B>
				<I>
					<xsl:value-of select="parameter[1]/@name" />
				</I>
				<B>&#160;<xsl:value-of select="$operatorsymbol" />&#160;</B>
				<I>
					<xsl:value-of select="parameter[2]/@name" />
				</I>
				<B>;</B>
				<xsl:text>&#10;</xsl:text>
			</xsl:when>
			<xsl:when test="@name='op_Increment' or @name = 'op_Decrement'">
				<I>returnValue</I>
				<B> = </B>
				<I>
					<xsl:value-of select="parameter[1]/@name" />
				</I>
				<B><xsl:value-of select="$operatorsymbol" />;</B>
				<xsl:text>&#10;</xsl:text>
				<B>-or-</B>
				<xsl:text>&#10;</xsl:text>
				<I>returnValue</I>
				<B> = <xsl:value-of select="$operatorsymbol" /></B>
				<I>
					<xsl:value-of select="parameter[1]/@name" />
				</I>
				<B>;</B>
				<xsl:text>&#10;</xsl:text>
			</xsl:when>
			<xsl:when test="@name='op_True' or @name = 'op_False'">
				<I>returnValue</I>
				<B> = <xsl:value-of select="$operatorsymbol" />;</B>
				<xsl:text>&#10;</xsl:text>
			</xsl:when>
			<xsl:when test="@name = 'op_Explicit'">
				<xsl:variable name="link-type">
					<xsl:call-template name="get-datatype">
						<xsl:with-param name="datatype" select="@returnType" />
						<xsl:with-param name="lang" select="'JScript'" />
					</xsl:call-template>
				</xsl:variable>
				<I>returnValue</I>
				<B><xsl:text>&#160;=&#160;</xsl:text>
				<xsl:call-template name="get-link-for-type-name">
						<xsl:with-param name="type-name" select="@returnType" />
						<xsl:with-param name="link-text" select="$link-type" />
					</xsl:call-template>					
				<xsl:text>(</xsl:text><I>
						<xsl:value-of select="parameter[1]/@name" />
					</I>);</B>
				<xsl:text>&#10;</xsl:text>
			</xsl:when>
			<xsl:when test="@name='op_Implicit'">
				<I>returnValue</I>
				<B> = <I>
						<xsl:value-of select="parameter[1]/@name" />
					</I>;</B>
				<xsl:text>&#10;</xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<I>returnValue</I>
				<B> = <xsl:value-of select="$operatorsymbol" /></B>
				<I>
					<xsl:value-of select="parameter[1]/@name" />
				</I>
				<B>;</B>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template match="structure | interface | class" mode="derivation">
		<xsl:param name="lang" />
		<xsl:choose>
			<xsl:when test="$lang = 'Visual Basic'">
				<xsl:if test="@baseType!=''">
					<xsl:text>&#10;&#160;&#160;&#160;&#160;Inherits&#160;</xsl:text>
					<xsl:variable name="link-type">
						<xsl:call-template name="get-datatype">
							<xsl:with-param name="datatype" select="@baseType" />
							<xsl:with-param name="lang" select="$lang" />
						</xsl:call-template>
					</xsl:variable>
					<xsl:call-template name="get-link-for-type">
						<xsl:with-param name="type" select="./base" />
						<xsl:with-param name="link-text" select="$link-type" />
					</xsl:call-template>
				</xsl:if>
				<xsl:if test="implements[not(@inherited)]">
					<xsl:text>&#10;&#160;&#160;&#160;&#160;Implements&#160;</xsl:text>
					<xsl:for-each select="implements[not(@inherited)]">
						<xsl:variable name="link-type">
							<xsl:call-template name="get-datatype">
								<xsl:with-param name="datatype" select="@type" />
								<xsl:with-param name="lang" select="$lang" />
							</xsl:call-template>
						</xsl:variable>
						<xsl:call-template name="get-link-for-type-name">
							<xsl:with-param name="type-name" select="@type" />
							<xsl:with-param name="link-text" select="$link-type" />
						</xsl:call-template>
						<xsl:if test="position()!=last()">
							<xsl:text>, </xsl:text>
						</xsl:if>
					</xsl:for-each>
				</xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<xsl:if test="@baseType!='' or implements[not(@inherited)]">
					<xsl:apply-templates select="." mode="inherits">
						<xsl:with-param name="lang" select="$lang" />
					</xsl:apply-templates>
					<xsl:if test="@baseType!=''">
						<xsl:variable name="link-type">
							<xsl:call-template name="get-datatype">
								<xsl:with-param name="datatype" select="@baseType" />
								<xsl:with-param name="lang" select="$lang" />
							</xsl:call-template>
						</xsl:variable>
						<xsl:call-template name="get-link-for-type">
							<xsl:with-param name="type" select="./base" />
							<xsl:with-param name="link-text" select="$link-type" />
						</xsl:call-template>
						<xsl:if test="implements[not(@inherited)]">
							<xsl:text>, </xsl:text>
						</xsl:if>
					</xsl:if>
					<xsl:for-each select="implements[not(@inherited)]">
						<xsl:variable name="link-type">
							<xsl:call-template name="get-datatype">
								<xsl:with-param name="datatype" select="@type" />
								<xsl:with-param name="lang" select="$lang" />
							</xsl:call-template>
						</xsl:variable>
						<xsl:call-template name="get-link-for-type-name">
							<xsl:with-param name="type-name" select="@type" />
							<xsl:with-param name="link-text" select="$link-type" />
						</xsl:call-template>
						<xsl:if test="position()!=last()">
							<xsl:text>, </xsl:text>
						</xsl:if>
					</xsl:for-each>
				</xsl:if>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="property-syntax">
		<xsl:param name="include-type-links" />
		<xsl:param name="lang" />
		<xsl:choose>
			<xsl:when test="$lang = 'Visual Basic'">
				<xsl:apply-templates select="." mode="vb-property-syntax">
					<xsl:with-param name="include-type-links" select="$include-type-links" />
				</xsl:apply-templates>
			</xsl:when>
			<xsl:when test="$lang = 'C#'">
				<xsl:apply-templates select="." mode="csharp-property-syntax">
					<xsl:with-param name="include-type-links" select="$include-type-links" />
				</xsl:apply-templates>
			</xsl:when>
			<xsl:when test="$lang = 'C++'">
				<xsl:apply-templates select="." mode="cpp-property-syntax">
					<xsl:with-param name="include-type-links" select="$include-type-links" />
				</xsl:apply-templates>
			</xsl:when>
			<xsl:when test="$lang = 'JScript'">
				<xsl:apply-templates select="." mode="js-property-syntax">
					<xsl:with-param name="include-type-links" select="$include-type-links" />
				</xsl:apply-templates>
			</xsl:when>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="parameters">
		<xsl:param name="lang" />
		<xsl:param name="namespace-name" />
		<xsl:param name="include-type-links" />
		<xsl:call-template name="parameters-list">
			<xsl:with-param name="lang" select="$lang" />
			<xsl:with-param name="namespace-name" select="$namespace-name" />
			<xsl:with-param name="include-type-links" select="$include-type-links" />
			<xsl:with-param name="open-paren" select="'('" />
			<xsl:with-param name="close-paren" select="')'" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template name="parameters-list">
		<xsl:param name="lang" />
		<xsl:param name="namespace-name" />
		<xsl:param name="include-type-links" />
		<xsl:param name="open-paren" />
		<xsl:param name="close-paren" />
		<xsl:param name="dir" />
		<xsl:value-of select="$open-paren" />
		<xsl:if test="parameter">
			<xsl:for-each select="parameter">
				<xsl:if test="$include-type-links=true()">
					<xsl:call-template name="statement-continue">
						<xsl:with-param name="lang" select="$lang" />
					</xsl:call-template>
					<xsl:text>&#10;</xsl:text>
					<xsl:text>&#160;&#160;&#160;</xsl:text>
				</xsl:if>
				<xsl:if test="$lang = 'Visual Basic' and @optional = 'true'">
					<xsl:text>Optional </xsl:text>
				</xsl:if>
				<xsl:apply-templates select="." mode="dir">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:apply-templates>
				<xsl:apply-templates select="." mode="param-array">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:apply-templates>
				<xsl:if test="$include-type-links=true()">
					<xsl:if test="$lang = 'Visual Basic'">
						<i>
							<xsl:value-of select="@name" />
						</i>
						<xsl:text>&#160;As&#160;</xsl:text>
					</xsl:if>
				</xsl:if>
				<xsl:choose>
					<xsl:when test="$include-type-links=true()">
						<xsl:variable name="link-type">
							<xsl:call-template name="get-datatype">
								<xsl:with-param name="datatype" select="@type" />
								<xsl:with-param name="lang" select="$lang" />
							</xsl:call-template>
						</xsl:variable>
						<xsl:call-template name="get-link-for-type-name">
							<xsl:with-param name="type-name" select="@type" />
							<xsl:with-param name="link-text" select="$link-type" />
						</xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="get-datatype">
							<xsl:with-param name="datatype" select="@type" />
							<xsl:with-param name="lang" select="$lang" />
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:if test="$include-type-links=true()">
					<xsl:if test="$lang != 'Visual Basic'">
						<xsl:text>&#160;</xsl:text>
						<i>
							<xsl:value-of select="@name" />
						</i>
					</xsl:if>
				</xsl:if>
				<xsl:if test="$lang = 'Visual Basic' and @optional = 'true'">
					<xsl:text> = </xsl:text>
					<xsl:if test="@type='System.String'">"</xsl:if>
					<xsl:value-of select="@defaultValue" />
					<xsl:if test="@type='System.String'">"</xsl:if>
				</xsl:if>
				<xsl:if test="$lang='C++' and contains(@type, '[')">
					<xsl:text>&#160;__gc[]</xsl:text>
				</xsl:if>
				<!-- c++ indexer setters also include the return type in the param list -->
				<xsl:if test="position() != last() or ($lang='C++' and parent::property and $dir = 'set')">
					<xsl:text>,</xsl:text>
				</xsl:if>
			</xsl:for-each>
			<xsl:if test="$include-type-links=true()">
				<xsl:call-template name="statement-continue">
					<xsl:with-param name="lang" select="$lang" />
				</xsl:call-template>
				<xsl:text>&#10;</xsl:text>
			</xsl:if>
		</xsl:if>
		<xsl:value-of select="$close-paren" />
	</xsl:template>
	<!-- -->
	<xsl:template name="get-datatype">
		<xsl:param name="datatype" />
		<xsl:param name="lang" />
		<xsl:variable name="type-temp">
			<xsl:call-template name="lang-type">
				<xsl:with-param name="runtime-type" select="$datatype" />
				<xsl:with-param name="lang" select="$lang" />
			</xsl:call-template>
		</xsl:variable>
		<xsl:call-template name="strip-namespace">
			<xsl:with-param name="name" select="$type-temp" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<!-- member.xslt is using this for title and h1.  should try and use parameters template above. -->
	<xsl:template name="get-param-list">
		<xsl:text>(</xsl:text>
		<xsl:for-each select="parameter">
			<xsl:call-template name="strip-namespace">
				<xsl:with-param name="name" select="@type" />
			</xsl:call-template>
			<xsl:if test="position()!=last()">
				<xsl:text>, </xsl:text>
			</xsl:if>
		</xsl:for-each>
		<xsl:text>)</xsl:text>
	</xsl:template>
	<!-- -->
	<!-- ATTRIBUTES -->
	<xsl:template name="attributes">
		<xsl:param name="lang" />
		<xsl:if test="$ndoc-document-attributes">
			<xsl:if test="attribute">
				<xsl:choose>
					<xsl:when test="$lang='Visual Basic'">
						<div class="attribute">
							<xsl:apply-templates select="." mode="attribute-open">
								<xsl:with-param name="lang" select="$lang" />
							</xsl:apply-templates>
							<xsl:for-each select="attribute">
								<xsl:call-template name="attribute">
									<xsl:with-param name="attname" select="@name" />
									<xsl:with-param name="lang" select="$lang" />
								</xsl:call-template>
								<xsl:if test="position()!=last()">
									<xsl:text>, _&#10;&#160;</xsl:text>
								</xsl:if>
							</xsl:for-each>
							<xsl:apply-templates select="." mode="attribute-close">
								<xsl:with-param name="lang" select="$lang" />
							</xsl:apply-templates>
						</div>
					</xsl:when>
					<xsl:otherwise>
						<xsl:for-each select="attribute">
							<div class="attribute">
								<xsl:apply-templates select="." mode="attribute-open">
									<xsl:with-param name="lang" select="$lang" />
								</xsl:apply-templates>
								<xsl:call-template name="attribute">
									<xsl:with-param name="attname" select="@name" />
									<xsl:with-param name="lang" select="$lang" />
								</xsl:call-template>
								<xsl:apply-templates select="." mode="attribute-close">
									<xsl:with-param name="lang" select="$lang" />
								</xsl:apply-templates>
							</div>
						</xsl:for-each>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:if>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="attribute">
		<xsl:param name="attname" />
		<xsl:param name="lang" />
		<xsl:if test="@target"><xsl:value-of select="@target" /> : </xsl:if>
		<xsl:call-template name="strip-namespace-and-attribute">
			<xsl:with-param name="name" select="@name" />
		</xsl:call-template>
		<xsl:if test="count(property | field) > 0">
			<xsl:text>(</xsl:text>
			<xsl:for-each select="property | field">
				<xsl:value-of select="@name" />
				<xsl:choose>
				<xsl:when test="$lang='Visual Basic'"><xsl:text>:=</xsl:text></xsl:when>
				<xsl:otherwise><xsl:text>=</xsl:text></xsl:otherwise>
				</xsl:choose>
				<xsl:choose>
					<xsl:when test="@value">
						<xsl:if test="@type='System.String'">
							<xsl:text>"</xsl:text>
						</xsl:if>
						<xsl:choose>
						<xsl:when test="@type!='System.String' and $lang='Visual Basic'"><xsl:value-of select="NUtil:Replace(@value,'|',' Or ')" /></xsl:when>
						<xsl:otherwise><xsl:value-of select="@value" /></xsl:otherwise>
						</xsl:choose>
						<xsl:if test="@type='System.String'">
							<xsl:text>"</xsl:text>
						</xsl:if>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>**UNKNOWN**</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:if test="position()!=last()">
					<xsl:text>, </xsl:text>
				</xsl:if>
			</xsl:for-each>
			<xsl:text>)</xsl:text>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="strip-namespace-and-attribute">
		<xsl:param name="name" />
		<xsl:choose>
			<xsl:when test="contains($name, '.')">
				<xsl:call-template name="strip-namespace-and-attribute">
					<xsl:with-param name="name" select="substring-after($name, '.')" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="substring-before(concat($name, '_____'), 'Attribute_____')" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
</xsl:stylesheet>
