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
		
		public Node[] Dependencies;

		ITypeResolver _resolver;
		
		bool _changed = false;

		public GraphNode(Node node, ITypeResolver resolver, Node[] deps)
		{
			Node = node;
			_resolver = resolver;
			Dependencies = deps;
		}
		
		public bool Changed
		{
			get
			{
				return _changed;
			}
		}
		
		public bool Resolve(SemanticStep parent)
		{			
			IBinding binding = ((ITypedBinding)BindingManager.GetBinding(Node)).BoundType;
			IBinding resolved = _resolver.Resolve(parent);
			_changed = binding != resolved;
			return _changed;
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
		
		public void Add(Node node, ITypeResolver resolver, params Node[] nodes)
		{
			Add(node, resolver, (IEnumerable)nodes);
		}
		
		public void Add(Node node, ITypeResolver resolver, IEnumerable dependencies)
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
			
			GraphNode gnode = new GraphNode(node, resolver, FilterDependencies(dependencies));			
			_nodes.Add(gnode);
			_nodeMap.Add(node, gnode);
		}
		
		Node[] FilterDependencies(IEnumerable nodes)
		{
			_buffer.Clear();
			foreach (Node node in nodes)
			{
				if (IsUnknown(node))
				{
					_buffer.Add(node);
				}
			}
			return (Node[])_buffer.ToArray(typeof(Node));
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
		
		GraphNode GetGraphNode(Node node)
		{
			GraphNode found = (GraphNode)_nodeMap[node];
			if (null == found)
			{
				throw new ApplicationException(string.Format("{0}: Node '{1}' not found in dependency graph!", node.LexicalInfo, node));
			}
			return found;
		}
		
		bool AnyNodeChanged(Node[] nodes)
		{
			foreach (Node node in nodes)
			{				
				if (GetGraphNode(node).Changed)
				{
					return true;
				}
			}
			return false;
		}
	}
}
