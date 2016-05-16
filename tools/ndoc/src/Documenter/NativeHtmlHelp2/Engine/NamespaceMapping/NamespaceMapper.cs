using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;
using System.Reflection;
using System.Collections;

namespace NDoc.Documenter.NativeHtmlHelp2.Engine.NamespaceMapping
{
	/// <summary>
	/// NamespaceMapper allows managed namespaces to be asscoitated with html help namespaces
	/// when creating XLinks in the compiled help
	/// </summary>
	public class NamespaceMapper
	{

		#region Static Members
		private readonly static string mapXmlNamespace = "urn:ndoc-sourceforge-net:documenters.NativeHtmlHelp2.schemas.namespaceMap";

		private static XmlSchema namespaceMapSchema = null;
		private static XmlNamespaceManager nsmgr;
		private static bool schemaIsValid = true;

		static NamespaceMapper()
		{
			Stream schemaStream = GetSchemaResource();
			try
			{
				XmlSchema s = XmlSchema.Read( schemaStream, new ValidationEventHandler( validateSchema ) );

				NameTable table = new NameTable();
				nsmgr = new XmlNamespaceManager( table );
				nsmgr.AddNamespace( "map", mapXmlNamespace );
				
				namespaceMapSchema = s;
			}
			catch ( Exception )
			{
				namespaceMapSchema = null;
				nsmgr = null;
				schemaIsValid = false;
			}
			finally
			{
				schemaStream.Close();
			}
		}

		static Stream GetSchemaResource()
		{
			string name = "NDoc.Documenter.NativeHtmlHelp2.Engine.NamespaceMapping.NamespaceMap.xsd";

			return Assembly.GetExecutingAssembly().GetManifestResourceStream( name );
		}

		private static void validateSchema(object sender, ValidationEventArgs e)
		{
			Trace.WriteLine( e.Message );	
			schemaIsValid = false;
		}
		#endregion

		private XmlNode map;		// XmlNode representing hte map data
		private Hashtable helpNamespaceCache ;	// a hashtable to improve lookup speeds

		/// <summary>
		/// Creates a new instance of the NamespaceMapper class based on the specified map file
		/// </summary>
		/// <param name="path">Path to the map file</param>
		public NamespaceMapper( string path )
		{
			if ( !schemaIsValid )
				throw new Exception( "The namespaceMap schema is not valid or could not be found" );

			if ( !File.Exists( path ) )
				throw new ArgumentException( string.Format( "{0} could not be found", path ), "path" );

			XmlValidatingReader reader = new XmlValidatingReader( new XmlTextReader( path ) );

			try
			{
				reader.Schemas.Add( namespaceMapSchema );
		
				XmlDocument doc = new XmlDocument();
				doc.Load( reader );
				map = doc.DocumentElement;
				Debug.Assert( map != null );
			}
			finally
			{
				reader.Close();
			}

			helpNamespaceCache = new Hashtable();
		}


		/// <summary>
		/// Merges the specified map into this map
		/// </summary>
		/// <param name="mapper">The map to merge into this one</param>
		public void MergeMaps( NamespaceMapper mapper )
		{
			XmlNodeList helpNamespaces = mapper.map.SelectNodes( "//map:helpNamespace", nsmgr );

			// for each help namespace in the new map, merge it into this map
			foreach ( XmlNode node in helpNamespaces )			
				MergeHelpNamespace( node );
		}

		private void MergeHelpNamespace( XmlNode namespaceNode )
		{
			string ns = namespaceNode.Attributes.GetNamedItem( "ns" ).Value;
			
			XmlNode existingNode = map.SelectSingleNode( string.Format( "//map:helpNamespace[ @ns ='{0}' ]", ns ), nsmgr );
			
			// if we've already got the help namespace in this map then merge it with the existing 
			if ( existingNode != null )
				MergeNewToExisting( existingNode, namespaceNode );
			else
				MergeNew( namespaceNode );
		}

		private void MergeNew( XmlNode newHelpNamespace )
		{
			XmlNodeList managedNamespaces = newHelpNamespace.SelectNodes( "map:managedNamespace", nsmgr );

			// find each managed namespace in the new help namespace and
			// and remove it from the existing map if it exists
			foreach ( XmlNode node in managedNamespaces )
			{
				string ns = node.Attributes.GetNamedItem( "ns" ).Value;
				XmlNode existingMNS = map.SelectSingleNode( string.Format( "//map:managedNamespace[ @ns ='{0}' ]", ns ), nsmgr );

				// we only need to add it if it's not already here
				if ( existingMNS != null )				
					existingMNS.ParentNode.RemoveChild( existingMNS );
			}

			// now we can safely append the new help namespace and all of its children
			map.AppendChild( map.OwnerDocument.ImportNode( newHelpNamespace, true ) );
		}

