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
using System.Linq;
using System.Reflection.Metadata;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem.ReflectionMetadata
{
	public class MetadataTypeSystemProvider
	{
		private readonly Dictionary<string, MetadataAssemblyReference> _referenceCache;

		private Dictionary<TypeDefinition, MetadataAssemblyReference> _assemblyTypeMap =
			new Dictionary<TypeDefinition, MetadataAssemblyReference>();

		public MetadataTypeSystemProvider()
		{
			_referenceCache = new Dictionary<string, MetadataAssemblyReference>();
			_typeDefCache = new MemoizedFunction<MetadataReader, TypeDefinition, MetadataExternalType>(TypeFromDefinition);
			_typeRefCache = new MemoizedFunction<MetadataReader, TypeReference, TypeDefinition>(DereferenceTypeRef);
			Initialize();
		}

		virtual protected void Initialize()
		{
			MapTo(typeof(object), new ObjectTypeImpl(this));
			MapTo(typeof(Builtins.duck), new ObjectTypeImpl(this));
			MapTo(typeof(void), new VoidTypeImpl(this));

			//special cases to avoid a stack overflow
			var asm = AssemblyReferenceFor(typeof(object).Assembly);
			var mDel = asm.TypeDefinitionFromName("System", "MulticastDelegate");
			MapTo(typeof(System.MulticastDelegate), new MetadataExternalType(this, mDel, asm.Reader));
		}

		protected void MapTo(Type type, IType entity)
		{
			AssemblyReferenceFor(type.Assembly).MapTo(type, entity);
		}

		private MetadataTypeSystemProvider(
			Dictionary<string, MetadataAssemblyReference> referenceCache)
		{
			_referenceCache = referenceCache;
		}

		#region Implementation of ICompilerReferenceProvider`

		public ICompileUnit ForAssembly(System.Reflection.Assembly assembly)
		{
			return AssemblyReferenceFor(assembly);
		}

		private MetadataAssemblyReference AssemblyReferenceFor(System.Reflection.Assembly assembly)
		{
			var name = System.IO.Path.GetFileNameWithoutExtension(assembly.Location);
			MetadataAssemblyReference result;
			if (!_referenceCache.TryGetValue(name, out result))
			{
				result = CreateReference(assembly.Location);
				_referenceCache.Add(name, result);
			}
			return result;
		}

		private HashSet<string> _loadingReferences = new HashSet<string>();

		internal void LoadReferences(MetadataReader reader, string localDir)
		{
			var refs = reader.AssemblyReferences.Select(reader.GetAssemblyReference);
			foreach (var reference in refs)
			{
				var name = reader.GetString(reference.Name);
				if (!(_referenceCache.ContainsKey(name) || _loadingReferences.Contains(name)))
				{
					_loadingReferences.Add(name);
					try
					{
						var loadPath = MetadataReferenceResolver.FindAssembly(reference, reader, localDir);
						var newAsm = CreateReference(loadPath);
						_referenceCache.Add(name, newAsm);
					}
					finally {
						_loadingReferences.Remove(name);
					}
				}
			}
		}

		private MetadataAssemblyReference CreateReference(string assembly)
		{
			var result = new MetadataAssemblyReference(this, assembly);
			foreach (var type in result.AllTypes)
				_assemblyTypeMap[type] = result;
			_assemblyCache[result.Reader] = result;
			return result;
		}

		#endregion

		private MemoizedFunction<MetadataReader, TypeDefinition, MetadataExternalType> _typeDefCache;
		private MemoizedFunction<MetadataReader, TypeReference, TypeDefinition> _typeRefCache;
		private Dictionary<MetadataReader, MetadataAssemblyReference> _assemblyCache = 
			new Dictionary<MetadataReader, MetadataAssemblyReference>();

		private MetadataExternalType TypeFromDefinition(MetadataReader reader, TypeDefinition definition)
		{
			return new MetadataExternalType(this, definition, reader);
		}

		internal IType GetTypeFromDefinition(TypeDefinition definition, MetadataReader reader)
		{
			return _typeDefCache.Invoke(reader, definition);
		}

		private TypeDefinition DereferenceTypeRefByName(MetadataReader reader,
			string ns, string name)
		{
			MetadataAssemblyReference ma = _assemblyCache[reader];
			return ma.TypeDefinitionFromName(ns, name);
		}

		private TypeDefinition DereferenceTypeRef(MetadataReader reader, TypeReference reference)
		{
			var scope = reference.ResolutionScope;
			var ns = reader.GetString(reference.Namespace);
			var name = reader.GetString(reference.Name);
			switch (scope.Kind)
			{
				case HandleKind.ModuleDefinition:
					return DereferenceTypeRefByName(reader, ns, name);
				case HandleKind.TypeReference:
					var parent = reader.GetTypeReference((TypeReferenceHandle)scope);
					return DereferenceNestedTypeRef(reader, DereferenceTypeRef(reader, parent), name);
				case HandleKind.ModuleReference:
					var mr = reader.GetModuleReference((ModuleReferenceHandle)scope);
					return DereferenceModuleRefTypeRef(reader, mr, ns, name);
				case HandleKind.AssemblyReference:
					var ar = reader.GetAssemblyReference((AssemblyReferenceHandle)scope);
					return DereferenceAssemblyRefTypeRef(reader, ar, ns, name);
				default:
					throw new NotSupportedException(string.Format("Unknown scope kind {0}", scope.Kind));
			}
		}

		private TypeDefinition DereferenceNestedTypeRef(MetadataReader reader, TypeDefinition typeDefinition, string name)
		{
			var subtypes = typeDefinition.GetNestedTypes().Select(h => reader.GetTypeDefinition(h));
			return subtypes.First(td => reader.GetString(td.Name).Equals(name));
		}

		private TypeDefinition DereferenceAssemblyRefTypeRef(MetadataReader reader, AssemblyReference ar, string ns, string name)
		{
			var asmName = reader.GetString(ar.Name);
			var otherReader = _referenceCache[asmName].Reader;
			return DereferenceTypeRefByName(otherReader, ns, name);
		}

		private TypeDefinition DereferenceModuleRefTypeRef(MetadataReader reader, ModuleReference mr, string ns, string name)
		{
			throw new NotImplementedException();
		}

		internal IType GetTypeFromReference(TypeReference reference, MetadataReader reader)
		{
			var definition = _typeRefCache.Invoke(reader, reference);
			return _typeDefCache.Invoke(reader, definition);
		}

		internal IType GetTypeFromEntityHandle(EntityHandle handle, MetadataReader reader)
		{
			if (handle.IsNil)
				return null;
			switch (handle.Kind)
			{
				case HandleKind.TypeDefinition:
					return _typeDefCache.Invoke(reader, reader.GetTypeDefinition((TypeDefinitionHandle)handle));
				case HandleKind.TypeReference:
					return GetTypeFromReference(reader.GetTypeReference((TypeReferenceHandle)handle), reader);
				case HandleKind.TypeSpecification:
					var spec = reader.GetTypeSpecification((TypeSpecificationHandle)handle);
					return spec.DecodeSignature(new MetadataSignatureDecoder(this, reader), null);
				default:
					throw new NotSupportedException(string.Format("Unknown type handle kind {0}", handle.Kind));
			}
		}

		internal IEnumerable<IType> GetCustomAttributeTypes(CustomAttributeHandleCollection attrs, MetadataReader reader)
		{
			foreach (var attr in attrs)
			{
				var ctor = reader.GetCustomAttribute(attr).Constructor;
				switch (ctor.Kind)
				{
					case HandleKind.MethodDefinition:
						var md = reader.GetMethodDefinition((MethodDefinitionHandle)ctor);
						yield return _typeDefCache.Invoke(reader, reader.GetTypeDefinition(md.GetDeclaringType()));
						break;
					case HandleKind.MemberReference:
						var mr = reader.GetMemberReference((MemberReferenceHandle)ctor);
						yield return GetTypeFromEntityHandle(mr.Parent, reader);
						break;
					default:
						throw new NotSupportedException(string.Format("Unknown ctor handle kind {0}", ctor.Kind));
				}
			}
		}

		#region Implementation of IReflectionTypeSystemProvider

		public TypeDefinition GetTypeDefinition(Type t)
		{
			return AssemblyReferenceFor(t.Assembly).GetTypeDefinition(t);
		}

		public IType Map(TypeDefinition type, MetadataReader reader)
		{
			return _typeDefCache.Invoke(reader, type);
		}

		public IType Map(Type type)
		{
			return AssemblyReferenceFor(type.Assembly).Map(type);
		}

		public IMethod Map(MetadataExternalType parent, MethodDefinition method)
		{
			return _assemblyTypeMap[parent.ActualType].MapMethod(parent, method);
		}

		public IField Map(MetadataExternalType parent, FieldDefinition field)
		{
			return _assemblyTypeMap[parent.ActualType].MapField(parent, field);
		}

		public IProperty Map(MetadataExternalType parent, PropertyDefinition prop)
		{
			return _assemblyTypeMap[parent.ActualType].MapProperty(parent, prop);
		}

		public IEvent Map(MetadataExternalType parent, EventDefinition ev)
		{
			return _assemblyTypeMap[parent.ActualType].MapEvent(parent, ev);
		}

		/*
		public IParameter[] Map(Parameter[] parameters, MetadataReader reader)
		{
			return parameters.Select(p => new MetadataExternalParameter(p, reader)).ToArray();
		}
		*/

		public virtual MetadataTypeSystemProvider Clone()
		{
			return new MetadataTypeSystemProvider(new Dictionary<string, MetadataAssemblyReference>(_referenceCache));
		}

		public virtual IType CreateEntityForRegularType(TypeDefinition type, MetadataReader reader)
		{
			return new MetadataExternalType(this, type, reader);
		}

		public virtual IType CreateEntityForCallableType(TypeDefinition type)
		{
			return new MetadataExternalCallableType(this, type, _assemblyTypeMap[type].Reader);
		}

		#endregion

		sealed class ObjectTypeImpl : MetadataExternalType
		{
			internal ObjectTypeImpl(MetadataTypeSystemProvider provider)
				: base(provider,
					  provider.GetTypeDefinition(Types.Object),
					  provider.AssemblyReferenceFor(Types.Object.Assembly).Reader)
			{
				
			}

			public override bool IsAssignableFrom(IType other)
			{
				return !other.IsVoid;
			}
		}

		#region VoidTypeImpl
		sealed class VoidTypeImpl : MetadataExternalType
		{
			internal VoidTypeImpl(MetadataTypeSystemProvider provider)
				: base(provider,
					  provider.GetTypeDefinition(Types.Void),
					  provider.AssemblyReferenceFor(Types.Void.Assembly).Reader)
			{
			}

			override public bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
			{
				return false;
			}

			override public bool IsSubclassOf(IType other)
			{
				return false;
			}

			override public bool IsAssignableFrom(IType other)
			{
				return false;
			}

			public override bool IsVoid
			{
				get { return true; }
			}
		}

		#endregion

	}
}
