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
		
		protected void EnterNamespace(INamespace ns)
		{
			_nameResolution.EnterNamespace(ns);
		}
		
		protected INamespace CurrentNamespace
		{
			get
			{
				return _nameResolution.CurrentNamespace;
			}
		}
		
		protected void LeaveNamespace()
		{
			_nameResolution.LeaveNamespace();
		}
		
		protected IElement Resolve(Node sourceNode, string name, ElementType tags)
		{
			return _nameResolution.Resolve(sourceNode, name, tags);
		}
		
		protected IElement Resolve(Node sourceNode, string name)
		{
			return _nameResolution.Resolve(sourceNode, name);
		}
		
		protected IElement ResolveQualifiedName(Node sourceNode, string name)
		{
			return _nameResolution.ResolveQualifiedName(sourceNode, name);
		}
		
		protected bool IsQualifiedName(string name)
		{
			return name.IndexOf('.') > 0;
		}	
	
		protected InternalType GetInternalType(TypeDefinition node)
		{
			return (InternalType)node.Tag;
		}
		
		protected IElement ResolveSimpleTypeReference(SimpleTypeReference node)
		{
			if (null != node.Tag)
			{
				return null;
			}
			
			IElement info = null;
			if (IsQualifiedName(node.Name))
			{
				info = ResolveQualifiedName(node, node.Name);
			}
			else
			{
				info = Resolve(node, node.Name, ElementType.TypeReference);
			}
			
			if (null == info || ElementType.TypeReference != info.ElementType)
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
