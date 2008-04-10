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

namespace Boo.Lang.Compiler.TypeSystem
{
    using System;
    using System.Collections.Generic;
    using Boo.Lang.Compiler.TypeSystem;
    using Boo.Lang.Compiler.Ast;

	/// <summary>
	/// A base class for a member mapped from a generic type onto a constructed type.
	/// </summary>
    public abstract class GenericMappedMember<T> : IMember where T : IMember
    {
        protected readonly TypeSystemServices _tss;
        readonly T _source;
        readonly GenericMapping _genericMapping;
        string _fullName = null;

        protected GenericMappedMember(TypeSystemServices tss, T source, GenericMapping genericMapping)
        {
            _tss = tss;
            _source = source;
            _genericMapping = genericMapping;
        }

        public T Source
        {
            get { return _source; }
        }

        private string BuildFullName()
        {
            return DeclaringType.FullName + "." + Name;
        }

        public GenericMapping GenericMapping
        {
            get { return _genericMapping; }
        }

        public bool IsDuckTyped
        {
            get { return Source.IsDuckTyped; }
        }

        public IType DeclaringType
        {
            get { return GenericMapping.Map(Source.DeclaringType); }
        }

        public bool IsStatic
        {
            get { return Source.IsStatic; }
        }

        public IType Type
        {
            get { return GenericMapping.Map(Source.Type); }
        }

        public EntityType EntityType
        {
            get { return Source.EntityType; }
        }

        public string Name
        {
            get
            {
                return Source.Name;
            }
        }

        public string FullName
        {
            get { return _fullName ?? (_fullName = BuildFullName()); }
        }

        public bool IsPublic
        {
            get { return Source.IsPublic; }
        }

		public override string ToString()
		{
			return FullName;
		}
    }

	/// <summary>
	/// A base class for an accessible member mapped from a generic type onto a constructed type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public abstract class GenericMappedAccessibleMember<T> : GenericMappedMember<T> where T : IAccessibleMember
    {
        protected GenericMappedAccessibleMember(TypeSystemServices tss, T source, GenericMapping genericMapping)
            : base(tss, source, genericMapping)
        {
        }

        public bool IsProtected
        {
            get { return Source.IsProtected; }
        }

        public bool IsInternal
        {
            get { return Source.IsInternal; }
        }

        public bool IsPrivate
        {
            get { return Source.IsPrivate; }
        }
    }

    #region class GenericMappedMethod

    /// <summary>
    /// A method on a generic constructed type.
    /// </summary>
    public class GenericMappedMethod : GenericMappedAccessibleMember<IMethod>, IMethod
    {
        IParameter[] _parameters = null;
        ICallableType _callableType = null;

        public GenericMappedMethod(TypeSystemServices tss, IMethod source, GenericMapping genericMapping)
            : base(tss, source, genericMapping)
        {
        }

        public bool IsAbstract
        {
            get { return Source.IsAbstract; }
        }

        public bool IsVirtual
        {
            get { return Source.IsVirtual; }
        }

        public bool IsSpecialName
        {
            get { return Source.IsSpecialName; }
        }

        public bool IsPInvoke
        {
            get { return Source.IsPInvoke; }
        }

        public virtual IConstructedMethodInfo ConstructedInfo
        {
            // Generic mapped methods are not generic methods - those are InternalGenericMethods
            get { return null; }
        }

        public IGenericMethodInfo GenericInfo
        {
            // TODO: Generic mapped methods can be generic definitions!
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

        public bool AcceptVarArgs
        {
            get { return Source.AcceptVarArgs; }
        }

        public bool IsExtension
        {
            get { return Source.IsExtension; }
        }

		public bool IsBooExtension 
		{ 
			get { return Source.IsBooExtension; } 
		}

		public bool IsClrExtension 
		{ 
			get { return Source.IsClrExtension; } 
		}

        public IType ReturnType
        {
            get { return GenericMapping.Map(Source.ReturnType); }
        }

        public IParameter[] GetParameters()
        {
            return _parameters ?? (_parameters = GenericMapping.Map(Source.GetParameters()));
        }
    }

    #endregion

    #region class GenericMappedConstructor

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

    #endregion

    #region class GenericMappedProperty

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
            return _parameters ?? (_parameters = GenericMapping.Map(Source.GetParameters()));
        }

        public IMethod GetGetMethod()
        {
            return GenericMapping.Map(Source.GetGetMethod());
        }

        public IMethod GetSetMethod()
        {
            return GenericMapping.Map(Source.GetSetMethod());
        }

        public override string ToString()
        {
            return string.Format("{0} as {1}", Name, Type);
        }

        public bool AcceptVarArgs
        {
            get { return Source.AcceptVarArgs; }
        }

        public bool IsExtension
        {
            get { return Source.IsExtension; }
        }

		public bool IsBooExtension 
		{ 
			get { return Source.IsBooExtension; } 
		}

		public bool IsClrExtension 
		{ 
			get { return Source.IsClrExtension; } 
		}

    }

    #endregion

    #region class GenericMappedEvent

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
            return GenericMapping.Map(Source.GetAddMethod());
        }

        public IMethod GetRemoveMethod()
        {
            return GenericMapping.Map(Source.GetRemoveMethod());
        }

        public IMethod GetRaiseMethod()
        {
            return GenericMapping.Map(Source.GetRemoveMethod());
        }

        public bool IsAbstract
        {
            get { return Source.IsAbstract; }
        }

        public bool IsVirtual
        {
            get { return Source.IsVirtual; }
        }
    }

    #endregion

    #region class GenericMappedField

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
            get { return Source.IsInitOnly; }
        }

        public bool IsLiteral
        {
            get { return Source.IsLiteral; }
        }

        public object StaticValue
        {
            get { return Source.StaticValue; }
        }
    }

    #endregion

    #region class GenericMappedParameter
	
    /// <summary>
    /// A parameter in a method constructed from a generic method, or a mapped onto a type constructed 
	/// from a generic type.
	/// </summary>
	public class GenericMappedParameter : IParameter
	{
		private GenericMapping _genericMapping;
		private IParameter _baseParameter;
		
		public GenericMappedParameter(IParameter parameter, GenericMapping genericMapping)
		{
			_genericMapping = genericMapping;
			_baseParameter = parameter;
		}
		
		public bool IsByRef
		{
			get { return _baseParameter.IsByRef; }
		}
		
		public IType Type
		{
			get { return _genericMapping.Map(_baseParameter.Type); }
		}
		
		public string Name
		{
			get { return _baseParameter.Name; }
		}
		
		public string FullName
		{
			get { return _baseParameter.FullName; }
		}
		
		public EntityType EntityType
		{
			get { return EntityType.Parameter; }
		}
	}
    
    #endregion
}
