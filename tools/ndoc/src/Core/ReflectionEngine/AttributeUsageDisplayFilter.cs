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
	internal class AttributeUsageDisplayFilter
	{
		private ArrayList AttributeFilters = new ArrayList();

		public AttributeUsageDisplayFilter(string documentedAttributesConfig)
		{
			string[] filters = documentedAttributesConfig.Split(new char[] {'|'});
			foreach (string filter in filters)
			{
				string[] filterParts = filter.Split(new char[] {','});
				if (filterParts.Length > 0)
				{
					if (filterParts[0].Length > 0)
					{
						DisplayFilter displayFilter = new DisplayFilter();
						displayFilter.attributeName = filterParts[0];
						if (filterParts.Length > 1)
						{
							string[] memberNames = new string[filterParts.Length - 1];
							for (int i = 1; i < filterParts.Length; i++)
							{
								memberNames[i - 1] = filterParts[i];
							}
							displayFilter.memberNames = memberNames;
						}
						AttributeFilters.Add(displayFilter);
					}
				}
			}
		}

		public bool Show(string attributeFullName)
		{
			if (AttributeFilters.Count == 0) return true;
			foreach (DisplayFilter filter in AttributeFilters)
			{
				if (attributeFullName.IndexOf(filter.attributeName) > -1) return true;
			}
			return false;
		}

		public bool Show(string attributeFullName, string memberName)
		{
			if (AttributeFilters.Count == 0) return true;
			foreach (DisplayFilter filter in AttributeFilters)
			{
				if (attributeFullName.IndexOf(filter.attributeName) > -1)
				{
					if (filter.memberNames == null) return true;
					foreach (string memberNameFilter in filter.memberNames)
					{
						if (memberName.IndexOf(memberNameFilter) > -1)  return true;
					}
					return false;
				}
			}
			return false;
		}

		private struct DisplayFilter
		{
			public string attributeName;
			public string[] memberNames;
		}
	}
}
