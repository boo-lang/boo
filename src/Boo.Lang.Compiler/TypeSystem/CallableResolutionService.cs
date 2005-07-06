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
				if (null == score)
				{
					return false;
				}
				return object.Equals(Entity, score.Entity);
			}
			
			override public string ToString()
			{
				return Entity.ToString();
			}
		}

		CallableScore GetBiggerScore(List scores)
		{
			scores.Sort();
			CallableScore first = (CallableScore)scores[0];
			CallableScore second = (CallableScore)scores[1];
			return first.Score > second.Score
				? first : null;
		}
		
		void ReScoreByHierarchyDepth(List scores)
		{
			foreach (CallableScore score in scores)
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
			if (null != entity)
			{
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
			}
			return false;
		}
		
		public IEntity ResolveCallableReference(NodeCollection args, IEntity[] candidates)
		{	
			_scores.Clear();

			for (int i=0; i<candidates.Length; ++i)
			{
				IEntity tag = candidates[i];
				IMethod mb = tag as IMethod;
				if (null == mb) continue;

				IParameter[] parameters = mb.GetParameters();
				if (args.Count == parameters.Length)
				{
					int score = 0;
					for (int argIndex=0; argIndex<parameters.Length; ++argIndex)
					{
						Node arg = args.GetNodeAt(argIndex);
						IType expressionType = GetExpressionTypeOrEntityType(arg);
						IType parameterType = parameters[argIndex].Type;
						
						if (parameterType == expressionType)
						{
							// exact match scores 3
							score += 3;
						}
						else if (parameterType.IsAssignableFrom(expressionType))
						{
							// upcast scores 2
							score += 2;
						}
						else if (
							TypeSystemServices.CanBeReachedByDownCastOrPromotion(parameterType, expressionType) ||
							IsValidByRefArg(parameterType, expressionType, arg))
						{
							// downcast scores 1
							score += 1;
						}
						else
						{
							score = -1;
							break;
						}
					}
					
					if (score >= 0)
					{
						// only positive scores are compatible
						_scores.Add(new CallableScore(mb, score));
					}
				}
			}
			
			if (1 == _scores.Count)
			{
				return ((CallableScore)_scores[0]).Entity;
			}
			
			if (_scores.Count > 1)
			{
				CallableScore score = GetBiggerScore(_scores);
				if (null != score)
				{
					return score.Entity;
				}
				
				ReScoreByHierarchyDepth(_scores);
				score = GetBiggerScore(_scores);
				if (null != score)
				{
					return score.Entity;
				}
			}
			return null;
		}
	}
}
