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

using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem
{
	/// <summary>
	/// Overload resolution service.
	/// </summary>
	public class CallableResolutionService : AbstractCompilerComponent
	{
		private const int CallableExactMatchScore = 10;
		private const int CallableUpCastScore = 9;
		private const int CallableImplicitConversionScore = 8;
		private const int ExactMatchScore = 8;
		private const int UpCastScore = 7;
		private const int WideningPromotion = 6;
		private const int ImplicitConversionScore = 5;
		private const int NarrowingPromotion = 4;
		private const int DowncastScore = 3;

		private List _candidates = new List();
		private ExpressionCollection _arguments;

		private Expression GetArgument(int index)
		{
			return _arguments[index];
		}

		public List ValidCandidates
		{
			get { return _candidates; }
		}

		public override void Dispose()
		{
			_candidates.Clear();
			base.Dispose();
		}

		public class Candidate
		{
			public IMethod Method;
			private CallableResolutionService _crs;
			int[] _scores = null;
			bool _expanded = false;

			public Candidate(CallableResolutionService crs, IMethod entity)
			{
				_crs = crs;
				Method = entity;
				_scores = new int[crs._arguments.Count];
			}

			public IParameter[] Parameters
			{
				get { return Method.GetParameters();  }
			}

			public int[] ArgumentScores
			{
				get
				{
					return _scores;
				}
			}

			public bool Expanded
			{
				get { return _expanded; }
				set { _expanded = value; }
			}

			public int Score(int argumentIndex)
			{
				_scores[argumentIndex] = _crs.CalculateArgumentScore(
					Parameters[argumentIndex],
					Parameters[argumentIndex].Type,
					_crs.GetArgument(argumentIndex));

				return _scores[argumentIndex];
			}

			public int ScoreVarArgs(int argumentIndex)
			{
				IParameter parameter = Parameters[Parameters.Length-1];
				_scores[argumentIndex] = _crs.CalculateArgumentScore(
					parameter,
					parameter.Type.GetElementType(),
					_crs.GetArgument(argumentIndex));

				return _scores[argumentIndex];
			}
			
			override public int GetHashCode()
			{
				return Method.GetHashCode();
			}
			
			override public bool Equals(object other)
			{
				Candidate score = other as Candidate;
				return null == score
					? false
					: Method == score.Method;
			}
			
			override public string ToString()
			{
				return Method.ToString();
			}
		}
		
		public int GetLogicalTypeDepth(IType type)
		{
			int depth = type.GetTypeDepth();
			if (type.IsValueType) return depth - 1;
			return depth;
		}
		
		IType GetExpressionTypeOrEntityType(Node node)
		{
			Expression e = node as Expression;
			return null != e
				? TypeSystemServices.GetExpressionType(e)
				: TypeSystem.TypeSystemServices.GetType(node);
		}
		
		public bool IsValidByRefArg(IParameter param, IType parameterType, IType argType, Node arg)
		{
			if ((parameterType.IsByRef &&
				argType == parameterType.GetElementType()))
			{
				return CanLoadAddress(arg);
			}
			else if (param.IsByRef &&
				argType == parameterType)
			{
				return CanLoadAddress(arg);
			}
			return false;
		}

		static bool CanLoadAddress(Node node)
		{
			IEntity entity = node.Entity;
			
			if (null == entity) return true;
			
			switch (entity.EntityType)
			{
				case EntityType.Local:
				{
					return !((InternalLocal)entity).IsPrivateScope;
				}
				
				case EntityType.Parameter:
				{
					return true;
				}
				
				case EntityType.Field:
				{
					return !TypeSystemServices.IsReadOnlyField((IField)entity);
				}
			}
			return false;
		}
		
		public IEntity ResolveCallableReference(ExpressionCollection args, IEntity[] candidates)
		{
			Reset(args);
			FindApplicableCandidates(candidates);
			if (ValidCandidates.Count == 0) return null;
			if (ValidCandidates.Count == 1) return ((Candidate)ValidCandidates[0]).Method;

			List dataPreserving = ValidCandidates.Collect(DoesNotRequireConversions);
			if (dataPreserving.Count > 0)
			{
				if (dataPreserving.Count == 1) return ((Candidate)dataPreserving[0]).Method;
				IEntity found = BestMethod(dataPreserving);
				if (null != found) return found;
			}
			return BestCandidate();
		}

		private static bool DoesNotRequireConversions(object candidate)
		{
			return !Array.Exists(((Candidate) candidate).ArgumentScores, RequiresConversion);
		}

		private static bool RequiresConversion(int score)
		{
			return score < WideningPromotion;
		}

		private IEntity BestCandidate()
		{
			return BestMethod(_candidates);
		}

		private IEntity BestMethod(List candidates)
		{
			candidates.Sort(new Comparer(BetterCandidate));

			if (BetterCandidate(candidates[-1], candidates[-2]) == 0)
			{
				object pivot = candidates[-2];

				candidates.RemoveAll(delegate(object item)
				                     	{	
				                     		return 0 != BetterCandidate(item, pivot);
				                     	});
				// Ambiguous match
				return null;
			}

			// SUCCESS: _candidates[-1] is the winner
			return ((Candidate)candidates[-1]).Method;
		}

		private int BetterCandidate(object lhs, object rhs)
		{
			return BetterCandidate((Candidate) lhs, (Candidate) rhs);
		}

		private bool ApplicableCandidate(Candidate candidate)
		{
			// Figure out whether method should be varargs-expanded
			bool expand =
				candidate.Method.AcceptVarArgs &&
				(_arguments.Count == 0 || (_arguments.Count > 0 && 
				!AstUtil.IsExplodeExpression(_arguments[-1])));

			// Determine number of fixed (non-varargs) parameters
			int fixedParams =
				(expand ? candidate.Parameters.Length - 1 : candidate.Parameters.Length);

			// Validate number of parameters against number of arguments
			if (_arguments.Count < fixedParams) return false;
			if (_arguments.Count > fixedParams && !expand) return false;

			// Score each argument against a fixed parameter
			for (int i = 0; i < fixedParams; i++)
			{
				if (candidate.Score(i) < 0)
				{
					return false;
				}
			}

			// If method should be expanded, match remaining arguments against
			// last parameter
			if (expand)
			{
				candidate.Expanded = true;
				for (int i = fixedParams; i < _arguments.Count; i++)
				{
					if (candidate.ScoreVarArgs(i) < 0)
					{
						return false;
					}
				}
			}

			return true;
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

		private int BetterCandidate(Candidate c1, Candidate c2)
		{
			int result = Math.Sign(TotalScore(c1) - TotalScore(c2));
//			int result = 0;
			if (false)
			{
				for (int i = 0; i < _arguments.Count; i++)
				{
					// Compare methods based on their score for the current argument 
					int better = Math.Sign(c1.ArgumentScores[i] - c2.ArgumentScores[i]);
					if (better == 0) continue;

					// If neither method has been selecteed yet, select one
					// based on this argument
					if (result == 0)
					{
						result = better;
					}
						// If this argument comparison is in conflict with previous selection,
						// neither method is better
					else if (result != better)
					{
						return 0;
					}
				}
			}
			
			if (result != 0)
			{
				return result;
			}

			result = c1.Method.DeclaringType.GetTypeDepth() - c2.Method.DeclaringType.GetTypeDepth();
			if (result != 0)
			{
				return result;
			}

			// --- Tie breaking mode! ---

			// Non-generic methods are better than generic ones

			// Commented out since current syntax distinguishes between invoking
			// a generic method and a non generic one
			/*
			IGenericMethodInfo generic1 = c1.Method.GenericMethodInfo;
			IGenericMethodInfo generic2 = c2.Method.GenericMethodInfo;
			if (generic1 == null && generic2 != null)
			{
				return 1;
			}
			else if (generic1 != null && generic2 == null)
			{
				return -1;
			}
			*/

			// Non-expanded methods are better than expanded ones
			if (!c1.Expanded && c2.Expanded)
			{
				return 1;
			}
			else if (c1.Expanded && !c2.Expanded)
			{
				return -1;
			}

			// An expanded method with more fixed parameters is better
			result = c1.Parameters.Length - c2.Parameters.Length;
			if (result != 0) return result;

			// As a last means of breaking this desparate tie, we select the
			// "more specific" candidate, if one exists
			return MoreSpecific(c1, c2);
		}

		private int MoreSpecific(Candidate c1, Candidate c2)
		{
			int result = 0;
			for (int i = 0; i < _arguments.Count && i < c1.Parameters.Length; ++i)
			{
				if (c1.ArgumentScores[i] <= DowncastScore) continue;

				int better = MoreSpecific(c1.Parameters[i].Type, c2.Parameters[i].Type);

				// Skip parameters that are the same for both candidates
				if (better == 0)
				{
					continue;
				}

				// Keep the first result that is not a tie
				if (result == 0)
				{
					result = better;
				}
				// If a further result contradicts the initial result, neither candidate is more specific					
				else if (result != better)
				{
					return 0;
				}
			}
			return result;
		}

		private int MoreSpecific(IType t1, IType t2)
		{
			if (t1.IsArray && t2.IsArray)
			{
				return MoreSpecific(t1.GetElementType(), t2.GetElementType());
			}
			return GetLogicalTypeDepth(t1) - GetLogicalTypeDepth(t2);

			// TODO: if one of the types was once a generic parameter, the other one is the more specific
			// ** This will solve BOO-960 **

			// TODO: recursively examine constructed types 
			
			// Neither type is more specific
			// return 0;
		}

		private void FindApplicableCandidates(IEntity[] candidates)
		{
			foreach (IEntity entity in candidates)
			{
				IMethod method = entity as IMethod;
				if (null == method) continue;

				Candidate candidate = new Candidate(this, method);

				if (!ApplicableCandidate(candidate)) continue;

				_candidates.Add(candidate);
			}
		}

		private void Reset(ExpressionCollection arguments)
		{
			_arguments = arguments;
			_candidates.Clear();
		}

		public bool IsValidVargsInvocation(IParameter[] parameters, ExpressionCollection args)
		{
			int lastIndex = parameters.Length - 1;
			if (args.Count < lastIndex) return false;

			IType lastParameterType = parameters[lastIndex].Type;
			if (!lastParameterType.IsArray) return false;

			if (!IsValidInvocation(parameters, args, lastIndex)) return false;

			if (args.Count > 0)
			{
				Node lastArg = args[-1];
				if (AstUtil.IsExplodeExpression(lastArg))
				{
					return CalculateArgumentScore(parameters[lastIndex], lastParameterType, lastArg) > 0;
				}
				else
				{
					IType varArgType = lastParameterType.GetElementType();
					for (int i = lastIndex; i < args.Count; ++i)
					{
						int argumentScore = CalculateArgumentScore(parameters[lastIndex], varArgType, args[i]);
						if (argumentScore < 0) return false;
					}
				}
			}
			return true;
		}

		private bool IsValidInvocation(IParameter[] parameters, ExpressionCollection args, int count)
		{
			for (int i = 0; i < count; ++i)
			{
				IParameter parameter = parameters[i];
				IType parameterType = parameter.Type;
				int argumentScore = CalculateArgumentScore(parameter, parameterType, args[i]);
				if (argumentScore < 0) return false;
			}
			return true;
		}

		
		private int CalculateArgumentScore(IParameter param, IType parameterType, Node arg)
		{
			IType argumentType = GetExpressionTypeOrEntityType(arg);
			if (param.IsByRef)
			{
				if (IsValidByRefArg(param, parameterType, argumentType, arg))
				{
					return ExactMatchScore;
				}
				return -1;
			}
			else if (parameterType == argumentType
				|| (TypeSystemServices.IsSystemObject(argumentType) && 
					TypeSystemServices.IsSystemObject(parameterType)))
			{
				return parameterType is ICallableType
					? CallableExactMatchScore
					: ExactMatchScore;
			}
			else if (parameterType.IsAssignableFrom(argumentType))
			{				
				ICallableType callableType = parameterType as ICallableType;
				ICallableType callableArg = argumentType as ICallableType;
				if (callableType != null && callableArg != null)
				{
					return CalculateCallableScore(callableType, callableArg);
				}
				return UpCastScore;
			}
			else if (TypeSystemServices.FindImplicitConversionOperator(argumentType, parameterType) != null)
			{
				return ImplicitConversionScore;
			}
			else if (TypeSystemServices.CanBeReachedByPromotion(parameterType, argumentType))
			{
				if (IsWideningPromotion(parameterType, argumentType)) return WideningPromotion;
				return NarrowingPromotion;
			}
			else if (TypeSystemServices.CanBeReachedByDowncast(parameterType, argumentType))
			{
				return DowncastScore;
			}
			return -1;
		}

		private bool IsWideningPromotion(IType paramType, IType argumentType)
		{
			ExternalType expected = paramType as ExternalType;
			if (null == expected) return false;
			ExternalType actual = argumentType as ExternalType;
			if (null == actual) return false;
			return Boo.Lang.Runtime.NumericTypes.IsWideningPromotion(expected.ActualType, actual.ActualType);
		}

		private static int CalculateCallableScore(ICallableType parameterType, ICallableType argType)
		{
			// upcast
			// parameterType == ICallableType, "ThreadStart"
			// argumentType == ICallableType, "Anonymous Closure"
			// RULES:
			// Number of arguments for argumentType && parameterType == same
			// Either: all arguments "IsAssignableFrom"
			//			OR
			//			all arguments == exactly (best case scenario)
			// ExactMatch -- (best case)
			// UpCast -- "not exact match, but very close" (this is OK)
			// ImplicitConversion -- "assignable, but wrong number of parameters / whatever" (boo does the normal thing)
			
			CallableSignature siggyType = parameterType.GetSignature();
			CallableSignature siggyArg = argType.GetSignature();
			// Ensuring that these callables have same number of arguments.
			// def foo(a, b,c) == { a, b, c| print foobar }					
			if (siggyType.Parameters.Length != siggyArg.Parameters.Length)
			{
				return CallableUpCastScore;
			}
			for (int i = 0; i < siggyType.Parameters.Length; i++)
			{
				if (siggyType.Parameters[i].Type != siggyArg.Parameters[i].Type)
				{	
					return CallableImplicitConversionScore;
				}
			}
			return siggyType.ReturnType == siggyArg.ReturnType
				? CallableExactMatchScore : CallableUpCastScore;
		}
	}
}
