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

namespace Boo.Lang.Compiler.Bindings
{
	using System;
	using System.Reflection;
	using System.Collections;
	using Boo.Lang.Compiler.Services;

	public class NamespaceBinding : IBinding, INamespace
	{		
		DefaultBindingService _bindingService;
		
		INamespace _parent;
		
		string _name;
		
		Hashtable _assemblies;
		
		Hashtable _childrenNamespaces;
		
		ArrayList _moduleNamespaces;
		
		public NamespaceBinding(INamespace parent, DefaultBindingService bindingManager, string name)
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
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Namespace;
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
		
		public void AddModule(ModuleBinding module)
		{
			_moduleNamespaces.Add(module);
		}
		
		public NamespaceBinding GetChildNamespace(string name)
		{
			NamespaceBinding binding = (NamespaceBinding)_childrenNamespaces[name];
			if (null == binding)
			{				
				binding = new NamespaceBinding(this, _bindingService, _name + "." + name);
				_childrenNamespaces[name] = binding;
			}
			return binding;
		}
		
		internal IBinding Resolve(string name, Assembly assembly)
		{
			NamespaceBinding binding = (NamespaceBinding)_childrenNamespaces[name];
			if (null != binding)
			{
				return new AssemblyQualifiedNamespaceBinding(assembly, binding);
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
		
		public IBinding Resolve(string name)
		{	
			IBinding binding = (IBinding)_childrenNamespaces[name];
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
		
		IBinding ResolveInternalType(string name)
		{
			IBinding binding = null;
			foreach (ModuleBinding ns in _moduleNamespaces)
			{
				binding = ns.ResolveMember(name);
				if (null != binding)
				{
					break;
				}
			}
			return binding;
		}
		
		IBinding ResolveExternalType(string name)
		{
			IBinding binding = null;
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
	
	public class AssemblyQualifiedNamespaceBinding : IBinding, INamespace
	{
		Assembly _assembly;
		NamespaceBinding _subject;
		
		public AssemblyQualifiedNamespaceBinding(Assembly assembly, NamespaceBinding subject)
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
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Namespace;
			}
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return _subject.ParentNamespace;
			}
		}
		
		public IBinding Resolve(string name)
		{
			return _subject.Resolve(name, _assembly);
		}
	}
	
	public class AliasedNamespaceBinding : IBinding, INamespace
	{
		string _alias;
		IBinding _subject;
		
		public AliasedNamespaceBinding(string alias, IBinding subject)
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
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Namespace;
			}
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return ((INamespace)_subject).ParentNamespace;
			}
		}
		
		public IBinding Resolve(string name)
		{
			if (name == _alias)
			{
				return _subject;
			}
			return null;
		}
	}
}
