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
using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem
{
	/// <summary>
	/// Overload resolution service.
	/// </summary>
	public class CallableResolutionService : AbstractCompilerComponent
	{
		protected const int CallableExactMatchScore = 11;
		protected const int CallableUpCastScore = 10;
		protected const int CallableImplicitConversionScore = 9;
		protected const int ExactMatchScore = 9;
		protected const int UpCastScore = 8;
        protected const int GenericInstantiateScore = 7;
        protected const int WideningPromotion = 6;
		protected const int ImplicitConversionScore = 5;
		protected const int NarrowingPromotion = 4;
		protected const int DowncastScore = 3;

		protected List<Candidate> _candidates = new List<Candidate>();
		protected ExpressionCollection _arguments;
	    private DowncastPermissions _downcastPermissions;
		readonly MemoizedFunction<IType, IType, int> _calculateArgumentScore;

	    public CallableResolutionService() : base(CompilerContext.Current)
	    {   
			_calculateArgumentScore = new MemoizedFunction<IType, IType, int>(CalculateArgumentScore);
	    	My<CurrentScope>.Instance.Changed += (sender, args) => _calculateArgumentScore.Clear();
	    }

	    protected Expression GetArgument(int index)
		{
			return _arguments[index];
		}

		public IList<Candidate> ValidCandidates
		{
			get { return _candidates; }
		}

		public class Candidate : IEquatable<Candidate>
		{
			public IMethod Method;
			private CallableResolutionService _crs;
			int[] _scores;
			bool _expanded;

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
				get { return _scores; }
			}

			public bool Expanded
			{
				get { return _expanded; }
				set { _expanded = value; }
			}

			public int Score(int argumentIndex)
			{
				var score = _crs.CalculateArgumentScore(
					Parameters[argumentIndex],
					Parameters[argumentIndex].Type,
					_crs.GetArgument(argumentIndex));
				_scores[argumentIndex] = score;
				return score;
			}

			public int ScoreVarArgs(int argumentIndex)
			{
				IParameter parameter = Parameters[Parameters.Length-1];
				_scores[argumentIndex] = _crs.CalculateArgumentScore(
					parameter,
					parameter.Type.ElementType,
					_crs.GetArgument(argumentIndex));

				return _scores[argumentIndex];
			}
			
			override public int GetHashCode()
			{
				return Method.GetHashCode();
			}
			
			override public bool Equals(object other)
			{
				return Equals(other as Candidate);
			}

			public bool Equals(Candidate other)
			{
				if (other == null) return false;
				if (other == this) return true;

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
		
		protected IType ArgumentType(Expression e)
		{
			return TypeSystemServices.GetExpressionType(e);
		}
		
		public bool IsValidByRefArg(IParameter param, IType parameterType, IType argType, Node arg)
		{
			if ((parameterType.IsByRef && argType == parameterType.ElementType)
			    || (param.IsByRef && argType == parameterType))
			{
				return CanLoadAddress(arg);
			}
			return false;
		}

		static bool CanLoadAddress(Node node)
		{
			var entity = node.Entity;
			
			if (entity == null || node is SelfLiteralExpression)
				return true;

			switch (entity.EntityType)
			{
				case EntityType.Local:
					return !((InternalLocal)entity).IsPrivateScope;
				case EntityType.Parameter:
					return true;
				case EntityType.Field:
					return !TypeSystemServices.IsReadOnlyField((IField)entity);
			}
			return false;
		}

		public IEntity ResolveCallableReference(ExpressionCollection args, IEntity[] candidates)
		{
			Reset(args, candidates);
			
			InferGenericMethods();
			FindApplicableCandidates();

			if (ValidCandidates.Count == 0) return null;
			if (ValidCandidates.Count == 1) return CheckCandidate(ValidCandidates[0], args);

			List<Candidate> dataPreserving = FindDataPreservingCandidates();
			if (dataPreserving.Count > 0)
			{
				FindBestMethod(dataPreserving);
                if (dataPreserving.Count == 1) return CheckCandidate(dataPreserving[0], args);
			}

			FindBestMethod(_candidates);
			if (ValidCandidates.Count > 1) PreferInternalEntitiesOverNonInternal();
            if (ValidCandidates.Count == 1) return CheckCandidate(ValidCandidates[0], args);
			return null;
		}

        // Required to support nested generic types
        private static IMethod CheckCandidate(Candidate value, ExpressionCollection args)
	    {
	        var scores = value.ArgumentScores;
            for (var i = 0; i < scores.Length; ++i)
	        {
                if (scores[i] == GenericInstantiateScore)
                {
                    var j = Math.Min(i, value.Parameters.Length - 1); //needed to support params* arguments
                    args[i].ExpressionType = value.Parameters[j].Type;
                }
	        }
	        return value.Method;
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
			return ValidCandidates.Any(IsNonInternalCandidate);
		}

		private bool HasInternalCandidate()
		{
			return ValidCandidates.Any(IsInternalCandidate);
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
			return ValidCandidates.Where(IsNonInternalCandidate).ToList();
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
			candidates.RemoveAll(candidate => 0 != BetterCandidate(candidate, pivot));
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
				if (candidate.Score(i) < 0)
					return false;

			// If method should be expanded, match remaining arguments against
			// last parameter
			if (expand)
			{
				candidate.Expanded = true;
				for (int i = fixedParams; i < _arguments.Count; i++)
					if (candidate.ScoreVarArgs(i) < 0)
						return false;
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
				total += score;
			return total;
		}

		private int BetterCandidate(Candidate c1, Candidate c2)
		{
			if (c1 == c2) return 0;

			int result = Math.Sign(TotalScore(c1) - TotalScore(c2));
			if (result != 0) return result;

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
			if (!c1.Expanded && c2.Expanded) return 1;
			if (c1.Expanded && !c2.Expanded) return -1;

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
					continue;

				// Keep the first result that is not a tie
				if (result == 0)
					result = better;
				// If a further result contradicts the initial result, neither candidate is more specific					
				else if (result != better)
					return 0;
			}
			return result;
		}

		private IType GetParameterTypeTemplate(Candidate candidate, int position)
		{
			// Get the method this candidate represents, or its generic template
			IMethod method = candidate.Method;
			if (candidate.Method.DeclaringType.ConstructedInfo != null)
				method = (IMethod)candidate.Method.DeclaringType.ConstructedInfo.UnMap(method);

			if (candidate.Method.ConstructedInfo != null)
				method = candidate.Method.ConstructedInfo.GenericDefinition;

			// If the parameter is the varargs parameter, use its element type
			IParameter[] parameters = method.GetParameters();
			if (candidate.Expanded && position >= parameters.Length - 1)
				return parameters[parameters.Length - 1].Type.ElementType;
			
			// Otherwise use the parameter's original type
			return parameters[position].Type;
		}

		private int MoreSpecific(IType t1, IType t2)
		{
			// Dive into array types and ref types
			if (t1.IsArray && t2.IsArray || t1.IsByRef && t2.IsByRef)
				return MoreSpecific(t1.ElementType, t2.ElementType);

            // A more concrete type is more specfic
            int result = GenericsServices.GetTypeConcreteness(t1) - GenericsServices.GetTypeConcreteness(t2);
            if (result != 0) return result;

			// With equal concreteness, the more generic type is more specific.
			//First search for open args, then for all args
			result = GenericsServices.GetTypeGenerity(t1) - GenericsServices.GetTypeGenerity(t2);
			if (result != 0) return result;
			result = GenericsServices.GetTypeGenericDepth(t1) - GenericsServices.GetTypeGenericDepth(t2);
			if (result != 0) return result;

			// If both types have the same genrity, the deeper-nested type is more specific
			return GetLogicalTypeDepth(t1) - GetLogicalTypeDepth(t2);
		}

		private void InferGenericMethods()
		{
			var gs = My<GenericsServices>.Instance;
			foreach (var candidate in _candidates)
			{
				if (candidate.Method.GenericInfo != null)
				{
					var inferredTypeParameters = gs.InferMethodGenericArguments(candidate.Method, _arguments);

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
			var lastIndex = parameters.Length - 1;
			var lastArg = args[-1];
			if (AstUtil.IsExplodeExpression(lastArg))
				return CalculateArgumentScore(parameters[lastIndex], parameters[lastIndex].Type, lastArg) > 0;

			IType varArgType = parameters[lastIndex].Type.ElementType;
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
		
		protected int CalculateArgumentScore(IParameter param, IType parameterType, Expression arg)
		{
			var argumentType = ArgumentType(arg);
			if (param.IsByRef)
			    return CalculateByRefArgumentScore(arg, param, parameterType, argumentType);
			return _calculateArgumentScore.Invoke(parameterType, argumentType);
		}

		private int CalculateByRefArgumentScore(Node arg, IParameter param, IType parameterType, IType argumentType)
		{
			return IsValidByRefArg(param, parameterType, argumentType, arg) ? ExactMatchScore : -1;
		}

		private int CalculateArgumentScore(IType parameterType, IType argumentType)
		{
			if (parameterType == argumentType || (TypeSystemServices.IsSystemObject(argumentType) &&  TypeSystemServices.IsSystemObject(parameterType)))
				return parameterType is ICallableType ? CallableExactMatchScore : ExactMatchScore;

			if (TypeCompatibilityRules.IsAssignableFrom(parameterType, argumentType))
			{				
				var callableType = parameterType as ICallableType;
				var callableArg = argumentType as ICallableType;
				if (callableType != null && callableArg != null)
					return CalculateCallableScore(callableType, callableArg);
                if (parameterType is IGenericArgumentsProvider && !(argumentType is IGenericArgumentsProvider) && !argumentType.IsNull())
                    return GenericInstantiateScore;
				return UpCastScore;
			}

			if (TypeSystemServices.FindImplicitConversionOperator(argumentType, parameterType) != null)
				return ImplicitConversionScore;

			if (TypeSystemServices.CanBeReachedByPromotion(parameterType, argumentType))
				return IsWideningPromotion(parameterType, argumentType) ? WideningPromotion : NarrowingPromotion;

			if (MyDowncastPermissions().CanBeReachedByDowncast(parameterType, argumentType))
				return DowncastScore;

			return -1;
		}

		private DowncastPermissions MyDowncastPermissions()
	    {
            return _downcastPermissions ?? (_downcastPermissions = My<DowncastPermissions>.Instance);
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
