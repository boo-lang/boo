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
    using System.Collections.Generic;
    using Boo.Lang.Compiler.TypeSystem;
    using Boo.Lang.Compiler.Ast;
    using System.Text;

    /// <summary>
    /// A type constructed by supplying type parameters to a generic type, involving internal types.
    /// </summary>
    /// <remarks>
    /// Constructed types constructed from an external generic type with external type arguments 
    /// are themselves external, and are represented as ExternalType instances. All other cases
    /// are represented by this type.
    /// </remarks>
    public class GenericConstructedType : IType, IConstructedTypeInfo
    {
        protected TypeSystemServices _tss;
        protected IType _definition;
        IType[] _arguments;
        GenericMapping _genericMapping;
        bool _fullyConstructed;
        
        string _fullName = null;

        public GenericConstructedType(TypeSystemServices tss, IType definition, IType[] arguments)
        {
            _tss = tss;
            _definition = definition;
            _arguments = arguments;
            _fullyConstructed = IsFullyConstructed();
            _genericMapping = new GenericMapping(tss, this, arguments);
        }

        protected bool IsFullyConstructed()
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

        protected string BuildFullName()
        {
            string[] argumentNames = Array.ConvertAll<IType, string>(
                GenericArguments, 
                delegate(IType t) { return t.FullName; });

            return string.Format("{0}[{1}]", _definition.FullName, string.Join(", ", argumentNames));
        }

        public GenericMapping GenericMapping
        {
            get { return _genericMapping; }
        }

        public bool IsClass
        {
            get { return _definition.IsClass; }
        }

        public bool IsAbstract
        {
            get { return _definition.IsAbstract; }
        }

        public bool IsInterface
        {
            get { return _definition.IsInterface; }
        }

        public bool IsEnum
        {
            get { return _definition.IsEnum; }
        }

        public bool IsByRef
        {
            get { return _definition.IsByRef; }
        }

        public bool IsValueType
        {
            get { return _definition.IsValueType; }
        }

        public bool IsFinal
        {
            get { return _definition.IsFinal; }
        }

        public bool IsArray
        {
            get { return _definition.IsArray; }
        }

        public int GetTypeDepth()
        {
            return _definition.GetTypeDepth();
        }

        public IType GetElementType()
        {
            return GenericMapping.Map(_definition.GetElementType());
        }

        public IType BaseType
        {
            get { return GenericMapping.Map(_definition.BaseType); }
        }

        public IEntity GetDefaultMember()
        {
            return GenericMapping.Map(_definition.GetDefaultMember());
        }

        public IConstructor[] GetConstructors()
        {
            return Array.ConvertAll<IConstructor, IConstructor>(
                _definition.GetConstructors(),
                delegate(IConstructor c) { return GenericMapping.Map(c); });
        }

        public IType[] GetInterfaces()
        {
            return Array.ConvertAll<IType, IType>(
                _definition.GetInterfaces(), 
                GenericMapping.Map);
        }

        public bool IsSubclassOf(IType other)
        {
            if (BaseType != null && (BaseType == other || BaseType.IsSubclassOf(other)))
            {
                return true;
            }

            if (other.IsInterface && Array.Exists(
                GetInterfaces(),
                delegate(IType i) { return other.IsAssignableFrom(i); }))
            {
                return true;
            }

            return false;
        }

        public virtual bool IsAssignableFrom(IType other)
        {
            if (other == null)
            {
                return false;
            }

            if (other == this || other.IsSubclassOf(this) || (other == Null.Default && !IsValueType))
            {
                return true;
            }

            return false;
        }

        public IGenericTypeInfo GenericInfo
        {
            get { return null; }
        }

        public IConstructedTypeInfo ConstructedInfo
        {
            get { return this; }
        }

        public IType Type
        {
            get { return this; }
        }

        public INamespace ParentNamespace
        {
            get 
            {              
                return GenericMapping.Map(_definition.ParentNamespace as IEntity) as INamespace; 
            }
        }

        public bool Resolve(List targetList, string name, EntityType filter)
        {
            // Resolve name using definition, and then map the matching members
            List definitionMatches = new List();
            if (_definition.Resolve(definitionMatches, name, filter))
            {
                foreach (IEntity match in definitionMatches)
                {
                    targetList.AddUnique(GenericMapping.Map(match));
                }
                return true;
            }
            return false;
        }

        public IEntity[] GetMembers()
        {
            return Array.ConvertAll<IEntity, IEntity>(_definition.GetMembers(), GenericMapping.Map);
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
            get { return EntityType.Type; }
        }

        public IType[] GenericArguments
        {
            get { return _arguments; }
        }

        public IType GenericDefinition
        {
            get { return _definition; }
        }

        public bool FullyConstructed
        {
            get { return _fullyConstructed; }
        }

		public override string ToString()
		{
			return FullName;
		}
    }

    public class GenericConstructedCallableType : GenericConstructedType, ICallableType
    {
        CallableSignature _signature;

        public GenericConstructedCallableType(TypeSystemServices tss, ICallableType definition, IType[] arguments) 
            : base(tss, definition, arguments) 
        {
        }

        public CallableSignature GetSignature()
        {
            if (_signature == null)
            {
                CallableSignature definitionSignature = ((ICallableType)_definition).GetSignature();
                
                IParameter[] parameters = GenericMapping.Map(definitionSignature.Parameters);
                IType returnType = GenericMapping.Map(definitionSignature.ReturnType);
                
                _signature = new CallableSignature(parameters, returnType);
            }

            return _signature;
        }

        override public bool IsAssignableFrom(IType other)
        {
            return _tss.IsCallableTypeAssignableFrom(this, other);
        }
    }
}