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

using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;

	public class NamespaceEntity : IEntity, INamespace
	{		
		TypeSystemServices _typeSystemServices;
		
		INamespace _parent;
		
		string _name;

		Dictionary<Assembly, Dictionary<string, List<Type>>> _assemblies;

		Dictionary<string, NamespaceEntity> _childrenNamespaces;
		
		List<ModuleEntity> _internalModules;
		
		List<ExternalType> _externalModules;
		
		public NamespaceEntity(INamespace parent, TypeSystemServices tagManager, string name)
		{			
			_parent = parent;
			_typeSystemServices = tagManager;
			_name = name;			_assemblies = new Dictionary<Assembly, Dictionary<string, List<Type>>>(AssemblyEqualityComparer.Default);
			_childrenNamespaces = new Dictionary<string, NamespaceEntity>();
			_internalModules = new List<ModuleEntity>();
			_externalModules = new List<ExternalType>();
		}
		
		public string Name
		{
			get
			{
				return GetLastPart(_name);
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
			Assembly assembly = type.Assembly;
			string name = TypeUtilities.TypeName(type);

			if (!_assemblies.ContainsKey(assembly))
			{
				_assemblies.Add(assembly, new Dictionary<string, List<Type>>());
				_assemblies[assembly].Add(name, new List<Type>());
			} else if (!_assemblies[assembly].ContainsKey(name)) {
				_assemblies[assembly].Add(name, new List<Type>());
			}

			_assemblies[assembly][name].Add(type);

			if (_typeSystemServices.IsModule(type))
			{
				_externalModules.Add((ExternalType) _typeSystemServices.Map(type));
			}
		}
		
		public void AddModule(ModuleEntity module)
		{
			_internalModules.Add(module);
		}
		
		public IEntity[] GetMembers()
		{
			List members = new List();
			members.Extend(_childrenNamespaces.Values);
			foreach (Type type in EnumerateTypes())
			{
				members.Add(_typeSystemServices.Map(type));
			}
			return (IEntity[])members.ToArray(typeof(IEntity));
		}

		private IEnumerable<Type> EnumerateTypes()
		{
			foreach (KeyValuePair<Assembly, Dictionary<string, List<Type>>> kv in _assemblies)
			{
				foreach (List<Type> types in kv.Value.Values)
				{
					foreach (Type type in types) yield return type;
				}
			}
		}
		
		public NamespaceEntity GetChildNamespace(string name)
		{
			NamespaceEntity tag;
			_childrenNamespaces.TryGetValue(name, out tag);
			if (null == tag)
			{				
				tag = new NamespaceEntity(this, _typeSystemServices, _name + "." + name);
				_childrenNamespaces[name] = tag;
			}
			return tag;
		}
		
		internal bool Resolve(List targetList, string name, Assembly assembly, EntityType flags)
		{
			NamespaceEntity entity;
			if (_childrenNamespaces.TryGetValue(name, out entity))
			{
				targetList.Add(new AssemblyQualifiedNamespaceEntity(assembly, entity));
				return true;
			}

			bool found = false;
			Dictionary<string, List<Type>> types;
			if (_assemblies.TryGetValue(assembly, out types))
			{
				found = ResolveType(targetList, name, types);
			}

			foreach (ExternalType external in _externalModules)
			{
				if (AssemblyEqualityComparer.Default.Equals(external.ActualType.Assembly, assembly))
				{
					if (external.Resolve(targetList, name, flags)) found = true; 
				}
			}
			return found;
		}
		
		public INamespace ParentNamespace
		{
			get { return _parent; }
		}
		
		public bool Resolve(List targetList, string name, EntityType flags)
		{	
			NamespaceEntity tag;
			_childrenNamespaces.TryGetValue(name, out tag);
			if (null != tag)
			{
				targetList.Add((IEntity) tag);
				return true;
			}
			
			if (ResolveInternalType(targetList, name, flags)) return true;

			bool found = ResolveExternalType(targetList, name);
			if (ResolveExternalModules(targetList, name, flags)) found = true;
			if (ResolveClrExtensions(targetList, name, flags)) found = true;
			return found;
		}

		private bool ResolveClrExtensions(List list, string name, EntityType flags)
		{
			if (!MetadataUtil.HasClrExtensions()) return false;

			if (!IsFlagSet(flags, EntityType.Method)) return false;

			bool found = false;
			foreach (Type type in EnumerateTypes())
			{
				if (ResolveClrExtensions(list, type, name)) found = true;
			}
			return found;
		}

		private bool ResolveClrExtensions(List list, Type type, string name)
		{
			MemberInfo[] members = MetadataUtil.GetClrExtensions(type, name);
			if (0 == members.Length) return false;
					
			foreach (MethodInfo method in members)
			{
				list.Add(_typeSystemServices.Map(method));
			}
			return true;
		}

		private static bool IsFlagSet(EntityType flags, EntityType flag)
		{
			return flag != (flag & flags);
		}

		bool ResolveInternalType(List targetList, string name, EntityType flags)
		{
			bool found = false;
			foreach (ModuleEntity ns in _internalModules)
			{
				if (ns.ResolveMember(targetList, name, flags)) found = true;
			}
			return found;
		}
		
		bool ResolveExternalType(List targetList, string name)
		{			
			foreach (KeyValuePair<Assembly, Dictionary<string, List<Type>>> kv in _assemblies)
			{
				//we do not use _assemblies.Values to keep fifo ordering
				if (ResolveType(targetList, name, kv.Value)) return true;
			}
			return false;
		}

		private bool ResolveType(List targetList, string name, Dictionary<string,List<Type>> types)
		{
			if (types.ContainsKey(name))
			{
				foreach (Type type in types[name])
				{
					targetList.Add(_typeSystemServices.Map(type));
					
					// Can't return right away, since we can have several types
					// with the same name but different number of generic arguments. 
				}
				return true;
			}
			return false;
		}

		bool ResolveExternalModules(List targetList, string name, EntityType flags)
		{
			bool found = false;
			foreach (INamespace ns in _externalModules)
			{
				if (ns.Resolve(targetList, name, flags)) found = true;
			}
			return found;
		}
		
		override public string ToString()
		{
			return _name;
		}

		private static string GetLastPart(string name)
		{
			int index = name.LastIndexOf('.');
			return index < 0 ? name : name.Substring(index+1);
		}
	}
}
