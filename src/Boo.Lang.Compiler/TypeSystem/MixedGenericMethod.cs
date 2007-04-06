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
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

#if NET_2_0

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Text;
	using System.Reflection;
	using System.Collections.Generic;

	/// <summary>
	/// A generic method constructed from an external definition but involving internal parameters.
	/// </summary>
	public class MixedGenericMethod : ExternalMethod, IGenericMethodInfo, ITypeMapper
	{
		#region Data Members

		ExternalMethod _definition;
		IType[] _arguments = null;
		GenericTypeMapper _typeMapper;
		bool _constructed;
		string _name = null;
		string _fullName = null;
		
		#endregion
		
		#region Constructor

		public MixedGenericMethod(TypeSystemServices tss, ExternalMethod definition, IType[] arguments) : base(tss, definition.MethodInfo)
		{
			_definition = definition;
			_arguments = arguments;
			_constructed = IsConstructed();
			_typeMapper = new GenericTypeMapper(
				tss,
				definition.GenericMethodDefinitionInfo.GenericParameters,
				arguments);
		}
		
		#endregion

		#region IGenericMethodInfo members
		
		public IType[] GenericArguments
		{
			get { return _arguments; }
		}
		
		public IMethod GenericDefinition
		{
			get { return _definition; }
		}

		public bool FullyConstructed
		{
			get { return _constructed; }
		}
		
		#endregion
		
		#region Properties
	
		public override string Name
		{
			get
			{
				if (_name == null)
				{
					_name = BuildName(false);
				}
				return _name;
			}
		}
		
		public override string FullName
		{
			get
			{
				if (_fullName == null)
				{
					_fullName = BuildName(true);
				}
				return _fullName;
			}
		}
		
		public override IType ReturnType
		{
			get { return MapType(_definition.ReturnType); }
		}
		
		public override IGenericMethodInfo GenericMethodInfo
		{
			get { return this; }
		}
				
		#endregion

		#region Private Methods

		private bool IsConstructed()
		{
			foreach (IType arg in _arguments)
			{
				if (arg is IGenericParameter) return false;
			}
			
			return true;
		}
		
		private string BuildName(bool full)
		{
			Converter<IType, string> argumentName = delegate(IType type)
			{
				return full ? "[" + type.FullName + "]" : type.Name;
			};
			
			string[] typeNames = Array.ConvertAll(_arguments, argumentName);
			
			return string.Format(
				"{0}[{1}]",
				full ? _definition.FullName : _definition.Name,
				string.Join(", ", typeNames));
		}
		
		#endregion

		#region Public Methods

		public override string ToString()
		{
			return FullName;
		}
		
		public override IParameter[] GetParameters()
		{
			return Array.ConvertAll<IParameter, IParameter>(
				_definition.GetParameters(),
				delegate(IParameter p) { return new MappedParameter(
					_typeSystemServices, (ExternalParameter)p, this); });
		}		
				
		public IType MapType(IType sourceType)
		{
			return _typeMapper.MapType(sourceType);
		}
		
		#endregion		
	}
}

#endif
