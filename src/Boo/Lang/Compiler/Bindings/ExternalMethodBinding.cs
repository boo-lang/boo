#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Reflection;

namespace Boo.Lang.Compiler.Bindings
{
	public class ExternalMethodBinding : IMethodBinding
	{
		BindingManager _bindingManager;
		
		MethodBase _mi;
		
		internal ExternalMethodBinding(BindingManager manager, MethodBase mi)
		{
			_bindingManager = manager;
			_mi = mi;
		}
		
		public ITypeBinding DeclaringType
		{
			get
			{
				return _bindingManager.AsTypeBinding(_mi.DeclaringType);
			}
		}
		
		public bool IsStatic
		{
			get
			{
				return _mi.IsStatic;
			}
		}
		
		public bool IsPublic
		{
			get
			{
				return _mi.IsPublic;
			}
		}
		
		public bool IsVirtual
		{
			get
			{
				return _mi.IsVirtual;
			}
		}
		
		public string Name
		{
			get
			{
				return _mi.Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _mi.DeclaringType.FullName + "." + _mi.Name;
			}
		}
		
		public virtual BindingType BindingType
		{
			get
			{
				return BindingType.Method;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return ReturnType;
			}
		}
		
		public int ParameterCount
		{
			get
			{
				return _mi.GetParameters().Length;
			}
		}
		
		public ITypeBinding GetParameterType(int parameterIndex)
		{
			return _bindingManager.AsTypeBinding(_mi.GetParameters()[parameterIndex].ParameterType);
		}
		
		public ITypeBinding ReturnType
		{
			get
			{
				MethodInfo mi = _mi as MethodInfo;
				if (null != mi)
				{
					return _bindingManager.AsTypeBinding(mi.ReturnType);
				}
				return null;
			}
		}
		
		public MethodBase MethodInfo
		{
			get
			{
				return _mi;
			}
		}
		
		public override string ToString()
		{
			return BindingManager.GetSignature(this);
		}
	}
	
	public class ExternalConstructorBinding : ExternalMethodBinding, IConstructorBinding
	{
		public ExternalConstructorBinding(BindingManager manager, ConstructorInfo ci) : base(manager, ci)
		{			
		}
		
		public override BindingType BindingType
		{
			get
			{
				return BindingType.Constructor;
			}
		}
		
		public ConstructorInfo ConstructorInfo
		{
			get
			{
				return (ConstructorInfo)MethodInfo;
			}
		}
	}
}
