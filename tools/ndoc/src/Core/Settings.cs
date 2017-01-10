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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.Collections;

namespace NDoc.Core
{
	/// <summary>
	/// This class manages read write access to application settings
	/// </summary>
	/// 
	public class Settings : IDisposable
	{

		/// <summary>
		/// The full path the the default user settings file
		/// </summary>
		public static string UserSettingsFile
		{
			get
			{
				return Path.Combine( UserSettingsLocation, "settings.xml" );
			}
		}
		
		/// <summary>
		/// The full path the the default machine settings file
		/// </summary>
		public static string MachineSettingsFile
		{
			get
			{
				return Path.Combine( MachineSettingsLocation, "settings.xml" );
			}
		}

		/// <summary>
		/// The path to the folder where the user specific settings file is stored
		/// </summary>
		private static string UserSettingsLocation
		{
			get
			{				
				return Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), SettingsFolderName );
			}
		}

		/// <summary>
		/// The path to the folder where the machine wide settings file is stored
		/// </summary>
		private static string MachineSettingsLocation
		{
			get
			{
				return Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData ), SettingsFolderName );
			}
		}

		/// <summary>
		/// Gets the name of the settings folder.
		/// </summary>
		/// <value></value>
		private static string SettingsFolderName
		{
			get
			{
				// create a path for this major.minor version of the app
				Version version = Assembly.GetExecutingAssembly().GetName().Version;
				return string.Format( "NDoc.{0}.{1}", version.Major, version.Minor );
			}
		}

		private XmlNode data;
		private string path;

		/// <summary>
		/// Creates a new instance of the <see cref="Settings"/> class.
		/// </summary>
		/// <param name="filePath">Path to serialized settings</param>
		public Settings( string filePath )
		{
			// if the file exists, try and open it
			if ( File.Exists( filePath ) )
			{
				XmlTextReader reader = null;
				try
				{
					XmlDocument doc = new XmlDocument();
					reader = new XmlTextReader( filePath );
				
					doc.Load( reader );
					if ( doc.DocumentElement != null )
						data = doc.DocumentElement;				
				}
				catch ( Exception )
				{
					// if for any reason we couldn't parse the settings
					// document, we're gonna delete it and create a new one
					data = null;
				}
				finally
				{
					if ( reader != null )
						reader.Close();
				}
			}

			// if we weren't able to open the file for any reason
			// create a new one
			if ( data == null )		
				data = CreateNew( filePath );
			
			Debug.Assert( data != null );

			path = filePath;
		}

		/// <summary>
		/// <see cref="System.IDisposable.Dispose"/>
		/// </summary>
		public void Dispose()
		{			
			this.SetSetting( "versions", "lastSavedBy", Assembly.GetExecutingAssembly().GetName().Version.ToString() );
			data.OwnerDocument.Save( path );
		}
		/// <summary>
		/// Retrieves the value of a setting
		/// </summary>
		/// <param name="section">The section name to store the list under</param>
		/// <param name="name">The name of the setting</param>
		/// <param name="defaultValue">The value to use if no setting is found</param>
		/// <returns>The stored setting or the default value if no stroed setting is found</returns>
		public bool GetSetting( string section, string name, bool defaultValue )
		{
			try 
			{
				XmlNode setting = data.SelectSingleNode( string.Format( "{0}/{1}", section, name ) );
				if ( setting != null )
					return XmlConvert.ToBoolean( setting.InnerText );
			}
			catch ( Exception e )
			{
				Trace.WriteLine( e.Message );
			}

			return defaultValue;

		}
		/// <summary>
		/// Retrieves the value of a setting
		/// </summary>
		/// <param name="section">The section name to store the list under</param>
		/// <param name="name">The name of the setting</param>
		/// <param name="defaultValue">The value to use if no setting is found</param>
		/// <returns>The stored setting or the default value if no stroed setting is found</returns>
		public int GetSetting( string section, string name, int defaultValue )
		{
			try 
			{
				XmlNode setting = data.SelectSingleNode( string.Format( "{0}/{1}", section, name ) );
				if ( setting != null )
					return XmlConvert.ToInt32( setting.InnerText );
			}
			catch ( Exception e )
			{
				Trace.WriteLine( e.Message );
			}

			return defaultValue;

		}
		/// <summary>
		/// Retrieves the value of a setting
		/// </summary>
		/// <param name="section">The section name to store the list under</param>
		/// <param name="name">The name of the setting</param>
		/// <param name="defaultValue">The value to use if no setting is found</param>
		/// <returns>The stored setting or the default value if no stroed setting is found</returns>
		public string GetSetting( string section, string name, string defaultValue )
		{
			try 
			{
				XmlNode setting = data.SelectSingleNode( string.Format( "{0}/{1}", section, name ) );
				if ( setting != null )
					return setting.InnerText;
			}
			catch ( Exception e )
			{
				Trace.WriteLine( e.Message );
			}

			return defaultValue;

		}

		/// <summary>
		/// Retrieves the value of a setting
		/// </summary>
		/// <param name="section">The section name to store the list under</param>
		/// <param name="name">The name of the setting</param>
		/// <param name="defaultValue">The value to use if no setting is found</param>
		/// <returns>The stored setting or the default value if no stroed setting is found</returns>
		public object GetSetting( string section, string name, object defaultValue )
		{
			if ( defaultValue == null )
				throw new NullReferenceException( "Null objects cannot be stored" );

			try 
			{
				XmlNode setting = data.SelectSingleNode( string.Format( "{0}/{1}", section, name ) );

				if ( setting != null )
				{
					XmlSerializer serializer = new XmlSerializer( defaultValue.GetType() );
				
					XmlNodeReader reader = new XmlNodeReader( setting.FirstChild );
					return serializer.Deserialize( reader );
				}
			}
			catch ( Exception e )
			{
				Trace.WriteLine( e.Message );
			}

			return defaultValue;
		}

		/// <summary>
		/// Retrieves a list of settings. If the list cannot be found
		/// then no items are added
		/// </summary>
		/// <param name="section">The section name to store the list under</param>
		/// <param name="name">The name of the setting</param>
		/// <param name="itemType">The type of each setting in the list</param>
		/// <param name="list">A <see cref="IList"/> into which to put each item</param>
		public void GetSettingList( string section, string name, Type itemType, ref IList list )
		{
			if ( list == null )
				throw new NullReferenceException();

			try 
			{
				XmlNode setting = data.SelectSingleNode( string.Format( "{0}/{1}", section, name ) );

				if ( setting != null )
				{
					foreach( XmlNode node in setting.ChildNodes )
					{
						XmlSerializer serializer = new XmlSerializer( itemType );
				
						XmlNodeReader reader = new XmlNodeReader( node.FirstChild );
						list.Add( serializer.Deserialize( reader ) );
					}
				}
			}
			catch ( Exception e )
			{
				Trace.WriteLine( e.Message );
			}
		}

		/// <summary>
		/// Stores a list of settings
		/// </summary>
		/// <param name="section">The section name to store the list under</param>
		/// <param name="name">The name of the setting</param>
		/// <param name="itemName">The name of each item in the list</param>
		/// <param name="list">The list</param>
		public void SetSettingList( string section, string name, string itemName, IList list )
		{
			if ( list == null )
				throw new NullReferenceException();

			XmlNode setting = GetOrCreateSettingNode( section, name );
			if ( setting.ChildNodes.Count > 0 )
				setting.RemoveAll();

			foreach( object o in list )
			{
				if ( !object.ReferenceEquals( o, null ) )
				{
					XmlNode item = setting.AppendChild( data.OwnerDocument.CreateElement( itemName ) );
					item.InnerXml = SerializeObject( o );
				}
			}
		}
		/// <summary>
		/// Stores a setting
		/// </summary>
		/// <param name="section">The section name to store the setting in</param>
		/// <param name="name">The name of the setting</param>
		/// <param name="val">The setting's value</param>
		public void SetSetting( string section, string name, bool val )
		{
			SetSetting( section, name, XmlConvert.ToString( val ) );
		}
		/// <summary>
		/// Stores a setting
		/// </summary>
		/// <param name="section">The section name to store the setting in</param>
		/// <param name="name">The name of the setting</param>
		/// <param name="val">The setting's value</param>
		public void SetSetting( string section, string name, int val )
		{
			SetSetting( section, name, XmlConvert.ToString( val ) );
		}
		/// <summary>
		/// Stores a setting
		/// </summary>
		/// <param name="section">The section name to store the setting in</param>
		/// <param name="name">The name of the setting</param>
		/// <param name="val">The setting's value</param>
		/// <remarks>Passing an emtpy string removes the setting</remarks>
		public void SetSetting( string section, string name, string val )
		{
			if ( val.Length > 0 )
			{
				XmlNode setting = GetOrCreateSettingNode( section, name );
				setting.InnerText = val;
			}
			else
			{
				RemoveSetting( section, name );
			}
		}

		/// <summary>
		/// Stores a setting
		/// </summary>
		/// <param name="section">The section name to store the setting in</param>
		/// <param name="name">The name of the setting</param>
		/// <param name="val">The setting's value</param>
		/// <remarks>Passing a null object removes the setting</remarks>
		public void SetSetting( string section, string name, object val )
		{
			if ( !object.ReferenceEquals( val, null ) )
			{
				XmlNode setting = GetOrCreateSettingNode( section, name );
			
				setting.InnerXml = SerializeObject( val );
			}
			else
			{
				RemoveSetting( section, name );
			}
		}

		/// <summary>
		/// Removes a setting
		/// </summary>
		/// <param name="section">Setting section</param>
		/// <param name="name">Setting name</param>
		public void RemoveSetting( string section, string name )
		{
			XmlNode sectionNode = data.SelectSingleNode( section );
			if ( sectionNode != null )			
			{
				XmlNode setting = sectionNode.SelectSingleNode( name );
				if ( setting != null )			
					sectionNode.RemoveChild( setting );
			}
		}

		private static string SerializeObject( object o )
		{
			Debug.Assert( !object.ReferenceEquals( o, null ) );
			Debug.Assert( o.GetType().IsSerializable );

			XmlSerializer serializer = new XmlSerializer( o.GetType() );

			StringBuilder sb = new StringBuilder();
			NoPrologXmlWriter writer = new NoPrologXmlWriter( new StringWriter( sb ) );

			serializer.Serialize( writer, o );
			writer.Close();

			return sb.ToString();
		}

		private XmlNode GetOrCreateSettingNode( string section, string name )
		{
			XmlNode sectionNode = data.SelectSingleNode( section );
			if ( sectionNode == null )			
				sectionNode = data.AppendChild( data.OwnerDocument.CreateElement( section ) );

			XmlNode setting = sectionNode.SelectSingleNode( name );
			if ( setting == null )			
				setting = sectionNode.AppendChild( data.OwnerDocument.CreateElement( name ) );

			return setting;
		}

		private static XmlNode CreateNew( string path )
		{
			if ( File.Exists( path ) )
				File.Delete( path );

			string folder = Path.GetDirectoryName( path );

			if ( !Directory.Exists( folder ) )
				Directory.CreateDirectory( folder );

			XmlDocument doc = new XmlDocument();
			doc.LoadXml( "<?xml version='1.0'?><setttings SchemaVersion='1.3'/>" );
			doc.Save( path );

			return doc.DocumentElement;
		}

		/// <summary>
		/// This class is used to serialize objects without inserting
		/// xml prolog or doctype declarations
		/// </summary>
		private class NoPrologXmlWriter : XmlTextWriter
		{
			public NoPrologXmlWriter( TextWriter writer ) : base( writer )
			{
			}
			public NoPrologXmlWriter(Stream stream, Encoding encoding) : base( stream, encoding )
			{
			}
			public NoPrologXmlWriter(String s, Encoding encoding) : base( s, encoding )
			{
			}

			public override void WriteDocType(string name,string pubid,string sysid,string subset)
			{

			}
			
			public override void WriteStartDocument(bool standalone)
			{

			}
			
			public override void WriteStartDocument()
			{

			}
		}
	}
}
