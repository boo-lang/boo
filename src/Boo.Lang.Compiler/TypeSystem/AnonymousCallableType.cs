namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	
	public class AnonymousCallableType : AbstractType
	{
		TypeSystemServices _typeSystemServices;
		CallableSignature _signature;
		
		internal AnonymousCallableType(TypeSystemServices services, CallableSignature signature)
		{
		}
		
		override public string Name
		{
			get
			{				
				return _signature.ToString(); 
			}
		}
		
		override public EntityType EntityType
		{
			get
			{
				return EntityType.Type;
			}
		}
	}
}
