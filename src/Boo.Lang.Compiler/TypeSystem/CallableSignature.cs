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
	using System.Text;

	public class CallableSignature
	{
		IParameter[] _parameters;
		IType _returnType;
		int _hashCode;
		
		public CallableSignature(IMethod method)
		{
			if (null == method)
			{
				throw new ArgumentNullException("method");
			}
			Initialize(method.GetParameters(), method.ReturnType);
		}
		
		public CallableSignature(IParameter[] parameters, IType returnType)
		{
			Initialize(parameters, returnType);
		}
		
		void Initialize(IParameter[] parameters, IType returnType)
		{
			if (null == parameters)
			{
				throw new ArgumentNullException("parameters");
			}
			if (null == returnType)
			{
				throw new ArgumentNullException("returnType");
			}
			_parameters = parameters;
			_returnType = returnType;
			InitializeHashCode();
		}
		
		public IParameter[] Parameters
		{
			get
			{
				return _parameters;
			}
		}
		
		public IType ReturnType
		{
			get
			{
				return _returnType;
			}
		}
		
		override public int GetHashCode()
		{
			return _hashCode;
		}
		
		override public bool Equals(object other)
		{
			CallableSignature rhs = other as CallableSignature;
			if (null == rhs)
			{
				return false;
			}
			if (_returnType != rhs._returnType)
			{
				return false;
			}
			return Equals(_parameters, rhs._parameters);
		}
		
		override public string ToString()
		{
			StringBuilder buffer = new StringBuilder("callable(");
			for (int i=0; i<_parameters.Length; ++i)
			{
				if (i > 0) { buffer.Append(", "); }
				buffer.Append(_parameters[i].Type.FullName);
			}
			buffer.Append(") as ");
			buffer.Append(_returnType.FullName);
			return buffer.ToString();
		}
		
		bool Equals(IParameter[] lhs, IParameter[] rhs)
		{
			if (lhs.Length != rhs.Length)
			{
				return false;
			}
			for (int i=0; i<lhs.Length; ++i)
			{
				if (lhs[i].Type != rhs[i].Type)
				{
					return false;
				}
			}
			return true;
		}
		
		void InitializeHashCode()
		{
			_hashCode = 1;
			foreach (IParameter parameter in _parameters)
			{
				_hashCode ^= parameter.Type.GetHashCode();
			}
			_hashCode ^= _returnType.GetHashCode();
		}
	}
}
