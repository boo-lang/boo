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
	using Boo.Lang.Compiler.Services;

	public class NamespaceInfo : IInfo, INamespace
	{		
		DefaultInfoService _bindingService;
		
		INamespace _parent;
		
		string _name;
		
		Hashtable _assemblies;
		
		Hashtable _childrenNamespaces;
		
		ArrayList _moduleNamespaces;
		
		public NamespaceInfo(INamespace parent, DefaultInfoService bindingManager, string name)
		{			
			_parent = parent;
			_bindingService = bindingManager;
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
		
		public InfoType InfoType
		{
			get
			{
				return InfoType.Namespace;
			}
		}
		
		public void Add(Type type)
		{
			Assembly assembly = type.Assembly;
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
		
		public NamespaceInfo GetChildNamespace(string name)
		{
			NamespaceInfo binding = (NamespaceInfo)_childrenNamespaces[name];
			if (null == binding)
			{				
				binding = new NamespaceInfo(this, _bindingService, _name + "." + name);
				_childrenNamespaces[name] = binding;
			}
			return binding;
		}
		
		internal IInfo Resolve(string name, Assembly assembly)
		{
			NamespaceInfo binding = (NamespaceInfo)_childrenNamespaces[name];
			if (null != binding)
			{
				return new AssemblyQualifiedNamespaceInfo(assembly, binding);
			}
			
			ArrayList types = (ArrayList)_assemblies[assembly];			                
			if (null != types)
			{
				foreach (Type type in types)
				{
					if (name == type.Name)
					{
						return _bindingService.AsTypeReference(type);
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
		
		public IInfo Resolve(string name)
		{	
			IInfo binding = (IInfo)_childrenNamespaces[name];
			if (null == binding)
			{
				binding = ResolveInternalType(name);
				if (null == binding)
				{
					binding = ResolveExternalType(name);
				}				
			}
			return binding;
		}
		
		IInfo ResolveInternalType(string name)
		{
			IInfo binding = null;
			foreach (ModuleInfo ns in _moduleNamespaces)
			{
				binding = ns.ResolveMember(name);
				if (null != binding)
				{
					break;
				}
			}
			return binding;
		}
		
		IInfo ResolveExternalType(string name)
		{
			IInfo binding = null;
			foreach (ArrayList types in _assemblies.Values)
			{
				foreach (Type type in types)
				{
					if (name == type.Name)
					{
						binding = _bindingService.AsTypeReference(type);
						break;
					}
				}
			}
			return binding;
		}
		
		override public string ToString()
		{
			return _name;
		}
	}
	
	public class AssemblyQualifiedNamespaceInfo : IInfo, INamespace
	{
		Assembly _assembly;
		NamespaceInfo _subject;
		
		public AssemblyQualifiedNamespaceInfo(Assembly assembly, NamespaceInfo subject)
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
		
		public InfoType InfoType
		{
			get
			{
				return InfoType.Namespace;
			}
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return _subject.ParentNamespace;
			}
		}
		
		public IInfo Resolve(string name)
		{
			return _subject.Resolve(name, _assembly);
		}
	}
	
	public class AliasedNamespaceInfo : IInfo, INamespace
	{
		string _alias;
		IInfo _subject;
		
		public AliasedNamespaceInfo(string alias, IInfo subject)
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
		
		public InfoType InfoType
		{
			get
			{
				return InfoType.Namespace;
			}
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return ((INamespace)_subject).ParentNamespace;
			}
		}
		
		public IInfo Resolve(string name)
		{
			if (name == _alias)
			{
				return _subject;
			}
			return null;
		}
	}
}
