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
