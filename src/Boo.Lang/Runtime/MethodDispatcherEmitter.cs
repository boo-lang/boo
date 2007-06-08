#region license
// Copyright (c) 2004, 2007 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//	 * Redistributions of source code must retain the above copyright notice,
//	 this list of conditions and the following disclaimer.
//	 * Redistributions in binary form must reproduce the above copyright notice,
//	 this list of conditions and the following disclaimer in the documentation
//	 and/or other materials provided with the distribution.
//	 * Neither the name of Rodrigo B. de Oliveira nor the names of its
//	 contributors may be used to endorse or promote products derived from this
//	 software without specific prior written permission.
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

using System;
using System.Reflection;
#if NET_2_0
using System.Collections.Generic;
using System.Reflection.Emit;
#endif

namespace Boo.Lang.Runtime
{
#if NET_2_0
	internal delegate object MethodDispatcher(object target, object[] args);

	internal class MethodDispatcherEmitter
	{
		private readonly CandidateMethod found;
		private readonly Type[] argumentTypes;
		private readonly DynamicMethod method;
		private readonly ILGenerator il;

		public MethodDispatcherEmitter(Type owner, CandidateMethod found, Type[] argumentTypes)
		{
			this.found = found;
			this.argumentTypes = argumentTypes;
			method = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[]) }, owner);
			il = method.GetILGenerator();
		}

		public MethodDispatcher Emit()
		{	
			EmitLoadTargetObject();
			EmitMethodArguments();
			EmitMethodCall();
			EmitMethodReturn();
			return (MethodDispatcher)method.CreateDelegate(typeof(MethodDispatcher));
		}

		private void EmitMethodCall()
		{
			il.Emit(found.Method.IsStatic ? OpCodes.Call : OpCodes.Callvirt, found.Method);
		}

		private void EmitMethodArguments()
		{
			EmitFixedMethodArguments();
			if (found.VarArgs) EmitVarArgsMethodArguments();
		}

		private void EmitFixedMethodArguments()
		{
			for (int i = 0; i < found.MinimumArgumentCount; ++i)
			{
				Type paramType = found.GetParameterType(i);

				EmitMethodArgument(i, paramType);
			}
		}

		private void EmitVarArgsMethodArguments()
		{
			int varArgCount = argumentTypes.Length - found.MinimumArgumentCount;
			Type varArgType = found.VarArgsParameterType;
			OpCode storeOpCode = GetStoreElementOpCode(varArgType);
			il.Emit(OpCodes.Ldc_I4, varArgCount);
			il.Emit(OpCodes.Newarr, varArgType);
			for (int i = 0; i < varArgCount; ++i)
			{
				il.Emit(OpCodes.Dup);
				il.Emit(OpCodes.Ldc_I4, i);
				if (IsStobj(storeOpCode))
				{
					il.Emit(OpCodes.Ldelema, varArgType);
					EmitMethodArgument(found.MinimumArgumentCount + i, varArgType);
					il.Emit(storeOpCode, varArgType);
				}
				else
				{
					EmitMethodArgument(found.MinimumArgumentCount + i, varArgType);
					il.Emit(storeOpCode);
				}
			}
		}
		
		bool IsStobj(OpCode code)
		{
			return OpCodes.Stobj.Value == code.Value;
		}

		OpCode GetStoreElementOpCode(Type type)
		{
			if (type.IsValueType)
			{
				if (type.IsEnum) return OpCodes.Stelem_I4;

				switch (Type.GetTypeCode(type))
				{
					case TypeCode.Byte:
						return OpCodes.Stelem_I1;
					case TypeCode.Int16:
						return OpCodes.Stelem_I2;
					case TypeCode.Int32:
						return OpCodes.Stelem_I4;
					case TypeCode.Int64:
						return OpCodes.Stelem_I8;
					case TypeCode.Single:
						return OpCodes.Stelem_R4;
					case TypeCode.Double:
						return OpCodes.Stelem_R8;
				}
				return OpCodes.Stobj;
			}
			return OpCodes.Stelem_Ref;
		}

		private void EmitMethodArgument(int argumentIndex, Type paramType)
		{
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldc_I4, argumentIndex);
			il.Emit(OpCodes.Ldelem_Ref);

			switch (found.ArgumentScores[argumentIndex])
			{
				case CandidateMethod.PromotionScore:
					il.Emit(OpCodes.Castclass, typeof(IConvertible));
					il.Emit(OpCodes.Ldnull);
					il.Emit(OpCodes.Callvirt, GetPromotionMethod(paramType));
					break;
				case CandidateMethod.ImplicitConversionScore:
					EmitCastOrUnbox(argumentTypes[argumentIndex]);
					il.Emit(OpCodes.Call, found.GetArgumentConversion(argumentIndex));
					break;
				default:
					EmitCastOrUnbox(paramType);
					break;
			}
		}

		private void EmitLoadTargetObject()
		{
			if (found.Method.IsStatic) return;
			il.Emit(OpCodes.Ldarg_0); // target object is the first argument
		}

		private void EmitMethodReturn()
		{
			Type returnType = found.Method.ReturnType;
			if (returnType == typeof(void))
			{
				il.Emit(OpCodes.Ldnull);
			}
			else
			{
				if (returnType.IsValueType)
				{
					il.Emit(OpCodes.Box, returnType);
				}
			}
			il.Emit(OpCodes.Ret);
		}

		private void EmitCastOrUnbox(Type type)
		{
			if (type.IsValueType)
			{
				il.Emit(OpCodes.Unbox, type);
				il.Emit(OpCodes.Ldobj, type);
			}
			else
			{
				il.Emit(OpCodes.Castclass, type);
			}
		}

		private MethodInfo GetPromotionMethod(Type type)
		{
			return typeof(IConvertible).GetMethod("To" + Type.GetTypeCode(type));
		}
	}
#endif
}
