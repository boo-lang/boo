namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	
	public class InternalCallableType : InternalType, ICallableType
	{
		CallableSignature _signature;
		
		internal InternalCallableType(TypeSystemServices typeSystemServices, TypeDefinition typeDefinition) :
			base(typeSystemServices, typeDefinition)
		{
		}
		
		public CallableSignature GetSignature()
		{
			if (null == _signature)
			{
				_signature = GetInvokeMethod().CallableType.GetSignature();
			}
			return _signature;
		}
		
		public IMethod GetInvokeMethod()
		{
			return (IMethod)_typeDefinition.Members["Invoke"].Entity;
		}
		
		override public bool IsAssignableFrom(IType other)
		{
			return TypeSystemServices.IsCallableTypeAssignableFrom(this, other);
		}
		
		override public string ToString()
		{
			return GetSignature().ToString();
		}
	}
}
