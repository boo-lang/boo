// TypeHierarchy.cs
// Copyright (C) 2005 Kevin Downs
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
using System.Text;

namespace NDoc.Core.Reflection
{
	/// <summary>
	/// 
	/// </summary>
	internal class TypeHierarchy
	{
		/// <summary>
		/// 
		/// </summary>
		public TypeHierarchy()
		{
			data = new Hashtable(7);
		}

		private Hashtable data;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseType">Base type.</param>
		/// <param name="derivedType">Derived type.</param>
		public void Add(Type baseType, Type derivedType)
		{
			string baseTypeMemberID = MemberID.GetMemberID(baseType);
			ArrayList derivedTypeList = data[baseTypeMemberID] as ArrayList;
			if (derivedTypeList == null)
			{
				derivedTypeList = new ArrayList();
				data.Add(baseTypeMemberID, derivedTypeList);
			}
			//            if (!derivedTypeList.Contains(derivedType))
			//            {
			//            }
#if NET_2_0
			if (derivedType.IsGenericType) derivedType = derivedType.GetGenericTypeDefinition();
#endif

			bool found = false;
			for (int i = 0; i < derivedTypeList.Count; i++)
			{
				if (((Type)derivedTypeList[i]).AssemblyQualifiedName == derivedType.AssemblyQualifiedName)
				{
					found = true;
					break;
				}
			}
			if (!found)
				derivedTypeList.Add(derivedType);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseType">Base type.</param>
		/// <returns></returns>
		public ArrayList GetDerivedTypes(Type baseType)
		{
			string baseTypeMemberID = MemberID.GetMemberID(baseType);
			ArrayList derivedTypeList = data[baseTypeMemberID] as ArrayList;
			if (derivedTypeList == null)
			{
				derivedTypeList = new ArrayList();
			}
			else
			{
				derivedTypeList.Sort(new TypeSorter());
			}
			return derivedTypeList;
		}

		private class TypeSorter : IComparer
		{
			public int Compare(object x, object y)
			{
				return String.Compare(MemberID.GetMemberID((Type)x), MemberID.GetMemberID((Type)y));
			}
		}
	}
}
