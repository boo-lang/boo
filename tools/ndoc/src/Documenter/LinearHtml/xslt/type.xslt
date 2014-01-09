<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- -->
	<xsl:output method="html" indent="no" />
	<!-- -->
	<xsl:include href="common.xslt" />
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
		<!-- this is commented out because XslTransform is throwing an InvalidCastException in it. -->
		<xsl:if test="count($list) &gt; 0">
			<!-- last() is causing an InvalidCastException in Beta 2. -->
			<xsl:variable name="last" select="count($list)" />
			<xsl:call-template name="indent">
				<xsl:with-param name="count" select="$level" />
			</xsl:call-template>
			
			<xsl:choose>
				<xsl:when test="starts-with($list[$last]/@type, 'System.')">
					<a>
						<xsl:attribute name="href">
							<xsl:call-template name="get-filename-for-system-type">
								<xsl:with-param name="type-name" select="$list[$last]/@type" />
							</xsl:call-template>
						</xsl:attribute>
						<xsl:call-template name="get-datatype">
							<xsl:with-param name="datatype" select="$list[$last]/@type" />
						</xsl:call-template>
					</a>
				</xsl:when>
				<xsl:otherwise>
					<xsl:variable name="base-class-id" select="string($list[$last]/@id)" />
					<xsl:variable name="base-class" select="//class[@id=$base-class-id]" />
					<xsl:choose>
						<xsl:when test="$base-class">
							<a>
								<xsl:attribute name="href">
									<xsl:call-template name="get-filename-for-type">
										<xsl:with-param name="id" select="$list[$last]/@id" />
									</xsl:call-template>
								</xsl:attribute>
								<xsl:call-template name="get-datatype">
									<xsl:with-param name="datatype" select="$list[$last]/@type" />
								</xsl:call-template>
							</a>
						</xsl:when>
						<xsl:otherwise>
							<xsl:call-template name="get-datatype">
								<xsl:with-param name="datatype" select="$list[$last]/@type" />
							</xsl:call-template>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:otherwise>
			</xsl:choose>
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
	<xsl:template match="delegate">
		<xsl:call-template name="type">
			<xsl:with-param name="type">Delegate</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="enumeration">
		<xsl:call-template name="type">
			<xsl:with-param name="type">Enumeration</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template name="type">
		<xsl:param name="type" />
		<html dir="LTR">
			<xsl:call-template name="html-head">
				<xsl:with-param name="title" select="concat(@name, ' ', $type)" />
			</xsl:call-template>
			<body id="bodyID" class="dtBODY">
				<xsl:call-template name="title-row">
					<xsl:with-param name="type-name" select="concat(@name, ' ', $type)" />
				</xsl:call-template>
				<div id="nstext">
					<xsl:call-template name="summary-section" />
					<xsl:if test="local-name()!='delegate' and local-name()!='enumeration'">
						<xsl:variable name="members-href">
							<xsl:call-template name="get-filename-for-type-members">
								<xsl:with-param name="id" select="@id" />
							</xsl:call-template>
						</xsl:variable>
						<xsl:if test="constructor|field|property|method|operator|event">
							<p>For a list of all members of this type, see <a href="{$members-href}"><xsl:value-of select="@name" /> Members</a>.</p>
						</xsl:if>
					</xsl:if>
					<xsl:if test="local-name() != 'delegate' and local-name() != 'enumeration'">
						<p>
							<xsl:choose>
								<xsl:when test="self::interface">
									<xsl:if test="base">
										<xsl:call-template name="draw-hierarchy">
											<xsl:with-param name="list" select="descendant::base" />
											<xsl:with-param name="level" select="0" />
										</xsl:call-template>
										<xsl:call-template name="indent">
											<xsl:with-param name="count" select="count(descendant::base)" />
										</xsl:call-template>
										<b>
											<xsl:value-of select="@name" />
										</b>
									</xsl:if>
								</xsl:when>
								<xsl:otherwise>
									<xsl:variable name="href">
										<xsl:call-template name="get-filename-for-system-type">
											<xsl:with-param name="type-name" select="'System.Object'" />
										</xsl:call-template>
									</xsl:variable>
									<a href="{$href}">System.Object</a>
									<br />
									<xsl:call-template name="draw-hierarchy">
										<xsl:with-param name="list" select="descendant::base" />
										<xsl:with-param name="level" select="1" />
									</xsl:call-template>
									<xsl:call-template name="indent">
										<xsl:with-param name="count" select="count(descendant::base) + 1" />
									</xsl:call-template>
									<b>
										<xsl:value-of select="@name" />
									</b>
								</xsl:otherwise>
							</xsl:choose>
						</p>
					</xsl:if>
					<xsl:call-template name="vb-type-syntax" />
					<xsl:call-template name="cs-type-syntax" />
					<xsl:if test="local-name() = 'delegate'">
						<xsl:call-template name="parameter-section" />
						<xsl:call-template name="returnvalue-section" />
					</xsl:if>
					<xsl:call-template name="remarks-section" />
					<xsl:call-template name="example-section" />
					<xsl:if test="local-name() = 'enumeration'">
						<xsl:call-template name="members-section" />
					</xsl:if>
					<h4 class="dtH4">Requirements</h4>
					<p>
						<b>Namespace: </b>
						<a>
							<xsl:attribute name="href">
								<xsl:call-template name="get-filename-for-namespace">
									<xsl:with-param name="name" select="../@name" />
								</xsl:call-template>
							</xsl:attribute>
							<xsl:value-of select="../@name" />
						</a>
					</p>
					<p>
						<b>Assembly: </b>
						<xsl:value-of select="../../../@name" /> (in <xsl:value-of select="../../@name" />)
					</p>
					<xsl:if test="documentation/permission">
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
					<xsl:variable name="page">
						<xsl:choose>
							<xsl:when test="local-name() = 'enumeration'">enumeration</xsl:when>
							<xsl:when test="local-name() = 'delegate'">delegate</xsl:when>
							<xsl:otherwise>type</xsl:otherwise>
						</xsl:choose>
					</xsl:variable>
					<xsl:call-template name="seealso-section">
						<xsl:with-param name="page" select="$page" />
					</xsl:call-template>
					
					<xsl:if test="local-name() = 'class'">
						<xsl:if test="not($ndoc-omit-object-tags)">
							<object type="application/x-oleobject" classid="clsid:1e2a7bd0-dab9-11d0-b93a-00c04fc99f9e" viewastext="true" style="display: none;">
								<xsl:element name="param">
									<xsl:attribute name="name">Keyword</xsl:attribute>
									<xsl:attribute name="value"><xsl:value-of select='@name' /> class, about <xsl:value-of select='@name' /> class</xsl:attribute>
								</xsl:element>
							</object>
						</xsl:if>
					</xsl:if>
					
					<xsl:if test="local-name() = 'interface'">
						<xsl:if test="not($ndoc-omit-object-tags)">
							<object type="application/x-oleobject" classid="clsid:1e2a7bd0-dab9-11d0-b93a-00c04fc99f9e" viewastext="true" style="display: none;">
								<xsl:element name="param">
									<xsl:attribute name="name">Keyword</xsl:attribute>
									<xsl:attribute name="value"><xsl:value-of select='@name' /> interface</xsl:attribute>
								</xsl:element>
							</object>
						</xsl:if>
					</xsl:if>

					<xsl:if test="local-name() = 'enumeration'">
						<xsl:if test="not($ndoc-omit-object-tags)">
							<object type="application/x-oleobject" classid="clsid:1e2a7bd0-dab9-11d0-b93a-00c04fc99f9e" viewastext="true" style="display: none;">
								<xsl:element name="param">
									<xsl:attribute name="name">Keyword</xsl:attribute>
									<xsl:attribute name="value"><xsl:value-of select='@name' /> enumeration</xsl:attribute>
								</xsl:element>
								<xsl:for-each select="field">
									<xsl:element name="param">
										<xsl:attribute name="name">Keyword</xsl:attribute>
										<xsl:attribute name="value"><xsl:value-of select='@name' /> enumeration member</xsl:attribute>
									</xsl:element>
								</xsl:for-each>
							</object>
						</xsl:if>
					</xsl:if>
					<xsl:call-template name="footer-row">
						<xsl:with-param name="type-name" select="concat(@name, ' ', $type)" />
					</xsl:call-template>
				</div>
			</body>
		</html>
	</xsl:template>
	<!-- -->
</xsl:stylesheet>
