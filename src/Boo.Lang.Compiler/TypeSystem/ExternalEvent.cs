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
	
	public class ExternalEvent : IEvent
	{
		TypeSystemServices _typeSystemServices;
		
		System.Reflection.EventInfo _event;
		
		public ExternalEvent(TypeSystemServices tagManager, System.Reflection.EventInfo event_)
		{
			_typeSystemServices = tagManager;
			_event = event_;
		}
		
		public IType DeclaringType
		{
			get
			{
				return _typeSystemServices.Map(_event.DeclaringType);
			}
		}
		
		public IMethod GetAddMethod()
		{
			return (IMethod)_typeSystemServices.Map(_event.GetAddMethod());
		}
		
		public IMethod GetRemoveMethod()
		{
			return (IMethod)_typeSystemServices.Map(_event.GetRemoveMethod());
		}
		
		public System.Reflection.EventInfo EventInfo
		{
			get
			{
				return _event;
			}
		}
		
		public bool IsPublic
		{
			get
			{
				return _event.GetAddMethod().IsPublic;
			}
		}
		
		public string Name
		{
			get
			{
				return _event.Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _event.DeclaringType.FullName + "." + _event.Name;
			}
		}
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.Event;
			}
		}
		
		public IType Type
		{
			get
			{
				return _typeSystemServices.Map(_event.EventHandlerType);
			}
		}
		
		public bool IsStatic
		{
			get
			{
				return false;
			}
		}
	}
}
