namespace Boo.Lang.Compiler.Pipeline
{
	using System;
	using System.Collections;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Bindings;
	
	[Serializable]
	public class TypeHierarchyResolver : AbstractSwitcherCompilerStep
	{
		protected NameResolutionSupport _nameResolution = new NameResolutionSupport();
		
		public TypeHierarchyResolver()
		{
		}
		
		override public void Run()
		{
			_nameResolution.Initialize(_context);
			
			foreach (Module module in CompileUnit.Modules)
			{
				Switch(module);
			}
		}
		
		override public void OnModule(Module module)
		{
			PushNamespace((INamespace)GetBinding(module));
			Switch(module.Members);
			PopNamespace();
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{			
			ResolveBaseTypes(new ArrayList(), node);
		}
		
		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{
			ResolveBaseTypes(new ArrayList(), node);
		}
		
		protected void ResolveBaseTypes(ArrayList visited, TypeDefinition node)
		{
			visited.Add(node);
			foreach (SimpleTypeReference type in node.BaseTypes)
			{                            
				TypeReferenceBinding binding = ResolveSimpleTypeReference(type) as TypeReferenceBinding;
				if (null != binding)
				{
					InternalTypeBinding internalType = binding.BoundType as InternalTypeBinding;
					if (null != internalType)
					{
						if (visited.Contains(internalType.TypeDefinition))
						{
							Error(CompilerErrorFactory.InheritanceCycle(type, internalType.FullName));
						}
						else
						{
							ResolveBaseTypes(visited, internalType.TypeDefinition);
						}
					}
				}
			}
		}
		
		override public void Dispose()
		{
			base.Dispose();
			
			_nameResolution.Dispose();
		}
		
		protected void PushNamespace(INamespace ns)
		{
			_nameResolution.PushNamespace(ns);
		}
		
		protected INamespace CurrentNamespace
		{
			get
			{
				return _nameResolution.CurrentNamespace;
			}
		}
		
		protected void PopNamespace()
		{
			_nameResolution.PopNamespace();
		}
		
		protected IBinding Resolve(Node sourceNode, string name, BindingType bindings)
		{
			return _nameResolution.Resolve(sourceNode, name, bindings);
		}
		
		protected IBinding Resolve(Node sourceNode, string name)
		{
			return _nameResolution.Resolve(sourceNode, name);
		}
		
		protected bool IsQualifiedName(string name)
		{
			return name.IndexOf('.') > 0;
		}
		
		protected IBinding ResolveQualifiedName(Node sourceNode, string name)
		{
			return _nameResolution.ResolveQualifiedName(sourceNode, name);
		}
		
		protected InternalTypeBinding GetInternalTypeBinding(TypeDefinition node)
		{
			InternalTypeBinding binding = (InternalTypeBinding)BindingManager.GetOptionalBinding(node);
			if (null == binding)
			{
				binding = new InternalTypeBinding(BindingManager, node);
				Bind(node, binding);
			}
			return binding;
		}
		
		protected IBinding ResolveSimpleTypeReference(SimpleTypeReference node)
		{
			if (BindingManager.IsBound(node))
			{
				return null;
			}
			
			IBinding info = null;
			if (IsQualifiedName(node.Name))
			{
				info = ResolveQualifiedName(node, node.Name);
			}
			else
			{
				info = Resolve(node, node.Name, BindingType.TypeReference);
			}
			
			if (null == info || BindingType.TypeReference != info.BindingType)
			{
				Error(CompilerErrorFactory.NameNotType(node, node.Name));
				Error(node);
			}
			else
			{
				node.Name = info.Name;
				Bind(node, info);
			}
			
			return info;
		}
	}
}
