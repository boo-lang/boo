#region license
// Copyright (c) 2021, Mason Wheeler
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//	   * Redistributions of source code must retain the above copyright notice,
//	   this list of conditions and the following disclaimer.
//	   * Redistributions in binary form must reproduce the above copyright notice,
//	   this list of conditions and the following disclaimer in the documentation
//	   and/or other materials provided with the distribution.
//	   * Neither the name of Rodrigo B. de Oliveira nor the names of its
//	   contributors may be used to endorse or promote products derived from this
//	   software without specific prior written permission.
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Security;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Resources;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Runtime;
using Attribute = Boo.Lang.Compiler.Ast.Attribute;
using Module = Boo.Lang.Compiler.Ast.Module;
using Method = Boo.Lang.Compiler.Ast.Method;
using ExceptionHandler = Boo.Lang.Compiler.Ast.ExceptionHandler;
using TypeDefinition = Boo.Lang.Compiler.Ast.TypeDefinition;
using TypeReference = Boo.Lang.Compiler.Ast.TypeReference;
using Boo.Lang.Compiler.Steps.Ecma335;
using System.Reflection.PortableExecutable;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps
{
	sealed class LoopInfoCore
	{
		public LabelHandle BreakLabel;

		public LabelHandle ContinueLabel;

		public int TryBlockDepth;

		public LoopInfoCore(LabelHandle breakLabel, LabelHandle continueLabel, int tryBlockDepth)
		{
			BreakLabel = breakLabel;
			ContinueLabel = continueLabel;
			TryBlockDepth = tryBlockDepth;
		}
	}

	public class EmitAssemblyCore : AbstractFastVisitorCompilerStep
	{
		private static readonly PropertyInfo[] RuntimeCompatibilityAttribute_Property = new[] { Properties.Of<System.Runtime.CompilerServices.RuntimeCompatibilityAttribute, bool>(a => a.WrapNonExceptionThrows) };
		private static readonly ConstructorInfo DebuggableAttribute_Constructor = Methods.ConstructorOf(() => new DebuggableAttribute(DebuggableAttribute.DebuggingModes.Default));
		private static readonly ConstructorInfo ParamArrayAttribute_Constructor = Methods.ConstructorOf(() => new ParamArrayAttribute());
		private static readonly ConstructorInfo DuckTypedAttribute_Constructor = Methods.ConstructorOf(() => new DuckTypedAttribute());
		private static readonly MethodInfo Hash_Add = Types.Hash.GetMethod("Add", new Type[] { typeof(object), typeof(object) });
		static ConstructorInfo RuntimeCompatibilityAttribute_Constructor = Methods.ConstructorOf(() => new System.Runtime.CompilerServices.RuntimeCompatibilityAttribute());
		static ConstructorInfo SerializableAttribute_Constructor = Methods.ConstructorOf(() => new SerializableAttribute());
		static MethodInfo RuntimeServices_NormalizeArrayIndex = Methods.Of<Array, int, int>(RuntimeServices.NormalizeArrayIndex);
		static MethodInfo RuntimeServices_ToBool_Object = Types.RuntimeServices.GetMethod("ToBool", new Type[] { Types.Object });
		static MethodInfo RuntimeServices_ToBool_Decimal = Types.RuntimeServices.GetMethod("ToBool", new Type[] { Types.Decimal });
		static MethodInfo Builtins_ArrayTypedConstructor = Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.Int });
		static MethodInfo Builtins_ArrayGenericConstructor = Types.Builtins.GetMethod("array", new Type[] { Types.Int });
		static MethodInfo Builtins_ArrayTypedCollectionConstructor = Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.ICollection });
		private static MethodInfo Array_get_Length = Methods.GetterOf<Array, int>(a => a.Length);
		static MethodInfo Math_Pow = Methods.Of<double, double, double>(Math.Pow);
		static ConstructorInfo List_EmptyConstructor = Types.List.GetConstructor(Type.EmptyTypes);
		static ConstructorInfo List_ArrayBoolConstructor = Types.List.GetConstructor(new Type[] { Types.ObjectArray, Types.Bool });
		static ConstructorInfo Hash_Constructor = Types.Hash.GetConstructor(Type.EmptyTypes);
		static ConstructorInfo Regex_Constructor = typeof(Regex).GetConstructor(new Type[] { Types.String });
		static ConstructorInfo Regex_Constructor_Options = typeof(Regex).GetConstructor(new Type[] { Types.String, typeof(RegexOptions) });
		private static ConstructorInfo TimeSpan_LongConstructor = Methods.ConstructorOf(() => new TimeSpan(default(long)));
		private static MethodInfo Type_GetTypeFromHandle = Methods.Of<RuntimeTypeHandle, Type>(Type.GetTypeFromHandle);
		static MethodInfo String_IsNullOrEmpty = Methods.Of<string, bool>(string.IsNullOrEmpty);
		static MethodInfo RuntimeHelpers_InitializeArray = Methods.Of<Array, RuntimeFieldHandle>(System.Runtime.CompilerServices.RuntimeHelpers.InitializeArray);

		private IMethod _arrayGetLength;
		private IMethod _builtinsArrayGenericConstructor;
		private IMethod _builtinsArrayTypedConstructor;
		private IMethod _builtinsArrayTypedCollectionConstructor;
		private IConstructor _timeSpanLongConstructor;
		private IConstructor _hashConstructor;
		private IConstructor _listEmptyConstructor;
		private IConstructor _listArrayBoolConstructor;
		private IConstructor _regexConstructor;
		private IConstructor _regexConstructorOptions;
		private IMethod _mathPow;
		private IMethod _runtimeServicesToBoolObject;
		private IMethod _runtimeServicesToBoolDecimal;
		private IMethod _runtimeServicesNormalizeArrayIndex;
		private IMethod _stringIsNullOrEmpty;
		private IMethod _typeGetTypeFromHandle;
		private EntityHandle _stringEmptyField;
		private IMethod _hashAdd;

		private MetadataBuilder _asmBuilder = new();
		private AssemblyDefinitionHandle _asmHandle;
		private ModuleDefinitionHandle _moduleBuilder;

		readonly Dictionary<string, ISymbolDocumentWriter> _symbolDocWriters = new();

		// IL generation state
		BlobBuilder _ilBlock = new();
		MethodBuilder _il;
		Method _method;       //current method
		int _returnStatements;//number of return statements in current method
		bool _hasLeaveWithStoredValue;//has a explicit return inside a try block (a `leave')
		bool _returnImplicit; //method ends with an implicit return
		LabelHandle _returnLabel;   //label for `ret'
		LabelHandle _implicitLabel; //label for implicit return (with default value)
		LabelHandle _leaveLabel;    //label to load the stored return value (because of a `leave')
		IType _returnType;
		int _tryBlock; // are we in a try block?
		bool _checked = true;
		bool _rawArrayIndexing = false;
		bool _perModuleRawArrayIndexing = false;

		readonly Dictionary<IType, Type> _typeCache = new();
		readonly List<Method> _moduleConstructorMethods = new();

		// keeps track of types on the IL stack
		readonly Stack<IType> _types = new();

		readonly Stack<LoopInfoCore> _LoopInfoCoreStack = new();

		readonly AttributeCollection _assemblyAttributes = new();
		TypeSystemBridge _typeSystem ;

		LoopInfoCore _currentLoopInfoCore;

		public void SetupTypeSystem()
		{
			_typeSystem = new(_asmBuilder, TypeSystemServices, this.GetValue);
			_arrayGetLength = TypeSystemServices.Map(Array_get_Length);
			_builtinsArrayGenericConstructor = TypeSystemServices.Map(Builtins_ArrayGenericConstructor);
			_builtinsArrayTypedConstructor = TypeSystemServices.Map(Builtins_ArrayTypedConstructor);
			_builtinsArrayTypedCollectionConstructor = TypeSystemServices.Map(Builtins_ArrayTypedCollectionConstructor);
			_timeSpanLongConstructor = TypeSystemServices.Map(TimeSpan_LongConstructor);
			_hashConstructor = TypeSystemServices.Map(Hash_Constructor);
			_listEmptyConstructor = TypeSystemServices.Map(List_EmptyConstructor);
			_listArrayBoolConstructor = TypeSystemServices.Map(List_ArrayBoolConstructor);
			_regexConstructor = TypeSystemServices.Map(Regex_Constructor);
			_regexConstructorOptions = TypeSystemServices.Map(Regex_Constructor_Options);
			_mathPow = TypeSystemServices.Map(Math_Pow);
			_runtimeServicesToBoolObject = TypeSystemServices.Map(RuntimeServices_ToBool_Object);
			_runtimeServicesToBoolDecimal = TypeSystemServices.Map(RuntimeServices_ToBool_Decimal);
			_stringIsNullOrEmpty = TypeSystemServices.Map(String_IsNullOrEmpty);
			_typeGetTypeFromHandle = TypeSystemServices.Map(Type_GetTypeFromHandle);
			_hashAdd = TypeSystemServices.Map(Hash_Add);
			_runtimeServicesNormalizeArrayIndex = TypeSystemServices.Map(RuntimeServices_NormalizeArrayIndex);
			_stringEmptyField = _typeSystem.FieldOf(TypeSystemServices.StringType, "Empty");
			_nullableHasValueBase = TypeSystemServices.Map(Types.Nullable.GetProperty("HasValue").GetGetMethod());
		}

		void EnterLoop(LabelHandle breakLabel, LabelHandle continueLabel)
		{
			_LoopInfoCoreStack.Push(_currentLoopInfoCore);
			_currentLoopInfoCore = new LoopInfoCore(breakLabel, continueLabel, _tryBlock);
		}

		bool InTryInLoop()
		{
			return _tryBlock > _currentLoopInfoCore.TryBlockDepth;
		}

		void LeaveLoop()
		{
			_currentLoopInfoCore = _LoopInfoCoreStack.Pop();
		}

		void PushType(IType type)
		{
			_types.Push(type);
		}

		void PushBool()
		{
			PushType(TypeSystemServices.BoolType);
		}

		void PushVoid()
		{
			PushType(TypeSystemServices.VoidType);
		}

		IType PopType()
		{
			return _types.Pop();
		}

		IType PeekTypeOnStack()
		{
			return (_types.Count != 0) ? _types.Peek() : null;
		}

		public override void Run()
		{
			if (Errors.Count > 0)
				return;

			SetupTypeSystem();
			GatherAssemblyAttributes();
			SetUpAssembly();

			DefineTypes();

			DefineResources();
			DefineAssemblyAttributes();
			DefineEntryPoint();
			DefineModuleConstructor();

			// Define the unmanaged information resources, which 
			// contains the attribute informaion applied earlier,
			// plus icon data if applicable
			DefineUnmanagedResource();
			FinalizeAssembly();
		}

        private void FinalizeAssembly()
        {
			_typeSystem.BuildGenericParameters();
			_typeSystem.BuildNestedTypes();
			var rootBuilder = new MetadataRootBuilder(_asmBuilder);
			var header = new PEHeaderBuilder(
				imageCharacteristics: Characteristics.ExecutableImage | Characteristics.Dll | Characteristics.LargeAddressAware,
				majorImageVersion: 1);
			DebugDirectoryBuilder debugBuilder = null;
			if (Parameters.Debug)
            {
				var rows = _asmBuilder.GetRowCounts();
				var pdb = new PortablePdbBuilder(_typeSystem.DebugBuilder, rows, default);
				var pdbBlob = new BlobBuilder();
				pdb.Serialize(pdbBlob);
				debugBuilder = new();
				debugBuilder.AddEmbeddedPortablePdbEntry(pdbBlob, pdb.FormatVersion);
			}
			var peBuilder = new ManagedPEBuilder(
				header: header,
				metadataRootBuilder: rootBuilder,
				ilStream: _ilBlock,
				debugDirectoryBuilder: debugBuilder,
				nativeResources: _resWriter
				);
			ContextAnnotations.SetPEBuilder(Context, peBuilder);
			if (Context.Parameters.GenerateInMemory)
			{
				Context.GeneratedPEBuilder = peBuilder;
			}
		}

        private Stream GetIconFile(string filename)
		{
			if (!Path.IsPathRooted(filename))
			{
				filename = Path.GetFullPath(filename);
			}
			try
			{
				return File.Open(filename, FileMode.Open);
			}
			catch (FileNotFoundException)
			{
				Context.Warnings.Add(CompilerWarningFactory.IconNotFound(filename));
				return null;
			}
		}

		private void DefineUnmanagedResource()
		{
			// always pass true for noManifest and a null manifest stream for now.  This can be updated
			// later if we add support for manifests to Boo
			var filename = BuildOutputAssemblyName();
			var isExe = filename.EndsWith(".exe");
			var iconName = Context.Parameters.Icon;
			var iconStream = iconName != null ? GetIconFile(iconName) : null;
			var resourceBytes = UnamangedResourceHelper.CreateDefaultWin32Resources(
				true, isExe, null, iconStream, CompileUnit);
			_resWriter = new UnmanagedResourceWriter(resourceBytes);
		}

        private class UnmanagedResourceWriter : ResourceSectionBuilder
        {
			private readonly byte[] _data;

			public UnmanagedResourceWriter(byte[] data)
            {
				_data = data;
            }

            protected override void Serialize(BlobBuilder builder, SectionLocation location)
            {
                builder.WriteBytes(_data);
            }
        }

        void GatherAssemblyAttributes()
		{
			foreach (var module in CompileUnit.Modules)
				foreach (var attribute in module.AssemblyAttributes)
					_assemblyAttributes.Add(attribute);
		}

		void DefineTypes()
		{
			if (CompileUnit.Modules.Count == 0)
				return;

			var types = CollectTypes();
			foreach (var type in types)
				DefineType(type);

			foreach (var type in types)
			{
				DefineGenericParameters(type);
				DefineTypeMembers(type);
			}

			//foreach (var type in types)
			//	DefineTypeMembers(type);

			_moduleTypeOrder = types.ToLookup(t => t.GetAncestor<Module>());
			foreach (var module in CompileUnit.Modules)
				OnModule(module);

			EmitAttributes();
			CreateTypes(types);
		}

		sealed class AttributeEmitVisitor : FastDepthFirstVisitor
		{
			readonly EmitAssemblyCore _emitter;

			public AttributeEmitVisitor(EmitAssemblyCore emitter)
			{
				_emitter = emitter;
			}

			public override void OnField(Field node)
			{
				_emitter.EmitFieldAttributes(node);
			}

			public override void OnEnumMember(EnumMember node)
			{
				_emitter.EmitFieldAttributes(node);
			}

			public override void OnEvent(Event node)
			{
				_emitter.EmitEventAttributes(node);
			}

			public override void OnProperty(Property node)
			{
				Visit(node.Getter);
				Visit(node.Setter);
				_emitter.EmitPropertyAttributes(node);
			}

			public override void OnConstructor(Constructor node)
			{
				Visit(node.Parameters);
				_emitter.EmitMethodAttributes(node);
			}

			public override void OnMethod(Method node)
			{
				Visit(node.Parameters);
				_emitter.EmitMethodAttributes(node);
			}

			public override void OnParameterDeclaration(ParameterDeclaration node)
			{
				_emitter.EmitParameterAttributes(node);
			}

			public override void OnClassDefinition(ClassDefinition node)
			{
				base.OnClassDefinition(node);
				_emitter.EmitTypeAttributes(node);
			}

			public override void OnInterfaceDefinition(InterfaceDefinition node)
			{
				base.OnInterfaceDefinition(node);
				_emitter.EmitTypeAttributes(node);
			}

			public override void OnEnumDefinition(EnumDefinition node)
			{
				base.OnEnumDefinition(node);
				_emitter.EmitTypeAttributes(node);
			}
		}

		void EmitAttributes(INodeWithAttributes node, IBuilder builder)
		{
			foreach (Attribute attribute in node.Attributes)
			{
				var attr = GetCustomAttributeBuilder(attribute);
				_typeSystem.SetCustomAttribute(builder, attr);
			}
		}

        void EmitPropertyAttributes(Property node)
		{
			var builder = GetPropertyBuilder(node);
			EmitAttributes(node, builder);
		}

		void EmitParameterAttributes(ParameterDeclaration node)
		{
			var builder = (ParameterBuilder)GetBuilder(node);
			EmitAttributes(node, builder);
		}

		void EmitEventAttributes(Event node)
		{
			var builder = (EventBuilder)GetBuilder(node);
			EmitAttributes(node, builder);
		}

		void EmitMethodAttributes(Method node)
		{
			var builder = (MethodBuilder)GetBuilder(node);
			EmitAttributes(node, builder);
		}

		void EmitTypeAttributes(TypeDefinition node)
		{
			TypeBuilder handle = GetTypeBuilder(node);
			EmitAttributes(node, handle);
		}

		void EmitFieldAttributes(TypeMember node)
		{
			var builder = GetFieldBuilder(node);
			EmitAttributes(node, builder);
		}

		void EmitAttributes()
		{
			var visitor = new AttributeEmitVisitor(this);
			foreach (var module in CompileUnit.Modules)
				module.Accept(visitor);
		}

		void CreateTypes(List<TypeDefinition> types)
		{
			new TypeCreator(this, types).Run();
		}

		/// <summary>
		/// Ensures that all types are created in the correct order.
		/// </summary>
		sealed class TypeCreator
		{
			readonly EmitAssemblyCore _emitter;

			readonly Set<TypeDefinition> _created = new();

			readonly List<TypeDefinition> _types;

			TypeDefinition _current;

			public TypeCreator(EmitAssemblyCore emitter, List<TypeDefinition> types)
			{
				_emitter = emitter;
				_types = types;
			}

			public void Run()
			{
				CreateTypes();
			}

			private void CreateTypes()
			{
				foreach (var type in _types)
					CreateType(type);
			}

			void CreateType(TypeDefinition type)
			{
				if (_created.Contains(type))
					return;

				_created.Add(type);

				var saved = _current;
				_current = type;
				try
				{
					HandleTypeCreation(type);
				}
				catch (Exception e)
				{
					throw CompilerErrorFactory.InternalError(type, string.Format("Failed to create '{0}' type.", type), e);
				}
				_current = saved;
			}

			private void HandleTypeCreation(TypeDefinition type)
			{
				Trace("creating type '{0}'", type);

				if (IsNestedType(type))
					CreateOuterTypeOf(type);

				CreateRelatedTypes(type);
				Trace("type '{0}' successfully created", type);
			}

			private void CreateOuterTypeOf(TypeMember type)
			{
				CreateType(type.DeclaringType);
			}

			private void CreateRelatedTypes(TypeDefinition typedef)
			{
				CreateRelatedTypes(typedef.BaseTypes);
				foreach (var gpd in typedef.GenericParameters)
					CreateRelatedTypes(gpd.BaseTypes);
			}

			private void EnsureInternalFieldDependencies(TypeDefinition typedef)
			{
				foreach (var field in typedef.Members.OfType<Field>())
					EnsureInternalDependencies((IType)field.Type.Entity);
			}

			private void CreateRelatedTypes(IEnumerable<TypeReference> typerefs)
			{
				foreach (var typeref in typerefs)
				{
					var type = _emitter.GetType(typeref);
					EnsureInternalDependencies(type);
				}
			}

			private void EnsureInternalDependencies(IType type)
			{
				if (type is AbstractInternalType internalType)
				{
					CreateType(internalType.TypeDefinition);
					return;
				}

				if (type.ConstructedInfo != null)
				{
					EnsureInternalDependencies(type.ConstructedInfo.GenericDefinition);
					foreach (var typeArg in type.ConstructedInfo.GenericArguments)
						EnsureInternalDependencies(typeArg);
				}
			}

			static bool IsNestedType(TypeMember type) => 
				type.ParentNode.NodeType switch
				{
					NodeType.ClassDefinition or NodeType.InterfaceDefinition => true,
					_ => false,
				};

			void Trace(string format, params object[] args)
			{
				_emitter.Context.TraceVerbose(format, args);
			}
		}

		List<TypeDefinition> CollectTypes()
		{
			var types = new List<TypeDefinition>();
			foreach (Module module in CompileUnit.Modules)
				CollectTypes(types, module.Members);
			return types;
		}

		void CollectTypes(List<TypeDefinition> types, TypeMemberCollection members)
		{
			foreach (var member in members)
			{
				switch (member.NodeType)
				{
					case NodeType.InterfaceDefinition:
					case NodeType.ClassDefinition:
						{
							var typeDefinition = (TypeDefinition)member;
							types.Add(typeDefinition);
							CollectTypes(types, typeDefinition.Members);
							break;
						}
					case NodeType.EnumDefinition:
						{
							types.Add((TypeDefinition)member);
							break;
						}
				}
			}
		}

		public override void Dispose()
		{
			base.Dispose();

			_asmBuilder = new();
			_ilBlock = new();
			_symbolDocWriters.Clear();
			_types.Clear();
			_typeCache.Clear();
			_builders.Clear();
			_assemblyAttributes.Clear();
			_packedArrays.Clear();
			GC.SuppressFinalize(this);
		}

		public override void OnAttribute(Attribute node)
		{
		}

		public override void OnModule(Module module)
		{
			_perModuleRawArrayIndexing = AstAnnotations.IsRawIndexing(module);
			_checked = AstAnnotations.IsChecked(module, Parameters.Checked);
			foreach (var member in module.Members.Where(m => m.Entity.EntityType != EntityType.Type))
            {
				Visit(member);
            }
			if (_moduleTypeOrder.Contains(module))
            {
				foreach (var type in _moduleTypeOrder[module])
                {
					Visit(type);
                }
            }
		}

		private const FieldAttributes ENUM_ATTRIBUTES = 
			FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal;

		public override void OnEnumDefinition(EnumDefinition node)
		{
			var typeBuilder = GetTypeBuilder(node);
			typeBuilder.Build();
		}

		private object InitializerValueOf(EnumMember enumMember, EnumDefinition enumType)
		{
			var value = ((IntegerLiteralExpression)enumMember.Initializer).Value;
			var type = GetEnumUnderlyingType(enumType);
			if (type == TypeSystemServices.IntType)
			{
				return (int)value;
			}
			if (type == TypeSystemServices.UIntType)
			{
				return unchecked((uint)value);
			}
			if (type == TypeSystemServices.ShortType)
			{
				return unchecked((short)value);
			}
			if (type == TypeSystemServices.UShortType)
			{
				return (short)value;
			}
			if (type == TypeSystemServices.SByteType)
			{
				return (sbyte)value;
			}
			if (type == TypeSystemServices.ByteType)
			{
				return unchecked((byte)value);
			}
			if (type == TypeSystemServices.ULongType)
			{
				return unchecked((ulong)value);
			}
			return value;
		}

		public override void OnArrayTypeReference(ArrayTypeReference node)
		{
		}

		public override void OnClassDefinition(ClassDefinition node)
		{
			EmitTypeDefinition(node);
		}

		public override void OnInterfaceDefinition(InterfaceDefinition node)
		{
			TypeBuilder builder = GetTypeBuilder(node);
			foreach (TypeReference baseType in node.BaseTypes)
			{
				builder.AddInterfaceImplementation((IType)baseType.Entity);
			}
			builder.Build();
		}

		public override void OnMacroStatement(MacroStatement node)
		{
			NotImplemented(node, "Unexpected macro: " + node.ToCodeString());
		}

		public override void OnCallableDefinition(CallableDefinition node)
		{
			NotImplemented(node, "Unexpected callable definition!");
		}

		void EmitTypeDefinition(TypeDefinition node)
		{
			foreach (var member in node.Members.Where(m => m.Entity.EntityType != EntityType.Type))
			{
				Visit(member);
			}
			TypeBuilder current = GetTypeBuilder(node);
			current.Build();
		}

		private class ModuleConstructorFinder : FastDepthFirstVisitor
        {
			public bool Found { get; private set; }

            public override void OnModule(Module node)
            {
				if (!Found)
	                base.OnModule(node);
            }

            public override void OnClassDefinition(ClassDefinition node)
            {
				if (!Found)
					base.OnClassDefinition(node);
            }

            public override void OnStructDefinition(StructDefinition node)
            {
				if (!Found)
					base.OnStructDefinition(node);
            }

            public override void OnMethod(Method method)
            {
				if (method.Name.StartsWith("$module_ctor"))
				{
					Found = true;
				}
			}
		}

		public override void OnMethod(Method method)
		{
			if (method.IsRuntime) return;
			if (IsPInvoke(method)) return;

			var methodBuilder = GetMethodBuilder(method);
			DefineExplicitImplementationInfo(method);

			EmitMethod(method, methodBuilder);
			if (method.Name.StartsWith("$module_ctor"))
			{
				_moduleConstructorMethods.Add(method);
			}
		}

		private void DefineExplicitImplementationInfo(Method method)
		{
			if (method.ExplicitInfo == null)
				return;

			IMethod ifaceMethod = (IMethod)method.ExplicitInfo.Entity;
			var ifaceInfo = GetMethodInfo(ifaceMethod);
			var implInfo = GetMethodInfo((IMethod)method.Entity);

			TypeBuilder builder = GetTypeBuilder(method.DeclaringType);
			builder.AddMethodImplementation(ifaceInfo, implInfo);
		}

		void EmitMethod(Method method, MethodBuilder builder)
		{
			_il = builder;
			_method = method;

			DefineLabels(method);
			Visit(method.Locals);

			BeginMethodBody(GetEntity(method).ReturnType);
			Visit(method.Body);
			EndMethodBody(method);
		}

		void BeginMethodBody(IType returnType)
		{
			_returnType = returnType;
			_returnStatements = 0;
			_returnImplicit = IsVoid(returnType);
			_hasLeaveWithStoredValue = false;

			//we may not actually use (any/all of) them, but at least they're ready
			_returnLabel = _il.DefineLabel();
			_leaveLabel = _il.DefineLabel();
			_implicitLabel = _il.DefineLabel();
		}

		void EndMethodBody(Method method)
		{
			if (!_returnImplicit)
				_returnImplicit = !AstUtil.AllCodePathsReturnOrRaise(method.Body);

			//At most a method epilogue contains 3 independent load instructions:
			//1) load of the value of an actual return (emitted elsewhere and branched to _returnLabel)
			//2) load of a default value (implicit returns [e.g return without expression])
			//3) load of the `leave' stored value

			bool hasDefaultValueReturn = _returnImplicit && !IsVoid(_returnType);
			if (hasDefaultValueReturn)
			{
				if (_returnStatements == -1) //emit branch only if instructed to do so (-1)
					_il.Branch(ILOpCode.Br_s, _returnLabel);

				//load default return value for implicit return
				_il.MarkLabel(_implicitLabel);
				EmitDefaultValue(_returnType);
				PopType();
			}

			if (_hasLeaveWithStoredValue)
			{
				if (hasDefaultValueReturn || _returnStatements == -1)
					_il.Branch(ILOpCode.Br_s, _returnLabel);

				//load the stored return value and `ret'
				_il.MarkLabel(_leaveLabel);
				_il.LoadLocal(_il.GetDefaultValueHolder(_returnType));
			}

			if (_returnImplicit || _returnStatements != 0)
			{
				_il.MarkLabel(_returnLabel);
				_il.OpCode(ILOpCode.Ret);
			}
		}

		private bool IsPInvoke(Method method)
		{
			return GetEntity(method).IsPInvoke;
		}

		public override void OnBlock(Block block)
		{
			var currentChecked = _checked;
			_checked = AstAnnotations.IsChecked(block, currentChecked);

			var currentArrayIndexing = _rawArrayIndexing;
			_rawArrayIndexing = _perModuleRawArrayIndexing || AstAnnotations.IsRawIndexing(block);

			Visit(block.Statements);

			_rawArrayIndexing = currentArrayIndexing;
			_checked = currentChecked;
		}

		void DefineLabels(Method method)
		{
			foreach (var label in LabelsOn(method))
				label.LabelHandle = _il.DefineLabel();
		}

		private static InternalLabel[] LabelsOn(Method method)
		{
			return ((InternalMethod)method.Entity).Labels;
		}

		public override void OnConstructor(Constructor constructor)
		{
			if (constructor.IsRuntime) return;

			var builder = GetConstructorBuilder(constructor);
			EmitMethod(constructor, builder);
		}

		public override void OnLocal(Local local)
		{
			InternalLocal info = GetInternalLocal(local);
			info.LocalVariableIndex = AddLocal(info);
		}

		private int AddLocal(InternalLocal local) => _il.AddLocal(local.Type, local.Name);

		public override void OnForStatement(ForStatement node)
		{
			NotImplemented("ForStatement");
		}

		public override void OnReturnStatement(ReturnStatement node)
		{
			EmitDebugInfo(node);

			var retOpCode = _tryBlock > 0 ? ILOpCode.Leave : ILOpCode.Br;
			var label = _returnLabel;

			var expression = node.Expression;
			if (expression != null)
			{
				++_returnStatements;

				LoadExpressionWithType(_returnType, expression);

				if (retOpCode == ILOpCode.Leave)
				{
					//`leave' clears the stack, so we have to store return value temporarily
					//we can use a default value holder for that since it won't be read afterwards
					//of course this is necessary only if return type is not void
					var temp = _il.GetDefaultValueHolder(_returnType);
					_il.StoreLocal(temp);
					label = _leaveLabel;
					_hasLeaveWithStoredValue = true;
				}
			}
			else if (_returnType != TypeSystemServices.VoidType)
			{
				_returnImplicit = true;
				label = _implicitLabel;
			}

			if (_method.Body.LastStatement != node)
				_il.Branch(retOpCode, label);
			else if (expression != null)
				_returnStatements = -1; //instruct epilogue to branch last ret only if necessary
		}

		private void LoadExpressionWithType(IType expectedType, Expression expression)
		{
			Visit(expression);
			EmitCastIfNeeded(expectedType, PopType());
		}

		public override void OnRaiseStatement(RaiseStatement node)
		{
			EmitDebugInfo(node);
			if (node.Exception == null)
			{
				_il.OpCode(ILOpCode.Rethrow);
			}
			else
			{
				Visit(node.Exception); PopType();
				_il.OpCode(ILOpCode.Throw);
			}
		}

		public override void OnTryStatement(TryStatement node)
		{
			++_tryBlock;
			_il.BeginTry();

			Visit(node.ProtectedBlock);
			_il.EndTryBody();
			Visit(node.ExceptionHandlers);

			if (null != node.FailureBlock)
			{
				_il.BeginFail();
				Visit(node.FailureBlock);
				_il.EndFail();
			}

			if (null != node.EnsureBlock)
			{
				_il.BeginFinally();
				Visit(node.EnsureBlock);
				_il.EndFinally();
			}
			--_tryBlock;
			_il.EndTry();
		}

		public override void OnExceptionHandler(ExceptionHandler node)
		{
			var isFilter = node.Flags.HasFlag(ExceptionHandlerFlags.Filter);
			if (isFilter)
			{
				OnFilterExceptionHandler(node);
				_il.BeginFilterBody();
			}
			else
			{
				_il.BeginCatchBlock(GetSystemType(node.Declaration.Type));
				// Clean up the stack or store the exception if not anonymous.
				EmitStoreOrPopException(node);
			}

			Visit(node.Declaration);
			Visit(node.Block);
			if (isFilter)
			{
				_il.EndFilter();
			}
			else
			{
				_il.EndCatchBlock();
			}
		}

		private void OnFilterExceptionHandler(ExceptionHandler node)
		{
			_il.BeginFilter();
			LabelHandle endLabel = _il.DefineLabel();

			// If the filter is not untyped, then test the exception type
			// before testing the filter condition
			if ((node.Flags & ExceptionHandlerFlags.Untyped) == ExceptionHandlerFlags.None)
			{
				LabelHandle filterCondition = _il.DefineLabel();

				// Test the type of the exception.
				_il.OpCode(ILOpCode.Isinst, GetSystemType(node.Declaration.Type));

				// Duplicate it.  If it is null, then it will be used to
				// skip the filter.
				Dup();

				// If the exception is of the right type, branch
				// to test the filter condition.
				_il.Branch(ILOpCode.Brtrue_s, filterCondition);

				// Otherwise, clean up the stack and prepare the stack
				// to skip the filter.
				EmitStoreOrPopException(node);

				_il.OpCode(ILOpCode.Ldc_i4_0);
				_il.Branch(ILOpCode.Br, endLabel);
				_il.MarkLabel(filterCondition);
			}
			else if ((node.Flags & ExceptionHandlerFlags.Anonymous) == ExceptionHandlerFlags.None)
			{
				// Cast the exception to the default except type
				_il.OpCode(ILOpCode.Isinst, GetSystemType(node.Declaration.Type));
			}

			EmitStoreOrPopException(node);

			// Test the condition and convert to boolean if needed.
			node.FilterCondition.Accept(this);
			PopType();
			EmitToBoolIfNeeded(node.FilterCondition);

			// If the type is right and the condition is true,
			// proceed with the handler.
			_il.MarkLabel(endLabel);
			_il.OpCode(ILOpCode.Ldc_i4_0);
			_il.OpCode(ILOpCode.Cgt_un);
		}

		private void EmitStoreOrPopException(ExceptionHandler node)
		{
			if ((node.Flags & ExceptionHandlerFlags.Anonymous) == ExceptionHandlerFlags.None)
			{
				_il.StoreLocal(GetLocalVariablePosition(node.Declaration));
			}
			else
			{
				_il.OpCode(ILOpCode.Pop);
			}
		}

		public override void OnUnpackStatement(UnpackStatement node)
		{
			NotImplemented("Unpacking");
		}

		public override void OnExpressionStatement(ExpressionStatement node)
		{
			EmitDebugInfo(node);

			base.OnExpressionStatement(node);

			// if the type of the inner expression is not
			// void we need to pop its return value to leave
			// the stack sane
			DiscardValueOnStack();
		}

		void DiscardValueOnStack()
		{
			if (!IsVoid(PopType()))
				_il.OpCode(ILOpCode.Pop);
		}

		bool IsVoid(IType type)
		{
			return type == TypeSystemServices.VoidType;
		}

		public override void OnUnlessStatement(UnlessStatement node)
		{
			LabelHandle endLabel = _il.DefineLabel();
			EmitDebugInfo(node);
			EmitBranchTrue(node.Condition, endLabel);
			node.Block.Accept(this);
			_il.MarkLabel(endLabel);
		}

		void OnSwitch(MethodInvocationExpression node)
		{
			var args = node.Arguments;
			LoadExpressionWithType(TypeSystemServices.IntType, args[0]);
			_il.Switch(args.Skip(1).Select(e => LabelFor(e)).ToArray());
			PushVoid();
		}

		private static LabelHandle LabelFor(Expression expression)
		{
			return ((InternalLabel)expression.Entity).LabelHandle;
		}

		public override void OnGotoStatement(GotoStatement node)
		{
			EmitDebugInfo(node);

			InternalLabel label = (InternalLabel)GetEntity(node.Label);
			int gotoDepth = AstAnnotations.GetTryBlockDepth(node);
			int targetDepth = AstAnnotations.GetTryBlockDepth(label.LabelStatement);

			if (targetDepth == gotoDepth)
			{
				_il.Branch(ILOpCode.Br, label.LabelHandle);
			}
			else
			{
				_il.Branch(ILOpCode.Leave, label.LabelHandle);
			}
		}

		public override void OnLabelStatement(LabelStatement node)
		{
			EmitDebugInfo(node);
			_il.MarkLabel(((InternalLabel)node.Entity).LabelHandle);
		}

		public override void OnConditionalExpression(ConditionalExpression node)
		{
			var type = GetExpressionType(node);

			var endLabel = _il.DefineLabel();

			EmitBranchFalse(node.Condition, endLabel);
			LoadExpressionWithType(type, node.TrueValue);

			var elseEndLabel = _il.DefineLabel();
			_il.Branch(ILOpCode.Br, elseEndLabel);
			_il.MarkLabel(endLabel);

			endLabel = elseEndLabel;
			LoadExpressionWithType(type, node.FalseValue);

			_il.MarkLabel(endLabel);

			PushType(type);
		}

		public override void OnIfStatement(IfStatement node)
		{
			LabelHandle endLabel = _il.DefineLabel();

			EmitDebugInfo(node);
			EmitBranchFalse(node.Condition, endLabel);

			node.TrueBlock.Accept(this);
			if (node.FalseBlock != null)
			{
				LabelHandle elseEndLabel = _il.DefineLabel();
				if (!node.TrueBlock.EndsWith<ReturnStatement>() && !node.TrueBlock.EndsWith<RaiseStatement>())
					_il.Branch(ILOpCode.Br, elseEndLabel);
				_il.MarkLabel(endLabel);

				endLabel = elseEndLabel;
				node.FalseBlock.Accept(this);
			}

			_il.MarkLabel(endLabel);
		}


		void EmitBranchTrue(Expression expression, LabelHandle label)
		{
			EmitBranch(true, expression, label);
		}

		void EmitBranchFalse(Expression expression, LabelHandle label)
		{
			EmitBranch(false, expression, label);
		}

		void EmitBranch(bool branchOnTrue, BinaryExpression expression, LabelHandle label)
		{
			switch (expression.Operator)
			{
				case BinaryOperatorType.TypeTest:
					EmitTypeTest(expression);
					_il.Branch(branchOnTrue ? ILOpCode.Brtrue : ILOpCode.Brfalse, label);
					break;

				case BinaryOperatorType.Or:
					if (branchOnTrue)
					{
						EmitBranch(true, expression.Left, label);
						EmitBranch(true, expression.Right, label);
					}
					else
					{
						LabelHandle skipRhs = _il.DefineLabel();
						EmitBranch(true, expression.Left, skipRhs);
						EmitBranch(false, expression.Right, label);
						_il.MarkLabel(skipRhs);
					}
					break;

				case BinaryOperatorType.And:
					if (branchOnTrue)
					{
						LabelHandle skipRhs = _il.DefineLabel();
						EmitBranch(false, expression.Left, skipRhs);
						EmitBranch(true, expression.Right, label);
						_il.MarkLabel(skipRhs);
					}
					else
					{
						EmitBranch(false, expression.Left, label);
						EmitBranch(false, expression.Right, label);
					}
					break;

				case BinaryOperatorType.Equality:
					if (IsZeroEquivalent(expression.Left))
						EmitBranch(!branchOnTrue, expression.Right, label);
					else if (IsZeroEquivalent(expression.Right))
						EmitBranch(!branchOnTrue, expression.Left, label);
					else
					{
						LoadCmpOperands(expression);
						_il.Branch(branchOnTrue ? ILOpCode.Beq : ILOpCode.Bne_un, label);
					}
					break;

				case BinaryOperatorType.Inequality:
					if (IsZeroEquivalent(expression.Left))
					{
						EmitBranch(branchOnTrue, expression.Right, label);
					}
					else if (IsZeroEquivalent(expression.Right))
					{
						EmitBranch(branchOnTrue, expression.Left, label);
					}
					else
					{
						LoadCmpOperands(expression);
						_il.Branch(branchOnTrue ? ILOpCode.Bne_un : ILOpCode.Beq, label);
					}
					break;

				case BinaryOperatorType.ReferenceEquality:
					if (IsNull(expression.Left))
					{
						EmitRawBranch(!branchOnTrue, expression.Right, label);
						break;
					}
					if (IsNull(expression.Right))
					{
						EmitRawBranch(!branchOnTrue, expression.Left, label);
						break;
					}
					Visit(expression.Left); PopType();
					Visit(expression.Right); PopType();
					_il.Branch(branchOnTrue ? ILOpCode.Beq : ILOpCode.Bne_un, label);
					break;

				case BinaryOperatorType.ReferenceInequality:
					if (IsNull(expression.Left))
					{
						EmitRawBranch(branchOnTrue, expression.Right, label);
						break;
					}
					if (IsNull(expression.Right))
					{
						EmitRawBranch(branchOnTrue, expression.Left, label);
						break;
					}
					Visit(expression.Left); PopType();
					Visit(expression.Right); PopType();
					_il.Branch(branchOnTrue ? ILOpCode.Bne_un : ILOpCode.Beq, label);
					break;

				case BinaryOperatorType.GreaterThan:
					LoadCmpOperands(expression);
					_il.Branch(branchOnTrue ? ILOpCode.Bgt : ILOpCode.Ble, label);
					break;

				case BinaryOperatorType.GreaterThanOrEqual:
					LoadCmpOperands(expression);
					_il.Branch(branchOnTrue ? ILOpCode.Bge : ILOpCode.Blt, label);
					break;

				case BinaryOperatorType.LessThan:
					LoadCmpOperands(expression);
					_il.Branch(branchOnTrue ? ILOpCode.Blt : ILOpCode.Bge, label);
					break;

				case BinaryOperatorType.LessThanOrEqual:
					LoadCmpOperands(expression);
					_il.Branch(branchOnTrue ? ILOpCode.Ble : ILOpCode.Bgt, label);
					break;

				default:
					EmitDefaultBranch(branchOnTrue, expression, label);
					break;
			}
		}

		void EmitBranch(bool branchOnTrue, UnaryExpression expression, LabelHandle label)
		{
			if (UnaryOperatorType.LogicalNot == expression.Operator)
			{
				EmitBranch(!branchOnTrue, expression.Operand, label);
			}
			else
			{
				EmitDefaultBranch(branchOnTrue, expression, label);
			}
		}

		void EmitBranch(bool branchOnTrue, Expression expression, LabelHandle label)
		{
			switch (expression.NodeType)
			{
				case NodeType.BinaryExpression:
					{
						EmitBranch(branchOnTrue, (BinaryExpression)expression, label);
						break;
					}

				case NodeType.UnaryExpression:
					{
						EmitBranch(branchOnTrue, (UnaryExpression)expression, label);
						break;
					}

				default:
					{
						EmitDefaultBranch(branchOnTrue, expression, label);
						break;
					}
			}
		}

		void EmitRawBranch(bool branch, Expression condition, LabelHandle label)
		{
			condition.Accept(this); PopType();
			_il.Branch(branch ? ILOpCode.Brtrue : ILOpCode.Brfalse, label);
		}

		void EmitDefaultBranch(bool branch, Expression condition, LabelHandle label)
		{
			if (branch && IsOneEquivalent(condition))
			{
				_il.Branch(ILOpCode.Br, label);
				return;
			}

			if (!branch && IsZeroEquivalent(condition))
			{
				_il.Branch(ILOpCode.Br, label);
				return;
			}

			condition.Accept(this);

			var type = PopType();
			if (TypeSystemServices.IsFloatingPointNumber(type))
			{
				EmitDefaultValue(type);
				_il.Branch(branch ? ILOpCode.Bne_un : ILOpCode.Beq, label);
				return;
			}

			EmitToBoolIfNeeded(condition);
			_il.Branch(branch ? ILOpCode.Brtrue : ILOpCode.Brfalse, label);
		}

		private static bool IsZeroEquivalent(Expression expression) =>
			IsNull(expression) || IsZero(expression) || IsFalse(expression);

		private static bool IsOneEquivalent(Expression expression) =>
			IsBooleanLiteral(expression, true) || IsNumberLiteral(expression, 1);

		private static bool IsNull(Expression expression) =>
			NodeType.NullLiteralExpression == expression.NodeType;

		private static bool IsFalse(Expression expression) =>
			IsBooleanLiteral(expression, false);

		private static bool IsBooleanLiteral(Expression expression, bool value) =>
			NodeType.BoolLiteralExpression == expression.NodeType
				&& (value == ((BoolLiteralExpression)expression).Value);

		private static bool IsZero(Expression expression) =>
			IsNumberLiteral(expression, 0);

		private static bool IsNumberLiteral(Expression expression, int value) =>
			(NodeType.IntegerLiteralExpression == expression.NodeType
				&& (value == ((IntegerLiteralExpression)expression).Value))
			||
			(NodeType.DoubleLiteralExpression == expression.NodeType
				&& (value == ((DoubleLiteralExpression)expression).Value));

		public override void OnBreakStatement(BreakStatement node) =>
			EmitGoTo(_currentLoopInfoCore.BreakLabel, node);

		private void EmitGoTo(LabelHandle label, Node debugInfo)
		{
			EmitDebugInfo(debugInfo);
			_il.Branch(InTryInLoop() ? ILOpCode.Leave : ILOpCode.Br, label);
		}

		public override void OnContinueStatement(ContinueStatement node) =>
			EmitGoTo(_currentLoopInfoCore.ContinueLabel, node);

		public override void OnWhileStatement(WhileStatement node)
		{
			LabelHandle endLabel = _il.DefineLabel();
			LabelHandle bodyLabel = _il.DefineLabel();
			LabelHandle conditionLabel = _il.DefineLabel();

			_il.Branch(ILOpCode.Br, conditionLabel);
			_il.MarkLabel(bodyLabel);

			EnterLoop(endLabel, conditionLabel);
			node.Block.Accept(this);
			LeaveLoop();

			_il.MarkLabel(conditionLabel);
			EmitDebugInfo(node);
			EmitBranchTrue(node.Condition, bodyLabel);
			Visit(node.OrBlock);
			Visit(node.ThenBlock);
			_il.MarkLabel(endLabel);
		}

		void EmitIntNot()
		{
			_il.OpCode(ILOpCode.Ldc_i4_0);
			_il.OpCode(ILOpCode.Ceq);
		}

		void EmitGenericNot()
		{
			// bool codification:
			// value_on_stack ? 0 : 1
			LabelHandle wasTrue = _il.DefineLabel();
			LabelHandle wasFalse = _il.DefineLabel();
			_il.Branch(ILOpCode.Brfalse_s, wasFalse);
			_il.OpCode(ILOpCode.Ldc_i4_0);
			_il.Branch(ILOpCode.Br_s, wasTrue);
			_il.MarkLabel(wasFalse);
			_il.OpCode(ILOpCode.Ldc_i4_1);
			_il.MarkLabel(wasTrue);
		}

		public override void OnUnaryExpression(UnaryExpression node)
		{
			switch (node.Operator)
			{
				case UnaryOperatorType.LogicalNot:
					{
						EmitLogicalNot(node);
						break;
					}

				case UnaryOperatorType.UnaryNegation:
					{
						EmitUnaryNegation(node);
						break;
					}

				case UnaryOperatorType.OnesComplement:
					{
						EmitOnesComplement(node);
						break;
					}

				case UnaryOperatorType.AddressOf:
					{
						EmitAddressOf(node);
						break;
					}

				case UnaryOperatorType.Indirection:
					{
						EmitIndirection(node);
						break;
					}

				default:
					{
						NotImplemented(node, "unary operator not supported");
						break;
					}
			}
		}

		IType _byAddress = null;
		bool IsByAddress(IType type)
		{
			return (_byAddress == type);
		}

		void EmitDefaultValue(IType type)
		{
			var isGenericParameter = GenericsServices.IsGenericParameter(type);

			if (!type.IsValueType && !isGenericParameter)
				_il.OpCode(ILOpCode.Ldnull);
			else if (type == TypeSystemServices.BoolType)
				_il.OpCode(ILOpCode.Ldc_i4_0);
			else if (TypeSystemServices.IsFloatingPointNumber(type))
				EmitLoadLiteral(type, 0.0);
			else if (TypeSystemServices.IsPrimitiveNumber(type) || type == TypeSystemServices.CharType)
				EmitLoadLiteral(type, 0);
			else if (isGenericParameter && TypeSystemServices.IsReferenceType(type))
			{
				_il.OpCode(ILOpCode.Ldnull);
				_il.OpCode(ILOpCode.Unbox_any, GetSystemType(type));
			}
			else //valuetype or valuetype/unconstrained generic parameter
			{
				//TODO: if MethodBody.InitLocals is false
				//_il.OpCode(ILOpCode.Ldloca, GetDefaultValueHolder(type));
				//_il.OpCode(ILOpCode.Initobj, GetSystemType(type));
				_il.LoadLocal(_il.GetDefaultValueHolder(type));
			}

			PushType(type);
		}

		private void EmitOnesComplement(UnaryExpression node)
		{
			node.Operand.Accept(this);
			_il.OpCode(ILOpCode.Not);
		}

		private void EmitLogicalNot(UnaryExpression node)
		{
			Expression operand = node.Operand;
			operand.Accept(this);
			IType typeOnStack = PopType();
			bool notContext = true;

			if (IsBoolOrInt(typeOnStack))
			{
				EmitIntNot();
			}
			else if (EmitToBoolIfNeeded(operand, ref notContext))
			{
				if (!notContext) //we are in a not context and emit to bool is also in a not context
					EmitIntNot();//so we do not need any not (false && false => true)
			}
			else
			{
				EmitGenericNot();
			}
			PushBool();
		}

		private void EmitUnaryNegation(UnaryExpression node)
		{
			var operandType = GetExpressionType(node.Operand);
			if (IsCheckedIntegerOperand(operandType))
			{
				_il.OpCode(ILOpCode.Ldc_i4_0);
				if (IsLong(operandType) || operandType == TypeSystemServices.ULongType)
					_il.OpCode(ILOpCode.Conv_i8);
				node.Operand.Accept(this);
				_il.OpCode(TypeSystemServices.IsSignedNumber(operandType)
							 ? ILOpCode.Sub_ovf
							 : ILOpCode.Sub_ovf_un);
				if (!IsLong(operandType) && operandType != TypeSystemServices.ULongType)
					EmitCastIfNeeded(operandType, TypeSystemServices.IntType);
			}
			else
			{
				//a single/double unary negation never overflow
				node.Operand.Accept(this);
				_il.OpCode(ILOpCode.Neg);
			}
		}

		private bool IsCheckedIntegerOperand(IType operandType)
		{
			return _checked && IsInteger(operandType);
		}

		void EmitAddressOf(UnaryExpression node)
		{
			_byAddress = GetExpressionType(node.Operand);
			node.Operand.Accept(this);
			PushType(PopType().MakePointerType());
			_byAddress = null;
		}

		void EmitIndirection(UnaryExpression node)
		{
			node.Operand.Accept(this);

			if (node.Operand.NodeType != NodeType.ReferenceExpression
				&& node.ParentNode.NodeType != NodeType.MemberReferenceExpression)
			{
				//pointer arithmetic, need to load the address
				IType et = PeekTypeOnStack().ElementType;
				ILOpCode code = GetLoadRefParamCode(et);
				_il.OpCode(code);
				if (code == ILOpCode.Ldobj)
				{
					_il.Token(GetSystemType(et));
				}
				PopType();
				PushType(et);
			}
		}

		static bool ShouldLeaveValueOnStack(Expression node)
		{
			return node.ParentNode.NodeType != NodeType.ExpressionStatement;
		}

		void OnReferenceComparison(BinaryExpression node)
		{
			node.Left.Accept(this); PopType();
			node.Right.Accept(this); PopType();
			_il.OpCode(ILOpCode.Ceq);
			if (BinaryOperatorType.ReferenceInequality == node.Operator)
			{
				EmitIntNot();
			}
			PushBool();
		}

		void OnAssignmentToSlice(BinaryExpression node)
		{
			var slice = (SlicingExpression)node.Left;
			Visit(slice.Target);

			var arrayType = (IArrayType)PopType();
			if (arrayType.Rank == 1)
				EmitAssignmentToSingleDimensionalArrayElement(arrayType, slice, node);
			else
				EmitAssignmentToMultiDimensionalArrayElement(arrayType, slice, node);
		}

		private void EmitAssignmentToMultiDimensionalArrayElement(IArrayType arrayType, SlicingExpression slice, BinaryExpression node)
		{
			var elementType = arrayType.ElementType;
			LoadArrayIndices(slice);
			var temp = LoadAssignmentOperand(elementType, node);
			CallArrayMethod(arrayType, "Set", TypeSystemServices.VoidType, ParameterTypesForArraySet(arrayType));
			FlushAssignmentOperand(elementType, temp);
		}

		private void EmitAssignmentToSingleDimensionalArrayElement(IArrayType arrayType, SlicingExpression slice, BinaryExpression node)
		{
			var elementType = arrayType.ElementType;

			var index = slice.Indices[0];
			EmitNormalizedArrayIndex(slice, index.Begin);

			var opcode = GetStoreEntityOpCode(elementType);
			bool stobj = IsStobj(opcode);
			if (stobj)
			{
				_il.OpCode(ILOpCode.Ldelema, GetSystemType(elementType));
			}

			var temp = LoadAssignmentOperand(elementType, node);

			_il.OpCode(opcode);
			if (stobj)
			{
				_il.Token(GetSystemType(elementType));
			}

			FlushAssignmentOperand(elementType, temp);
		}

		private void FlushAssignmentOperand(IType elementType, int? temp)
		{
			if (temp.HasValue)
				LoadLocal(temp.Value, elementType);
			else
				PushVoid();
		}

		private int? LoadAssignmentOperand(IType elementType, BinaryExpression node)
		{
			LoadExpressionWithType(elementType, node.Right);
			var leaveValueOnStack = ShouldLeaveValueOnStack(node);
			int? temp = default;
			if (leaveValueOnStack)
			{
				Dup();
				temp = StoreTempLocal(elementType);
			}
			return temp;
		}

		int _currentLocal;

		void LoadLocal(int slot, IType localType)
		{
			_il.LoadLocal(slot);

			PushType(localType);
			_currentLocal = slot;
		}

		void LoadLocal(InternalLocal local)
		{
			var idx = local.LocalVariableIndex;
			if (IsByAddress(local.Type))
			{
				_il.LoadLocalAddress(idx);
			}
			else
			{
				_il.LoadLocal(idx);
			}

			PushType(local.Type);
			_currentLocal = idx;
		}

		void LoadIndirectLocal(InternalLocal local)
		{
			LoadLocal(local);

			IType et = local.Type.ElementType;
			PopType();
			PushType(et);
			ILOpCode code = GetLoadRefParamCode(et);
			_il.OpCode(code);
			if (code == ILOpCode.Ldobj)
			{
				_il.Token(GetSystemType(et));
			}
		}

		private int StoreTempLocal(IType elementType)
		{
			var temp = _il.AddLocal(elementType);
			_il.StoreLocal(temp);
			return temp;
		}

		void OnAssignment(BinaryExpression node)
		{
			if (node.Left.NodeType == NodeType.SlicingExpression)
			{
				OnAssignmentToSlice(node);
				return;
			}

			// when the parent is not a statement we need to leave
			// the value on the stack
			bool leaveValueOnStack = ShouldLeaveValueOnStack(node);
			IEntity tag = TypeSystemServices.GetEntity(node.Left);
			switch (tag.EntityType)
			{
				case EntityType.Local:
					{
						SetLocal(node, (InternalLocal)tag, leaveValueOnStack);
						break;
					}

				case EntityType.Parameter:
					{
						InternalParameter param = (InternalParameter)tag;
						if (param.Parameter.IsByRef)
						{
							SetByRefParam(param, node.Right, leaveValueOnStack);
							break;
						}

						LoadExpressionWithType(param.Type, node.Right);

						if (leaveValueOnStack)
						{
							Dup();
							PushType(param.Type);
						}
						_il.StoreArgument(param.Index);
						break;
					}

				case EntityType.Field:
					{
						IField field = (IField)tag;
						SetField(node, field, node.Left, node.Right, leaveValueOnStack);
						break;
					}

				case EntityType.Property:
					{
						SetProperty((IProperty)tag, node.Left, node.Right, leaveValueOnStack);
						break;
					}

				case EntityType.Event: //event=null (always internal in this context)
					{
						InternalEvent e = (InternalEvent)tag;
						ILOpCode opcode = e.IsStatic ? ILOpCode.Stsfld : ILOpCode.Stfld;
						_il.OpCode(ILOpCode.Ldnull);
						_il.OpCode(opcode, GetFieldBuilder(e.BackingField.Field).Handle);
						break;
					}

				default:
					{
						NotImplemented(node, tag.ToString());
						break;
					}
			}
			if (!leaveValueOnStack)
			{
				PushVoid();
			}
		}

		private void SetByRefParam(InternalParameter param, Expression right, bool leaveValueOnStack)
		{
			if (!leaveValueOnStack && IsGenericDefaultInvocation(right))
			{
				LoadParam(param);
				_il.OpCode(ILOpCode.Initobj, GetSystemType(((MethodInvocationExpression)right).ExpressionType));
				return;
			}

			int temp = 0;
			IType tempType = null;
			if (leaveValueOnStack)
			{
				Visit(right);
				tempType = PopType();
				temp = StoreTempLocal(tempType);
			}

			LoadParam(param);
			if (tempType != null)
			{
				LoadLocal(temp, tempType);
				PopType();
			}
			else
				LoadExpressionWithType(param.Type, right);

			var storecode = GetStoreRefParamCode(param.Type);
			_il.OpCode(storecode);
			if (IsStobj(storecode)) //passing struct/decimal byref
			{
				_il.Token(GetSystemType(param.Type));
			}
			

			if (tempType != null)
				LoadLocal(temp, tempType);
		}

		private static bool IsGenericDefaultInvocation(Expression node)
		{
			if (node.NodeType != NodeType.MethodInvocationExpression)
				return false;
			var target = ((MethodInvocationExpression)node).Target;
			if (target.Entity != BuiltinFunction.Default)
				return false;
			return node.ExpressionType is InternalGenericParameter;
		}

		void EmitTypeTest(BinaryExpression node)
		{
			Visit(node.Left);
			IType actualType = PopType();

			EmitBoxIfNeeded(TypeSystemServices.ObjectType, actualType);

			var type = node.Right.NodeType == NodeType.TypeofExpression
				? GetSystemType(((TypeofExpression)node.Right).Type)
				: GetSystemType(node.Right);

			_il.OpCode(ILOpCode.Isinst, type);
		}

		void OnTypeTest(BinaryExpression node)
		{
			EmitTypeTest(node);

			_il.OpCode(ILOpCode.Ldnull);
			_il.OpCode(ILOpCode.Cgt_un);

			PushBool();
		}

		void LoadCmpOperands(BinaryExpression node)
		{
			var lhs = node.Left.ExpressionType;
			var rhs = node.Right.ExpressionType;
			if (lhs != rhs)
			{
				var type = TypeSystemServices.GetPromotedNumberType(lhs, rhs);
				LoadExpressionWithType(type, node.Left);
				LoadExpressionWithType(type, node.Right);
			}
			else //no need for conversion
			{
				Visit(node.Left);
				PopType();
				Visit(node.Right);
				PopType();
			}
		}

		void OnEquality(BinaryExpression node)
		{
			LoadCmpOperands(node);
			_il.OpCode(ILOpCode.Ceq);
			PushBool();
		}

		void OnInequality(BinaryExpression node)
		{
			LoadCmpOperands(node);
			_il.OpCode(ILOpCode.Ceq);
			EmitIntNot();
			PushBool();
		}

		void OnGreaterThan(BinaryExpression node)
		{
			LoadCmpOperands(node);
			_il.OpCode(ILOpCode.Cgt);
			PushBool();
		}

		void OnGreaterThanOrEqual(BinaryExpression node)
		{
			OnLessThan(node);
			EmitIntNot();
		}

		void OnLessThan(BinaryExpression node)
		{
			LoadCmpOperands(node);
			_il.OpCode(ILOpCode.Clt);
			PushBool();
		}

		void OnLessThanOrEqual(BinaryExpression node)
		{
			OnGreaterThan(node);
			EmitIntNot();
		}

		void OnExponentiation(BinaryExpression node)
		{
			var doubleType = TypeSystemServices.DoubleType;
			LoadOperandsWithType(doubleType, node);
			Call(_mathPow);
			PushType(doubleType);
		}

		private void LoadOperandsWithType(IType type, BinaryExpression node)
		{
			LoadExpressionWithType(type, node.Left);
			LoadExpressionWithType(type, node.Right);
		}

		void OnArithmeticOperator(BinaryExpression node)
		{
			var type = node.ExpressionType;
			LoadOperandsWithType(type, node);
			_il.OpCode(GetArithmeticOpCode(type, node.Operator));
			PushType(type);
		}

		bool EmitToBoolIfNeeded(Expression expression)
		{
			bool notContext = false;
			return EmitToBoolIfNeeded(expression, ref notContext);
		}

		//mostly used for logical operators and ducky mode
		//other cases of bool context are handled by PMB
		bool EmitToBoolIfNeeded(Expression expression, ref bool notContext)
		{
			bool inNotContext = notContext;
			notContext = false;

			//use a builtin conversion operator just for the logical operator trueness test
			IType type = GetExpressionType(expression);
			if (TypeSystemServices.ObjectType == type || TypeSystemServices.DuckType == type)
			{
				Call(_runtimeServicesToBoolObject);
				return true;
			}
			else if (TypeSystemServices.IsNullable(type))
			{
				_il.LoadLocalAddress(_currentLocal);
				var sType = TypeSystemServices.GetNullableUnderlyingType(type);
				Call(GetNullableHasValue(sType));
				int hasValue = StoreTempLocal(TypeSystemServices.BoolType);
				_il.OpCode(ILOpCode.Pop); //pop nullable address (ldloca)
				_il.LoadLocal(hasValue);
				return true;
			}
			else if (TypeSystemServices.StringType == type)
			{
				Call(_stringIsNullOrEmpty);
				if (!inNotContext)
					EmitIntNot(); //reverse result (true for not empty)
				else
					notContext = true;
				return true;
			}
			else if (IsInteger(type))
			{
				if (IsLong(type) || TypeSystemServices.ULongType == type)
					_il.OpCode(ILOpCode.Conv_i4);
				return true;
			}
			else if (TypeSystemServices.SingleType == type)
			{
				EmitDefaultValue(TypeSystemServices.SingleType);
				_il.OpCode(ILOpCode.Ceq);
				if (!inNotContext)
					EmitIntNot();
				else
					notContext = true;
				return true;
			}
			else if (TypeSystemServices.DoubleType == type)
			{
				EmitDefaultValue(TypeSystemServices.DoubleType);
				_il.OpCode(ILOpCode.Ceq);
				if (!inNotContext)
					EmitIntNot();
				else
					notContext = true;
				return true;
			}
			else if (TypeSystemServices.DecimalType == type)
			{
				Call(_runtimeServicesToBoolDecimal);
				return true;
			}
			else if (!type.IsValueType)
			{
				if (null == expression.GetAncestor<BinaryExpression>()
					&& null != expression.GetAncestor<IfStatement>())
					return true; //use br(true|false) directly (most common case)

				_il.OpCode(ILOpCode.Ldnull);
				if (!inNotContext)
				{
					_il.OpCode(ILOpCode.Cgt_un);
				}
				else
				{
					_il.OpCode(ILOpCode.Ceq);
					notContext = true;
				}
				return true;
			}
			return false;
		}

		void EmitAnd(BinaryExpression node)
		{
			EmitLogicalOperator(node, ILOpCode.Brtrue, ILOpCode.Brfalse);
		}

		void EmitOr(BinaryExpression node)
		{
			EmitLogicalOperator(node, ILOpCode.Brfalse, ILOpCode.Brtrue);
		}

		void EmitLogicalOperator(BinaryExpression node, ILOpCode brForValueType, ILOpCode brForRefType)
		{
			var type = GetExpressionType(node);
			Visit(node.Left);

			var lhsType = PopType();

			if (lhsType != null && lhsType.IsValueType && !type.IsValueType)
			{
				// if boxing, first evaluate the value
				// as it is and then box it...
				LabelHandle evalRhs = _il.DefineLabel();
				LabelHandle end = _il.DefineLabel();

				Dup();
				EmitToBoolIfNeeded(node.Left);  // may need to convert decimal to bool
				_il.Branch(brForValueType, evalRhs);
				EmitCastIfNeeded(type, lhsType);
				_il.Branch(ILOpCode.Br_s, end);

				_il.MarkLabel(evalRhs);
				_il.OpCode(ILOpCode.Pop);
				LoadExpressionWithType(type, node.Right);

				_il.MarkLabel(end);
			}
			else
			{
				LabelHandle end = _il.DefineLabel();

				EmitCastIfNeeded(type, lhsType);
				Dup();

				EmitToBoolIfNeeded(node.Left);

				_il.Branch(brForRefType, end);

				_il.OpCode(ILOpCode.Pop);
				LoadExpressionWithType(type, node.Right);
				_il.MarkLabel(end);
			}

			PushType(type);
		}

		IType GetExpectedTypeForBitwiseRightOperand(BinaryExpression node) =>
			node.Operator switch
			{
				BinaryOperatorType.ShiftLeft or BinaryOperatorType.ShiftRight => TypeSystemServices.IntType,
				_ => GetExpressionType(node),
			};

		void EmitBitwiseOperator(BinaryExpression node)
		{
			var type = node.ExpressionType;
			LoadExpressionWithType(type, node.Left);
			LoadExpressionWithType(GetExpectedTypeForBitwiseRightOperand(node), node.Right);

			switch (node.Operator)
			{
				case BinaryOperatorType.BitwiseOr:
					{
						_il.OpCode(ILOpCode.Or);
						break;
					}

				case BinaryOperatorType.BitwiseAnd:
					{
						_il.OpCode(ILOpCode.And);
						break;
					}

				case BinaryOperatorType.ExclusiveOr:
					{
						_il.OpCode(ILOpCode.Xor);
						break;
					}

				case BinaryOperatorType.ShiftLeft:
					{
						_il.OpCode(ILOpCode.Shl);
						break;
					}
				case BinaryOperatorType.ShiftRight:
					{
						_il.OpCode(TypeSystemServices.IsSignedNumber(type) ? ILOpCode.Shr : ILOpCode.Shr_un);
						break;
					}
			}

			PushType(type);
		}

		public override void OnBinaryExpression(BinaryExpression node)
		{
			switch (node.Operator)
			{
				case BinaryOperatorType.ShiftLeft:
				case BinaryOperatorType.ShiftRight:
				case BinaryOperatorType.ExclusiveOr:
				case BinaryOperatorType.BitwiseAnd:
				case BinaryOperatorType.BitwiseOr:
					{
						EmitBitwiseOperator(node);
						break;
					}

				case BinaryOperatorType.Or:
					{
						EmitOr(node);
						break;
					}

				case BinaryOperatorType.And:
					{
						EmitAnd(node);
						break;
					}

				case BinaryOperatorType.Addition:
				case BinaryOperatorType.Subtraction:
				case BinaryOperatorType.Multiply:
				case BinaryOperatorType.Division:
				case BinaryOperatorType.Modulus:
					{
						OnArithmeticOperator(node);
						break;
					}

				case BinaryOperatorType.Exponentiation:
					{
						OnExponentiation(node);
						break;
					}

				case BinaryOperatorType.Assign:
					{
						OnAssignment(node);
						break;
					}

				case BinaryOperatorType.Equality:
					{
						OnEquality(node);
						break;
					}

				case BinaryOperatorType.Inequality:
					{
						OnInequality(node);
						break;
					}

				case BinaryOperatorType.GreaterThan:
					{
						OnGreaterThan(node);
						break;
					}

				case BinaryOperatorType.LessThan:
					{
						OnLessThan(node);
						break;
					}

				case BinaryOperatorType.GreaterThanOrEqual:
					{
						OnGreaterThanOrEqual(node);
						break;
					}

				case BinaryOperatorType.LessThanOrEqual:
					{
						OnLessThanOrEqual(node);
						break;
					}

				case BinaryOperatorType.ReferenceInequality:
					{
						OnReferenceComparison(node);
						break;
					}

				case BinaryOperatorType.ReferenceEquality:
					{
						OnReferenceComparison(node);
						break;
					}

				case BinaryOperatorType.TypeTest:
					{
						OnTypeTest(node);
						break;
					}

				default:
					{
						OperatorNotImplemented(node);
						break;
					}
			}
		}

		void OperatorNotImplemented(BinaryExpression node)
		{
			NotImplemented(node, node.Operator.ToString());
		}

		public override void OnTypeofExpression(TypeofExpression node)
		{
			EmitGetTypeFromHandle(GetSystemType(node.Type));
		}

		public override void OnCastExpression(CastExpression node)
		{
			var type = GetType(node.Type);
			LoadExpressionWithType(type, node.Target);
			PushType(type);
		}

		public override void OnTryCastExpression(TryCastExpression node)
		{
			var type = GetSystemType(node.Type);
			node.Target.Accept(this); PopType();
			Isinst(type);
			PushType(node.ExpressionType);
		}

		private void Isinst(EntityHandle type)
		{
			_il.OpCode(ILOpCode.Isinst, type);
		}

		void InvokeMethod(IMethod method, MethodInvocationExpression node)
		{
			if (!InvokeOptimizedMethod(method, node))
				InvokeRegularMethod(method, node);
		}

		bool InvokeOptimizedMethod(IMethod method, MethodInvocationExpression node)
		{
			if (method == _arrayGetLength)
			{
				// don't use ldlen for System.Array
				var target = (node.Target as MemberReferenceExpression)?.Target;
				if (target == null || target.Entity == null)
                {
					target = node.Target;
                }
				if (!GetType(target).IsArray)
					return false;

				// optimize constructs such as:
				//		len(anArray)
				//		anArray.Length
				Visit(node.Target);
				PopType();
				_il.OpCode(ILOpCode.Ldlen);
				_il.OpCode(ILOpCode.Conv_i4);
				PushType(TypeSystemServices.IntType);
				return true;
			}

			if (method.DeclaringType != TypeSystemServices.BuiltinsType)
				return false;

			if (method.ConstructedInfo != null)
			{
				if (method.ConstructedInfo.GenericDefinition == _builtinsArrayGenericConstructor)
				{
					// optimize constructs such as:
					//		array[of int](2)
					IType type = method.ConstructedInfo.GenericArguments[0];
					EmitNewArray(type, node.Arguments[0]);
					return true;
				}

				if (method.Name == "matrix")
				{
					EmitNewMatrix(node);
					return true;
				}
				return false;
			}

			if (method == _builtinsArrayTypedConstructor)
			{
				// optimize constructs such as:
				//		array(int, 2)
				IType type = TypeSystemServices.GetReferencedType(node.Arguments[0]);
				if (null != type)
				{
					EmitNewArray(type, node.Arguments[1]);
					return true;
				}
			}
			else if (method == _builtinsArrayTypedCollectionConstructor)
			{
				// optimize constructs such as:
				//		array(int, (1, 2, 3))
				//		array(byte, [1, 2, 3, 4])
				IType type = TypeSystemServices.GetReferencedType(node.Arguments[0]);
				if (type != null)
				{
					if (node.Arguments[1] is ListLiteralExpression items)
					{
						EmitArray(type, items.Items);
						PushType(type.MakeArrayType(1));
						return true;
					}
				}
			}
			return false;
		}

		private void EmitNewMatrix(MethodInvocationExpression node)
		{
			var expressionType = GetExpressionType(node);
			var matrixType = GetSystemType(expressionType);
			var elementType = GetSystemType(expressionType.ElementType);

			// matrix of type(dimensions)
			EmitGetTypeFromHandle(elementType);
			PopType();

			EmitArray(TypeSystemServices.IntType, node.Arguments);

			Call(Array_CreateInstance);
			Castclass(matrixType);
			PushType(expressionType);
		}

		IMethod Array_CreateInstance
		{
			get
			{
				if (_Builtins_TypedMatrixConstructor == null)
					_Builtins_TypedMatrixConstructor = TypeSystemServices.Map(Types.Array.GetMethod("CreateInstance", new Type[] { Types.Type, typeof(int[]) }));
				return _Builtins_TypedMatrixConstructor;
			}
		}

		private IMethod _Builtins_TypedMatrixConstructor;

		void EmitNewArray(IType type, Expression length)
		{
			LoadIntExpression(length);
			_il.OpCode(ILOpCode.Newarr, GetSystemType(type));
			PushType(type.MakeArrayType(1));
		}

		void InvokeRegularMethod(IMethod method, MethodInvocationExpression node)
		{
			method = SelfMapMethodIfNeeded(method);
			// Do not emit call if conditional attributes (if any) do not match defined symbols
			if (!CheckConditionalAttributes(method))
			{
				EmitNop();
				PushType(method.ReturnType); // keep a valid state
				return;
			}

			IType targetType = null;
			Expression target = null;
			if (!method.IsStatic)
			{
				target = GetTargetObject(node);
				targetType = target.ExpressionType;
				PushTargetObjectFor(method, target, targetType);
			}

			PushArguments(method, node.Arguments);

			// Emit a constrained call if target is a generic parameter
			if (targetType != null && targetType is IGenericParameter)
			{
				_il.OpCode(ILOpCode.Constrained, GetSystemType(targetType));
			}

			_il.Call(GetCallOpCode(target, method), method);

			PushType(method.ReturnType);
		}

        //returns true if no conditional attribute match the defined symbols
        //else return false (which means the method won't get emitted)
        private bool CheckConditionalAttributes(IMethod method)
		{
			foreach (string conditionalSymbol in GetConditionalSymbols(method))
				if (!Parameters.Defines.ContainsKey(conditionalSymbol))
				{
					Context.TraceInfo("call to method '{0}' not emitted because the symbol '{1}' is not defined.", method, conditionalSymbol);
					return false;
				}
			return true;
		}

		private IEnumerable<string> GetConditionalSymbols(IMethod method)
		{
			if (method is GenericMappedMethod mappedMethod)
				return GetConditionalSymbols(mappedMethod.SourceMember);

			if (method is GenericConstructedMethod constructedMethod)
				return GetConditionalSymbols(constructedMethod.GenericDefinition);

			if (method is ExternalMethod externalMethod)
				return GetConditionalSymbols(externalMethod);

			if (method is InternalMethod internalMethod)
				return GetConditionalSymbols(internalMethod);

			return NoSymbols;
		}

		private static readonly string[] NoSymbols = Array.Empty<string>();

		private static IEnumerable<string> GetConditionalSymbols(ExternalMethod method)
		{
			foreach (ConditionalAttribute attr in method.MethodInfo.GetCustomAttributes(typeof(ConditionalAttribute), false))
				yield return attr.ConditionString;
		}

		private IEnumerable<string> GetConditionalSymbols(InternalMethod method)
		{
			foreach (var attr in MetadataUtil.GetCustomAttributes(method.Method, TypeSystemServices.ConditionalAttribute))
			{
				if (1 != attr.Arguments.Count) continue;

				if (attr.Arguments[0] is not StringLiteralExpression conditionString) continue;

				yield return conditionString.Value;
			}
		}

		private void PushTargetObjectFor(IMethod method, Expression target, IType targetType)
		{
			if (targetType is IGenericParameter)
			{
				// If target is a generic parameter, its address must be loaded
				// to allow a constrained method call
				LoadAddress(target);
				return;
			}

			if (targetType.IsValueType)
			{
				if (method.DeclaringType.IsValueType)
					LoadAddress(target);
				else
				{
					Visit(target);
					EmitBox(PopType());
				}
				return;
			}

			// pushes target reference
			Visit(target);
			PopType();
		}

		private static Expression GetTargetObject(MethodInvocationExpression node)
		{
			var target = node.Target;

			// Skip over generic reference expressions
			if (target is GenericReferenceExpression genericRef)
				target = genericRef.Target;

			if (target is MemberReferenceExpression memberRef)
				return memberRef.Target;

			return null;
		}

		private static ILOpCode GetCallOpCode(Expression target, IMethod method)
		{
			if (method.IsStatic) return ILOpCode.Call;
			if (NodeType.SuperLiteralExpression == target.NodeType) return ILOpCode.Call;
			if (IsValueTypeMethodCall(target, method)) return ILOpCode.Call;
			return ILOpCode.Callvirt;
		}

		private static bool IsValueTypeMethodCall(Expression target, IMethod method)
		{
			IType type = target.ExpressionType;
			return type.IsValueType && (method.DeclaringType == type || method.DeclaringType.ConstructedInfo?.GenericDefinition == type);
		}

		void InvokeSuperMethod(IMethod method, MethodInvocationExpression node)
		{
			var super = (IMethod)GetEntity(node.Target);
			if (method.DeclaringType.IsValueType)
				_il.LoadArgument(0);
			else
				_il.OpCode(ILOpCode.Ldarg_0); // this
			PushArguments(super, node.Arguments);
			Call(super);
			PushType(super.ReturnType);
		}

		void EmitGetTypeFromHandle(EntityHandle type)
		{
			_il.OpCode(ILOpCode.Ldtoken, type);
			Call(_typeGetTypeFromHandle);
			PushType(TypeSystemServices.TypeType);
		}

		void OnEval(MethodInvocationExpression node)
		{
			int allButLast = node.Arguments.Count - 1;
			for (int i = 0; i < allButLast; ++i)
			{
				Visit(node.Arguments[i]);
				DiscardValueOnStack();
			}

			Visit(node.Arguments[-1]);
		}

		void OnAddressOf(MethodInvocationExpression node)
		{
			MemberReferenceExpression methodRef = (MemberReferenceExpression)node.Arguments[0];
			var ent = SelfMapMethodIfNeeded((IMethod)GetEntity(methodRef));
			var method = GetMethodInfo(ent);
			if (ent.IsVirtual)
			{
				Dup();
				_il.OpCode(ILOpCode.Ldvirtftn);
			}
			else
			{
				_il.OpCode(ILOpCode.Ldftn);
			}
			_il.Token(method);
			PushType(TypeSystemServices.IntPtrType);
		}

		void OnBuiltinFunction(BuiltinFunction function, MethodInvocationExpression node)
		{
			switch (function.FunctionType)
			{
				case BuiltinFunctionType.Switch:
					{
						OnSwitch(node);
						break;
					}

				case BuiltinFunctionType.AddressOf:
					{
						OnAddressOf(node);
						break;
					}

				case BuiltinFunctionType.Eval:
					{
						OnEval(node);
						break;
					}

				case BuiltinFunctionType.InitValueType:
					{
						OnInitValueType(node);
						break;
					}

				case BuiltinFunctionType.Default:
					{
						EmitDefaultValue(node.ExpressionType);
						break;
					}

				default:
					{
						NotImplemented(node, "BuiltinFunction: " + function.FunctionType);
						break;
					}
			}
		}

		private void OnInitValueType(MethodInvocationExpression node)
		{
			Debug.Assert(1 == node.Arguments.Count);

			Expression argument = node.Arguments[0];
			LoadAddressForInitObj(argument);
			var expressionType = GetExpressionType(argument);
			var type = GetSystemType(expressionType);
			Debug.Assert(expressionType.IsValueType);
			_il.OpCode(ILOpCode.Initobj, type);
			PushVoid();
		}

		private void LoadAddressForInitObj(Expression argument)
		{
			IEntity entity = argument.Entity;
			switch (entity.EntityType)
			{
				case EntityType.Local:
					{
						InternalLocal local = (InternalLocal)entity;
						int slot = local.LocalVariableIndex;
						_il.LoadLocalAddress(slot);
						break;
					}
				case EntityType.Field:
					{
						EmitLoadFieldAddress(argument, (IField)entity);
						break;
					}
				default:
					NotImplemented(argument, "__initobj__");
					break;
			}
		}

		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			IEntity entity = TypeSystemServices.GetEntity(node.Target);
			switch (entity.EntityType)
			{
				case EntityType.BuiltinFunction:
					{
						OnBuiltinFunction((BuiltinFunction)entity, node);
						break;
					}

				case EntityType.Method:
					{
						var methodInfo = (IMethod)entity;
						if (node.Target.NodeType == NodeType.SuperLiteralExpression)
							InvokeSuperMethod(methodInfo, node);
						else
							InvokeMethod(methodInfo, node);
						break;
					}

				case EntityType.Constructor:
					{
						IConstructor constructorInfo = (IConstructor)SelfMapMethodIfNeeded((IMethod)entity);

						if (node.Target.NodeType is NodeType.SuperLiteralExpression or NodeType.SelfLiteralExpression)
						{
							// super constructor call
							_il.OpCode(ILOpCode.Ldarg_0);
							PushArguments(constructorInfo, node.Arguments);
							_il.Call(ILOpCode.Call, constructorInfo);
							PushVoid();
						}
						else
						{
							PushArguments(constructorInfo, node.Arguments);
							_il.Call(ILOpCode.Newobj, constructorInfo);

							// constructor invocation resulting type is
							PushType(constructorInfo.DeclaringType);
						}
						break;
					}

				default:
					{
						NotImplemented(node, entity.ToString());
						break;
					}
			}
		}

		public override void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node)
		{
			EmitLoadLiteral(node.Value.Ticks);
			_il.Call(ILOpCode.Newobj, _timeSpanLongConstructor);
			PushType(TypeSystemServices.TimeSpanType);
		}

		public override void OnIntegerLiteralExpression(IntegerLiteralExpression node)
		{
			IType type = node.ExpressionType ?? TypeSystemServices.IntType;
			EmitLoadLiteral(type, node.Value);
			PushType(type);
		}

		public override void OnDoubleLiteralExpression(DoubleLiteralExpression node)
		{
			IType type = node.ExpressionType ?? TypeSystemServices.DoubleType;
			EmitLoadLiteral(type, node.Value);
			PushType(type);
		}

		void EmitLoadLiteral(int i)
		{
			EmitLoadLiteral(TypeSystemServices.IntType, i);
		}

		void EmitLoadLiteral(long l)
		{
			EmitLoadLiteral(TypeSystemServices.LongType, l);
		}

		void EmitLoadLiteral(IType type, double d)
		{
			if (type == TypeSystemServices.SingleType)
			{
				if (d != 0) {
					_il.OpCode(ILOpCode.Ldc_r4);
					_il.CodeBuilder.WriteSingle((float)d);
				}
				else
				{
					_il.OpCode(ILOpCode.Ldc_i4_0);
					_il.OpCode(ILOpCode.Conv_r4);
				}
				return;
			}

			if (type == TypeSystemServices.DoubleType)
			{
				if (d != 0) {
					_il.OpCode(ILOpCode.Ldc_r8);
					_il.CodeBuilder.WriteDouble(d);
				}
				else
				{
					_il.OpCode(ILOpCode.Ldc_i4_0);
					_il.OpCode(ILOpCode.Conv_r8);
				}
				return;
			}

			throw new InvalidOperationException(string.Format("`{0}' is not a literal", type));
		}

		void EmitLoadLiteral(IType type, long l)
		{
			if (type.IsEnum)
				type = TypeSystemServices.Map(GetEnumUnderlyingType(type));

			if (!(IsInteger(type) || type == TypeSystemServices.CharType))
				throw new InvalidOperationException();

			var needsLongConv = true;
			switch (l)
			{
				case -1L:
					{
						if (IsLong(type) || type == TypeSystemServices.ULongType)
						{
							_il.LoadConstant(-1L);
							needsLongConv = false;
						}
						else
							_il.OpCode(ILOpCode.Ldc_i4_m1);
					}
					break;
				case 0L:
					_il.OpCode(ILOpCode.Ldc_i4_0);
					break;
				case 1L:
					_il.OpCode(ILOpCode.Ldc_i4_1);
					break;
				case 2L:
					_il.OpCode(ILOpCode.Ldc_i4_2);
					break;
				case 3L:
					_il.OpCode(ILOpCode.Ldc_i4_3);
					break;
				case 4L:
					_il.OpCode(ILOpCode.Ldc_i4_4);
					break;
				case 5L:
					_il.OpCode(ILOpCode.Ldc_i4_5);
					break;
				case 6L:
					_il.OpCode(ILOpCode.Ldc_i4_6);
					break;
				case 7L:
					_il.OpCode(ILOpCode.Ldc_i4_7);
					break;
				case 8L:
					_il.OpCode(ILOpCode.Ldc_i4_8);
					break;
				default:
					{
						if (IsLong(type))
						{
							_il.LoadConstant(l);
							return;
						}

						if (l == (sbyte)l) //fits in an signed i1
						{
							_il.OpCode(ILOpCode.Ldc_i4_s);
							_il.CodeBuilder.WriteSByte((sbyte)l);
						}
						else if (l == (int)l || l == (uint)l) //fits in an i4
						{
							if ((int)l == -1)
								_il.OpCode(ILOpCode.Ldc_i4_m1);
							else
								_il.LoadConstant((int)l);
						}
						else
						{
							_il.LoadConstant(l);
							needsLongConv = false;
						}
					}
					break;
			}

			if (needsLongConv && IsLong(type))
				_il.OpCode(ILOpCode.Conv_i8);
			else if (type == TypeSystemServices.ULongType)
				_il.OpCode(ILOpCode.Conv_u8);
		}

		private bool IsLong(IType type)
		{
			return type == TypeSystemServices.LongType;
		}

		public override void OnBoolLiteralExpression(BoolLiteralExpression node)
		{
			if (node.Value)
			{
				_il.OpCode(ILOpCode.Ldc_i4_1);
			}
			else
			{
				_il.OpCode(ILOpCode.Ldc_i4_0);
			}
			PushBool();
		}

		public override void OnHashLiteralExpression(HashLiteralExpression node)
		{
			_il.Call(ILOpCode.Newobj, _hashConstructor);

			var objType = TypeSystemServices.ObjectType;
			foreach (ExpressionPair pair in node.Items)
			{
				Dup();

				LoadExpressionWithType(objType, pair.First);
				LoadExpressionWithType(objType, pair.Second);
				_il.Call(ILOpCode.Callvirt, _hashAdd);
			}

			PushType(TypeSystemServices.HashType);
		}

		public override void OnGeneratorExpression(GeneratorExpression node)
		{
			NotImplemented(node, node.ToString());
		}

		public override void OnListLiteralExpression(ListLiteralExpression node)
		{
			if (node.Items.Count > 0)
			{
				EmitObjectArray(node.Items);
				_il.OpCode(ILOpCode.Ldc_i4_1);
				_il.Call(ILOpCode.Newobj, _listArrayBoolConstructor);
			}
			else
			{
				_il.Call(ILOpCode.Newobj, _listEmptyConstructor);
			}
			PushType(TypeSystemServices.ListType);
		}

		public override void OnArrayLiteralExpression(ArrayLiteralExpression node)
		{
			var type = node.ExpressionType;
			EmitArray(type.ElementType, node.Items);
			PushType(type);
		}

		public override void OnRELiteralExpression(RELiteralExpression node)
		{
			RegexOptions options = AstUtil.GetRegexOptions(node);

			_il.LoadString(node.Pattern);
			if (options == RegexOptions.None)
			{
				_il.Call(ILOpCode.Newobj, _regexConstructor);
			}
			else
			{
				EmitLoadLiteral((int)options);
				_il.Call(ILOpCode.Newobj, _regexConstructorOptions);
			}

			PushType(node.ExpressionType);
		}

		public override void OnStringLiteralExpression(StringLiteralExpression node)
		{
			if (null == node.Value)
			{
				_il.OpCode(ILOpCode.Ldnull);
			}
			else if (node.Value.Length > 0)
			{
				_il.LoadString(node.Value);
			}
			else /* force use of CLR-friendly string.Empty */
			{
				_il.OpCode(ILOpCode.Ldsfld, _stringEmptyField);
			}
			PushType(TypeSystemServices.StringType);
		}

		public override void OnCharLiteralExpression(CharLiteralExpression node)
		{
			EmitLoadLiteral(node.Value[0]);
			PushType(TypeSystemServices.CharType);
		}

		public override void OnSlicingExpression(SlicingExpression node)
		{
			if (node.IsTargetOfAssignment())
				return;

			Visit(node.Target);
			var type = (IArrayType)PopType();

			if (type.Rank == 1)
				LoadSingleDimensionalArrayElement(node, type);
			else
				LoadMultiDimensionalArrayElement(node, type);

			PushType(type.ElementType);
		}

		private void LoadMultiDimensionalArrayElement(SlicingExpression node, IArrayType arrayType)
		{
			LoadArrayIndices(node);
			CallArrayMethod(arrayType, "Get", arrayType.ElementType, ParameterTypesForArrayGet(arrayType));
		}

        private IType[] ParameterTypesForArrayGet(IArrayType arrayType) =>
			Enumerable.Repeat(TypeSystemServices.IntType, arrayType.Rank).ToArray();

        private IType[] ParameterTypesForArraySet(IArrayType arrayType)
		{
			var types = new IType[arrayType.Rank + 1];
			for (var i = 0; i < arrayType.Rank; ++i)
				types[i] = TypeSystemServices.IntType;
			types[arrayType.Rank] = arrayType.ElementType;
			return types;
		}

		private void CallArrayMethod(IType arrayType, string methodName, IType returnType, IType[] parameterTypes)
		{
			_il.CallArrayMethod(arrayType, methodName, returnType, parameterTypes);
		}

		private void LoadArrayIndices(SlicingExpression node)
		{
			foreach (var index in node.Indices.Select(index => index.Begin))
				LoadIntExpression(index);
		}

		private void LoadSingleDimensionalArrayElement(SlicingExpression node, IType arrayType)
		{
			EmitNormalizedArrayIndex(node, node.Indices[0].Begin);

			var elementType = arrayType.ElementType;
			var opcode = GetLoadEntityOpCode(elementType);
			_il.OpCode(opcode);
			if (opcode is ILOpCode.Ldelem or ILOpCode.Ldelema)
			{
				var systemType = GetSystemType(elementType);
				_il.Token(systemType);
				if (opcode == ILOpCode.Ldelema && !IsByAddress(elementType))
				{
					_il.OpCode(ILOpCode.Ldobj, systemType);
				}
			}
		}

		void EmitNormalizedArrayIndex(SlicingExpression sourceNode, Expression index)
		{
			bool isNegative = false;
			if (CanBeNegative(index, ref isNegative)
				&& !_rawArrayIndexing
				&& !AstAnnotations.IsRawIndexing(sourceNode))
			{
				if (isNegative)
				{
					Dup();
					_il.OpCode(ILOpCode.Ldlen);
					LoadIntExpression(index);
					_il.OpCode(ILOpCode.Add);
				}
				else
				{
					Dup();
					LoadIntExpression(index);
					Call(_runtimeServicesNormalizeArrayIndex);
				}
			}
			else
				LoadIntExpression(index);
		}

		static bool CanBeNegative(Expression expression, ref bool isNegative)
		{
			if (expression is IntegerLiteralExpression integer)
			{
				if (integer.Value >= 0)
					return false;
				isNegative = true;
			}
			return true;
		}

		void LoadIntExpression(Expression expression)
		{
			LoadExpressionWithType(TypeSystemServices.IntType, expression);
		}

		public override void OnExpressionInterpolationExpression(ExpressionInterpolationExpression node)
		{
			var constructor = _typeSystem.ConstructorOf<StringBuilder>(Type.EmptyTypes);
			var constructorString = _typeSystem.ConstructorOf<StringBuilder>(TypeSystemServices.StringType);

			var appendObject = _typeSystem.MethodOf<StringBuilder>("Append", TypeSystemServices.ObjectType);
			var appendString = _typeSystem.MethodOf<StringBuilder>("Append", TypeSystemServices.StringType);
			Expression arg0 = node.Expressions[0];
			IType argType = arg0.ExpressionType;

			/* if arg0 is a string, let's call StringBuilder constructor
			 * directly with the string */
			if ((typeof(StringLiteralExpression) == arg0.GetType()
				   && ((StringLiteralExpression)arg0).Value.Length > 0)
				|| (typeof(StringLiteralExpression) != arg0.GetType()
					 && TypeSystemServices.StringType == argType))
			{
				Visit(arg0);
				PopType();
				_il.Call(ILOpCode.Newobj, constructorString);
			}
			else
			{
				_il.Call(ILOpCode.Newobj, constructor);
				arg0 = null; /* arg0 is not a string so we want it to be appended below */
			}

			string formatString;
			foreach (Expression arg in node.Expressions)
			{
				/* we do not need to append literal string.Empty
				 * or arg0 if it has been handled by ctor */
				if ((typeof(StringLiteralExpression) == arg.GetType()
					   && ((StringLiteralExpression)arg).Value.Length == 0)
					|| arg == arg0)
				{
					continue;
				}

				formatString = arg["formatString"] as string; //annotation
				if (!string.IsNullOrEmpty(formatString))
					_il.LoadString(string.Format("{{0:{0}}}", formatString));

				Visit(arg);
				argType = PopType();

				if (!string.IsNullOrEmpty(formatString))
				{
					EmitCastIfNeeded(TypeSystemServices.ObjectType, argType);
					Call(StringFormat);
				}

				if (TypeSystemServices.StringType == argType || !string.IsNullOrEmpty(formatString))
				{
					Call(appendString);
				}
				else
				{
					EmitCastIfNeeded(TypeSystemServices.ObjectType, argType);
					Call(appendObject);
				}
			}
			Call(_typeSystem.MethodOf<StringBuilder>("ToString", Type.EmptyTypes));
			PushType(TypeSystemServices.StringType);
		}

		void LoadMemberTarget(Expression self, IMember member)
		{
			if (member.DeclaringType.IsValueType)
			{
				LoadAddress(self);
			}
			else
			{
				Visit(self);
				PopType();
			}
		}

		void EmitLoadFieldAddress(Expression expression, IField field)
		{
			field = SelfMapFieldIfNeeded(field);
			if (field.IsStatic)
			{
				_il.OpCode(ILOpCode.Ldsflda, GetFieldInfo(field));
			}
			else
			{
				LoadMemberTarget(((MemberReferenceExpression)expression).Target, field);
				_il.OpCode(ILOpCode.Ldflda, GetFieldInfo(field));
			}
		}

		void EmitLoadField(Expression self, IField fieldInfo)
		{
			fieldInfo = SelfMapFieldIfNeeded(fieldInfo);
			if (fieldInfo.IsStatic)
			{
				if (fieldInfo.IsLiteral)
				{
					EmitLoadLiteralField(self, fieldInfo);
				}
				else
				{
					if (fieldInfo.IsVolatile)
						_il.OpCode(ILOpCode.Volatile);
					_il.OpCode(IsByAddress(fieldInfo.Type) ? ILOpCode.Ldsflda : ILOpCode.Ldsfld,
						GetFieldInfo(fieldInfo));
				}
			}
			else
			{
				LoadMemberTarget(self, fieldInfo);
				if (fieldInfo.IsVolatile)
					_il.OpCode(ILOpCode.Volatile);
				_il.OpCode(IsByAddress(fieldInfo.Type) ? ILOpCode.Ldflda : ILOpCode.Ldfld,
					GetFieldInfo(fieldInfo));
			}
			PushType(fieldInfo.Type);
		}

		private static IType SelfMapTypeIfNeeded(IType type)
        {
			if (type.ConstructedInfo == null && type.GenericInfo != null)
			{
				type = type.GenericInfo.ConstructType(type.GenericInfo.GenericParameters);
			}
			return type;
		}

		private static IField SelfMapFieldIfNeeded(IField field)
        {
			var type = field.DeclaringType;
			if (type.ConstructedInfo == null && type.GenericInfo != null)
            {
				var conType = type.GenericInfo.ConstructType(type.GenericInfo.GenericParameters);
				return (IField)conType.ConstructedInfo.Map(field);
            }
			return field;
        }

		private static IMethod SelfMapMethodIfNeeded(IMethod method)
		{
			var type = method.DeclaringType;
			if (type.ConstructedInfo == null && type.GenericInfo != null)
			{
				var conType = type.GenericInfo.ConstructType(type.GenericInfo.GenericParameters);
				return (IMethod)conType.ConstructedInfo.Map(method);
			}
			return method;
		}

		object GetStaticValue(IField field)
		{
			if (field is InternalField internalField)
			{
				return GetInternalFieldStaticValue(internalField);
			}
			return field.StaticValue;
		}

		object GetInternalFieldStaticValue(InternalField field)
		{
			return GetValue(field.Type, (Expression)field.StaticValue);
		}

		void EmitLoadLiteralField(Node node, IField fieldInfo)
		{
			object value = GetStaticValue(fieldInfo);
			IType type = fieldInfo.Type;
			IType enumType = null;
			if (type.IsEnum)
			{
				enumType = type;
				Type underlyingType = GetEnumUnderlyingType(type);
				type = TypeSystemServices.Map(underlyingType);
				value = Convert.ChangeType(value, underlyingType);
			}

			if (null == value)
			{
				_il.OpCode(ILOpCode.Ldnull);
			}
			else if (type == TypeSystemServices.BoolType)
			{
				_il.OpCode(((bool)value) ? ILOpCode.Ldc_i4_1 : ILOpCode.Ldc_i4_0);
			}
			else if (type == TypeSystemServices.StringType)
			{
				_il.LoadString((string)value);
			}
			else if (type == TypeSystemServices.CharType)
			{
				EmitLoadLiteral(type, (long)(char)value);
			}
			else if (type == TypeSystemServices.IntType)
			{
				EmitLoadLiteral(type, (long)(int)value);
			}
			else if (type == TypeSystemServices.UIntType)
			{
				EmitLoadLiteral(type, unchecked((long)(uint)value));
			}
			else if (IsLong(type))
			{
				EmitLoadLiteral(type, (long)value);
			}
			else if (type == TypeSystemServices.ULongType)
			{
				EmitLoadLiteral(type, unchecked((long)(ulong)value));
			}
			else if (type == TypeSystemServices.SingleType)
			{
				EmitLoadLiteral(type, (double)(float)value);
			}
			else if (type == TypeSystemServices.DoubleType)
			{
				EmitLoadLiteral(type, (double)value);
			}
			else if (type == TypeSystemServices.SByteType)
			{
				EmitLoadLiteral(type, (long)(sbyte)value);
			}
			else if (type == TypeSystemServices.ByteType)
			{
				EmitLoadLiteral(type, (long)(byte)value);
			}
			else if (type == TypeSystemServices.ShortType)
			{
				EmitLoadLiteral(type, (long)(short)value);
			}
			else if (type == TypeSystemServices.UShortType)
			{
				EmitLoadLiteral(type, (long)(ushort)value);
			}
			else
			{
				NotImplemented(node, "Literal field type: " + type.ToString());
			}
			if (enumType != null)
            {
				var local = StoreTempLocal(enumType);
				_il.LoadLocal(local);
            }
		}

		public override void OnGenericReferenceExpression(GenericReferenceExpression node)
		{
			IEntity entity = TypeSystemServices.GetEntity(node);
			switch (entity.EntityType)
			{
				case EntityType.Type:
					{
						EmitGetTypeFromHandle(GetSystemType(node));
						break;
					}

				case EntityType.Method:
					{
						node.Target.Accept(this);
						break;
					}

				default:
					{
						NotImplemented(node, entity.ToString());
						break;
					}
			}
		}

		public override void OnMemberReferenceExpression(MemberReferenceExpression node)
		{
			var tag = TypeSystemServices.GetEntity(node);
			switch (tag.EntityType)
			{
				case EntityType.Ambiguous:
				case EntityType.Method:
					{
						node.Target.Accept(this);
						break;
					}

				case EntityType.Field:
					{
						EmitLoadField(node.Target, (IField)tag);
						break;
					}

				case EntityType.Type:
					{
						EmitGetTypeFromHandle(GetSystemType(node));
						break;
					}

				default:
					{
						NotImplemented(node, tag.ToString());
						break;
					}
			}
		}

		void LoadAddress(Expression expression)
		{
			if (expression.NodeType == NodeType.SelfLiteralExpression && expression.ExpressionType.IsValueType)
			{
				_il.OpCode(ILOpCode.Ldarg_0);
				return;
			}

			var entity = expression.Entity;
			if (entity != null)
			{
				switch (entity.EntityType)
				{
					case EntityType.Local:
					{
						var local = (InternalLocal)entity;
						var idx = local.LocalVariableIndex;
						if (local.Type.IsPointer)
						{
							_il.LoadLocal(idx);
						}
						else
						{
							_il.LoadLocalAddress(idx);
						}
						return;
					}

					case EntityType.Parameter:
					{
						var param = (InternalParameter)entity;
						if (param.Parameter.IsByRef)
							LoadParam(param);
						else
							_il.LoadArgumentAddress(param.Index);
						return;
					}

					case EntityType.Field:
					{
						var field = (IField)entity;
						if (!field.IsLiteral)
						{
							EmitLoadFieldAddress(expression, field);
							return;
						}
						break;
					}
				}
			}

			if (IsValueTypeArraySlicing(expression))
			{
				LoadArrayElementAddress((SlicingExpression)expression);
				return;
			}

			Visit(expression);
			if (!AstUtil.IsIndirection(expression))
			{
				// declare local to hold value type
				var temp = _il.AddLocal(PopType());
				_il.StoreLocal(temp);
				_il.LoadLocalAddress(temp);
			}
		}

		private void LoadArrayElementAddress(SlicingExpression slicing)
		{
			Visit(slicing.Target);
			var arrayType = (IArrayType)PopType();

			if (arrayType.Rank == 1)
				LoadSingleDimensionalArrayElementAddress(slicing, arrayType);
			else
				LoadMultiDimensionalArrayElementAddress(slicing, arrayType);
		}

		private void LoadMultiDimensionalArrayElementAddress(SlicingExpression slicing, IArrayType arrayType)
		{
			LoadArrayIndices(slicing);
			CallArrayMethod(arrayType, "Address", arrayType.ElementType.MakeByRefType(), ParameterTypesForArrayGet(arrayType));
		}

		private void LoadSingleDimensionalArrayElementAddress(SlicingExpression slicing, IArrayType arrayType)
		{
			EmitNormalizedArrayIndex(slicing, slicing.Indices[0].Begin);
			_il.OpCode(ILOpCode.Ldelema, GetSystemType(arrayType.ElementType));
		}

		static bool IsValueTypeArraySlicing(Expression expression)
		{
			if (expression is SlicingExpression slicing)
			{
				var type = (IArrayType)slicing.Target.ExpressionType;
				return type.ElementType.IsValueType;
			}
			return false;
		}

		public override void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			LoadSelf(node);
		}

		public override void OnSuperLiteralExpression(SuperLiteralExpression node)
		{
			LoadSelf(node);
		}

		private void LoadSelf(Expression node)
		{
			_il.OpCode(ILOpCode.Ldarg_0);
			if (node.ExpressionType.IsValueType)
				_il.OpCode(ILOpCode.Ldobj, GetSystemType(node.ExpressionType));
			PushType(node.ExpressionType);
		}

		public override void OnNullLiteralExpression(NullLiteralExpression node)
		{
			_il.OpCode(ILOpCode.Ldnull);
			PushType(null);
		}

		public override void OnReferenceExpression(ReferenceExpression node)
		{
			var entity = TypeSystemServices.GetEntity(node);
			switch (entity.EntityType)
			{
				case EntityType.Local:
					{
						if (!AstUtil.IsIndirection(node.ParentNode))
							LoadLocal((InternalLocal)entity);
						else
							LoadIndirectLocal((InternalLocal)entity);
						break;
					}

				case EntityType.Parameter:
					{
						var param = (InternalParameter)entity;
						LoadParam(param);

						if (param.Parameter.IsByRef)
						{
							var code = GetLoadRefParamCode(param.Type);
							_il.OpCode(code);
							if (code == ILOpCode.Ldobj)
							{
								_il.Token(GetSystemType(param.Type));
							}
						}
						PushType(param.Type);
						break;
					}

				case EntityType.Array:
				case EntityType.Type:
					{
						EmitGetTypeFromHandle(GetSystemType(node));
						break;
					}

				default:
					{
						NotImplemented(node, entity.ToString());
						break;
					}

			}
		}

		void LoadParam(InternalParameter param)
		{
			int index = param.Index;
			_il.LoadArgument(index);
		}

		void SetLocal(BinaryExpression node, InternalLocal tag, bool leaveValueOnStack)
		{
			if (AstUtil.IsIndirection(node.Left))
				_il.LoadLocal(tag.LocalVariableIndex);

			node.Right.Accept(this); // leaves type on stack

			IType typeOnStack;

			if (leaveValueOnStack)
			{
				typeOnStack = PeekTypeOnStack();
				Dup();
			}
			else
			{
				typeOnStack = PopType();
			}

			if (!AstUtil.IsIndirection(node.Left))
				EmitAssignment(tag, typeOnStack);
			else
				EmitIndirectAssignment(tag, typeOnStack);
		}

		void EmitAssignment(InternalLocal tag, IType typeOnStack)
		{
			// todo: assignment result must be type on the left in the
			// case of casting
			int slot = tag.LocalVariableIndex;
			EmitCastIfNeeded(tag.Type, typeOnStack);
			_il.StoreLocal(slot);
		}

		void EmitIndirectAssignment(InternalLocal local, IType typeOnStack)
		{
			var elementType = local.Type.ElementType;
			EmitCastIfNeeded(elementType, typeOnStack);

			var code = GetStoreRefParamCode(elementType);
			_il.OpCode(code);
			if (code == ILOpCode.Stobj)
				_il.Token(GetSystemType(elementType));
		}

		void SetField(Node sourceNode, IField field, Expression reference, Expression value, bool leaveValueOnStack)
		{
			field = SelfMapFieldIfNeeded(field);
			ILOpCode opSetField = ILOpCode.Stsfld;
			if (!field.IsStatic)
			{
				opSetField = ILOpCode.Stfld;
				if (null != reference)
				{
					LoadMemberTarget(
						((MemberReferenceExpression)reference).Target,
						field);
				}
			}

			LoadExpressionWithType(field.Type, value);

			int local = default;
			if (leaveValueOnStack)
			{
				Dup();
				local = _il.AddLocal(field.Type);
				_il.StoreLocal(local);
			}

			if (field.IsVolatile)
				_il.OpCode(ILOpCode.Volatile);
			_il.OpCode(opSetField, GetFieldInfo(field));

			if (leaveValueOnStack)
			{
				_il.LoadLocal(local);
				PushType(field.Type);
			}
		}

		void SetProperty(IProperty property, Expression reference, Expression value, bool leaveValueOnStack)
		{
			ILOpCode callOpCode = ILOpCode.Call;

			var setMethod = SelfMapMethodIfNeeded(property.GetSetMethod());
			IType targetType = null;
			if (null != reference)
			{
				if (!setMethod.IsStatic)
				{
					Expression target = ((MemberReferenceExpression)reference).Target;
					targetType = target.ExpressionType;
					if (setMethod.DeclaringType.IsValueType || targetType is IGenericParameter)
						LoadAddress(target);
					else
					{
						callOpCode = GetCallOpCode(target, property.GetSetMethod());
						target.Accept(this);
						PopType();
					}
				}
			}

			LoadExpressionWithType(property.Type, value);

			int local = default;
			if (leaveValueOnStack)
			{
				Dup();
				local = _il.AddLocal(property.Type);
				_il.StoreLocal(local);
			}

			if (targetType is IGenericParameter)
			{
				_il.OpCode(ILOpCode.Constrained, GetSystemType(targetType));
				callOpCode = ILOpCode.Callvirt;
			}

			_il.Call(callOpCode, setMethod);

			if (leaveValueOnStack)
			{
				_il.LoadLocal(local);
				PushType(property.Type);
			}
		}

		bool EmitDebugInfo(Node node)
		{
			if (!Parameters.Debug)
				return false;
			var start = node.LexicalInfo;
			_il.MarkSequencePoint(start);
			return true;
			//return EmitDebugInfo(node, node);
		}

		private const int _DBG_SYMBOLS_QUEUE_CAPACITY = 5;

		private readonly Queue<LexicalInfo> _dbgSymbols = new(_DBG_SYMBOLS_QUEUE_CAPACITY);

		bool EmitDebugInfo(Node startNode, Node endNode)
		{
			LexicalInfo start = startNode.LexicalInfo;
			if (!start.IsValid) return false;

			ISymbolDocumentWriter writer = GetDocumentWriter(start.FullPath);
			if (null == writer) return false;

			// ensure there is no duplicate emitted
			if (_dbgSymbols.Contains(start))
			{
				Context.TraceInfo("duplicate symbol emit attempt for '{0}' : '{1}'.", start, startNode);
				return false;
			}
			if (_dbgSymbols.Count >= _DBG_SYMBOLS_QUEUE_CAPACITY) _dbgSymbols.Dequeue();
			_dbgSymbols.Enqueue(start);

			try
			{
				//_il.MarkSequencePoint(writer, start.Line, 0, start.Line+1, 0);
			}
			catch (Exception x)
			{
				Error(CompilerErrorFactory.InternalError(startNode, x));
				return false;
			}
			return true;
		}

		private void EmitNop()
		{
			_il.OpCode(ILOpCode.Nop);
		}

		private ISymbolDocumentWriter GetDocumentWriter(string fname)
		{
			ISymbolDocumentWriter writer = GetCachedDocumentWriter(fname);
			if (null != writer) return writer;
			/*
			writer = _moduleBuilder.DefineDocument(
				fname,
				Guid.Empty,
				Guid.Empty,
				SymDocumentType.Text);
			_symbolDocWriters.Add(fname, writer);

			return writer;
			*/
			return null;
		}

		private ISymbolDocumentWriter GetCachedDocumentWriter(string fname) =>
			_symbolDocWriters[fname];

		bool IsBoolOrInt(IType type) => 
			TypeSystemServices.BoolType == type
			|| TypeSystemServices.IntType == type;

		void PushArguments(IMethodBase entity, ExpressionCollection args)
		{
			var parameters = entity.GetParameters();
			for (var i = 0; i < args.Count; ++i)
			{
				var parameterType = parameters[i].Type;
				var arg = args[i];
				if (parameters[i].IsByRef)
					LoadAddress(arg);
				else
					LoadExpressionWithType(parameterType, arg);
			}
		}

		void EmitObjectArray(ExpressionCollection items)
		{
			EmitArray(TypeSystemServices.ObjectType, items);
		}

		const int InlineArrayItemCountLimit = 3;

		void EmitArray(IType type, ExpressionCollection items)
		{
			EmitLoadLiteral(items.Count);
			_il.OpCode(ILOpCode.Newarr, GetSystemType(type));

			if (items.Count == 0)
				return;

			var inlineStores = 0;
			if (items.Count > InlineArrayItemCountLimit && TypeSystemServices.IsPrimitiveNumber(type))
			{
				//packed array are only supported for a literal array of
				//an unique primitive type. check that all items are literal
				//and count number of actual stores in order to build/emit
				//a packed array only if is is an advantage
				foreach (Expression item in items)
				{
					if ((item.NodeType != NodeType.IntegerLiteralExpression
						&& item.NodeType != NodeType.DoubleLiteralExpression)
						|| type != item.ExpressionType)
					{
						inlineStores = 0;
						break;
					}
					if (IsZeroEquivalent(item))
						continue;
					++inlineStores;
				}
			}

			if (inlineStores <= InlineArrayItemCountLimit)
				EmitInlineArrayInit(type, items);
			else
				EmitPackedArrayInit(type, items);
		}

		void EmitInlineArrayInit(IType type, ExpressionCollection items)
		{
			ILOpCode opcode = GetStoreEntityOpCode(type);
			for (int i = 0; i < items.Count; ++i)
			{
				if (IsNull(items[i]))
					continue; //do not emit even if types are not the same (null is any)
				if (type == items[i].ExpressionType && IsZeroEquivalent(items[i]))
					continue; //do not emit unnecessary init to zero
				StoreEntity(opcode, i, items[i], type);
			}
		}

		void EmitPackedArrayInit(IType type, ExpressionCollection items)
		{
			// TODO: Fix this later
			EmitInlineArrayInit(type, items);
			return;

			/*
			byte[] ba = CreateByteArrayFromLiteralCollection(type, items);
			if (ba == null)
			{
				EmitInlineArrayInit(type, items);
				return;
			}

			if (!_packedArrays.TryGetValue(ba, out FieldBuilder fb))
			{
				//there is no previously emitted bytearray to reuse, create it then
				fb = _moduleBuilder.DefineInitializedData(Context.GetUniqueName("newarr"), ba, FieldAttributes.Private);
				_packedArrays.Add(ba, fb);
			}

			Dup(); //dup (newarr)
			_il.OpCode(ILOpCode.Ldtoken, fb.Handle);
			Call(_runtimeHelpersInitializeArray);
			*/
		}

		readonly Dictionary<byte[], FieldBuilder> _packedArrays = new(ValueTypeArrayEqualityComparer<byte>.Default);

		byte[] CreateByteArrayFromLiteralCollection(IType type, ExpressionCollection items)
		{
			using var ms = new MemoryStream(items.Count * TypeSystemServices.SizeOf(type));
			using var writer = new BinaryWriter(ms);
			foreach (Expression item in items)
			{
				//TODO: BOO-1222 NumericLiteralExpression.GetValueAs<T>()
				if (item.NodeType == NodeType.IntegerLiteralExpression)
				{
					IntegerLiteralExpression literal = (IntegerLiteralExpression)item;
					if (type == TypeSystemServices.IntType)
						writer.Write(Convert.ToInt32(literal.Value));
					else if (type == TypeSystemServices.UIntType)
						writer.Write(Convert.ToUInt32(literal.Value));
					else if (IsLong(type))
						writer.Write(Convert.ToInt64(literal.Value));
					else if (type == TypeSystemServices.ULongType)
						writer.Write(Convert.ToUInt64(literal.Value));
					else if (type == TypeSystemServices.ShortType)
						writer.Write(Convert.ToInt16(literal.Value));
					else if (type == TypeSystemServices.UShortType)
						writer.Write(Convert.ToUInt16(literal.Value));
					else if (type == TypeSystemServices.ByteType)
						writer.Write(Convert.ToByte(literal.Value));
					else if (type == TypeSystemServices.SByteType)
						writer.Write(Convert.ToSByte(literal.Value));
					else if (type == TypeSystemServices.SingleType)
						writer.Write(Convert.ToSingle(literal.Value));
					else if (type == TypeSystemServices.DoubleType)
						writer.Write(Convert.ToDouble(literal.Value));
					else
						return null;
				}
				else if (item.NodeType == NodeType.DoubleLiteralExpression)
				{
					DoubleLiteralExpression literal = (DoubleLiteralExpression)item;
					if (type == TypeSystemServices.SingleType)
						writer.Write(Convert.ToSingle(literal.Value));
					else if (type == TypeSystemServices.DoubleType)
						writer.Write(Convert.ToDouble(literal.Value));
					else if (type == TypeSystemServices.IntType)
						writer.Write(Convert.ToInt32(literal.Value));
					else if (type == TypeSystemServices.UIntType)
						writer.Write(Convert.ToUInt32(literal.Value));
					else if (IsLong(type))
						writer.Write(Convert.ToInt64(literal.Value));
					else if (type == TypeSystemServices.ULongType)
						writer.Write(Convert.ToUInt64(literal.Value));
					else if (type == TypeSystemServices.ShortType)
						writer.Write(Convert.ToInt16(literal.Value));
					else if (type == TypeSystemServices.UShortType)
						writer.Write(Convert.ToUInt16(literal.Value));
					else if (type == TypeSystemServices.ByteType)
						writer.Write(Convert.ToByte(literal.Value));
					else if (type == TypeSystemServices.SByteType)
						writer.Write(Convert.ToSByte(literal.Value));
					else
						return null;
				}
				else
					return null;
			}
			return ms.ToArray();
		}

		bool IsInteger(IType type)
		{
			return TypeSystemServices.IsIntegerNumber(type);
		}

		IMethodBase GetToDecimalConversionMethod(IType type)
		{
			var method = _typeSystem.MethodOf<decimal>("op_Implicit", type);

			if (method == null)
			{
				method = _typeSystem.MethodOf<decimal>("op_Explicit", type);
				if (method == null)
				{
					NotImplemented($"Numeric promotion for {type} to decimal not implemented!");
				}
			}
			return method;
		}

		IMethodBase GetFromDecimalConversionMethod(IType type)
		{
			string toType = "To" + type.Name;

			var method = _typeSystem.MethodOf<decimal>(toType, type);
			if (method == null)
			{
				NotImplemented($"Numeric promotion for decimal to {type} not implemented!");
			}
			return method;
		}

		ILOpCode GetArithmeticOpCode(IType type, BinaryOperatorType op)
		{
			if (IsCheckedIntegerOperand(type))
			{
				switch (op)
				{
					case BinaryOperatorType.Addition: return ILOpCode.Add_ovf;
					case BinaryOperatorType.Subtraction: return ILOpCode.Sub_ovf;
					case BinaryOperatorType.Multiply: return ILOpCode.Mul_ovf;
					case BinaryOperatorType.Division: return ILOpCode.Div;
					case BinaryOperatorType.Modulus: return ILOpCode.Rem;
				}
			}
			else
			{
				switch (op)
				{
					case BinaryOperatorType.Addition: return ILOpCode.Add;
					case BinaryOperatorType.Subtraction: return ILOpCode.Sub;
					case BinaryOperatorType.Multiply: return ILOpCode.Mul;
					case BinaryOperatorType.Division: return ILOpCode.Div;
					case BinaryOperatorType.Modulus: return ILOpCode.Rem;
				}
			}
			throw new ArgumentException($"No artithmetic opcode found for type {type} and operator type {op}");
		}

		ILOpCode GetLoadEntityOpCode(IType type)
		{
			if (IsByAddress(type))
				return ILOpCode.Ldelema;

			if (!type.IsValueType)
			{
				return type is IGenericParameter
					? ILOpCode.Ldelem
					: ILOpCode.Ldelem_ref;
			}

			if (type.IsEnum)
			{
				type = TypeSystemServices.Map(GetEnumUnderlyingType(type));
			}

			if (TypeSystemServices.IntType == type)
			{
				return ILOpCode.Ldelem_i4;
			}
			if (TypeSystemServices.UIntType == type)
			{
				return ILOpCode.Ldelem_u4;
			}
			if (IsLong(type))
			{
				return ILOpCode.Ldelem_i8;
			}
			if (TypeSystemServices.SByteType == type)
			{
				return ILOpCode.Ldelem_i1;
			}
			if (TypeSystemServices.ByteType == type)
			{
				return ILOpCode.Ldelem_u1;
			}
			if (TypeSystemServices.ShortType == type ||
				TypeSystemServices.CharType == type)
			{
				return ILOpCode.Ldelem_i2;
			}
			if (TypeSystemServices.UShortType == type)
			{
				return ILOpCode.Ldelem_u2;
			}
			if (TypeSystemServices.SingleType == type)
			{
				return ILOpCode.Ldelem_r4;
			}
			if (TypeSystemServices.DoubleType == type)
			{
				return ILOpCode.Ldelem_r8;
			}
			//NotImplemented("LoadEntityOpCode(" + tag + ")");
			return ILOpCode.Ldelema;
		}

		ILOpCode GetStoreEntityOpCode(IType tag)
		{
			if (tag.IsValueType || tag is IGenericParameter)
			{
				if (tag.IsEnum)
				{
					tag = TypeSystemServices.Map(GetEnumUnderlyingType(tag));
				}

				if (TypeSystemServices.IntType == tag ||
					TypeSystemServices.UIntType == tag)
				{
					return ILOpCode.Stelem_i4;
				}
				else if (IsLong(tag) ||
					TypeSystemServices.ULongType == tag)
				{
					return ILOpCode.Stelem_i8;
				}
				else if (TypeSystemServices.ShortType == tag ||
					TypeSystemServices.CharType == tag)
				{
					return ILOpCode.Stelem_i2;
				}
				else if (TypeSystemServices.ByteType == tag ||
					TypeSystemServices.SByteType == tag)
				{
					return ILOpCode.Stelem_i1;
				}
				else if (TypeSystemServices.SingleType == tag)
				{
					return ILOpCode.Stelem_r4;
				}
				else if (TypeSystemServices.DoubleType == tag)
				{
					return ILOpCode.Stelem_r8;
				}
				return ILOpCode.Stobj;
			}

			return ILOpCode.Stelem_ref;
		}

		ILOpCode GetLoadRefParamCode(IType tag)
		{
			if (tag.IsValueType)
			{
				if (tag.IsEnum)
				{
					tag = TypeSystemServices.Map(GetEnumUnderlyingType(tag));
				}
				if (TypeSystemServices.IntType == tag)
				{
					return ILOpCode.Ldind_i4;
				}
				if (IsLong(tag) ||
					TypeSystemServices.ULongType == tag)
				{
					return ILOpCode.Ldind_i8;
				}
				if (TypeSystemServices.ByteType == tag)
				{
					return ILOpCode.Ldind_u1;
				}
				if (TypeSystemServices.ShortType == tag ||
					TypeSystemServices.CharType == tag)
				{
					return ILOpCode.Ldind_i2;
				}
				if (TypeSystemServices.SingleType == tag)
				{
					return ILOpCode.Ldind_r4;
				}
				if (TypeSystemServices.DoubleType == tag)
				{
					return ILOpCode.Ldind_r8;
				}
				if (TypeSystemServices.UShortType == tag)
				{
					return ILOpCode.Ldind_u2;
				}
				if (TypeSystemServices.UIntType == tag)
				{
					return ILOpCode.Ldind_u4;
				}

				return ILOpCode.Ldobj;
			}
			return ILOpCode.Ldind_ref;
		}

		ILOpCode GetStoreRefParamCode(IType tag)
		{
			if (tag.IsValueType)
			{
				if (tag.IsEnum)
				{
					tag = TypeSystemServices.Map(GetEnumUnderlyingType(tag));
				}
				if (TypeSystemServices.IntType == tag
					|| TypeSystemServices.UIntType == tag)
				{
					return ILOpCode.Stind_i4;
				}
				if (IsLong(tag)
					|| TypeSystemServices.ULongType == tag)
				{
					return ILOpCode.Stind_i8;
				}
				if (TypeSystemServices.ByteType == tag)
				{
					return ILOpCode.Stind_i1;
				}
				if (TypeSystemServices.ShortType == tag ||
					TypeSystemServices.CharType == tag)
				{
					return ILOpCode.Stind_i2;
				}
				if (TypeSystemServices.SingleType == tag)
				{
					return ILOpCode.Stind_r4;
				}
				if (TypeSystemServices.DoubleType == tag)
				{
					return ILOpCode.Stind_r8;
				}

				return ILOpCode.Stobj;
			}
			return ILOpCode.Stind_ref;
		}

		bool IsAssignableFrom(IType expectedType, IType actualType)
		{
			return (IsPtr(expectedType) && IsPtr(actualType))
				|| TypeCompatibilityRules.IsAssignableFrom(expectedType, actualType);
		}

		bool IsPtr(IType type)
		{
			return (type == TypeSystemServices.IntPtrType)
				|| (type == TypeSystemServices.UIntPtrType);
		}

		void EmitCastIfNeeded(IType expectedType, IType actualType)
		{
			if (actualType == null) // see NullLiteralExpression
				return;

			if (expectedType == actualType)
				return;

			if (expectedType.IsPointer || actualType.IsPointer) //no cast needed for addresses
				return;

			if (IsAssignableFrom(expectedType, actualType))
			{
				EmitBoxIfNeeded(expectedType, actualType);
				return;
			}

			var method = TypeSystemServices.FindImplicitConversionOperator(actualType, expectedType)
						 ?? TypeSystemServices.FindExplicitConversionOperator(actualType, expectedType);
			if (method != null)
			{
				EmitBoxIfNeeded(method.GetParameters()[0].Type, actualType);
				Call(method);
				return;
			}

			if (expectedType is IGenericParameter)
			{
				// Since expected type is a generic parameter, we don't know whether to emit
				// an unbox opcode or a castclass opcode; so we emit an unbox.any opcode which
				// works as either of those at runtime
				_il.OpCode(ILOpCode.Unbox_any, GetSystemType(expectedType));
				return;
			}

			if (expectedType.IsValueType)
			{
				if (!actualType.IsValueType)
				{
					// To get a value type out of a reference type we emit an unbox opcode
					EmitUnbox(expectedType);
					return;
				}

				// numeric promotion
				if (TypeSystemServices.DecimalType == expectedType)
				{
					Call(GetToDecimalConversionMethod(actualType));
				}
				else if (TypeSystemServices.DecimalType == actualType)
				{
					Call(GetFromDecimalConversionMethod(expectedType));
				}
				else
				{
					//we need to get the real underlying type here and no earlier
					//(because cause enum casting from int can occur [e.g enums-13])
					if (actualType.IsEnum)
						actualType = TypeSystemServices.Map(GetEnumUnderlyingType(actualType));
					if (expectedType.IsEnum)
						expectedType = TypeSystemServices.Map(GetEnumUnderlyingType(expectedType));
					if (actualType != expectedType) //do we really need conv?
						_il.OpCode(GetNumericPromotionOpCode(expectedType));
				}
				return;
			}

			EmitRuntimeCoercionIfNeeded(expectedType, actualType);
		}

		private void EmitRuntimeCoercionIfNeeded(IType expectedType, IType actualType)
		{
			// In order to cast to a reference type we emit a castclass opcode
			Context.TraceInfo("castclass: expected type='{0}', type on stack='{1}'", expectedType, actualType);
			var expectedSystemType = GetSystemType(expectedType);
			if (TypeSystemServices.IsSystemObject(actualType))
			{
				Dup();
				Isinst(expectedSystemType);

				var skipCoercion = _il.DefineLabel();
				_il.Branch(ILOpCode.Brtrue, skipCoercion);

				EmitGetTypeFromHandle(expectedSystemType); PopType();
				Call(RuntimeServices_Coerce);

				_il.MarkLabel(skipCoercion);
			}
			Castclass(expectedSystemType);
		}

		private void Call(IMethodBase method)
		{
			_il.Call(ILOpCode.Call, method);
		}

		private void Castclass(EntityHandle expectedSystemType)
		{
			_il.OpCode(ILOpCode.Castclass, expectedSystemType);
		}

		private IMethod _RuntimeServices_Coerce;

		private IMethod RuntimeServices_Coerce
		{
			get
			{
				if (_RuntimeServices_Coerce == null)
					_RuntimeServices_Coerce = TypeSystemServices.Map(Types.RuntimeServices.GetMethod("Coerce", new Type[] { Types.Object, Types.Type }));
				return _RuntimeServices_Coerce;
			}
		}

		private void EmitBoxIfNeeded(IType expectedType, IType actualType)
		{
			if ((actualType.IsValueType && !expectedType.IsValueType)
				|| (actualType is IGenericParameter && expectedType is not IGenericParameter))
				EmitBox(actualType);
		}

		void EmitBox(IType type)
		{
			_il.OpCode(ILOpCode.Box, GetSystemType(type));
		}

		void EmitUnbox(IType expectedType)
		{
			var unboxMethod = UnboxMethodFor(expectedType);
			if (unboxMethod != null)
			{
				Call(unboxMethod);
			}
			else
			{
				var type = GetSystemType(expectedType);
				_il.OpCode(ILOpCode.Unbox, type);
				_il.OpCode(ILOpCode.Ldobj, type);
			}
		}

		IMethodBase UnboxMethodFor(IType type)
		{
			if (type == TypeSystemServices.ByteType) return _typeSystem.MethodOf<RuntimeServices>("UnboxByte", typeof(object));
			if (type == TypeSystemServices.SByteType) return _typeSystem.MethodOf<RuntimeServices>("UnboxSByte", typeof(object));
			if (type == TypeSystemServices.ShortType) return _typeSystem.MethodOf<RuntimeServices>("UnboxInt16", typeof(object));
			if (type == TypeSystemServices.UShortType) return _typeSystem.MethodOf<RuntimeServices>("UnboxUInt16", typeof(object));
			if (type == TypeSystemServices.IntType) return _typeSystem.MethodOf<RuntimeServices>("UnboxInt32", typeof(object));
			if (type == TypeSystemServices.UIntType) return _typeSystem.MethodOf<RuntimeServices>("UnboxUInt32", typeof(object));
			if (IsLong(type)) return _typeSystem.MethodOf<RuntimeServices>("UnboxInt64", typeof(object));
			if (type == TypeSystemServices.ULongType) return _typeSystem.MethodOf<RuntimeServices>("UnboxUInt64", typeof(object));
			if (type == TypeSystemServices.SingleType) return _typeSystem.MethodOf<RuntimeServices>("UnboxSingle", typeof(object));
			if (type == TypeSystemServices.DoubleType) return _typeSystem.MethodOf<RuntimeServices>("UnboxDouble", typeof(object));
			if (type == TypeSystemServices.DecimalType) return _typeSystem.MethodOf<RuntimeServices>("UnboxDecimal", typeof(object));
			if (type == TypeSystemServices.BoolType) return _typeSystem.MethodOf<RuntimeServices>("UnboxBoolean", typeof(object));
			if (type == TypeSystemServices.CharType) return _typeSystem.MethodOf<RuntimeServices>("UnboxChar", typeof(object));
			return default;
		}

		ILOpCode GetNumericPromotionOpCode(IType type)
		{
			return NumericPromotionOpcodeFor(TypeCodeFor(type), _checked);
		}

		private static ILOpCode NumericPromotionOpcodeFor(TypeCode typeCode, bool @checked) =>
			typeCode switch
			{
				TypeCode.SByte => @checked ? ILOpCode.Conv_ovf_i1 : ILOpCode.Conv_i1,
				TypeCode.Byte => @checked ? ILOpCode.Conv_ovf_u1 : ILOpCode.Conv_u1,
				TypeCode.Int16 => @checked ? ILOpCode.Conv_ovf_i2 : ILOpCode.Conv_i2,
				TypeCode.UInt16 or TypeCode.Char => @checked ? ILOpCode.Conv_ovf_u2 : ILOpCode.Conv_u2,
				TypeCode.Int32 => @checked ? ILOpCode.Conv_ovf_i4 : ILOpCode.Conv_i4,
				TypeCode.UInt32 => @checked ? ILOpCode.Conv_ovf_u4 : ILOpCode.Conv_u4,
				TypeCode.Int64 => @checked ? ILOpCode.Conv_ovf_i8 : ILOpCode.Conv_i8,
				TypeCode.UInt64 => @checked ? ILOpCode.Conv_ovf_u8 : ILOpCode.Conv_u8,
				TypeCode.Single => ILOpCode.Conv_r4,
				TypeCode.Double => ILOpCode.Conv_r8,
				_ => throw new ArgumentException(typeCode.ToString()),
			};

		private static TypeCode TypeCodeFor(IType type)
		{
			if (type is ExternalType externalType)
				return Type.GetTypeCode(externalType.ActualType);
			throw new NotImplementedException(string.Format("TypeCodeFor({0}) not implemented!", type));
		}

		void StoreEntity(ILOpCode opcode, int index, Expression value, IType elementType)
		{
			// array reference
			Dup();
			EmitLoadLiteral(index); // element index

			bool stobj = IsStobj(opcode); // value type sequence?
			if (stobj)
			{
				var systemType = GetSystemType(elementType);
				_il.OpCode(ILOpCode.Ldelema, systemType);
				LoadExpressionWithType(elementType, value); // might need to cast to decimal
				_il.OpCode(opcode, systemType);
			}
			else
			{
				LoadExpressionWithType(elementType, value);
				_il.OpCode(opcode);
			}
		}

		private void Dup()
		{
			_il.OpCode(ILOpCode.Dup);
		}

		static bool IsStobj(ILOpCode code) => ILOpCode.Stobj == code;

		void DefineAssemblyAttributes()
		{
			foreach (Attribute attribute in _assemblyAttributes)
			{
				_typeSystem.SetCustomAttribute(_asmHandle, GetCustomAttributeBuilder(attribute));
			}
		}

		private AttributeBuilder CreateDebuggableAttribute()
		{
			var cb = My<BooCodeBuilder>.Instance;
			var type = typeof(DebuggableAttribute.DebuggingModes);
			var f1 = cb.CreateMemberReference((IMember)TypeSystemServices.Map(type.GetMember("Default").Single()));
			var f2 = cb.CreateMemberReference((IMember)TypeSystemServices.Map(type.GetMember("DisableOptimizations").Single()));
			var expr = cb.CreateBoundBinaryExpression(TypeSystemServices.Map(type), BinaryOperatorType.BitwiseOr, f1, f2);
			var attr = cb.CreateAttribute(TypeSystemServices.Map(DebuggableAttribute_Constructor), expr);
			return new AttributeBuilder(attr, _typeSystem);
		}

		AttributeBuilder CreateRuntimeCompatibilityAttribute()
        {
			var cb = My<BooCodeBuilder>.Instance;
			var attr = cb.CreateAttribute(typeof(System.Runtime.CompilerServices.RuntimeCompatibilityAttribute));
			var prop = TypeSystemServices.Map(RuntimeCompatibilityAttribute_Property);
			attr.NamedArguments.Add(new(cb.CreateReference(prop), cb.CreateBoolLiteral(true)));
			return new AttributeBuilder(attr, _typeSystem);
        }

		AttributeBuilder CreateTargetFrameworkAttribute()
		{
			var cb = My<BooCodeBuilder>.Instance;
			var type = typeof(System.Runtime.Versioning.TargetFrameworkAttribute);
			var s = cb.CreateStringLiteral(".NETCoreApp,Version=v5.0");
			var attr = cb.CreateAttribute(TypeSystemServices.Map(type.GetConstructors().First()), s);
			var prop = TypeSystemServices.Map(type.GetProperty("FrameworkDisplayName"));
			attr.NamedArguments.Add(new(cb.CreateReference(prop), cb.CreateStringLiteral("")));
			return new AttributeBuilder(attr, _typeSystem);
		}

		private AttributeBuilder CreateUnverifiableCodeAttribute()
		{
			var cb = My<BooCodeBuilder>.Instance;
			var attr = cb.CreateAttribute(typeof(UnverifiableCodeAttribute));
			return new AttributeBuilder(attr, _typeSystem);
		}

