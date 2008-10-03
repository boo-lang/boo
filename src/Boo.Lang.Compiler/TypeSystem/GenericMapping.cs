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
	using System.Text;
	using System.Reflection;
	using System.Collections.Generic;

	/// <summary>
	/// Maps entities onto their constructed counterparts, substituting type arguments for generic parameters.
	/// </summary>
	public class GenericMapping
	{
		TypeSystemServices _tss;
		IDictionary<IGenericParameter, IType> _map = new Dictionary<IGenericParameter, IType>();
        IDictionary<IEntity, IEntity> _cache = new Dictionary<IEntity, IEntity>();
		IEntity _constructedOwner = null;
		IEntity _genericSource = null;

		/// <summary>
		/// Constructs a new generic mapping between a generic type and one of its constructed types.
		/// </summary>
		public GenericMapping(TypeSystemServices tss, IType constructedType, IType[] arguments)
			: this(tss, constructedType.ConstructedInfo.GenericDefinition.GenericInfo.GenericParameters, arguments)
		{
			_constructedOwner = constructedType;
			_genericSource = constructedType.ConstructedInfo.GenericDefinition;
		}

		/// <summary>
		/// Constructs a new generic mapping between a generic method and one of its constructed methods.
		/// </summary>
		public GenericMapping(TypeSystemServices tss, IMethod constructedMethod, IType[] arguments)
			: this(tss, constructedMethod.ConstructedInfo.GenericDefinition.GenericInfo.GenericParameters, arguments)
		{
			_constructedOwner = constructedMethod;
			_genericSource = constructedMethod.ConstructedInfo.GenericDefinition;
		}

        /// <summary>
		/// Constrcuts a new GenericMapping for a specific mapping of generic parameters to type arguments.
        /// </summary>
        /// <param name="parameters">The generic parameters that should be mapped.</param>
        /// <param name="arguments">The type arguments to map generic parameters to.</param>
		protected GenericMapping(TypeSystemServices tss, IGenericParameter[] parameters, IType[] arguments)
		{
			_tss = tss;
			for (int i = 0; i < parameters.Length; i++)
			{
				_map.Add(parameters[i], arguments[i]);
			}
		}

		/// <summary>
		/// Maps a type involving generic parameters to the corresponding type after substituting concrete
		/// arguments for generic parameters.
		/// </summary>
		/// <remarks>
		/// If the source type is a generic parameter, it is mapped to the corresponding argument.
		/// If the source type is an open generic type using any of the specified generic parameters, it 
		/// is mapped to a closed constructed type based on the specified arguments.
		/// </remarks>
		protected virtual IType MapType(IType sourceType)
		{
			if (sourceType == null)
			{
				return null;
			}

			// Strip reference types
			if (sourceType.IsByRef)
			{
				return MapType(sourceType.GetElementType());
			}

			// Map generic parameter to corresponding argument
			IGenericParameter gp = sourceType as IGenericParameter;
			if (gp != null)	
			{
				if (_map.ContainsKey(gp)) return _map[gp];
				if (_cache.ContainsKey(gp.DeclaringMethod)) return new GenericMappedTypeParameter(_tss, gp, this);
			}

			// Map open constructed type using generic parameters to closed constructed type
			// using corresponding arguments
			if (null != sourceType.ConstructedInfo)
			{
				IType[] mappedArguments = Array.ConvertAll<IType, IType>(
					sourceType.ConstructedInfo.GenericArguments,
					MapType);
				
				IType mapped = sourceType.ConstructedInfo.
					GenericDefinition.GenericInfo.
					ConstructType(mappedArguments);
				
				return mapped;
			}

			// TODO: Map nested types
			// GenericType[of T].NestedType => GenericType[of int].NestedType

			// Map array types
			IArrayType array = (sourceType as IArrayType);
			if (array != null)
			{
				IType elementType = array.GetElementType();
				IType mappedElementType = MapType(elementType);
				if (mappedElementType != elementType)
				{
					return _tss.GetArrayType(mappedElementType, array.GetArrayRank());
				}
			}
			
			// Map callable types
			ICallableType callable = sourceType as ICallableType;
			if (callable != null && EntityNeedsMapping(callable))
			{
				CallableSignature signature = callable.GetSignature();

				IType returnType = MapType(signature.ReturnType);
                IParameter[] parameters = Map(signature.Parameters);
					
				CallableSignature mappedSignature = new CallableSignature(
					parameters, returnType, signature.AcceptVarArgs);
					
				return _tss.GetCallableType(mappedSignature);
			}
			
			// If source type doesn't require mapping, return it as is
			return sourceType;
		}
		
		public bool EntityNeedsMapping(IEntity entity)
		{
			if (entity is ICallableType)
			{
				if (entity is ExternalCallableType)
				{
					return false;
				}
				
				ICallableType callable = entity as ICallableType;
				
				if (callable.BaseType == _tss.MulticastDelegateType
					|| callable.BaseType.IsSubclassOf(_tss.MulticastDelegateType))
				{
					return callable.ConstructedInfo != null
						|| callable.GenericInfo != null;
				}
				return true;
			}
			if (entity is IMember)
			{
				IMember member = entity as IMember;
				return member.DeclaringType.ConstructedInfo != null
					|| member.DeclaringType.GenericInfo != null;
			}
			if (entity is IConstructedTypeInfo)
			{
				return true;
			}
			return false;
		}

        /// <summary>
        /// Maps a type member involving generic arguments to its constructed counterpart, after substituting 
        /// concrete types for generic arguments.
        /// </summary>
        public IEntity Map(IEntity source)
        {
            if (source == null)
                return null;

			// Map generic source to the constructed owner of this mapping
			if (source == _genericSource)
			{
				return _constructedOwner;
			}

			// Use cache if possible
            if (_cache.ContainsKey(source))
            {
                return _cache[source];
            }

			// Map entity based on its entity type
            IEntity mapped = null;
            switch (source.EntityType)
            {
				case EntityType.Ambiguous:
            		mapped = MapAmbiguousEntity((Ambiguous)source);
            		break;

                case EntityType.Method:
                    mapped = new GenericMappedMethod(_tss, ((IMethod)source), this);
                    break;

                case EntityType.Constructor:
                    mapped = new GenericMappedConstructor(_tss, ((IConstructor)source), this);
                    break;

                case EntityType.Field:
                    mapped = new GenericMappedField(_tss, ((IField)source), this);
                    break;

                case EntityType.Property:
                    mapped = new GenericMappedProperty(_tss, ((IProperty)source), this);
                    break;

                case EntityType.Event:
                    mapped = new GenericMappedEvent(_tss, ((IEvent)source), this);
                    break;

                case EntityType.Parameter:
                    mapped = new GenericMappedParameter((IParameter)source, this);
                    break;

                case EntityType.Array:
                case EntityType.Type:
                    mapped = MapType((IType)source);
                    break;

                default:
                    return source;
            }

			// Cache mapped result and return it
            _cache[source] = mapped;
            return mapped;
        }

        /// <summary>
        /// Maps a method on a generic type definition to its constructed counterpart.
        /// </summary>
        public IMethod Map(IMethod source)
        {
            return (IMethod)Map((IEntity)source);
        }

        /// <summary>
        /// Maps a constructor on a generic type definition to its constructed counterpart.
        /// </summary>
        public IConstructor Map(IConstructor source)
        {
            return (IConstructor)Map((IEntity)source);
        }

        /// <summary>
        /// Maps a field on a generic type definition to its constructed counterpart.
        /// </summary>
        public IField Map(IField source)
        {
            return (IField)Map((IEntity)source);
        }

        /// <summary>
        /// Maps a property on a generic type definition to its constructed counterpart.
        /// </summary>
        public IProperty Map(IProperty source)
        {
            return (IProperty)Map((IEntity)source);
        }

        /// <summary>
        /// Maps an event on a generic type definition to its constructed counterpart.
        /// </summary>
        public IEvent Map(IEvent source)
        {
            return (IEvent)Map((IEvent)source);
        }

        /// <summary>
        /// Maps a type involving generic parameters to its constructed counterpart.
        /// </summary>
        public IType Map(IType source)
        {
            return (IType)Map((IEntity)source);
        }

		public IGenericParameter Map(IGenericParameter source)
		{
			return (IGenericParameter)Map((IEntity)source);
		}

        /// <summary>
        /// Maps a parameter in a generic, constructed or mapped method to its constructed counterpart.
        /// </summary>
        public IParameter Map(IParameter source)
        {
            return (IParameter)Map((IEntity)source);
        }

		/// <summary>
		/// Maps an array of parameters in a generic, constructed or mapped method to their constructed counterparts.
		/// </summary>
        public IParameter[] Map(IParameter[] sources) 
        {
            return Array.ConvertAll<IParameter, IParameter>(sources, Map);
        }

		private IEntity MapAmbiguousEntity(Ambiguous source)
		{
			// Map each individual entity in the ambiguous list 
			return new Ambiguous(Array.ConvertAll<IEntity, IEntity>(source.Entities, Map));
		}

		/// <summary>
		/// Gets the method from which the specified method was mapped.
		/// </summary>
		public IMethod UnMap(IMethod method)
		{
			GenericMappedMethod mapped = method as GenericMappedMethod;
			if (mapped == null)
			{
				return null;
			}
			return mapped.Source;
		}
	}
}

