using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Lang.Runtime
{
	public abstract class DispatcherEmitter
	{
		protected readonly DynamicMethod _method;
		protected readonly ILGenerator _il;

		public DispatcherEmitter(Type owner)
		{
			_method = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[]) }, owner);
			_il = _method.GetILGenerator();
		}

		public Dispatcher Emit()
		{
			EmitMethodBody();
			return CreateMethodDispatcher();
		}

		protected abstract void EmitMethodBody();

		protected Dispatcher CreateMethodDispatcher()
		{
			return (Dispatcher)_method.CreateDelegate(typeof(Dispatcher));
		}

		protected bool IsStobj(OpCode code)
		{
			return OpCodes.Stobj.Value == code.Value;
		}

		protected void EmitCastOrUnbox(Type type)
		{
			if (type.IsValueType)
			{
				_il.Emit(OpCodes.Unbox, type);
				_il.Emit(OpCodes.Ldobj, type);
			}
			else
			{
				_il.Emit(OpCodes.Castclass, type);
			}
		}

		protected void BoxIfNeeded(Type returnType)
		{
			if (returnType.IsValueType)
			{
				_il.Emit(OpCodes.Box, returnType);
			}
		}

		protected void EmitLoadTargetObject(Type expectedType)
		{
			_il.Emit(OpCodes.Ldarg_0); // target object is the first argument
			if (expectedType.IsValueType) 
			{
				_il.Emit(OpCodes.Unbox, expectedType);
			}
			else
			{
				_il.Emit(OpCodes.Castclass, expectedType);
			}
		}

		protected void EmitReturn(Type typeOnStack)
		{
			if (typeOnStack == typeof(void))
			{
				_il.Emit(OpCodes.Ldnull);
			}
			else
			{
				BoxIfNeeded(typeOnStack);
			}
			_il.Emit(OpCodes.Ret);
		}

		protected void EmitPromotion(Type expectedType)
		{
			_il.Emit(OpCodes.Castclass, typeof(IConvertible));
			_il.Emit(OpCodes.Ldnull);
			_il.Emit(OpCodes.Callvirt, GetPromotionMethod(expectedType));
		}

		protected void EmitArgArrayElement(int argumentIndex)
		{
			_il.Emit(OpCodes.Ldarg_1);
			_il.Emit(OpCodes.Ldc_I4, argumentIndex);
			_il.Emit(OpCodes.Ldelem_Ref);
		}

		private MethodInfo GetPromotionMethod(Type type)
		{
			return typeof(IConvertible).GetMethod("To" + Type.GetTypeCode(type));
		}

		protected void Dup()
		{
			_il.Emit(OpCodes.Dup);
		}
	}
}