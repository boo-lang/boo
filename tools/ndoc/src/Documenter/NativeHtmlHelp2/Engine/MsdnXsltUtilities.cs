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
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using System.Collections;

using NDoc.Documenter.NativeHtmlHelp2.Engine.NamespaceMapping;

namespace NDoc.Documenter.NativeHtmlHelp2.Engine
{
	/// <summary>
	/// Provides an extension object for the xslt transformations.
	/// </summary>
	public class MsdnXsltUtilities
	{
		private Hashtable descriptions;

		private Hashtable aIndexCache;

		private NamespaceMapper nsMapper;
		private FileNameMapper _fileMapper;

		/// <summary>
		/// Initializes a new instance of class MsdnXsltUtilities
		/// </summary>
		/// <param name="namespaceMapper">The namespace mapper used to look up XLink help namespace for foreign types</param>	
		/// <param name="fileMapper">The mapper used to look up local filenames</param>	
		public MsdnXsltUtilities(NamespaceMapper namespaceMapper, FileNameMapper fileMapper)
		{
			if (namespaceMapper == null)
			{
				throw new ArgumentNullException("namespaceMapper");
			}
			if (fileMapper == null)
			{
				throw new ArgumentNullException("fileMapper");
			}

			ResetDescriptions();
			nsMapper = namespaceMapper;
			_fileMapper = fileMapper;
			aIndexCache = new Hashtable();
		}

		/// <summary>
		/// Resets the descriptions collections
		/// </summary>
		public void ResetDescriptions()
		{
			descriptions = new Hashtable();
		}
#if MONO
		/// <summary>
		/// Returns an HRef for a CRef.
		/// </summary>
		/// <param name="list">The argument list containing the 
		/// cRef for which the HRef will be looked up.</param>
		/// <remarks>Mono needs this overload, as its XsltTransform can only
		/// call methods with an ArraList parameter.</remarks>
		public string GetHRef(System.Collections.ArrayList list)
		{
			string cref = (string)list[0];
			return GetHRef(cref);
		}
#endif
		/// <summary>
		/// Gets the href for a namespace topic
		/// </summary>
		/// <param name="namespaceName">The namespace name</param>
		/// <returns>Relative HRef to the Topic</returns>
		public string GetNamespaceHRef(string namespaceName)
		{
			return FileNameMapper.GetFilenameForNamespace(namespaceName);
		}

		/// <summary>
		/// Gets the Href for the namespace hierarchy topic
		/// </summary>
		/// <param name="namespaceName">The namespace name</param>
		/// <returns>Relative HRef to the Topic</returns>
		public string GetNamespaceHierarchyHRef(string namespaceName)
		{
			return FileNameMapper.GetFileNameForNamespaceHierarchy(namespaceName);
		}

		/// <summary>
		/// Gets the Href for the type hierarchy topic
		/// </summary>
		/// <param name="typeName">The type name</param>
		/// <returns>Relative HRef to the Topic</returns>
		public string GetTypeHierarchyHRef(string typeName)
		{
			return FileNameMapper.GetFileNameForTypeHierarchy(typeName);
		}

		/// <summary>
		/// Gets the href for an overview page
		/// </summary>
		/// <param name="typeID">The id of the type</param>
		/// <param name="pageType">The type of overview page to generate</param>
		/// <returns>Relative HRef to the Topic</returns>
		public string GetOverviewHRef(string typeID, string pageType)
		{
			return FileNameMapper.GetFilenameForOverviewPage(typeID, pageType);
		}

		/// <summary>
		/// Gets the href for a constructor
		/// </summary>
		/// <param name="xPathNode">The node selection for the contsructor</param>
		/// <returns>Relative HRef to the Topic</returns>
		public string GetConstructorHRef(XPathNodeIterator xPathNode)
		{
			string href = "";
			xPathNode.MoveNext();

			if (xPathNode.Current != null) //&& xPathNode.Current is IHasXmlNode )
			{
				XPathNavigator navigator = xPathNode.Current.Clone();
				
				string id = GetAttributeValue(navigator, "id");
				bool isStatic = GetAttributeValue(navigator, "contract") == "Static";
				string overload = GetAttributeValue(navigator, "overload");

				if (overload.Length > 0)				
					href = FileNameMapper.GetFilenameForConstructor(id, isStatic, overload);
				
				else
					href = FileNameMapper.GetFilenameForConstructor(id, isStatic);

			}

			return href;
		}

		/// <summary>
		/// Get the href for a member overloads topic
		/// </summary>
		/// <param name="typeID">The id of the type</param>
		/// <param name="methodName">The name of the method</param>
		/// <returns>Relative HRef to the Topic</returns>
		public string GetMemberOverloadsHRef(string typeID, string methodName)
		{
			return FileNameMapper.GetFilenameForMethodOverloads(typeID, methodName);
		}

