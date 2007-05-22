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
	public class MethodResolver
	{
		public const int ExactMatchScore = 7;
		public const int UpCastScore = 6;
		public const int ImplicitConversionScore = 5;
		public const int PromotionScore = 4;
		public const int DowncastScore = 3;

		private object _target;
		private Type _type;
		private string _methodName;
		private object[] _arguments;

#if NET_2_0
		private delegate object MethodDispatcher(object target, object[] args);

		private static Dictionary<MethodDispatcherKey, MethodDispatcher> _cache =
			new Dictionary<MethodDispatcherKey, MethodDispatcher>(MethodDispatcherKey.EqualityComparer);
#endif
		public MethodResolver(object target, Type type, string methodName, object[] arguments)
		{
			_target = target;
			_type = type;
			_methodName = methodName;
			_arguments = arguments;
		}

		bool IsStaticMethodInvocation
		{
			get { return _target == null;  }
		}

		public object InvokeResolvedMethod()
		{
#if NET_2_0
			MethodDispatcherKey key = new MethodDispatcherKey(_type, _methodName, GetArgumentTypes());
			MethodDispatcher dispatcher;
			if (!_cache.TryGetValue(key, out dispatcher))
			{
				Candidate found = ResolveMethod();
				dispatcher = EmitMethodDispatcher(found);
				_cache.Add(key, dispatcher);
			}
			return dispatcher(_target, _arguments);
#else
			Candidate found = ResolveMethod();
			return found.Method.Invoke(_target, AdjustArguments(found));
#endif
		}

#if NET_2_0
		private static Type[] NoArguments = new Type[0];

		private Type[] GetArgumentTypes()
		{
			if (_arguments.Length == 0) return NoArguments;

			Type[] types = new Type[_arguments.Length];
			for (int i=0; i<types.Length; ++i)
			{
				types[i] = GetArgumentType(i);
			}
			return types;
		}

		private MethodDispatcher EmitMethodDispatcher(Candidate found)
		{
			DynamicMethod method = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[])}, _type);
			ILGenerator il = method.GetILGenerator();
			EmitLoadTargetObject(il);
			EmitMethodArguments(found, il);
			EmitMethodCall(found, il);
			EmitMethodReturn(found, il);
			return (MethodDispatcher)method.CreateDelegate(typeof(MethodDispatcher));
		}

		private void EmitMethodCall(Candidate found, ILGenerator il)
		{
			il.Emit(IsStaticMethodInvocation ? OpCodes.Call : OpCodes.Callvirt, found.Method);
		}

		private void EmitMethodArguments(Candidate found, ILGenerator il)
		{
			for (int i = 0; i < _arguments.Length; ++i)
			{
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldc_I4, i);
				il.Emit(OpCodes.Ldelem_Ref);

				Type paramType = found.GetParameterType(i);
				switch (found.ArgumentScores[i])
				{
					case PromotionScore:
						il.Emit(OpCodes.Castclass, typeof(IConvertible));
						il.Emit(OpCodes.Ldnull);
						il.Emit(OpCodes.Callvirt, GetPromotionMethod(paramType));
						break;
					case ImplicitConversionScore:
						EmitCastOrUnbox(il, GetArgumentType(i));
						il.Emit(OpCodes.Call, found.GetArgumentConversion(i));
						break;
					default:
						EmitCastOrUnbox(il, paramType);
						break;
				}
			}
		}

		private void EmitLoadTargetObject(ILGenerator il)
		{
			if (IsStaticMethodInvocation) return;
			il.Emit(OpCodes.Ldarg_0); // target object is the first argument
		}

		private static void EmitMethodReturn(Candidate found, ILGenerator il)
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

		private static void EmitCastOrUnbox(ILGenerator il, Type type)
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
#else
		private object[] AdjustArguments(Candidate candidate)
		{
			for (int i = 0; i < _arguments.Length; ++i)
			{
				_arguments[i] = AdjustArgument(candidate, i, _arguments[i]);
			}
			return _arguments;
		}

		private object AdjustArgument(Candidate candidate, int argumentIndex, object argument)
		{
			switch(candidate.ArgumentScores[argumentIndex])
			{
				case PromotionScore:
					return PromoteNumericArgument(candidate.GetParameterType(argumentIndex), argument);
				case ImplicitConversionScore:
					return candidate.GetArgumentConversion(argumentIndex).Invoke(null, new object[] {argument});
			}
			return argument;
		}


		private object PromoteNumericArgument(Type type, object argument)
		{
			IConvertible convertible = (IConvertible) argument;
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Byte: return convertible.ToByte(null);
				case TypeCode.SByte: return convertible.ToSByte(null);
				case TypeCode.Int16: return convertible.ToInt16(null);
				case TypeCode.Int32: return convertible.ToInt32(null);
				case TypeCode.Int64: return convertible.ToInt64(null);
				case TypeCode.UInt16: return convertible.ToUInt16(null);
				case TypeCode.UInt32: return convertible.ToUInt32(null);
				case TypeCode.UInt64: return convertible.ToUInt64(null);
				case TypeCode.Single: return convertible.ToSingle(null);
				case TypeCode.Double: return convertible.ToDouble(null);
				case TypeCode.Boolean: return convertible.ToBoolean(null);
				case TypeCode.Decimal: return convertible.ToDecimal(null);
				case TypeCode.Char: return convertible.ToChar(null);
			}
			throw new ArgumentException();
		}
