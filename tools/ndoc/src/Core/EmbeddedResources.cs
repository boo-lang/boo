// EmbeddedResources.cs - utilities to write embedded resources
// Copyright (C) 2001  Kral Ferch, Jason Diamond
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

namespace NDoc.Core
{
	using System;
	using System.IO;
	using System.Reflection;

	/// <summary>Utilties to help reading and writing embedded resources.</summary>
	/// <remarks>This is used to access the stylesheets.</remarks>
	public sealed class EmbeddedResources
	{
		// no public constructor - only static methods...
		private EmbeddedResources(){}

		/// <summary>Writes all the embedded resources with the specified prefix to disk.</summary>
		/// <param name="assembly">The assembly containing the embedded resources.</param>
		/// <param name="prefix">The prefix to search for.</param>
		/// <param name="directory">The directory to write the resources to.</param>
		public static void WriteEmbeddedResources(
			Assembly assembly,
			string prefix,
			string directory)
		{
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			string[] names = assembly.GetManifestResourceNames();

			foreach (string name in names)
			{
				if (name.StartsWith(prefix))
				{
					WriteEmbeddedResource(
						assembly,
						name,
						directory,
						name.Substring(prefix.Length + 1));
				}
			}
		}

		/// <summary>Writes an embedded resource to disk.</summary>
		/// <param name="assembly">The assembly containing the embedded resource.</param>
		/// <param name="name">The name of the embedded resource.</param>
		/// <param name="directory">The directory to write the resource to.</param>
		/// <param name="filename">The filename of the resource on disk.</param>
		public static void WriteEmbeddedResource(
			Assembly assembly,
			string name,
			string directory,
			string filename)
		{
			const int size = 512;
			byte[] buffer = new byte[size];
			int count = 0;

			Stream input = assembly.GetManifestResourceStream(name);
			Stream output = File.Open(Path.Combine(directory, filename), FileMode.Create);

			try
			{
				while ((count = input.Read(buffer, 0, size)) > 0)
				{
					output.Write(buffer, 0, count);
				}
			}
			finally
			{
				output.Close();
			}
		}
	}
}
