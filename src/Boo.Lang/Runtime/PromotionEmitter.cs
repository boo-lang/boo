using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Lang.Runtime
{
	class PromotionEmitter : DispatcherEmitter
	{
		private Type _toType;

		public PromotionEmitter(Type toType) : base(toType, "NumericPromotion")
		{
			_toType = toType;
		}

		protected override void EmitMethodBody()
		{
			_il.Emit(OpCodes.Ldarg_0);
			MethodInfo promotion = EmitPromotion(_toType);
			EmitReturn(promotion.ReturnType);
		}
	}
}
