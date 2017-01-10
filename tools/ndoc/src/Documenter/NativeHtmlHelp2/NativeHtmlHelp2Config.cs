// MsdnDocumenter.cs - a MSDN-like documenter
// Copyright (C) 2003 Don Kackman
// Parts copyright 2001  Kral Ferch, Jason Diamond
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Diagnostics;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;

using NDoc.Core;
using NDoc.Core.Reflection;
using NDoc.Core.PropertyGridUI;

namespace NDoc.Documenter.NativeHtmlHelp2
{

	/// <summary>
	/// Specifies how the collection will be integrated with the help browser
	/// </summary>
	public enum TOCStyle
	{
		/// <summary>
		/// Each root topic in the TOC is appears at the plug in point
		/// </summary>
		Flat, 

		/// <summary>
		/// Creates a root node in the browser at the plug in point
		/// </summary>
		Hierarchical
	}

	/// <summary>
	/// Config settings for the native Html Help 2 Documenter
	/// </summary>
	/// <remarks>
	/// <para></para>
	/// </remarks>
	[DefaultProperty("OutputDirectory")]
	public class NativeHtmlHelp2Config : BaseReflectionDocumenterConfig
	{
		private const string HTMLHELP2_CONFIG_CATEGORY = "Html Help 2 Settings";
		private const string DEPLOYMENT_CATEGORY = "Html Help 2 Deployment";
		private const string ADDITIONAL_CONTENT_CATEGORY = "Html Help 2 Additional Content";

		/// <summary>Initializes a new instance of the NativeHtmlHelp2Config class.</summary>
		public NativeHtmlHelp2Config( NativeHtmlHelp2DocumenterInfo info ) : base( info )
		{
		}
		
		/// <summary>
		/// Creates an instance of a documenter <see cref="IDocumenterConfig.CreateDocumenter"/>
		/// </summary>
		/// <returns>IDocumenter instance</returns>		
		public override IDocumenter CreateDocumenter()
		{
			return new NativeHtmlHelp2Documenter( this );
		}

		#region Main Settings properties
		string _outputDirectory = string.Format( ".{0}doc{0}", Path.DirectorySeparatorChar );
		
