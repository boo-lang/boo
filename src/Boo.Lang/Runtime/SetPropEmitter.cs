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
			EmitInvocation();
			ParameterInfo[] parameters = _found.Parameters;
			int valueIndex = parameters.Length-1;
			Type valueType = parameters[valueIndex].ParameterType;
			EmitMethodArgument(valueIndex, valueType);
			BoxIfNeeded(valueType);
			_il.Emit(OpCodes.Ret);
		}
	}
}
