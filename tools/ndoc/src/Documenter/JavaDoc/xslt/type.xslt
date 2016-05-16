<?xml version="1.0" encoding="utf-8" ?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- -->
	<xsl:include href="javadoc.xslt" />
	<!-- -->
	<xsl:param name="global-path-to-root" />
	<xsl:param name="global-type-id" />
	<!-- -->
	<xsl:variable name="spaces" select="'                                                                '" />
	<!-- -->
	<xsl:template match="/">
		<xsl:variable name="type" select="ndoc/assembly/module/namespace/*[@id=$global-type-id]" />
		<xsl:variable name="fields" select="$type/field[not(@declaringType)]" />
		<xsl:variable name="constructors" select="$type/constructor" />
		<xsl:variable name="properties" select="$type/property[not(@declaringType)]" />
		<xsl:variable name="methods" select="$type/method[not(@declaringType)]" />
		<xsl:variable name="operators" select="$type/operator" />
		<xsl:variable name="events" select="$type/event[not(@declaringType)]" />
		<html>
			<xsl:call-template name="output-head" />
			<body>
				<xsl:variable name="navigation-bar">
					<xsl:call-template name="output-navigation-bar">
						<xsl:with-param name="select" select="'Type'" />
						<xsl:with-param name="link-namespace" select="true()" />
						<xsl:with-param name="prev-next-what" select="'TYPE'" />
						<xsl:with-param name="type-node" select="$type" />
						<xsl:with-param name="fields" select="$fields" />
						<xsl:with-param name="constructors" select="$constructors" />
						<xsl:with-param name="properties" select="$properties" />
						<xsl:with-param name="methods" select="$methods" />
						<xsl:with-param name="operators" select="$operators" />
						<xsl:with-param name="events" select="$events" />
					</xsl:call-template>
				</xsl:variable>
				<xsl:copy-of select="$navigation-bar" />
				<hr />
				<h2>
					<span class="namespaceName">
						<xsl:value-of select="$type/parent::namespace/@name" />
					</span>
					<br />
					<span class="className">
						<xsl:choose>
							<xsl:when test="local-name($type)='interface'">
								<xsl:text>Interface </xsl:text>
							</xsl:when>
							<xsl:when test="local-name($type)='class'">
								<xsl:text>Class </xsl:text>
							</xsl:when>
							<xsl:when test="local-name($type)='structure'">
								<xsl:text>Structure </xsl:text>
							</xsl:when>
						</xsl:choose>
						<xsl:value-of select="$type/@name" />
					</span>
				</h2>
				<xsl:if test="$type/documentation/summary">
					<p>
						<xsl:apply-templates select="$type/documentation/summary" mode="doc" />
					</p>
				</xsl:if>
				<xsl:if test="$type/documentation/remarks">
					<p>
						<xsl:apply-templates select="$type/documentation/remarks" mode="doc" />
					</p>
				</xsl:if>
				<a name="field-summary" />
				<xsl:if test="$fields">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th colspan="2">Field Summary</th>
							</tr>
						</thead>
						<xsl:apply-templates select="$fields">
							<xsl:sort select="@name" />
						</xsl:apply-templates>
					</table>
					<br />
				</xsl:if>
				<xsl:for-each select="$type/descendant::base">
					<xsl:call-template name="inherited-members">
						<xsl:with-param name="type" select="$type" />
						<xsl:with-param name="base-id" select="substring-after(@id, 'T:')" />
						<xsl:with-param name="type-of-members" select="'Fields'" />
						<xsl:with-param name="member-element" select="'field'" />
					</xsl:call-template>
				</xsl:for-each>
				<a name="constructor-summary" />
				<xsl:if test="$constructors">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th colspan="2">Constructor Summary</th>
							</tr>
						</thead>
						<xsl:apply-templates select="$constructors" />
					</table>
					<br />
				</xsl:if>
				<a name="property-summary" />
				<xsl:if test="$properties">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th colspan="2">Property Summary</th>
							</tr>
						</thead>
						<xsl:apply-templates select="$properties">
							<xsl:sort select="@name" />
						</xsl:apply-templates>
					</table>
					<br />
				</xsl:if>
				<xsl:for-each select="$type/descendant::base">
					<xsl:call-template name="inherited-members">
						<xsl:with-param name="type" select="$type" />
						<xsl:with-param name="base-id" select="substring-after(@id, 'T:')" />
						<xsl:with-param name="type-of-members" select="'Properties'" />
						<xsl:with-param name="member-element" select="'property'" />
					</xsl:call-template>
				</xsl:for-each>
				<a name="method-summary" />
				<xsl:if test="$methods">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th colspan="2">Method Summary</th>
							</tr>
						</thead>
						<xsl:apply-templates select="$methods">
							<xsl:sort select="@name" />
						</xsl:apply-templates>
					</table>
					<br />
				</xsl:if>
				<xsl:for-each select="$type/descendant::base">
					<xsl:call-template name="inherited-members">
						<xsl:with-param name="type" select="$type" />
						<xsl:with-param name="base-id" select="substring-after(@id, 'T:')" />
						<xsl:with-param name="type-of-members" select="'Methods'" />
						<xsl:with-param name="member-element" select="'method'" />
					</xsl:call-template>
				</xsl:for-each>
				<xsl:call-template name="inherited-members">
					<xsl:with-param name="type" select="$type" />
					<xsl:with-param name="base-id" select="'System.Object'" />
					<xsl:with-param name="type-of-members" select="'Methods'" />
					<xsl:with-param name="member-element" select="'method'" />
				</xsl:call-template>
				<a name="operator-summary" />
				<xsl:if test="$operators">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th colspan="2">Operator Summary</th>
							</tr>
						</thead>
						<xsl:apply-templates select="$operators">
							<xsl:sort select="@name" />
						</xsl:apply-templates>
					</table>
					<br />
				</xsl:if>
				<a name="event-summary" />
				<xsl:if test="$events">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th colspan="2">Event Summary</th>
							</tr>
						</thead>
						<xsl:apply-templates select="$events">
							<xsl:sort select="@name" />
						</xsl:apply-templates>
					</table>
					<br />
				</xsl:if>
				<xsl:for-each select="$type/descendant::base">
					<xsl:call-template name="inherited-members">
						<xsl:with-param name="type" select="$type" />
						<xsl:with-param name="base-id" select="substring-after(@id, 'T:')" />
						<xsl:with-param name="type-of-members" select="'Events'" />
						<xsl:with-param name="member-element" select="'event'" />
					</xsl:call-template>
				</xsl:for-each>
				<br />
				<a name="field-detail" />
				<xsl:if test="$fields">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th>Field Detail</th>
							</tr>
						</thead>
					</table>
					<xsl:variable name="last" select="count($fields)" />
					<xsl:for-each select="$fields">
						<xsl:sort select="@name" />
						<xsl:apply-templates select="." mode="detail" />
						<xsl:if test="position()!=$last">
							<hr />
						</xsl:if>
					</xsl:for-each>
				</xsl:if>
				<a name="constructor-detail" />
				<xsl:if test="$constructors">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th>Constructor Detail</th>
							</tr>
						</thead>
					</table>
					<xsl:variable name="last" select="count($constructors)" />
					<xsl:for-each select="$constructors">
						<xsl:apply-templates select="." mode="detail" />
						<xsl:if test="position()!=$last">
							<hr />
						</xsl:if>
					</xsl:for-each>
				</xsl:if>
				<a name="property-detail" />
				<xsl:if test="$properties">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th>Property Detail</th>
							</tr>
						</thead>
					</table>
					<xsl:variable name="last" select="count($properties)" />
					<xsl:for-each select="$properties">
						<xsl:sort select="@name" />
						<xsl:apply-templates select="." mode="detail" />
						<xsl:if test="position()!=$last">
							<hr />
						</xsl:if>
					</xsl:for-each>
				</xsl:if>
				<a name="method-detail" />
				<xsl:if test="$methods">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th>Method Detail</th>
							</tr>
						</thead>
					</table>
					<xsl:variable name="last" select="count($methods)" />
					<xsl:for-each select="$methods">
						<xsl:sort select="@name" />
						<xsl:apply-templates select="." mode="detail" />
						<xsl:if test="position()!=$last">
							<hr />
						</xsl:if>
					</xsl:for-each>
				</xsl:if>
				<a name="operator-detail" />
				<xsl:if test="$operators">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th>Operator Detail</th>
							</tr>
						</thead>
					</table>
					<xsl:variable name="last" select="count($operators)" />
					<xsl:for-each select="$operators">
						<xsl:sort select="@name" />
						<xsl:apply-templates select="." mode="detail" />
						<xsl:if test="position()!=$last">
							<hr />
						</xsl:if>
					</xsl:for-each>
				</xsl:if>
				<a name="event-detail" />
				<xsl:if test="$events">
					<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
					<table class="table" cellspacing="0">
						<thead>
							<tr>
								<th colspan="2">Event Detail</th>
							</tr>
						</thead>
					</table>
					<xsl:variable name="last" select="count($events)" />
					<xsl:for-each select="$events">
						<xsl:sort select="@name" />
						<xsl:apply-templates select="." mode="detail" />
						<xsl:if test="position()!=$last">
							<hr />
						</xsl:if>
					</xsl:for-each>
				</xsl:if>
				<hr />
				<xsl:copy-of select="$navigation-bar" />
			</body>
		</html>
	</xsl:template>
	<!-- -->
	<xsl:template match="field">
		<tr>
			<!-- Is there a CSS property that can emulate the valign attribute? -->
			<td class="fieldType" valign="top">
				<xsl:call-template name="csharp-type">
					<xsl:with-param name="type" select="@type" />
				</xsl:call-template>
			</td>
			<td class="field">
				<a href="#{substring-after(@id, 'F:')}">
					<xsl:value-of select="@name" />
				</a>
				<xsl:if test="documentation/summary">
					<br />
					<xsl:text>&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;</xsl:text>
					<xsl:apply-templates select="documentation/summary" mode="doc" />
				</xsl:if>
			</td>
		</tr>
	</xsl:template>
	<!-- -->
	<xsl:template match="constructor">
		<tr>
			<!-- Is there a CSS property that can emulate the valign attribute? -->
			<td class="constructor" valign="top">
				<a href="#{substring-after(@id, 'M:')}">
					<xsl:value-of select="../@name" />
				</a>
				<xsl:text>(</xsl:text>
				<xsl:for-each select="parameter">
					<xsl:call-template name="csharp-type">
						<xsl:with-param name="type" select="@type" />
					</xsl:call-template>
					<xsl:text>&#160;</xsl:text>
					<xsl:value-of select="@name" />
					<xsl:if test="position()!=last()">
						<xsl:text>,&#160;</xsl:text>
					</xsl:if>
				</xsl:for-each>
				<xsl:text>)</xsl:text>
				<xsl:if test="documentation/summary">
					<br />
					<xsl:text>&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;</xsl:text>
					<xsl:apply-templates select="documentation/summary" mode="doc" />
				</xsl:if>
			</td>
		</tr>
	</xsl:template>
	<!-- -->
	<xsl:template match="property">
		<tr>
			<!-- Is there a CSS property that can emulate the valign attribute? -->
			<td class="propertyType" valign="top">
				<xsl:call-template name="csharp-type">
					<xsl:with-param name="type" select="@type" />
				</xsl:call-template>
			</td>
			<td class="property">
				<a href="#{substring-after(@id, 'P:')}">
					<xsl:value-of select="@name" />
				</a>
				<xsl:if test="parameter">
					<xsl:text>[</xsl:text>
					<xsl:for-each select="parameter">
						<xsl:call-template name="csharp-type">
							<xsl:with-param name="type" select="@type" />
						</xsl:call-template>
						<xsl:text>&#160;</xsl:text>
						<xsl:value-of select="@name" />
						<xsl:if test="position()!=last()">
							<xsl:text>,&#160;</xsl:text>
						</xsl:if>
					</xsl:for-each>
					<xsl:text>]</xsl:text>
				</xsl:if>
				<xsl:if test="documentation/summary">
					<br />
					<xsl:text>&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;</xsl:text>
					<xsl:apply-templates select="documentation/summary" mode="doc" />
				</xsl:if>
			</td>
		</tr>
	</xsl:template>
	<!-- -->
	<xsl:template match="method|operator">
		<tr>
			<!-- Is there a CSS property that can emulate the valign attribute? -->
			<td class="returnType" valign="top">
				<xsl:call-template name="csharp-type">
					<xsl:with-param name="type" select="@returnType" />
				</xsl:call-template>
			</td>
			<td class="method">
				<a href="#{substring-after(@id, 'M:')}">
					<xsl:choose>
						<xsl:when test="name()='operator'">
							<xsl:call-template name="csharp-operator-name">
								<xsl:with-param name="name" select="@name" />
							</xsl:call-template>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="@name" />
						</xsl:otherwise>
					</xsl:choose>
				</a>
				<xsl:text>(</xsl:text>
				<xsl:for-each select="parameter">
					<xsl:call-template name="csharp-type">
						<xsl:with-param name="type" select="@type" />
					</xsl:call-template>
					<xsl:text>&#160;</xsl:text>
					<xsl:value-of select="@name" />
					<xsl:if test="position()!=last()">
						<xsl:text>,&#160;</xsl:text>
					</xsl:if>
				</xsl:for-each>
				<xsl:text>)</xsl:text>
				<xsl:if test="documentation/summary">
					<br />
					<xsl:text>&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;</xsl:text>
					<xsl:apply-templates select="documentation/summary" mode="doc" />
				</xsl:if>
			</td>
		</tr>
	</xsl:template>
	<!-- -->
	<xsl:template match="event">
		<tr>
			<!-- Is there a CSS property that can emulate the valign attribute? -->
			<td class="eventType" valign="top">
				<xsl:call-template name="csharp-type">
					<xsl:with-param name="type" select="@type" />
				</xsl:call-template>
			</td>
			<td class="event">
				<a href="#{substring-after(@id, 'E:')}">
					<xsl:value-of select="@name" />
				</a>
				<xsl:if test="documentation/summary">
					<br />
					<xsl:text>&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;</xsl:text>
					<xsl:apply-templates select="documentation/summary" mode="doc" />
				</xsl:if>
			</td>
		</tr>
	</xsl:template>
	<!-- -->
	<xsl:template name="inherited-members">
		<xsl:param name="type" />
		<xsl:param name="base-id" />
		<xsl:param name="type-of-members" />
		<xsl:param name="member-element" />
		<xsl:variable name="inherited-members" select="$type/*[name()=$member-element and @declaringType=$base-id]" />
		<xsl:if test="$inherited-members">
			<!-- IE doesn't support the border-spacing CSS property so we have to set the cellspacing attribute here. -->
			<table class="subtable" cellspacing="0">
				<thead>
					<tr>
						<th>
							<xsl:value-of select="$type-of-members" />
							<xsl:text> inherited from class </xsl:text>
							<xsl:value-of select="$base-id" />
						</th>
					</tr>
				</thead>
				<tr>
					<td>
						<xsl:variable name="member-count" select="count($inherited-members)" />
						<xsl:for-each select="$inherited-members">
							<xsl:sort select="@name" />
							<a>
								<xsl:attribute name="href">
									<xsl:call-template name="get-href-to-cref">
										<xsl:with-param name="cref" select="@id" />
									</xsl:call-template>
								</xsl:attribute>
								<xsl:value-of select="@name" />
							</a>
							<xsl:if test="position() &lt; $member-count">, </xsl:if>
						</xsl:for-each>
					</td>
				</tr>
			</table>
			<br />
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="field" mode="detail">
		<a name="{substring-after(@id, 'F:')}" />
		<h3>
			<xsl:value-of select="@name" />
		</h3>
		<pre>
			<xsl:call-template name="csharp-member-access">
				<xsl:with-param name="access" select="@access" />
			</xsl:call-template>
			<xsl:text>&#32;</xsl:text>
			<xsl:call-template name="csharp-type">
				<xsl:with-param name="type" select="@type" />
			</xsl:call-template>
			<xsl:text>&#32;</xsl:text>
			<b>
				<xsl:value-of select="@name" />
			</b>
		</pre>
		<dl>
			<dd>
				<p>
					<xsl:apply-templates select="documentation/summary" mode="doc" />
				</p>
				<p>
					<xsl:apply-templates select="documentation/remarks" mode="doc" />
				</p>
			</dd>
		</dl>
	</xsl:template>
	<!-- -->
	<xsl:template match="constructor" mode="detail">
		<a name="{substring-after(@id, 'M:')}" />
		<h3>
			<xsl:value-of select="../@name" />
		</h3>
		<pre>
			<xsl:variable name="constructor">
				<xsl:call-template name="csharp-member-access">
					<xsl:with-param name="access" select="@access" />
				</xsl:call-template>
				<xsl:text>&#32;</xsl:text>
				<b>
					<xsl:value-of select="../@name" />
				</b>
			</xsl:variable>
			<xsl:value-of select="$constructor" />
			<xsl:text>(</xsl:text>
			<xsl:for-each select="parameter">
				<xsl:call-template name="csharp-type">
					<xsl:with-param name="type" select="@type" />
				</xsl:call-template>
				<xsl:text>&#32;</xsl:text>
				<xsl:value-of select="@name" />
				<xsl:if test="position()!=last()">
					<xsl:text>,&#10;&#32;</xsl:text>
					<xsl:value-of select="substring($spaces, 1, string-length($constructor))" />
				</xsl:if>
			</xsl:for-each>
			<xsl:text>)</xsl:text>
		</pre>
		<dl>
			<dd>
				<p>
					<xsl:apply-templates select="documentation/summary" mode="doc" />
				</p>
				<p>
					<xsl:apply-templates select="documentation/remarks" mode="doc" />
				</p>
				<xsl:call-template name="param-section" />
				<xsl:call-template name="exception-section" />
			</dd>
		</dl>
	</xsl:template>
	<!-- -->
	<xsl:template match="property" mode="detail">
		<a name="{substring-after(@id, 'P:')}" />
		<h3>
			<xsl:value-of select="@name" />
		</h3>
		<pre>
			<xsl:variable name="property">
				<xsl:call-template name="csharp-member-access">
					<xsl:with-param name="access" select="@access" />
				</xsl:call-template>
				<xsl:text>&#32;</xsl:text>
				<xsl:call-template name="csharp-type">
					<xsl:with-param name="type" select="@type" />
				</xsl:call-template>
				<xsl:text>&#32;</xsl:text>
				<b>
					<xsl:choose>
						<xsl:when test="@name='Item'">this</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="@name" />
						</xsl:otherwise>
					</xsl:choose>
				</b>
			</xsl:variable>
			<xsl:value-of select="$property" />
			<xsl:if test="parameter">
				<xsl:text>[</xsl:text>
				<xsl:for-each select="parameter">
					<xsl:call-template name="csharp-type">
						<xsl:with-param name="type" select="@type" />
					</xsl:call-template>
					<xsl:text>&#32;</xsl:text>
					<xsl:value-of select="@name" />
					<xsl:if test="position()!=last()">
						<xsl:text>,&#10;&#32;</xsl:text>
						<xsl:value-of select="substring($spaces, 1, string-length($property))" />
					</xsl:if>
				</xsl:for-each>
				<xsl:text>]</xsl:text>
			</xsl:if>
		</pre>
		<dl>
			<dd>
				<p>
					<xsl:apply-templates select="documentation/summary" mode="doc" />
				</p>
				<p>
					<xsl:apply-templates select="documentation/remarks" mode="doc" />
				</p>
				<xsl:call-template name="param-section" />
				<xsl:call-template name="value-section" />
				<xsl:call-template name="exception-section" />
			</dd>
		</dl>
	</xsl:template>
	<!-- -->
	<xsl:template match="method|operator" mode="detail">
		<a name="{substring-after(@id, 'M:')}" />
		<h3>
			<xsl:choose>
				<xsl:when test="name()='operator'">
					<xsl:call-template name="operator-name">
						<xsl:with-param name="name" select="@name" />
					</xsl:call-template>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="@name" />
				</xsl:otherwise>
			</xsl:choose>
		</h3>
		<pre>
			<xsl:variable name="method">
				<xsl:call-template name="csharp-member-access">
					<xsl:with-param name="access" select="@access" />
				</xsl:call-template>
				<xsl:text>&#32;</xsl:text>
				<xsl:call-template name="csharp-type">
					<xsl:with-param name="type" select="@returnType" />
				</xsl:call-template>
				<xsl:text>&#32;</xsl:text>
				<b>
					<xsl:choose>
						<xsl:when test="name()='operator'">
							<xsl:call-template name="csharp-operator-name">
								<xsl:with-param name="name" select="@name" />
							</xsl:call-template>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="@name" />
						</xsl:otherwise>
					</xsl:choose>
				</b>
			</xsl:variable>
			<xsl:value-of select="$method" />
			<xsl:text>(</xsl:text>
			<xsl:for-each select="parameter">
				<xsl:call-template name="csharp-type">
					<xsl:with-param name="type" select="@type" />
				</xsl:call-template>
				<xsl:text>&#32;</xsl:text>
				<xsl:value-of select="@name" />
				<xsl:if test="position()!=last()">
					<xsl:text>,&#10;&#32;</xsl:text>
					<xsl:value-of select="substring($spaces, 1, string-length($method))" />
				</xsl:if>
			</xsl:for-each>
			<xsl:text>)</xsl:text>
		</pre>
		<dl>
			<dd>
				<p>
					<xsl:apply-templates select="documentation/summary" mode="doc" />
				</p>
				<p>
					<xsl:apply-templates select="documentation/remarks" mode="doc" />
				</p>
				<xsl:call-template name="param-section" />
				<xsl:call-template name="returns-section" />
				<xsl:call-template name="exception-section" />
			</dd>
		</dl>
	</xsl:template>
	<!-- -->
	<xsl:template match="event" mode="detail">
		<a name="{substring-after(@id, 'E:')}" />
		<h3>
			<xsl:value-of select="@name" />
		</h3>
		<pre>
			<xsl:call-template name="csharp-member-access">
				<xsl:with-param name="access" select="@access" />
			</xsl:call-template>
			<xsl:text>&#32;</xsl:text>
			<xsl:call-template name="csharp-type">
				<xsl:with-param name="type" select="@type" />
			</xsl:call-template>
			<xsl:text>&#32;</xsl:text>
			<b>
				<xsl:value-of select="@name" />
			</b>
		</pre>
		<dl>
			<dd>
				<p>
					<xsl:apply-templates select="documentation/summary" mode="doc" />
				</p>
				<p>
					<xsl:apply-templates select="documentation/remarks" mode="doc" />
				</p>
			</dd>
		</dl>
	</xsl:template>
	<!-- -->
	<xsl:template name="param-section">
		<xsl:if test="documentation/param">
			<b>Parameters:</b>
			<dl>
				<xsl:apply-templates select="documentation/param" />
			</dl>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="param">
		<dd>
			<code>
				<xsl:value-of select="@name" />
			</code>
			<xsl:text> - </xsl:text>
			<xsl:apply-templates mode="doc" />
		</dd>
	</xsl:template>
	<!-- -->
	<xsl:template name="value-section">
		<xsl:if test="documentation/value">
			<b>Value:</b>
			<dl>
				<dd>
					<xsl:apply-templates select="documentation/value/node()" mode="doc" />
				</dd>
			</dl>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="returns-section">
		<xsl:if test="documentation/returns">
			<b>Returns:</b>
			<dl>
				<dd>
					<xsl:apply-templates select="documentation/returns/node()" mode="doc" />
				</dd>
			</dl>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="exception-section">
		<xsl:if test="documentation/exception">
			<b>Throws:</b>
			<dl>
				<xsl:apply-templates select="documentation/exception" />
			</dl>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="exception">
		<dd>
			<code>
				<xsl:value-of select="substring-after(@cref, 'T:')" />
			</code>
			<xsl:text> - </xsl:text>
			<xsl:apply-templates mode="doc" />
		</dd>
	</xsl:template>
	<!-- -->
</xsl:transform>
