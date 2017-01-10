// Copyright (C) 2004  Kevin Downs
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
using System.Collections;
using System.Diagnostics;
using System.Xml;
using System.Text;
using System.IO;

namespace NDoc.Core.Reflection
{
	/// <summary>
	/// AssemblyXmlDocCache.
	/// </summary>
	internal class AssemblyXmlDocCache
	{
		private Hashtable docs;
		private Hashtable excludeTags;

		/// <summary>
		/// Creates a new instance of the <see cref="AssemblyXmlDocCache"/> class.
		/// </summary>
		public AssemblyXmlDocCache()
		{
			Flush();
		}

		/// <summary>
		/// Flushes the Cache.
		/// </summary>
		public void Flush()
		{
			docs = new Hashtable();
			excludeTags = new Hashtable();
		}


		/// <summary>
		/// Populates cache from the given file.
		/// </summary>
		/// <param name="fileName">Fully-qualified filename of xml file with which to populate the cache.</param>
		public void CacheDocFile(string fileName)
		{
			XmlTextReader reader = new XmlTextReader(fileName);
			reader.WhitespaceHandling=WhitespaceHandling.All;
			CacheDocs(reader);
		}
		
		
		/// <summary>
		/// Cache the xmld docs into a hashtable for faster access.
		/// </summary>
		/// <param name="reader">An XMLTextReader containg the docs the cache</param>
		private void CacheDocs(XmlTextReader reader)
		{
			object oMember = reader.NameTable.Add("member");
			reader.MoveToContent();

			while (!reader.EOF) 
			{
				if ((reader.NodeType == XmlNodeType.Element) && (reader.Name.Equals(oMember)))
				{
					string ID = reader.GetAttribute("name");
					string doc = reader.ReadInnerXml().Trim();
					doc = PreprocessDoc(ID, doc);
					if (docs.ContainsKey(ID))
					{
						Trace.WriteLine("Warning: Multiple <member> tags found with id=\"" + ID + "\"");
					}
					else
					{
						docs.Add(ID, doc);
					}
				}
				else
				{
					reader.Read();
				}
			}
		}

		/// <summary>
		/// Preprocess documentation before placing it in the cache.
		/// </summary>
		/// <param name="id">Member name 'id' to which the docs belong</param>
		/// <param name="doc">A string containing the members documentation</param>
		/// <returns>processed doc string</returns>
		private string PreprocessDoc(string id, string doc)
		{
			//create an XmlDocument containg the memeber's documentation
			XmlTextReader reader=new XmlTextReader(new StringReader("<root>" + doc + "</root>"));
			reader.WhitespaceHandling=WhitespaceHandling.All;
			
			XmlDocument xmldoc = new XmlDocument();
			xmldoc.PreserveWhitespace=true;
			xmldoc.Load(reader);
 
			//
			CleanupNodes(id, xmldoc.DocumentElement.ChildNodes);
			//
			ProcessSeeLinks(id, xmldoc.DocumentElement.ChildNodes);
			return xmldoc.DocumentElement.InnerXml;
		}

		/// <summary>
		/// strip out redundant newlines and spaces from documentation.
		/// </summary>
		/// <param name="id">member</param>
		/// <param name="nodes">list of nodes</param>
		private void CleanupNodes(string id, XmlNodeList nodes)
		{
			foreach (XmlNode node in nodes)
			{
				if (node.NodeType == XmlNodeType.Element) 
				{
					if (node.Name == "exclude")
					{
						excludeTags.Add(id, null);
					}
					
					if (node.Name == "code")
					{
						FixupCodeTag(node);
					}
					else
					{
						CleanupNodes(id, node.ChildNodes);
					}

					// Trim attribute values...
					foreach(XmlNode attr in node.Attributes)
					{
						attr.Value=attr.Value.Trim();
					}
				}
				if (node.NodeType == XmlNodeType.Text)
				{
					node.Value = ((string)node.Value).Replace("\t", "    ").Replace("\n", " ").Replace("\r", " ").Replace("        ", " ").Replace("    ", " ").Replace("   ", " ").Replace("  ", " ");
				}
			}
		}

