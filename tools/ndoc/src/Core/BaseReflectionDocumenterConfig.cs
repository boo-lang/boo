// BaseReflectionDocumenterConfig.cs - base XML documenter config class
// Copyright (C) 2004  Kevin Downs
// Parts Copyright (C) 2001  Kral Ferch, Jason Diamond
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms.Design;
using System.Xml;

using NDoc.Core.PropertyGridUI;

namespace NDoc.Core.Reflection
{
	/// <summary>The base config class for documenters which use the <see cref="ReflectionEngine"/> to extract 
	/// documentation from .Net assemblies.</summary>
	/// <remarks>
	/// <para>Generating the documentation consists of two high level steps:
	/// <list type="number">
	/// <item><description>Merging the /doc XML summary with reflected meta-data from the assemblies.</description></item>
	/// <item><description>Transforming that merged XML into the documentation (HTML for the MSDN and VS.NET documenters).</description></item>
	/// </list></para>
	/// <para>The settings below govern how exactly the XML summary data is merged
	/// with the reflected meta-data and therefore govern what items will and will not 
	/// appear in the final documentation.
	/// </para>
	/// </remarks>
	abstract public class BaseReflectionDocumenterConfig : BaseDocumenterConfig
	{
		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		/// <param name="info">Info class descrbing the documenter</param>
		protected BaseReflectionDocumenterConfig( IDocumenterInfo info ) : base( info )
		{
			_ShowMissingSummaries = false;
			_ShowMissingRemarks = false;
			_ShowMissingParams = false;
			_ShowMissingReturns = false;
			_ShowMissingValues = false;

			_DocumentInheritedMembers = true;
			_DocumentInheritedFrameworkMembers = true;
			_DocumentExplicitInterfaceImplementations = false;

			_DocumentInternals = false;
			_DocumentProtected = true;
			_DocumentSealedProtected = false;
			_DocumentPrivates = false;
			_DocumentProtectedInternalAsProtected = false;
			_DocumentEmptyNamespaces = false;
			_EditorBrowsableFilter = EditorBrowsableFilterLevel.Off;

			_AssemblyVersionInfo = AssemblyVersionInformationType.None;
			_CopyrightText = string.Empty;
			_CopyrightHref = string.Empty;

			_SkipNamespacesWithoutSummaries = false;
			_UseNamespaceDocSummaries = false;
			_AutoPropertyBackerSummaries = false;
			_AutoDocumentConstructors = true;

			_DocumentAttributes = false;
			_DocumentInheritedAttributes = true;
			_ShowTypeIdInAttributes = false;
			_DocumentedAttributes = string.Empty;
		}
		
		/// <summary>
		/// Gets or sets a collection of additional paths to search for reference assemblies.
		/// </summary>
		[NonPersisted]
		[Category("(Global)")]
		[Description("A collection of additional paths to search for reference assemblies.\nNote: This is a PROJECT level property that is shared by all documenters...")]
		public ReferencePathCollection ReferencePaths
		{
			get
			{
				return Project._referencePaths;
			}
			set
			{
				Project._referencePaths = value;
				SetDirty();
			}
		}

		#region Show Missing Documentation Options

		private bool _ShowMissingSummaries;

		/// <summary>Gets or sets the ShowMissingSummaries property.</summary>
		/// <remarks>If this is true, all members without /doc <b>&lt;summary&gt;</b>
		/// comments will contain the phrase <font color="red">Missing Documentation</font> in the
		/// generated documentation.</remarks>
		[Category("Show Missing Documentation")]
		[Description("Turning this flag on will show you where you are missing summaries.")]
		[DefaultValue(false)]
		public bool ShowMissingSummaries
		{
			get { return _ShowMissingSummaries; }

			set
			{
				_ShowMissingSummaries = value;
				SetDirty();
			}
		}

		private bool _ShowMissingRemarks;

