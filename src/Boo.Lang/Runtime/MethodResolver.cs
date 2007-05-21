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

namespace Boo.Lang.Runtime
{
	public class MethodResolver
	{
		public const int ExactMatchScore = 7;
		public const int UpCastScore = 6;
		public const int ImplicitConversionScore = 5;
		public const int PromotionScore = 4;
		public const int DowncastScore = 3;

		private Type _type;
		private string _methodName;
		private object[] _arguments;

		public MethodResolver(Type type, string methodName, object[] arguments)
		{
			_type = type;
			_methodName = methodName;
			_arguments = arguments;
		}

		public object InvokeResolvedMethod(object instance)
		{
			Candidate found = ResolveMethod();
			return found.Method.Invoke(instance, AdjustArguments(found));
		}

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
	}
}