		/// <summary>
		/// Get the href for a member topic
		/// </summary>
		/// <param name="xPathNode">The member selection</param>
		/// <returns>Relative HRef to the Topic</returns>			
		public string GetMemberHRef(XPathNodeIterator xPathNode)
		{
			xPathNode.MoveNext();

			if (xPathNode.Current != null) 
			{
				XPathNavigator navigator = xPathNode.Current.Clone();
				return GetMemberOverloadsHRef(GetAttributeValue(navigator, "id"), navigator);
			}

			return "";
		}

		/// <summary>
		/// Returns the topic href for a specific implementation of an inherited member
		/// </summary>
		/// <param name="targetMemberID">The id of the source member</param>
		/// <param name="xPathNode">The current member being processed</param>
		/// <returns>HRef of the inherited member topic</returns>
		public string GetInheritedMemberOverloadHRef(string targetMemberID, XPathNodeIterator xPathNode)
		{
			xPathNode.MoveNext();

			if (xPathNode.Current != null) 
				return GetMemberOverloadHRef(targetMemberID, xPathNode.Current.Clone());

			return "";
		}
		
		/// <summary>
		/// Returns the topic href for a specific implementation of a member
		/// </summary>
		/// <param name="xPathNode">The member being processed</param>
		/// <returns>HRef of the member topic</returns>
		public string GetMemberOverloadHRef(XPathNodeIterator xPathNode)
		{
			xPathNode.MoveNext();

			if (xPathNode.Current != null) 
			{
				XPathNavigator navigator = xPathNode.Current.Clone();
				return GetMemberOverloadHRef(GetAttributeValue(navigator, "id"), navigator);
			}

			return "";
		}

		/// <summary>
		/// Returns the topic href for an inherited member
		/// </summary>
		/// <param name="targetMemberID">The id of the source member</param>
		/// <param name="xPathNode">The current member being processed</param>
		/// <returns>HRef of the inherited member topic</returns>
		public string GetInheritedMemberHRef(string targetMemberID, XPathNodeIterator xPathNode)
		{
			xPathNode.MoveNext();

			if (xPathNode.Current != null) 
				return GetMemberOverloadsHRef(targetMemberID, xPathNode.Current.Clone());

			return "";
		}

		private string GetMemberOverloadsHRef(string targetMemberId, XPathNavigator navigator)
		{
			if (targetMemberId.Length > 0)
			{
				switch (navigator.Name)
				{
					case "field" : 
						return GetFieldHRef(targetMemberId);
					case "event" : 
						return GetEventHRef(targetMemberId);
					case "method" : 
					case "property" : 
					case "operator" : 
						return FileNameMapper.GetFileNameForMemberOverload(targetMemberId, "");
					case "constructor" : 
						return FileNameMapper.GetFilenameForConstructor(targetMemberId, false, "");
					default : 
						return String.Empty;
				}
			}

			return "";
		}

		private string GetMemberOverloadHRef(string targetMemberId, XPathNavigator navigator)
		{
			if (targetMemberId.Length > 0)
			{
				switch (navigator.Name)
				{
					case "field" : 
						return GetFieldHRef(targetMemberId);
					case "event" : 
						return GetEventHRef(targetMemberId);
					case "method" : 
					case "property" : 
					case "operator" : 
						return FileNameMapper.GetFileNameForMemberOverload(targetMemberId, GetAttributeValue(navigator, "overload"));
					case "constructor" : 
						return FileNameMapper.GetFilenameForConstructor(targetMemberId, false, GetAttributeValue(navigator, "overload"));
					default : 
						return String.Empty;
				}
			}

			return "";
		}

		/// <summary>
		/// Get the HRef for a local method topic
		/// </summary>
		/// <param name="typeID">The id of the containing type</param>
		/// <param name="memberName"></param>
		/// <returns>Relative HRef to the Topic</returns>			
		public string GetMethodHRef(string typeID, string memberName)
		{
			return FileNameMapper.GetFilenameForMethodOverloads(typeID, memberName);
		}

		/// <summary>
		/// Get the HRef for a local property topic
		/// </summary>
		/// <param name="typeID">The id of the containing type</param>
		/// <param name="propertyName">The property name</param>
		/// <returns>Relative HRef to the Topic</returns>			
		public string GetPropertyHRef(string typeID, string propertyName)
		{
			return FileNameMapper.GetFilenameForPropertyOverloads(typeID, propertyName);
		}

		/// <summary>
		/// Get the HRef for a local field topic
		/// </summary>
		/// <param name="fieldID">The ID of the field</param>
		/// <returns>Relative HRef to the Topic</returns>			
		public string GetFieldHRef(string fieldID)
		{
			return FileNameMapper.GetFilenameForField(fieldID);
		}