#endif
		private Candidate ResolveMethod()
		{
			List applicable = FindApplicableMethods();
			if (applicable.Count == 1) return ((Candidate)applicable[0]);
			if (applicable.Count == 0) throw new System.MissingMethodException(_type.FullName, _methodName);
			return BestMethod(applicable);
		}

		private Candidate BestMethod(List applicable)
		{
			applicable.Sort(new Comparer(BetterCandidate));
			return ((Candidate)applicable[-1]);
		}

		private int TotalScore(Candidate c1)
		{
			int total = 0;
			foreach (int score in c1.ArgumentScores)
			{
				total += score;
			}
			return total;
		}

		private int BetterCandidate(object lhs, object rhs)
		{
			return BetterCandidate((Candidate)lhs, (Candidate)rhs);
		}

		private int BetterCandidate(Candidate c1, Candidate c2)
		{
			return Math.Sign(TotalScore(c1) - TotalScore(c2));
		}

		private List FindApplicableMethods()
		{
			List applicable = new List();
			foreach (MethodInfo method in _type.GetMethods(RuntimeServices.DefaultBindingFlags))
			{
				if (_methodName != method.Name) continue;
				Candidate candidate = IsApplicableMethod(method);
				if (null == candidate) continue;
				applicable.Add(candidate);
			}
			return applicable;
		}

		private Candidate IsApplicableMethod(MethodInfo method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length != _arguments.Length) return null;

			Candidate candidate = new Candidate(method, _arguments.Length);
			if (CalculateCandidateScore(candidate)) return candidate;

			return null;
		}

		private bool CalculateCandidateScore(Candidate candidate)
		{
			ParameterInfo[] parameters = candidate.Method.GetParameters();
			for (int i=0; i<parameters.Length; ++i)
			{
				int score = CalculateArgumentScore(candidate, i, parameters[i], GetArgumentType(i));
				if (score < 0) return false;

				candidate.ArgumentScores[i] = score;
			}
			return true;
		}

		private int CalculateArgumentScore(Candidate candidate, int argumentIndex, ParameterInfo parameter, Type argType)
		{
			if (parameter.IsOut) return -1;

			if (null == argType)
			{
				if (parameter.ParameterType.IsValueType) return -1;
				return ExactMatchScore;
			}
			else
			{
				Type paramType = parameter.ParameterType;

				if (paramType == argType) return ExactMatchScore;

				if (paramType.IsAssignableFrom(argType)) return UpCastScore;

				if (argType.IsAssignableFrom(paramType)) return DowncastScore;

				if (IsNumericPromotion(paramType, argType)) return PromotionScore;

				MethodInfo conversion = RuntimeServices.FindImplicitConversionOperator(argType, paramType);
				if (null != conversion)
				{
					candidate.RememberArgumentConversion(argumentIndex, conversion);
					return ImplicitConversionScore;
				}
			}
			return -1;
		}

		private bool IsNumericPromotion(Type paramType, Type argType)
		{
			return RuntimeServices.IsPromotableNumeric(Type.GetTypeCode(paramType))
				&& RuntimeServices.IsPromotableNumeric(Type.GetTypeCode(argType));
		}

		private Type GetArgumentType(int i)
		{
			object arg = _arguments[i];
			if (null == arg) return null;
			return arg.GetType();
		}

		public class Candidate
		{
			private MethodInfo _method;
			private int[] _argumentScores;
			private MethodInfo[] _argumentConversions;

			public Candidate(MethodInfo method, int argumentCount)
			{
				_method = method;
				_argumentScores = new int[argumentCount];
			}

			public MethodInfo Method
			{
				get { return _method;  }
			}

			public int[] ArgumentScores
			{
				get { return _argumentScores;  }
			}

			public Type GetParameterType(int i)
			{
				return _method.GetParameters()[i].ParameterType;
			}

			public void RememberArgumentConversion(int argumentIndex, MethodInfo conversion)
			{
				if (null == _argumentConversions)
				{
					_argumentConversions = new MethodInfo[_argumentScores.Length];
				}
				_argumentConversions[argumentIndex] = conversion;
			}

			public MethodInfo GetArgumentConversion(int argumentIndex)
			{
				return _argumentConversions[argumentIndex];
			}
		}

#if NET_2_0
		class MethodDispatcherKey
		{
			public static readonly IEqualityComparer<MethodDispatcherKey> EqualityComparer = new _EqualityComparer();

			private Type _type;
			private string _methodName;
			private Type[] _arguments;

			public MethodDispatcherKey(Type type, string methodName, Type[] arguments)
			{
				_type = type;
				_methodName = methodName;
				_arguments = arguments;
			}

			class _EqualityComparer : IEqualityComparer<MethodDispatcherKey>
			{
				public int GetHashCode(MethodDispatcherKey key)
				{
					return key._type.GetHashCode() ^ key._methodName.GetHashCode() ^ key._arguments.Length;
				}

				public bool Equals(MethodDispatcherKey x, MethodDispatcherKey y)
				{
					if (x._type != y._type) return false;
					if (x._methodName != y._methodName) return false;
					if (x._arguments.Length != y._arguments.Length) return false;
					for (int i = 0; i < x._arguments.Length; ++i)
					{
						if (x._arguments[i] != y._arguments[i]) return false;
					}
					return true;
				}
			}
		}
#endif
	}
}