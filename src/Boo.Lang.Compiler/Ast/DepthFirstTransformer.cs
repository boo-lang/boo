using System;
using System.Collections.Generic;
using System.Text;

namespace Boo.Lang.Compiler.Ast
{
	public partial class DepthFirstTransformer
	{
		protected Node _resultingNode = null;

		protected virtual void RemoveCurrentNode()
		{
			_resultingNode = null;
		}

		protected virtual void ReplaceCurrentNode(Node replacement)
		{
			_resultingNode = replacement;
		}

		protected virtual void OnNode(Node node)
		{
			node.Accept(this);
		}

		public virtual Node VisitNode(Node node)
		{
			if (null != node)
			{
				try
				{
					Node saved = _resultingNode;
					_resultingNode = node;
					OnNode(node);
					Node result = _resultingNode;
					_resultingNode = saved;
					return result;
				}
				catch (Boo.Lang.Compiler.CompilerError)
				{
					throw;
				}
				catch (Exception error)
				{
					OnError(node, error);
				}
			}
			return null;
		}

		protected virtual void OnError(Node node, Exception error)
		{
			throw Boo.Lang.Compiler.CompilerErrorFactory.InternalError(node, error);
		}

		public Node Visit(Node node)
		{
			return VisitNode(node);
		}

		public Expression Visit(Expression node)
		{
			return (Expression)VisitNode(node);
		}

		public Statement Visit(Statement node)
		{
			return (Statement)VisitNode(node);
		}

		public virtual bool Visit<T>(NodeCollection<T> collection) where T : Node
		{
			if (null == collection) return false;
			
			int removed = 0;

			T[] nodes = collection.ToArray();
			for (int i = 0; i < nodes.Length; ++i)
			{
				T currentNode = nodes[i];
				T resultingNode = (T)VisitNode(currentNode);
				if (currentNode != resultingNode)
				{
					int actualIndex = i - removed;
					if (null == resultingNode)
					{
						++removed;
						collection.RemoveAt(actualIndex);
					}
					else
					{
						collection.ReplaceAt(actualIndex, resultingNode);
					}
				}
			}
			return true;
		}
	}
}
