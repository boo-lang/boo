<?xml version="1.0" encoding="utf-8" ?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- -->
	<xsl:template name="output-head">
		<head>
			<link rel="stylesheet" type="text/css" href="{$global-path-to-root}JavaDoc.css" />
		</head>
	</xsl:template>
	<!-- -->
	<xsl:template name="output-navigation-bar">
		<xsl:param name="select" />
		<xsl:param name="link-namespace" />
		<xsl:param name="prev-next-what" />
		<xsl:param name="type-node" />
		<xsl:param name="fields" select="false()" />
		<xsl:param name="constructors" select="false()" />
		<xsl:param name="properties" select="false()" />
		<xsl:param name="methods" select="false()" />
		<xsl:param name="operators" select="false()" />
		<xsl:param name="events" select="false()" />
		<table class="nav">
			<tr>
				<td class="nav1" colspan="2">
					<table cellspacing="3">
						<tr>
							<td>
								<xsl:choose>
									<xsl:when test="$select='Overview'">
										<xsl:attribute name="class">nav1sel</xsl:attribute>
										<xsl:text>&#160;Overview&#160;</xsl:text>
									</xsl:when>
									<xsl:otherwise>
										<a href="{$global-path-to-root}overview-summary.html">&#160;Overview&#160;</a>
									</xsl:otherwise>
								</xsl:choose>
							</td>
							<td>
								<xsl:choose>
									<xsl:when test="$select='Namespace'">
										<xsl:attribute name="class">nav1sel</xsl:attribute>
										<xsl:text>&#160;Namespace&#160;</xsl:text>
									</xsl:when>
									<xsl:otherwise>
										<xsl:choose>
											<xsl:when test="$link-namespace">
												<a href="namespace-summary.html">Namespace</a>
												<xsl:text>&#160;</xsl:text>
											</xsl:when>
											<xsl:otherwise>
												<xsl:text>Namespace&#160;</xsl:text>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:otherwise>
								</xsl:choose>
							</td>
							<td>
								<xsl:if test="$select='Type'">
									<xsl:attribute name="class">nav1sel</xsl:attribute>
									<xsl:text>&#160;</xsl:text>
								</xsl:if>
								<xsl:text>Type&#160;</xsl:text>
							</td>
							<td>
								<xsl:if test="$select='Use'">
									<xsl:attribute name="class">nav1sel</xsl:attribute>
									<xsl:text>&#160;</xsl:text>
								</xsl:if>
								<xsl:text>Use&#160;</xsl:text>
							</td>
							<td>
								<xsl:if test="$select='Tree'">
									<xsl:attribute name="class">nav1sel</xsl:attribute>
									<xsl:text>&#160;</xsl:text>
								</xsl:if>
								<xsl:text>Tree&#160;</xsl:text>
							</td>
							<td>
								<xsl:if test="$select='Deprecated'">
									<xsl:attribute name="class">nav1sel</xsl:attribute>
									<xsl:text>&#160;</xsl:text>
								</xsl:if>
								<xsl:text>Deprecated&#160;</xsl:text>
							</td>
							<td>
								<xsl:if test="$select='Index'">
									<xsl:attribute name="class">nav1sel</xsl:attribute>
									<xsl:text>&#160;</xsl:text>
								</xsl:if>
								<xsl:text>Index&#160;</xsl:text>
							</td>
							<td>
								<xsl:if test="$select='Help'">
									<xsl:attribute name="class">nav1sel</xsl:attribute>
									<xsl:text>&#160;</xsl:text>
								</xsl:if>
								<xsl:text>Help&#160;</xsl:text>
							</td>
						</tr>
					</table>
				</td>
				<td class="logo" rowspan="2">.NET Framework</td>
			</tr>
			<tr class="nav2">
				<td>
					<xsl:text>PREV</xsl:text>
					<xsl:if test="$prev-next-what">
						<xsl:text>&#160;</xsl:text>
						<xsl:value-of select="$prev-next-what" />
					</xsl:if>
					<xsl:text>&#160;&#160;&#160;&#160;NEXT</xsl:text>
					<xsl:if test="$prev-next-what">
						<xsl:text>&#160;</xsl:text>
						<xsl:value-of select="$prev-next-what" />
					</xsl:if>
				</td>
				<td>FRAMES&#160;&#160;&#160;&#160;NO FRAMES</td>
			</tr>
			<xsl:if test="$type-node">
				<tr class="nav2">
					<td>
						<xsl:text>SUMMARY: </xsl:text>
						<xsl:text>INNER</xsl:text>
						<xsl:text> | </xsl:text>
						<xsl:choose>
							<xsl:when test="$fields">
								<a href="#field-summary">FIELD</a>
							</xsl:when>
							<xsl:otherwise>FIELD</xsl:otherwise>
						</xsl:choose>
						<xsl:text> | </xsl:text>
						<xsl:choose>
							<xsl:when test="$constructors">
								<a href="#constructor-summary">CONST</a>
							</xsl:when>
							<xsl:otherwise>CONST</xsl:otherwise>
						</xsl:choose>
						<xsl:text> | </xsl:text>
						<xsl:choose>
							<xsl:when test="$properties">
								<a href="#property-summary">PROP</a>
							</xsl:when>
							<xsl:otherwise>PROP</xsl:otherwise>
						</xsl:choose>
						<xsl:text> | </xsl:text>
						<xsl:choose>
							<xsl:when test="$methods">
								<a href="#method-summary">METHOD</a>
							</xsl:when>
							<xsl:otherwise>METHOD</xsl:otherwise>
						</xsl:choose>
						<xsl:text> | </xsl:text>
						<xsl:choose>
							<xsl:when test="$operators">
								<a href="#operator-summary">OP</a>
							</xsl:when>
							<xsl:otherwise>OP</xsl:otherwise>
						</xsl:choose>
						<xsl:text> | </xsl:text>
						<xsl:choose>
							<xsl:when test="$events">
								<a href="#event-summary">EVENT</a>
							</xsl:when>
							<xsl:otherwise>EVENT</xsl:otherwise>
						</xsl:choose>
					</td>
					<td>
						<xsl:text>DETAIL: </xsl:text>
						<xsl:choose>
							<xsl:when test="$fields">
								<a href="#field-detail">FIELD</a>
							</xsl:when>
							<xsl:otherwise>FIELD</xsl:otherwise>
						</xsl:choose>
						<xsl:text> | </xsl:text>
						<xsl:choose>
							<xsl:when test="$constructors">
								<a href="#constructor-detail">CONST</a>
							</xsl:when>
							<xsl:otherwise>CONST</xsl:otherwise>
						</xsl:choose>
						<xsl:text> | </xsl:text>
						<xsl:choose>
							<xsl:when test="$properties">
								<a href="#property-detail">PROP</a>
							</xsl:when>
							<xsl:otherwise>PROP</xsl:otherwise>
						</xsl:choose>
						<xsl:text> | </xsl:text>
						<xsl:choose>
							<xsl:when test="$methods">
								<a href="#method-detail">METHOD</a>
							</xsl:when>
							<xsl:otherwise>METHOD</xsl:otherwise>
						</xsl:choose>
						<xsl:text> | </xsl:text>
						<xsl:choose>
							<xsl:when test="$operators">
								<a href="#operator-detail">OP</a>
							</xsl:when>
							<xsl:otherwise>OP</xsl:otherwise>
						</xsl:choose>
						<xsl:text> | </xsl:text>
						<xsl:choose>
							<xsl:when test="$events">
								<a href="#event-detail">EVENT</a>
							</xsl:when>
							<xsl:otherwise>EVENT</xsl:otherwise>
						</xsl:choose>
					</td>
				</tr>
			</xsl:if>
		</table>
	</xsl:template>
	<!-- -->
	<xsl:template name="get-href-to-namespace-summary">
		<xsl:param name="namespace-name" />
		<xsl:value-of select="concat(translate($namespace-name, '.', '/'), '/namespace-summary.html')" />
	</xsl:template>
	<!-- -->
	<xsl:template name="get-href-to-type">
		<xsl:param name="type-name" />
		<xsl:value-of select="concat($type-name, '.html')" />
	</xsl:template>
	<!-- -->
	<xsl:template name="get-href-to-constructor">
		<xsl:param name="constructor-node" />
		<xsl:value-of select="concat('#.ctor', $constructor-node/@overload)" />
	</xsl:template>
	<!-- -->
	<xsl:template name="get-href-to-field">
		<xsl:param name="field-node" />
		<xsl:value-of select="concat('#', $field-node/@name)" />
	</xsl:template>
	<!-- -->
	<xsl:template name="get-href-to-property">
		<xsl:param name="property-node" />
		<xsl:value-of select="concat('#', $property-node/@name, $property-node/@overload)" />
	</xsl:template>
	<!-- -->
	<xsl:template name="get-href-to-method">
		<xsl:param name="method-node" />
		<xsl:value-of select="concat('#', $method-node/@name, $method-node/@overload)" />
	</xsl:template>
	<!-- -->
	<xsl:template name="get-href-to-event">
		<xsl:param name="event-node" />
		<xsl:value-of select="concat('#', $event-node/@name)" />
	</xsl:template>
	<!-- -->
	<xsl:template match="*" mode="doc">
		<xsl:apply-templates mode="doc" />
	</xsl:template>
	<!-- -->
	<xsl:template match="summary" mode="doc">
		<xsl:apply-templates mode="doc" />
	</xsl:template>
	<!-- -->
	<xsl:template match="remarks" mode="doc">
		<xsl:apply-templates mode="doc" />
	</xsl:template>
	<!-- -->
	<xsl:template name="get-namespace">
		<xsl:param name="name" />
		<xsl:param name="namespace" />
		<xsl:choose>
			<xsl:when test="contains($name, '.')">
				<xsl:call-template name="get-namespace">
					<xsl:with-param name="name" select="substring-after($name, '.')" />
					<xsl:with-param name="namespace" select="concat($namespace, substring-before($name, '.'), '.')" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="substring($namespace, 1, string-length($namespace) - 1)" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<!-- -->
	<xsl:template name="strip-namespace">
		<xsl:param name="name" />
		<xsl:choose>
			<xsl:when test="contains($name, '.')">
				<xsl:call-template name="strip-namespace">
					<xsl:with-param name="name" select="substring-after($name, '.')" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$name" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="csharp-type">
		<xsl:param name="type" />
		<xsl:variable name="old-type">
			<xsl:choose>
				<xsl:when test="contains($type, '[]')">
					<xsl:value-of select="substring-before($type, '[]')" />
				</xsl:when>
				<xsl:when test="contains($type, '&amp;')">
					<xsl:value-of select="substring-before($type, '&amp;')" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$type" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="new-type">
			<xsl:choose>
				<xsl:when test="$old-type='System.Byte'">byte</xsl:when>
				<xsl:when test="$old-type='System.SByte'">sbyte</xsl:when>
				<xsl:when test="$old-type='System.Int16'">short</xsl:when>
				<xsl:when test="$old-type='System.UInt16'">ushort</xsl:when>
				<xsl:when test="$old-type='System.Int32'">int</xsl:when>
				<xsl:when test="$old-type='System.UInt32'">uint</xsl:when>
				<xsl:when test="$old-type='System.Int64'">long</xsl:when>
				<xsl:when test="$old-type='System.UInt64'">ulong</xsl:when>
				<xsl:when test="$old-type='System.Single'">float</xsl:when>
				<xsl:when test="$old-type='System.Double'">double</xsl:when>
				<xsl:when test="$old-type='System.Decimal'">decimal</xsl:when>
				<xsl:when test="$old-type='System.String'">string</xsl:when>
				<xsl:when test="$old-type='System.Char'">char</xsl:when>
				<xsl:when test="$old-type='System.Boolean'">bool</xsl:when>
				<xsl:when test="$old-type='System.Void'">void</xsl:when>
				<xsl:when test="$old-type='System.Object'">object</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$old-type" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="contains($type, '[]')">
				<xsl:value-of select="concat($new-type, '[]')" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$new-type" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="csharp-member-access">
		<xsl:param name="access" />
		<xsl:choose>
			<xsl:when test="$access='Public'">public</xsl:when>
			<xsl:when test="$access='Family'">protected</xsl:when>
			<xsl:when test="$access='FamilyOrAssembly'">protected internal</xsl:when>
			<xsl:when test="$access='Assembly'">internal</xsl:when>
			<xsl:when test="$access='Private'">private</xsl:when>
			<xsl:otherwise>ERROR</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="operator-name">
		<xsl:param name="name" />
		<xsl:choose>
			<xsl:when test="$name='op_UnaryPlus'">Unary Plus Operator</xsl:when>
			<xsl:when test="$name='op_UnaryNegation'">Unary Negation Operator</xsl:when>
			<xsl:when test="$name='op_LogicalNot'">Logical Not Operator</xsl:when>
			<xsl:when test="$name='op_OnesComplement'">Ones Complement Operator</xsl:when>
			<xsl:when test="$name='op_Increment'">Increment Operator</xsl:when>
			<xsl:when test="$name='op_Decrement'">Decrement Operator</xsl:when>
			<xsl:when test="$name='op_True'">True Operator</xsl:when>
			<xsl:when test="$name='op_False'">False Operator</xsl:when>
			<xsl:when test="$name='op_Addition'">Addition Operator</xsl:when>
			<xsl:when test="$name='op_Subtraction'">Subtraction Operator</xsl:when>
			<xsl:when test="$name='op_Multiply'">Multiplication Operator</xsl:when>
			<xsl:when test="$name='op_Division'">Division Operator</xsl:when>
			<xsl:when test="$name='op_Modulus'">Modulus Operator</xsl:when>
			<xsl:when test="$name='op_BitwiseAnd'">Bitwise And Operator</xsl:when>
			<xsl:when test="$name='op_BitwiseOr'">Bitwise Or Operator</xsl:when>
			<xsl:when test="$name='op_ExclusiveOr'">Exclusive Or Operator</xsl:when>
			<xsl:when test="$name='op_LeftShift'">Left Shift Operator</xsl:when>
			<xsl:when test="$name='op_RightShift'">Right Shift Operator</xsl:when>
			<xsl:when test="$name='op_Equality'">Equality Operator</xsl:when>
			<xsl:when test="$name='op_Inequality'">Inequality Operator</xsl:when>
			<xsl:when test="$name='op_LessThan'">Less Than Operator</xsl:when>
			<xsl:when test="$name='op_GreaterThan'">Greater Than Operator</xsl:when>
			<xsl:when test="$name='op_LessThanOrEqual'">Less Than Or Equal Operator</xsl:when>
			<xsl:when test="$name='op_GreaterThanOrEqual'">Greater Than Or Equal Operator</xsl:when>
			<xsl:otherwise>ERROR</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="csharp-operator-name">
		<xsl:param name="name" />
		<xsl:choose>
			<xsl:when test="$name='op_UnaryPlus'">operator +</xsl:when>
			<xsl:when test="$name='op_UnaryNegation'">operator -</xsl:when>
			<xsl:when test="$name='op_LogicalNot'">operator !</xsl:when>
			<xsl:when test="$name='op_OnesComplement'">operator ~</xsl:when>
			<xsl:when test="$name='op_Increment'">operator ++</xsl:when>
			<xsl:when test="$name='op_Decrement'">operator --</xsl:when>
			<xsl:when test="$name='op_True'">operator true</xsl:when>
			<xsl:when test="$name='op_False'">operator false</xsl:when>
			<xsl:when test="$name='op_Addition'">operator +</xsl:when>
			<xsl:when test="$name='op_Subtraction'">operator -</xsl:when>
			<xsl:when test="$name='op_Multiply'">operator *</xsl:when>
			<xsl:when test="$name='op_Division'">operator /</xsl:when>
			<xsl:when test="$name='op_Modulus'">operator %</xsl:when>
			<xsl:when test="$name='op_BitwiseAnd'">operator &amp;</xsl:when>
			<xsl:when test="$name='op_BitwiseOr'">operator |</xsl:when>
			<xsl:when test="$name='op_ExclusiveOr'">operator ^</xsl:when>
			<xsl:when test="$name='op_LeftShift'">operator &lt;&lt;</xsl:when>
			<xsl:when test="$name='op_RightShift'">operator >></xsl:when>
			<xsl:when test="$name='op_Equality'">operator ==</xsl:when>
			<xsl:when test="$name='op_Inequality'">operator !=</xsl:when>
			<xsl:when test="$name='op_LessThan'">operator &lt;</xsl:when>
			<xsl:when test="$name='op_GreaterThan'">operator ></xsl:when>
			<xsl:when test="$name='op_LessThanOrEqual'">operator &lt;=</xsl:when>
			<xsl:when test="$name='op_GreaterThanOrEqual'">operator >=</xsl:when>
			<xsl:otherwise>ERROR</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template match="c" mode="doc">
		<i>
			<xsl:apply-templates mode="doc" />
		</i>
	</xsl:template>
	<!-- -->
	<xsl:template match="code" mode="doc">
		<pre class="code">
			<xsl:apply-templates mode="doc" />
		</pre>
	</xsl:template>
	<!-- -->
	<xsl:template match="list[@type='bullet']" mode="doc">
		<ul>
			<xsl:apply-templates select="item" mode="doc" />
		</ul>
	</xsl:template>
	<!-- -->
	<xsl:template match="list[@type='number']" mode="doc">
		<ol>
			<xsl:apply-templates select="item" mode="doc" />
		</ol>
	</xsl:template>
	<!-- -->
	<xsl:template match="item" mode="doc">
		<li>
			<xsl:apply-templates mode="doc" />
		</li>
	</xsl:template>
	<!-- -->
	<xsl:template match="term" mode="doc">
		<b>
			<xsl:apply-templates mode="doc" />
			<xsl:text> - </xsl:text>
		</b>
	</xsl:template>
	<!-- -->
	<xsl:template match="description" mode="doc">
		<xsl:apply-templates mode="doc" />
	</xsl:template>
	<!-- -->
	<xsl:template match="para" mode="doc">
		<p>
			<xsl:apply-templates mode="doc" />
		</p>
	</xsl:template>
	<!-- -->
	<xsl:template match="paramref[@name]" mode="doc">
		<i>
			<xsl:value-of select="@name" />
		</i>
	</xsl:template>
	<!-- -->
	<xsl:template match="see[@cref]" mode="doc">
		<a>
			<xsl:attribute name="href">
				<xsl:call-template name="get-href-to-cref">
					<xsl:with-param name="cref" select="@cref" />
				</xsl:call-template>
			</xsl:attribute>
			<xsl:value-of select="substring(@cref, 3)" />
		</a>
	</xsl:template>
	<!-- -->
	<xsl:template name="get-href-to-cref">
		<xsl:param name="cref" />
		<xsl:choose>
			<xsl:when test="starts-with($cref, 'T:')">
				<xsl:variable name="namespace">
					<xsl:call-template name="get-namespace">
						<xsl:with-param name="name" select="substring-after($cref, 'T:')" />
					</xsl:call-template>
				</xsl:variable>
				<xsl:variable name="name">
					<xsl:call-template name="strip-namespace">
						<xsl:with-param name="name" select="substring-after($cref, 'T:')" />
					</xsl:call-template>
				</xsl:variable>
				<xsl:value-of select="concat($global-path-to-root, translate($namespace, '.', '/'), '/', $name, '.html')" />
			</xsl:when>
			<xsl:when test="starts-with($cref, 'M:')">
				<xsl:variable name="type">
					<xsl:call-template name="get-namespace">
						<xsl:with-param name="name" select="substring-after($cref, 'M:')" />
					</xsl:call-template>
				</xsl:variable>
				<xsl:variable name="path">
					<xsl:call-template name="get-href-to-cref">
						<xsl:with-param name="cref" select="concat('T:', $type)" />
					</xsl:call-template>
				</xsl:variable>
				<xsl:variable name="member">
					<xsl:call-template name="strip-namespace">
						<xsl:with-param name="name" select="substring-after($cref, 'M:')" />
					</xsl:call-template>
				</xsl:variable>
				<xsl:value-of select="concat($path, '#', $member)" />
			</xsl:when>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template match="see[@langword]" mode="doc">
		<xsl:choose>
			<xsl:when test="@langword='null'">
				<xsl:text>a null reference (</xsl:text>
				<b>Nothing</b>
				<xsl:text> in Visual Basic)</xsl:text>
			</xsl:when>
			<xsl:when test="@langword='sealed'">
				<xsl:text>sealed (</xsl:text>
				<b>NotInheritable</b>
				<xsl:text> in Visual Basic)</xsl:text>
			</xsl:when>
			<xsl:when test="@langword='static'">
				<xsl:text>static (</xsl:text>
				<b>Shared</b>
				<xsl:text> in Visual Basic)</xsl:text>
			</xsl:when>
			<xsl:when test="@langword='abstract'">
				<xsl:text>abstract (</xsl:text>
				<b>MustInherit</b>
				<xsl:text> in Visual Basic)</xsl:text>
			</xsl:when>
			<xsl:when test="@langword='virtual'">
				<xsl:text>virtual (</xsl:text>
				<b>CanOverride</b>
				<xsl:text> in Visual Basic)</xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<b>
					<xsl:value-of select="@langword" />
				</b>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template match="br" mode="slashdoc" doc:group="inline">
		<br/>
	</xsl:template>
</xsl:transform>
