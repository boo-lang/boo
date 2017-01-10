// IDocumenterConfig.cs - interface for all documenter configs
// Copyright (C) 2001  Jason Diamond
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
using System.Xml;

namespace NDoc.Core
{
	/// <summary>Specifies the methods that are common to all documenter configs.</summary>
	public interface IDocumenterConfig
	{
		/// <summary>
		/// Returns information about the config
		/// </summary>
		IDocumenterInfo DocumenterInfo{ get; }

		/// <summary>Associates the config with a project.</summary>
		/// <remarks>Changes to the config will notify the project so that 
		/// it can be marked as modified and saved.</remarks>
		void SetProject(Project project);

		/// <summary>Gets a list of property names.</summary>
		/// <returns>An enumerable list of property names.</returns>
		IEnumerable GetProperties();

		/// <summary>Sets the value of a property.</summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="value">The value of the property.</param>
		void SetValue(string name, string value);

		/// <summary>Reads the previously serialized state of the documenter into memory.</summary>
		/// <param name="reader">An XmlReader positioned on a documenter element.</param>
		/// <remarks>This method uses reflection to set all of the public properties in the documenter.</remarks>
		void Read(XmlReader reader);

		/// <summary>Writes the current state of the documenter to the specified XmlWrtier.</summary>
		/// <param name="writer">An XmlWriter.</param>
		/// <remarks>This method uses reflection to serialize all of the public properties in the documenter.</remarks>
		void Write(XmlWriter writer);

		/// <summary>
		/// Creates an instance of a documenter 
		/// </summary>
		/// <returns>IDocumenter instance</returns>		
		IDocumenter CreateDocumenter();
	}
}
