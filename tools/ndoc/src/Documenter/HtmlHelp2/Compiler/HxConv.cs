using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Net;
using NDoc.Core;



namespace NDoc.Documenter.HtmlHelp2.Compiler
{
	/*
		HXCONV HTML Help 1.x to Microsoft Help 2.0 Converter v1.0
		Copyright (c) 2000 Microsoft Corp.

		Usage:
			HXCONV [options] <input file>

		Options:
			-o <output dir>    Specify an output directory
			-l <logfile>       Specify a log file
			-m <mapping file>  Specify a CHM-to-namespace mapping file
			-w                 Output Hx files as Unicode
			-u                 Output Hx files as UTF-8
			-y                 Automatically overwrite existing files
			-v                 Verbose mode: output all errors
			-q                 Quiet mode: output nothing
			-s                 Suppress running display of progress

			-?                 Display this help message
	*/

	/// <summary>
	/// Converts compiled Html Help version 1 CHM files into
	/// Compiled Html Help version 2 HxS files
	/// This class wraps the HxConv.exe converter supplied with the HTML v2 SDK
	/// </summary>
	public class HxConv : HxObject
	{

		/// <summary>
		/// Create new instance of a Chm2HxsConverter
		/// </summary>
		/// <param name="compilerPath"><see cref="HxObject.CompilerPath"/></param>
		public HxConv( string compilerPath ) : base( compilerPath, "HxConv.exe" )
		{
		}

		/// <summary>
		/// Converts the specified CHM files
		/// </summary>
		/// <param name="CHMFile">The CHM Help file to convert</param>
		public void Convert( FileInfo CHMFile )
		{
			Execute( GetArguments( CHMFile ), CHMFile.Directory.FullName );
		}

		private string GetArguments( FileInfo CHMFile )
		{
			StringBuilder ret = new StringBuilder();

			ret.Append( " -q " ); //quiet mode
			ret.Append( " -s " ); //suppress progress
			ret.Append( " -y " ); //overwrite files

			ret.Append( " -l HxConv.log " );	//make a log file

			if ( _CharacterSet == CharacterSet.UTF8 )
				ret.Append( " -u " );
			else if ( _CharacterSet == CharacterSet.Unicode )
				ret.Append( " -w " );

			ret.Append( " -o . " );	//set the output directory to the processes working directory

			ret.Append( '"' );
			ret.Append( CHMFile.FullName );
			ret.Append( '"' );

			return ret.ToString();
		}

		CharacterSet _CharacterSet = CharacterSet.Ascii;
		/// <summary>
		/// Gets or sets the character set that will be used when converting the CHM file.
		/// Defaults to Ascii.
		/// </summary>
		public CharacterSet CharacterSet
		{
			get{ return _CharacterSet; }
			set	{ _CharacterSet = value; }
		}

		/// <summary>
		/// Convert between v1 and v2 htmlhelp icons 
		/// </summary>
		/// <param name="InputCHMPath">path to chm file</param>
		/// <remarks>
		/// Image numbers have changed between HtmlHelp v1 and v2. 
		/// Also, MS have introduced the 'new' attribute supposidly instead of
		/// using image numbers...BUT hxConv.exe DOESN'T TAKE THIS INTO ACCOUNT
		/// so we have to do it ourselves
		/// </remarks>
		public void ConvertHxTIcons(string InputCHMPath)
		{
			//work out to HxT file name
			string InputHxTFilePath = InputCHMPath.Replace(".chm",".HxT");

			XmlDocument TocDoc = OpenDocument(InputHxTFilePath);

			try
			{
				for ( int i=0; i<TocDoc.ChildNodes.Count; i++ )
				{
					XmlNode ChildNode=TocDoc.ChildNodes[i];
					if ( ChildNode.NodeType == XmlNodeType.Element )
					{
						ConvertHxtEle( ChildNode );
					}
				}
				TocDoc.Save(InputHxTFilePath);
			}
			catch ( Exception e )
			{
				throw new DocumenterException( "HxT conversion error", e );
			}
		}

		private void ConvertHxtEle( XmlNode Node )
		{
			FixNode( (XmlElement) Node );	

			for ( int i=0; i<Node.ChildNodes.Count; i++ )
			{
				XmlNode ChildNode=Node.ChildNodes[i];
				if ( ChildNode.NodeType == XmlNodeType.Element )
				{
					ConvertHxtEle( ChildNode );
				}
			}
		}

