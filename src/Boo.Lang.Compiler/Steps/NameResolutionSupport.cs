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
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Taxonomy;
	
	public class NameResolutionSupport : IDisposable
	{
		static readonly char[] DotArray = new char[] { '.' };
		
		protected CompilerContext _context;
		
		protected INamespace _current;
		
		public void Initialize(CompilerContext context)
		{
			_context = context;
			
			PushNamespace((INamespace)TaxonomyHelper.GetInfo(context.CompileUnit));
		}
		
		public INamespace CurrentNamespace
		{
			get
			{
				return _current;
			}
		}		
		
		public void Dispose()
		{
			_context = null;
			_current = null;
		}
		
		public IInfo Resolve(Node sourceNode, string name)
		{
			return Resolve(sourceNode, name, InfoType.Any);
		}
		
		public IInfo Resolve(Node sourceNode, string name, InfoType bindings)
		{
			if (null == sourceNode)
			{
				throw new ArgumentNullException("sourceNode");
			}
			
			IInfo binding = _context.TaxonomyHelper.ResolvePrimitive(name);
			if (null == binding)
			{
				INamespace ns = _current;
				while (null != ns)
				{
					_context.TraceVerbose("Trying to resolve {0} against {1}...", name, ns);
					binding = ns.Resolve(name);
					if (null != binding)
					{
						if (IsFlagSet(bindings, binding.InfoType))
						{
							break;
						}
					}
					ns = ns.ParentNamespace;
				}
			}
			
			if (null != binding)
			{
				_context.TraceInfo("{0}: {1} bound to {2}.", sourceNode.LexicalInfo, name, binding);
			}
			return binding;
		}
		
		public IInfo ResolveQualifiedName(Node sourceNode, string name)
		{			
			string[] parts = name.Split(DotArray);
			string topLevel = parts[0];
			IInfo binding = Resolve(sourceNode, topLevel);
			for (int i=1; i<parts.Length; ++i)				
			{				
				INamespace ns = binding as INamespace;
				if (null == ns)
				{
					binding = null;
					break;
				}
				binding = ns.Resolve(parts[i]);
			}
			return binding;
		}
		
		static bool IsFlagSet(InfoType bindings, InfoType binding)
		{
			return binding == (bindings & binding);
		}
		
		public void Restore(INamespace saved)
		{
			_current = saved;
		}		
		
		public void PushNamespace(INamespace ns)
		{
			if (null == ns)
			{
				throw new ArgumentNullException("ns");
			}
			_current = ns;
		}
		
		public void PopNamespace()
		{
			_current = _current.ParentNamespace;
		}		
	}
}
