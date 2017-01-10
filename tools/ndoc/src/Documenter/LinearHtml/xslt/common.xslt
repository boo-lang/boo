<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- -->
	<xsl:include href="filenames.xslt" />
	<xsl:include href="syntax.xslt" />
	<xsl:include href="vb-syntax.xslt" />
	<xsl:include href="tags.xslt" />
	<!-- -->
	<xsl:param name="ndoc-title" />
	<xsl:param name="ndoc-omit-object-tags" select="false" />
	<!-- -->
	<xsl:template name="csharp-type">
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
				<xsl:when test="$old-type='System.Byte'">byte</xsl:when>
				<xsl:when test="$old-type='Byte'">byte</xsl:when>
				<xsl:when test="$old-type='System.SByte'">sbyte</xsl:when>
				<xsl:when test="$old-type='SByte'">sbyte</xsl:when>
				<xsl:when test="$old-type='System.Int16'">short</xsl:when>
				<xsl:when test="$old-type='Int16'">short</xsl:when>
				<xsl:when test="$old-type='System.UInt16'">ushort</xsl:when>
				<xsl:when test="$old-type='UInt16'">ushort</xsl:when>
				<xsl:when test="$old-type='System.Int32'">int</xsl:when>
				<xsl:when test="$old-type='Int32'">int</xsl:when>
				<xsl:when test="$old-type='System.UInt32'">uint</xsl:when>
				<xsl:when test="$old-type='UInt32'">uint</xsl:when>
				<xsl:when test="$old-type='System.Int64'">long</xsl:when>
				<xsl:when test="$old-type='Int64'">long</xsl:when>
				<xsl:when test="$old-type='System.UInt64'">ulong</xsl:when>
				<xsl:when test="$old-type='UInt64'">ulong</xsl:when>
				<xsl:when test="$old-type='System.Single'">float</xsl:when>
				<xsl:when test="$old-type='Single'">float</xsl:when>
				<xsl:when test="$old-type='System.Double'">double</xsl:when>
				<xsl:when test="$old-type='Double'">double</xsl:when>
				<xsl:when test="$old-type='System.Decimal'">decimal</xsl:when>
				<xsl:when test="$old-type='Decimal'">decimal</xsl:when>
				<xsl:when test="$old-type='System.String'">string</xsl:when>
				<xsl:when test="$old-type='String'">string</xsl:when>
				<xsl:when test="$old-type='System.Char'">char</xsl:when>
				<xsl:when test="$old-type='Char'">char</xsl:when>
				<xsl:when test="$old-type='System.Boolean'">bool</xsl:when>
				<xsl:when test="$old-type='Boolean'">bool</xsl:when>
				<xsl:when test="$old-type='System.Void'">void</xsl:when>
				<xsl:when test="$old-type='Void'">void</xsl:when>
				<xsl:when test="$old-type='System.Object'">object</xsl:when>
				<xsl:when test="$old-type='Object'">object</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$old-type" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="contains($runtime-type, '[')">
				<xsl:value-of select="concat($new-type, '[', substring-after($runtime-type, '['))" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$new-type" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="type-access">
		<xsl:param name="access" />
		<xsl:param name="type" />
		<xsl:choose>
			<xsl:when test="$access='Public'">public</xsl:when>
			<xsl:when test="$access='NotPublic'">internal</xsl:when>
			<xsl:when test="$access='NestedPublic'">public</xsl:when>
			<xsl:when test="$access='NestedFamily'">protected</xsl:when>
			<xsl:when test="$access='NestedFamilyOrAssembly'">protected internal</xsl:when>
			<xsl:when test="$access='NestedAssembly'">internal</xsl:when>
			<xsl:when test="$access='NestedPrivate'">private</xsl:when>
			<xsl:otherwise>/* unknown */</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="method-access">
		<xsl:param name="access" />
		<xsl:choose>
			<xsl:when test="$access='Public'">public</xsl:when>
			<xsl:when test="$access='Family'">protected</xsl:when>
			<xsl:when test="$access='FamilyOrAssembly'">protected internal</xsl:when>
			<xsl:when test="$access='Assembly'">internal</xsl:when>
			<xsl:when test="$access='Private'">private</xsl:when>
			<xsl:otherwise>/* unknown */</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="contract">
		<xsl:param name="contract" />
		<xsl:choose>
			<xsl:when test="$contract='Static'">static</xsl:when>
			<xsl:when test="$contract='Abstract'">abstract</xsl:when>
			<xsl:when test="$contract='Final'"></xsl:when>
			<xsl:when test="$contract='Virtual'">virtual</xsl:when>
			<xsl:when test="$contract='Override'">override</xsl:when>
			<xsl:when test="$contract='Normal'"></xsl:when>
			<xsl:otherwise>/* unknown */</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="parameter-topic">
		<dl>
		<xsl:for-each select="parameter">
			<xsl:variable name="name" select="@name" />
			<dt>
				<i>
					<xsl:value-of select="@name" />
				</i>
			</dt>
			<dd>
				<xsl:apply-templates select="parent::node()/documentation/param[@name=$name]/node()" mode="slashdoc" />
			</dd>
		</xsl:for-each>
		</dl>
	</xsl:template>
	<!-- -->
	<xsl:template name="type-mixed">
		<xsl:choose>
			<xsl:when test="local-name()='constructor' or local-name()='property' or local-name()='method' or local-name()='event' or local-name()='operator'">
				<xsl:choose>
					<xsl:when test="local-name(..)='interface'">Interface</xsl:when>
					<xsl:otherwise>Class</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<xsl:choose>
					<xsl:when test="local-name()='interface'">Interface</xsl:when>
					<xsl:otherwise>Class</xsl:otherwise>
				</xsl:choose>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="type-element">
		<xsl:choose>
			<xsl:when test="local-name()='constructor' or local-name()='field' or local-name()='property' or local-name()='method' or local-name()='event' or local-name()='operator'">
				<xsl:value-of select="local-name(..)" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="local-name()" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="type-name">
		<xsl:choose>
			<xsl:when test="local-name()='constructor' or local-name()='field' or local-name()='property' or local-name()='method' or local-name()='event' or local-name()='operator'">
				<xsl:value-of select="../@name" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="@name" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="type-id">
		<xsl:choose>
			<xsl:when test="local-name()='constructor' or local-name()='field' or local-name()='property' or local-name()='method' or local-name()='event' or local-name()='operator'">
				<xsl:value-of select="../@id" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="@id" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="namespace-name">
		<xsl:choose>
			<xsl:when test="local-name()='constructor' or local-name()='field' or local-name()='property' or local-name()='method' or local-name()='event' or local-name()='operator'">
				<xsl:value-of select="../../@name" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="../@name" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="seealso-section">
		<xsl:param name="page" />
		<xsl:variable name="typeMixed">
			<xsl:call-template name="type-mixed" />
		</xsl:variable>
		<xsl:variable name="typeElement">
			<xsl:call-template name="type-element" />
		</xsl:variable>
		<xsl:variable name="typeName">
			<xsl:call-template name="type-name" />
		</xsl:variable>
		<xsl:variable name="typeID">
			<xsl:call-template name="type-id" />
		</xsl:variable>
		<xsl:variable name="namespaceName">
			<xsl:call-template name="namespace-name" />
		</xsl:variable>
		<h4 class="dtH4">See Also</h4>
		<p>
			<xsl:if test="$page!='type' and $page!='enumeration' and $page!='delegate'">
				<xsl:variable name="type-filename">
					<xsl:call-template name="get-filename-for-type">
						<xsl:with-param name="id" select="$typeID" />
					</xsl:call-template>
				</xsl:variable>
				<a href="{$type-filename}">
					<xsl:value-of select="concat($typeName, ' ', $typeMixed)" />
				</a>
				<xsl:text> | </xsl:text>
			</xsl:if>
			<xsl:if test="$page!='members' and $page!='enumeration' and $page!='delegate' and $page!='methods' and $page!='properties' and $page!='fields' and $page!='events'">
				<a>
					<xsl:attribute name="href">
						<xsl:call-template name="get-filename-for-type-members">
							<xsl:with-param name="id" select="$typeID" />
						</xsl:call-template>
					</xsl:attribute>
					<xsl:value-of select="$typeName" />
					<xsl:text> Members</xsl:text>
				</a>
				<xsl:text> | </xsl:text>
			</xsl:if>
			<a>
				<xsl:attribute name="href">
					<xsl:call-template name="get-filename-for-namespace">
						<xsl:with-param name="name" select="$namespaceName" />
					</xsl:call-template>
				</xsl:attribute>
				<xsl:value-of select="$namespaceName" />
				<xsl:text> Namespace</xsl:text>
			</a>
			<xsl:if test="$page='member' or $page='property'">
				<xsl:variable name="memberName" select="@name" />
				<xsl:if test="count(parent::node()/*[@name=$memberName]) &gt; 1">
					<xsl:choose>
						<xsl:when test="local-name()='constructor'">
 				      <xsl:text> | </xsl:text>
							<a>
								<xsl:attribute name="href">
									<xsl:call-template name="get-filename-for-current-constructor-overloads" />
								</xsl:attribute>
								<xsl:value-of select="$typeName" />
								<xsl:text> Constructor Overload List</xsl:text>
							</a>
						</xsl:when>
						<xsl:when test="local-name()='operator'">
						  <xsl:if test="@name!='op_Implicit' and @name!='op_Explicit'">
   				      <xsl:text> | </xsl:text>
							  <a>
								  <xsl:attribute name="href">
									  <xsl:call-template name="get-filename-for-current-method-overloads" />
								  </xsl:attribute>
								  <xsl:value-of select="$typeName" />
								  <xsl:text> </xsl:text>
								  <xsl:call-template name="operator-name">
								    <xsl:with-param name="name"><xsl:value-of select="@name" /></xsl:with-param>
								  </xsl:call-template>
								  <xsl:text> Overload List</xsl:text>
							  </a>
						  </xsl:if>
						</xsl:when>
	          <xsl:when test="local-name()='property'">
	 			      <xsl:text> | </xsl:text>
              <a>
                <xsl:attribute name="href">
                  <xsl:call-template name="get-filename-for-current-property-overloads" />
                </xsl:attribute>
                <xsl:value-of select="concat($typeName, '.', @name)" />
                <xsl:text> Overload List</xsl:text>
              </a>
            </xsl:when>
						<xsl:when test="local-name()='method'">
	 			      <xsl:text> | </xsl:text>
   						<a>
								<xsl:attribute name="href">
									<xsl:call-template name="get-filename-for-current-method-overloads" />
								</xsl:attribute>
								<xsl:value-of select="concat($typeName, '.', @name)" />
								<xsl:text> Overload List</xsl:text>
							</a>
						</xsl:when>
					</xsl:choose>
				</xsl:if>
			</xsl:if>
			<xsl:if test="documentation//seealso">
				<xsl:for-each select="documentation//seealso">
					<xsl:text> | </xsl:text>
					<xsl:choose>
						<xsl:when test="@cref">
							<xsl:call-template name="get-a-href">
								<xsl:with-param name="cref" select="@cref"/>
							</xsl:call-template>
						</xsl:when>
						<xsl:when test="@href">
							<a href="{@href}">
								<xsl:value-of select="." />
							</a>
						</xsl:when>
					</xsl:choose>
				</xsl:for-each>
			</xsl:if>
		</p>
	</xsl:template>
	<!-- -->
	<xsl:template name="output-paragraph">
		<xsl:param name="nodes" />
		<xsl:choose>
			<xsl:when test="not($nodes/self::para | $nodes/self::p)">
				<p>
					<xsl:apply-templates select="$nodes" mode="slashdoc" />
				</p>
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates select="$nodes" mode="slashdoc" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template match="para | p" mode="no-para">
		<xsl:apply-templates mode="slashdoc" />
	</xsl:template>
	<!-- -->
	<xsl:template match="note" mode="no-para">
		<blockquote class="dtBlock">
			<b>Note</b>
			<xsl:text>&#160;&#160;&#160;</xsl:text>
			<xsl:apply-templates mode="slashdoc" />
		</blockquote>
	</xsl:template>
	<!-- -->
	<xsl:template match="node()" mode="no-para">
		<xsl:apply-templates select="." mode="slashdoc" />
	</xsl:template>
	<!-- -->
	<xsl:template name="summary-section">
		<xsl:call-template name="output-paragraph">
			<xsl:with-param name="nodes" select="(documentation/summary)[1]/node()" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template name="summary-with-no-paragraph">
		<xsl:param name="member" select="." />
		<xsl:apply-templates select="($member/documentation/summary)[1]/node()" mode="no-para" />
	</xsl:template>
	<!-- -->
	<xsl:template name="overloads-summary-section">
		<xsl:variable name="memberName" select="@name" />
		<xsl:choose>
			<xsl:when test="parent::node()/*[@name=$memberName]/documentation/overloads/summary">
				<xsl:call-template name="output-paragraph">
					<xsl:with-param name="nodes" select="(parent::node()/*[@name=$memberName]/documentation/overloads/summary)[1]/node()" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="parent::node()/*[@name=$memberName]/documentation/overloads">
				<xsl:call-template name="output-paragraph">
					<xsl:with-param name="nodes" select="(parent::node()/*[@name=$memberName]/documentation/overloads)[1]/node()" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="summary-section" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="overloads-summary-with-no-paragraph">
		<xsl:param name="overloads" select="." />
		<xsl:variable name="memberName" select="@name" />
		<xsl:choose>
			<xsl:when test="$overloads/../*[@name=$memberName]/documentation/overloads/summary">
				<xsl:apply-templates select="($overloads/../*[@name=$memberName]/documentation/overloads/summary)[1]/node()" mode="no-para" />
			</xsl:when>
			<xsl:when test="$overloads/../*[@name=$memberName]/documentation/overloads">
				<xsl:apply-templates select="($overloads/../*[@name=$memberName]/documentation/overloads)[1]/node()" mode="no-para" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="summary-with-no-paragraph">
					<xsl:with-param name="member" select="$overloads" />
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="overloads-remarks-section">
		<xsl:if test="documentation/overloads/remarks">
			<h4 class="dtH4">Remarks</h4>
			<p>
				<xsl:apply-templates select="(documentation/overloads/remarks)[1]/node()" mode="slashdoc" />
			</p>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="overloads-example-section">
		<xsl:if test="documentation/overloads/example">
			<h4 class="dtH4">Example</h4>
			<p>
				<xsl:apply-templates select="(documentation/overloads/example)[1]/node()" mode="slashdoc" />
			</p>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="parameter-section">
		<xsl:if test="documentation/param">
			<h4 class="dtH4">Parameters</h4>
			<xsl:call-template name="parameter-topic" />
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="returnvalue-section">
		<xsl:if test="documentation/returns">
			<h4 class="dtH4">Return Value</h4>
			<p>
				<xsl:apply-templates select="documentation/returns/node()" mode="slashdoc" />
			</p>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="implements-section">
		<xsl:if test="implements">
			<xsl:variable name="member" select="local-name()" />
			<h4 class="dtH4">Implements</h4>
			<xsl:for-each select="implements">
				<xsl:variable name="declaring-type-id" select="@interfaceId" />
				<xsl:variable name="name" select="@name" />
				<xsl:variable name="declaring-interface" select="//interface[@id=$declaring-type-id]" />
				<p>
					<a>
						<xsl:attribute name="href">
							<xsl:choose>
								<xsl:when test="$member='property'">
									<xsl:choose>
										<xsl:when test="starts-with(@declaringType, 'System.')" >
											<xsl:call-template name="get-filename-for-system-property" />
										</xsl:when>
										<xsl:otherwise>
											<xsl:call-template name="get-filename-for-property">
												<xsl:with-param name="property" select="$declaring-interface/property[@name=$name]" />
											</xsl:call-template>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:when>
								<xsl:when test="$member='event'">
									<xsl:choose>
										<xsl:when test="starts-with(@declaringType, 'System.')" >
											<xsl:call-template name="get-filename-for-system-event" />
										</xsl:when>
										<xsl:otherwise>
											<xsl:call-template name="get-filename-for-event">
												<xsl:with-param name="event" select="$declaring-interface/event[@name=$name]" />
											</xsl:call-template>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:when>
								<xsl:otherwise>
									<xsl:choose>
										<xsl:when test="starts-with(@declaringType, 'System.')" >
											<xsl:call-template name="get-filename-for-system-method" />
										</xsl:when>
										<xsl:otherwise>
											<xsl:call-template name="get-filename-for-method">
												<xsl:with-param name="method" select="$declaring-interface/method[@name=$name]" />
											</xsl:call-template>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>
						<xsl:value-of select="@interface" /><xsl:text>.</xsl:text><xsl:value-of select="@name" />
					</a>
				</p>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="remarks-section">
		<xsl:if test="documentation/remarks">
			<h4 class="dtH4">Remarks</h4>
			<xsl:variable name="first-element" select="local-name(documentation/remarks/*[1])" />
			<xsl:choose>
				<xsl:when test="$first-element!='para' and $first-element!='p'">
					<p>
						<xsl:apply-templates select="documentation/remarks/node()" mode="slashdoc" />
					</p>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates select="documentation/remarks/node()" mode="slashdoc" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="value-section">
		<xsl:if test="documentation/value">
			<h4 class="dtH4">Property Value</h4>
			<p>
				<xsl:apply-templates select="documentation/value/node()" mode="slashdoc" />
			</p>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="events-section">
		<xsl:if test="documentation/event">
			<h4 class="dtH4">Events</h4>
			<div class="tablediv">
				<table class="dtTABLE" cellspacing="0">
					<tr valign="top">
						<th width="50%">Event Type</th>
						<th width="50%">Reason</th>
					</tr>
					<xsl:for-each select="documentation/event">
						<xsl:sort select="@name" />
						<tr valign="top">
							<td width="50%">
								<xsl:variable name="type-filename">
									<xsl:call-template name="get-filename-for-cref">
										<xsl:with-param name="cref" select="@cref" />
									</xsl:call-template>
								</xsl:variable>
								<a href="{$type-filename}">
									<xsl:call-template name="strip-namespace">
										<xsl:with-param name="name" select="substring-after(@cref, 'F:')" />
									</xsl:call-template>
								</a>
							</td>
							<td width="50%">
								<xsl:apply-templates select="./node()" mode="slashdoc" />
							</td>
						</tr>
					</xsl:for-each>
				</table>
			</div>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="exceptions-section">
		<xsl:if test="documentation/exception">
			<h4 class="dtH4">Exceptions</h4>
			<div class="tablediv">
				<table class="dtTABLE" cellspacing="0">
					<tr valign="top">
						<th width="50%">Exception Type</th>
						<th width="50%">Condition</th>
					</tr>
					<xsl:for-each select="documentation/exception">
						<xsl:sort select="@name" />
						<tr valign="top">
							<td width="50%">
								<xsl:variable name="type-filename">
									<xsl:call-template name="get-filename-for-cref">
										<xsl:with-param name="cref" select="@cref" />
									</xsl:call-template>
								</xsl:variable>
								<a href="{$type-filename}">
									<xsl:call-template name="strip-namespace">
										<xsl:with-param name="name" select="substring-after(@cref, 'T:')"/>
									</xsl:call-template>
								</a>
							</td>
							<td width="50%">
								<xsl:apply-templates select="./node()" mode="slashdoc" />
							</td>
						</tr>
					</xsl:for-each>
				</table>
			</div>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="example-section">
		<xsl:if test="documentation/example">
			<h4 class="dtH4">Example</h4>
			<p>
				<xsl:apply-templates select="documentation/example/node()" mode="slashdoc" />
			</p>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="members-section">
		<xsl:if test="field">
			<h4 class="dtH4">Members</h4>
			<div class="tablediv">
				<table class="dtTABLE" cellspacing="0">
					<tr valign="top">
						<th width="50%">Member Name</th>
						<th width="50%">Description</th>
					</tr>
					<xsl:for-each select="field">
						<xsl:text>&#10;</xsl:text>
						<tr valign="top">
							<td width="50%">
								<b>
									<xsl:value-of select="@name" />
								</b>
							</td>
							<td width="50%">
								<xsl:apply-templates select="documentation/summary/node()" mode="slashdoc" />
							</td>
						</tr>
					</xsl:for-each>
				</table>
			</div>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="get-lang">
		<xsl:param name="lang" />
		<xsl:choose>
			<xsl:when test="$lang = 'VB' or $lang='Visual Basic'">
				<xsl:value-of select="'Visual&#160;Basic'" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$lang" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- get-a-href -->
	<xsl:template name="get-a-href">
		<xsl:param name="cref" />
		<xsl:variable name="href" />
		<xsl:choose>
			<xsl:when test="$href=''">
				<b>Dummy</b>
			</xsl:when>
			<xsl:otherwise>
				<a>
					<xsl:attribute name="href">
						<xsl:value-of select="$href" />
					</xsl:attribute>
					<xsl:choose>
						<xsl:when test="node()">
							<xsl:value-of select="." />
						</xsl:when>
						<xsl:otherwise>
							Dummy1
						</xsl:otherwise>
					</xsl:choose>
				</a>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="value">
		<xsl:param name="type" />
		<xsl:variable name="namespace">
			<xsl:value-of select="concat(../../@name, '.')" />
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="contains($type, $namespace)">
				<xsl:value-of select="substring-after($type, $namespace)" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="csharp-type">
					<xsl:with-param name="runtime-type" select="$type" />
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template name="html-head">
		<xsl:param name="title" />
		<head>
			<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
			<title>
				<xsl:value-of select="$title" />
			</title>
			<xml></xml>
			<link rel="stylesheet" type="text/css" href="MSDN.css" />
		</head>
	</xsl:template>
	<!-- -->
	<xsl:template name="title-row">
		<xsl:param name="type-name" />
		<div id="nsbanner">
			<xsl:variable name="headerHtml" />
			<xsl:choose>
				<xsl:when test="$headerHtml=''">
					<div id="bannerrow1">
						<table class="bannerparthead" cellspacing="0"><tr id="hdr">
							<td class="runninghead">
							<xsl:value-of select="$ndoc-title" />
							</td>
							<td class="product">
							</td>
						</tr></table>
					</div>
					<div id="TitleRow">
						<h1 class="dtH1">
							<xsl:value-of select="$type-name" />
						</h1>
					</div>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$headerHtml" disable-output-escaping="yes" />
				</xsl:otherwise>
			</xsl:choose>
		</div>
	</xsl:template>
	<!-- -->
	<xsl:template name="footer-row">
		<xsl:param name="type-name" />
		<xsl:variable name="assembly-name">
			<xsl:value-of select="ancestor-or-self::assembly/./@name" />
		</xsl:variable>
		<xsl:variable name="assembly-version">
			<xsl:value-of select="ancestor-or-self::assembly/./@version" />
		</xsl:variable>
		<xsl:variable name="footerHtml" />
		<xsl:variable name="copyright-rtf">
			<xsl:call-template name="copyright-notice" />
		</xsl:variable>
		<xsl:variable name="version-rtf">
			<xsl:call-template name="generated-from-assembly-version" />
		</xsl:variable>
		<xsl:if test="string($copyright-rtf) or string($version-rtf) or string($footerHtml) or /ndoc/feedbackEmail">
			<hr />
			<div id="footer">
				<xsl:apply-templates select="/ndoc/feedbackEmail">
					<xsl:with-param name="page" select="$type-name"/>
				</xsl:apply-templates>
				<xsl:choose>
					<xsl:when test="$footerHtml=''">
						<p>
							<xsl:copy-of select="$copyright-rtf" />
						</p>
						<p>
							<xsl:copy-of select="$version-rtf" />
						</p>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="$footerHtml" disable-output-escaping="yes" />
					</xsl:otherwise>
				</xsl:choose>
			</div>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="copyright-notice">
		<xsl:variable name="copyright-text">
			<xsl:value-of select="/ndoc/copyright/@text" />
		</xsl:variable>
		<xsl:variable name="copyright-href">
			<xsl:value-of select="/ndoc/copyright/@href" />
		</xsl:variable>
		<xsl:if test="$copyright-text != ''">
			<a>
				<xsl:if test="$copyright-href != ''">
					<xsl:attribute name="href">
						<xsl:value-of select="$copyright-href" />
					</xsl:attribute>
				</xsl:if>
				<xsl:value-of select="/ndoc/copyright/@text" />
			</a>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="generated-from-assembly-version">
		<xsl:variable name="assembly-name">
			<xsl:value-of select="ancestor-or-self::assembly/./@name" />
		</xsl:variable>
		<xsl:variable name="assembly-version">
			<xsl:value-of select="ancestor-or-self::assembly/./@version" />
		</xsl:variable>
		<xsl:if test="$assembly-version != ''">
			<xsl:text>Generated from assembly </xsl:text>
			<xsl:value-of select="$assembly-name" />
			<xsl:text> [</xsl:text>
			<xsl:value-of select="$assembly-version" />
			<xsl:text>]</xsl:text>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template name="operator-name">
		<xsl:param name="name" />
		<xsl:param name="from" />
		<xsl:param name="to" />
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
			<xsl:when test="$name='op_Implicit'">
				<xsl:text>Implicit </xsl:text>
				<xsl:value-of select="$from" />
				<xsl:text> to </xsl:text>
				<xsl:value-of select="$to" />
				<xsl:text> Conversion</xsl:text>
			</xsl:when>
			<xsl:when test="$name='op_Explicit'">
				<xsl:text>Explicit </xsl:text>
				<xsl:value-of select="$from" />
				<xsl:text> to </xsl:text>
				<xsl:value-of select="$to" />
				<xsl:text> Conversion</xsl:text>
			</xsl:when>
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
	<xsl:template name="requirements-section">
		<xsl:if test="documentation/permission">
			<h4 class="dtH4">Requirements</h4>
			<p>
				<b>.NET Framework Security: </b>
				<ul class="permissions">
					<xsl:for-each select="documentation/permission">
						<li>
							<a>
								<xsl:attribute name="href">
									<xsl:call-template name="get-filename-for-type-name">
										<xsl:with-param name="type-name" select="substring-after(@cref, 'T:')" />
									</xsl:call-template>
								</xsl:attribute>
								<xsl:value-of select="substring-after(@cref, 'T:')" />
							</a>
							<xsl:text>&#160;</xsl:text>
							<xsl:apply-templates mode="slashdoc" />
						</li>
					</xsl:for-each>
				</ul>
			</p>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="feedbackEmail">
		<xsl:param name="page"/>
		<p>
			<a>
				<xsl:attribute name="href">
					<xsl:text>mailto:</xsl:text>
					<xsl:value-of select="."/>
					<xsl:text>?subject=</xsl:text>
					<xsl:value-of select="$ndoc-title" />
					<xsl:text> Documentation Feedback: </xsl:text>
					<xsl:value-of select="$page"/>
				</xsl:attribute>
				<xsl:text>Send comments on this topic.</xsl:text>
			</a>
		</p>
	</xsl:template>	
</xsl:stylesheet>
