<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:MSHelp="http://msdn.microsoft.com/mshelp"
	xmlns:NUtil="urn:ndoc-sourceforge-net:documenters.NativeHtmlHelp2.xsltUtilities" exclude-result-prefixes="NUtil">
	<!-- -->
	<!-- provide no-op override for all non-specified types -->
	<xsl:template match="@* | node() | text()" mode="FIndex" />
	<xsl:template match="@* | node() | text()" mode="KIndex" />
	<xsl:template match="@* | node() | text()" mode="AIndex" />
	<xsl:template match="@* | node() | text()" mode="AIndex-hierarchy" />
	<!-- -->
	<!-- this is just here until each type has it's own title logic -->
	<xsl:template match="* | node() | text()" mode="MSHelpTitle">
		<MSHelp:TOCTitle>
			<xsl:attribute name="Title">
				<xsl:value-of select="@id" />
			</xsl:attribute>
		</MSHelp:TOCTitle>
		<MSHelp:RLTitle>
			<xsl:attribute name="Title">
				<xsl:value-of select="@id" />
			</xsl:attribute>
		</MSHelp:RLTitle>
	</xsl:template>
	<!-- -->
	<xsl:template match="ndoc" mode="MSHelpTitle">
		<xsl:param name="title" />
		<MSHelp:TOCTitle>
			<xsl:attribute name="Title">
				<xsl:value-of select="$title" />
			</xsl:attribute>
		</MSHelp:TOCTitle>
		<MSHelp:RLTitle>
			<xsl:attribute name="Title">
				<xsl:value-of select="concat( $title, ' Namespace' )" />
			</xsl:attribute>
		</MSHelp:RLTitle>
	</xsl:template>
	<!-- -->
	<xsl:template match="enumeration | delegate | constructor" mode="MSHelpTitle">
		<xsl:param name="title" />
		<MSHelp:TOCTitle>
			<xsl:attribute name="Title">
				<xsl:value-of select="$title" />
			</xsl:attribute>
		</MSHelp:TOCTitle>
		<MSHelp:RLTitle>
			<xsl:attribute name="Title">
				<xsl:value-of select="$title" />
			</xsl:attribute>
		</MSHelp:RLTitle>
	</xsl:template>
	<!-- -->
	<xsl:template match="field | property | method | event | operator" mode="MSHelpTitle">
		<xsl:param name="title" />
		<MSHelp:TOCTitle>
			<xsl:attribute name="Title">
				<xsl:value-of select="$title" />
			</xsl:attribute>
		</MSHelp:TOCTitle>
		<MSHelp:RLTitle>
			<xsl:attribute name="Title">
				<xsl:value-of select="concat( parent::node()/@name, '.', $title )" />
			</xsl:attribute>
		</MSHelp:RLTitle>
	</xsl:template>
	<!-- -->
	<xsl:template match="class | interface | structure" mode="MSHelpTitle">
		<xsl:param name="title" />
		<xsl:param name="page-type" />
		<MSHelp:TOCTitle>
			<xsl:attribute name="Title">
				<xsl:choose>
					<xsl:when test="$page-type='type' or $page-type='hierarchy' or $page-type='Members' or $page-type='TypeHierarchy'">
						<xsl:value-of select="$title" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="$page-type" />
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
		</MSHelp:TOCTitle>
		<MSHelp:RLTitle>
			<xsl:attribute name="Title">
				<xsl:value-of select="$title" />
			</xsl:attribute>
		</MSHelp:RLTitle>
	</xsl:template>
	<!-- -->
	<xsl:template match="ndoc" mode="AIndex">
		<xsl:call-template name="add-a-index">
			<xsl:with-param name="filename" select="NUtil:GetNamespaceHRef( string( $namespace ) )" />
		</xsl:call-template>
		<xsl:call-template name="add-index-term">
			<xsl:with-param name="index">A</xsl:with-param>
			<xsl:with-param name="term" select="concat('N:', $namespace)" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="ndoc" mode="AIndex-hierarchy">
		<xsl:call-template name="add-a-index">
			<xsl:with-param name="filename" select="NUtil:GetNamespaceHierarchyHRef( string( $namespace ) )" />
		</xsl:call-template>
		<xsl:call-template name="add-index-term">
			<xsl:with-param name="index">A</xsl:with-param>
			<xsl:with-param name="term" select="concat('Hierarchy.N:', $namespace)" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="enumeration | delegate" mode="AIndex">
		<xsl:call-template name="add-a-index">
			<xsl:with-param name="filename" select="NUtil:GetTypeHRef( string( @id ) )" />
		</xsl:call-template>
		<xsl:call-template name="add-index-term">
			<xsl:with-param name="index">A</xsl:with-param>
			<xsl:with-param name="term" select="string( @id )" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="field | event | property" mode="AIndex">
		<xsl:call-template name="add-a-index">
			<xsl:with-param name="filename" select="NUtil:GetMemberHRef( . )" />
		</xsl:call-template>
		<xsl:call-template name="add-index-term">
			<xsl:with-param name="index">A</xsl:with-param>
			<xsl:with-param name="term" select="string( @id )" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="method | operator" mode="AIndex">
		<xsl:param name="overload-page" />
		<xsl:choose>
			<xsl:when test="$overload-page=true()">
				<!-- need to deal with inherited overloads -->
				<xsl:call-template name="add-a-index">
					<xsl:with-param name="filename" select="NUtil:GetMemberOverloadsHRef( string( ../@id ), string( @name ) )" />
				</xsl:call-template>
				<xsl:call-template name="add-index-term">
					<xsl:with-param name="index">A</xsl:with-param>
					<xsl:with-param name="term" select="concat('Overload:', string( ../@id ), string( @name ))" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="add-a-index">
					<xsl:with-param name="filename" select="NUtil:GetMemberHRef( . )" />
				</xsl:call-template>
				<xsl:call-template name="add-index-term">
					<xsl:with-param name="index">A</xsl:with-param>
					<xsl:with-param name="term" select="string( @id )" />
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template match="constructor" mode="AIndex">
		<xsl:param name="overload-page" />
		<xsl:choose>
			<xsl:when test="$overload-page=true()">
				<!-- need to deal with inherited overloads -->
				<xsl:call-template name="add-a-index">
					<xsl:with-param name="filename" select="NUtil:GetMemberOverloadsHRef( string( ../@id ), 'Constructor' )" />
				</xsl:call-template>
				<xsl:call-template name="add-index-term">
					<xsl:with-param name="index">A</xsl:with-param>
					<xsl:with-param name="term" select="concat('Overload:', string( ../@id ), string( @name ))" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="add-a-index">
					<xsl:with-param name="filename" select="NUtil:GetConstructorHRef( . )" />
				</xsl:call-template>
				<xsl:call-template name="add-index-term">
					<xsl:with-param name="index">A</xsl:with-param>
					<xsl:with-param name="term" select="string( @id )" />
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template match="class | interface | structure" mode="AIndex">
		<xsl:param name="page-type" />
		<xsl:variable name="filename">
			<xsl:choose>
				<xsl:when test="$page-type='Members'">
					<xsl:value-of select="NUtil:GetOverviewHRef( string( @id ), 'Members' )" />
				</xsl:when>
				<xsl:when test="$page-type='Properties'">
					<xsl:value-of select="NUtil:GetOverviewHRef( string( @id ), 'Properties' )" />
				</xsl:when>
				<xsl:when test="$page-type='Events'">
					<xsl:value-of select="NUtil:GetOverviewHRef( string( @id ), 'Events' )" />
				</xsl:when>
				<xsl:when test="$page-type='Operators'">
					<xsl:value-of select="NUtil:GetOverviewHRef( string( @id ), 'Operators' )" />
				</xsl:when>
				<xsl:when test="$page-type='Type Conversions'">
					<xsl:value-of select="NUtil:GetOverviewHRef( string( @id ), 'Operators' )" />
				</xsl:when>
				<xsl:when test="$page-type='Operators and Type Conversions'">
					<xsl:value-of select="NUtil:GetOverviewHRef( string( @id ), 'Operators' )" />
				</xsl:when>
				<xsl:when test="$page-type='Methods'">
					<xsl:value-of select="NUtil:GetOverviewHRef( string( @id ), 'Methods' )" />
				</xsl:when>
				<xsl:when test="$page-type='Fields'">
					<xsl:value-of select="NUtil:GetOverviewHRef( string( @id ), 'Fields' )" />
				</xsl:when>
				<xsl:when test="$page-type='TypeHierarchy'">
					<xsl:value-of select="NUtil:GetTypeHierarchyHRef( string( @id ) )" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="NUtil:GetTypeHRef( string( @id ) )" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="idPrefix">
			<xsl:choose>
				<xsl:when test="$page-type='Members'">
					<xsl:value-of select="'AllMembers.'" />
				</xsl:when>
				<xsl:when test="$page-type='Properties'">
					<xsl:value-of select="'Properties.'" />
				</xsl:when>
				<xsl:when test="$page-type='Events'">
					<xsl:value-of select="'Events.'" />
				</xsl:when>
				<xsl:when test="$page-type='Operators'">
					<xsl:value-of select="'Operators.'" />
				</xsl:when>
				<xsl:when test="$page-type='Type Conversions'">
					<xsl:value-of select="'Operators.'" />
				</xsl:when>
				<xsl:when test="$page-type='Operators and Type Conversions'">
					<xsl:value-of select="'Operators.'" />
				</xsl:when>
				<xsl:when test="$page-type='Methods'">
					<xsl:value-of select="'Methods.'" />
				</xsl:when>
				<xsl:when test="$page-type='Fields'">
					<xsl:value-of select="'Fields.'" />
				</xsl:when>
				<xsl:when test="$page-type='TypeHierarchy'">
					<xsl:value-of select="'DerivedTypeList.'" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="''" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:call-template name="add-a-index">
			<xsl:with-param name="filename" select="$filename" />
		</xsl:call-template>
		<xsl:call-template name="add-index-term">
			<xsl:with-param name="index">A</xsl:with-param>
			<xsl:with-param name="term" select="concat($idPrefix, string( @id ))" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template name="filename-to-aindex">
		<xsl:param name="filename" />
		<!-- there is a bug in this line in that if a type has ".html" in its full name this will fail to produce the correct result -->
		<xsl:value-of select="substring-before( $filename, '.html' )" />
	</xsl:template>
	<!-- -->
	<xsl:template name="add-a-index">
		<xsl:param name="filename" />
		<xsl:variable name="aindex">
			<xsl:call-template name="filename-to-aindex">
				<xsl:with-param name="filename" select="$filename" />
			</xsl:call-template>
		</xsl:variable>
		<xsl:call-template name="add-index-term">
			<xsl:with-param name="index">A</xsl:with-param>
			<xsl:with-param name="term" select="$aindex" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="ndoc" mode="FIndex">
		<xsl:param name="title" />
		<xsl:call-template name="add-index-term">
			<xsl:with-param name="index">F</xsl:with-param>
			<xsl:with-param name="term" select="$title" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="delegate" mode="FIndex">
		<xsl:call-template name="add-index-term">
			<xsl:with-param name="index">F</xsl:with-param>
			<xsl:with-param name="term" select="substring-after( @id, ':' )" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="enumeration" mode="FIndex">
		<xsl:call-template name="add-index-term">
			<xsl:with-param name="index">F</xsl:with-param>
			<xsl:with-param name="term" select="substring-after( @id, ':' )" />
		</xsl:call-template>
		<xsl:apply-templates select="field" mode="FIndex" />
	</xsl:template>
	<!-- -->
	<xsl:template match="class | structure | interface" mode="FIndex">
		<xsl:param name="title" />
		<xsl:param name="page-type" />
		<xsl:if test="$page-type='Members' or $page-type='type'">
			<xsl:call-template name="add-index-term">
				<xsl:with-param name="index">F</xsl:with-param>
				<xsl:with-param name="term" select="substring-after( @id, ':' )" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="enumeration/field" mode="FIndex">
		<xsl:call-template name="add-index-term">
			<xsl:with-param name="index">F</xsl:with-param>
			<xsl:with-param name="term" select="substring-after( @id, ':')" />
		</xsl:call-template>
		<xsl:call-template name="add-index-term">
			<xsl:with-param name="index">F</xsl:with-param>
			<xsl:with-param name="term" select="concat( parent::node()/@name, '.', @name )" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="constructor" mode="FIndex">
		<xsl:param name="overload-page" />
		<xsl:if test="$overload-page=true() or not(@overload)">
			<xsl:call-template name="add-index-term">
				<xsl:with-param name="index">F</xsl:with-param>
				<xsl:with-param name="term" select="concat( substring-after( parent::node()/@id, ':' ), '.', parent::node()/@name )" />
			</xsl:call-template>
			<xsl:call-template name="add-index-term">
				<xsl:with-param name="index">F</xsl:with-param>
				<xsl:with-param name="term" select="concat( parent::node()/@name, '.', parent::node()/@name )" />
			</xsl:call-template>
			<xsl:call-template name="add-index-term">
				<xsl:with-param name="index">F</xsl:with-param>
				<xsl:with-param name="term" select="concat( substring-after( parent::node()/@id, ':' ), '.New' )" />
			</xsl:call-template>
			<xsl:call-template name="add-index-term">
				<xsl:with-param name="index">F</xsl:with-param>
				<xsl:with-param name="term" select="concat( parent::node()/@name, '.New' )" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="field | property | method | event" mode="FIndex">
		<xsl:param name="overload-page" />
		<xsl:if test="$overload-page=true() or not(@overload)">
			<xsl:call-template name="add-index-term">
				<xsl:with-param name="index">F</xsl:with-param>
				<xsl:with-param name="term" select="concat( substring-after( parent::node()/@id, ':' ), '.', @name )" />
			</xsl:call-template>
			<xsl:call-template name="add-index-term">
				<xsl:with-param name="index">F</xsl:with-param>
				<xsl:with-param name="term" select="concat( parent::node()/@name, '.', @name )" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="ndoc" mode="KIndex">
		<xsl:param name="title" />
		<xsl:if test="contains( $title, '.' )">
			<xsl:call-template name="add-index-term">
				<xsl:with-param name="index">K</xsl:with-param>
				<xsl:with-param name="term" select="concat( substring-after( $title, '.' ), ' Namespace' )" />
			</xsl:call-template>
		</xsl:if>
		<xsl:call-template name="add-index-term">
			<xsl:with-param name="index">K</xsl:with-param>
			<xsl:with-param name="term" select="concat( $title, ' Namespace' )" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="class | interface | structure" mode="KIndex">
		<xsl:param name="title" />
		<xsl:param name="page-type" />
		<xsl:choose>
			<xsl:when test="$page-type='Members'">
				<xsl:call-template name="add-index-term">
					<xsl:with-param name="index">K</xsl:with-param>
					<xsl:with-param name="term" select="concat( @name, ' ', local-name() )" />
				</xsl:call-template>
				<xsl:call-template name="add-index-term">
					<xsl:with-param name="index">K</xsl:with-param>
					<xsl:with-param name="term" select="concat( @name, ' ', local-name(), ', all members' )" />
				</xsl:call-template>
				<xsl:call-template name="add-index-term">
					<xsl:with-param name="index">K</xsl:with-param>
					<xsl:with-param name="term" select="concat( substring-after( @id, ':' ), ' ', local-name() )" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$page-type='Properties'">
				<xsl:call-template name="add-index-term">
					<xsl:with-param name="index">K</xsl:with-param>
					<xsl:with-param name="term" select="concat( @name, ' ', local-name(), ', properties' )" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$page-type='Events'">
				<xsl:call-template name="add-index-term">
					<xsl:with-param name="index">K</xsl:with-param>
					<xsl:with-param name="term" select="concat( @name, ' ', local-name(), ', events' )" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$page-type='Operators'">
				<xsl:call-template name="add-index-term">
					<xsl:with-param name="index">K</xsl:with-param>
					<xsl:with-param name="term" select="concat( @name, ' ', local-name(), ', operators' )" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$page-type='Type Conversions'">
				<xsl:call-template name="add-index-term">
					<xsl:with-param name="index">K</xsl:with-param>
					<xsl:with-param name="term" select="concat( @name, ' ', local-name(), ', type conversions' )" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$page-type='Operators and Type Conversions'">
				<xsl:call-template name="add-index-term">
					<xsl:with-param name="index">K</xsl:with-param>
					<xsl:with-param name="term" select="concat( @name, ' ', local-name(), ', operators and type conversions' )" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$page-type='Methods'">
				<xsl:call-template name="add-index-term">
					<xsl:with-param name="index">K</xsl:with-param>
					<xsl:with-param name="term" select="concat( @name, ' ', local-name(), ', methods' )" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$page-type='Fields'">
				<xsl:call-template name="add-index-term">
					<xsl:with-param name="index">K</xsl:with-param>
					<xsl:with-param name="term" select="concat( @name, ' ', local-name(), ', fields' )" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="add-index-term">
					<xsl:with-param name="index">K</xsl:with-param>
					<xsl:with-param name="term" select="concat( @name, ' ', local-name(), ', about ', @name, ' ', local-name() )" />
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- -->
	<xsl:template match="constructor" mode="KIndex">
		<xsl:param name="overload-page" />
		<xsl:if test="$overload-page=true() or not(@overload)">
			<xsl:call-template name="add-index-term">
				<xsl:with-param name="index">K</xsl:with-param>
				<xsl:with-param name="term" select="concat( parent::node()/@name, ' ', local-name( parent::node() ), ', ', local-name() )" />
			</xsl:call-template>
			<xsl:call-template name="add-index-term">
				<xsl:with-param name="index">K</xsl:with-param>
				<xsl:with-param name="term" select="concat( substring-after( parent::node()/@id, ':' ), ' ', local-name() )" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="field | property | method | event" mode="KIndex">
		<xsl:param name="overload-page" />
		<xsl:if test="$overload-page=true() or not(@overload)">
			<xsl:call-template name="add-index-term">
				<xsl:with-param name="index">K</xsl:with-param>
				<xsl:with-param name="term" select="concat( parent::node()/@name, '.', @name, ' ', local-name() )" />
			</xsl:call-template>
			<xsl:call-template name="add-index-term">
				<xsl:with-param name="index">K</xsl:with-param>
				<xsl:with-param name="term" select="concat( @name, ' ', local-name() )" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<!-- -->
	<xsl:template match="enumeration" mode="KIndex">
		<xsl:apply-templates select="field" mode="KIndex" />
		<xsl:call-template name="add-index-term">
			<xsl:with-param name="index">K</xsl:with-param>
			<xsl:with-param name="term" select="concat( substring-after( @id, ':'), ' enumeration' )" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="enumeration/field" mode="KIndex">
		<xsl:call-template name="add-index-term">
			<xsl:with-param name="index">K</xsl:with-param>
			<xsl:with-param name="term" select="concat( @name, ' enumeration member' )" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template match="delegate" mode="KIndex">
		<xsl:call-template name="add-index-term">
			<xsl:with-param name="index">K</xsl:with-param>
			<xsl:with-param name="term" select="concat( @name, ' delegate' )" />
		</xsl:call-template>
		<xsl:call-template name="add-index-term">
			<xsl:with-param name="index">K</xsl:with-param>
			<xsl:with-param name="term" select="concat( substring-after( @id, ':' ), ' delegate' )" />
		</xsl:call-template>
	</xsl:template>
	<!-- -->
	<xsl:template name="add-index-term">
		<xsl:param name="index" />
		<xsl:param name="term" />
		<MSHelp:Keyword>
			<xsl:attribute name="Index">
				<xsl:value-of select="$index" />
			</xsl:attribute>
			<xsl:attribute name="Term">
				<xsl:value-of select="$term" />
			</xsl:attribute>
		</MSHelp:Keyword>
	</xsl:template>
	<!-- -->
</xsl:stylesheet>
