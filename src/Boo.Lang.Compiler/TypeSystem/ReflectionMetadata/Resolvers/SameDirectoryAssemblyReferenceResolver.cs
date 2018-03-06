using System;
using System.IO;
using System.Reflection;

namespace Boo.Lang.Compiler.TypeSystem.ReflectionMetadata.Resolvers
{
	public sealed class SameDirectoryAssemblyReferenceResolver : IAssemblyReferenceResolver
	{
		private readonly string baseDirectory;

		public SameDirectoryAssemblyReferenceResolver(string baseDirectory)
		{
			this.baseDirectory = baseDirectory ?? throw new ArgumentNullException(nameof(baseDirectory));
		}

		public bool TryGetAssemblyPath(AssemblyReferenceData assemblyName, out string path)
		{
			if (assemblyName != null)
			{
				var baseDirectoryPath = Path.Combine(baseDirectory, assemblyName.Name + ".dll");

				if (File.Exists(baseDirectoryPath))
				{
					path = baseDirectoryPath;
					return true;
				}
			}

			path = null;
			return false;
		}
	}
}