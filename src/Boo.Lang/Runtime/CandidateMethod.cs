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