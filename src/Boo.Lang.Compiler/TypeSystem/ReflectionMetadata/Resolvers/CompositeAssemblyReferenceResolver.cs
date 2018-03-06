using System;
using System.Reflection;

namespace Boo.Lang.Compiler.TypeSystem.ReflectionMetadata.Resolvers
{
	public sealed class CompositeAssemblyReferenceResolver : IAssemblyReferenceResolver
	{
		private readonly IAssemblyReferenceResolver[] resolvers;

		internal CompositeAssemblyReferenceResolver(params IAssemblyReferenceResolver[] resolvers)
		{
			this.resolvers = resolvers ?? throw new ArgumentNullException(nameof(resolvers));
		}

		public bool TryGetAssemblyPath(AssemblyReferenceData assemblyName, out string path)
		{
			foreach (var resolver in resolvers)
				if (resolver.TryGetAssemblyPath(assemblyName, out path))
					return true;

			path = null;
			return false;
		}
	}
}