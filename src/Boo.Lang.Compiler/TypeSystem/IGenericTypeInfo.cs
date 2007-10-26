namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IGenericTypeInfo
	{
		IGenericParameter[] GenericParameters { get; }
		IType ConstructType(params IType[] arguments);
	}
}