		private void FixNode( XmlElement Element )
		{
			string NewIcon="";
			string CurrentIcon = Element.GetAttribute("Icon");

			if (CurrentIcon=="") return;
			
			bool   IsNew=false;
			int    CurrentIconNum = int.Parse(CurrentIcon);

			//Note: MS states in its doco that we must set the new attribute
			//in order to get the new indicator - but that doesn't work..!
			//Instead, we explicitly set the icon num as well...
			
			switch (CurrentIconNum)
			{
				case 9:		//Question mark page icon 
					NewIcon="18";
					break;
				case 10:	//Question mark page icon, New property selected 
					NewIcon="19";
					IsNew=true;
					break;
				case 11:	//Page icon 
					NewIcon="16";
					break;
				case 12:	//Page icon, New property selected 
					NewIcon="17";
					IsNew=true;
					break;
				case 13:	//World page icon 
					NewIcon="20";
					break;
				case 14:	//World page icon, New property selected 
					NewIcon="21";
					IsNew=true;
					break;
				case 15:	//World Internet Explorer page icon 
					NewIcon="22";
					break;
				case 16:	//World Internet Explorer page icon, New property selected 
					NewIcon="23";
					IsNew=true;
					break;
				case 17:	//Information page icon 
					NewIcon="24";
					break;
				case 18:	//Information page icon, New property selected 
					NewIcon="25";
					IsNew=true;
					break;
				case 19:	//Shortcut page icon 
					NewIcon="26";
					break;
				case 20:	//Shortcut page icon, New property selected 
					NewIcon="27";
					IsNew=true;
					break;
				case 21:	//Envelope page icon 
					NewIcon="28";
					break;
				case 22:	//Envelope page icon, New property selected 
					NewIcon="29";
					IsNew=true;
					break;
				case 23:	//Envelope page icon 
					NewIcon="28";
					break;
				case 24:	//Envelope page icon, New property selected 
					NewIcon="29";
					IsNew=true;
					break;
				case 25:	//Envelope page icon 
					NewIcon="28";
					break;
				case 26:	//Envelope page icon, New property selected 
					NewIcon="29";
					IsNew=true;
					break;
				case 27:	//Person page icon 
					NewIcon="30";
					break;
				case 28:	//Person page icon, New property selected 
					NewIcon="31";
					IsNew=true;
					break;
				case 29:	//Sound page icon 
					NewIcon="32";
					break;
				case 30:	//Sound page icon, New property selected 
					NewIcon="33";
					IsNew=true;
					break;
				case 31:	//Disc page icon 
					NewIcon="34";
					break;
				case 32:	//Disc page icon, New property selected 
					NewIcon="35";
					IsNew=true;
					break;
				case 33:	//Video page icon 
					NewIcon="36";
					break;
				case 34:	//Video page icon, New property selected 
					NewIcon="37";
					IsNew=true;
					break;
				case 35:	//Steps page icon 
					NewIcon="38";
					break;
				case 36:	//Steps page icon, New property selected 
					NewIcon="39";
					IsNew=true;
					break;
				case 37:	//Question page icon 
					NewIcon="40";
					break;
				case 38:	//Question page icon, New property selected 
					NewIcon="41";
					IsNew=true;
					break;
				case 39:	//Pencil page icon 
					NewIcon="42";
					break;
				case 40:	//Pencil page icon, New property selected 
					NewIcon="43";
					IsNew=true;
					break;
				case 41:	//Tool page icon 
					NewIcon="44";
					break;
				case 42:	//Tool page icon, New property selected 
					NewIcon="45";
					IsNew=true;
					break;
				default:
					break;
			}

			if (NewIcon =="")
			{
				Element.RemoveAttribute("Icon");
			}
			else
			{
				Element.SetAttribute("Icon", NewIcon);
			}


			if (IsNew)
			{
				Element.SetAttribute("New","Yes");
			}
		}

		private XmlDocument OpenDocument( string FilePath ) 
		{
			XmlValidatingReader reader = new XmlValidatingReader( new XmlTextReader( FilePath ) );
			reader.ValidationType = ValidationType.None;
			reader.XmlResolver = null;		

			XmlDocument doc = new XmlDocument();
			doc.Load( reader );
			reader.Close();			//make sure we close the reader before saving

			return doc;
		}

	}
}
