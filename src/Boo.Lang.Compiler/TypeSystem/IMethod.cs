namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IMethod : IMethodBase, IExtensionEnabled
	{	
		IType ReturnType
		{
			get;
		}
		
		bool IsAbstract
		{
			get;
		}
		
		bool IsVirtual
		{
			get;
		}
		
		bool IsSpecialName
		{
			get;
		}

		bool IsPInvoke
		{
			get;
		}
		
		IConstructedMethodInfo ConstructedInfo
		{
			get; 
		}
		
		IGenericMethodInfo GenericInfo
		{
			get;
		}
	}
}