namespace Boo.Lang.Compiler.TypeSystem.ReflectionMetadata.Resolvers
{
	internal interface IAssemblyReferenceResolver
	{
		bool TryGetAssemblyPath(AssemblyReferenceData assemblyReference, out string path);
	}
}
