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
		
		public virtual bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
		{
			IEntity element = (IEntity)_children[name];
			if (null != element && NameResolutionService.IsFlagSet(flags, element.EntityType))
			{
				targetList.Add(element);
				return true;
			}
			return false;
		}
	}
	
	public class GlobalNamespace : SimpleNamespace
	{
		INamespace _empty;
		
		public GlobalNamespace(IDictionary children) : base(null, children)
		{
			_empty = (INamespace)children[""];
			if (null == _empty)
			{
				_empty = NullNamespace.Default;
			}
		}
		
		override public bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
		{
			if (!base.Resolve(targetList, name, flags))
			{
				return _empty.Resolve(targetList, name, flags);
			}
			return true;
		}
	}
	
	public class AssemblyQualifiedNamespaceEntity : IEntity, INamespace
	{
		System.Reflection.Assembly _assembly;
		NamespaceEntity _subject;
		
		public AssemblyQualifiedNamespaceEntity(System.Reflection.Assembly assembly, NamespaceEntity subject)
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
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.Namespace;
			}
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return _subject.ParentNamespace;
			}
		}
		
		public bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
		{
			return _subject.Resolve(targetList, name, _assembly, flags);
		}
	}
	
	public class AliasedNamespace : IEntity, INamespace
	{
		string _alias;
		IEntity _subject;
		
		public AliasedNamespace(string alias, IEntity subject)
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
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.Namespace;
			}
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return ((INamespace)_subject).ParentNamespace;
			}
		}
		
		public bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
		{
			if (name == _alias && NameResolutionService.IsFlagSet(flags, _subject.EntityType))
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
		
		public NamespaceDelegator(INamespace parent, params INamespace[] namespaces)
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
		
		public bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
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
		TypeSystemServices _typeSystemServices;
		Boo.Lang.Compiler.Ast.DeclarationCollection _declarations;
		
		public DeclarationsNamespace(INamespace parent, TypeSystemServices tagManager, Boo.Lang.Compiler.Ast.DeclarationCollection declarations)
		{
			_parent = parent;
			_typeSystemServices = tagManager;
			_declarations = declarations;
		}
		
		public DeclarationsNamespace(INamespace parent, TypeSystemServices tagManager, Boo.Lang.Compiler.Ast.Declaration declaration)
		{
			_parent = parent;
			_typeSystemServices = tagManager;
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
		
		public bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
		{
			Boo.Lang.Compiler.Ast.Declaration found = _declarations[name];
			if (null != found)
			{
				IEntity element = TypeSystemServices.GetEntity(found);
				if (NameResolutionService.IsFlagSet(flags, element.EntityType))
				{
					targetList.Add(element);
					return true;
				}
			}
			return false;
		}
	}

}
