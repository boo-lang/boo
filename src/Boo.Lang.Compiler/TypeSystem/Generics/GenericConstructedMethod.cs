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

using System;

namespace Boo.Lang.Compiler.TypeSystem.Generics
{
	/// <summary>
	/// A method constructed by supplying type arguments to a generic method, involving internal types.
	/// </summary>
	/// <remarks>
	/// Constructed methods constructed from external generic methods with external type parameters 
	/// are themselves external, and are represented as ExternalMethod instances. All other cases
	/// are represented by this type.
	/// </remarks>
	public class GenericConstructedMethod : IMethod, IConstructedMethodInfo
	{
		IType[] _genericArguments;
		IMethod _definition;
		
		GenericMapping _genericMapping;		
		bool _fullyConstructed;
		string _fullName = null;
		IParameter[] _parameters = null;		
		
		public GenericConstructedMethod(IMethod definition, IType[] arguments)
		{
			_definition = definition;
			_genericArguments = arguments;
			
			_genericMapping = new InternalGenericMapping(this, arguments);
				
			_fullyConstructed = IsFullyConstructed();
		}
		
		private bool IsFullyConstructed()
		{
			foreach (IType arg in GenericArguments)
			{
				if (GenericsServices.IsOpenGenericType(arg))
				{
					return false;
				}
			}
			return true;
		}

		private string BuildFullName()
		{
			string[] argumentNames = Array.ConvertAll<IType, string>(
				GenericArguments,
				delegate(IType t) { return t.FullName; });
			
			return string.Format(
				"{0}.{1}[of {2}]", 
				DeclaringType.FullName, 
				Name,
				string.Join(", ", argumentNames));
		}

		protected GenericMapping GenericMapping
		{
			get { return _genericMapping; }
		}

		public IParameter[] GetParameters()
		{
			return _parameters ?? (_parameters = GenericMapping.MapParameters(_definition.GetParameters()));
		}

		public IType ReturnType
		{
			get { return GenericMapping.MapType(_definition.ReturnType); }
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
		
		public IConstructedMethodInfo ConstructedInfo
		{
			get { return this; }
		}
		
		public IGenericMethodInfo GenericInfo
		{
			get { return null; }
		}

		public ICallableType CallableType
		{
			get { return My<TypeSystemServices>.Instance.GetCallableType(this); }
		}
		
		public bool IsExtension 
		{ 
			get { return _definition.IsExtension; } 
		}

		public bool IsBooExtension
		{
			get { return _definition.IsBooExtension; }
		}

		public bool IsClrExtension
		{
			get { return _definition.IsClrExtension; }
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
			get { return _definition.Name; }
		}
		
		public string FullName
		{
			get { return _fullName ?? (_fullName = BuildFullName()); }
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
			get { return _fullyConstructed; }
		}

		public bool IsDefined(IType attributeType)
		{
			return _definition.IsDefined(attributeType);
		}
		
		public override string ToString()
		{
			return FullName;
		}
	}
}