		/// <summary>Gets or sets the ShowMissingRemarks property.</summary>
		/// <remarks>If this is true, all members without /doc <b>&lt;remarks&gt;</b>
		/// comments will contain the phrase <font color="red">Missing Documentation</font> in the
		/// generated documentation.</remarks>
		[Category("Show Missing Documentation")]
		[Description("Turning this flag on will show you where you are missing Remarks.")]
		[DefaultValue(false)]
		public bool ShowMissingRemarks
		{
			get { return _ShowMissingRemarks; }

			set
			{
				_ShowMissingRemarks = value;
				SetDirty();
			}
		}

		private bool _ShowMissingParams;

		/// <summary>Gets or sets the ShowMissingParams property.</summary>
		/// <remarks>If this is true, all parameters without /doc <b>&lt;param&gt;</b>
		/// comments will contain the phrase <font color="red">Missing Documentation</font> in the
		/// generated documentation.</remarks>
		[Category("Show Missing Documentation")]
		[Description("Turning this flag on will show you where you are missing Params.")]
		[DefaultValue(false)]
		public bool ShowMissingParams
		{
			get { return _ShowMissingParams; }

			set
			{
				_ShowMissingParams = value;
				SetDirty();
			}
		}

		private bool _ShowMissingReturns;

		/// <summary>Gets or sets the ShowMissingReturns property.</summary>
		/// <remarks>If this is true, all members without /doc <b>&lt;returns&gt;</b>
		/// comments will contain the phrase <font color="red">Missing Documentation</font> in the
		/// generated documentation.</remarks>
		[Category("Show Missing Documentation")]
		[Description("Turning this flag on will show you where you are missing Returns.")]
		[DefaultValue(false)]
		public bool ShowMissingReturns
		{
			get { return _ShowMissingReturns; }

			set
			{
				_ShowMissingReturns = value;
				SetDirty();
			}
		}

		private bool _ShowMissingValues;

		/// <summary>Gets or sets the ShowMissingValues property.</summary>
		/// <remarks>If this is true, all properties without /doc <b>&lt;value&gt;</b>
		/// comments will contain the phrase <font color="red">Missing Documentation</font> in the
		/// generated documentation.</remarks>
		[Category("Show Missing Documentation")]
		[Description("Turning this flag on will show you where you are missing Values.")]
		[DefaultValue(false)]
		public bool ShowMissingValues
		{
			get { return _ShowMissingValues; }

			set
			{
				_ShowMissingValues = value;
				SetDirty();
			}
		}

		#endregion

		#region Visibility Options
		
		private bool _DocumentInheritedMembers;

		/// <summary>Gets or sets the DocumentInheritedMembers property.</summary>
		/// <remarks>Determines whether inherited members are documented. 
		/// </remarks>
		[Category("Visibility")]
		[Description("Turn this flag on to document inherited members.")]
		[DefaultValue(true)]
		public bool DocumentInheritedMembers
		{
			get { return _DocumentInheritedMembers; }

			set
			{
				_DocumentInheritedMembers = value;
				SetDirty();
			}
		}

		private bool _DocumentInheritedFrameworkMembers;

		/// <summary>Gets or sets the DocumentInheritedFrameworkMembers property.</summary>
		/// <remarks>If true, members inherited from .Net framework classes will be documented. 
		/// </remarks>
		[Category("Visibility")]
		[Description("Turn this flag on to document members inherited from framework classes.\nNote: DocumentInheritedMembers must be true if any inherited mebers are to be documented.")]
		[DefaultValue(true)]
		public bool DocumentInheritedFrameworkMembers
		{
			get { return _DocumentInheritedFrameworkMembers; }

			set
			{
				_DocumentInheritedFrameworkMembers = value;
				SetDirty();
			}
		}

		private bool _DocumentExplicitInterfaceImplementations;

		/// <summary>Gets or sets the DocumentInternals property.</summary>
		/// <remarks>If this is true, members which explicitly implement interfaces will
		/// be included in the documentation. Normally, these members are not documented.</remarks>
		[Category("Visibility")]
		[Description("Turn this flag on to document explicit interface implementations.")]
		[DefaultValue(false)]
		public bool DocumentExplicitInterfaceImplementations
		{
			get { return _DocumentExplicitInterfaceImplementations; }

			set
			{
				_DocumentExplicitInterfaceImplementations = value;
				SetDirty();
			}
		}

