using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Lang.Runtime
{
	public abstract class DispatcherEmitter
	{
		private DynamicMethod _dynamicMethod;
		protected readonly ILGenerator _il;

		public DispatcherEmitter(Type owner, string dynamicMethodName)
		{
			_dynamicMethod = new DynamicMethod(owner.Name + "$" + dynamicMethodName, typeof(object), new Type[] { typeof(object), typeof(object[]) }, owner);
			_il = _dynamicMethod.GetILGenerator();
		}

		public Dispatcher Emit()
		{
			EmitMethodBody();
			return CreateMethodDispatcher();
		}

		protected abstract void EmitMethodBody();

		protected Dispatcher CreateMethodDispatcher()
		{
			return (Dispatcher)_dynamicMethod.CreateDelegate(typeof(Dispatcher));
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

		protected MethodInfo EmitPromotion(Type expectedType)
		{
			_il.Emit(OpCodes.Castclass, typeof(IConvertible));
			_il.Emit(OpCodes.Ldnull);
			MethodInfo method = GetPromotionMethod(expectedType);
			_il.Emit(OpCodes.Callvirt, method);
			return method;
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

		protected void EmitCoercion(Type actualType, Type expectedType, int score)
		{
			switch (score)
			{
				case CandidateMethod.WideningPromotion:
				case CandidateMethod.NarrowingPromotion:
					EmitPromotion(expectedType);
					break;
				case CandidateMethod.ImplicitConversionScore:
					EmitCastOrUnbox(actualType);
					_il.Emit(OpCodes.Call, RuntimeServices.FindImplicitConversionOperator(actualType, expectedType));
					break;
				default:
					EmitCastOrUnbox(expectedType);
					break;
			}
		}

		protected void LoadLocal(LocalBuilder value)
		{
			_il.Emit(OpCodes.Ldloc, value);
		}

		protected void StoreLocal(LocalBuilder value)
		{
			_il.Emit(OpCodes.Stloc, value);
		}

		protected LocalBuilder DeclareLocal(Type type)
		{
			return _il.DeclareLocal(type);
		}
	}
}