		/// <summary>
		/// Get the HRef for a local event topic
		/// </summary>
		/// <param name="eventID">The ID of the event</param>
		/// <returns>Relative HRef to the Topic</returns>			
		public string GetEventHRef(string eventID)
		{
			return FileNameMapper.GetFilenameForEvent(eventID);
		}

		/// <summary>
		/// Gets the href for a local type topic
		/// </summary>
		/// <param name="typeID">The id of the type</param>
		/// <returns>Relative HRef to the Topic</returns>
		public string GetTypeHRef(string typeID)
		{
			return FileNameMapper.GetFilenameForType(typeID);
		}

		/// <summary>
		/// Returns an HRef for a CRef. This may be local or system
		/// </summary>
		/// <param name="cref">The local html filename for local topics or the assocaitave index for system topics</param>
		public string GetLocalHRef(string cref)
		{
			// if it's not a type string return nothing
			if (cref == null || cref.Length <= 2 || cref[1] != ':')
			{
				Trace.WriteLine("[WARNING] MsdnXsltUtilities.GetLocalHRef : Malformed cref found(" + cref + ")");
				return String.Empty;
			}

			string memberName = string.Empty;
			string typeID = String.Empty;
			int lastDot = -1;

			switch (cref.Substring(0, 2))
			{
				case "N:" : 
					return GetNamespaceHRef(cref.Substring(2));
				case "T:" : 
					return GetTypeHRef(cref);
				case "F:" : 
					return GetFieldHRef(cref);
				case "E:" : 
					return GetEventHRef(cref);
				case "P:" : 
					lastDot = cref.LastIndexOf('.');
					if ((lastDot > -1) && (lastDot < cref.Length))
					{
						memberName = cref.Substring(lastDot + 1);
						typeID = cref.Substring(0, lastDot);
						return GetPropertyHRef(typeID, memberName);
					}
					else
					{
						Trace.WriteLine("[WARNING] MsdnXsltUtilities.GetLocalHRef : Malformed cref found(" + cref + ")");
						return String.Empty;
					}
				case "M:" : 
					lastDot = cref.LastIndexOf('.');
					if ((lastDot > -1) && (lastDot < cref.Length))
					{
						memberName = cref.Substring(lastDot + 1);
						typeID = cref.Substring(0, lastDot);
						return GetMethodHRef(typeID, memberName);
					}
					else
					{
						Trace.WriteLine("[WARNING] MsdnXsltUtilities.GetLocalHRef : Malformed cref found(" + cref + ")");
						return String.Empty;
					}
				default : 
					Trace.WriteLine("[WARNING] MsdnXsltUtilities.GetLocalHRef : Malformed cref found(" + cref + ")");
					return String.Empty;
			}
		}

		/// <summary>
		/// gets the filename for a local cref 
		/// </summary>
		/// <param name="cref">The cref to link to</param>
		/// <returns>a filename</returns>
		public string GetLocalCRef(string cref)
		{
			return _fileMapper[cref];
		}

		/// <summary>
		/// Determines the associative index for a cref
		/// </summary>
		/// <param name="cref">The cref to link to</param>
		/// <returns>The associative index</returns>
		public string GetAIndex(string cref)
		{
			// if it's not a type string return nothing
			if ((cref.Length <= 2) || (cref[1] != ':'))
			{
				Trace.WriteLine("[WARNING] MsdnXsltUtilities.GetAIndex : Malformed cref found(" + cref + ")");
				return String.Empty;
			}

			string aindex = (string)aIndexCache[cref];

			if (aindex != null && aindex.Length > 0)
				return aindex;

			ManagedName name = new ManagedName(cref);

			// if the cref is from the system or microsoft namespace generate a MS AIndex
			if (name.RootNamespace == "System" || name.RootNamespace == "Microsoft")
				aindex = GetSystemAIndex(cref);
				// otherwise we're going to assume that the foreign type was documented with NDoc
				// and generate an NDoc AIndex
			else
				aindex = GetNDocAIndex(cref);

			aIndexCache[cref] = aindex;
			return aindex;
		}

		private string GetNDocAIndex(string cref)
		{
			string fileName = GetLocalHRef(cref);
			return fileName.Replace(".html", "");
		}

		private string GetSystemAIndex(string cref)
		{
			switch (cref.Substring(0, 2))
			{
				case "N:" : // Namespace
					return "frlrf" + cref.Substring(2).Replace(".", "");
				case "T:" : // Type: class, interface, struct, enum, delegate
					return "frlrf" + cref.Substring(2).Replace(".", "").Replace("*", "") + "ClassTopic";
				case "F:" : // Field
				case "P:" : // Property
				case "M:" : // Method
				case "E:" : // Event
					return GetAIndexForSystemMember(cref);
				default : 
				{
					Trace.WriteLine("[WARNING] MsdnXsltUtilities.GetNDocAIndex : Malformed cref found(" + cref + ")");
					return String.Empty;
				}
			}
		}

