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
		private readonly CandidateMethod _found;
		private readonly Type[] _argumentTypes;
		private readonly DynamicMethod _method;
		private readonly ILGenerator _il;

		public MethodDispatcherEmitter(Type owner, CandidateMethod found, Type[] argumentTypes)
		{
			_found = found;
			_argumentTypes = argumentTypes;
			_method = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[]) }, owner);
			_il = _method.GetILGenerator();
		}

		public MethodDispatcher Emit()
		{	
			EmitLoadTargetObject();
			EmitMethodArguments();
			EmitMethodCall();
			EmitMethodReturn();
			return CreateMethodDispatcher();
		}

		private MethodDispatcher CreateMethodDispatcher()
		{
			return (MethodDispatcher)_method.CreateDelegate(typeof(MethodDispatcher));
		}

		private void EmitMethodCall()
		{
			_il.Emit(_found.Method.IsStatic ? OpCodes.Call : OpCodes.Callvirt, _found.Method);
		}

		private void EmitMethodArguments()
		{
			EmitFixedMethodArguments();
			if (_found.VarArgs) EmitVarArgsMethodArguments();
		}

		private void EmitFixedMethodArguments()
		{
			for (int i = 0; i < _found.MinimumArgumentCount; ++i)
			{
				Type paramType = _found.GetParameterType(i);

				EmitMethodArgument(i, paramType);
			}
		}

		private void EmitVarArgsMethodArguments()
		{
			int varArgCount = _argumentTypes.Length - _found.MinimumArgumentCount;
			Type varArgType = _found.VarArgsParameterType;
			OpCode storeOpCode = GetStoreElementOpCode(varArgType);
			_il.Emit(OpCodes.Ldc_I4, varArgCount);
			_il.Emit(OpCodes.Newarr, varArgType);
			for (int i = 0; i < varArgCount; ++i)
			{
				_il.Emit(OpCodes.Dup);
				_il.Emit(OpCodes.Ldc_I4, i);
				if (IsStobj(storeOpCode))
				{
					_il.Emit(OpCodes.Ldelema, varArgType);
					EmitMethodArgument(_found.MinimumArgumentCount + i, varArgType);
					_il.Emit(storeOpCode, varArgType);
				}
				else
				{
					EmitMethodArgument(_found.MinimumArgumentCount + i, varArgType);
					_il.Emit(storeOpCode);
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
			_il.Emit(OpCodes.Ldarg_1);
			_il.Emit(OpCodes.Ldc_I4, argumentIndex);
			_il.Emit(OpCodes.Ldelem_Ref);

			switch (_found.ArgumentScores[argumentIndex])
			{
				case CandidateMethod.PromotionScore:
					_il.Emit(OpCodes.Castclass, typeof(IConvertible));
					_il.Emit(OpCodes.Ldnull);
					_il.Emit(OpCodes.Callvirt, GetPromotionMethod(paramType));
					break;
				case CandidateMethod.ImplicitConversionScore:
					EmitCastOrUnbox(_argumentTypes[argumentIndex]);
					_il.Emit(OpCodes.Call, _found.GetArgumentConversion(argumentIndex));
					break;
				default:
					EmitCastOrUnbox(paramType);
					break;
			}
		}

		private void EmitLoadTargetObject()
		{
			if (_found.Method.IsStatic) return;
			_il.Emit(OpCodes.Ldarg_0); // target object is the first argument
		}

		private void EmitMethodReturn()
		{
			Type returnType = _found.Method.ReturnType;
			if (returnType == typeof(void))
			{
				_il.Emit(OpCodes.Ldnull);
			}
			else
			{
				if (returnType.IsValueType)
				{
					_il.Emit(OpCodes.Box, returnType);
				}
			}
			_il.Emit(OpCodes.Ret);
		}

		private void EmitCastOrUnbox(Type type)
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

		private MethodInfo GetPromotionMethod(Type type)
		{
			return typeof(IConvertible).GetMethod("To" + Type.GetTypeCode(type));
		}
	}
#endif
}
