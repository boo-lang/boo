#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using System.Collections;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Bindings;
	
	[Serializable]
	public class BindBaseTypes : AbstractSwitcherCompilerStep
	{
		protected NameResolutionSupport _nameResolution = new NameResolutionSupport();
		
		public BindBaseTypes()
		{
		}
		
		override public void Run()
		{
			_nameResolution.Initialize(_context);
			
			Switch(CompileUnit.Modules);
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
