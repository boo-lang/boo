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
using System.Collections.Generic;

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem
{
	/// <summary>
	/// Overload resolution service.
	/// </summary>
	public class CallableResolutionService : AbstractCompilerComponent
	{
		protected const int CallableExactMatchScore = 10;
		protected const int CallableUpCastScore = 9;
		protected const int CallableImplicitConversionScore = 8;
		protected const int ExactMatchScore = 8;
		protected const int UpCastScore = 7;
		protected const int WideningPromotion = 6;
		protected const int ImplicitConversionScore = 5;
		protected const int NarrowingPromotion = 4;
		protected const int DowncastScore = 3;

		protected List<Candidate> _candidates = new List<Candidate>();
		protected ExpressionCollection _arguments;

		public CallableResolutionService(CompilerContext context)
		{
			Initialize(context);
		}

		protected Expression GetArgument(int index)
		{
			return _arguments[index];
		}

		public IList<Candidate> ValidCandidates
		{
			get { return _candidates; }
		}

		public override void Dispose()
		{
			_candidates.Clear();
			base.Dispose();
		}

		public class Candidate : IEquatable<Candidate>
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
				if (null == other) return false;
				if (this == other) return true;

				Candidate candidate = other as Candidate;
				return Equals(candidate);
			}

			public bool Equals(Candidate other)
			{
				if (null == other) return false;
				if (this == other) return true;

				return Method == other.Method;
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
		
		protected IType ArgumentType(Node node)
		{
			Expression e = node as Expression;
			return null != e
				? TypeSystemServices.GetExpressionType(e)
				: TypeSystem.TypeSystemServices.GetType(node);
		}
		
		public bool IsValidByRefArg(IParameter param, IType parameterType, IType argType, Node arg)
		{
			if ((parameterType.IsByRef && argType == parameterType.GetElementType())
			    || (param.IsByRef && argType == parameterType))
			{
				return CanLoadAddress(arg);
			}
			return false;
		}

		static bool CanLoadAddress(Node node)
		{
			IEntity entity = node.Entity;
			
			if (null == entity || node is SelfLiteralExpression)
				return true;

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
			Reset(args, candidates);
			
			InferGenericMethods();
			FindApplicableCandidates();

			if (ValidCandidates.Count == 0) return null;
			if (ValidCandidates.Count == 1) return (ValidCandidates[0]).Method;

			List<Candidate> dataPreserving = FindDataPreservingCandidates();
			if (dataPreserving.Count > 0)
			{
				FindBestMethod(dataPreserving);
				if (dataPreserving.Count == 1) return (dataPreserving[0]).Method;
			}

			FindBestMethod(_candidates);
			if (ValidCandidates.Count > 1) PreferInternalEntitiesOverNonInternal();
			if (ValidCandidates.Count == 1) return (ValidCandidates[0].Method);
			return null;
		}

		private void PreferInternalEntitiesOverNonInternal()
		{
			bool isAmbiguousBetweenInternalAndExternalEntities = HasInternalCandidate() &&
																 HasNonInternalCandidate();
			if (isAmbiguousBetweenInternalAndExternalEntities)
				foreach (Candidate c in GetNonInternalCandidates())
					ValidCandidates.Remove(c);
		}

		private bool HasNonInternalCandidate()
		{
			return Collections.Any(ValidCandidates, IsNonInternalCandidate);
		}

		private bool HasInternalCandidate()
		{
			return Collections.Any(ValidCandidates, IsInternalCandidate);
		}

		private static bool IsInternalCandidate(Candidate c)
		{
			return EntityPredicates.IsInternalEntity(c.Method);
		}

		private static bool IsNonInternalCandidate(Candidate c)
		{
			return EntityPredicates.IsNonInternalEntity(c.Method);
		}

		private IEnumerable<Candidate> GetNonInternalCandidates()
		{
			return new List<Candidate>(Collections.Where(ValidCandidates, IsNonInternalCandidate));
		}

		private List<Candidate> FindDataPreservingCandidates()
		{
			return _candidates.FindAll(DoesNotRequireConversions);
		}

		private static bool DoesNotRequireConversions(Candidate candidate)
		{
			return !Array.Exists(candidate.ArgumentScores, RequiresConversion);
		}

		private static bool RequiresConversion(int score)
		{
			return score < WideningPromotion;
		}

		private void FindBestMethod(List<Candidate> candidates)
		{
			candidates.Sort(new System.Comparison<Candidate>(BetterCandidate));

			Candidate pivot = candidates[candidates.Count - 1];
			candidates.RemoveAll(delegate(Candidate candidate)
          		{
          			return 0 != BetterCandidate(candidate, pivot);
          		});
		}

		private bool ApplicableCandidate(Candidate candidate)
		{
			// Figure out whether method should be varargs-expanded
			bool expand = ShouldExpandVarArgs(candidate);

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

		private bool ShouldExpandVarArgs(Candidate candidate)
		{
			IMethod method = candidate.Method;
			if (!method.AcceptVarArgs) return false;
			if (_arguments.Count == 0) return true;
			return ShouldExpandArgs(method, _arguments);
		}

		protected virtual bool ShouldExpandArgs(IMethod method, ExpressionCollection args)
		{
			return args.Count > 0 && !AstUtil.IsExplodeExpression(args[-1]);
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
			if (c1 == c2) return 0;

			int result = Math.Sign(TotalScore(c1) - TotalScore(c2));
//			int result = 0;
			/*
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
			*/
			
			if (result != 0)
			{
				return result;
			}

			// Prefer methods declared on deeper types
			result = 
				c1.Method.DeclaringType.GetTypeDepth() - 
				c2.Method.DeclaringType.GetTypeDepth();
			
			if (result != 0) return result;

			// Prefer methods with less generic parameters
			result = 
				GenericsServices.GetMethodGenerity(c2.Method) - 
				GenericsServices.GetMethodGenerity(c1.Method);
			
			if (result != 0) return result;

			// --- Tie breaking mode! ---

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

			// As a last means of breaking this desperate tie, we select the
			// "more specific" candidate, if one exists
			return MoreSpecific(c1, c2);
		}

		private int MoreSpecific(Candidate c1, Candidate c2)
		{
			int result = 0;
		
			for (int i = 0; i < _arguments.Count && i < c1.Parameters.Length; ++i)
			{
				if (c1.ArgumentScores[i] <= DowncastScore) continue;

				if (_arguments[i] is NullLiteralExpression)
					return 0; //neither type can be more specific wrt null

				// Select the most specific of the parameters' types, 
				// taking into account generic mapped parameters
				int better = MoreSpecific(
					GetParameterTypeTemplate(c1, i), 
					GetParameterTypeTemplate(c2, i));

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

		private IType GetParameterTypeTemplate(Candidate candidate, int position)
		{
			// Get the method this candidate represents, or its generic template
			IMethod method = candidate.Method;
			if (candidate.Method.DeclaringType.ConstructedInfo != null)
			{
				method = (IMethod)candidate.Method.DeclaringType.ConstructedInfo.UnMap(method);
			}

			if (candidate.Method.ConstructedInfo != null)
			{
				method = candidate.Method.ConstructedInfo.GenericDefinition;
			}

			// If the parameter is the varargs parameter, use its element type
			IParameter[] parameters = method.GetParameters();
			if (candidate.Expanded && position >= parameters.Length - 1)
			{
				return parameters[parameters.Length - 1].Type.GetElementType();
			}
			
			// Otherwise use the parameter's original type
			return parameters[position].Type;
		}

		private int MoreSpecific(IType t1, IType t2)
		{
			// Dive into array types and ref types
			if (t1.IsArray && t2.IsArray || t1.IsByRef && t2.IsByRef)
			{
				return MoreSpecific(t1.GetElementType(), t2.GetElementType());
			}

			// The less-generic type is more specific
			int result = GenericsServices.GetTypeGenerity(t2) - GenericsServices.GetTypeGenerity(t1);
			if (result != 0) return result;

			// If both types have the same genrity, the deeper-nested type is more specific
			return GetLogicalTypeDepth(t1) - GetLogicalTypeDepth(t2);
		}

		private void InferGenericMethods()
		{
			GenericsServices gs = Context.Produce<GenericsServices>();

			foreach (Candidate candidate in _candidates)
			{
				if (candidate.Method.GenericInfo != null)
				{
					IType[] inferredTypeParameters = gs.InferMethodGenericArguments(candidate.Method, _arguments);

					if (inferredTypeParameters == null || 
						!gs.CheckGenericConstruction(candidate.Method, inferredTypeParameters)) continue;

					candidate.Method = candidate.Method.GenericInfo.ConstructMethod(inferredTypeParameters);
				}
			}
		}

		private void FindApplicableCandidates()
		{
			_candidates = _candidates.FindAll(ApplicableCandidate);
		}

		private void Reset(ExpressionCollection arguments, IEnumerable<IEntity> candidateEntities)
		{
			_arguments = arguments;

			InitializeCandidates(candidateEntities);
		}

		private void InitializeCandidates(IEnumerable<IEntity> candidateEntities)
		{
			_candidates.Clear();
			foreach (IEntity entity in candidateEntities)
			{
				IMethod method = entity as IMethod;
				if (null == method) continue;

				Candidate candidate = new Candidate(this, method);

				_candidates.Add(candidate);
			}			
		}

		public bool IsValidVargsInvocation(IParameter[] parameters, ExpressionCollection args)
		{
			int lastIndex = parameters.Length - 1;

			if (args.Count < lastIndex) return false;

			if (!parameters[lastIndex].Type.IsArray) return false;

			if (!IsValidInvocation(parameters, args, lastIndex)) return false;

			if (args.Count > 0) return CheckVarArgsParameter(parameters, args);

			return true;
		}

		protected virtual bool CheckVarArgsParameter(IParameter[] parameters, ExpressionCollection args)
		{
			int lastIndex = parameters.Length - 1;
			Node lastArg = args[-1];
			if (AstUtil.IsExplodeExpression(lastArg))
			{
				return CalculateArgumentScore(parameters[lastIndex], parameters[lastIndex].Type, lastArg) > 0;
			}

			IType varArgType = parameters[lastIndex].Type.GetElementType();
			for (int i = lastIndex; i < args.Count; ++i)
			{
				int argumentScore = CalculateArgumentScore(parameters[lastIndex], varArgType, args[i]);
				if (argumentScore < 0) return false;
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
		
		protected int CalculateArgumentScore(IParameter param, IType parameterType, Node arg)
		{
			IType argumentType = ArgumentType(arg);
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
