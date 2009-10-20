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
	using System.Text;

	public class CallableSignature : IEquatable<CallableSignature>
	{
		IParameter[] _parameters;
		IType _returnType;
		int _hashCode;
		bool _acceptVarArgs;

		public CallableSignature(IMethod method)
		{
			if (null == method) throw new ArgumentNullException("method");

			Initialize(method.GetParameters(), method.ReturnType, method.AcceptVarArgs);
		}

		public CallableSignature(IParameter[] parameters, IType returnType)
		{
			Initialize(parameters, returnType, false);
		}
		
		public CallableSignature(IParameter[] parameters, IType returnType, bool acceptVarArgs)
		{
			Initialize(parameters, returnType, acceptVarArgs);
		}

		private void Initialize(IParameter[] parameters, IType returnType, bool acceptVarArgs)
		{
			if (null == parameters) throw new ArgumentNullException("parameters");
			if (null == returnType) throw new ArgumentNullException("returnType");

			_parameters = parameters;
			_returnType = returnType;
			_acceptVarArgs = acceptVarArgs;
		}
		
		public IParameter[] Parameters
		{
			get { return _parameters; }
		}
		
		public IType ReturnType
		{
			get { return _returnType; }
		}

		public bool AcceptVarArgs
		{
			get { return _acceptVarArgs; }
		}
		
		override public int GetHashCode()
		{
			if (0 == _hashCode)
				InitializeHashCode();
			return _hashCode;
		}
		
		override public bool Equals(object other)
		{
			if (null == other) return false;
			if (this == other) return true;

			CallableSignature signature = other as CallableSignature;
			return Equals(signature);
		}

		public bool Equals(CallableSignature other)
		{
			if (null == other) return false;
			if (this == other) return true;

			return _returnType.Equals(other._returnType)
			       && _acceptVarArgs == other._acceptVarArgs
			       && AreSameParameters(_parameters, other._parameters);
		}

		override public string ToString()
		{
			StringBuilder buffer = new StringBuilder("callable(");
			for (int i=0; i<_parameters.Length; ++i)
			{
				if (i > 0) { buffer.Append(", "); }
				if (_parameters[i].IsByRef) buffer.Append("ref ");
				if (_acceptVarArgs && i == _parameters.Length-1) buffer.Append('*');
				buffer.Append(_parameters[i].Type.ToString());
			}
			buffer.Append(") as ");
			buffer.Append(_returnType.ToString());
			return buffer.ToString();
		}

		static public bool AreSameParameters(IParameter[] lhs, IParameter[] rhs)
		{
			int len = lhs.Length;
			if (len != rhs.Length)
				return false;

			for (int i=0; i < len; ++i)
			{
				IParameter lp = lhs[i];
				IParameter rp = rhs[i];
				if (lp.IsByRef != rp.IsByRef)
					return false;
				IType lpType = lp.IsByRef ? (lp.Type.GetElementType() ?? lp.Type) : lp.Type;
				IType rpType = rp.IsByRef ? (rp.Type.GetElementType() ?? rp.Type) : rp.Type;
				if (!lpType.Equals(rpType))
					return false;
			}

			return true;
		}

		void InitializeHashCode()
		{
			_hashCode = _acceptVarArgs ? 1 : 2;
			foreach (IParameter parameter in _parameters)
			{
				_hashCode ^= parameter.Type.GetHashCode();
			}
			_hashCode ^= _returnType.GetHashCode();
		}

	}

}