		private bool _DocumentInternals;

		/// <summary>Gets or sets the DocumentInternals property.</summary>
		/// <remarks>If this is true, types and members marked as internal will
		/// be included in the documentation. Normally, internal items are not documented.</remarks>
		[Category("Visibility")]
		[Description("Turn this flag on to document internal code.")]
		[DefaultValue(false)]
		public bool DocumentInternals
		{
			get { return _DocumentInternals; }

			set
			{
				_DocumentInternals = value;
				SetDirty();
			}
		}

		private bool _DocumentProtected;

		/// <summary>Gets or sets the DocumentProtected property.</summary>
		/// <remarks>If this is true, protected members will be included in the
		/// documentation. Since protected members of non-internal types can be
		/// accessed outside of an assembly, this is true by default.</remarks>
		[Category("Visibility")]
		[Description("Turn this flag on to document protected code.")]
		[DefaultValue(true)]
		public bool DocumentProtected
		{
			get { return _DocumentProtected; }

			set
			{
				_DocumentProtected = value;

				// If DocumentProtected is turned off, then we automatically turn off
				// DocumentSealedProtected, too.
				if (!value)
				{
					_DocumentSealedProtected = false;
				}
				SetDirty();
			}
		}

		private bool _DocumentSealedProtected;

		/// <summary>Gets or sets the DocumentSealedProtected property.</summary>
		/// <remarks>Turn this flag on to document protected members of sealed classes. 
		/// <b>DocumentProtected</b> must be turned on, too.</remarks>
		[Category("Visibility")]
		[Description("Turn this flag on to document protected members of sealed classes. DocumentProtected must be turned on, too.")]
		[DefaultValue(false)]
		public bool DocumentSealedProtected
		{
			get { return _DocumentSealedProtected; }

			set
			{
				_DocumentSealedProtected = value;

				// If DocumentSealedProtected is turned on, then we automatically turn on
				// DocumentProtected, too.
				if (value)
				{
					_DocumentProtected = true;
				}
				SetDirty();
			}
		}

		private bool _DocumentPrivates;

		/// <summary>Gets or sets the DocumentPrivates property.</summary>
		/// <remarks>
		/// <para>If this is true, types and members marked as private will
		/// be included in the documentation.</para>
		/// <para>Normally private items are not documented. This is useful
		/// when use NDoc to create documentation intended for internal use.</para></remarks>
		[Category("Visibility")]
		[Description("Turn this flag on to document private code.")]
		[DefaultValue(false)]
		public bool DocumentPrivates
		{
			get { return _DocumentPrivates; }

			set
			{
				_DocumentPrivates = value;
				SetDirty();
			}
		}

		private bool _DocumentProtectedInternalAsProtected;

		/// <summary>Gets or sets the DocumentProtectedInternalAsProtected property.</summary>
		/// <remarks>If this is true, NDoc will treat "protected internal" members as "protected" only.</remarks>
		[Category("Visibility")]
		[Description("If true, NDoc will treat \"protected internal\" members as \"protected\" only.")]
		[DefaultValue(false)]
		public bool DocumentProtectedInternalAsProtected
		{
			get { return _DocumentProtectedInternalAsProtected; }

			set
			{
				_DocumentProtectedInternalAsProtected = value;
				SetDirty();
			}
		}

		private bool _DocumentEmptyNamespaces;

		/// <summary>Gets or sets the DocumentPrivates property.</summary>
		/// <remarks>If this is true, empty namespaces will be included in the documentation.
		/// Normally, empty namespaces are not documented.</remarks>
		[Category("Visibility")]
		[Description("Turn this flag on to document empty namespaces.")]
		[DefaultValue(false)]
		public bool DocumentEmptyNamespaces
		{
			get { return _DocumentEmptyNamespaces; }

			set
			{
				_DocumentEmptyNamespaces = value;
				SetDirty();
			}
		}

		private bool _SkipNamespacesWithoutSummaries;

