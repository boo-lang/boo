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
using System.IO;

namespace NDoc.Core
{
	/// <summary>
	/// Utility Routines for path handling
	/// </summary>
	public sealed class PathUtilities
	{
		// no public constructor - only static methods...
		private PathUtilities(){}

		
		/// <summary>
		/// Combines the specified path with basePath 
		/// to form a full path to file or directory.
		/// </summary>
		/// <param name="basePath">The reference path.</param>
		/// <param name="path">The relative or absolute path.</param>
		/// <returns>
		/// A rooted path.
		/// </returns>
		public static string GetFullPath(string basePath, string path) 
		{
			if (path != null && path.Length > 0)
			{
				if (!Path.IsPathRooted(path)) 
				{
					path = Path.GetFullPath(Path.Combine(basePath, path));
				}
			}

			return path;
		}

		/// <summary>
		/// Gets the relative path of the passed path with respect to basePath
		/// </summary>
		/// <param name="basePath">The reference path.</param>
		/// <param name="path">The relative or absolute path.</param>
		/// <returns>
		/// A relative path.
		/// </returns>
		public static string GetRelativePath(string basePath, string path) 
		{
			if (path != null && path.Length > 0)
			{
				if (Path.IsPathRooted(path)) 
				{
					path = PathUtilities.AbsoluteToRelativePath(basePath, path);
				}
			}

			return path;
		}

		/// <summary>
		/// Converts an absolute path to one relative to the given base directory path
		/// </summary>
		/// <param name="basePath">The base directory path</param>
		/// <param name="absolutePath">An absolute path</param>
		/// <returns>A path to the given absolute path, relative to the base path</returns>
		public static string AbsoluteToRelativePath(string basePath, string absolutePath)
		{
			char[] separators = {
									Path.DirectorySeparatorChar, 
									Path.AltDirectorySeparatorChar, 
									Path.VolumeSeparatorChar 
								};

			//split the paths into their component parts
			string[] basePathParts = basePath.Split(separators);
			string[] absPathParts = absolutePath.Split(separators);
			int indx = 0;

			//work out how much they have in common
			int minLength = Math.Min(basePathParts.Length, absPathParts.Length);
			for (; indx < minLength; ++indx)
			{
				if (String.Compare(basePathParts[indx], absPathParts[indx], true) != 0)
					break;
			}
			
			//if they have nothing in common, just return the absolute path
			if (indx == 0) 
			{
				return absolutePath;
			}
			
			
			//start constructing the relative path
			string relPath = "";
			
			if (indx == basePathParts.Length)
			{
				// the entire base path is in the abs path
				// so the rel path starts with "./"
				relPath += "." + Path.DirectorySeparatorChar;
			} 
			else 
			{
				//step up from the base to the common root 
				for (int i = indx; i < basePathParts.Length; ++i) 
				{
					relPath += ".." + Path.DirectorySeparatorChar;
				}
			}
			//add the path from the common root to the absPath
			relPath += String.Join(Path.DirectorySeparatorChar.ToString(), absPathParts, indx, absPathParts.Length - indx);
			
			return relPath;
		}

		
		/// <summary>
		/// Converts a given base and relative path to an absolute path
		/// </summary>
		/// <param name="basePath">The base directory path</param>
		/// <param name="relativePath">A path to the base directory path</param>
		/// <returns>An absolute path</returns>
		public static string RelativeToAbsolutePath(string basePath, string relativePath)
		{
			//if the relativePath isn't... 
			if (Path.IsPathRooted(relativePath))
			{
				return relativePath;
			}

			//split the paths into their component parts
			string[] basePathParts = basePath.Split(Path.DirectorySeparatorChar);
			string[] relPathParts = relativePath.Split(Path.DirectorySeparatorChar);

			//determine how many we must go up from the base path
			int indx = 0;
			for (; indx < relPathParts.Length; ++indx) 
			{
				if (!relPathParts[indx].Equals("..")) 
				{
					break;
				}
			}
			
			//if the rel path contains no ".." it is below the base
			//therefor just concatonate the rel path to the base
			if (indx == 0) 
			{
				int offset=0;
				//ingnore the first part, if it is a rooting "."
				if (relPathParts[0]==".") offset=1;

				return basePath + Path.DirectorySeparatorChar + String.Join(Path.DirectorySeparatorChar.ToString(), relPathParts, offset, relPathParts.Length - offset);
			}
			
			string absPath = String.Join(Path.DirectorySeparatorChar.ToString(), basePathParts, 0, Math.Max(0, basePathParts.Length - indx));
			
			absPath += Path.DirectorySeparatorChar + String.Join(Path.DirectorySeparatorChar.ToString(), relPathParts, indx, relPathParts.Length - indx);
			
			return absPath;
		}
	}
}