#if NET
		private static readonly ConstructorInfo EntryPointAttr = typeof(EntryPointAttribute).GetConstructor(Array.Empty<Type>());
		private static readonly ConstructorInfo EntryPointTypeAttr = typeof(EntryPointTypeAttribute).GetConstructor(Array.Empty<Type>());
#endif

		void DefineEntryPoint()
		{
			if (CompilerOutputType.Library != Parameters.OutputType)
			{
				Method method = ContextAnnotations.GetEntryPoint(Context);
				if (method != null)
				{
					var cb = My<BooCodeBuilder>.Instance;
					MethodBuilder entryPoint = GetMethodBuilder(method);
					var ep = cb.CreateAttribute(TypeSystemServices.Map(EntryPointAttr));
					var builder = new AttributeBuilder(ep, _typeSystem);
					_typeSystem.SetCustomAttribute(entryPoint, builder);
					var ept = cb.CreateAttribute(TypeSystemServices.Map(EntryPointTypeAttr));
					var builder2 = new AttributeBuilder(ept, _typeSystem);
					var parentType = GetTypeBuilder(method.GetAncestor<TypeDefinition>());
					_typeSystem.SetCustomAttribute(parentType, builder2);
				}
				else
				{
					Errors.Add(CompilerErrorFactory.NoEntryPoint());
				}
			}
		}

		void DefineModuleConstructor()
		{
			if (_moduleConstructorMethods.Count == 0)
				return;

			var attrs = MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
			//MethodBuilder mb = this._moduleBuilder.DefineGlobalMethod(".cctor", attrs, null, Array.Empty<Type>());
			Method m = CodeBuilder.CreateMethod(".cctor", TypeSystemServices.VoidType, TypeMemberModifiers.Static);
			foreach (var reference in _moduleConstructorMethods.OrderBy(reference => (int)reference["Ordering"]))
				m.Body.Add(CodeBuilder.CreateMethodInvocation((IMethod)reference.Entity));
			var mb = new MethodBuilder((InternalMethod)m.Entity, new MethodBodyStreamEncoder(_ilBlock), attrs, Parameters.Debug, _typeSystem, _moduleInitHandle);
			mb.Build();
		}

		private static IType[] GetParameterTypes(ParameterDeclarationCollection parameters)
		{
			IType[] types = new IType[parameters.Count];
			for (int i = 0; i < types.Length; ++i)
			{
				types[i] = (IType)parameters[i].Type.Entity;
				if (parameters[i].IsByRef && !types[i].IsByRef)
				{
					types[i] = types[i].MakeByRefType();
				}
			}
			return types;
		}

		readonly Dictionary<Node, IBuilder> _builders = new();

		void SetBuilder(Node node, IBuilder builder)
		{
			_builders[node] = builder ?? throw new ArgumentNullException(nameof(builder));
		}

		object GetBuilder(Node node)
		{
			return _builders[node];
		}

		internal TypeBuilder GetTypeBuilder(Node node)
		{
			return (TypeBuilder)_builders[node];
		}

		PropertyBuilder GetPropertyBuilder(Node node)
		{
			return (PropertyBuilder)_builders[node];
		}

		FieldBuilder GetFieldBuilder(Node node)
		{
			return (FieldBuilder)_builders[node];
		}

		MethodBuilder GetMethodBuilder(Method method)
		{
			return (MethodBuilder)_builders[method];
		}

		MethodBuilder GetConstructorBuilder(Method method) => GetMethodBuilder(method);

		int GetLocalVariablePosition(Node local)
		{
			return GetInternalLocal(local).LocalVariableIndex;
		}

		EntityHandle GetFieldInfo(IField tag) => _typeSystem.LookupField(tag);

		EntityHandle GetMethodInfo(IMethod entity) => _typeSystem.LookupMethod(entity);

		EntityHandle GetConstructorInfo(IConstructor entity) => _typeSystem.LookupMethod(entity);

		EntityHandle GetSystemType(Node node) => GetSystemType(GetType(node));

		EntityHandle GetSystemType(IType entity) => _typeSystem.LookupType(entity);

		private static PropertyAttributes PropertyAttributesFor(Property property) => 
			property.ExplicitInfo != null
				? PropertyAttributes.SpecialName | PropertyAttributes.RTSpecialName
				: PropertyAttributes.None;

		private static MethodAttributes PropertyAccessorAttributesFor(TypeMember property)
		{
			const MethodAttributes defaultPropertyAccessorAttributes = MethodAttributes.SpecialName | MethodAttributes.HideBySig;
			return defaultPropertyAccessorAttributes | MethodBuilder.MethodAttributesFor(property);
		}

		static FieldAttributes FieldAttributesFor(Field field)
		{
			var attributes = FieldVisibilityAttributeFor(field);
			if (field.IsStatic)
				attributes |= FieldAttributes.Static;
			if (field.IsTransient)
				attributes |= FieldAttributes.NotSerialized;
			if (field.IsFinal)
			{
				IField entity = (IField)field.Entity;
				if (entity.IsLiteral)
					attributes |= FieldAttributes.Literal;
				else
					attributes |= FieldAttributes.InitOnly;
			}
			return attributes;
		}

		private static FieldAttributes FieldVisibilityAttributeFor(Field field)
		{
			if (field.IsProtected)
				return FieldAttributes.Family;
			if (field.IsPublic)
				return FieldAttributes.Public;
			if (field.IsInternal)
				return FieldAttributes.Assembly;
			return FieldAttributes.Private;
		}

		static readonly Type IsVolatileType = typeof(System.Runtime.CompilerServices.IsVolatile);

		static ParameterAttributes GetParameterAttributes(ParameterDeclaration param) =>
			ParameterAttributes.None;

		void DefineEvent(TypeBuilder typeBuilder, Event node)
		{
			var builder = typeBuilder.DefineEvent((IEvent)node.Entity);
			builder.SetAddOnMethod(DefineEventMethod(typeBuilder, node.Add));
			builder.SetOnRemoveMethod(DefineEventMethod(typeBuilder, node.Remove));
			if (node.Raise != null)
				builder.SetRaiseMethod(DefineEventMethod(typeBuilder, node.Raise));
			SetBuilder(node, builder);
		}

		private MethodBuilder DefineEventMethod(TypeBuilder typeBuilder, Method method)
		{
			return DefineMethod(typeBuilder, method, MethodAttributes.SpecialName | MethodBuilder.GetMethodAttributes((IMethod)method.Entity));
		}

		void DefineProperty(TypeBuilder typeBuilder, Property property)
		{
			var name = property.ExplicitInfo != null
				? property.ExplicitInfo.InterfaceType.Name + "." + property.Name
				: property.Name;

			var propEnt = (IProperty)property.Entity;
			var builder = typeBuilder.DefineProperty(propEnt, PropertyAttributesFor(property));
			var getter = property.Getter;
			if (getter != null)
				builder.SetGetMethod(DefinePropertyAccessor(typeBuilder, property, getter));

			var setter = property.Setter;
			if (setter != null)
				builder.SetSetMethod(DefinePropertyAccessor(typeBuilder, property, setter));

			if (GetEntity(property).IsDuckTyped)
				_typeSystem.SetCustomAttribute(builder, CreateDuckTypedCustomAttribute());

			SetBuilder(property, builder);
		}

		private MethodBuilder DefinePropertyAccessor(TypeBuilder typeBuilder, Property property, Method accessor)
		{
			if (!accessor.IsVisibilitySet)
				accessor.Visibility = property.Visibility;
			return DefineMethod(typeBuilder, accessor, PropertyAccessorAttributesFor(accessor));
		}

		void DefineField(TypeBuilder typeBuilder, Field field)
		{
			if (field.ParentNode.NodeType == NodeType.EnumDefinition)
				return;

			var attrs = FieldAttributesFor(field);
			var constValue = attrs.HasFlag(FieldAttributes.Literal) ? GetInternalFieldStaticValue((InternalField)field.Entity) : null;
			var builder = typeBuilder.DefineField((IField)field.Entity, (IType)field.Type.Entity, attrs, constValue);
			SetBuilder(field, builder);
		}

		delegate ParameterBuilder ParameterFactory(IParameter param, int index, ParameterAttributes attributes);

		private void DefineParameters(MethodBuilder builder, ParameterDeclarationCollection parameters)
		{
			DefineParameters(parameters, builder.DefineParameter);
		}

		private void DefineParameters(ParameterDeclarationCollection parameters, ParameterFactory defineParameter)
		{
			for (int i = 0; i < parameters.Count; ++i)
			{
				ParameterBuilder paramBuilder = defineParameter((IParameter)parameters[i].Entity, i + 1, GetParameterAttributes(parameters[i]));
				if (parameters[i].IsParamArray)
				{
					SetParamArrayAttribute(paramBuilder);
				}
				SetBuilder(parameters[i], paramBuilder);
			}
		}

		void SetParamArrayAttribute(ParameterBuilder builder)
		{
			var cb = My<BooCodeBuilder>.Instance;
			var attr = cb.CreateAttribute(TypeSystemServices.Map(ParamArrayAttribute_Constructor));
			var ab = new AttributeBuilder(attr, _typeSystem);
			_typeSystem.SetCustomAttribute(builder, ab);
		}

		static MethodImplAttributes ImplementationFlagsFor(Method method)
		{
			return method.IsRuntime
				? MethodImplAttributes.Runtime
				: MethodImplAttributes.Managed;
		}

		MethodBuilder DefineMethod(TypeBuilder typeBuilder, Method method, MethodAttributes attributes)
		{
			MethodBuilder builder = typeBuilder.DefineMethod((InternalMethod)method.Entity, attributes);

			if (method.GenericParameters.Count != 0)
			{
				DefineGenericParameters(builder, method.GenericParameters.ToArray());
			}

			DefineParameters(builder, method.Parameters);
			SetBuilder(method, builder);

			if (((IMethod)method.Entity).IsDuckTyped)
			{
				_typeSystem.SetCustomAttribute(builder, CreateDuckTypedCustomAttribute());
			}
			return builder;
		}

		void DefineGenericParameters(TypeDefinition typeDefinition)
		{
			if (typeDefinition is EnumDefinition)
				return;

			TypeBuilder type = GetTypeBuilder(typeDefinition);
			if (type.IsGenericType)
				return; //early-bound, do not redefine generic parameters again

			if (typeDefinition.GenericParameters.Count > 0)
			{
				DefineGenericParameters(type, typeDefinition.GenericParameters.ToArray());
			}
		}

		/// <summary>
		/// Defines the generic parameters of an internal generic type.
		/// </summary>
		void DefineGenericParameters(TypeBuilder builder, GenericParameterDeclaration[] parameters)
		{
			var entities = parameters.Select(p => (IGenericParameter)p.Entity);

			GenericTypeParameterBuilder[] builders = builder.DefineGenericParameters(entities);

			DefineGenericParameters(builders, parameters);
		}

		/// <summary>
		/// Defines the generic parameters of an internal generic method.
		/// </summary>
		void DefineGenericParameters(MethodBuilder builder, GenericParameterDeclaration[] parameters)
		{
			var entities = parameters.Select(p => (IGenericParameter)p.Entity);

            GenericTypeParameterBuilder[] builders = builder.DefineGenericParameters(entities);

			DefineGenericParameters(builders, parameters);
		}

		void DefineGenericParameters(GenericTypeParameterBuilder[] builders, GenericParameterDeclaration[] declarations)
		{
			for (int i = 0; i < builders.Length; i++)
			{
				//SetBuilder(declarations[i], builders[i]);
				DefineGenericParameter((InternalGenericParameter)declarations[i].Entity, builders[i]);
			}
		}

		private void DefineGenericParameter(InternalGenericParameter parameter, GenericTypeParameterBuilder builder)
		{
			// Set base type constraint
			if (parameter.BaseType != TypeSystemServices.ObjectType)
			{
				var baseType = SelfMapTypeIfNeeded(parameter.BaseType);
				builder.SetBaseTypeConstraint(baseType);
			}

			// Set interface constraints
			var interfaceTypes = parameter.GetInterfaces();

			if (interfaceTypes.Length > 0)
			{
				builder.SetInterfaceConstraints(interfaceTypes.Select(SelfMapTypeIfNeeded).ToArray());
			}

			// Set special attributes
			GenericParameterAttributes attributes = GenericParameterAttributes.None;
			if (parameter.IsClass)
				attributes |= GenericParameterAttributes.ReferenceTypeConstraint;
			if (parameter.IsValueType)
				attributes |= GenericParameterAttributes.NotNullableValueTypeConstraint;
			if (parameter.MustHaveDefaultConstructor)
				attributes |= GenericParameterAttributes.DefaultConstructorConstraint;

			builder.SetGenericParameterAttributes(attributes);
		}

		private AttributeBuilder CreateDuckTypedCustomAttribute()
		{
			var cb = My<BooCodeBuilder>.Instance;
			var attr = cb.CreateAttribute(TypeSystemServices.Map(DuckTypedAttribute_Constructor));
			return new AttributeBuilder(attr, _typeSystem);
		}

		private static bool IsEnumDefinition(TypeMember type) => 
			NodeType.EnumDefinition == type.NodeType;

		void DefineType(TypeDefinition typeDefinition)
		{
			SetBuilder(typeDefinition, CreateTypeBuilder(typeDefinition));
		}

		InternalLocal GetInternalLocal(Node local)
		{
			return (InternalLocal)GetEntity(local);
		}

		TypeBuilder CreateTypeBuilder(TypeDefinition type)
		{
			TypeBuilder typeBuilder;

			if (type.ParentNode is ClassDefinition enclosingType)
			{
				typeBuilder = GetTypeBuilder(enclosingType).DefineNestedType(type);
			}
			else
			{
				typeBuilder = new TypeBuilder(type, _ilBlock, _typeSystem, Parameters.Debug);
			}

			return typeBuilder;
		}

		void NotImplemented(string feature)
		{
			throw new NotImplementedException(feature);
		}

		AttributeBuilder GetCustomAttributeBuilder(Attribute node) => new (node, _typeSystem);

		object GetValue(IType expectedType, Expression expression)
		{
			switch (expression.NodeType)
			{
				case NodeType.NullLiteralExpression:
					return null;

				case NodeType.StringLiteralExpression:
					return ((StringLiteralExpression)expression).Value;

				case NodeType.CharLiteralExpression:
					return ((CharLiteralExpression)expression).Value[0];

				case NodeType.BoolLiteralExpression:
					return ((BoolLiteralExpression)expression).Value;

				case NodeType.IntegerLiteralExpression:
					var ile = (IntegerLiteralExpression)expression;
					return ConvertValue(expectedType,
										ile.IsLong ? (object)ile.Value : (int)ile.Value);

				case NodeType.DoubleLiteralExpression:
					return ConvertValue(expectedType,
											((DoubleLiteralExpression)expression).Value);

				case NodeType.TypeofExpression:
					return GetSystemType(((TypeofExpression)expression).Type);

				case NodeType.CastExpression:
					return GetValue(expectedType, ((CastExpression)expression).Target);

				case NodeType.BinaryExpression:
					return GetBinaryValue(expectedType, (BinaryExpression)expression);

				default:
					return GetComplexExpressionValue(expectedType, expression);
			}
		}

        private object GetBinaryValue(IType expectedType, BinaryExpression expression)
        {
            var l = GetValue(expectedType, expression.Left);
			var r = GetValue(expectedType, expression.Right);
			switch (expression.Operator)
            {
				case BinaryOperatorType.BitwiseOr:
					return BitOrValues(l, r);
				default:
					throw new EcmaBuildException($"Unsupported GetBinaryValue operator: {expression.Operator}");
            }
        }

        private static object BitOrValues(object l, object r)
        {
			if (l.GetType() == r.GetType())
			{
				if (l is Enum)
				{
					return Enum.ToObject(l.GetType(), Convert.ToInt64(l) | Convert.ToInt64(r));
				}
			}
			throw new EcmaBuildException($"Unsupported binary operation");
		}

        private object GetComplexExpressionValue(IType expectedType, Expression expression)
		{
			IEntity tag = GetEntity(expression);
			if (EntityType.Type == tag.EntityType)
				return GetSystemType(expression);

			if (EntityType.Field == tag.EntityType)
			{
				IField field = (IField)tag;
				if (field.IsLiteral)
				{
					//Scenario:
					//IF:
					//SomeType.StaticReference = "hamsandwich"
					//[RandomAttribute(SomeType.StaticReferenece)]
					//THEN:
					//field.StaticValue != "hamsandwich"
					//field.StaticValue == SomeType.StaticReference
					//SO:
					//If field.StaticValue is an AST Expression, call GetValue() on it
					if (field.StaticValue is Expression)
						return GetValue(expectedType, field.StaticValue as Expression);
					return field.StaticValue;
				}
			}

			NotImplemented(expression, "Expression value: " + expression);
			return null;
		}

		private static object ConvertValue(IType expectedType, object value)
		{
			if (expectedType.IsEnum)
			{
				return Convert.ChangeType(value, GetEnumUnderlyingType(expectedType));
			}
			return Convert.ChangeType(value, ((ExternalType)expectedType).ActualType);
		}

		private IType GetEnumUnderlyingType(EnumDefinition node) =>
			TypeSystemServices.Map(((InternalEnum)node.Entity).UnderlyingType);

		private static Type GetEnumUnderlyingType(IType enumType) =>
			enumType is IInternalEntity
				? ((InternalEnum)enumType).UnderlyingType
				: Enum.GetUnderlyingType(((ExternalType)enumType).ActualType);

		void DefineTypeMembers(TypeDefinition typeDefinition)
		{
			var typeBuilder = GetTypeBuilder(typeDefinition);
			TypeMemberCollection members = typeDefinition.Members;
			foreach (TypeMember member in members)
			{
				switch (member.NodeType)
				{
					case NodeType.Constructor:
						{
							DefineMethod(typeBuilder, (Method)member, MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
							break;
						}
					case NodeType.Method:
						{
							DefineMethod(typeBuilder, (Method)member, 0);
							break;
						}

					case NodeType.Field:
						{
							DefineField(typeBuilder, (Field)member);
							break;
						}

					case NodeType.Property:
						{
							DefineProperty(typeBuilder, (Property)member);
							break;
						}

					case NodeType.Event:
						{
							DefineEvent(typeBuilder, (Event)member);
							break;
						}
					case NodeType.EnumMember:
                        {
							DefineEnumMember(typeBuilder, (EnumMember)member, (EnumDefinition)typeDefinition);
							break;
                        }
				}
			}
			if (IsEnumDefinition(typeDefinition))
			{
				var enumDef = typeDefinition as EnumDefinition;
				var fld = My<BooCodeBuilder>.Instance.CreateField("value__", GetEnumUnderlyingType(enumDef));
				typeDefinition.Members.Add(fld);
				var fldBuilder = typeBuilder.DefineField((IField)fld.Entity,
								((IField)fld.Entity).Type,
								FieldAttributes.Public |
								FieldAttributes.SpecialName |
								FieldAttributes.RTSpecialName);
				SetBuilder(fld, fldBuilder);
			}
		}

		private void DefineEnumMember(TypeBuilder typeBuilder, EnumMember member, EnumDefinition parent)
        {
			if (member.Entity == null)
			{
				member.Entity = My<InternalTypeSystemProvider>.Instance.EntityFor(member);
			}
			var field = typeBuilder.DefineField(
				(IField)member.Entity,
				GetEnumUnderlyingType(parent),
				ENUM_ATTRIBUTES,
				InitializerValueOf(member, parent));
			SetBuilder(member, field);
		}

		private static string GetAssemblySimpleName(string fname) =>
			Path.GetFileNameWithoutExtension(fname);

		private static string GetTargetDirectory(string fname) =>
			Permissions.WithDiscoveryPermission(() => Path.GetDirectoryName(Path.GetFullPath(fname)));

		string BuildOutputAssemblyName()
		{
			string configuredOutputAssembly = Parameters.OutputAssembly;
			if (!string.IsNullOrEmpty(configuredOutputAssembly))
				return TryToGetFullPath(configuredOutputAssembly);

			string outputAssembly = CompileUnit.Modules[0].Name;
			if (!HasDllOrExeExtension(outputAssembly))
			{
				if (CompilerOutputType.Library == Parameters.OutputType)
					outputAssembly += ".dll";
				else
					outputAssembly += ".exe";
			}
			return TryToGetFullPath(outputAssembly);
		}

		private static string TryToGetFullPath(string path) => Path.GetFullPath(path) ?? path;

		private static bool HasDllOrExeExtension(string fname)
		{
			var extension = Path.GetExtension(fname);
			return extension.ToLower() switch
			{
				".dll" or ".exe" => true,
				_ => false,
			};
		}

		void DefineResources()
		{
			foreach (ICompilerResource resource in Parameters.Resources)
				resource.WriteResource(_sreResourceService);
		}

		SREResourceService _sreResourceService;

		sealed class SREResourceService : IResourceService
		{
			private readonly MetadataBuilder _builder;
			BlobWriter _resBuilder = new();
			private readonly List<IResourceWriter> _resWriters = new();
			private readonly MemoryStream _stream = new();

			public SREResourceService(MetadataBuilder builder)
			{
				_builder = builder;
			}

			public bool EmbedFile(string resourceName, string fname)
			{
				_builder.AddManifestResource(ManifestResourceAttributes.Public, _builder.GetOrAddString(resourceName), default, (uint)_resBuilder.Length);
				_resBuilder.WriteBytes(File.ReadAllBytes(fname));
				return true;
			}

			public IResourceWriter DefineResource(string resourceName, string resourceDescription)
			{
				var result = new ResourceWriter(_stream);
				_resWriters.Add(result);
				return result;
			}
		}

		private void SetUpAssembly()
		{
			var outputFile = BuildOutputAssemblyName();
			var asmName = CreateAssemblyName(outputFile);
			_asmHandle = _asmBuilder.AddAssembly(
				_asmBuilder.GetOrAddString(asmName.Name),
				new Version(1,0,0,0),
				asmName.CultureName != null ? _asmBuilder.GetOrAddString(asmName.CultureName) : default,
				asmName.GetPublicKey()?.Length > 0 ? _asmBuilder.GetOrAddBlob(asmName.GetPublicKey()) : default,
				TypeSystemBridge.ConvertAssemblyNameFlags(asmName.Flags),
				(AssemblyHashAlgorithm)asmName.HashAlgorithm);
			var mcf = new ModuleConstructorFinder();
			CompileUnit.Accept(mcf);
			_moduleInitHandle = (mcf.Found) ? (MethodDefinitionHandle)_typeSystem.ReserveMethod() : MetadataTokens.MethodDefinitionHandle(1);
			var rHandle = _typeSystem.ReserveType();
			_typeModuleHandle = _asmBuilder.AddTypeDefinition(
				default,
				default,
				_asmBuilder.GetOrAddString("<Module>"),
				default,
				MetadataTokens.FieldDefinitionHandle(1),
				_moduleInitHandle);
			if (_typeModuleHandle != rHandle)
            {
				throw new EcmaBuildException("Module type handle does not match expected value");
            }
			_moduleBuilder = _asmBuilder.AddModule(
				0,
				_asmBuilder.GetOrAddString(asmName.Name + ".dll"),
				_asmBuilder.GetOrAddGuid(Guid.NewGuid()),
				default,
				default);

			if (Parameters.Debug)
			{
				// ikvm tip:  Set DebuggableAttribute to assembly before
				// creating the module, to make sure Visual Studio (Whidbey)
				// picks up the attribute when debugging dynamically generated code.
				_typeSystem.SetCustomAttribute(_asmHandle, CreateDebuggableAttribute());
			}

			_typeSystem.SetCustomAttribute(_asmHandle, CreateRuntimeCompatibilityAttribute());
			_typeSystem.SetCustomAttribute(_asmHandle, CreateTargetFrameworkAttribute());

			if (Parameters.Unsafe)
				_typeSystem.SetCustomAttribute(_moduleBuilder, CreateUnverifiableCodeAttribute());

			_sreResourceService = new SREResourceService(_asmBuilder);

			Context.GeneratedAssemblyFileName = outputFile;
		}

		AssemblyName CreateAssemblyName(string outputFile)
		{
            var assemblyName = new AssemblyName
            {
                Name = GetAssemblySimpleName(outputFile),
                Version = GetAssemblyVersion()
            };
			try
			{
				if (Parameters.DelaySign)
					assemblyName.SetPublicKey(GetAssemblyKeyPair(outputFile).PublicKey);
				else
					assemblyName.KeyPair = GetAssemblyKeyPair(outputFile);
			} catch (PlatformNotSupportedException) { }
			return assemblyName;
		}

		StrongNameKeyPair GetAssemblyKeyPair(string outputFile)
		{
			var attribute = GetAssemblyAttribute("System.Reflection.AssemblyKeyNameAttribute");
			if (Parameters.KeyContainer != null)
			{
				if (attribute != null)
					Warnings.Add(CompilerWarningFactory.HaveBothKeyNameAndAttribute(attribute));
				if (Parameters.KeyContainer.Length != 0)
					return new StrongNameKeyPair(Parameters.KeyContainer);
			}
			else if (attribute != null)
			{
				var asmName = ((StringLiteralExpression)attribute.Arguments[0]).Value;
				if (asmName.Length != 0) //ignore empty AssemblyKeyName values, like C# does
					return new StrongNameKeyPair(asmName);
			}

			string fname = null;
			string srcFile = null;
			attribute = GetAssemblyAttribute("System.Reflection.AssemblyKeyFileAttribute");

			if (Parameters.KeyFile != null)
			{
				fname = Parameters.KeyFile;
				if (attribute != null)
					Warnings.Add(CompilerWarningFactory.HaveBothKeyFileAndAttribute(attribute));
			}
			else if (attribute != null)
			{
				fname = ((StringLiteralExpression)attribute.Arguments[0]).Value;
				if (attribute.LexicalInfo != null)
					srcFile = attribute.LexicalInfo.FileName;
			}

			if (!string.IsNullOrEmpty(fname))
			{
				if (!Path.IsPathRooted(fname))
					fname = ResolveRelative(outputFile, srcFile, fname);
				using FileStream stream = File.OpenRead(fname);
				//Parameters.DelaySign is ignored.
				return new StrongNameKeyPair(stream);
			}
			return null;
		}

		static string ResolveRelative(string targetFile, string srcFile, string relativeFile)
		{
			//relative to current directory:
			var fname = Path.GetFullPath(relativeFile);
			if (File.Exists(fname))
				return fname;

			//relative to source file:
			if (srcFile != null)
			{
				fname = ResolveRelativePath(srcFile, relativeFile);
				if (File.Exists(fname))
					return fname;
			}

			//relative to output assembly:
			if (targetFile != null)
				return ResolveRelativePath(targetFile, relativeFile);

			return fname;
		}

		private static string ResolveRelativePath(string srcFile, string relativeFile)
		{
			return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(srcFile), relativeFile));
		}

		Version GetAssemblyVersion()
		{
			var version = GetAssemblyAttributeValue("System.Reflection.AssemblyVersionAttribute");
			if (version == null)
				return new Version();

			/* 1.0.* -- BUILD -- based on days since January 1, 2000
			 * 1.0.0.* -- REVISION -- based on seconds since midnight, January 1, 2000, divided by 2			 *
			 */
			string[] sliced = version.Split('.');
			if (sliced.Length > 2)
			{
				var baseTime = new DateTime(2000, 1, 1);
				var mark = DateTime.Now - baseTime;
				if (sliced[2].StartsWith("*"))
					sliced[2] = Math.Round(mark.TotalDays).ToString();
				if (sliced.Length > 3)
					if (sliced[3].StartsWith("*"))
						sliced[3] = Math.Round(mark.TotalSeconds).ToString();
				version = string.Join(".", sliced);
			}
			return new Version(version);
		}

		string GetAssemblyAttributeValue(string name)
		{
			Attribute attribute = GetAssemblyAttribute(name);
			if (null != attribute)
				return ((StringLiteralExpression)attribute.Arguments[0]).Value;
			return null;
		}

		Attribute GetAssemblyAttribute(string name)
		{
			return _assemblyAttributes.Get(name).FirstOrDefault();
		}

		protected override IType GetExpressionType(Expression node)
		{
			IType type = base.GetExpressionType(node);
			if (TypeSystemServices.IsUnknown(type)) throw CompilerErrorFactory.InvalidNode(node);
			return type;
		}

		private IMethod StringFormat
		{
			get
			{
				if (_stringFormat == null)
					_stringFormat = TypeSystemServices.Map(Methods.Of<string, object, string>(string.Format));
				return _stringFormat;
			}
		}
		static IMethod _stringFormat;
        private UnmanagedResourceWriter _resWriter;
        private MethodDefinitionHandle _moduleInitHandle;
        private TypeDefinitionHandle _typeModuleHandle;
        static readonly Dictionary<IType, IMethod> _Nullable_HasValue = new();
		private IMethod _nullableHasValueBase;
		private ILookup<Module, TypeDefinition> _moduleTypeOrder;

		private IMethod GetNullableHasValue(IType type)
		{
			if (_Nullable_HasValue.TryGetValue(type, out var method))
				return method;
			var constructedType = _nullableHasValueBase.DeclaringType.GenericInfo.ConstructType(type);
			method = (IMethod)constructedType.ConstructedInfo.Map(_nullableHasValueBase);
			_Nullable_HasValue.Add(type, method);
			return method;
		}
	}
}