		/// <summary>Gets or sets the SkipNamespacesWithoutSummaries property.</summary>
		/// <remarks>Setting this property to true , NDoc will not document namespaces 
		/// that don't have an associated namespace summary.</remarks>
		[Category("Visibility")]
		[Description("Setting this property to true will not document namespaces that don't have an associated namespace summary.")]
		[DefaultValue(false)]
		public bool SkipNamespacesWithoutSummaries
		{
			get { return _SkipNamespacesWithoutSummaries; }

			set
			{
				_SkipNamespacesWithoutSummaries = value;
				SetDirty();
			}
		}

		
		private EditorBrowsableFilterLevel _EditorBrowsableFilter;

		/// <summary>Specifies the level of filtering on the EditorBrowsable attribute.</summary>
		/// <remarks><para>Sets the level of filtering to apply on types/members marked with the <b>EditorBrowsable</b> attribute.  
		/// <b>Warning: enabling this filter might result in invalid links in the documentation.</b></para>
		/// <para>As of version 1.3 of NDoc, the <b>&lt;exclude/&gt;</b> tag is the preferred mechanism for
		/// suppressing the documentation of types or members.</para></remarks>
		[Category("Visibility")]
		[Description("Sets the level of filtering to apply on types/members marked with the EditorBrowsable attribute.  Warning: enabling this filter might result in invalid links in the documentation.")]
		[DefaultValue(EditorBrowsableFilterLevel.Off)]
		public EditorBrowsableFilterLevel EditorBrowsableFilter
		{
			get { return _EditorBrowsableFilter; }

			set
			{
				_EditorBrowsableFilter = value;
				SetDirty();
			}
		}

		#endregion

		#region Documentation Main Settings 
		
        private string _UseNDocXmlFile = string.Empty;

