#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
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
	using System.Collections;
	
	public class NamespaceEntity : IEntity, INamespace
	{		
		TypeSystemServices _typeSystemServices;
		
		INamespace _parent;
		
		string _name;
		
		Hashtable _assemblies;
		
		Hashtable _childrenNamespaces;
		
		Boo.Lang.List _moduleNamespaces;
		
		public NamespaceEntity(INamespace parent, TypeSystemServices tagManager, string name)
		{			
			_parent = parent;
			_typeSystemServices = tagManager;
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
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.Namespace;
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
		
		public void AddModule(Boo.Lang.Compiler.TypeSystem.ModuleEntity module)
		{
			_moduleNamespaces.Add(module);
		}
		
		public NamespaceEntity GetChildNamespace(string name)
		{
			NamespaceEntity tag = (NamespaceEntity)_childrenNamespaces[name];
			if (null == tag)
			{				
				tag = new NamespaceEntity(this, _typeSystemServices, _name + "." + name);
				_childrenNamespaces[name] = tag;
			}
			return tag;
		}
		
		internal bool Resolve(Boo.Lang.List targetList, string name, System.Reflection.Assembly assembly, EntityType flags)
		{
			NamespaceEntity tag = (NamespaceEntity)_childrenNamespaces[name];
			if (null != tag)
			{
				targetList.Add(new AssemblyQualifiedNamespaceEntity(assembly, tag));
				return true;
			}
			
			Boo.Lang.List types = (Boo.Lang.List)_assemblies[assembly];			                
			if (null != types)
			{
				foreach (Type type in types)
				{
					if (name == type.Name)
					{
						targetList.Add(_typeSystemServices.Map(type));
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
		
		public bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
		{	
			IEntity tag = (IEntity)_childrenNamespaces[name];
			if (null != tag)
			{
				targetList.Add(tag);
				return true;
			}
			
			if (!ResolveInternalType(targetList, name, flags))
			{
				return ResolveExternalType(targetList, name);
			}
			return false;
		}
		
		bool ResolveInternalType(Boo.Lang.List targetList, string name, EntityType flags)
		{
			foreach (ModuleEntity ns in _moduleNamespaces)
			{
				ns.ResolveMember(targetList, name, flags);
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
						targetList.Add(_typeSystemServices.Map(type));
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
