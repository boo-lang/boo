using System;
using System.Reflection;
using System.Collections;

namespace Boo.Ast.Compilation.Binding
{
	public class NamespaceBinding : IBinding, INamespace
	{		
		BindingManager _bindingManager;
		
		string _name;
		
		Hashtable _assemblies;
		
		Hashtable _childrenNamespaces;
		
		public NamespaceBinding(BindingManager bindingManager, string name)
		{			
			_bindingManager = bindingManager;
			_name = name;
			_assemblies = new Hashtable();
			_childrenNamespaces = new Hashtable();
			_assemblies = new Hashtable();
		}
		
		public string Name
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
		
		public NamespaceBinding GetChildNamespace(string name)
		{
			NamespaceBinding binding = (NamespaceBinding)_childrenNamespaces[name];
			if (null == binding)
			{				
				binding = new NamespaceBinding(_bindingManager, _name + "." + name);
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
						return _bindingManager.ToTypeReference(type);
					}
				}
			}
			return null;
		}
		
		public IBinding Resolve(string name)
		{	
			IBinding binding = (IBinding)_childrenNamespaces[name];
			if (null != binding)
			{
				return binding;
			}
			
			foreach (ArrayList types in _assemblies.Values)
			{
				foreach (Type type in types)
				{
					if (name == type.Name)
					{
						return _bindingManager.ToTypeReference(type);
					}
				}
			}
			return null;
		}
		
		public override string ToString()
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
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Namespace;
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
