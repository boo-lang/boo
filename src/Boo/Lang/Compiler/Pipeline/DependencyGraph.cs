namespace Boo.Lang.Compiler.Pipeline
{
	using System;
	using System.Collections;
	using Boo.Lang.Ast;
	using Boo.Lang.Compiler.Bindings;
	
	/// <summary>
	/// Resolves the type of an expression or method.
	/// </summary>
	public interface ITypeResolver
	{
		/// <summary>
		/// Resolves the type of the expression and returns true
		/// if the type has changed since the last iteration.
		/// </summary>
		IBinding Resolve(SemanticStep parent);
		
		/// <summary>
		/// Invoked at the end of the resolution process, when
		/// all nodes are considered resolved.
		/// </summary>
		void OnResolved(SemanticStep parent);
	}
	
	class GraphNode
	{
		public readonly Node Node;

		ITypeResolver _resolver;

		public GraphNode(Node node, ITypeResolver resolver)
		{
			Node = node;
			_resolver = resolver;
		}
		
		public bool Resolve(SemanticStep parent)
		{			
			IBinding binding = ((ITypedBinding)BindingManager.GetBinding(Node)).BoundType;
			IBinding resolved = _resolver.Resolve(parent);
			return binding != resolved;
		}
		
		public void OnResolved(SemanticStep parent)
		{
			_resolver.OnResolved(parent);
		}
	}
	
	public class DependencyGraph
	{
		ArrayList _nodes = new ArrayList();
		
		Hashtable _nodeMap = new Hashtable();
		
		ArrayList _buffer = new ArrayList();
		
		CompilerContext _context;
		
		public DependencyGraph(CompilerContext context)
		{
			_context = context;
		}
		
		public void Add(Node node, ITypeResolver resolver)
		{				
			if (_nodeMap.ContainsKey(node))
			{
				throw new ArgumentException(string.Format("Node '{0}' is already in the graph!", node));
			}
			if (!IsUnknown(node))
			{
				throw new ArgumentException(string.Format("Node '{0}' is not bound to unknown!", node));
			}
			
			_context.TraceInfo("{0}: Node '{1}' added to the dependency graph.", node.LexicalInfo, node);
			
			GraphNode gnode = new GraphNode(node, resolver);			
			_nodes.Add(gnode);
			_nodeMap.Add(node, gnode);
		}
		
		bool IsUnknown(Node node)
		{
			return UnknownBinding.Default == ((ITypedBinding)BindingManager.GetBinding(node)).BoundType;
		}
		
		public int Resolve(SemanticStep parent)
		{
			if (0 == _nodes.Count)
			{
				return 0;
			}
			
			int iterations = 1;
			while (true)
			{	
				bool changed = false;			
				foreach (GraphNode node in _nodes)
				{
					if (node.Resolve(parent))
					{
						changed = true;
					}
				}
				if (!changed)
				{
					break;
				}				
				++iterations;
			}
			
			foreach (GraphNode node in _nodes)
			{
				node.OnResolved(parent);
			}
			
			return iterations;
		}
	}
}
