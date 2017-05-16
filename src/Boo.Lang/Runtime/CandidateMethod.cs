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


using System;
using System.Reflection;

namespace Boo.Lang.Runtime
{
	public class CandidateMethod
	{
		public const int ExactMatchScore = 7;
		public const int UpCastScore = 6;
		public const int WideningPromotion = 5;
		public const int ImplicitConversionScore = 4;
		public const int NarrowingPromotion = 3;
		public const int DowncastScore = 2;

		public static int CalculateArgumentScore(Type paramType, Type argType)
		{
#if !DNXCORE50
			if (null == argType)
				return !paramType.IsValueType ? ExactMatchScore : -1;
#else
		    if (null == argType)
		        return !paramType.GetTypeInfo().IsValueType ? ExactMatchScore : -1;
#endif

			if (paramType == argType) return ExactMatchScore;
			if (paramType.IsAssignableFrom(argType)) return UpCastScore;
			if (argType.IsAssignableFrom(paramType)) return DowncastScore;
			if (IsNumericPromotion(paramType, argType))
				return NumericTypes.IsWideningPromotion(paramType, argType) ? WideningPromotion : NarrowingPromotion;
			var conversion = RuntimeServices.FindImplicitConversionOperator(argType, paramType);
			if (null != conversion) return ImplicitConversionScore;
			return -1;
		}

		private readonly MethodInfo _method;
		private readonly int[] _argumentScores;
		private readonly bool _varArgs;

		public CandidateMethod(MethodInfo method, int argumentCount, bool varArgs)
		{
			_method = method;
			_argumentScores = new int[argumentCount];
			_varArgs = varArgs;
		}

		public MethodInfo Method
		{
			get { return _method;  }
		}

		public int[] ArgumentScores
		{
			get { return _argumentScores;  }
		}

		public bool VarArgs
		{
			get { return _varArgs;  }
		}

		public int MinimumArgumentCount
		{
			get { return _varArgs ? Parameters.Length - 1 : Parameters.Length; }
		}

		public ParameterInfo[] Parameters
		{
			get { return _method.GetParameters();  }
		}

		public Type VarArgsParameterType
		{
			get { return GetParameterType(Parameters.Length-1).GetElementType(); }
		}

		public bool DoesNotRequireConversions
		{
			get { return !RequiresConversions; }
		}

		private bool RequiresConversions
		{
			get { return Array.Exists(_argumentScores, RequiresConversion); }
		}

		private static bool RequiresConversion(int score)
		{
			return score < WideningPromotion;
		}

		public Type GetParameterType(int i)
		{
			return Parameters[i].ParameterType;
		}

		public static bool IsNumericPromotion(Type paramType, Type argType)
		{
			return RuntimeServices.IsPromotableNumeric(Type.GetTypeCode(paramType))
			       && RuntimeServices.IsPromotableNumeric(Type.GetTypeCode(argType));
		}

		public object DynamicInvoke(object target, object[] args)
		{
			return _method.Invoke(target, AdjustArgumentsForInvocation(args));
		}

		private object[] AdjustArgumentsForInvocation(object[] arguments)
		{
			if (VarArgs)
			{
				var varArgsParameterType = VarArgsParameterType;
				var minimumArgumentCount = MinimumArgumentCount;
				var newArguments = new object[minimumArgumentCount + 1];
				for (int i = 0; i < minimumArgumentCount; ++i)
					newArguments[i] = RequiresConversion(ArgumentScores[i]) ? RuntimeServices.Coerce(arguments[i], GetParameterType(i)) : arguments[i];
				newArguments[minimumArgumentCount] = CreateVarArgsArray(arguments, minimumArgumentCount, varArgsParameterType);
				return newArguments;
			}

			if (RequiresConversions)
				for (int i = 0; i < arguments.Length; ++i)
					arguments[i] = RequiresConversion(ArgumentScores[i]) ? RuntimeServices.Coerce(arguments[i], GetParameterType(i)) : arguments[i];
			return arguments;
		}

		private static Array CreateVarArgsArray(object[] arguments, int minimumArgumentCount, Type varArgsParameterType)
		{
			var length = arguments.Length - minimumArgumentCount;
			var result = Array.CreateInstance(varArgsParameterType, length);
			for (int i=0; i<result.Length; ++i)
				result.SetValue(RuntimeServices.Coerce(arguments[minimumArgumentCount + i], varArgsParameterType), i);
			return result;
		}
	}
}