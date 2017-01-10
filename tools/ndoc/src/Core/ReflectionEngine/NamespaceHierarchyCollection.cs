// NamespaceHierarchyCollection.cs
// Copyright (C) 2005  Kevin Downs
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

namespace NDoc.Core.Reflection
{
	/// <summary>
	/// 
	/// </summary>
	internal class NamespaceHierarchyCollection
	{
		private Hashtable namespaces;
        
		/// <summary>
		/// 
		/// </summary>
		public NamespaceHierarchyCollection()
		{
			namespaces = new Hashtable(15);
		}
        
		/// <summary>
		/// 
		/// </summary>
		/// <param name="namespaceName"></param>
		/// <param name="baseType"></param>
		/// <param name="derivedType"></param>
		public void Add(string namespaceName, Type baseType, Type derivedType)
		{
			TypeHierarchy derivedTypesCollection = namespaces[namespaceName] as TypeHierarchy;
			if (derivedTypesCollection == null)
			{
				derivedTypesCollection = new TypeHierarchy();
				namespaces.Add(namespaceName, derivedTypesCollection);
			}
			derivedTypesCollection.Add(baseType, derivedType);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="namespaceName"></param>
		/// <returns></returns>
		public TypeHierarchy GetDerivedTypesCollection(string namespaceName)
		{
			TypeHierarchy derivedTypesCollection = namespaces[namespaceName] as TypeHierarchy;
			return derivedTypesCollection;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public ICollection DefinedNamespaces
		{
			get
			{
				return namespaces.Keys;
			}
		}
	}
}