		/// <summary>
		/// Remove leading spaces from code tag contents.
		/// </summary>
		/// <param name="node">a code tag node</param>
		private void FixupCodeTag(XmlNode node)
		{
			string codeText = (string)node.InnerXml;
			if (codeText.TrimStart(new Char[] {' '}).StartsWith("\r\n"))
			{
				codeText = codeText.TrimStart(new Char[] {' '}).Substring(2);
			}
			codeText = codeText.Replace("\r\n", "\n");
			codeText = codeText.Replace("\t", "    ");
			string[] codeLines = codeText.Split(new Char[] {'\r', '\n'});
			if (codeLines.Length > 0)
			{
				int numberOfCharsToRemove = int.MaxValue;
				for (int index = 0; index < codeLines.Length; index++)
				{
					string testLine = codeLines[index];
					int leadingWhitespaceChars = 0; //number of chars at start of line
					while (leadingWhitespaceChars < testLine.Length && testLine.Substring(leadingWhitespaceChars, 1) == " ")
					{
						leadingWhitespaceChars++;
					}
					if (numberOfCharsToRemove > leadingWhitespaceChars)
					{
						numberOfCharsToRemove = leadingWhitespaceChars;
					}
				}

				if (numberOfCharsToRemove < int.MaxValue && numberOfCharsToRemove > 0)
				{

					for (int index = 0; index < codeLines.Length; index++)
					{
						if (numberOfCharsToRemove < codeLines[index].Length)
							codeLines[index] = codeLines[index].Substring(numberOfCharsToRemove);
						else
							codeLines[index] = codeLines[index].TrimStart();
					}
				}

				string newtext = String.Join(System.Environment.NewLine, codeLines);

				XmlAttribute escaped = node.Attributes["escaped"];
				if (escaped!=null && escaped.Value=="true")
				{
					node.InnerText = newtext;
				}
				else
				{
					node.InnerXml = newtext;
				}
			}

		}

		/// <summary>
		/// Add 'nolink' attributes to self referencing or duplicate see tags.
		/// </summary>
		/// <param name="id">current member name 'id'</param>
		/// <param name="nodes">list of top-level nodes</param>
		/// <remarks>
		/// </remarks>
		private void ProcessSeeLinks(string id, XmlNodeList nodes)
		{
			foreach (XmlNode node in nodes)
			{
				if (node.NodeType == XmlNodeType.Element) 
				{
					Hashtable linkTable=null;
					MarkupSeeLinks(ref linkTable, id, node);
				}
			}
		}

		/// <summary>
		/// Search tags for duplicate or self-referencing see links.
		/// </summary>
		/// <param name="linkTable">A table of previous links.</param>
		/// <param name="id">current member name 'id'</param>
		/// <param name="node">an Xml Node containing a doc tag</param>
		private void MarkupSeeLinks(ref Hashtable linkTable, string id, XmlNode node)
		{
			if (node.LocalName=="see")
			{
				//we will only do this for crefs
				XmlAttribute cref = node.Attributes["cref"];
				if (cref !=null)
				{
					if (cref.Value==id) //self referencing tag
					{
						XmlAttribute dup = node.OwnerDocument.CreateAttribute("nolink");
						dup.Value="true";
						node.Attributes.Append(dup);
					}
					else
					{
						if (linkTable==null)
						{
							//assume an resonable initial table size,
							//so we don't have to resize often.
							linkTable = new Hashtable(16);
						}
						if (linkTable.ContainsKey(cref.Value))
						{
							XmlAttribute dup = node.OwnerDocument.CreateAttribute("nolink");
							dup.Value="true";
							node.Attributes.Append(dup);
						}
						else
						{
							linkTable.Add(cref.Value,null);
						}
					}
				}
			}
				
			//search this tags' children
			foreach (XmlNode childnode in node.ChildNodes)
			{
				if (childnode.NodeType == XmlNodeType.Element) 
				{
					MarkupSeeLinks(ref linkTable, id, childnode);
				}
			}
		}


		/// <summary>
		/// Gets Xml documentation for the given ID
		/// </summary>
		/// <param name="memberId">The ID of the item for which documentation is required</param>
		/// <returns>a string containg the Xml documentation</returns>
		public string GetDoc(string memberId)
		{
			return (string)docs[memberId];
		}

		/// <summary>
		/// Returns whether a member has an exclude tag
		/// </summary>
		/// <param name="memberId">ID to check</param>
		/// <returns>true if the member has an exclude tag, otherwise false</returns>
		public bool HasExcludeTag(string memberId)
		{
			return excludeTags.ContainsKey(memberId);
		}
	}
}
