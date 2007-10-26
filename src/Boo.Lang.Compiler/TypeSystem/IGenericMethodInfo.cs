namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IGenericMethodInfo
	{
		IGenericParameter[] GenericParameters { get; }
		IMethod ConstructMethod(params IType[] arguments);
	}
}