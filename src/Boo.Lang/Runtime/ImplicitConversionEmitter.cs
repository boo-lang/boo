using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Lang.Runtime
{
	class ImplicitConversionEmitter : DispatcherEmitter
	{
		private MethodInfo _conversion;

		public ImplicitConversionEmitter(MethodInfo conversion) : base(conversion.DeclaringType, conversion.Name)
		{
			_conversion = conversion;
		}

		protected override void EmitMethodBody()
		{
			_il.Emit(OpCodes.Ldarg_0);
			EmitCastOrUnbox(_conversion.GetParameters()[0].ParameterType);
			_il.Emit(OpCodes.Call, _conversion);
			EmitReturn(_conversion.ReturnType);
		}
	}
}
