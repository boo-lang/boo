using System;
using System.IO;
using System.Text;
using System.Collections;

namespace NDoc.Documenter.NativeHtmlHelp2.HxProject
{
	/// <summary>
	/// Summary description for H2RegFile.
	/// </summary>
	public class H2RegFile
	{
		private string collectionNamespace = string.Empty;
		private string pluginNamespace = string.Empty;
		private string description = string.Empty;
		private int langId;
		private string collectionFileName = string.Empty;
		private string docSetName = string.Empty;
		private string docSetFilter = string.Empty;

		private ArrayList titles;

		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		/// <param name="name">The name of the file that will be created</param>
		public H2RegFile( string name )
		{
			collectionNamespace = name;
			titles = new ArrayList();
		}

		/// <summary>
		/// Saves the INI file to disk
		/// </summary>
		/// <param name="path">The full path without filename</param>
		public void Save( string path )
		{
			StringBuilder sb = new StringBuilder();

			sb.Append( "; This file is intended for input into the H2Reg.exe help registration utility.\r\n" );
			sb.Append( "; H2Reg.exe is a shareware product from The Helpware Group that can be integrated into\r\n" );
			sb.Append( "; installers used for deploying html help 2 titles and collections.\r\n" );
			sb.Append( "; H2Reg.exe can be downloaded and licensed from http://www.helpware.net\r\n\r\n" );

			sb.Append( "[Reg_Namespace]\r\n" );
			sb.AppendFormat( "{0}|{1}|{2}\r\n\r\n", CollectionNamespace, CollectionFileName, Description );

			sb.Append( "[UnReg_Namespace]\r\n" );
			sb.AppendFormat( "{0}\r\n\r\n", CollectionNamespace );

			if ( PluginNamespace.Length > 0 )
			{
				sb.Append( "[Reg_Plugin]\r\n" );
				sb.AppendFormat( "{0}+|_DEFAULT|{1}|_DEFAULT|\r\n\r\n", PluginNamespace, CollectionNamespace );

				sb.Append( "[UnReg_Plugin]\r\n" );
				sb.AppendFormat( "{0}+|_DEFAULT|{1}|_DEFAULT|\r\n\r\n", PluginNamespace, CollectionNamespace );
			}

			if ( docSetName.Length > 0 && docSetFilter.Length > 0 )
			{
				sb.Append( "[Reg_Filter]\r\n" );
				sb.AppendFormat( "{0}|{1}|(\"DocSet\"=\"{2}\")\r\n\r\n", CollectionNamespace, docSetName, docSetFilter );

				sb.Append( "[UnReg_Filter]\r\n" );
				sb.AppendFormat( "{0}|{1}\r\n\r\n", CollectionNamespace, docSetName );
			}

			sb.Append( "[Reg_Title]\r\n" );
			foreach( HelpTitle title in titles )
				sb.AppendFormat( "{0}|{1}|{2}|{3}|{4}||||||\r\n", CollectionNamespace, title.TitleId, title.LangId, title.FileName, title.IndexFileName );

			sb.Append( "\r\n[UnReg_Title]\r\n" );
			foreach( HelpTitle title in titles )
				sb.AppendFormat( "{0}|{1}|{2}\r\n", CollectionNamespace, title.TitleId, title.LangId );

			using ( StreamWriter writer = File.CreateText( Path.Combine( path, CollectionNamespace + ".h2reg.ini" ) ) )
				writer.Write( sb.ToString() );
		}

		/// <summary>
		/// Sets a DocSet filter that will be installed with the collection
		/// </summary>
		/// <param name="name">The friendly name of the filter</param>
		/// <param name="docSet">The DocSet query value</param>
		public void SetDocSetFilter( string name, string docSet )
		{
			docSetName = name;
			docSetFilter = docSet;
		}

		/// <summary>
		/// The namespace of the collection being registered
		/// </summary>
		public string CollectionNamespace
		{
			get { return collectionNamespace; }
		}

		/// <summary>
		/// The optional namespace of a collection to plug this collection into
		/// (ms.vcc is the VS.NET namespace)
		/// </summary>
		public string PluginNamespace
		{
			get { return pluginNamespace; }
			set { pluginNamespace = value; }
		}

		/// <summary>
		/// Language identifier for the collection
		/// </summary>
		public int LangId
		{
			get{ return langId; }
			set{ langId = value; }
		}

		/// <summary>
		/// Description of the help collection
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		/// <summary>
		/// Filename of the HxC that describes the collection
		/// </summary>
		public string CollectionFileName
		{
			get { return collectionFileName; }
			set { collectionFileName = value; }
		}	

		/// <summary>
		/// Adds a title to the collection
		/// </summary>
		/// <param name="titleId">The id of the title</param>
		/// <param name="titleLangId">The language id of the title</param>
		/// <param name="titleFileName">The name of the HxS file containing the title</param>
		public void AddTitle( string titleId, int titleLangId, string titleFileName )
		{
			AddTitle( titleId, titleLangId, titleFileName, titleFileName );
		}

		/// <summary>
		/// Adds a title to the collection
		/// </summary>
		/// <param name="titleId">The id of the title</param>
		/// <param name="titleLangId">The language id of the title</param>
		/// <param name="titleFileName">The name of the HxS file containing the title</param>
		/// <param name="indexFileName">The name of the HxI file containing the title's index</param>
		public void AddTitle( string titleId, int titleLangId, string titleFileName, string indexFileName )
		{
			HelpTitle title = new HelpTitle();

			title.TitleId = titleId;
			title.LangId = titleLangId;
			title.FileName = titleFileName;
			title.IndexFileName = indexFileName;

			titles.Add( title );
		}

		private struct HelpTitle
		{
			public string TitleId;
			public int LangId;
			public string FileName;
			public string IndexFileName;
		}

	}
}
