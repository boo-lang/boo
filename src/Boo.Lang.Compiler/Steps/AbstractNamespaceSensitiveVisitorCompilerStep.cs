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
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;	
	using Boo.Lang.Compiler.Taxonomy;
	
	public abstract class AbstractNamespaceSensitiveVisitorCompilerStep : AbstractVisitorCompilerStep
	{
		protected NameResolutionSupport _nameResolution = new NameResolutionSupport();
		
		override public void Initialize(CompilerContext context)
		{
			base.Initialize(context);
			_nameResolution.Initialize(context);
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
		
		protected IInfo Resolve(Node sourceNode, string name, InfoType bindings)
		{
			return _nameResolution.Resolve(sourceNode, name, bindings);
		}
		
		protected IInfo Resolve(Node sourceNode, string name)
		{
			return _nameResolution.Resolve(sourceNode, name);
		}
		
		protected IInfo ResolveQualifiedName(Node sourceNode, string name)
		{
			return _nameResolution.ResolveQualifiedName(sourceNode, name);
		}
		
		protected bool IsQualifiedName(string name)
		{
			return name.IndexOf('.') > 0;
		}	
	
		protected InternalTypeInfo GetInternalTypeInfo(TypeDefinition node)
		{
			InternalTypeInfo binding = (InternalTypeInfo)InfoService.GetOptionalInfo(node);
			if (null == binding)
			{
				binding = new InternalTypeInfo(InfoService, node);
				Bind(node, binding);
			}
			return binding;
		}
		
		protected IInfo ResolveSimpleTypeReference(SimpleTypeReference node)
		{
			if (InfoService.IsBound(node))
			{
				return null;
			}
			
			IInfo info = null;
			if (IsQualifiedName(node.Name))
			{
				info = ResolveQualifiedName(node, node.Name);
			}
			else
			{
				info = Resolve(node, node.Name, InfoType.TypeReference);
			}
			
			if (null == info || InfoType.TypeReference != info.InfoType)
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
