<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:NUtil="urn:NDocUtil"
	exclude-result-prefixes="NUtil">
	<!-- -->
	<xsl:template name="get-link-for-member-overload">
		<xsl:param name="member" />
		<xsl:param name="link-text" />
		<xsl:variable name="href">
			<xsl:choose>
				<xsl:when test="@declaringType and starts-with(@declaringType, 'System.')">
					<xsl:call-template name="get-filename-for-system-method" />
				</xsl:when>
				<xsl:when test="@declaringType">
					<xsl:call-template name="get-filename-for-inherited-method-overloads">
						<xsl:with-param name="declaring-type" select="@declaringType" />
						<xsl:with-param name="method-name" select="@name" />
					</xsl:call-template>
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="get-filename-for-cref-overload">
						<xsl:with-param name="cref" select="@id" />
						<xsl:with-param name="overload" select="@overload" />
					</xsl:call-template>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<a>
			<xsl:attribute name="href">
				<xsl:value-of select="$href" />
			</xsl:attribute>
			<xsl:value-of select="$link-text" />
		</a>
	</xsl:template>
	<!-- -->
	<xsl:template name="get-link-for-type-name">
		<xsl:param name="type-name" />
		<xsl:param name="link-text" />
		<xsl:variable name="basic-type-name">
			<xsl:choose>
				<xsl:when test="contains($type-name, '[')">
					<xsl:value-of select="substring-before($type-name, '[')" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$type-name" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:call-template name="get-link-for-type">
			<xsl:with-param name="type" select="concat( 'T:', $basic-type-name )" />
			<xsl:with-param name="link-text" select="$link-text" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template name="get-link-for-type">
		<xsl:param name="type" />
		<xsl:param name="link-text" />
		<xsl:variable name="filename">
			<xsl:choose>
				<xsl:when test="starts-with(substring-after($type,':'), 'System.')">
					<xsl:call-template name="get-filename-for-system-type">
						<xsl:with-param name="type-name" select="substring-after($type,':')" />
					</xsl:call-template>
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="get-filename-for-type">
						<xsl:with-param name="id" select="$type" />
					</xsl:call-template>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<a>
			<xsl:attribute name="href">
				<xsl:value-of select="$filename" />
			</xsl:attribute>
			<xsl:value-of select="$link-text" />
		</a>
	</xsl:template>
</xsl:stylesheet>
