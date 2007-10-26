namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IType : ITypedEntity, INamespace
	{	
		bool IsClass
		{
			get;
		}
		
		bool IsAbstract
		{
			get;
		}
		
		bool IsInterface
		{
			get;
		}
		
		bool IsEnum
		{
			get;
		}
		
		bool IsByRef
		{
			get;
		}
		
		bool IsValueType
		{
			get;
		}
		
		bool IsFinal
		{
			get;
		}
		
		bool IsArray
		{
			get;
		}
		
		int GetTypeDepth();
		
		IType GetElementType();
		
		IType BaseType
		{
			get;
		}
		
		IEntity GetDefaultMember();
		
		IConstructor[] GetConstructors();
		
		IType[] GetInterfaces();
		
		bool IsSubclassOf(IType other);
		
		bool IsAssignableFrom(IType other);
		
		IGenericTypeInfo GenericInfo { get; }
		
		IConstructedTypeInfo ConstructedInfo { get; }
	}
}