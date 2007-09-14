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
using System.Reflection.Emit;

namespace Boo.Lang.Runtime
{
	public delegate object Dispatcher(object target, object[] args);

	public class MethodDispatcherEmitter : DispatcherEmitter
	{
		protected readonly CandidateMethod _found;
		protected readonly Type[] _argumentTypes;

		public MethodDispatcherEmitter(CandidateMethod found, params  Type[] argumentTypes) : this(found.Method.DeclaringType, found, argumentTypes)
		{	
		}

		public MethodDispatcherEmitter(Type owner, CandidateMethod found, Type[] argumentTypes) : base(owner, found.Method.Name + "$" + Builtins.join(argumentTypes, "$"))
		{
			_found = found;
			_argumentTypes = argumentTypes;
		}

		protected override void EmitMethodBody()
		{
			EmitInvocation();
			EmitMethodReturn();
		}

		protected void EmitInvocation()
		{
			EmitLoadTargetObject();
			EmitMethodArguments();
			EmitMethodCall();
		}

		protected void EmitMethodCall()
		{
			_il.Emit(_found.Method.IsStatic ? OpCodes.Call : OpCodes.Callvirt, _found.Method);
		}

		protected void EmitMethodArguments()
		{
			EmitFixedMethodArguments();
			if (_found.VarArgs) EmitVarArgsMethodArguments();
		}

		private void EmitFixedMethodArguments()
		{
			int offset = FixedArgumentOffset;
			int count = MinimumArgumentCount();
			for (int i = 0; i < count; ++i)
			{
				Type paramType = _found.GetParameterType(i + offset);
				EmitMethodArgument(i, paramType);
			}
		}

		protected virtual int FixedArgumentOffset
		{
			get { return 0; }
		}

		private void EmitVarArgsMethodArguments()
		{
			int varArgCount = _argumentTypes.Length - MinimumArgumentCount();
			Type varArgType = _found.VarArgsParameterType;
			OpCode storeOpCode = GetStoreElementOpCode(varArgType);
			_il.Emit(OpCodes.Ldc_I4, varArgCount);
			_il.Emit(OpCodes.Newarr, varArgType);
			for (int i = 0; i < varArgCount; ++i)
			{
				Dup();
				_il.Emit(OpCodes.Ldc_I4, i);
				if (IsStobj(storeOpCode))
				{
					_il.Emit(OpCodes.Ldelema, varArgType);
					EmitMethodArgument(MinimumArgumentCount() + i, varArgType);
					_il.Emit(storeOpCode, varArgType);
				}
				else
				{
					EmitMethodArgument(MinimumArgumentCount() + i, varArgType);
					_il.Emit(storeOpCode);
				}
			}
		}

		private int MinimumArgumentCount()
		{
			return _found.MinimumArgumentCount - FixedArgumentOffset;
		}

		static OpCode GetStoreElementOpCode(Type type)
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

		protected void EmitMethodArgument(int argumentIndex, Type expectedType)
		{
			EmitArgArrayElement(argumentIndex);
			EmitCoercion(argumentIndex, expectedType, _found.ArgumentScores[argumentIndex]);
		}

		private void EmitCoercion(int argumentIndex, Type expectedType, int score)
		{
			EmitCoercion(_argumentTypes[argumentIndex], expectedType, score);
		}

		protected virtual void EmitLoadTargetObject()
		{
			if (_found.Method.IsStatic) return;
			EmitLoadTargetObject(_found.Method.DeclaringType);
		}

		private void EmitMethodReturn()
		{
			EmitReturn(_found.Method.ReturnType);
		}
	}
}
