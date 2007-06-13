#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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
// CAUSED AND TODON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using Boo.Lang.Compiler.TypeSystem;
	using Boo.Lang.Compiler.Ast;

	public class InternalGenericMethod : IMethod, IGenericMethodInfo, ITypeMapper
	{
		TypeSystemServices _tss; 
		IType[] _genericArguments;
		IMethod _definition;
		
		GenericTypeMapper _typeMapper;		
		ICallableType _callableType;		
		bool _fullyConstructed;
		string _name = null;
		string _fullName = null;
		IParameter[] _parameters = null;		
		
		public InternalGenericMethod(TypeSystemServices tss, IMethod definition, IType[] arguments)
		{
			_tss = tss;
			_definition = definition;
			_genericArguments = arguments;
			
			_typeMapper = new GenericTypeMapper(
				tss, 
				definition.GenericMethodDefinitionInfo.GenericParameters, 
				arguments);
				
			_fullyConstructed = IsFullyConstructed();
		}
		
		private bool IsFullyConstructed()
		{
			foreach (IType arg in GenericArguments)
			{
				if (TypeSystemServices.IsOpenGenericType(arg))
				{
					return false;
				}
			}
			return true;
		}
		
		private void BuildName()
		{
			string[] argumentNames = Array.ConvertAll<IType, string>(
				GenericArguments,
				delegate(IType t) { return "[" + t.Name + "]"; });
			
			_name = string.Format(
				"{0}`[{1}]", 
				_definition.Name, 
				string.Join(",", argumentNames));
		}

		private void BuildFullName()
		{
			string[] argumentNames = Array.ConvertAll<IType, string>(
				GenericArguments,
				delegate(IType t) { return t.FullName; });
			
			_fullName = string.Format(
				"{0}[{1}]", 
				_definition.FullName, 
				string.Join(",", argumentNames));
		}
		
		public IType MapType(IType source)
		{
			return _typeMapper.MapType(source);
		}
		
		public IParameter[] GetParameters()
		{
			if (_parameters == null)
			{
				_parameters = Array.ConvertAll<IParameter, IParameter>(
					_definition.GetParameters(), 
					delegate(IParameter p) { return new MappedParameter(_tss, p, this); });
			}
			return _parameters;
		}

		public IType ReturnType
		{
			get { return MapType(_definition.ReturnType); }
		}
		
		public bool IsAbstract
		{
			get { return _definition.IsAbstract; }
		}
		
		public bool IsVirtual
		{
			get { return _definition.IsVirtual; }
		}
		
		public bool IsSpecialName
		{
			get { return _definition.IsSpecialName; }
		}

		public bool IsPInvoke
		{
			get { return _definition.IsPInvoke; }
		}
		
		public IGenericMethodInfo GenericMethodInfo
		{
			get { return this; }
		}
		
		public IGenericMethodDefinitionInfo GenericMethodDefinitionInfo
		{
			get { return null; }
		}

		public ICallableType CallableType
		{
			get
			{
				if (null == _callableType)
				{
					_callableType = _tss.GetCallableType(this);
				}
				return _callableType;
			}
		}
		
		public bool IsExtension 
		{ 
			get { return _definition.IsExtension; } 
		}		

		public bool IsProtected
		{
			get { return _definition.IsProtected; }
		}

		public bool IsInternal
		{
			get { return _definition.IsInternal; }
		}

		public bool IsPrivate
		{
			get { return _definition.IsPrivate; }
		}

		public bool AcceptVarArgs
		{
			get { return _definition.AcceptVarArgs; }
		}	
		public bool IsDuckTyped
		{
			get { return _definition.IsDuckTyped; }
		}

		public IType DeclaringType
		{
			get { return _definition.DeclaringType; }
		}
		
		public bool IsStatic
		{
			get { return _definition.IsStatic; }
		}
		
		public bool IsPublic
		{
			get { return _definition.IsPublic; }
		}

		public IType Type
		{
			get { return CallableType; }			
		}

		public string Name
		{
			get 
			{ 
				if (_name == null) BuildName();
				return _name; 
			}
		}
		
		public string FullName
		{
			get
			{
				if (_fullName == null) BuildFullName();
				return _fullName;
			}
		}
		
		public EntityType EntityType
		{
			get { return EntityType.Method; }
		}
		
		public IMethod GenericDefinition
		{
			get { return _definition; }
		}
		
		public IType[] GenericArguments
		{
			get { return _genericArguments; }
		}
		
		public bool FullyConstructed
		{
			get 
			{
				return _fullyConstructed;
			}
		}
		
		public override string ToString()
		{
			return FullName;
		}

	}	
}