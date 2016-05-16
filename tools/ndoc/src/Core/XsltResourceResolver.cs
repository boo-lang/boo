// XsltResourceResolver
// Copyright (C) 2004 Kevin Downs
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
using System.Xml;
using System.Reflection;
using System.IO;

namespace NDoc.Core
{
	/// <summary>	
	/// Resolves URLs stored as embedded resources in an assembly.
	/// </summary> 
	/// <remarks>for debugging purposes, it is possible to direct the resolver to look for the resources in a 
	/// disk directory rather than extracting them from the assembly. 
	/// This is especially useful  as it allows the stylesheets to be changed 
	/// and re-run without recompiling the assembly.</remarks>
	public class XsltResourceResolver : XmlUrlResolver
	{
		private string _ExtensibiltyStylesheet;
		private string _ResourceBase;
		private Assembly _Assembly;
		private bool _UseEmbeddedResources;


		/// <summary>
		/// Creates a new instance of the <see cref="XsltResourceResolver"/> class.
		/// </summary>
		/// <param name="resourceBase">Either, the namespace of the embedded resources, or a file URI to a disk directory where the recources may be found.</param>
		public XsltResourceResolver(string resourceBase)
		{
			_ExtensibiltyStylesheet=String.Empty;
			if (resourceBase.StartsWith("file://"))
			{
				_ResourceBase=resourceBase.Substring(7);
				_UseEmbeddedResources=false;
			}
			else
			{
				_ResourceBase = resourceBase;
				_Assembly=Assembly.GetCallingAssembly();
				_UseEmbeddedResources=true;
			}
		}
	
		/// <summary>
		/// User-defined Extensibility Stylesheet
		/// </summary>
		/// <value>fully-qualified filename of exstensibility stylesheet</value>
		public string ExtensibilityStylesheet 
		{
			get 
			{
				if (_ExtensibiltyStylesheet.Length==0) {return String.Empty;}
				if ( Path.IsPathRooted( _ExtensibiltyStylesheet ) )
				{
					return _ExtensibiltyStylesheet;
				}
				else
				{
					return Path.GetFullPath( _ExtensibiltyStylesheet );
				}
			}
			set { _ExtensibiltyStylesheet = value; }
		}
		
		/// <summary>
		/// Resolves the absolute URI from the base and relative URIs.
		/// </summary>
		/// <param name="baseUri">The base URI used to resolve the relative URI.</param>
		/// <param name="relativeUri">The URI to resolve. The URI can be absolute or relative. If absolute, this value effectively replaces the <paramref name="baseUri"/> value. If relative, it combines with the <paramref name="baseUri"/> to make an absolute URI.</param>
		/// <returns>A <see cref="Uri"/> representing the absolute URI or <see langword="null"/> if the relative URI can not be resolved.</returns>
		/// <remarks><paramref name="baseURI"/> is always <see langword="null"/> when this method is called from <see cref="System.Xml.Xsl.XslTransform.Load(System.Xml.XmlReader)">XslTransform.Load</see></remarks>
		public override Uri ResolveUri(Uri baseUri, string relativeUri)
		{
			Uri temp=null;
			if (relativeUri.StartsWith("res:"))
			{
				temp = new Uri(relativeUri);
			}
			else if (relativeUri.StartsWith("user:"))
			{
				if (ExtensibilityStylesheet.Length==0)
				{
					temp = new Uri("res:blank.xslt" );
				}
				else
				{
					temp = base.ResolveUri (baseUri, ExtensibilityStylesheet);
				}
			}
			else if (relativeUri.StartsWith("file:"))
			{
				temp = base.ResolveUri (baseUri, relativeUri);
			}
			else
			{
				temp = new Uri("res:" + relativeUri);
			}
			
			return temp;
		}
	
		/// <summary>
		/// Maps a URI to an object containing the actual resource.
		/// </summary>
		/// <param name="absoluteUri">The URI returned from <see cref="ResolveUri"/>.</param>
		/// <param name="role">unused.</param>
		/// <param name="ofObjectToReturn">The type of object to return. The current implementation only returns <b>System.IO.Stream</b> or <b>System.Xml.XmlReader</b> objects.</param>
		/// <returns></returns>
		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			Stream xsltStream;
			if (absoluteUri.Scheme=="res")
			{
				if (_UseEmbeddedResources)
				{
					string resourceName = _ResourceBase + "." + absoluteUri.AbsolutePath;
					xsltStream=_Assembly.GetManifestResourceStream(resourceName);
				}
				else
				{
					Uri fileUri = new Uri(_ResourceBase + Path.DirectorySeparatorChar + absoluteUri.AbsolutePath);
					xsltStream= base.GetEntity(fileUri, role, Type.GetType("System.IO.Stream")) as Stream;
				}
			}
			else
			{
				xsltStream= base.GetEntity(absoluteUri, role, Type.GetType("System.IO.Stream")) as Stream;
			}

			if (ofObjectToReturn==typeof(XmlReader))
			{
				return new XmlTextReader(xsltStream);
			}
			else
			{
				return xsltStream;
			}

		}
	}
}
