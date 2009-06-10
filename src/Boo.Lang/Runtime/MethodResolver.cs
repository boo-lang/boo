#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//	   * Redistributions of source code must retain the above copyright notice,
//	   this list of conditions and the following disclaimer.
//	   * Redistributions in binary form must reproduce the above copyright notice,
//	   this list of conditions and the following disclaimer in the documentation
//	   and/or other materials provided with the distribution.
//	   * Neither the name of Rodrigo B. de Oliveira nor the names of its
//	   contributors may be used to endorse or promote products derived from this
//	   software without specific prior written permission.
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
using System.Collections.Generic;
using System.Reflection;

namespace Boo.Lang.Runtime
{
	public class MethodResolver
	{
		public static Type[] GetArgumentTypes(object[] arguments)
		{
			if (arguments.Length == 0)
				return Type.EmptyTypes;

			Type[] types = new Type[arguments.Length];
			for (int i = 0; i < types.Length; ++i)
			{
				types[i] = GetObjectTypeOrNull(arguments[i]);
			}
			return types;
		}

		private static Type GetObjectTypeOrNull(object arg)
		{
			if (null == arg) return null;
			return arg.GetType();
		}

		private readonly Type[] _arguments;

		public MethodResolver(params Type[] argumentTypes)
		{
			_arguments = argumentTypes;
		}

		public CandidateMethod ResolveMethod(IEnumerable<MethodInfo> candidates)
		{
			List<CandidateMethod> applicable = FindApplicableMethods(candidates);
			if (applicable.Count == 0) return null;
			if (applicable.Count == 1) return applicable[0];
			
			List<CandidateMethod> dataPreserving = applicable.FindAll(DoesNotRequireConversions);
			if (dataPreserving.Count > 0) return BestMethod(dataPreserving);
			return BestMethod(applicable);
		}

		private static bool DoesNotRequireConversions(CandidateMethod candidate)
		{
			return candidate.DoesNotRequireConversions;
		}

		private CandidateMethod BestMethod(List<CandidateMethod> applicable)
		{
			applicable.Sort(new System.Comparison<CandidateMethod>(BetterCandidate));
			return applicable[applicable.Count - 1];
		}

		private static int TotalScore(CandidateMethod c1)
		{
			int total = 0;
			foreach (int score in c1.ArgumentScores)
			{
				total += score;
			}
			return total;
		}

		private int BetterCandidate(CandidateMethod c1, CandidateMethod c2)
		{
			int result = Math.Sign(TotalScore(c1) - TotalScore(c2));
			if (result != 0) return result;
			if (c1.VarArgs && !c2.VarArgs) return -1;
			if (c2.VarArgs && !c1.VarArgs) return 1;

			int minArgumentCount = Math.Min(c1.MinimumArgumentCount, c2.MinimumArgumentCount);
			for (int i = 0; i < minArgumentCount; ++i)
			{
				result += MoreSpecificType(c1.GetParameterType(i), c2.GetParameterType(i));
			}
			if (result != 0) return result;

			if (c1.VarArgs && c2.VarArgs)
			{
				return MoreSpecificType(c1.VarArgsParameterType, c2.VarArgsParameterType);
			}
			return 0;
		}

		private static int MoreSpecificType(Type t1, Type t2)
		{
			// The less-generic type is more specific
			int result = GetTypeGenerity(t2) - GetTypeGenerity(t1);
			if (result != 0) return result;

			// If both types have the same generity, the deeper-nested type is more specific
			return GetLogicalTypeDepth(t1) - GetLogicalTypeDepth(t2);
		}

		private static int GetTypeGenerity(Type type)
		{
			if (!type.ContainsGenericParameters) return 0;
			return type.GetGenericArguments().Length;
		}

		private static int GetLogicalTypeDepth(Type type)
		{
			int depth = GetTypeDepth(type);
			return (type.IsValueType) ? depth - 1 : depth;
		}

		private static int GetTypeDepth(Type type)
		{
			if (type.IsByRef)
			{
				return GetTypeDepth(type.GetElementType());
			}
			else if (type.IsInterface)
			{
				return GetInterfaceDepth(type);
			}
			return GetClassDepth(type);
		}

		private static int GetClassDepth(Type type)
		{
			int depth = 0;
			Type objectType = typeof(object);
			while (type != null && type != objectType)
			{
				type = type.BaseType;
				++depth;
			}
			return depth;
		}

		private static int GetInterfaceDepth(Type type)
		{
			Type[] interfaces = type.GetInterfaces();
			if (interfaces.Length > 0)
			{
				int current = 0;
				foreach (Type i in interfaces)
				{
					int depth = GetInterfaceDepth(i);
					if (depth > current) current = depth;
				}
				return 1+current;
			}
			return 1;
		}

		private List<CandidateMethod> FindApplicableMethods(IEnumerable<MethodInfo> candidates)
		{
			List<CandidateMethod> applicable = new List<CandidateMethod>();
			foreach (MethodInfo method in candidates)
			{
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
			int score = CandidateMethod.CalculateArgumentScore(paramType, _arguments[argumentIndex]);
			if (score < 0) return false;

			candidateMethod.ArgumentScores[argumentIndex] = score;
			return true;
		}
	}
}