		private void MergeNewToExisting( XmlNode existingHelpNamespace, XmlNode newHelpNamespace )
		{
			XmlNodeList managedNamespaces = newHelpNamespace.SelectNodes( "map:managedNamespace", nsmgr );

			// find each managed namespace in the new help namespace and
			// add it to the existing help namespace
			foreach ( XmlNode node in managedNamespaces )
			{
				string ns = node.Attributes.GetNamedItem( "ns" ).Value;
				XmlNode existingMNS = existingHelpNamespace.SelectSingleNode( string.Format( "map:managedNamespace[ @ns ='{0}' ]", ns ), nsmgr );

				// we only need to add it if it's not already here
				if ( existingMNS == null )				
					existingHelpNamespace.AppendChild( map.OwnerDocument.ImportNode( node, true ) );
			}
		}

		/// <summary>
		/// Saves the map to disk
		/// </summary>
		/// <param name="path">The path and filename to save to</param>
		public void Save( string path )
		{
			map.OwnerDocument.Save( path );
		}

		/// <summary>
		/// Sets the help namespace to use for system types
		/// </summary>
		/// <param name="systemHelpNamespace">The help namespace associates with system types</param>
		public void SetSystemNamespace( string systemHelpNamespace )
		{
			XmlNode systemNode = map.SelectSingleNode( "//map:managedNamespace[ @ns = 'System' ]", nsmgr );
			Debug.Assert( systemNode != null );
			XmlNode helpNSNode = systemNode.SelectSingleNode( "parent::node()/@ns", nsmgr );
			Debug.Assert( helpNSNode != null );
			helpNSNode.Value = systemHelpNamespace;
		}

		/// <summary>
		/// Looks up the html help 2 namespace associated with the specified manage namesapce
		/// </summary>
		/// <param name="managedName">The managed name to query for (case sensitive)</param>
		/// <returns>The best match for the managed namespace or an empty string if none is found</returns>
		public string LookupHelpNamespace( string managedName )
		{
			// first see if this managed name is in the cache
			// if it is not in the cache add it
			if ( !helpNamespaceCache.Contains( managedName ) )
				helpNamespaceCache.Add( managedName, SelectHelpNamesapce( managedName ) );
			
			// return the value from the cache
			return helpNamespaceCache[managedName].ToString();
		}

		private string SelectHelpNamesapce( string managedName )
		{
			string helpNamespace = String.Empty;

			ManagedName name = new ManagedName( managedName );

			// since in most cases all managed names in a namespace will be in the
			// same help collection, let's fisrt try to short circuit the search by seeing
			// if there is a single managedNamespace entry that starts with the root of the name we are looking for
			string xpath = string.Format( "//map:managedNamespace[ starts-with( @ns,'{0}' ) ]", name.RootNamespace );
			XmlNodeList firstTry = map.SelectNodes( xpath, nsmgr );

			// if only one managedNamespace start with the root we can just return its help namespace
			if ( firstTry.Count == 1 )
			{
				helpNamespace = GetNSFromManagedNameNode( firstTry.Item( 0 ) );
			}
			else
			{
				// Since there is more than one managed name that starts with the root  
				// being searched, we have to search for a more specific match.
				// We do this by starting with the most specified name and working backwards to the root
				// e.g. if we are looking for NS1.NS2.N3.NS4 we would start
				// with NS1.NS2.N3.NS4, then NS1.NS2.N3, then NS1.NS2
				//
				// In this way managed namespace documentation can be spread across help namesapces
				string[] s = name.Parts;
				for ( int i = s.Length - 1; i >= 0; i-- )
				{
					string ns = FindMatch( s[i] );
					if ( ns.Length > 0 )
					{
						helpNamespace = ns;
						break;
					}
				}
			}

			return helpNamespace;
		}

		private string GetNSFromManagedNameNode( XmlNode node )
		{
			Debug.Assert( node != null );
			Debug.Assert( node.Name == "managedNamespace" );
			XmlNode helpNSNode = node.SelectSingleNode( "parent::node()/@ns", nsmgr );
			Debug.Assert( helpNSNode != null );
			return helpNSNode.Value;
		}

		private string FindMatch( string match )
		{
			XmlNode matchNode = map.SelectSingleNode( string.Format( "//map:managedNamespace[ @ns='{0}' ]", match ), nsmgr );

			if ( matchNode != null )
				return GetNSFromManagedNameNode( matchNode );

			return string.Empty;
		}

	}
}
