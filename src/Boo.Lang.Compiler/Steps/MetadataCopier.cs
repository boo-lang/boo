#region license
// Copyright (c) 2026 the Boo contributors
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
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Boo.Lang.Compiler.Steps;

/// <summary>
/// Copies metadata row for row, letting each type reference be pointed at a
/// different assembly on the way through.
/// </summary>
/// <remarks>
/// A MetadataBuilder cannot be read back, so the only way to revise what it
/// holds is to serialize it, read it, and build it again. Row order is
/// preserved, which keeps every token embedded in IL, signatures and symbols
/// valid and leaves the method bodies and field data untouched.
/// </remarks>
internal sealed class MetadataCopier
{
	// A method with no body has an RVA of zero, so bodies are laid out above
	// zero to tell that apart from a body at the start of the stream.
	private const int MethodBodyBase = 0x10000;
	private const int MappedFieldDataBase = 0x20000;

	private readonly MetadataReader _source;
	private readonly MetadataBuilder _target = new();
	private readonly Dictionary<int, StringHandle> _strings = new();
	private readonly Dictionary<int, BlobHandle> _blobs = new();

	private MetadataCopier(MetadataReader source)
	{
		_source = source;
	}

	/// <summary>
	/// The assembly each referenced type belongs to, or a nil handle to leave
	/// the reference where it is. Called once per type reference.
	/// </summary>
	internal Func<EntityHandle, string, string, EntityHandle> ResolveScope { get; set; }

	/// <summary>
	/// The identity to write an assembly reference under, or null to keep the
	/// one it has. Called once per assembly reference.
	/// </summary>
	internal Func<string, AssemblyName> ResolveAssemblyName { get; set; }

	/// <summary>
	/// The metadata being read, so a resolver can look up what it is deciding
	/// about.
	/// </summary>
	internal MetadataReader Source => _source;

	/// <summary>
	/// The builder being written, so a scope resolver can add rows to it.
	/// </summary>
	internal MetadataBuilder Target => _target;

