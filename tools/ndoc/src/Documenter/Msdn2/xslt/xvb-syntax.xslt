<?xml version="1.0" encoding="UTF-8" ?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:user="urn:my-scripts"
    exclude-result-prefixes="msxsl user"
>
	<!-- -->
	<xsl:param name="ndoc-vb-syntax" />
	<!-- -->
	<xsl:template name="vb-type">
		<xsl:param name="runtime-type" />
		<xsl:variable name="old-type">
			<xsl:choose>
				<xsl:when test="contains($runtime-type, '[')">
					<xsl:value-of select="substring-before($runtime-type, '[')" />
				</xsl:when>
				<xsl:when test="contains($runtime-type, '&amp;')">
					<xsl:value-of select="substring-before($runtime-type, '&amp;')" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$runtime-type" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="new-type">
			<xsl:choose>
				<xsl:when test="$old-type='System.Byte'">Byte</xsl:when>
				<xsl:when test="$old-type='System.Int16'">Short</xsl:when>
				<xsl:when test="$old-type='System.Int32'">Integer</xsl:when>
				<xsl:when test="$old-type='System.Int64'">Long</xsl:when>
				<xsl:when test="$old-type='System.Single'">Single</xsl:when>
				<xsl:when test="$old-type='System.Double'">Double</xsl:when>
				<xsl:when test="$old-type='System.Decimal'">Decimal</xsl:when>
				<xsl:when test="$old-type='System.String'">String</xsl:when>
				<xsl:when test="$old-type='System.Char'">Char</xsl:when>
				<xsl:when test="$old-type='System.Boolean'">Boolean</xsl:when>
				<xsl:when test="$old-type='System.DateTime'">Date</xsl:when>
				<xsl:when test="$old-type='System.Object'">Object</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="strip-namespace">
						<xsl:with-param name="name" select="$old-type" />
					</xsl:call-template>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="contains($runtime-type, '[')">
				<xsl:value-of select="concat($new-type, '(', translate(substring-after($runtime-type, '['), ']', ')'))" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$new-type" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="vb-type-syntax">
		<xsl:if test="$ndoc-vb-syntax">
			<div class="syntax">
				<span class="lang">[Visual&#160;Basic]</span>
				<br/>
				<xsl:call-template name="vb-attributes"/>
				<xsl:if test="@abstract = 'true'">
					<xsl:text>MustInherit&#160;</xsl:text>
				</xsl:if>
				<xsl:if test="@sealed = 'true'">
					<xsl:text>NotInheritable&#160;</xsl:text>
				</xsl:if>
				<xsl:call-template name="vb-type-access">
					<xsl:with-param name="access" select="@access" />
					<xsl:with-param name="type" select="local-name()" />
				</xsl:call-template>
				<xsl:text>&#160;</xsl:text>
				<xsl:choose>
					<xsl:when test="local-name() = 'class'">Class</xsl:when>
					<xsl:when test="local-name() = 'interface'">Interface</xsl:when>
					<xsl:when test="local-name() = 'structure'">Structure</xsl:when>
					<xsl:when test="local-name() = 'enumeration'">Enum</xsl:when>
					<xsl:when test="local-name() = 'delegate'">
						<xsl:text>Delegate&#160;</xsl:text>
						<xsl:choose>
							<xsl:when test="@returnType = 'System.Void'">Sub</xsl:when>
							<xsl:otherwise>Function</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<xsl:otherwise>ERROR</xsl:otherwise>
				</xsl:choose>
				<xsl:text>&#160;</xsl:text>
				<xsl:value-of select="@name" />
				<xsl:choose>
					<xsl:when test="local-name() != 'delegate'">
						<xsl:if test="@baseType">
							<div>
							<xsl:text>Inherits&#160;</xsl:text>
							<xsl:value-of select="@baseType" />
							</div>
						</xsl:if>
						<xsl:if test="implements[not(@inherited)]">
							<div>
							<xsl:text>Implements&#160;</xsl:text>
							<xsl:for-each select="implements[not(@inherited)]">
								<xsl:value-of select="." />
								<xsl:if test="position()!=last()">
									<xsl:text>, </xsl:text>
								</xsl:if>
							</xsl:for-each>
							</div>
						</xsl:if>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="vb-parameters" />
					</xsl:otherwise>
				</xsl:choose>
			</div>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="vb-parameters">
		<xsl:choose>
			<xsl:when test="parameter">
				<xsl:text>( _</xsl:text>
				<br />
				<xsl:apply-templates select="parameter" mode="vb" />
				<xsl:text>)</xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<xsl:text>()</xsl:text>
			</xsl:otherwise>
		</xsl:choose>
		<xsl:if test="@returnType != 'System.Void'">
			<xsl:text>&#160;As&#160;</xsl:text>
			<a>
				<xsl:attribute name="href">
					<xsl:call-template name="get-filename-for-type-name">
						<xsl:with-param name="type-name" select="@returnType" />
					</xsl:call-template>
				</xsl:attribute>
				<xsl:call-template name="vb-type">
					<xsl:with-param name="runtime-type" select="@returnType" />
				</xsl:call-template>
			</a>
		</xsl:if>
		<xsl:if test="implements">
			<xsl:text> Implements _</xsl:text>
			<br /><xsl:text>&#160;&#160;&#160;</xsl:text>
			<xsl:value-of select="implements/@interface" /><xsl:text>.</xsl:text><xsl:value-of select="implements/@name" />
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="vb-type-access">
		<xsl:param name="access" />
		<xsl:param name="type" />
		<xsl:choose>
			<xsl:when test="$access='Public'">Public</xsl:when>
			<xsl:when test="$access='NotPublic'">Friend</xsl:when>
			<xsl:when test="$access='NestedPublic'">Public</xsl:when>
			<xsl:when test="$access='NestedFamily'">Protected</xsl:when>
			<xsl:when test="$access='NestedFamilyOrAssembly'">Protected Friend</xsl:when>
			<xsl:when test="$access='NestedAssembly'">Friend</xsl:when>
			<xsl:when test="$access='NestedPrivate'">Private</xsl:when>
			<xsl:otherwise>/* unknown */</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="vb-method-access">
		<xsl:param name="access" />
		<xsl:choose>
			<xsl:when test="$access='Public'">Public</xsl:when>
			<xsl:when test="$access='Family'">Protected</xsl:when>
			<xsl:when test="$access='FamilyOrAssembly'">Protected Friend</xsl:when>
			<xsl:when test="$access='Assembly'">Friend</xsl:when>
			<xsl:when test="$access='Private'">Private</xsl:when>
			<xsl:otherwise>/* unknown */</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template match="parameter" mode="vb">
		<xsl:text>&#160;&#160;&#160;</xsl:text>
		<xsl:if test="@optional = 'true'">
			<xsl:text>Optional </xsl:text>
		</xsl:if>
		<xsl:choose>
			<xsl:when test="@isParamArray = 'true'">
				<xsl:text>ParamArray </xsl:text>
			</xsl:when>
			<xsl:when test="@direction = 'ref' or @direction = 'out'">
				<xsl:text>ByRef </xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<xsl:text>ByVal </xsl:text>
			</xsl:otherwise>
		</xsl:choose>
		<i><xsl:value-of select="@name" /></i>
		<xsl:text>&#160;As&#160;</xsl:text>
		<a>
			<xsl:attribute name="href">
				<xsl:call-template name="get-filename-for-type-name">
					<xsl:with-param name="type-name" select="@type" />
				</xsl:call-template>
			</xsl:attribute>
			<xsl:call-template name="vb-type">
				<xsl:with-param name="runtime-type" select="@type" />
			</xsl:call-template>
		</a>
		<xsl:if test="@optional = 'true'">
		  <xsl:text> = </xsl:text>
		  <xsl:if test="@type='System.String'">"</xsl:if>
		  <xsl:value-of select="@defaultValue" />
		  <xsl:if test="@type='System.String'">"</xsl:if>
		</xsl:if>
		<xsl:if test="position() != last()">
			<xsl:text>,</xsl:text>
		</xsl:if>
		<xsl:text>&#160;_</xsl:text>
		<br />
	</xsl:template>
	<!-- -->
	<xsl:template name="vb-member-syntax">
		<xsl:if test="$ndoc-vb-syntax">
			<div class="syntax">
				<span class="lang">[Visual&#160;Basic]</span>
				<br />
				<xsl:call-template name="vb-attributes"/>
				<xsl:choose>
					<xsl:when test="local-name() != 'operator'">
						<xsl:if test="not(parent::interface or @interface)">
							<xsl:choose>
								<xsl:when test="@contract='Abstract'">
									<xsl:text>MustOverride&#160;</xsl:text>
								</xsl:when>
								<xsl:when test="@contract='Final'">
									<xsl:text>NotOverridable&#160;</xsl:text>
								</xsl:when>
								<xsl:when test="@contract='Override'">
									<xsl:text>Overrides&#160;</xsl:text>
								</xsl:when>
								<xsl:when test="@contract='Virtual'">
									<xsl:text>Overridable&#160;</xsl:text>
								</xsl:when>
							</xsl:choose>
							<xsl:if test="@overload">
								<xsl:text>Overloads&#160;</xsl:text>
							</xsl:if>
							<xsl:call-template name="vb-method-access">
								<xsl:with-param name="access" select="@access" />
							</xsl:call-template>
							<xsl:text>&#160;</xsl:text>
							<xsl:if test="@contract='Static'">
								<xsl:text>Shared&#160;</xsl:text>
							</xsl:if>
						</xsl:if>
						<xsl:choose>
							<xsl:when test="@returnType!='System.Void'">
								<xsl:text>Function</xsl:text>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>Sub</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:text>&#160;</xsl:text>
						<xsl:choose>
							<xsl:when test="local-name() = 'constructor'">
								<xsl:text>New</xsl:text>
							</xsl:when>
							<xsl:otherwise>
								<xsl:call-template name="strip-namespace">
									<xsl:with-param name="name" select="@name" />
								</xsl:call-template>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:call-template name="vb-parameters" />
					</xsl:when>
					<xsl:otherwise>
						<span class="meta">returnValue = </span>
						<xsl:value-of select="../@name" />
						<xsl:text>.</xsl:text>
						<xsl:value-of select="@name" />
						<xsl:text>(</xsl:text>
						<xsl:for-each select="parameter">
							<xsl:value-of select="@name" />
							<xsl:if test="position() &lt; last()">
								<xsl:text>,&#160;</xsl:text>
							</xsl:if>
						</xsl:for-each>
						<xsl:text>)</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</div>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="vb-field-or-event-syntax">
		<xsl:if test="$ndoc-vb-syntax">
			<div class="syntax">
				<span class="lang">[Visual&#160;Basic]</span>
				<br />
				<xsl:call-template name="vb-attributes"/>
				<xsl:if test="not(parent::interface)">
					<xsl:call-template name="vb-method-access">
						<xsl:with-param name="access" select="@access" />
					</xsl:call-template>
					<xsl:text>&#160;</xsl:text>
				</xsl:if>
				<xsl:if test="@contract='Static'">
					<xsl:choose>
						<xsl:when test="@literal='true'">
							<xsl:text>Const&#160;</xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>Shared&#160;</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:if>
				<xsl:if test="@initOnly='true'">
					<xsl:text>ReadOnly&#160;</xsl:text>
				</xsl:if>
				<xsl:if test="local-name() = 'event'">
					<xsl:text>Event&#160;</xsl:text>
				</xsl:if>
				<xsl:value-of select="@name" />
				<xsl:text>&#160;As&#160;</xsl:text>
				<a>
					<xsl:attribute name="href">
						<xsl:call-template name="get-filename-for-type-name">
							<xsl:with-param name="type-name" select="@type" />
						</xsl:call-template>
					</xsl:attribute>
					<xsl:call-template name="vb-type">
						<xsl:with-param name="runtime-type" select="@type" />
					</xsl:call-template>
				</a>
			</div>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="vb-property-syntax">
		<xsl:if test="$ndoc-vb-syntax">
			<xsl:call-template name="vb-attributes"/>
			<xsl:if test="not(parent::interface)">
				<xsl:choose>
					<xsl:when test="@contract='Abstract'">
						<xsl:text>MustOverride&#160;</xsl:text>
					</xsl:when>
					<xsl:when test="@contract='Final'">
						<xsl:text>NotOverridable&#160;</xsl:text>
					</xsl:when>
					<xsl:when test="@contract='Override'">
						<xsl:text>Overrides&#160;</xsl:text>
					</xsl:when>
					<xsl:when test="@contract='Virtual'">
						<xsl:text>Overridable&#160;</xsl:text>
					</xsl:when>
				</xsl:choose>
				<xsl:if test="@overload">
					<xsl:text>Overloads&#160;</xsl:text>
				</xsl:if>
				<xsl:call-template name="vb-method-access">
					<xsl:with-param name="access" select="@access" />
				</xsl:call-template>
				<xsl:text>&#160;</xsl:text>
				<xsl:if test="@contract='Static'">
					<xsl:text>Shared&#160;</xsl:text>
				</xsl:if>
				<xsl:if test="parameter">
					<xsl:text>Default&#160;</xsl:text>
				</xsl:if>
				<xsl:if test="@set != 'true'">
					<xsl:text>ReadOnly&#160;</xsl:text>
				</xsl:if>
			</xsl:if>
			<xsl:text>Property&#160;</xsl:text>
			<xsl:value-of select="@name" />
			<xsl:if test="parameter">
				<xsl:call-template name="vb-parameters" />
			</xsl:if>
			<xsl:text>&#160;As&#160;</xsl:text>
			<a>
				<xsl:attribute name="href">
					<xsl:call-template name="get-filename-for-type-name">
						<xsl:with-param name="type-name" select="@type" />
					</xsl:call-template>
				</xsl:attribute>
				<xsl:call-template name="vb-type">
					<xsl:with-param name="runtime-type" select="@type" />
				</xsl:call-template>
			</a>
			<xsl:if test="implements">
				<xsl:text> Implements _</xsl:text>
				<br /><xsl:text>&#160;&#160;&#160;</xsl:text>
				<xsl:value-of select="implements/@interface" /><xsl:text>.</xsl:text><xsl:value-of select="implements/@name" />
			</xsl:if>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<!-- ATTRIBUTES -->
	<xsl:template name="vb-attributes">
		<xsl:if test="$ndoc-document-attributes">
			<xsl:if test="attribute">
				<xsl:for-each select="attribute">
					<div class="attribute"><xsl:call-template name="vb-attribute">
						<xsl:with-param name="attname" select="@name" />
					</xsl:call-template></div>
				</xsl:for-each>
			</xsl:if>
		</xsl:if>
	</xsl:template>
	<!-- -->	
	<xsl:template name="vb-attribute">
		<xsl:param name="attname" />
		<xsl:if test="user:isAttributeWanted($ndoc-documented-attributes, @name)">			
			<xsl:text>&lt;</xsl:text>
			<xsl:if test="@target"><xsl:value-of select="@target" /> : </xsl:if>
			<xsl:call-template name="strip-namespace-and-attribute">
				<xsl:with-param name="name" select="@name" />
			</xsl:call-template>
			<xsl:if test="count(property) > 0">
				<xsl:text>(</xsl:text>
				<xsl:for-each select="property">
					<xsl:if test="user:isPropertyWanted($ndoc-documented-attributes, @name) and @value!=''">
						<xsl:value-of select="@name" />
						<xsl:text>="</xsl:text>
						<xsl:value-of select="@value" />
						<xsl:text>"</xsl:text>
						<xsl:if test="position()!=last()"><xsl:text>, </xsl:text></xsl:if>
					</xsl:if>
				</xsl:for-each>
				<xsl:text>)</xsl:text>
			</xsl:if>
			<xsl:text>&gt;</xsl:text>
		</xsl:if>
	</xsl:template>
	<!-- -->
</xsl:transform>
