namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Reflection;
	
	public class ExternalCallableType : ExternalType, ICallableType
	{
		IMethod _invoke;
		
		internal ExternalCallableType(TypeSystemServices typeSystemServices, Type type) : base(typeSystemServices, type)
		{
			_invoke = (IMethod)typeSystemServices.Map(type.GetMethod("Invoke"));
		}
		
		public IParameter[] GetParameters()
		{
			return _invoke.GetParameters();
		}
		
		public IType ReturnType
		{
			get
			{
				return _invoke.ReturnType;
			}
		}
	}
}
