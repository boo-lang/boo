using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Lang.Runtime
{
	class SetPropEmitter : MethodDispatcherEmitter
	{
		public SetPropEmitter(Type type, CandidateMethod found, Type[] argumentTypes) : base(type, found, argumentTypes)
		{	
		}

		protected override void EmitMethodBody()
		{
			Type valueType = GetValueType();

			LocalBuilder retVal = DeclareLocal(valueType);
			EmitLoadTargetObject();
			EmitMethodArguments();

			// Store last argument in a local variable
			Dup();
			StoreLocal(retVal);

			EmitMethodCall();

			LoadLocal(retVal);
			BoxIfNeeded(valueType);
			_il.Emit(OpCodes.Ret);
		}

		private Type GetValueType()
		{
			ParameterInfo[] parameters = _found.Parameters;
			return parameters[parameters.Length-1].ParameterType;
		}
	}
}