		/// <summary>
		/// Finds the help namespace most closely mapped to the managed name
		/// </summary>
		/// <param name="managedName">The managed name to look up. This can be a namespace, type or member</param>
		/// <returns>The help namespace or empty string if no match is found</returns>
		public string GetHelpNamespace(string managedName)
		{
			int index = managedName.IndexOf(':');
			if (index > -1)
				managedName = managedName.Substring(index + 1);

			return nsMapper.LookupHelpNamespace(managedName);
		}


#if MONO
		/// <summary>
		/// Returns a name for a CRef.
		/// </summary>
		/// <param name="list">The argument list containing the 
		/// cRef for which the HRef will be looked up.</param>
		/// <remarks>Mono needs this overload, as its XsltTransform can only
		/// call methods with an ArraList parameter.</remarks>
		public string GetName(System.Collections.ArrayList list)
		{
			string cref = (string)list[0];
			return GetName(cref);
		}
#endif


		/// <summary>
		/// Returns a name for a CRef.
		/// </summary>
		/// <param name="cref">CRef for which the name will be looked up.</param>
		public string GetName(string cref)
		{
			int index;
			if (cref.Length < 2)
				return cref;

			if (cref[1] == ':')
			{
				if ((index = cref.IndexOf(".#c")) >= 0)
					cref = cref.Substring(2, index - 2);
				else if ((index = cref.IndexOf("(")) >= 0)
					cref = cref.Substring(2, index - 2);
				else
					cref = cref.Substring(2);
			}

			index = cref.LastIndexOf(".");
			if ((index > -1) && (index < cref.Length))
			{
				return cref.Substring(index + 1);
			}
			else
			{
				Trace.WriteLine("[WARNING] MsdnXsltUtilities.GetName : Malformed cref found(" + cref + ")");
				return cref;
			}
		}

		private string GetAIndexForSystemMember(string cref)
		{
			string crefName;
			int index;

			if ((index = cref.IndexOf(".#c")) >= 0)
				crefName = cref.Substring(2, index - 2) + ".ctor";
			else if ((index = cref.IndexOf("(")) >= 0)
				crefName = cref.Substring(2, index - 2);
			else
				crefName = cref.Substring(2);

			index = crefName.LastIndexOf(".");
			if ((index > -1) && (index < crefName.Length))
			{
				string crefType = crefName.Substring(0, index);
				string crefMember = crefName.Substring(index + 1);
				return "frlrf" + crefType.Replace(".", "") + "Class" + crefMember + "Topic";
			}
			else
			{
				Trace.WriteLine("[WARNING] MsdnXsltUtilities.GetAIndexForSystemMember : Malformed cref found(" + cref + ")");
				return String.Empty;
			}
		}

		/// <summary>
		/// Looks up, whether a member has similar overloads, that have already been documented.
		/// </summary>
		/// <param name="description">A string describing this overload.</param>
		/// <returns>true, if there has been a member with the same description.</returns>
		/// <remarks>
		/// <para>On the members pages overloads are cumulated. Instead of adding all overloads
		/// to the members page, a link is added to the members page, that points
		/// to an overloads page.</para>
		/// <para>If for example one overload is public, while another one is protected,
		/// we want both to appear on the members page. This is to make the search
		/// for suitable members easier.</para>
		/// <para>This leads us to the similarity of overloads. Two overloads are considered
		/// similar, if they have the same name, declaring type, access (public, protected, ...)
		/// and contract (static, non static). The description contains these four attributes
		/// of the member. This means, that two members are similar, when they have the same
		/// description.</para>
		/// <para>Asking for the first time, if a member has similar overloads, will return false.
		/// After that, if asking with the same description again, it will return true, so
		/// the overload does not need to be added to the members page.</para>
		/// </remarks>
		public bool HasSimilarOverloads(string description)
		{
			if (descriptions.Contains(description))
				return true;

			descriptions.Add(description, description);
			return false;
		}

		/// <summary>
		/// Exposes <see cref="System.String.Replace"/> to XSLT
		/// </summary>
		/// <param name="str">The string to search</param>
		/// <param name="oldValue">The string to search for</param>
		/// <param name="newValue">The string to replace</param>
		/// <returns>A new string</returns>
		public string Replace(string str, string oldValue, string newValue)
		{
			return str.Replace(oldValue, newValue);
		}	

		private string GetAttributeValue(XPathNavigator navigator, string name)
		{
			string val = "";
			if (navigator.MoveToAttribute(name, ""))
			{
				val = navigator.Value;
				navigator.MoveToParent();
			}
			return val;
		}	
	}
}
