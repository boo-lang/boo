using System;

namespace Boo.Lang.Compiler.Ast
{
	public partial class DepthFirstVisitor
	{
		public virtual bool Visit(Node node)
		{
			if (null != node)
			{
				try
				{
					node.Accept(this);
					return true;
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
			return false;
		}

		protected virtual void OnError(Node node, Exception error)
		{
			throw Boo.Lang.Compiler.CompilerErrorFactory.InternalError(node, error);
		}

		public virtual void Visit(Node[] array, NodeType nodeType)
		{
			foreach (Node node in array)
			{
				if (node.NodeType == nodeType)
				{
					Visit(node);
				}
			}
		}

		public virtual bool Visit<T>(NodeCollection<T> collection, NodeType nodeType) where T : Node
		{
			if (null != collection)
			{
				Visit(collection.ToArray(), nodeType);
				return true;
			}
			return false;
		}

		public virtual void Visit(Node[] array)
		{
			foreach (Node node in array)
			{
				Visit(node);
			}
		}

		public virtual bool Visit<T>(NodeCollection<T> collection) where T : Node
		{
			if (null != collection)
			{
				Visit(collection.ToArray());
				return true;
			}
			return false;
		}
	}
}
