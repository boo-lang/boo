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
	using System.Reflection;
	using System.Collections;

	public class Namespace : IElement, INamespace
	{		
		TagService _tagService;
		
		INamespace _parent;
		
		string _name;
		
		Hashtable _assemblies;
		
		Hashtable _childrenNamespaces;
		
		ArrayList _moduleNamespaces;
		
		public Namespace(INamespace parent, TagService tagManager, string name)
		{			
			_parent = parent;
			_tagService = tagManager;
			_name = name;
			_assemblies = new Hashtable();
			_childrenNamespaces = new Hashtable();
			_assemblies = new Hashtable();
			_moduleNamespaces = new ArrayList();
		}
		
		public string Name
		{
			get
			{
				return _name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _name;
			}
		}
		
		public ElementType ElementType
		{
			get
			{
				return ElementType.Namespace;
			}
		}
		
		public void Add(Type type)
		{
			System.Reflection.Assembly assembly = type.Assembly;
			ArrayList types = (ArrayList)_assemblies[assembly];
			if (null == types)
			{
				types = new ArrayList();
				_assemblies[assembly] = types;
			}
			types.Add(type);			
		}
		
		public void AddModule(ModuleInfo module)
		{
			_moduleNamespaces.Add(module);
		}
		
		public Namespace GetChildNamespace(string name)
		{
			Namespace tag = (Namespace)_childrenNamespaces[name];
			if (null == tag)
			{				
				tag = new Namespace(this, _tagService, _name + "." + name);
				_childrenNamespaces[name] = tag;
			}
			return tag;
		}
		
		internal IElement Resolve(string name, System.Reflection.Assembly assembly)
		{
			Namespace tag = (Namespace)_childrenNamespaces[name];
			if (null != tag)
			{
				return new AssemblyQualifiedNamespace(assembly, tag);
			}
			
			ArrayList types = (ArrayList)_assemblies[assembly];			                
			if (null != types)
			{
				foreach (Type type in types)
				{
					if (name == type.Name)
					{
						return _tagService.GetTypeReference(type);
					}
				}
			}
			return null;
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
			IElement tag = (IElement)_childrenNamespaces[name];
			if (null == tag)
			{
				tag = ResolveInternalType(name);
				if (null == tag)
				{
					tag = ResolveExternalType(name);
				}				
			}
			return tag;
		}
		
		IElement ResolveInternalType(string name)
		{
			IElement tag = null;
			foreach (ModuleInfo ns in _moduleNamespaces)
			{
				tag = ns.ResolveMember(name);
				if (null != tag)
				{
					break;
				}
			}
			return tag;
		}
		
		IElement ResolveExternalType(string name)
		{
			IElement tag = null;
			foreach (ArrayList types in _assemblies.Values)
			{
				foreach (Type type in types)
				{
					if (name == type.Name)
					{
						tag = _tagService.GetTypeReference(type);
						break;
					}
				}
			}
			return tag;
		}
		
		override public string ToString()
		{
			return _name;
		}
	}
	
	public class AssemblyQualifiedNamespace : IElement, INamespace
	{
		System.Reflection.Assembly _assembly;
		Namespace _subject;
		
		public AssemblyQualifiedNamespace(System.Reflection.Assembly assembly, Namespace subject)
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
		
		public IElement Resolve(string name)
		{
			return _subject.Resolve(name, _assembly);
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
		
		public IElement Resolve(string name)
		{
			if (name == _alias)
			{
				return _subject;
			}
			return null;
		}
	}
}
