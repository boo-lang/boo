<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:doc="http://ndoc.sf.net/doc"
	xmlns:NUtil="urn:NDocUtil" exclude-result-prefixes="doc NUtil">
	<!-- NDoc resolves this custom url to a user-supplied extensibility stylesheet -->
	<xsl:include href="user:extensibility" />
	<!--
	 | Identity Template
	 +-->
	<xsl:template match="node()|@*" mode="slashdoc">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()" mode="slashdoc" />
		</xsl:copy>
	</xsl:template>
	<!--
	 | Block Tags
	 +-->
	<doc:template>
		<summary>A normal paragraph. This ends up being a <b>p</b> tag.
		(Did we really need the extra three letters?)</summary>
	</doc:template>
	<xsl:template match="para" mode="slashdoc" doc:group="block" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrfpara.htm">
		<p>
			<xsl:apply-templates select="@*" mode="slashdoc" />
			<xsl:apply-templates select="./node()" mode="slashdoc" />
		</p>
	</xsl:template>
	<doc:template>
		<summary>Use the lang attribute to indicate that the text of the
		paragraph is only appropriate for a specific language.</summary>
	</doc:template>
	<xsl:template match="para[@lang]" mode="slashdoc" doc:group="block">
		<p>
			<xsl:apply-templates select="@*[not(name()='lang')]" mode="slashdoc" />
			<span class="lang">
				<xsl:text>[</xsl:text>
				<xsl:call-template name="get-lang">
					<xsl:with-param name="lang" select="@lang" />
				</xsl:call-template>
				<xsl:text>] </xsl:text>
			</span>
			<xsl:apply-templates select="./node()" mode="slashdoc" />
		</p>
	</xsl:template>
	<doc:template>
		<summary>Multiple lines of code.</summary>
	</doc:template>
	<xsl:template match="code" mode="slashdoc" doc:group="block" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrfcode.htm">
		<pre>
			<xsl:choose>
				<xsl:when test="@class">
					<xsl:attribute name="class">
						<xsl:value-of select="@class" />
					</xsl:attribute>
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="class">code</xsl:attribute>
				</xsl:otherwise>
			</xsl:choose>
			<xsl:apply-templates select="@*[not(name()='class')]" mode="slashdoc" />
			<xsl:apply-templates mode="slashdoc" />
		</pre>
	</xsl:template>
	<doc:template>
		<summary>Use the lang attribute to indicate that the code
		sample is only appropriate for a specific language.</summary>
	</doc:template>
	<xsl:template match="code[@lang]" mode="slashdoc" doc:group="block">
		<pre>
			<xsl:choose>
				<xsl:when test="@class">
					<xsl:attribute name="class">
						<xsl:value-of select="@class" />
					</xsl:attribute>
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="class">code</xsl:attribute>
				</xsl:otherwise>
			</xsl:choose>
			<xsl:apply-templates select="@*[not(name()='class') and not(name()='lang')]" mode="slashdoc" />
			<span class="lang">
				<xsl:text>[</xsl:text>
				<xsl:call-template name="get-lang">
					<xsl:with-param name="lang" select="@lang" />
				</xsl:call-template>
				<xsl:text>]&#10;</xsl:text>
			</span>
			<xsl:apply-templates mode="slashdoc" />
		</pre>
	</xsl:template>
	<doc:template>
		<summary>See <a href="ms-help://MS.NETFrameworkSDKv1.1/cpref/html/frlrfSystemXmlXmlDocumentClassLoadTopic.htm">XmlDocument.Load</a>
		for an example of a note.</summary>
	</doc:template>
	<xsl:template match="note" mode="slashdoc" doc:group="block">
		<blockquote class="dtBlock">
			<xsl:apply-templates select="@*[not(name()='type')]" mode="slashdoc" />
			<xsl:choose>
				<xsl:when test="@type='caution'">
					<b>CAUTION</b>
				</xsl:when>
				<xsl:when test="@type='inheritinfo'">
					<b>Notes to Inheritors: </b>
				</xsl:when>
				<xsl:when test="@type='inotes'">
					<b>Notes to Implementers: </b>
				</xsl:when>
				<xsl:otherwise>
					<b>Note</b>
				</xsl:otherwise>
			</xsl:choose>
			<xsl:text>&#160;&#160;&#160;</xsl:text>
			<xsl:apply-templates mode="slashdoc" />
		</blockquote>
	</xsl:template>
	<xsl:template match="list[@type='bullet']" mode="slashdoc" doc:group="block" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<ul type="disc">
			<xsl:apply-templates select="item" mode="slashdoc" />
		</ul>
	</xsl:template>
	<xsl:template match="list[@type='bullet']/item" mode="slashdoc" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<li>
			<xsl:apply-templates select="./node()" mode="slashdoc" />
		</li>
	</xsl:template>
	<xsl:template match="list[@type='bullet']/item/term" mode="slashdoc" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<xsl:choose>
			<xsl:when test="../description">
				<b><xsl:apply-templates select="./node()" mode="slashdoc" /> - </b>
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates select="./node()" mode="slashdoc" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template match="list[@type='bullet']/item/description" mode="slashdoc" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<xsl:apply-templates select="./node()" mode="slashdoc" />
	</xsl:template>
	<xsl:template match="list[@type='number']" mode="slashdoc" doc:group="block" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<ol>
			<xsl:apply-templates select="item" mode="slashdoc" />
		</ol>
	</xsl:template>
	<xsl:template match="list[@type='number']/item" mode="slashdoc" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<li>
			<xsl:apply-templates select="./node()" mode="slashdoc" />
		</li>
	</xsl:template>
	<xsl:template match="list[@type='number']/item/term" mode="slashdoc" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<xsl:choose>
			<xsl:when test="../description">
				<b><xsl:apply-templates select="./node()" mode="slashdoc" /> - </b>
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates select="./node()" mode="slashdoc" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template match="list[@type='number']/item/description" mode="slashdoc" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<xsl:apply-templates select="./node()" mode="slashdoc" />
	</xsl:template>
	<xsl:template match="list[@type='table']" mode="slashdoc" doc:group="block" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<div class="tablediv">
			<table class="dtTABLE" cellspacing="0">
				<xsl:apply-templates select="listheader" mode="slashdoc" />
				<xsl:apply-templates select="item" mode="slashdoc" />
			</table>
		</div>
	</xsl:template>
	<xsl:template match="list[@type='table']/listheader" mode="slashdoc" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<tr valign="top">
			<xsl:apply-templates mode="slashdoc" />
		</tr>
	</xsl:template>
	<xsl:template match="list[@type='table']/listheader/term" mode="slashdoc" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<th width="50%">
			<xsl:apply-templates select="./node()" mode="slashdoc" />
		</th>
	</xsl:template>
	<xsl:template match="list[@type='table']/listheader/description" mode="slashdoc" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<th width="50%">
			<xsl:apply-templates select="./node()" mode="slashdoc" />
		</th>
	</xsl:template>
	<xsl:template match="list[@type='table']/item" mode="slashdoc" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<tr valign="top">
			<xsl:apply-templates mode="slashdoc" />
		</tr>
	</xsl:template>
	<xsl:template match="list[@type='table']/item/term" mode="slashdoc" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<td>
			<xsl:apply-templates select="./node()" mode="slashdoc" />
		</td>
	</xsl:template>
	<xsl:template match="list[@type='table']/item/description" mode="slashdoc" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<td>
			<xsl:apply-templates select="./node()" mode="slashdoc" />
		</td>
	</xsl:template>
	<xsl:template match="list[@type='definition']" mode="slashdoc" doc:group="block" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<dl>
			<xsl:apply-templates select="item" mode="slashdoc" />
		</dl>
	</xsl:template>
	<xsl:template match="list[@type='definition']/item/term" mode="slashdoc" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<dt>
			<xsl:apply-templates select="./node()" mode="slashdoc" />
		</dt>
	</xsl:template>
	<xsl:template match="list[@type='definition']/item/description" mode="slashdoc" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrflist.htm">
		<dd>
			<xsl:apply-templates select="./node()" mode="slashdoc" />
		</dd>
	</xsl:template>
	<!--
	 | MonoDoc/ECMA Tags
	 +-->
	<xsl:template match="block[@type='note']" mode="slashdoc">
		<p>
			<xsl:apply-templates select="@*[not(name()='type')]" mode="slashdoc" />
			<b>Note: </b>
			<xsl:apply-templates mode="slashdoc" />
		</p>
	</xsl:template>
	<!-- -->
	<xsl:template match="block[@type='behaviors']" mode="slashdoc">
		<p>
			<xsl:apply-templates select="@*[not(name()='type')]" mode="slashdoc" />
			<h4 class=".dtH4">Operation</h4>
			<xsl:apply-templates select="./node()" mode="slashdoc" />
		</p>
	</xsl:template>
	<!-- -->
	<xsl:template match="block[@type='overrides']" mode="slashdoc">
		<p>
			<xsl:apply-templates select="@*[not(name()='type')]" mode="slashdoc" />
			<h4 class=".dtH4">Note to Inheritors</h4>
			<xsl:apply-templates select="./node()" mode="slashdoc" />
		</p>
	</xsl:template>
	<!-- -->
	<xsl:template match="block[@type='usage']" mode="slashdoc">
		<p>
			<xsl:apply-templates select="@*[not(name()='type')]" mode="slashdoc" />
			<h4 class=".dtH4">Usage</h4>
			<xsl:apply-templates select="./node()" mode="slashdoc" />
		</p>
	</xsl:template>
	<!-- -->
	<xsl:template match="block[@type='default']" mode="slashdoc">
		<p>
			<xsl:apply-templates select="@*[not(name()='type')]" mode="slashdoc" />
			<h4 class=".dtH4">Default</h4>
			<xsl:apply-templates select="./node()" mode="slashdoc" />
		</p>
	</xsl:template>
	<!-- -->
	<xsl:template match="block[@type='example']" mode="slashdoc">
		<p>
			<xsl:apply-templates select="@*[not(name()='type')]" mode="slashdoc" />
			<b>For example: </b>
			<xsl:apply-templates mode="slashdoc" />
		</p>
	</xsl:template>
	<!--
	 | Inline Tags
	 +-->
	<xsl:template match="c" mode="slashdoc" doc:group="inline" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrfc.htm">
		<code>
			<xsl:apply-templates mode="slashdoc" />
		</code>
	</xsl:template>
	<xsl:template match="paramref[@name]" mode="slashdoc" doc:group="inline" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrfparamref.htm">
		<i>
			<xsl:value-of select="@name" />
		</i>
	</xsl:template>
	<xsl:template match="see[@cref]" mode="slashdoc" doc:group="inline" doc:msdn="ms-help://MS.NETFrameworkSDKv1.1/csref/html/vclrfsee.htm">
		<xsl:choose>
			<xsl:when test="@nolink='true'">
				<b>
					<xsl:choose>
						<xsl:when test="node()">
							<xsl:value-of select="." />
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="NUtil:GetName(@cref)" />
						</xsl:otherwise>
					</xsl:choose>
				</b>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="get-a-href">
					<xsl:with-param name="cref" select="@cref" />
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template match="see[@href]" mode="slashdoc" doc:group="inline">
		<a href="{@href}">
			<xsl:choose>
				<xsl:when test="node()">
					<xsl:value-of select="." />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="@href" />
				</xsl:otherwise>
			</xsl:choose>
		</a>
	</xsl:template>
	<xsl:template match="see[@langword]" mode="slashdoc" doc:group="inline">
		<xsl:choose>
			<xsl:when test="@langword='null'">
				<xsl:text>a null reference</xsl:text>
				<xsl:if test="$ndoc-vb-syntax">
				  (<b>Nothing</b><xsl:text> in Visual Basic)</xsl:text></xsl:if>
			</xsl:when>
			<xsl:when test="@langword='sealed'">
				<xsl:text>sealed</xsl:text>
				<xsl:if test="$ndoc-vb-syntax">
				  (<b>NotInheritable</b><xsl:text> in Visual Basic)</xsl:text></xsl:if>
			</xsl:when>
			<xsl:when test="@langword='static'">
				<xsl:text>static</xsl:text>
				<xsl:if test="$ndoc-vb-syntax">
				  (<b>Shared</b><xsl:text> in Visual Basic)</xsl:text></xsl:if>
			</xsl:when>
			<xsl:when test="@langword='abstract'">
				<xsl:text>abstract</xsl:text>
				<xsl:if test="$ndoc-vb-syntax">
				  (<b>MustInherit</b><xsl:text> in Visual Basic)</xsl:text></xsl:if>
			</xsl:when>
			<xsl:when test="@langword='virtual'">
				<xsl:text>virtual</xsl:text>
				<xsl:if test="$ndoc-vb-syntax">
				  (<b>CanOverride</b><xsl:text> in Visual Basic)</xsl:text></xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<b>
					<xsl:value-of select="@langword" />
				</b>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template match="br" mode="slashdoc" doc:group="inline">
		<br />
	</xsl:template>
</xsl:stylesheet>