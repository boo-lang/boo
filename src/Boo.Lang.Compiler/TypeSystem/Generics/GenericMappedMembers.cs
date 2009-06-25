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
using System.Collections.Generic;

namespace Boo.Lang.Compiler.TypeSystem.Generics
{
	public interface IGenericMappedMember : IMember
	{
		IMember SourceMember { get; }
	}

	/// <summary>
	/// A base class for a member mapped from a generic type onto a constructed type.
	/// </summary>
	public abstract class GenericMappedMember<T> : IGenericMappedMember where T : IMember
	{
		protected readonly TypeSystemServices _tss;
		readonly T _sourceMember;
		readonly GenericMapping _genericMapping;
		string _fullName = null;

		protected GenericMappedMember(TypeSystemServices tss, T sourceMember, GenericMapping genericMapping)
		{
			_tss = tss;
			_sourceMember = sourceMember;
			_genericMapping = genericMapping;
		}

		public T SourceMember
		{
			get { return _sourceMember; }
		}

		IMember IGenericMappedMember.SourceMember
		{
			get { return SourceMember; }
		}

		protected virtual string BuildFullName()
		{
			return DeclaringType.FullName + "." + Name;
		}

		public GenericMapping GenericMapping
		{
			get { return _genericMapping; }
		}

		public bool IsDuckTyped
		{
			get { return SourceMember.IsDuckTyped; }
		}

		public IType DeclaringType
		{
			get { return GenericMapping.MapType(SourceMember.DeclaringType); }
		}

		public bool IsStatic
		{
			get { return SourceMember.IsStatic; }
		}

		public IType Type
		{
			get { return GenericMapping.MapType(SourceMember.Type); }
		}

		public EntityType EntityType
		{
			get { return SourceMember.EntityType; }
		}

		public string Name
		{
			get
			{
				return SourceMember.Name;
			}
		}

		public string FullName
		{
			get { return _fullName ?? (_fullName = BuildFullName()); }
		}

		public bool IsPublic
		{
			get { return SourceMember.IsPublic; }
		}

		public override string ToString()
		{
			return FullName;
		}

		public bool IsDefined(IType attributeType)
		{
			return _sourceMember.IsDefined(attributeType);
		}
	}

	/// <summary>
	/// A base class for an accessible member mapped from a generic type onto a constructed type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class GenericMappedAccessibleMember<T> : GenericMappedMember<T>, IAccessibleMember where T : IAccessibleMember
	{
		protected GenericMappedAccessibleMember(TypeSystemServices tss, T source, GenericMapping genericMapping)
			: base(tss, source, genericMapping)
		{
		}

		public bool IsProtected
		{
			get { return SourceMember.IsProtected; }
		}

		public bool IsInternal
		{
			get { return SourceMember.IsInternal; }
		}

		public bool IsPrivate
		{
			get { return SourceMember.IsPrivate; }
		}
	}

	/// <summary>
	/// A method on a generic constructed type.
	/// </summary>
	public class GenericMappedMethod : GenericMappedAccessibleMember<IMethod>, IMethod, IGenericMethodInfo
	{
		IParameter[] _parameters = null;
		IGenericParameter[] _typeParameters = null; 
		ICallableType _callableType = null;
		IDictionary<IType[], IMethod> _constructedMethods = new Dictionary<IType[], IMethod>();

		public GenericMappedMethod(TypeSystemServices tss, IMethod source, GenericMapping genericMapping)
			: base(tss, source, genericMapping)
		{
		}

		public bool IsAbstract
		{
			get { return SourceMember.IsAbstract; }
		}

		public bool IsVirtual
		{
			get { return SourceMember.IsVirtual; }
		}

		public bool IsSpecialName
		{
			get { return SourceMember.IsSpecialName; }
		}

		public bool IsPInvoke
		{
			get { return SourceMember.IsPInvoke; }
		}

		public virtual IConstructedMethodInfo ConstructedInfo
		{
			// Generic mapped methods are not constructed methods, rather they're methods on constructed types.
			get { return null; }
		}

