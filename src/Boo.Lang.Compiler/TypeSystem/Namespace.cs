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

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Reflection;
	using System.Collections;
	
	public class SimpleNamespace : INamespace
	{		
		INamespace _parent;
		IDictionary _children;
		
		public SimpleNamespace(INamespace parent, IDictionary children)
		{
			if (null == children)
			{
				throw new ArgumentNullException("children");
			}
			_parent = parent;
			_children = children;			
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
			return (IElement)_children[name];
		}
		
		public bool Resolve(Boo.Lang.List targetList, string name, ElementType flags)
		{
			IElement element = Resolve(name);
			if (null != element && NameResolutionService.IsFlagSet(flags, element.ElementType))
			{
				targetList.Add(element);
				return true;
			}
			return false;
		}
	}
	
	public class AssemblyQualifiedNamespaceTag : IElement, INamespace
	{
		System.Reflection.Assembly _assembly;
		NamespaceTag _subject;
		
		public AssemblyQualifiedNamespaceTag(System.Reflection.Assembly assembly, NamespaceTag subject)
		{
			_assembly = assembly;
			_subject = subject;
		}
		
		public string Name
		{
			get
			{
				return _subject.Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return string.Format("{0}, {1}", _subject.Name, _assembly);
			}
		}
		
		public ElementType ElementType
		{
			get
			{
				return ElementType.Namespace;
			}
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return _subject.ParentNamespace;
			}
		}
		
		public bool Resolve(Boo.Lang.List targetList, string name, ElementType flags)
		{
			return _subject.Resolve(targetList, name, _assembly, flags);
		}
	}
	
	public class AliasedNamespace : IElement, INamespace
	{
		string _alias;
		IElement _subject;
		
		public AliasedNamespace(string alias, IElement subject)
		{
			_alias = alias;			
			_subject = subject;
		}
		
		public string Name
		{
			get
			{
				return _alias;
			}
		}
		
		public string FullName
		{
			get
			{
				return _subject.FullName;
			}
		}
		
		public ElementType ElementType
		{
			get
			{
				return ElementType.Namespace;
			}
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return ((INamespace)_subject).ParentNamespace;
			}
		}
		
		public bool Resolve(Boo.Lang.List targetList, string name, ElementType flags)
		{
			if (name == _alias && NameResolutionService.IsFlagSet(flags, _subject.ElementType))
			{
				targetList.Add(_subject);
				return true;
			}
			return false;
		}
	}
	
	public class NamespaceDelegator : INamespace
	{
		INamespace _parent;
		
		INamespace[] _namespaces;
		
		public NamespaceDelegator(INamespace parent, INamespace[] namespaces)
		{
			_parent = parent;
			_namespaces = namespaces;
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return _parent;
			}
		}
		
		public bool Resolve(Boo.Lang.List targetList, string name, ElementType flags)
		{
			bool found = false;
			foreach (INamespace ns in _namespaces)
			{
				if (ns.Resolve(targetList, name, flags))
				{
					found = true;
				}
			}
			return found;
		}
	}
	
	class DeclarationsNamespace : INamespace
	{
		INamespace _parent;
		TypeSystemServices _tagService;
		Boo.Lang.Compiler.Ast.DeclarationCollection _declarations;
		
		public DeclarationsNamespace(INamespace parent, TypeSystemServices tagManager, Boo.Lang.Compiler.Ast.DeclarationCollection declarations)
		{
			_parent = parent;
			_tagService = tagManager;
			_declarations = declarations;
		}
		
		public DeclarationsNamespace(INamespace parent, TypeSystemServices tagManager, Boo.Lang.Compiler.Ast.Declaration declaration)
		{
			_parent = parent;
			_tagService = tagManager;
			_declarations = new Boo.Lang.Compiler.Ast.DeclarationCollection();
			_declarations.Add(declaration);
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return _parent;
			}
		}
		
		public bool Resolve(Boo.Lang.List targetList, string name, ElementType flags)
		{
			Boo.Lang.Compiler.Ast.Declaration found = _declarations[name];
			if (null != found)
			{
				IElement element = TypeSystemServices.GetTag(found);
				if (NameResolutionService.IsFlagSet(flags, element.ElementType))
				{
					targetList.Add(element);
					return true;
				}
			}
			return false;
		}
	}

}
