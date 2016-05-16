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

namespace NDoc.Documenter.NativeHtmlHelp2.Engine
{
	/// <summary>
	/// Used as an extension object to the xslt processor to allow
	/// retrieving user-provided raw html.
	/// </summary>
	public class ExternalHtmlProvider
	{
		/// <summary>
		/// Contructs the ExternalHtmlProvider
		/// </summary>
		/// <param name="header">The Html to include in the header</param>
		/// <param name="footer">The Html to include in the footer</param>
		public ExternalHtmlProvider( string header, string footer )
		{
			_headerHtml = header;
			_footerHtml = footer;
		}

		/// <summary>
		/// Set the filename of the current html file being generated
		/// </summary>
		/// <param name="fileName">The name of the HTML file</param>
		public void SetFilename( string fileName )
		{
			_fileName = fileName;
		}

		/// <summary>
		/// Retrieves user-provided raw html to use as page headers.
		/// </summary>
		/// <param name="topicTitle">The title of the current topic.</param>
		/// <returns></returns>
		public string GetHeaderHtml(string topicTitle)
		{
			if ( _headerHtml == null )
				return string.Empty;

			string headerHtml = _headerHtml;
			headerHtml = headerHtml.Replace("%TOPIC_TITLE%", topicTitle);
			headerHtml = headerHtml.Replace("%FILE_NAME%", _fileName);

			return headerHtml;
		}

		/// <summary>
		/// Retrieves user-provided raw html to use as page footers.
		/// </summary>
		/// <param name="assemblyName">The name of the assembly for the current topic.</param>
		/// <param name="assemblyVersion">The version of the assembly for the current topic.</param>
		/// <param name="topicTitle">The title of the current topic.</param>
		/// <returns></returns>
		public string GetFooterHtml(string assemblyName, string assemblyVersion, string topicTitle)
		{
			if ( _footerHtml == null )
				return string.Empty;

			string footerHtml = _footerHtml;
			footerHtml = footerHtml.Replace("%ASSEMBLY_NAME%", assemblyName);
			footerHtml = footerHtml.Replace("%ASSEMBLY_VERSION%", assemblyVersion);
			footerHtml = footerHtml.Replace("%TOPIC_TITLE%", topicTitle);
			footerHtml = footerHtml.Replace("%FILE_NAME%", _fileName);

			return footerHtml;
		}

		private string _footerHtml;
		private string _headerHtml;

		private string _fileName;
	}
}
