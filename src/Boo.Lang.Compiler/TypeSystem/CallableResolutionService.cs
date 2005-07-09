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
				: TypeSystemServices.GetType(node);
		}

		public bool IsValidByRefArg(IType parameterType, IType argType, Node arg)
		{
			return parameterType.IsByRef &&
				(argType == parameterType.GetElementType()) &&
				CanLoadAddress(arg);
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
			IType lastParameterType = parameters[lenMinusOne].Type;
			if (!lastParameterType.IsArray) return -1;

			int score = CalculateScore(parameters, args, lenMinusOne);
			if (score < 0) return -1;

			IType varArgType = lastParameterType.GetElementType();
			for (int i=lenMinusOne; i<args.Count; ++i)
			{
				int argumentScore = CalculateArgumentScore(varArgType, args.GetNodeAt(i));
				if (argumentScore < 0) return -1;
				score += (argumentScore - 3);
			}
			return score;
		}

		private int CalculateExactArgsScore(IParameter[] parameters, NodeCollection args)
		{
			int parameterCount = parameters.Length;
			return args.Count == parameterCount
				? CalculateScore(parameters, args, parameterCount)
				: -1;
		}

		private int CalculateScore(IParameter[] parameters, NodeCollection args, int count)
		{
			int score = 0;
			for (int i=0; i<count; ++i)
			{	
				IType parameterType = parameters[i].Type;
				int argumentScore = CalculateArgumentScore(parameterType, args.GetNodeAt(i));
				if (argumentScore < 0) return -1;
				score += argumentScore;
			}
			return score;
		}

		private int CalculateArgumentScore(IType parameterType, Node arg)
		{
			IType argumentType = GetExpressionTypeOrEntityType(arg);
			if (parameterType == argumentType)
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
			else if (IsValidByRefArg(parameterType, argumentType, arg))
			{
				// boo does not like byref
				return 3;
			}
			return -1;
		}
	}
}