	internal static MetadataBuilder Copy(MetadataBuilder source, Action<MetadataCopier> configure)
	{
		var serialized = new BlobBuilder();
		new MetadataRootBuilder(source).Serialize(serialized, MethodBodyBase, MappedFieldDataBase);
		var image = serialized.ToArray();

		using var provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.Create(image));
		var copier = new MetadataCopier(provider.GetMetadataReader());
		configure?.Invoke(copier);
		copier.Run();
		return copier._target;
	}

	private void Run()
	{
		CopyUserStrings();
		CopyModule();
		CopyAssembly();
		CopyAssemblyReferences();
		CopyTypeReferences();
		CopyTypeDefinitions();
		CopyFields();
		CopyMethods();
		CopyParameters();
		CopyInterfaceImplementations();
		CopyMemberReferences();
		CopyConstants();
		CopyCustomAttributes();
		CopyDeclarativeSecurity();
		CopyLayouts();
		CopyStandaloneSignatures();
		CopyEvents();
		CopyProperties();
		CopyMethodSemantics();
		CopyMethodImplementations();
		CopyModuleReferences();
		CopyTypeSpecifications();
		CopyImports();
		CopyFieldRvas();
		CopyFiles();
		CopyExportedTypes();
		CopyManifestResources();
		CopyNestedTypes();
		CopyGenericParameters();
		CopyMethodSpecifications();
		CopyGenericParameterConstraints();
		CopyMarshallingDescriptors();
		VerifyRowCounts();
	}

	/// <summary>
	/// Fields carry a descriptor through their own definition; parameters are
	/// reached by row, having no handle collection of their own.
	/// </summary>
	private void CopyMarshallingDescriptors()
	{
		foreach (var handle in _source.FieldDefinitions)
		{
			var descriptor = _source.GetFieldDefinition(handle).GetMarshallingDescriptor();
			if (!descriptor.IsNil)
				_target.AddMarshallingDescriptor(handle, B(descriptor));
		}

		for (var row = 1; row <= _source.GetTableRowCount(TableIndex.Param); row++)
		{
			var handle = MetadataTokens.ParameterHandle(row);
			var descriptor = _source.GetParameter(handle).GetMarshallingDescriptor();
			if (!descriptor.IsNil)
				_target.AddMarshallingDescriptor(handle, B(descriptor));
		}
	}

	/// <summary>
	/// Every table has to come out the size it went in.
	/// </summary>
	/// <remarks>
	/// A table this class forgets loses its rows rather than failing, which
	/// leaves an assembly wrong in a way nothing else here asks about.
	/// </remarks>
	private void VerifyRowCounts()
	{
		var counts = _target.GetRowCounts();
		foreach (TableIndex table in Enum.GetValues<TableIndex>())
		{
			var expected = _source.GetTableRowCount(table);
			var actual = counts[(int) table];

			// A resolver adds the assembly references the types it repoints
			// need, so that one table is allowed to grow.
			var lost = table == TableIndex.AssemblyRef ? actual < expected : actual != expected;
			if (lost)
				throw new InvalidOperationException(
					$"Metadata copy lost rows in {table}: {expected} became {actual}.");
		}
	}

	private StringHandle S(StringHandle handle)
	{
		if (handle.IsNil)
			return default;

		var key = MetadataTokens.GetHeapOffset(handle);
		if (_strings.TryGetValue(key, out var copy))
			return copy;

		copy = _target.GetOrAddString(_source.GetString(handle));
		_strings.Add(key, copy);
		return copy;
	}

	private BlobHandle B(BlobHandle handle)
	{
		if (handle.IsNil)
			return default;

		var key = MetadataTokens.GetHeapOffset(handle);
		if (_blobs.TryGetValue(key, out var copy))
			return copy;

		copy = _target.GetOrAddBlob(_source.GetBlobBytes(handle));
		_blobs.Add(key, copy);
		return copy;
	}

	/// <summary>
	/// The user string heap is addressed by offset straight out of ldstr, so
	/// the entries are re-added in their original order to land where they
	/// were.
	/// </summary>
	private void CopyUserStrings()
	{
		foreach (var text in UserStrings())
			_target.GetOrAddUserString(text);
	}

	private IEnumerable<string> UserStrings()
	{
		var size = _source.GetHeapSize(HeapIndex.UserString);
		var offset = 1; // the heap opens with an empty entry
		while (offset < size)
		{
			var handle = MetadataTokens.UserStringHandle(offset);
			var text = _source.GetUserString(handle);
			yield return text;

			// The entry is the compressed byte count, the UTF-16 characters,
			// and one trailing byte saying whether any of them is non-ASCII.
			var bytes = text.Length * 2 + 1;
			offset += CompressedSize(bytes) + bytes;
		}
	}

	private static int CompressedSize(int value)
	{
		if (value < 0x80) return 1;
		return value < 0x4000 ? 2 : 4;
	}

	private void CopyModule()
	{
		var module = _source.GetModuleDefinition();
		_target.AddModule(
			module.Generation,
			S(module.Name),
			_target.GetOrAddGuid(_source.GetGuid(module.Mvid)),
			module.GenerationId.IsNil ? default : _target.GetOrAddGuid(_source.GetGuid(module.GenerationId)),
			module.BaseGenerationId.IsNil ? default : _target.GetOrAddGuid(_source.GetGuid(module.BaseGenerationId)));
	}

	private void CopyAssembly()
	{
		if (_source.GetTableRowCount(TableIndex.Assembly) == 0)
			return;

		var assembly = _source.GetAssemblyDefinition();
		_target.AddAssembly(
			S(assembly.Name),
			assembly.Version,
			S(assembly.Culture),
			B(assembly.PublicKey),
			assembly.Flags,
			assembly.HashAlgorithm);
	}

	private void CopyAssemblyReferences()
	{
		foreach (var handle in _source.AssemblyReferences)
		{
			var reference = _source.GetAssemblyReference(handle);
			var replacement = ResolveAssemblyName?.Invoke(_source.GetString(reference.Name));
			if (replacement != null)
			{
				_target.AddAssemblyReference(
					_target.GetOrAddString(replacement.Name),
					replacement.Version,
					default,
					_target.GetOrAddBlob(replacement.GetPublicKeyToken() ?? Array.Empty<byte>()),
					default,
					default);
				continue;
			}

			_target.AddAssemblyReference(
				S(reference.Name),
				reference.Version,
				S(reference.Culture),
				B(reference.PublicKeyOrToken),
				reference.Flags,
				B(reference.HashValue));
		}
	}

	private void CopyTypeReferences()
	{
		foreach (var handle in _source.TypeReferences)
		{
			var type = _source.GetTypeReference(handle);
			var scope = type.ResolutionScope;

			if (ResolveScope != null)
			{
				var resolved = ResolveScope(
					scope,
					_source.GetString(type.Namespace),
					_source.GetString(type.Name));
				if (!resolved.IsNil)
					scope = resolved;
			}

			_target.AddTypeReference(scope, S(type.Namespace), S(type.Name));
		}
	}

	/// <summary>
	/// A list column points one past the end when the list is empty, so the
	/// rows carry a running count rather than a nil handle.
	/// </summary>
	private void CopyTypeDefinitions()
	{
		var field = 1;
		var method = 1;
		foreach (var handle in _source.TypeDefinitions)
		{
			var type = _source.GetTypeDefinition(handle);
			_target.AddTypeDefinition(
				type.Attributes,
				S(type.Namespace),
				S(type.Name),
				type.BaseType,
				MetadataTokens.FieldDefinitionHandle(field),
				MetadataTokens.MethodDefinitionHandle(method));

			foreach (var _ in type.GetFields()) field++;
			foreach (var _ in type.GetMethods()) method++;
		}
	}

	private void CopyFields()
	{
		foreach (var handle in _source.FieldDefinitions)
		{
			var field = _source.GetFieldDefinition(handle);
			_target.AddFieldDefinition(field.Attributes, S(field.Name), B(field.Signature));
		}
	}

	private void CopyMethods()
	{
		var parameter = 1;
		foreach (var handle in _source.MethodDefinitions)
		{
			var method = _source.GetMethodDefinition(handle);
			var body = method.RelativeVirtualAddress == 0
				? -1
				: method.RelativeVirtualAddress - MethodBodyBase;

			_target.AddMethodDefinition(
				method.Attributes,
				method.ImplAttributes,
				S(method.Name),
				B(method.Signature),
				body,
				MetadataTokens.ParameterHandle(parameter));

			foreach (var _ in method.GetParameters()) parameter++;
		}
	}

	private void CopyParameters()
	{
		for (var row = 1; row <= _source.GetTableRowCount(TableIndex.Param); row++)
		{
			var parameter = _source.GetParameter(MetadataTokens.ParameterHandle(row));
			_target.AddParameter(parameter.Attributes, S(parameter.Name), parameter.SequenceNumber);
		}
	}

	/// <summary>
	/// Sorted by the implementing type, which is the order the type
	/// definitions are already in.
	/// </summary>
	private void CopyInterfaceImplementations()
	{
		foreach (var handle in _source.TypeDefinitions)
			foreach (var implementation in _source.GetTypeDefinition(handle).GetInterfaceImplementations())
				_target.AddInterfaceImplementation(
					handle,
					_source.GetInterfaceImplementation(implementation).Interface);
	}

	private void CopyMemberReferences()
	{
		foreach (var handle in _source.MemberReferences)
		{
			var member = _source.GetMemberReference(handle);
			_target.AddMemberReference(member.Parent, S(member.Name), B(member.Signature));
		}
	}

	private void CopyConstants()
	{
		for (var row = 1; row <= _source.GetTableRowCount(TableIndex.Constant); row++)
		{
			var constant = _source.GetConstant(MetadataTokens.ConstantHandle(row));
			var value = _source.GetBlobReader(constant.Value).ReadConstant(constant.TypeCode);
			_target.AddConstant(constant.Parent, value);
		}
	}

	private void CopyCustomAttributes()
	{
		foreach (var handle in _source.CustomAttributes)
		{
			var attribute = _source.GetCustomAttribute(handle);
			_target.AddCustomAttribute(attribute.Parent, attribute.Constructor, B(attribute.Value));
		}
	}

	private void CopyDeclarativeSecurity()
	{
		foreach (var handle in _source.DeclarativeSecurityAttributes)
		{
			var security = _source.GetDeclarativeSecurityAttribute(handle);
			_target.AddDeclarativeSecurityAttribute(security.Parent, security.Action, B(security.PermissionSet));
		}
	}

	private void CopyLayouts()
	{
		foreach (var handle in _source.TypeDefinitions)
		{
			var layout = _source.GetTypeDefinition(handle).GetLayout();
			if (!layout.IsDefault)
				_target.AddTypeLayout(handle, (ushort) layout.PackingSize, (uint) layout.Size);
		}

		foreach (var handle in _source.FieldDefinitions)
		{
			var offset = _source.GetFieldDefinition(handle).GetOffset();
			if (offset >= 0)
				_target.AddFieldLayout(handle, offset);
		}
	}

	private void CopyStandaloneSignatures()
	{
		for (var row = 1; row <= _source.GetTableRowCount(TableIndex.StandAloneSig); row++)
		{
			var signature = _source.GetStandaloneSignature(MetadataTokens.StandaloneSignatureHandle(row));
			_target.AddStandaloneSignature(B(signature.Signature));
		}
	}

	private void CopyEvents()
	{
		foreach (var handle in _source.EventDefinitions)
		{
			var @event = _source.GetEventDefinition(handle);
			_target.AddEvent(@event.Attributes, S(@event.Name), @event.Type);
		}

		foreach (var handle in _source.TypeDefinitions)
		{
			var events = _source.GetTypeDefinition(handle).GetEvents();
			if (events.Count > 0)
				_target.AddEventMap(handle, First(events));
		}
	}

	private void CopyProperties()
	{
		foreach (var handle in _source.PropertyDefinitions)
		{
			var property = _source.GetPropertyDefinition(handle);
			_target.AddProperty(property.Attributes, S(property.Name), B(property.Signature));
		}

		foreach (var handle in _source.TypeDefinitions)
		{
			var properties = _source.GetTypeDefinition(handle).GetProperties();
			if (properties.Count > 0)
				_target.AddPropertyMap(handle, First(properties));
		}
	}

	private static EventDefinitionHandle First(EventDefinitionHandleCollection events)
	{
		foreach (var handle in events) return handle;
		return default;
	}

	private static PropertyDefinitionHandle First(PropertyDefinitionHandleCollection properties)
	{
		foreach (var handle in properties) return handle;
		return default;
	}

	/// <summary>
	/// One row per accessor, found through the event or property it belongs to.
	/// </summary>
	private void CopyMethodSemantics()
	{
		foreach (var handle in _source.EventDefinitions)
		{
			var accessors = _source.GetEventDefinition(handle).GetAccessors();
			Semantics(handle, accessors.Adder, MethodSemanticsAttributes.Adder);
			Semantics(handle, accessors.Remover, MethodSemanticsAttributes.Remover);
			Semantics(handle, accessors.Raiser, MethodSemanticsAttributes.Raiser);
			foreach (var other in accessors.Others)
				Semantics(handle, other, MethodSemanticsAttributes.Other);
		}

		foreach (var handle in _source.PropertyDefinitions)
		{
			var accessors = _source.GetPropertyDefinition(handle).GetAccessors();
			Semantics(handle, accessors.Getter, MethodSemanticsAttributes.Getter);
			Semantics(handle, accessors.Setter, MethodSemanticsAttributes.Setter);
			foreach (var other in accessors.Others)
				Semantics(handle, other, MethodSemanticsAttributes.Other);
		}
	}

	private void Semantics(EntityHandle association, MethodDefinitionHandle accessor, MethodSemanticsAttributes semantics)
	{
		if (!accessor.IsNil)
			_target.AddMethodSemantics(association, semantics, accessor);
	}

	private void CopyMethodImplementations()
	{
		foreach (var handle in _source.TypeDefinitions)
			foreach (var implementation in _source.GetTypeDefinition(handle).GetMethodImplementations())
			{
				var method = _source.GetMethodImplementation(implementation);
				_target.AddMethodImplementation(handle, method.MethodBody, method.MethodDeclaration);
			}
	}

	private void CopyModuleReferences()
	{
		for (var row = 1; row <= _source.GetTableRowCount(TableIndex.ModuleRef); row++)
		{
			var module = _source.GetModuleReference(MetadataTokens.ModuleReferenceHandle(row));
			_target.AddModuleReference(S(module.Name));
		}
	}

	private void CopyTypeSpecifications()
	{
		for (var row = 1; row <= _source.GetTableRowCount(TableIndex.TypeSpec); row++)
		{
			var specification = _source.GetTypeSpecification(MetadataTokens.TypeSpecificationHandle(row));
			_target.AddTypeSpecification(B(specification.Signature));
		}
	}

	private void CopyImports()
	{
		foreach (var handle in _source.MethodDefinitions)
		{
			var import = _source.GetMethodDefinition(handle).GetImport();
			if (!import.Module.IsNil)
				_target.AddMethodImport(handle, import.Attributes, S(import.Name), import.Module);
		}
	}

	private void CopyFieldRvas()
	{
		foreach (var handle in _source.FieldDefinitions)
		{
			var rva = _source.GetFieldDefinition(handle).GetRelativeVirtualAddress();
			if (rva != 0)
				_target.AddFieldRelativeVirtualAddress(handle, rva - MappedFieldDataBase);
		}
	}

	private void CopyFiles()
	{
		foreach (var handle in _source.AssemblyFiles)
		{
			var file = _source.GetAssemblyFile(handle);
			_target.AddAssemblyFile(S(file.Name), B(file.HashValue), file.ContainsMetadata);
		}
	}

	private void CopyExportedTypes()
	{
		foreach (var handle in _source.ExportedTypes)
		{
			var type = _source.GetExportedType(handle);
			_target.AddExportedType(
				type.Attributes,
				S(type.Namespace),
				S(type.Name),
				type.Implementation,
				type.IsForwarder ? 0 : type.GetTypeDefinitionId());
		}
	}

	private void CopyManifestResources()
	{
		foreach (var handle in _source.ManifestResources)
		{
			var resource = _source.GetManifestResource(handle);
			_target.AddManifestResource(
				resource.Attributes,
				S(resource.Name),
				resource.Implementation,
				(uint) resource.Offset);
		}
	}

	/// <summary>
	/// Sorted by the nested type rather than the enclosing one, so the pairs
	/// are gathered before they are written.
	/// </summary>
	private void CopyNestedTypes()
	{
		var nested = new List<(TypeDefinitionHandle Type, TypeDefinitionHandle Enclosing)>();
		foreach (var handle in _source.TypeDefinitions)
			foreach (var inner in _source.GetTypeDefinition(handle).GetNestedTypes())
				nested.Add((inner, handle));

		nested.Sort((left, right) =>
			MetadataTokens.GetRowNumber(left.Type).CompareTo(MetadataTokens.GetRowNumber(right.Type)));

		foreach (var pair in nested)
			_target.AddNestedType(pair.Type, pair.Enclosing);
	}

	private void CopyGenericParameters()
	{
		for (var row = 1; row <= _source.GetTableRowCount(TableIndex.GenericParam); row++)
		{
			var parameter = _source.GetGenericParameter(MetadataTokens.GenericParameterHandle(row));
			_target.AddGenericParameter(parameter.Parent, parameter.Attributes, S(parameter.Name), parameter.Index);
		}
	}

	private void CopyMethodSpecifications()
	{
		for (var row = 1; row <= _source.GetTableRowCount(TableIndex.MethodSpec); row++)
		{
			var specification = _source.GetMethodSpecification(MetadataTokens.MethodSpecificationHandle(row));
			_target.AddMethodSpecification(specification.Method, B(specification.Signature));
		}
	}

	private void CopyGenericParameterConstraints()
	{
		for (var row = 1; row <= _source.GetTableRowCount(TableIndex.GenericParamConstraint); row++)
		{
			var constraint = _source.GetGenericParameterConstraint(
				MetadataTokens.GenericParameterConstraintHandle(row));
			_target.AddGenericParameterConstraint(constraint.Parameter, constraint.Type);
		}
	}
}
