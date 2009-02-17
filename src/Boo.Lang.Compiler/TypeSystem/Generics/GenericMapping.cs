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
using Boo.Lang.Compiler.TypeSystem.Generics;

namespace Boo.Lang.Compiler.TypeSystem.Generics
{
	/// <summary>
	/// Maps entities onto their constructed counterparts, substituting type arguments for generic parameters.
	/// </summary>
	public abstract class GenericMapping : TypeMapper
	{
		IDictionary<IGenericParameter, IType> _map = new Dictionary<IGenericParameter, IType>();
		IDictionary<IMember, IMember> _memberCache = new Dictionary<IMember, IMember>();

		IEntity _constructedOwner = null;
		IEntity _genericSource = null;

		/// <summary>
		/// Constructs a new generic mapping between a generic type and one of its constructed types.
		/// </summary>
		public GenericMapping(IType constructedType, IType[] arguments)
			: this(constructedType.ConstructedInfo.GenericDefinition.GenericInfo.GenericParameters, arguments)
		{
			_constructedOwner = constructedType;
			_genericSource = constructedType.ConstructedInfo.GenericDefinition;
		}

		/// <summary>
		/// Constructs a new generic mapping between a generic method and one of its constructed methods.
		/// </summary>
		public GenericMapping(IMethod constructedMethod, IType[] arguments)
			: this(constructedMethod.ConstructedInfo.GenericDefinition.GenericInfo.GenericParameters, arguments)
		{
			_constructedOwner = constructedMethod;
			_genericSource = constructedMethod.ConstructedInfo.GenericDefinition;
		}

		/// <summary>
		/// Constrcuts a new GenericMapping for a specific mapping of generic parameters to type arguments.
		/// </summary>
		/// <param name="parameters">The generic parameters that should be mapped.</param>
		/// <param name="arguments">The type arguments to map generic parameters to.</param>
		protected GenericMapping(IGenericParameter[] parameters, IType[] arguments)
		{
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
		override public IType MapType(IType sourceType)
		{
			if (sourceType == _genericSource) return _constructedOwner as IType;

			IGenericParameter gp = sourceType as IGenericParameter;
			if (gp != null)
			{
				// Map type parameters declared on our source
				if (_map.ContainsKey(gp)) return _map[gp];

				// Map type parameters declared on members of our source (methods / nested types)
				return GenericsServices.GetGenericParameters(Map(gp.DeclaringEntity))[gp.GenericParameterPosition];
			}

			// TODO: Map nested types
			// GenericType[of T].NestedType => GenericType[of int].NestedType

			return base.MapType(sourceType);
		}

		/// <summary>
		/// Maps a type member involving generic arguments to its constructed counterpart, after substituting 
		/// concrete types for generic arguments.
		/// </summary>
		public IEntity Map(IEntity source)
		{
			if (source == null) return null;

			// Map generic source to the constructed owner of this mapping
			if (source == _genericSource) return _constructedOwner;

			Ambiguous ambiguous = source as Ambiguous;
			if (ambiguous != null) return MapAmbiguousEntity(ambiguous);

			IMember member = source as IMember;
			if (member != null) return MapMember(member);

			IType type = source as IType;
			if (type != null) return MapType(type);

			return source;
		}

		private IMember MapMember(IMember source)
		{
			// Use cached mapped member if available
			if (_memberCache.ContainsKey(source)) return _memberCache[source];

			// Map members declared on our source
			if (source.DeclaringType == _genericSource)
			{
				return CacheMember(source, CreateMappedMember(source));
			}

			// If member is declared on a basetype of our source, that is itself constructed, let its own mapper map it
			IType declaringType = source.DeclaringType;
			if (declaringType.ConstructedInfo != null)
			{
				source = declaringType.ConstructedInfo.UnMap(source);
			}

			IType mappedDeclaringType = MapType(declaringType);
			if (mappedDeclaringType.ConstructedInfo != null)
			{
				return mappedDeclaringType.ConstructedInfo.Map(source);
			}

			return source;
		}

		abstract protected IMember CreateMappedMember(IMember source);

		public IConstructor Map(IConstructor source)
		{
			return (IConstructor)Map((IEntity)source);
		}

		public IMethod Map(IMethod source)
		{
			return (IMethod)Map((IEntity)source);
		}

		public IField Map(IField source)
		{
			return (IField)Map((IEntity)source);
		}

		public IProperty Map(IProperty source)
		{
			return (IProperty)Map((IEntity)source);
		}

		public IEvent Map(IEvent source)
		{
			return (IEvent)Map((IEntity)source);
		}

		internal IGenericParameter MapGenericParameter(IGenericParameter source)
		{
			return Cache<IGenericParameter>(source, new GenericMappedTypeParameter(TypeSystemServices, source, this));
		}

		private IEntity MapAmbiguousEntity(Ambiguous source)
		{
			// Map each individual entity in the ambiguous list 
			return new Ambiguous(Array.ConvertAll<IEntity, IEntity>(source.Entities, Map));
		}

		/// <summary>
		/// Gets the method from which the specified method was mapped.
		/// </summary>
		public virtual IMember UnMap(IMember mapped)
		{
			foreach (KeyValuePair<IMember, IMember> kvp in _memberCache)
			{
				if (kvp.Value == mapped) return kvp.Key;
			}
			return null;
		}

		private IMember CacheMember(IMember source, IMember mapped)
		{
			_memberCache[source] = mapped;
			return mapped;
		}
	}
}