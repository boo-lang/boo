#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
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
