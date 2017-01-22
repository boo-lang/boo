using System;
using System.IO;
using System.ComponentModel;

using Microsoft.Win32;

using NDoc.Core;
using NDoc.Documenter.Msdn;

namespace NDoc.Documenter.HtmlHelp2
{
	/// <summary>
	/// Represents the character set  that will be used when compiling the Hxs file
	/// </summary>
	public enum CharacterSet
	{
		/// <summary>
		/// Ascii characters set
		/// </summary>
		Ascii,
		/// <summary>
		/// UTF 8 character set
		/// </summary>
		UTF8,
		/// <summary>
		/// Unicode chacracters
		/// </summary>
		Unicode
	}

	/// <summary>
	/// Config setting for the CHM to HxS converter/compiler
	/// </summary>
	/// 
	[Obsolete( "This documenter is now obsolete, you should use the VS.NET (NativeHtmlHelp2) documenter instead" ) ]
	public class HtmlHelp2Config : MsdnDocumenterConfig
	{
		private const string HTMLHELP2_CONFIG_CATEGORY = "Html Help v2.0 Settings";

		/// <summary>Initializes a new instance of the MsdnHelpConfig class.</summary>
		public HtmlHelp2Config() : base( "HtmlHelp2" )
		{
		}

		CharacterSet _CharacterSet = CharacterSet.Ascii;
		/// <summary>
		/// Gets or sets the character set that will be used when compiling the help file.
		/// Defaults to Ascii.
		/// </summary>
		[Category(HTMLHELP2_CONFIG_CATEGORY)]
		[Description("Gets or sets the character set that will be used when compiling the help file")]
		[DefaultValue(CharacterSet.Ascii)]
		public CharacterSet CharacterSet
		{
			get{ return _CharacterSet; }
			set
			{
				_CharacterSet = value;
				SetDirty();
			}
		}


		private string _HtmlHelp2CompilerPath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
			"Microsoft Help 2.0 SDK");

		private bool FindHxComp()
		{
			return File.Exists(Path.Combine(_HtmlHelp2CompilerPath, "hxcomp.exe"));
		}

		internal string HtmlHelp2CompilerPath
		{
			get
			{
				if (FindHxComp())
				{
					return _HtmlHelp2CompilerPath;
				}

				//not in default dir, try to locate it from the registry
				RegistryKey key = Registry.ClassesRoot.OpenSubKey("Hxcomp.HxComp");
				if (key != null)
				{
					key = key.OpenSubKey("CLSID");
					if (key != null)
					{
						object val = key.GetValue(null);
						if (val != null)				
						{
							string clsid = (string)val;
							key = Registry.ClassesRoot.OpenSubKey("CLSID");
							if (key != null)
							{
								key = key.OpenSubKey(clsid);
								if (key != null)
								{
									key = key.OpenSubKey("LocalServer32");
									if (key != null)
									{
										val = key.GetValue(null);
										if (val != null)
										{
											string path = (string)val;
											_HtmlHelp2CompilerPath = Path.GetDirectoryName(path);
											if (FindHxComp())
											{
												return _HtmlHelp2CompilerPath;
											}
										}
									}
								}
							}
						}
					}
				}

				//still not finding the compiler, give up
				throw new DocumenterException(
					"Unable to find the HTML Help 2 Compiler. Please verify that the Microsoft Visual Studio .NET Help Integration Kit has been installed.");
			}
		}

		
		bool _DeleteCHM = false;

		/// <summary>Flag that indicates whether to keep the CHM file after successful conversion</summary>
		[Category(HTMLHELP2_CONFIG_CATEGORY)]
		[Description("If true the CHM file will be deleted after the HxS file is created")]
		[DefaultValue(false)]
		public bool DeleteCHM
		{
			get { return _DeleteCHM; }

			set
			{
				_DeleteCHM = value;
				SetDirty();
			}
		}

		bool _AugmentXmlDataIslands = true;

		/// <summary>Adds additional tags to the embedded Xml data islands (results in slower builds but tighter VS.NET integration)</summary>
		[Category(HTMLHELP2_CONFIG_CATEGORY)]
		[Description("Adds additional tags to the embedded Xml data islands (results in slower builds but tighter VS.NET integration)")]
		[DefaultValue(true)]
		public bool AugmentXmlDataIslands
		{
			get { return _AugmentXmlDataIslands; }

			set
			{
				_AugmentXmlDataIslands = value;
				SetDirty();
			}
		}

		bool _RegisterTitleWithNamespace = false;

		/// <summary>
		/// Should the compiled Html 2 title be registered after it is compiled. (If true ParentCollectionNamespace is required)
		/// </summary>
		[Category(HTMLHELP2_CONFIG_CATEGORY)]
		[Description("Should the compiled Html 2 title be registered after it is compiled. (If true ParentCollectionNamespace is required)")]
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

		string _ParentCollectionNamespace = String.Empty;

		/// <summary>
		/// If RegisterTitleWithNamespace is true this is the namesapce to which it will be added.
		/// </summary>
		[Category(HTMLHELP2_CONFIG_CATEGORY)]
		[Description("The Html Help 2 registry namespace (avoid spaces). Only used if RegisterTitleWithNamespace is True.")]
		[DefaultValue("")]
		public string ParentCollectionNamespace
		{
			get { return _ParentCollectionNamespace; }

			set
			{
				_ParentCollectionNamespace = value;
				SetDirty();
			}
		}		

		bool _RegisterTitleAsCollection = false;

		/// <summary>
		/// If true the HxS title will be registered as a collection (ignored if RegisterTitleWithNamespace is ture)
		/// </summary>
		[Category(HTMLHELP2_CONFIG_CATEGORY)]
		[Description("If true the HxS title will be registered as a collection (ignored if RegisterTitleWithNamespace is ture)")]
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

		#region Not yet implemented features
		
//		bool _BuildSeperateIndexFile = true;
//
//		/// <summary>Gets or sets the property that causes a seperate index file to be generated.</summary>
//		[Category(HTMLHELP2_CONFIG_CATEGORY)]
//		[Description("If true, create a seperate index file (HxI), otherwise the index is compiled into the HxS file.")]
//		public bool BuildSeperateIndexFile
//		{
//			get { return _BuildSeperateIndexFile; }
//
//			set
//			{
//				_BuildSeperateIndexFile = value;
//				SetDirty();
//			}
//		}
//
//
//		string _Version = "1.0.0.0";
//
//		/// <summary>Gets or sets the base directory used to resolve directory and assembly references.</summary>
//		[Category(HTMLHELP2_CONFIG_CATEGORY)]
//		[Description("The version number for the help file (#.#.#.#)")]
//		public string Version
//		{
//			get { return _Version; }
//
//			set
//			{
//				_Version = value;
//				SetDirty();
//			}
//		}
	

		#endregion
	
	}
}
