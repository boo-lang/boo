#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
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
		
		public IEntity[] GetMembers()
		{
			Boo.Lang.List members = new Boo.Lang.List();
			members.Extend(_childrenNamespaces.Values);
			foreach (Boo.Lang.List types in _assemblies.Values)
			{
				members.Extend(types);
			}
			return (IEntity[])members.ToArray(typeof(IEntity));
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
