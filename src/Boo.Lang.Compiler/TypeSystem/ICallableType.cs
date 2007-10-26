namespace Boo.Lang.Compiler.TypeSystem
{
	public interface ICallableType : IType
	{
		CallableSignature GetSignature();
	}
}