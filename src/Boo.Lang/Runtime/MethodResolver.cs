using System;
using System.Collections.Generic;
using System.Reflection;

namespace Boo.Lang.Runtime
{
	public class MethodResolver
	{
		public static Type[] GetArgumentTypes(object[] arguments)
		{
			if (arguments.Length == 0) return NoArguments;

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

		private static Type[] NoArguments = new Type[0];

		private readonly Type[] _arguments;

		public MethodResolver(Type[] argumentTypes)
		{
			_arguments = argumentTypes;
		}

		public CandidateMethod ResolveMethod(IEnumerable<MethodInfo> candidates)
		{
			List applicable = FindApplicableMethods(candidates);
			if (applicable.Count == 0) return null;
			if (applicable.Count == 1) return ((CandidateMethod)applicable[0]);
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

		private List FindApplicableMethods(IEnumerable<MethodInfo> candidates)
		{
			List applicable = new List();
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
			int score = CalculateArgumentScore(candidateMethod, argumentIndex, paramType, _arguments[argumentIndex]);
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

	}
}
