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
	
	public class AnonymousCallableType : AbstractType, ICallableType
	{
		TypeSystemServices _typeSystemServices;
		CallableSignature _signature;
		IType _concreteType;
		
		internal AnonymousCallableType(TypeSystemServices services, CallableSignature signature)
		{
			if (null == services)
			{
				throw new ArgumentNullException("services");
			}
			if (null == signature)
			{
				throw new ArgumentNullException("signature");
			}
			_typeSystemServices = services;
			_signature = signature;
		}
		
		public IType ConcreteType
		{
			get
			{
				return _concreteType;
			}
			
			set
			{
				System.Diagnostics.Debug.Assert(null != value);
				_concreteType = value;
			}
		}
		
		override public IType BaseType
		{
			get
			{
				return _typeSystemServices.MulticastDelegateType;
			}
		}
		
		override public bool IsSubclassOf(IType other)
		{			
			return BaseType.IsSubclassOf(other) || other == BaseType ||
				other == _typeSystemServices.ICallableType;				
		}
		
		public CallableSignature GetSignature()
		{
			return _signature;
		}
		
		override public string Name
		{
			get
			{				
				return _signature.ToString(); 
			}
		}
		
		override public EntityType EntityType
		{
			get
			{
				return EntityType.Type;
			}
		}
	}
}