        /// <summary>Gets or sets a value indicating whether to use the specified XML file as input instead of reflecting the list of assemblies specified on the project.</summary>
        /// <remarks><para>When set, NDoc will use the specified XML file as 
        /// input instead of reflecting the list of assemblies specified 
        /// on the project.</para>
        /// <para>Very useful for debugging documenters. <b><i>Leave empty for normal usage.</i></b></para>
        /// </remarks>
        [Category("Documentation Main Settings")]
        [Description("When set, NDoc will use the specified XML file as input instead of reflecting the list of assemblies specified on the project.  Very useful for debugging documenters.  Leave empty for normal usage.")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        [DefaultValue("")]
        public string UseNDocXmlFile
        {
            get { return _UseNDocXmlFile; }
            set
            {
                _UseNDocXmlFile = value;
                SetDirty();
            }
        }

        private AssemblyVersionInformationType _AssemblyVersionInfo;

		/// <summary>Gets or sets the AssemblyVersion property.</summary>
		/// <remarks>Determines what type of Assembly Version information is documented. 
		/// </remarks>
		[Category("Documentation Main Settings")]
		[Description("Determines what type of Assembly Version information is documented.")]
		[DefaultValue(AssemblyVersionInformationType.None)]
		[System.ComponentModel.TypeConverter(typeof(EnumDescriptionConverter))]
		public AssemblyVersionInformationType AssemblyVersionInfo
		{
			get { return _AssemblyVersionInfo; }

			set
			{
				_AssemblyVersionInfo = value;
				SetDirty();
			}
		}

		private string _CopyrightText;

		/// <summary>Gets or sets the CopyrightText property.</summary>
		/// <remarks>A textual copyright notice that will be included with each topic.</remarks>
		[Category("Documentation Main Settings")]
		[Description("A copyright notice text that will be included in the generated docs.")]
		[Editor(typeof(TextEditor), typeof(UITypeEditor))]
		[DefaultValue("")]
		public string CopyrightText
		{
			get { return _CopyrightText; }

			set
			{
				_CopyrightText = value;
				SetDirty();
			}
		}

		private string _CopyrightHref;

		/// <summary>Gets or sets the CopyrightHref property.</summary>
		/// <remarks>The URI of a copyright notice. A link to this URI will be included
		/// with each topic.</remarks>
		[Category("Documentation Main Settings")]
		[Description("An URL referenced by the copyright notice.")]
		[DefaultValue("")]
		public string CopyrightHref
		{
			get { return _CopyrightHref; }

			set
			{
				_CopyrightHref = value;
				SetDirty();
			}
		}

		private string _FeedbackEmailAddress = string.Empty;

		/// <summary>Gets or sets the FeedbackEmailAddress property.</summary>
		/// <remarks>If an email address is supplied, a <b>mailto</b> link 
		/// will be placed at the bottom of each page, pointing to this address.</remarks>
		[Category("Documentation Main Settings")]
		[Description("If an email address is supplied, a mailto link will be placed at the bottom of each page using this address.")]
		[DefaultValue("")]
		public string FeedbackEmailAddress
		{
			get { return _FeedbackEmailAddress; }
			set
			{
				_FeedbackEmailAddress = value;
				SetDirty();
			}
		}
		
		private bool _UseNamespaceDocSummaries;

		/// <summary>Gets or sets the UseNamespaceDocSummaries property.</summary>
		/// <remarks>If true, the documenter will look for a class with the name 
		/// <b>NamespaceDoc</b> in each namespace. The summary from that class 
		/// will then be used as the namespace summary.  The class itself will not 
		/// show up in the resulting documentation output. 
		/// <para>You may want to use <b>#if</b> ... <b>#endif</b>
		/// together with conditional compilation constants to 
		/// exclude the <b>NamespaceDoc</b> classes from release build assemblies.</para></remarks>
		[Category("Documentation Main Settings")]
		[Description("If true, the documenter will look for a class with the name "
			 + "\"NamespaceDoc\" in each namespace. The summary from that class "
			 + "will then be used as the namespace summary.  The class itself will not "
			 + "show up in the resulting documentation output. You may want to use "
			 + "#if ... #endif together with conditional compilation constants to "
			 + "exclude the NamespaceDoc classes from release build assemblies.")]
		[DefaultValue(false)]
		public bool UseNamespaceDocSummaries
		{
			get { return _UseNamespaceDocSummaries; }

			set
			{
				_UseNamespaceDocSummaries = value;
				SetDirty();
			}
		}

		private bool _AutoPropertyBackerSummaries;

		/// <summary>Gets or sets the AutoPropertyBackerSummaries property.</summary>
		/// <remarks>If true, the documenter will automatically add a summary 
		/// for fields which look like they back (hold the value for) a 
		/// property. The summary is only added if there is no existing summary, 
		/// which gives you a way to opt out of this behavior in particular cases. 
		/// Currently the naming conventions supported are such that 
		/// fields <b>_Length</b> and <b>length</b> will be inferred to back property <b>Length</b>.</remarks>
		[Category("Documentation Main Settings")]
		[Description("If true, the documenter will automatically add a summary "
			 + "for fields which look like they back (hold the value for) a "
			 + "property. The summary is only added if there is no existing summary, "
			 + "which gives you a way to opt out of this behavior in particular cases. "
			 + "Currently the naming conventions supported are such that "
			 + "fields '_Length' and 'length' will be inferred to back property 'Length'.")]
		[DefaultValue(false)]
		public bool AutoPropertyBackerSummaries
		{
			get { return _AutoPropertyBackerSummaries; }

			set
			{
				_AutoPropertyBackerSummaries = value;
				SetDirty();
			}
		}

		private bool _AutoDocumentConstructors;

		/// <summary>Gets or sets the AutoDocumentConstructors property.</summary>
		/// <remarks>Turning this flag on will enable automatic summary 
		/// documentation for default constructors. If no summary for a parameter-less
		/// constructor is present, the default constructor summary of
		/// <b>Initializes a new instance of the CLASSNAME class</b> is inserted.</remarks>
		[Category("Documentation Main Settings")]
		[Description("Turning this flag on will enable automatic summary documentation for default constructors.")]
		[DefaultValue(true)]
		public bool AutoDocumentConstructors
		{
			get { return _AutoDocumentConstructors; }

			set
			{
				_AutoDocumentConstructors = value;
				SetDirty();
			}
		}

		private bool _Preliminary = false;

		/// <summary>Get/set the Preliminary preoperty</summary>
		/// <remarks>
		/// <para>If true, NDoc will mark every topic as being preliminary documentation.
		/// Each topic will include a notice that the documentation is preliminary</para>
		/// <para>The default notice is <font color="red">[This is preliminary documentation 
		/// and subject to change.]</font></para></remarks>
		[Category("Documentation Main Settings")]
		[Description("If true, NDoc will mark every topic as being preliminary documentation.")]
		[DefaultValue(false)]
		public bool Preliminary
		{
			get { return _Preliminary; }

			set
			{
				_Preliminary = value;
				SetDirty();
			}
		}

		SdkVersion _SdkDocVersion = SdkVersion.SDK_v1_1;

		/// <summary>Gets or sets the LinkToSdkDocVersion property.</summary>
		/// <remarks>Specifies to which version of the .NET Framework SDK documentation the links to system types will be pointing.</remarks>
		[Category("Documentation Main Settings")]
		[Description("Specifies to which version of the .NET Framework SDK documentation the links to system types will be pointing.")]
		[DefaultValue(SdkVersion.SDK_v1_1)]
		[System.ComponentModel.TypeConverter(typeof(EnumDescriptionConverter))]
		public SdkVersion SdkDocVersion
		{
			get { return _SdkDocVersion; }
			set
			{
				_SdkDocVersion = value;
				SetDirty();
			}
		}

		SdkLanguage _SdkDocLanguage = SdkLanguage.en;

		/// <summary>Gets or sets the SdkDocLanguage property.</summary>
		/// <remarks>Specifies to which Language of the .NET Framework SDK documentation the links to system types will be pointing.</remarks>
		[Category("Documentation Main Settings")]
		[Description("Specifies to which Language version of the .NET Framework SDK documentation the links to system types will be pointing.")]
		[DefaultValue(SdkLanguage.en)]
		[System.ComponentModel.TypeConverter(typeof(EnumDescriptionConverter))]
		public SdkLanguage SdkDocLanguage
		{
			get { return _SdkDocLanguage; }
			set
			{
				_SdkDocLanguage = value;
				SetDirty();
			}
		}

		#endregion
		
		#region Show Attributes Options

		private bool _DocumentAttributes;

		/// <summary>Gets or sets whether or not to document the attributes.</summary>
		/// <remarks>Set this to true to output the attributes of the types/members 
		/// in the syntax portion of topics.</remarks>
		[Category("Show Attributes")]
		[Description("Set this to true to output the attributes of the types/members in the syntax portion.")]
		[DefaultValue(false)]
		public bool DocumentAttributes
		{
			get { return _DocumentAttributes; }

			set 
			{ 
				_DocumentAttributes = value;
				SetDirty();
			}
		}

		private bool _DocumentInheritedAttributes;

		/// <summary>Gets or sets whether or not to document the attributes inherited from base types.</summary>
		/// <remarks>Set this to true to output the attributes of the base types/members 
		/// in the syntax portion of topics.</remarks>
		[Category("Show Attributes")]
		[Description("Set this to true to output the attributes of the base types/members in the syntax portion.\nNote: This attribute has no effect unless DocumentAttributes is set to true.")]
		[DefaultValue(true)]
		public bool DocumentInheritedAttributes
		{
			get { return _DocumentInheritedAttributes; }

			set 
			{ 
				_DocumentInheritedAttributes = value;
				SetDirty();
			}
		}

		private bool _ShowTypeIdInAttributes;

		/// <summary>Gets or sets whether or not to show the TypeId property in attributes.</summary>
		/// <remarks>Set this to true to output the <b>TypeId</b> property in the attributes.</remarks>
		[Category("Show Attributes")]
		[Description("Set this to true to output the TypeId property in the attributes.")]
		[DefaultValue(false)]
		public bool ShowTypeIdInAttributes
		{
			get { return _ShowTypeIdInAttributes; }

			set 
			{ 
				_ShowTypeIdInAttributes = value;
				SetDirty();
			}
		}

		private string _DocumentedAttributes;

		/// <summary>Gets or sets which attributes should be documented.</summary>
		/// <remarks><para>When <b>DocumentAttributes</b> is set to true, this specifies 
		/// which attributes/property are visible.  Empty to show all.  </para>
		/// <para>Format: '&lt;attribute-name-starts-with&gt;,&lt;property-to-show&gt;,&lt;property-to-show&gt;|
		/// &lt;attribute-name-starts-with&gt;,&lt;property-to-show&gt;,&lt;property-to-show&gt;|(etc...)'.</para></remarks>
		[Category("Show Attributes")]
		[Description("When DocumentAttributes is set to true, this specifies which attributes/property are visible.  Empty to show all.  Format: '<attribute-name-starts-with>,<property-to-show>,<property-to-show>|<attribute-name-starts-with>,<property-to-show>,<property-to-show>|(etc...)'.")]
		[Editor(typeof(AttributesEditor), typeof(UITypeEditor))]
		[DefaultValue("")]
		public string DocumentedAttributes
		{
			get { return _DocumentedAttributes; }

			set
			{
				_DocumentedAttributes = value;
				SetDirty();
			}
		}

		#endregion

		#region Thread Safety Options

		private bool _IncludeDefaultThreadSafety = true;

		/// <summary>Gets or sets the IncludeDefaultThreadSafety property.</summary>
		/// <remarks>When true, typs that do not have an explicit &lt;threadsafety&gt;
		/// tag will include thread safety documentation corresponding to StaticMembersDefaultToSafe 
		/// and InstanceMembersDefaultToSafe.
		/// </remarks>
		[Category("Thread Safety")]
		[Description("When true, typs that do not have an explicit <threadsafety> tag will include thread safety documentation corresponding to StaticMembersDefaultToSafe and InstanceMembersDefaultToSafe.")]
		[DefaultValue(true)]
		public bool IncludeDefaultThreadSafety 
		{
			get { return _IncludeDefaultThreadSafety; }
			set
			{
				_IncludeDefaultThreadSafety = value;
				SetDirty();
			}
		}

		private bool _StaticMembersDefaultToSafe = true;

		/// <summary>Gets or sets the StaticMembersDefaultToSafe property.</summary>
		/// <remarks>When true, types that do not have an explicit &lt;threadsafety&gt;
		/// tag will default to being safe for accessing static members across threads. 
		/// (ignored if IncludeDefaultThreadSafety is false)</remarks>
		[Category("Thread Safety")]
		[Description("When true, types that do not have an explicit <threadsafety> tag will default to being safe for accessing static members across threads. (ignored if IncludeDefaultThreadSafety is false)")]
		[DefaultValue(true)]
		public bool StaticMembersDefaultToSafe 
		{
			get { return _StaticMembersDefaultToSafe; }
			set
			{
				_StaticMembersDefaultToSafe = value;
				SetDirty();
			}
		}

		private bool _InstanceMembersDefaultToSafe = false;

		/// <summary>Gets or sets the InstanceMembersDefaultToSafe property.</summary>
		/// <remarks>When true, types that do not have an explicit &lt;threadsafety&gt;
		///  tag will default to being safe for accessing instance members across threads. 
		///  (ignored if IncludeDefaultThreadSafety is false)</remarks>
		[Category("Thread Safety")]
		[Description("When true, types that do not have an explicit <threadsafety> tag will default to being safe for accessing instance members across threads. (ignored if IncludeDefaultThreadSafety is false)")]
		[DefaultValue(false)]
		public bool InstanceMembersDefaultToSafe 
		{
			get { return _InstanceMembersDefaultToSafe; }
			set
			{
				_InstanceMembersDefaultToSafe = value;
				SetDirty();
			}
		}
		#endregion

	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected override string HandleUnknownPropertyType(string name, string value)
		{
			string FailureMessages = String.Empty;

			if (String.Compare(name, "ReferencesPath", true) == 0) 
			{
				if (value.Length > 0)
				{
					Trace.WriteLine("WARNING: " + base.DocumenterInfo.Name + " Configuration - property 'ReferencesPath' is OBSOLETE. Please use the project level property 'ReferencePath'\n");
					Project.ReferencePaths.Add(new ReferencePath(value));
				}
			}
			else if (String.Compare(name, "IncludeAssemblyVersion", true) == 0) 
			{
				if (value.Length > 0)
				{
					Trace.WriteLine("WARNING: " + base.DocumenterInfo.Name + " Configuration - property 'IncludeAssemblyVersion' is OBSOLETE. Please use new property 'AssemblyVersionInfo'\n");

					string newValue = String.Empty;
					if (String.Compare(value, "true", true) == 0)
					{
						newValue = "AssemblyVersion";
					}
					else
					{
						newValue = "None";
					}

					FailureMessages += base.ReadProperty("AssemblyVersionInfo", newValue);
				}
			}
			else
			{
				// if we don't know how to handle this, let the base class have a go
				FailureMessages = base.HandleUnknownPropertyType(name, value);
			}
			return FailureMessages;
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected override string HandleUnknownPropertyValue(PropertyInfo property, string value)
		{
			string FailureMessages = String.Empty;

			// DocumentInheritedMembers has changed from an enumerated value to a simple boolean,
			// since static members cannot actually be inherited; The extra complexity was 
			// pointless and caused awkward edge conditions when a static member had the 
			// same name as an instance member....
			if (property.Name == "DocumentInheritedMembers")
			{
				bool newValue = true;
				if (String.Compare(value, "none", true)==0)
				{
					newValue = true;
				}
				FailureMessages += base.ReadProperty("DocumentInheritedMembers", newValue.ToString());
			}
			else
			{
				// if we don't know how to handle this, let the base class have a go
				FailureMessages = base.HandleUnknownPropertyValue(property, value);
			}
			return FailureMessages;
		}
	}

	/// <summary>
	/// Defines the levels of filtering on the EditorBrowsable attribute.
	/// </summary>
	public enum EditorBrowsableFilterLevel
	{
		/// <summary>No filtering.</summary>
		Off, 

		/// <summary>Hide members flagged with EditorBrowsableState.Never.</summary>
		HideNever, 

		/// <summary>Hide members flagged with EditorBrowsableState.Never or EditorBrowsableState.Advanced.</summary>
		HideAdvanced
	}

	/// <summary>
	/// Defines a version of the .NET Framework documentation.
	/// </summary>
	public enum SdkVersion
	{
		/// <summary>The SDK version 1.0.</summary>
		[Description(".Net Version 1.0")]
		SDK_v1_0, 

		/// <summary>The SDK version 1.1.</summary>
		[Description(".Net Version 1.1")]
		SDK_v1_1, 
	}

	/// <summary>
	/// Defines a language version of the .NET Framework documentation.
	/// </summary>
	public enum SdkLanguage
	{
		/// <summary>
		/// English
		/// </summary>
		[Description("English")] en, 
		/// <summary>
		/// French
		/// </summary>
		[Description("French")] fr, 
		/// <summary>
		/// German
		/// </summary>
		[Description("German")] de, 
		/// <summary>
		/// Italian
		/// </summary>
		[Description("Italian")] it, 
		/// <summary>
		/// Japanese
		/// </summary>
		[Description("Japanese")] ja, 
		/// <summary>
		/// Korean
		/// </summary>
		[Description("Korean")] ko, 
		/// <summary>
		/// Spanish
		/// </summary>
		[Description("Spanish")] es 
	}

	/// <summary>
	/// Defines the type of version information to document.
	/// </summary>
	public enum AssemblyVersionInformationType
	{
		/// <summary>
		/// None
		/// </summary>
		[Description("None")] None, 
		/// <summary>
		/// AssemblyVersion Attrribute.
		/// <para>
		/// This is the standard /.Net version information specified in the AssemblyVersionAttribute.
		/// </para>
		/// </summary>
		[Description("Assembly Version")] AssemblyVersion, 
		/// <summary>
		/// AssemblyFileVersion Attribute
		/// <para>
		/// This is the file version specified in the AssemblyFileVersion attribute, as opposed to the /.Net standard Assembly Version.
		/// </para>
		/// <para>This type of version information is useful if an Assembly is to installed in the GAC, and the developer need to avoid side-by-side versioning issues, but wishes to provide build version information...
		/// </para>
		/// </summary>
		[Description("File Version.")] AssemblyFileVersion
	}
}