		/// <summary>Gets or sets the OutputDirectory property.</summary>
		/// <remarks>The folder where the root of the HTML set will be located.
		/// This can be absolute or relative from the .ndoc project file.</remarks>
		[Category("Documentation Main Settings")]
		[Description("The directory in which .html files and the .Hx* files will be generated.\nThis can be absolute or relative from the .ndoc project file.")]
		[Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
		public string OutputDirectory
		{
			get { return _outputDirectory; }

			set
			{
				if ( value.IndexOfAny(new char[]{'#','?', ';'}) != -1) 
				{
					throw new FormatException("Output Directory '" + value + 
						"' is not valid because it contains '#','?' or ';' which" +
						" are reserved characters in HTML URLs."); 
				}

				_outputDirectory = value;

				if (!_outputDirectory.EndsWith( Path.DirectorySeparatorChar.ToString() ))
				{
					_outputDirectory += Path.DirectorySeparatorChar;
				}

				SetDirty();
			}
		}
		void ResetOutputDirectory() { _outputDirectory = string.Format( ".{0}doc{0}", Path.DirectorySeparatorChar ); }


		string _htmlHelpName = "Documentation";

		/// <summary>Gets or sets the HtmlHelpName property.</summary>
		/// <remarks>The HTML Help project file and the compiled HTML Help file
		/// use this property plus the appropriate extension as names.</remarks>
		[Category("Documentation Main Settings")]
		[Description("The name of the HTML Help project and the Compiled HTML Help file.")]
		public string HtmlHelpName
		{
			get { return _htmlHelpName; }

			set 
			{ 
				if (Path.GetExtension(value).ToLower() == ".hxs") 
				{
					HtmlHelpName = Path.GetFileNameWithoutExtension(value);
				}
				else
				{
					_htmlHelpName = value;
				}

				SetDirty();
			}
		}

		private string _Title = "An NDoc documented library";

		/// <summary>Gets or sets the Title property.</summary>
		/// <remarks>This is the title displayed at the top of every page.</remarks>
		[Category("Documentation Main Settings")]
		[Description("This is the title displayed at the top of every page.")]
		public string Title
		{
			get { return _Title; }

			set 
			{ 
				_Title = value;
				SetDirty();
			}
		}

		#endregion

		#region Deployment properties
		bool _RegisterTitleWithNamespace = false;

		/// <summary>
		/// Gets or sets the RegisterTitleWithNamespace property
		/// </summary>
		/// <remarks>Should the compiled Html 2 title be registered on this 
		/// machine after it is compiled. Good for testing. (If true CollectionNamespace is required)</remarks>
		[Category(DEPLOYMENT_CATEGORY)]
		[Description("Should the compiled Html 2 title be registered on this machine after it is compiled. Good for testing. (If true CollectionNamespace is required).")]
		[DefaultValue(false)]
		public bool RegisterTitleWithNamespace
		{
			get { return _RegisterTitleWithNamespace; }

			set
			{
				_RegisterTitleWithNamespace = value;
				SetDirty();
			}
		}

		string _CollectionNamespace = String.Empty;

		/// <summary>
		/// Gets or sets the CollectionNamespace property
		/// </summary>
		/// <remarks>The Html Help 2 registry namespace (avoid spaces). 
		/// Used in conjunction with GenerateCollectionFiles and RegisterTitleWithNamespace</remarks>
		[Category(DEPLOYMENT_CATEGORY)]
		[Description("The Html Help 2 registry namespace (avoid spaces). Used in conjunction with GenerateCollectionFiles and RegisterTitleWithNamespace.")]
		[DefaultValue("")]
		public string CollectionNamespace
		{
			get { return _CollectionNamespace; }

			set
			{
				_CollectionNamespace = value;
				SetDirty();
			}
		}		

		bool _RegisterTitleAsCollection = false;

		/// <summary>
		/// Gets or sets the RegisterTitleAsCollection property
		/// </summary>
		/// <remarks>If true the HxS title will be registered as a collection (ignored if RegisterTitleWithNamespace is true)</remarks>
		[Category(DEPLOYMENT_CATEGORY)]
		[Description("If true the HxS title will be registered as a collection on this machine (uses HtmlHelpName as the namespace name).  Good for testing. (ignored if RegisterTitleWithNamespace is true)")]
		[DefaultValue(false)]
		public bool RegisterTitleAsCollection
		{
			get { return _RegisterTitleAsCollection; }

			set
			{
				_RegisterTitleAsCollection = value;
				SetDirty();
			}
		}	

		bool _GenerateCollectionFiles = false;

		/// <summary>
		/// Gets or sets the GenerateCollectionFiles property
		/// </summary>
		/// <remarks>If true creates collection files to contain the help title. 
		/// These all the title to be plugged into the Visual Studio help namespace during deployment.</remarks>
		[Category(DEPLOYMENT_CATEGORY)]
		[Description("If true creates collection files to contain the help title. These all the title to be plugged into the Visual Studio help namespace during deployment.")]
		[DefaultValue(false)]
		public bool GenerateCollectionFiles
		{
			get { return _GenerateCollectionFiles; }

			set
			{
				_GenerateCollectionFiles = value;
				SetDirty();
			}
		}	

		string _PlugInNamespace = "ms.vscc";

		/// <summary>
		/// Gets or sets the PlugInNamespace property
		/// </summary>
		/// <remarks>If GenerateCollectionFiles is true, the resulting 
		/// collection will be plugged into this namespace during deployment</remarks>
		[Category(DEPLOYMENT_CATEGORY)]
		[Description("If GenerateCollectionFiles is true, the resulting collection will be plugged into this namespace during deployment. ('ms.vscc' is the VS.NET help namespace)")]
		[DefaultValue("ms.vscc")]
		public string PlugInNamespace
		{
			get { return _PlugInNamespace; }

			set
			{
				_PlugInNamespace = value;
				SetDirty();
			}
		}

		
		TOCStyle _CollectionTOCStyle = TOCStyle.Hierarchical;

		/// <summary>
		/// Gets or sets the CollectionTOCStyle property
		/// </summary>
		/// <remarks>Determines how the collection table of contents will appear in the help browser</remarks>
		[Category(DEPLOYMENT_CATEGORY)]
		[Description("Determines how the collection table of contents will appear in the help browser")]
		[DefaultValue(TOCStyle.Hierarchical)]
		public TOCStyle CollectionTOCStyle
		{
			get { return _CollectionTOCStyle; }

			set
			{
				_CollectionTOCStyle = value;
				SetDirty();
			}
		}
		#endregion

		#region HTML Help 2 properties

		short _LangID = 1033;

		/// <summary>Gets or sets the LangID property</summary>
		/// <remarks>The language ID of the locale used by the compiled helpfile</remarks>
		[Category(HTMLHELP2_CONFIG_CATEGORY)]
		[Description("The ID of the language the help file is in.")]
		[DefaultValue((short)1033)]
		[Editor(typeof(LangIdEditor), typeof(UITypeEditor))]
		public short LangID
		{
			get { return _LangID; }

			set
			{
				_LangID = value;
				SetDirty();
			}
		}	

		bool _BuildSeparateIndexFile = false;

		/// <summary>Gets or sets the BuildSeparateIndexFile property</summary>
		/// <remarks>If true a seperate index file is generated, otherwise it is compiled into the HxS (recommended)</remarks>
		[Category(HTMLHELP2_CONFIG_CATEGORY)]
		[Description("If true, create a separate index file (HxI), otherwise the index is compiled into the HxS file.")]
		[DefaultValue(false)]
		public bool BuildSeparateIndexFile
		{
			get { return _BuildSeparateIndexFile; }

			set
			{
				_BuildSeparateIndexFile = value;
				SetDirty();
			}
		}

		string _DocSetList = "NETFramework";

		/// <summary>Get's or sets the DocSetList property</summary>
		/// <remarks>A comma-seperated list of DocSet filter identifiers in which topics in this title will included.</remarks>
		[Category(HTMLHELP2_CONFIG_CATEGORY)]
		[Description("A comma-seperated list of DocSet filter identifiers in which topics in this title will included.")]
		[DefaultValue("NETFramework")]
		public string DocSetList
		{
			get { return _DocSetList; }

			set
			{
				_DocSetList = value;
				SetDirty();
			}
		}
	

		string _Version = "1.0.0.0";

		/// <summary>Get's or sets the version property</summary>
		/// <remarks>The version number for the help file (#.#.#.#)</remarks>
		[Category(HTMLHELP2_CONFIG_CATEGORY)]
		[Description("The version number for the help file (#.#.#.#)")]
		[DefaultValue("1.0.0.0")]
		public string Version
		{
			get { return _Version; }

			set
			{
				_Version = value;
				SetDirty();
			}
		}
	

		bool _CreateFullTextIndex = true;

		/// <summary>Gets or sets the CreateFullTextIndex property</summary>
		/// <remarks>If true creates a full text index for the help file</remarks>
		[Category(HTMLHELP2_CONFIG_CATEGORY)]
		[Description("If true creates a full text index for the help file")]
		[DefaultValue(true)]
		public bool CreateFullTextIndex
		{
			get { return _CreateFullTextIndex; }

			set
			{
				_CreateFullTextIndex = value;
				SetDirty();
			}
		}

		bool _IncludeDefaultStopWordList = true;

		/// <summary>Gets or sets the IncludeDefaultStopWordList property</summary>
		/// <remarks>If true the default stop word list is compiled into the help file. 
		/// (A stop word list is a list of words that will be ignored during a full text search)</remarks>
		[Category(HTMLHELP2_CONFIG_CATEGORY)]
		[Description("If true the default stop word list is compiled into the help file. (A stop word list is a " + 
			 "list of words that will be ignored during a full text search)")]
		[DefaultValue(true)]
		public bool IncludeDefaultStopWordList
		{
			get { return _IncludeDefaultStopWordList; }

			set
			{
				_IncludeDefaultStopWordList = value;
				SetDirty();
			}
		}

		FilePath _UseHelpNamespaceMappingFile = new FilePath();

		/// <summary>Gets or sets the UseHelpNamespaceMappingFile property.</summary>
		/// <remarks>If the documentation includes references to types registered in a seperate html help 2 
		/// namespace, supplying a mapping file allows XLinks to be created to topics within that namespace.
		/// </remarks>
		[Category(HTMLHELP2_CONFIG_CATEGORY)]
		[Description("If the documentation includes references to types registered in a seperate html help 2 " + 
			 "namespace, supplying a mapping file allows XLinks to be created to topics within that namespace. " + 
			 "Refer to the user's guide for more information about XLinks to other topics.")]
		[NDoc.Core.PropertyGridUI.FilenameEditor.FileDialogFilter
			 ("Select Namespace Mapping File", "XML files (*.xml)|*.xml|All files (*.*)|*.*")]
		public FilePath UseHelpNamespaceMappingFile
		{
			get { return _UseHelpNamespaceMappingFile; }

			set
			{
				if (_UseHelpNamespaceMappingFile.Path != value.Path)
				{
					_UseHelpNamespaceMappingFile = value;
					SetDirty();
				}
			}
		}
		void ResetUseHelpNamespaceMappingFile() { _UseHelpNamespaceMappingFile = new FilePath(); }

		
		string _HeaderHtml;

		/// <summary>Gets or sets the HeaderHtml property.</summary>
		/// <remarks>Raw HTML that is used as a page header instead of the default blue banner. 
		/// %FILE_NAME%\" is dynamically replaced by the name of the file for the current html page. 
		/// %TOPIC_TITLE%\" is dynamically replaced by the title of the current page.</remarks>
		[Category(HTMLHELP2_CONFIG_CATEGORY)]
		[Description("Raw HTML that is used as a page header instead of the default blue banner. " + 
			 "\"%FILE_NAME%\" is dynamically replaced by the name of the file for the current html page. " + 
			 "\"%TOPIC_TITLE%\" is dynamically replaced by the title of the current page.")]
		[Editor(typeof(TextEditor), typeof(UITypeEditor))]
		public string HeaderHtml
		{
			get { return _HeaderHtml; }

			set
			{
				_HeaderHtml = value;
				SetDirty();
			}
		}

		string _FooterHtml;

		/// <summary>Gets or sets the FooterHtml property.</summary>
		/// <remarks>Raw HTML that is used as a page footer instead of the default footer.
		/// %FILE_NAME% is dynamically replaced by the name of the file for the current html page. 
		/// %ASSEMBLY_NAME% is dynamically replaced by the name of the assembly for the current page.
		/// %ASSEMBLY_VERSION% is dynamically replaced by the version of the assembly for the current page.
		/// %TOPIC_TITLE% is dynamically replaced by the title of the current page.</remarks>
		[Category(HTMLHELP2_CONFIG_CATEGORY)]
		[Description("Raw HTML that is used as a page footer instead of the default footer." + 
			 "\"%FILE_NAME%\" is dynamically replaced by the name of the file for the current html page. " + 
			 "\"%ASSEMBLY_NAME%\" is dynamically replaced by the name of the assembly for the current page. " + 
			 "\"%ASSEMBLY_VERSION%\" is dynamically replaced by the version of the assembly for the current page. " + 
			 "\"%TOPIC_TITLE%\" is dynamically replaced by the title of the current page.")]
		[Editor(typeof(TextEditor), typeof(UITypeEditor))]
		public string FooterHtml
		{
			get { return _FooterHtml; }

			set
			{
				_FooterHtml = value;
				SetDirty();
			}
		}

		#endregion

		#region Additonal content properties
		
		FilePath _IntroductionPage = new FilePath();

		/// <summary>Gets or sets the IntroductionPage property</summary>
		/// <remarks>An HTML page that will be dispayed when the root TOC node is selected.</remarks>
		[Category(ADDITIONAL_CONTENT_CATEGORY)]
		[Description("An HTML page that will be dispayed when the root TOC node is selected")]
		[NDoc.Core.PropertyGridUI.FilenameEditor.FileDialogFilter
			 ("Select Introduction Page", "HTML files (*.html;*.htm)|*.html;*.htm|All files (*.*)|*.*")]
		public FilePath IntroductionPage
		{
			get { return _IntroductionPage; }

			set
			{
				if (_IntroductionPage.Path != value.Path)
				{
					_IntroductionPage = value;
					SetDirty();
				}
			}
		}
		void ResetIntroductionPage() { _IntroductionPage = new FilePath(); }

		
		FilePath _AboutPageInfo = new FilePath();

		/// <summary>Gets or sets the AboutPageInfo property</summary>
		/// <remarks>Displays product information in Help About.</remarks>
		[Category(ADDITIONAL_CONTENT_CATEGORY)]
		[Description("Displays product information in Help About.")]
		[NDoc.Core.PropertyGridUI.FilenameEditor.FileDialogFilter
			 ("Select AboutPageInfo", "HTML files (*.html;*.htm)|*.html;*.htm|All files (*.*)|*.*")]
		public FilePath AboutPageInfo
		{
			get { return _AboutPageInfo; }

			set
			{
				if (_AboutPageInfo.Path != value.Path)
				{
					_AboutPageInfo = value;
					SetDirty();
				}
			}
		}
		void ResetAboutPageInfo() { _AboutPageInfo = new FilePath(); }


		FilePath _EmptyIndexTermPage = new FilePath();

		/// <summary>Gets or sets the EmptyIndexTermPage property</summary>
		/// <remarks>Displays when a user chooses a keyword index term that has 
		/// subkeywords but is not directly associated with a topic itself.</remarks>
		[Category(ADDITIONAL_CONTENT_CATEGORY)]
		[Description("Displays when a user chooses a keyword index term that has subkeywords but is not directly associated with a topic itself.")]
		[NDoc.Core.PropertyGridUI.FilenameEditor.FileDialogFilter
			 ("Select EmptyIndexTerm Page", "HTML files (*.html;*.htm)|*.html;*.htm|All files (*.*)|*.*")]
		public FilePath EmptyIndexTermPage
		{
			get { return _EmptyIndexTermPage; }

			set
			{
				if (_EmptyIndexTermPage.Path != value.Path)
				{
					_EmptyIndexTermPage = value;
					SetDirty();
				}
			}
		}		
		void ResetEmptyIndexTermPage() { _EmptyIndexTermPage = new FilePath(); }


		FilePath _NavFailPage = new FilePath();

		/// <summary>Gets or sets the NavFailPage property</summary>
		/// <remarks>Page that opens if a link to a topic or URL is broken.</remarks>
		[Category(ADDITIONAL_CONTENT_CATEGORY)]
		[Description("Opens if a link to a topic or URL is broken.")]
		[NDoc.Core.PropertyGridUI.FilenameEditor.FileDialogFilter
			 ("Select NavFail Page", "HTML files (*.html;*.htm)|*.html;*.htm|All files (*.*)|*.*")]
		public FilePath NavFailPage
		{
			get { return _NavFailPage; }

			set
			{
				if (_NavFailPage.Path != value.Path)
				{
					_NavFailPage = value;
					SetDirty();
				}
			}
		}	
		void ResetNavFailPage() { _NavFailPage = new FilePath(); }

	
		FilePath _AboutPageIconPage = new FilePath();

		/// <summary>Gets or sets the AboutPageIconPage property</summary>
		/// <remarks>HTML file that displays the Help About image.</remarks>
		[Category(ADDITIONAL_CONTENT_CATEGORY)]
		[Description("HTML file that displays the Help About image.")]
		[NDoc.Core.PropertyGridUI.FilenameEditor.FileDialogFilter
			 ("Select AboutPageIcon Page", "HTML files (*.html;*.htm)|*.html;*.htm|All files (*.*)|*.*")]
		public FilePath AboutPageIconPage
		{
			get { return _AboutPageIconPage; }

			set
			{
				if (_AboutPageIconPage.Path != value.Path)
				{
					_AboutPageIconPage = value;
					SetDirty();
				}
			}
		}		
		void ResetAboutPageIconPage() { _AboutPageIconPage = new FilePath(); }


		FolderPath _AdditionalContentResourceDirectory = new FolderPath();

		/// <summary>Gets or sets the AdditionalContentResourceDirectory property</summary>
		/// <remarks>Directory that contains resources (images etc.) used by the additional content pages. 
		/// This directory will be recursively compiled into the help file.</remarks>
		[Category(ADDITIONAL_CONTENT_CATEGORY)]
		[Description("Directory that contains resources (images etc.) used by the additional content pages. This directory will be recursively compiled into the help file.")]
		[NDoc.Core.PropertyGridUI.FoldernameEditor.FolderDialogTitle("Select AdditionalContentResourceDirectory")]
		public FolderPath AdditionalContentResourceDirectory
		{
			get { return _AdditionalContentResourceDirectory; }

			set
			{
				if (_AdditionalContentResourceDirectory.Path != value.Path)
				{
					_AdditionalContentResourceDirectory = value;
					SetDirty();
				}
			}
		}	
		void ResetAdditionalContentResourceDirectory() { _AdditionalContentResourceDirectory = new FolderPath(); }
		#endregion


		#region Extensibility properties
		FilePath _ExtensibilityStylesheet = new FilePath();

		/// <summary>Gets or sets the ExtensibilityStylesheet property</summary>
		/// <remarks>Path to an xslt stylesheet that contains templates for documenting extensibility tags.</remarks>
		[Category("Extensibility")]
		[Description("Path to an xslt stylesheet that contains templates for documenting extensibility tags. Refer to the NDoc user's guide for more details on extending NDoc.")]
		[NDoc.Core.PropertyGridUI.FilenameEditor.FileDialogFilter
			 ("Select Extensibility Stylesheet", "Stylesheet files (*.xslt)|*.xslt|All files (*.*)|*.*")]
		public FilePath ExtensibilityStylesheet
		{
			get { return _ExtensibilityStylesheet; }

			set
			{
				if (_ExtensibilityStylesheet != value)
				{
					_ExtensibilityStylesheet = value;
					SetDirty();
				}
			}
		}	
		void ResetExtensibilityStylesheet() { _ExtensibilityStylesheet = new FilePath(); }
		#endregion
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected override string HandleUnknownPropertyType(string name, string value)
		{
			string FailureMessages = "";

			if (String.Compare(name, "LinkToSdkDocVersion", true) == 0) 
			{
				Trace.WriteLine("WARNING: " + base.DocumenterInfo.Name + " Configuration - property 'LinkToSdkDocVersion' is OBSOLETE. Please use new property 'SdkDocVersion'\n");
				Project.SuspendDirtyCheck=false;
				FailureMessages += base.ReadProperty("SdkDocVersion", value);
				Project.SuspendDirtyCheck=true;
			}
			else
			{
				// if we don't know how to handle this, let the base class have a go
				FailureMessages = base.HandleUnknownPropertyType(name, value);
			}
			return FailureMessages;
		}
	}
}
