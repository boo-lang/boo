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

namespace Boo.Lang.Compiler.Taxonomy
{
	using System;	
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	
	public class NameResolutionService
	{
		static readonly char[] DotArray = new char[] { '.' };
		
		protected CompilerContext _context;
		
		protected INamespace _current;
		
		protected INamespace _global;
		
		public NameResolutionService(CompilerContext context)
		{
			if (null == context)
			{
				throw new ArgumentNullException("context");
			}
			_context = context;
		}
		
		public INamespace GlobalNamespace
		{
			get
			{
				return _global;
			}
			
			set
			{
				_global = value;
			}
		}		
		
		public void EnterNamespace(INamespace ns)
		{
			if (null == ns)
			{
				throw new ArgumentNullException("ns");
			}
			_current = ns;
		}
		
		public INamespace CurrentNamespace
		{
			get
			{
				return _current;
			}
		}
		
		public void Reset()
		{
			if (null == _global)
			{
				throw new InvalidOperationException(Boo.ResourceManager.GetString("GlobalNamespaceIsNotSet"));
			}
			EnterNamespace(_global);
		}
		
		public void Restore(INamespace saved)
		{
			if (null == saved)
			{
				throw new ArgumentNullException("saved");
			}
			_current = saved;
		}
		
		public void LeaveNamespace()
		{
			_current = _current.ParentNamespace;
		}
		
		public IElement Resolve(Node sourceNode, string name)
		{
			return Resolve(sourceNode, name, ElementType.Any);
		}
		
		public IElement Resolve(Node sourceNode, string name, ElementType tags)
		{
			if (null == sourceNode)
			{
				throw new ArgumentNullException("sourceNode");
			}
			
			IElement tag = _context.TagService.ResolvePrimitive(name);
			if (null == tag)
			{
				INamespace ns = _current;
				while (null != ns)
				{
					_context.TraceVerbose("Trying to resolve {0} against {1}...", name, ns);
					tag = ns.Resolve(name);
					if (null != tag)
					{
						if (IsFlagSet(tags, tag.ElementType))
						{
							break;
						}
					}
					ns = ns.ParentNamespace;
				}
			}
			
			if (null != tag)
			{
				_context.TraceInfo("{0}: {1} bound to {2}.", sourceNode.LexicalInfo, name, tag);
			}
			return tag;
		}
		
		public IElement ResolveQualifiedName(Node sourceNode, string name)
		{			
			string[] parts = name.Split(DotArray);
			string topLevel = parts[0];
			IElement tag = Resolve(sourceNode, topLevel);
			for (int i=1; i<parts.Length; ++i)				
			{				
				INamespace ns = tag as INamespace;
				if (null == ns)
				{
					tag = null;
					break;
				}
				tag = ns.Resolve(parts[i]);
			}
			return tag;
		}
		
		public void ResolveTypeReference(TypeReference node)
		{
			if (NodeType.ArrayTypeReference == node.NodeType)
			{
				ResolveArrayTypeReference((ArrayTypeReference)node);
			}
			else
			{
				ResolveSimpleTypeReference((SimpleTypeReference)node);
			}
		}
		
		public void ResolveArrayTypeReference(ArrayTypeReference node)
		{
			if (node.Tag != null)
			{
				return;
			}

			ResolveTypeReference(node.ElementType);
			
			IType elementType = TagService.GetType(node.ElementType);
			if (TagService.IsError(elementType))
			{
				node.Tag = TagService.ErrorTag;
			}
			else
			{
				node.Tag = _context.TagService.GetArrayType(elementType);
			}
		}
		
		public void ResolveSimpleTypeReference(SimpleTypeReference node)
		{
			if (null != node.Tag)
			{
				return;
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
				_context.Errors.Add(CompilerErrorFactory.NameNotType(node, node.Name));
				info = TagService.ErrorTag;
			}
			else
			{
				node.Name = info.Name;
			}
			
			node.Tag = info;
		}
		
		static bool IsQualifiedName(string name)
		{
			return name.IndexOf('.') > 0;
		}	
		
		static bool IsFlagSet(ElementType tags, ElementType tag)
		{
			return tag == (tags & tag);
		}		
	}
}
