using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem.Builders
{
	public interface ITypeReferenceFactory
	{
		TypeReference TypeReferenceFor(IType type);
	}
}