		public IGenericMethodInfo GenericInfo
		{
			get 
			{
				if (SourceMember.GenericInfo == null)
				{
					return null;
				}
				return this; 
			}
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

		public bool AcceptVarArgs
		{
			get { return SourceMember.AcceptVarArgs; }
		}

		public bool IsExtension
		{
			get { return SourceMember.IsExtension; }
		}

		public bool IsBooExtension 
		{ 
			get { return SourceMember.IsBooExtension; } 
		}

		public bool IsClrExtension 
		{ 
			get { return SourceMember.IsClrExtension; } 
		}

		public IType ReturnType
		{
			get { return GenericMapping.MapType(SourceMember.ReturnType); }
		}

		public IParameter[] GetParameters()
		{
			if (_parameters == null)
			{
				_parameters = GenericMapping.MapParameters(SourceMember.GetParameters());
			}
			return _parameters;
		}

		IGenericParameter[] IGenericMethodInfo.GenericParameters
		{
			get 
			{
				if (_typeParameters == null)
				{
					_typeParameters = Array.ConvertAll<IGenericParameter, IGenericParameter>(
						SourceMember.GenericInfo.GenericParameters,
						GenericMapping.MapGenericParameter);
				}
				return _typeParameters;
			}
		}

		IMethod IGenericMethodInfo.ConstructMethod(params IType[] arguments)
		{
			IMethod constructedMethod = null;
			if (!_constructedMethods.TryGetValue(arguments, out constructedMethod))
			{
				constructedMethod = new GenericConstructedMethod(this, arguments);
				_constructedMethods.Add(arguments, constructedMethod);
			}
			return constructedMethod;
		}

		protected override string BuildFullName()
		{
			// TODO: pull all name-building logic for types and methods together somewhere
			// instead of repeating it every time with slight variations (BOO-1097)
			System.Text.StringBuilder sb = new System.Text.StringBuilder(base.BuildFullName());
			
			if (GenericInfo != null)
			{
				sb.Append("[of ");

				string[] genericParameterNames = Array.ConvertAll<IGenericParameter, string>(
					GenericInfo.GenericParameters,
					delegate(IGenericParameter gp) { return gp.Name; });

				sb.Append(string.Join(", ", genericParameterNames));
				sb.Append("]");
			}

			sb.Append("(");

			string[] parameterTypeNames = Array.ConvertAll<IParameter, string>(
				GetParameters(),
				delegate(IParameter p) { return p.Type.Name; });

			sb.Append(string.Join(", ", parameterTypeNames));
			sb.Append(")");

			return sb.ToString();
		}
	}

	/// <summary>
	/// A constructor on a generic constructed type.
	/// </summary>
	public class GenericMappedConstructor : GenericMappedMethod, IConstructor
	{
		public GenericMappedConstructor(TypeSystemServices tss, IConstructor source, GenericMapping genericMapping)
			: base(tss, (IMethod)source, genericMapping)
		{
		}
	}

	/// <summary>
	/// A property on a generic constructed type.
	/// </summary>
	public class GenericMappedProperty : GenericMappedAccessibleMember<IProperty>, IProperty
	{
		IParameter[] _parameters;

		public GenericMappedProperty(TypeSystemServices tss, IProperty source, GenericMapping genericMapping)
			: base(tss, source, genericMapping)
		{
		}

		public IParameter[] GetParameters()
		{
			return _parameters ?? (_parameters = GenericMapping.MapParameters(SourceMember.GetParameters()));
		}

		public IMethod GetGetMethod()
		{
			return GenericMapping.Map(SourceMember.GetGetMethod());
		}

		public IMethod GetSetMethod()
		{
			return GenericMapping.Map(SourceMember.GetSetMethod());
		}

		public override string ToString()
		{
			return string.Format("{0} as {1}", Name, Type);
		}

		public bool AcceptVarArgs
		{
			get { return SourceMember.AcceptVarArgs; }
		}

		public bool IsExtension
		{
			get { return SourceMember.IsExtension; }
		}

		public bool IsBooExtension 
		{ 
			get { return SourceMember.IsBooExtension; } 
		}

		public bool IsClrExtension 
		{ 
			get { return SourceMember.IsClrExtension; } 
		}

	}

	/// <summary>
	/// An event in a constructed generic type.
	/// </summary>
	public class GenericMappedEvent : GenericMappedMember<IEvent>, IEvent
	{
		public GenericMappedEvent(TypeSystemServices tss, IEvent source, GenericMapping genericMapping)
			: base(tss, source, genericMapping)
		{
		}

		public IMethod GetAddMethod()
		{
			return GenericMapping.Map(SourceMember.GetAddMethod());
		}

		public IMethod GetRemoveMethod()
		{
			return GenericMapping.Map(SourceMember.GetRemoveMethod());
		}

		public IMethod GetRaiseMethod()
		{
			return GenericMapping.Map(SourceMember.GetRemoveMethod());
		}

		public bool IsAbstract
		{
			get { return SourceMember.IsAbstract; }
		}

		public bool IsVirtual
		{
			get { return SourceMember.IsVirtual; }
		}
	}

	/// <summary>
	/// A field on a generic constructed type.
	/// </summary>
	public class GenericMappedField : GenericMappedAccessibleMember<IField>, IField
	{
		public GenericMappedField(TypeSystemServices tss, IField source, GenericMapping genericMapping)
			: base(tss, source, genericMapping)
		{
		}

		public bool IsInitOnly
		{
			get { return SourceMember.IsInitOnly; }
		}

		public bool IsLiteral
		{
			get { return SourceMember.IsLiteral; }
		}

		public object StaticValue
		{
			get { return SourceMember.StaticValue; }
		}

		public bool IsVolatile
		{
			get { return SourceMember.IsVolatile; }
		}
	}}

