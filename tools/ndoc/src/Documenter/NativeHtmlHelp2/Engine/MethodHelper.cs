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

namespace NDoc.Documenter.NativeHtmlHelp2.Engine
{
	/// <summary>
	/// Helper functions to get information about a method
	/// </summary>
	public sealed class MethodHelper
	{
		/// <summary>No public constructor since this type only defines static methods...</summary>
		/// <remarks>Empty private constructor stops C# creating a public default constructor.</remarks>
		private MethodHelper(){}

		/// <summary>
		/// Determines if an overload exists
		/// </summary>
		/// <param name="methodNodes">The list of methods</param>
		/// <param name="indexes">an array of indices</param>
		/// <param name="index">The current index</param>
		/// <returns>True if no overload exists</returns>
		public static bool IsMethodAlone(XmlNodeList methodNodes, int[] indexes, int index)
		{
			string name = methodNodes[indexes[index]].Attributes["name"].Value;
			int lastIndex = methodNodes.Count - 1;
			if (lastIndex <= 0)
				return true;
			bool previousNameDifferent = (index == 0)
				|| (methodNodes[indexes[index - 1]].Attributes["name"].Value != name);
			bool nextNameDifferent = (index == lastIndex)
				|| (methodNodes[indexes[index + 1]].Attributes["name"].Value != name);
			return (previousNameDifferent && nextNameDifferent);
		}

		/// <summary>
		/// Determines if a method is the first overload
		/// </summary>
		/// <param name="methodNodes">Collection of method node</param>
		/// <param name="indexes">an array of indices</param>
		/// <param name="index">The current index</param>
		/// <returns>True if the method is the first overload</returns>
		public static bool IsMethodFirstOverload(XmlNodeList methodNodes, int[] indexes, int index)
		{
			if ((methodNodes[indexes[index]].Attributes["declaringType"] != null)
				|| IsMethodAlone(methodNodes, indexes, index))
				return false;

			string name			= methodNodes[indexes[index]].Attributes["name"].Value;
			string previousName	= GetPreviousMethodName(methodNodes, indexes, index);
			return previousName != name;
		}

		/// <summary>
		/// Determines if a method is the alst overload
		/// </summary>
		/// <param name="methodNodes"></param>
		/// <param name="indexes">an array of indices</param>
		/// <param name="index">The current index</param>
		/// <returns>True if the method is the last overload</returns>
		public static bool IsMethodLastOverload(XmlNodeList methodNodes, int[] indexes, int index)
		{
			if ((methodNodes[indexes[index]].Attributes["declaringType"] != null)
				|| IsMethodAlone(methodNodes, indexes, index))
				return false;

			string name		= methodNodes[indexes[index]].Attributes["name"].Value;
			string nextName	= GetNextMethodName(methodNodes, indexes, index);
			return nextName != name;
		}

		private static string GetPreviousMethodName(XmlNodeList methodNodes, int[] indexes, int index)
		{
			while ( --index >= 0 )
			{
				if (methodNodes[indexes[index]].Attributes["declaringType"] == null)
					return methodNodes[indexes[index]].Attributes["name"].Value;
			}
			return null;
		}

		private static string GetNextMethodName(XmlNodeList methodNodes, int[] indexes, int index)
		{
			while (++index < methodNodes.Count)
			{
				if (methodNodes[indexes[index]].Attributes["declaringType"] == null)
					return methodNodes[indexes[index]].Attributes["name"].Value;
			}
			return null;
		}
	}
}
