using System;
using System.Reflection;

namespace Boo.Lang.Runtime
{
	internal class CandidateMethod
	{
		public const int ExactMatchScore = 7;
		public const int UpCastScore = 6;
		public const int ImplicitConversionScore = 5;
		public const int PromotionScore = 4;
		public const int DowncastScore = 3;

		private MethodInfo _method;
		private int[] _argumentScores;
		private MethodInfo[] _argumentConversions;
		private bool _varArgs;

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
			get
			{
				return _varArgs ? Parameters.Length - 1 : Parameters.Length;
			}
		}

		public ParameterInfo[] Parameters
		{
			get { return _method.GetParameters();  }
		}

		public Type VarArgsParameterType
		{
			get { return GetParameterType(Parameters.Length-1).GetElementType(); }
		}

		public Type GetParameterType(int i)
		{
			return Parameters[i].ParameterType;
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