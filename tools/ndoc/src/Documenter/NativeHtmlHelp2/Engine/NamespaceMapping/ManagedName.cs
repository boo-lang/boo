using System;
using System.Collections;
using System.Diagnostics;

namespace NDoc.Documenter.NativeHtmlHelp2.Engine.NamespaceMapping
{
	/// <summary>
	/// The ManagedName class represents a type, member or namespace name
	/// </summary>
	public class ManagedName
	{
		string _ns;

		/// <summary>
		/// Construct a new instnace of the class
		/// </summary>
		/// <param name="ns"><see cref="System.String"/> representing the name</param>
		public ManagedName( string ns )
		{
			// strip off any NDoc type prefix
			int colonPos = ns.IndexOf( ':' );
			if ( colonPos > -1 )
				_ns = ns.Substring( colonPos + 1 );
			else
				_ns = ns;
		}

		/// <summary>
		/// The root namespace of the name
		/// </summary>
		/// <example>For System.Object, returns System</example>
		public string RootNamespace
		{
			get
			{
				int firstDot = _ns.IndexOf( '.' );
				if ( firstDot > -1 )
					return _ns.Substring( 0, firstDot );

				return _ns;
			}
		}

		/// <summary>
		/// Returns a string array with each part of the name decomposed from least to most qualified
		/// </summary>
		/// <example>
		/// For the name System.Runtime.InteropServices
		/// Parts[0] = "System"
		/// Parts[1] = "System.Runtime"
		/// Parts[2] = "System.Runtime.InteropServices
		/// </example>
		public string[] Parts
		{
			get
			{
				string[] s = _ns.Split( new char[]{ '.' } );
				
				Debug.Assert( s.Length > 0 );
				string[] parts = new string[ s.Length ];

				parts[0] = s[0];
				for ( int i = 1; i < s.Length; i++ )
					parts[i] = string.Format( "{0}.{1}", parts[i-1], s[i] );
				
				return parts;
			}
		}

		/// <summary>
		/// Returns the fully specified managed name
		/// </summary>
		/// <returns>String representation of the name</returns>
		public override string ToString()
		{
			return _ns;
		}
	}
}
