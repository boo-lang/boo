#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

#if !NO_SYSTEM_REFLECTION_EMIT

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Lang.Runtime.DynamicDispatching.Emitters
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
#if !DNXCORE50
		    if (type.IsValueType)
#else
            if (type.GetTypeInfo().IsValueType)
#endif
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
#if !DNXCORE50
		    if (returnType.IsValueType)
#else
		    if (returnType.GetTypeInfo().IsValueType)
#endif
			{
				_il.Emit(OpCodes.Box, returnType);
			}
		}

		protected void EmitLoadTargetObject(Type expectedType)
		{
			_il.Emit(OpCodes.Ldarg_0); // target object is the first argument
#if !DNXCORE50
		    if (expectedType.IsValueType)
#else
		    if (expectedType.GetTypeInfo().IsValueType)
#endif
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
				_il.Emit(OpCodes.Ldnull);
			else
				BoxIfNeeded(typeOnStack);
			_il.Emit(OpCodes.Ret);
		}

		protected void EmitPromotion(Type expectedType, Type actualType)
		{
			_il.Emit(OpCodes.Unbox_Any, actualType);
			_il.Emit(NumericPromotionOpcodeFor(Type.GetTypeCode(expectedType), true));
		}

		private static OpCode NumericPromotionOpcodeFor(TypeCode typeCode, bool @checked)
		{
			switch (typeCode)
			{
				case TypeCode.SByte:
					return @checked ? OpCodes.Conv_Ovf_I1 : OpCodes.Conv_I1;
				case TypeCode.Byte:
					return @checked ? OpCodes.Conv_Ovf_U1 : OpCodes.Conv_U1;
				case TypeCode.Int16:
					return @checked ? OpCodes.Conv_Ovf_I2 : OpCodes.Conv_I2;
				case TypeCode.UInt16:
				case TypeCode.Char:
					return @checked ? OpCodes.Conv_Ovf_U2 : OpCodes.Conv_U2;
				case TypeCode.Int32:
					return @checked ? OpCodes.Conv_Ovf_I4 : OpCodes.Conv_I4;
				case TypeCode.UInt32:
					return @checked ? OpCodes.Conv_Ovf_U4 : OpCodes.Conv_U4;
				case TypeCode.Int64:
					return @checked ? OpCodes.Conv_Ovf_I8 : OpCodes.Conv_I8;
				case TypeCode.UInt64:
					return @checked ? OpCodes.Conv_Ovf_U8 : OpCodes.Conv_U8;
				case TypeCode.Single:
					return OpCodes.Conv_R4;
				case TypeCode.Double:
					return OpCodes.Conv_R8;
				default:
					throw new ArgumentException(typeCode.ToString());
			}
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
					EmitPromotion(expectedType, actualType);
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
#endif