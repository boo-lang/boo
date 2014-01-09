// StyleSheetCollection
// Copyright (C) 2004 Kevin Downs
// Parts Copyright (c) 2003 Don Kackman
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
using System.Text;
using System.IO;
using System.Collections;
using System.Xml;
using System.Xml.Xsl;
using System.Diagnostics;
using System.Reflection;

using NDoc.Core;

namespace NDoc.Documenter.Intellisense
{
	/// <summary>
	/// The collection of xslt stylesheets used to generate the Html
	/// </summary>
	internal class StyleSheetCollection : DictionaryBase
	{
		/// <summary>
		/// Load the predefined set of xslt stylesheets into a dictionary
		/// </summary>
		/// <param name="extensibilityStylesheet"></param>
		/// <returns>The populated collection</returns>
		public static StyleSheetCollection LoadStyleSheets(string extensibilityStylesheet)
		{
			StyleSheetCollection stylesheets = new StyleSheetCollection();

#if NO_RESOURCES
			string resourceBase = "file://" + Path.GetFullPath(Path.Combine(System.Windows.Forms.Application.StartupPath, @"..\..\..\Documenter\Intellisense\xslt"));
#else
			string resourceBase = "NDoc.Documenter.Intellisense.xslt";
#endif

			XsltResourceResolver resolver = new XsltResourceResolver(resourceBase);
			resolver.ExtensibilityStylesheet = extensibilityStylesheet;
			Trace.Indent();

			stylesheets.AddFrom("assembly", resolver);

			Trace.Unindent();

			return stylesheets;
		}


		private StyleSheetCollection()
		{
		}

		/// <summary>
		/// Return a named stylesheet from the collection
		/// </summary>
		public XslTransform this[string name]
		{
			get
			{
				Debug.Assert(base.InnerHashtable.Contains(name));
				return (XslTransform)base.InnerHashtable[name];
			}
		}

		private void AddFrom(string name, XsltResourceResolver resolver)
		{
			base.InnerHashtable.Add(name, MakeTransform(name, resolver));
		}

		private static XslTransform MakeTransform(string name, XsltResourceResolver resolver)
		{
			try
			{
				Trace.WriteLine(name + ".xslt");
				XslTransform transform = new XslTransform();
				XmlReader reader = (XmlReader)resolver.GetEntity(new Uri("res:" + name + ".xslt"), null, typeof(XmlReader));
#if (NET_1_0)
				transform.Load(reader, resolver);
#else
				transform.Load(reader, resolver, Assembly.GetExecutingAssembly().Evidence);
#endif
				return transform;
			}
			catch (Exception e)
			{
				throw new Exception(string.Format("Error compiling the {0} stylesheet", name), e);
			}
		}

	}
}
