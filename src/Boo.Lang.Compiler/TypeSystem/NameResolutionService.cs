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
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	
	public class NameResolutionService
	{
		public static readonly char[] DotArray = new char[] { '.' };
		
		protected CompilerContext _context;
		
		protected INamespace _current;
		
		protected INamespace _global;
		
		protected Boo.Lang.List _buffer = new Boo.Lang.List();
		
		protected Boo.Lang.List _innerBuffer = new Boo.Lang.List();
		
		public NameResolutionService(CompilerContext context)
		{
			if (null == context)
			{
				throw new ArgumentNullException("context");
			}
			_context = context;
		}
		
		public INamespace GlobalNamespace
		{
			get
			{
				return _global;
			}
			
			set
			{
				_global = value;
			}
		}		
		
		public void EnterNamespace(INamespace ns)
		{
			if (null == ns)
			{
				throw new ArgumentNullException("ns");
			}
			_current = ns;
		}
		
		public INamespace CurrentNamespace
		{
			get
			{
				return _current;
			}
		}
		
		public void Reset()
		{
			if (null == _global)
			{
				throw new InvalidOperationException(Boo.ResourceManager.GetString("GlobalNamespaceIsNotSet"));
			}
			EnterNamespace(_global);
		}
		
		public void Restore(INamespace saved)
		{
			if (null == saved)
			{
				throw new ArgumentNullException("saved");
			}
			_current = saved;
		}
		
		public void LeaveNamespace()
		{
			_current = _current.ParentNamespace;
		}
		
		public IEntity Resolve(string name)
		{
			return Resolve(name, EntityType.Any);
		}
		
		public IEntity Resolve(string name, EntityType flags)
		{			
			_buffer.Clear();
			Resolve(_buffer, name, flags);
			return GetEntityFromBuffer();
		}
		
		public bool Resolve(Boo.Lang.List targetList, string name)
		{
			return Resolve(targetList, name, EntityType.Any);
		}
		
		public bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
		{			
			IEntity tag = _context.TypeSystemServices.ResolvePrimitive(name);
			if (null != tag)
			{
				targetList.Add(tag);
				return true;
			}
			else
			{
				INamespace ns = _current;
				while (null != ns)
				{					
					if (ns.Resolve(targetList, name, flags))
					{
						return true;
					}
					ns = ns.ParentNamespace;
				}
			}
			return false;
		}
		
		public IEntity ResolveQualifiedName(string name)
		{			
			_buffer.Clear();
			ResolveQualifiedName(_buffer, name);
			return GetEntityFromBuffer();
		}
		
		public bool ResolveQualifiedName(Boo.Lang.List targetList, string name)
		{
			return ResolveQualifiedName(targetList, name, EntityType.Any);
		}
		
		public bool ResolveQualifiedName(Boo.Lang.List targetList, string name, EntityType flags)
		{
			if (!IsQualifiedName(name))
			{
				return Resolve(targetList, name, flags);
			}
			
			string[] parts = name.Split(DotArray);
			string topLevel = parts[0];
			
			_innerBuffer.Clear();
			if (Resolve(_innerBuffer, topLevel) && 1 == _innerBuffer.Count)
			{
				INamespace ns = _innerBuffer[0] as INamespace;
				if (null != ns)
				{
					int last = parts.Length-1;
					for (int i=1; i<last; ++i)				
					{	
						_innerBuffer.Clear();
						if (!ns.Resolve(_innerBuffer, parts[i], EntityType.Any) ||
							1 != _innerBuffer.Count)
						{
							return false;
						}				
						ns = _innerBuffer[0] as INamespace;
						if (null == ns)
						{
							return false;
						}
					}
					return ns.Resolve(targetList, parts[last], flags);
				}
			}
			return false;
		}
		
		public void ResolveTypeReference(TypeReference node)
		{
			if (NodeType.ArrayTypeReference == node.NodeType)
			{
				ResolveArrayTypeReference((ArrayTypeReference)node);
			}
			else
			{
				ResolveSimpleTypeReference((SimpleTypeReference)node);
			}
		}
		
		public void ResolveArrayTypeReference(ArrayTypeReference node)
		{
			if (node.Entity != null)
			{
				return;
			}

			ResolveTypeReference(node.ElementType);
			
			IType elementType = TypeSystemServices.GetType(node.ElementType);
			if (TypeSystemServices.IsError(elementType))
			{
				node.Entity = TypeSystemServices.ErrorEntity;
			}
			else
			{
				node.Entity = _context.TypeSystemServices.GetArrayType(elementType);
			}
		}
		
		public void ResolveSimpleTypeReference(SimpleTypeReference node)
		{
			if (null != node.Entity)
			{
				return;
			}
			
			IEntity info = null;
			if (IsQualifiedName(node.Name))
			{
				info = ResolveQualifiedName(node.Name);
			}
			else
			{
				info = Resolve(node.Name, EntityType.Type);
			}
			
			if (null == info || EntityType.Type != info.EntityType)
			{
				_context.Errors.Add(CompilerErrorFactory.NameNotType(node, node.Name));
				info = TypeSystemServices.ErrorEntity;
			}
			else
			{
				node.Name = info.FullName;
			}
			
			node.Entity = info;
		}
		
		IEntity GetEntityFromBuffer()
		{
			return GetEntityFromList(_buffer);
		}
		
		public static IEntity GetEntityFromList(Boo.Lang.List list)
		{
			IEntity element = null;
			if (list.Count > 0)
			{
				if (list.Count > 1)
				{
					element = new Ambiguous((IEntity[])list.ToArray(typeof(IEntity)));
				}
				else
				{
					element = (IEntity)list[0];
				}
				list.Clear();
			}
			return element;
		}
		
		static bool IsQualifiedName(string name)
		{
			return name.IndexOf('.') > 0;
		}	
		
		public static bool IsFlagSet(EntityType flags, EntityType flag)
		{
			return flag == (flags & flag);
		}		
	}
}
