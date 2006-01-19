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
		List _scores = new List();

		public CallableResolutionService()
		{
		}

		public List ValidCandidates
		{
			get
			{
				return _scores;
			}
		}

		public override void Dispose()
		{
			_scores.Clear();
			base.Dispose();
		}

		public class CallableScore : IComparable
		{
			public IMethod Entity;
			public int Score;
			
			public CallableScore(IMethod entity, int score)
			{
				Entity = entity;
				Score = score;
			}
			
			public int CompareTo(object other)
			{
				return ((CallableScore)other).Score-Score;
			}
			
			override public int GetHashCode()
			{
				return Entity.GetHashCode();
			}
			
			override public bool Equals(object other)
			{
				CallableScore score = other as CallableScore;
				return null == score
					? false
					: Entity == score.Entity;
			}
			
			override public string ToString()
			{
				return Entity.ToString();
			}
		}

		CallableScore GetBiggerScore()
		{
			_scores.Sort();
			CallableScore first = (CallableScore)_scores[0];
			CallableScore second = (CallableScore)_scores[1];
			return first.Score > second.Score
				? first
				: null;
		}
		
		void ReScoreByHierarchyDepth()
		{
			foreach (CallableScore score in _scores)
			{
				score.Score += score.Entity.DeclaringType.GetTypeDepth();
				
				IParameter[] parameters = score.Entity.GetParameters();
				for (int i=0; i<parameters.Length; ++i)
				{
					score.Score += parameters[i].Type.GetTypeDepth();
				}
			}
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
				return true;
			}
			return false;
		}
		
		bool CanLoadAddress(Node node)
		{
			IEntity entity = node.Entity;
			
			if (null == entity) return false;
			
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
		
		public IEntity ResolveCallableReference(NodeCollection args, IEntity[] candidates)
		{	
			_scores.Clear();

			CalculateScores(candidates, args);

			if (1 == _scores.Count)
			{
				return ((CallableScore)_scores[0]).Entity;
			}
			
			if (_scores.Count > 1)
			{
				CallableScore score = GetBiggerScore();
				if (null != score)
				{
					return score.Entity;
				}
				
				ReScoreByHierarchyDepth();
				score = GetBiggerScore();
				if (null != score)
				{
					return score.Entity;
				}
			}
			return null;
		}

		private void CalculateScores(IEntity[] candidates, NodeCollection args)
		{
			for (int i=0; i<candidates.Length; ++i)
			{
				IMethod candidate = candidates[i] as IMethod;
				if (null == candidate) continue;

				IParameter[] parameters = candidate.GetParameters();

				int score = candidate.AcceptVarArgs
					? CalculateVarArgsScore(parameters, args)
					: CalculateExactArgsScore(parameters, args);
				
				if (score >= 0)
				{
					// only positive scores are compatible
					_scores.Add(new CallableScore(candidate, score));
				}
			}
		}

		public int CalculateVarArgsScore(IParameter[] parameters, NodeCollection args)
		{
			int lenMinusOne = parameters.Length-1;
			if (args.Count < lenMinusOne) return -1;

			IParameter lastParameter = parameters[lenMinusOne];
			IType lastParameterType = lastParameter.Type;
			if (!lastParameterType.IsArray) return -1;

			int score = CalculateScore(parameters, args, lenMinusOne);
			if (score < 0) return -1;

			if (args.Count > 0)
			{
				Node lastArg = args.GetNodeAt(-1);
				if (AstUtil.IsExplodeExpression(lastArg))
				{	
					int argumentScore = CalculateArgumentScore(lastParameter, lastParameterType, lastArg);
					if (argumentScore < 0) return -1;
					score += argumentScore;
					// this is the one the user wants
					return score;
				}
				else
				{
					IType varArgType = lastParameterType.GetElementType();
					for (int i=lenMinusOne; i<args.Count; ++i)
					{
						int argumentScore = CalculateArgumentScore(lastParameter, varArgType, args.GetNodeAt(i));
						if (argumentScore < 0) return -1;
						score += argumentScore;
					}
				}
			}
			// varargs should not be preferred over non varargs methods
			return score - ((args.Count + 1) * 3);
		}

		private int CalculateExactArgsScore(IParameter[] parameters, NodeCollection args)
		{
			return args.Count == parameters.Length
				? CalculateScore(parameters, args, args.Count)
				: -1;
		}

		private int CalculateScore(IParameter[] parameters, NodeCollection args, int count)
		{	
			int score = 3;
			for (int i=0; i<count; ++i)
			{
				IParameter parameter = parameters[i];
				IType parameterType = parameter.Type;
				int argumentScore = CalculateArgumentScore(parameter, parameterType, args.GetNodeAt(i));
				if (argumentScore < 0) return -1;
				score += argumentScore;
			}
			return score;
		}

		private int CalculateArgumentScore(IParameter param, IType parameterType, Node arg)
		{
			IType argumentType = GetExpressionTypeOrEntityType(arg);
			if (param.IsByRef)
			{
				if (IsValidByRefArg(param, parameterType, argumentType, arg))
				{
					return 7;
				}
			}
			else if (parameterType == argumentType)
			{
				// exact match
				return 6;
			}
			else if (parameterType.IsAssignableFrom(argumentType))
			{
				// upcast
				return 5;
			}
			else if (TypeSystemServices.CanBeReachedByDownCastOrPromotion(parameterType, argumentType))
			{
				// downcast
				return 4;
			}
			return -1;
		}
	}
}
