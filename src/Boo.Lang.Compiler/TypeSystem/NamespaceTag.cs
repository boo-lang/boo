namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Collections;
	
	public class NamespaceTag : IElement, INamespace
	{		
		TypeSystemServices _tagService;
		
		INamespace _parent;
		
		string _name;
		
		Hashtable _assemblies;
		
		Hashtable _childrenNamespaces;
		
		Boo.Lang.List _moduleNamespaces;
		
		public NamespaceTag(INamespace parent, TypeSystemServices tagManager, string name)
		{			
			_parent = parent;
			_tagService = tagManager;
			_name = name;
			_assemblies = new Hashtable();
			_childrenNamespaces = new Hashtable();
			_assemblies = new Hashtable();
			_moduleNamespaces = new Boo.Lang.List();
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
			Boo.Lang.List types = (Boo.Lang.List)_assemblies[assembly];
			if (null == types)
			{
				types = new Boo.Lang.List();
				_assemblies[assembly] = types;
			}
			types.Add(type);			
		}
		
		public void AddModule(Boo.Lang.Compiler.TypeSystem.ModuleTag module)
		{
			_moduleNamespaces.Add(module);
		}
		
		public NamespaceTag GetChildNamespace(string name)
		{
			NamespaceTag tag = (NamespaceTag)_childrenNamespaces[name];
			if (null == tag)
			{				
				tag = new NamespaceTag(this, _tagService, _name + "." + name);
				_childrenNamespaces[name] = tag;
			}
			return tag;
		}
		
		internal bool Resolve(Boo.Lang.List targetList, string name, System.Reflection.Assembly assembly, ElementType flags)
		{
			NamespaceTag tag = (NamespaceTag)_childrenNamespaces[name];
			if (null != tag)
			{
				targetList.Add(new AssemblyQualifiedNamespaceTag(assembly, tag));
				return true;
			}
			
			Boo.Lang.List types = (Boo.Lang.List)_assemblies[assembly];			                
			if (null != types)
			{
				foreach (Type type in types)
				{
					if (name == type.Name)
					{
						targetList.Add(_tagService.GetTypeReference(type));
						return true;
					}
				}
			}
			return false;
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
			IElement tag = (IElement)_childrenNamespaces[name];
			if (null != tag)
			{
				targetList.Add(tag);
				return true;
			}
			
			if (!ResolveInternalType(targetList, name))
			{
				return ResolveExternalType(targetList, name);
			}
			return false;
		}
		
		bool ResolveInternalType(Boo.Lang.List targetList, string name)
		{
			foreach (ModuleTag ns in _moduleNamespaces)
			{
				IElement tag = ns.ResolveMember(name);
				if (null != tag)
				{
					targetList.Add(tag);
					return true;
				}
			}
			return false;
		}
		
		bool ResolveExternalType(Boo.Lang.List targetList, string name)
		{
			foreach (Boo.Lang.List types in _assemblies.Values)
			{
				foreach (Type type in types)
				{
					if (name == type.Name)
					{
						targetList.Add(_tagService.GetTypeReference(type));
						return true;
					}
				}
			}
			return false;
		}
		
		override public string ToString()
		{
			return _name;
		}
	}
}
