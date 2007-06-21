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
using System.Collections.Generic;

namespace Boo.Lang.Runtime
{
	internal class MethodInvoker
	{
		private object _target;
		private Type _type;
		private string _methodName;
		private object[] _arguments;

		private static Dictionary<MethodDispatcherKey, MethodDispatcher> _cache =
			new Dictionary<MethodDispatcherKey, MethodDispatcher>(MethodDispatcherKey.EqualityComparer);

		public MethodInvoker(object target, Type type, string methodName, object[] arguments)
		{
			_target = target;
			_type = type;
			_methodName = methodName;
			_arguments = arguments;
		}

		public object InvokeResolvedMethod()
		{
			Type[] argumentTypes = GetArgumentTypes();
			MethodDispatcherKey key = new MethodDispatcherKey(_type, _methodName, argumentTypes);
			MethodDispatcher dispatcher;
			if (!_cache.TryGetValue(key, out dispatcher))
			{
				CandidateMethod found = ResolveMethod();
				dispatcher = EmitMethodDispatcher(found, argumentTypes);
				_cache.Add(key, dispatcher);
			}
			return dispatcher(_target, _arguments);
		}

		private static Type[] NoArguments = new Type[0];

		private Type[] GetArgumentTypes()
		{
			if (_arguments.Length == 0) return NoArguments;

			Type[] types = new Type[_arguments.Length];
			for (int i = 0; i < types.Length; ++i)
			{
				types[i] = GetArgumentType(i);
			}
			return types;
		}

		private MethodDispatcher EmitMethodDispatcher(CandidateMethod found, Type[] argumentTypes)
		{
			return new MethodDispatcherEmitter(_type, found, argumentTypes).Emit();
		}

		private CandidateMethod ResolveMethod()
		{
			List applicable = FindApplicableMethods();
			if (applicable.Count == 1) return ((CandidateMethod)applicable[0]);
			if (applicable.Count == 0) throw new System.MissingMethodException(_type.FullName, _methodName);
			return BestMethod(applicable);
		}

		private CandidateMethod BestMethod(List applicable)
		{
			applicable.Sort(new Comparer(BetterCandidate));
			return ((CandidateMethod)applicable[-1]);
		}

		private int TotalScore(CandidateMethod c1)
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
			return BetterCandidate((CandidateMethod)lhs, (CandidateMethod)rhs);
		}

		private int BetterCandidate(CandidateMethod c1, CandidateMethod c2)
		{
			int result = Math.Sign(TotalScore(c1) - TotalScore(c2));
			if (result != 0) return result;

			if (c1.VarArgs) return c2.VarArgs ? 0 : -1;
			return c2.VarArgs ? 1 : 0;
		}

		private List FindApplicableMethods()
		{
			List applicable = new List();
			foreach (MethodInfo method in _type.GetMethods(RuntimeServices.DefaultBindingFlags))
			{
				if (_methodName != method.Name) continue;
				CandidateMethod candidateMethod = IsApplicableMethod(method);
				if (null == candidateMethod) continue;
				applicable.Add(candidateMethod);
			}
			return applicable;
		}

		private CandidateMethod IsApplicableMethod(MethodInfo method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			bool varargs = IsVarArgs(parameters);
			if (!ValidArgumentCount(parameters, varargs)) return null;

			CandidateMethod candidateMethod = new CandidateMethod(method, _arguments.Length, varargs);
			if (CalculateCandidateScore(candidateMethod)) return candidateMethod;

			return null;
		}

		private bool ValidArgumentCount(ParameterInfo[] parameters, bool varargs)
		{
			if (varargs)
			{
				int minArgumentCount = parameters.Length - 1;
				return _arguments.Length >= minArgumentCount;
			}
			return _arguments.Length == parameters.Length;
		}

		private bool IsVarArgs(ParameterInfo[] parameters)
		{
			if (parameters.Length == 0) return false;
			return HasParamArrayAttribute(parameters[parameters.Length - 1]);
		}

		private bool HasParamArrayAttribute(ParameterInfo info)
		{
			return info.IsDefined(typeof(ParamArrayAttribute), true);
		}

		private bool CalculateCandidateScore(CandidateMethod candidateMethod)
		{
			ParameterInfo[] parameters = candidateMethod.Parameters;
			for (int i = 0; i < candidateMethod.MinimumArgumentCount; ++i)
			{
				if (parameters[i].IsOut) return false;

				if (!CalculateCandidateArgumentScore(candidateMethod, i, parameters[i].ParameterType))
				{
					return false;
				}
			}

			if (candidateMethod.VarArgs)
			{
				Type varArgItemType = candidateMethod.VarArgsParameterType;
				for (int i = candidateMethod.MinimumArgumentCount; i < _arguments.Length; ++i)
				{
					if (!CalculateCandidateArgumentScore(candidateMethod, i, varArgItemType))
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool CalculateCandidateArgumentScore(CandidateMethod candidateMethod, int argumentIndex, Type paramType)
		{
			int score = CalculateArgumentScore(candidateMethod, argumentIndex, paramType, GetArgumentType(argumentIndex));
			if (score < 0) return false;

			candidateMethod.ArgumentScores[argumentIndex] = score;
			return true;
		}

		private int CalculateArgumentScore(CandidateMethod candidateMethod, int argumentIndex, Type paramType, Type argType)
		{
			if (null == argType)
			{
				if (paramType.IsValueType) return -1;
				return CandidateMethod.ExactMatchScore;
			}
			else
			{
				if (paramType == argType) return CandidateMethod.ExactMatchScore;

				if (paramType.IsAssignableFrom(argType)) return CandidateMethod.UpCastScore;

				if (argType.IsAssignableFrom(paramType)) return CandidateMethod.DowncastScore;

				if (IsNumericPromotion(paramType, argType)) return CandidateMethod.PromotionScore;

				MethodInfo conversion = RuntimeServices.FindImplicitConversionOperator(argType, paramType);
				if (null != conversion)
				{
					candidateMethod.RememberArgumentConversion(argumentIndex, conversion);
					return CandidateMethod.ImplicitConversionScore;
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
					if (x._arguments.Length != y._arguments.Length) return false;
					if (x._methodName != y._methodName) return false;
					for (int i = 0; i < x._arguments.Length; ++i)
					{
						if (x._arguments[i] != y._arguments[i]) return false;
					}
					return true;
				}
			}
		}
	}
}