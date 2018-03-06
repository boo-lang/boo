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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem.ReflectionMetadata
{
	using AssemblyFlags = System.Reflection.AssemblyFlags;
	using AssemblyContentType = System.Reflection.AssemblyContentType;
	using AssemblyNameFlags = System.Reflection.AssemblyNameFlags;
	using TypeAttributes = System.Reflection.TypeAttributes;

	public class MetadataAssemblyReference : ICompileUnit, IEquatable<MetadataAssemblyReference>
	{
		private string FullNameRef(string ns, string name)
		{
			return string.Format("{0}::{1}", ns, name);
		}

		private readonly MetadataReader _reader;
		private readonly PEReader _peReader;
		private readonly MetadataTypeSystemProvider _provider;
		private readonly string _assemblyName;
		private INamespace _rootNamespace;

		private readonly MemoizedFunction<TypeSpecification, IType> _typeSpecEntityCache;
		private readonly MemoizedFunction<TypeDefinition, IType> _typeEntityCache;
		private readonly MemoizedFunction<MetadataExternalType, MethodDefinition, IMethod> _methodCache;
		private readonly MemoizedFunction<MetadataExternalType, PropertyDefinition, IProperty> _propertyCache;
		private readonly MemoizedFunction<MetadataExternalType, FieldDefinition, IField> _fieldCache;
		private readonly MemoizedFunction<MetadataExternalType, EventDefinition, IEvent> _eventCache;
		private Dictionary<string, TypeDefinition> _allTypeDefs;
		private readonly Stream _stream;

		internal MetadataAssemblyReference(MetadataTypeSystemProvider provider, string assembly)
		{
			if (string.IsNullOrEmpty(assembly))
				throw new ArgumentNullException("assembly");
			_assemblyName = Path.GetFileNameWithoutExtension(assembly);
			_provider = provider;
			/*
			using (var stream = File.OpenRead(assembly))
			{
				_peReader = new PEReader(stream, PEStreamOptions.PrefetchEntireImage & PEStreamOptions.PrefetchMetadata);
				_reader = _peReader.GetMetadataReader();
			}*/
			_stream = File.OpenRead(assembly);
			_peReader = new PEReader(_stream, PEStreamOptions.PrefetchEntireImage & PEStreamOptions.PrefetchMetadata);
			_reader = _peReader.GetMetadataReader();
			_provider.LoadReferences(_reader, Path.GetDirectoryName(assembly));
			_name = _reader.GetString(_reader.GetAssemblyDefinition().Name);
			_fullName = GetFullName();
			_typeSpecEntityCache = new MemoizedFunction<TypeSpecification, IType>(NewType);
			_typeEntityCache = new MemoizedFunction<TypeDefinition, IType>(CreateEntityForType);
			_methodCache = new MemoizedFunction<MetadataExternalType, MethodDefinition, IMethod>(NewEntityForMethod);
			_propertyCache = new MemoizedFunction<MetadataExternalType, PropertyDefinition, IProperty>(NewEntityForProperty);
			_fieldCache = new MemoizedFunction<MetadataExternalType, FieldDefinition, IField>(NewEntityForField);
			_eventCache = new MemoizedFunction<MetadataExternalType, EventDefinition, IEvent>(NewEntityForEvent);
			DebugAllTypeDefs();
			_allTypeDefs = _reader.TypeDefinitions
				.Select(tdh => _reader.GetTypeDefinition(tdh))
				.Where(td => (!td.NamespaceDefinition.IsNil) && (td.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.Public)
				.ToDictionary(td => FullNameRef(_reader.GetString(td.Namespace), _reader.GetString(td.Name)));
		}

		private void DebugAllTypeDefs()
		{
			var result = _reader.TypeDefinitions
				.Select(tdh => _reader.GetTypeDefinition(tdh))
				.Where(td => (!td.NamespaceDefinition.IsNil) && (td.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.Public);
			var tracking = new HashSet<string>();
			foreach (var td in result)
			{
				var fnr = FullNameRef(_reader.GetString(td.Namespace), _reader.GetString(td.Name));
				if (tracking.Contains(fnr))
				{}
				else {
					tracking.Add(fnr);
				}
			}
		}

		internal IEnumerable<TypeDefinition> AllTypes
		{
			get { return _allTypeDefs.Values; }
		}

		public string Name
		{
			get { return _name; }
		}

		string _name;

		string _fullName;

		public string FullName
		{
			get { return _fullName; }
		}

		internal TypeDefinition TypeDefinitionFromName(string ns, string name)
		{
			return _allTypeDefs[FullNameRef(ns, name)];
		}

		internal MetadataReader Reader { get { return _reader; } }

		private string GetFullName()
		{
			var def = _reader.GetAssemblyDefinition();
			var flags = def.Flags;
			var name = new System.Reflection.AssemblyName
			{
				Name = _name,
				Version = def.Version,
				Flags = (AssemblyNameFlags)(flags & AssemblyFlags.PublicKey),
				ContentType = (AssemblyContentType)((int)(flags & AssemblyFlags.ContentTypeMask) >> 9)
			};
			var blob = _reader.GetBlobBytes(def.PublicKey);
			if ((flags & AssemblyFlags.PublicKey) != 0)
				name.SetPublicKey(blob);
			else name.SetPublicKeyToken(blob);
			return name.FullName;
		}

		public EntityType EntityType
		{
			get { return EntityType.Assembly; }
		}

		#region Implementation of ICompileUnit

		public INamespace RootNamespace
		{
			get
			{
				if (_rootNamespace != null)
					return _rootNamespace;
				return _rootNamespace = CreateRootNamespace();
			}
		}

		private INamespace CreateRootNamespace()
		{
			return new MetadataNamespaceBuilder(_provider, _reader).Build();
		}

		#endregion

		public override bool Equals(object other)
		{
			if (null == other) return false;
			if (this == other) return true;

			MetadataAssemblyReference aref = other as MetadataAssemblyReference;
			return Equals(aref);
		}

		public bool Equals(MetadataAssemblyReference other)
		{
			if (null == other) return false;
			if (this == other) return true;

			return IsReferencing(other._assemblyName);
		}

		private bool IsReferencing(string assembly)
		{
			return _assemblyName.Equals(assembly);
		}

		public override int GetHashCode()
		{
			return _assemblyName.GetHashCode();
		}

		public override string ToString()
		{
			return _fullName;
		}

		public IType Map(TypeDefinition type)
		{
			//AssertAssembly(type);
			return _typeEntityCache.Invoke(type);
		}

		public IType Map(Type type)
		{
			AssertAssembly(type);
			if (type.IsNested)
			{
				var parent = type.DeclaringType;
				var parentType = Map(parent);
				return parentType.GetMembers().OfType<IType>().Single(t => t.Name == type.Name);
			}
			return this.Map(TypeDefinitionFromName(type.Namespace, type.Name));
		}

		public IMethod MapMethod(MetadataExternalType parent, MethodDefinition value)
		{
			return _methodCache.Invoke(parent, value);
		}

		public IField MapField(MetadataExternalType parent, FieldDefinition value)
		{
			return _fieldCache.Invoke(parent, value);
		}

		public IProperty MapProperty(MetadataExternalType parent, PropertyDefinition value)
		{
			return _propertyCache.Invoke(parent, value);
		}

		public IEvent MapEvent(MetadataExternalType parent, EventDefinition value)
		{
			return _eventCache.Invoke(parent, value);
		}

		/*
		public IEntity MapMember(MemberInfo mi)
		{
			AssertAssembly(mi);
			if (mi.MemberType == MemberTypes.NestedType)
				return Map((Type)mi);
			return _memberCache.Invoke(mi);
		}
		*/

		private void AssertAssembly(Type member)
		{
			if (!IsReferencing(member.Module.Assembly.GetName().Name))
				throw new ArgumentException(string.Format("{0} doesn't belong to assembly '{1}'.", member, _assemblyName));
		}

		private IMethod NewEntityForMethod(MetadataExternalType parent, MethodDefinition md)
		{
			var isStatic = (md.Attributes & System.Reflection.MethodAttributes.Static) != 0;
			var name = _reader.GetString(md.Name);
			var isCtor = (isStatic && name == ".cctor") || ((!isStatic) && name == ".ctor");
			if (isCtor)
				new MetadataExternalConstructor(_provider, md, parent, _reader);
			return new MetadataExternalMethod(_provider, md, parent, _reader);
		}

		private IField NewEntityForField(MetadataExternalType parent, FieldDefinition fd)
		{
			return new MetadataExternalField(_provider, fd, parent, _reader);
		}

		private IProperty NewEntityForProperty(MetadataExternalType parent, PropertyDefinition pd)
		{
			return new MetadataExternalProperty(_provider, pd, parent, _reader);
		}

		private IEvent NewEntityForEvent(MetadataExternalType parent, EventDefinition ed)
		{
			return new MetadataExternalEvent(_provider, ed, parent, _reader);
		}

		public void MapTo(Type type, IType entity)
		{
			AssertAssembly(type);
			TypeDefinition typedef;
			if (type.DeclaringType == null)
			{
				typedef = TypeDefinitionFromName(type.Namespace, type.Name);
			} else {
				var parent = TypeDefinitionFromName(type.Namespace, type.DeclaringType.Name);
				typedef = parent.GetNestedTypes()
					.Select(_reader.GetTypeDefinition)
					.Single(td => _reader.GetString(td.Name).Equals(type.Name));
			}
			_typeEntityCache.Add(typedef, entity);
		}

		private IType NewType(TypeSpecification type)
		{
			return type.DecodeSignature(new MetadataSignatureDecoder(_provider, _reader), null);
		}

		private IType CreateEntityForType(TypeDefinition type)
		{
			if (!type.BaseType.IsNil)
			{
				var baseType = _provider.GetTypeFromEntityHandle(type.BaseType, _reader);
				if (baseType.IsAssignableFrom(_provider.Map(Types.MulticastDelegate)))
					return _provider.CreateEntityForCallableType(type);
			}
			return _provider.CreateEntityForRegularType(type, _reader);
		}

		internal TypeDefinition GetTypeDefinition(Type t)
		{
			var result = this.TypeDefinitionFromName(t.Namespace, t.Name);
			return result;
		}

	}
}
