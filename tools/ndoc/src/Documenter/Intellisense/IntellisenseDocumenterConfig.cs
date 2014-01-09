using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

using NDoc.Core;
using NDoc.Core.Reflection;

namespace NDoc.Documenter.Intellisense
{
	/// <summary>
	/// Config settings for the native Intellisense Documenter
	/// </summary>
	[DefaultProperty("OutputDirectory")]
	public class IntellisenseDocumenterConfig : BaseReflectionDocumenterConfig
	{
		/// <summary>
		/// Creates a new <see cref="IntellisenseDocumenterConfig"/> instance.
		/// </summary>
		public IntellisenseDocumenterConfig( IntellisenseDocumenterInfo info ) : base( info )
		{
			base.AutoDocumentConstructors = true;
			base.DocumentAttributes = false;
			base.DocumentInheritedMembers = false;
			base.DocumentInheritedFrameworkMembers = false;
			base.UseNamespaceDocSummaries = false;
		}
		
		/// <summary>
		/// Creates an instance of a documenter <see cref="IDocumenterConfig.CreateDocumenter"/>
		/// </summary>
		/// <returns>IDocumenter instance</returns>		
		public override IDocumenter CreateDocumenter()
		{
			return new IntellisenseDocumenter( this );
		}


		string _outputDirectory = string.Format(".{0}intellisense{0}", Path.DirectorySeparatorChar);
		
		/// <summary>Gets or sets the OutputDirectory property.</summary>
		/// <remarks>The folder where the root of the HTML set will be located.
		/// This can be absolute or relative from the .ndoc project file.</remarks>
		[Category("Documentation Main Settings")]
		[Description("The directory in which the XML files will be generated.\nThis can be absolute or relative from the .ndoc project file.")]
		[Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
		public string OutputDirectory
		{
			get { return _outputDirectory; }

			set
			{
				_outputDirectory = value;

				if (!_outputDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()))
				{
					_outputDirectory += Path.DirectorySeparatorChar;
				}

				SetDirty();
			}
		}
		void ResetOutputDirectory() { _outputDirectory = string.Format(".{0}intellisense{0}", Path.DirectorySeparatorChar); }


		/// <summary>
		/// Gets or sets a value indicating whether to exclude 'NameSpaceDoc' classes.
		/// </summary>
		[Category("Visibility")]
		[Description("If true, classes named 'NamespaceDoc' will be excluded from the xml.\nDefault is false.")]
		[DefaultValue(false)]
		public bool ExcludeNameSpaceDocClasses
		{
			get { return base.UseNamespaceDocSummaries; }
			set { base.UseNamespaceDocSummaries = value; }
		}
		
		// HIDE BaseReflectionDocumenter that we do not need
		#region non - browsable properties
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool UseNamespaceDocSummaries { get { return base.UseNamespaceDocSummaries; }  }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new AssemblyVersionInformationType AssemblyVersionInfo { get { return base.AssemblyVersionInfo; }  }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool AutoDocumentConstructors { get { return base.AutoDocumentConstructors; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool AutoPropertyBackerSummaries { get { return base.AutoPropertyBackerSummaries; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new string CopyrightHref { get { return base.CopyrightHref; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new string CopyrightText { get { return base.CopyrightText; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new string FeedbackEmailAddress { get { return base.FeedbackEmailAddress; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool DocumentAttributes { get { return base.DocumentAttributes; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new string DocumentedAttributes { get { return base.DocumentedAttributes; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool DocumentInheritedAttributes { get { return base.DocumentInheritedAttributes; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool ShowTypeIdInAttributes { get { return base.ShowTypeIdInAttributes; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool DocumentInheritedFrameworkMembers { get { return base.DocumentInheritedFrameworkMembers; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool DocumentInheritedMembers { get { return base.DocumentInheritedMembers; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool Preliminary { get { return base.Preliminary; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool IncludeDefaultThreadSafety { get { return base.IncludeDefaultThreadSafety; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool InstanceMembersDefaultToSafe { get { return base.InstanceMembersDefaultToSafe; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool StaticMembersDefaultToSafe { get { return base.StaticMembersDefaultToSafe; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool DocumentEmptyNamespaces { get { return false; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool ShowMissingParams { get { return base.ShowMissingParams; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool ShowMissingRemarks { get { return base.ShowMissingRemarks; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool ShowMissingReturns { get { return base.ShowMissingReturns; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool ShowMissingSummaries { get { return base.ShowMissingSummaries; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool ShowMissingValues { get { return base.ShowMissingValues; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new SdkLanguage SdkDocLanguage { get { return base.SdkDocLanguage; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new SdkVersion SdkDocVersion { get { return base.SdkDocVersion; } }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool CleanIntermediates { get { return base.CleanIntermediates; } }
		#endregion
	}
}
