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
	using System.Collections;
	using Boo.Lang.Compiler.Ast;
	
	public interface INamespace
	{			
		INamespace ParentNamespace
		{
			get;
		}
		
		IElement Resolve(string name);
	}
	
	public class NullNamespace : INamespace
	{
		public static readonly INamespace Default = new NullNamespace();
		
		private NullNamespace()
		{
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return null;
			}
		}
		
		public IElement Resolve(string name)
		{
			return null;
		}
	}
	
	class DeclarationsNamespace : INamespace
	{
		INamespace _parent;
		TagService _tagService;
		DeclarationCollection _declarations;
		
		public DeclarationsNamespace(INamespace parent, TagService tagManager, DeclarationCollection declarations)
		{
			_parent = parent;
			_tagService = tagManager;
			_declarations = declarations;
		}
		
		public DeclarationsNamespace(INamespace parent, TagService tagManager, Declaration declaration)
		{
			_parent = parent;
			_tagService = tagManager;
			_declarations = new DeclarationCollection();
			_declarations.Add(declaration);
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return _parent;
			}
		}
		
		public IElement Resolve(string name)
		{
			Declaration d = _declarations[name];
			if (null != d)
			{
				return TagService.GetTag(d);
			}
			return null;
		}
	}	
}
