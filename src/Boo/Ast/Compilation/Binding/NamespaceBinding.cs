using System;
using System.Collections;

namespace Boo.Ast.Compilation.Binding
{
	public class NamespaceBinding : IBinding, INameSpace
	{
		public class AssemblyInfo
		{
			public System.Reflection.Assembly Assembly;
			
			public Type[] Types;
			
			public AssemblyInfo(System.Reflection.Assembly assembly, Type[] types)
			{
				Assembly = assembly;
				Types = types;
			}
			
			public override int GetHashCode()
			{
				return Assembly.GetHashCode();
			}
			
			public override bool Equals(object other)
			{
				return ((AssemblyInfo)other).Assembly == Assembly;
			}
		}
		
		BindingManager _bindingManager;
		
		string _name;
		
		ArrayList _assemblies;
		
		Hashtable _childrenNamespaces;
		
		public NamespaceBinding(BindingManager bindingManager, string name)
		{			
			_bindingManager = bindingManager;
			_name = name;
			_assemblies = new ArrayList();
			_childrenNamespaces = new Hashtable();
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
		
		public void Add(AssemblyInfo info)
		{
			if (!_assemblies.Contains(info))
			{
				_assemblies.Add(info);
			}
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
		
		public IBinding Resolve(string name)
		{	
			IBinding binding = (IBinding)_childrenNamespaces[name];
			if (null != binding)
			{
				return binding;
			}
			
			foreach (AssemblyInfo info in _assemblies)
			{
				foreach (Type type in info.Types)
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
	
	public class AliasedNamespaceBinding : IBinding, INameSpace
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
