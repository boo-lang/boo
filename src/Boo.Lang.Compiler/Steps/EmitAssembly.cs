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
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Security;
using System.Security.Permissions;

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Runtime;
using Attribute = Boo.Lang.Compiler.Ast.Attribute;
using Module = Boo.Lang.Compiler.Ast.Module;
using System.Collections.Generic;

namespace Boo.Lang.Compiler.Steps
{
	sealed class LoopInfo
	{
		public Label BreakLabel;
		
		public Label ContinueLabel;
		
		public int TryBlockDepth;
		
		public LoopInfo(Label breakLabel, Label continueLabel, int tryBlockDepth)
		{
			BreakLabel = breakLabel;
			ContinueLabel = continueLabel;
			TryBlockDepth = tryBlockDepth;
		}
	}
	
	public class EmitAssembly : AbstractVisitorCompilerStep
	{
		static ConstructorInfo DebuggableAttribute_Constructor = typeof(DebuggableAttribute).GetConstructor(new Type[] { Types.Bool, Types.Bool });
		
		static ConstructorInfo RuntimeCompatibilityAttribute_Constructor = typeof(System.Runtime.CompilerServices.RuntimeCompatibilityAttribute).GetConstructor(Type.EmptyTypes);

		static ConstructorInfo SerializableAttribute_Constructor = typeof(SerializableAttribute).GetConstructor(Type.EmptyTypes);

		static PropertyInfo[] RuntimeCompatibilityAttribute_Property = new PropertyInfo[] { typeof(System.Runtime.CompilerServices.RuntimeCompatibilityAttribute).GetProperty("WrapNonExceptionThrows") };

		static ConstructorInfo DuckTypedAttribute_Constructor = Types.DuckTypedAttribute.GetConstructor(Type.EmptyTypes);
		
		static ConstructorInfo ParamArrayAttribute_Constructor = Types.ParamArrayAttribute.GetConstructor(Type.EmptyTypes);
		
		static MethodInfo RuntimeServices_NormalizeArrayIndex = Types.RuntimeServices.GetMethod("NormalizeArrayIndex");
		
		static MethodInfo RuntimeServices_ToBool_Object = Types.RuntimeServices.GetMethod("ToBool", new Type[] { Types.Object });

		static MethodInfo RuntimeServices_ToBool_Decimal = Types.RuntimeServices.GetMethod("ToBool", new Type[] { Types.Decimal });

		static MethodInfo Builtins_ArrayTypedConstructor = Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.Int });

		static MethodInfo Builtins_ArrayGenericConstructor = Types.Builtins.GetMethod("array", new Type[] { Types.Int });

		static MethodInfo Builtins_ArrayTypedCollectionConstructor = Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.ICollection });

		static MethodInfo Array_get_Length = typeof(Array).GetProperty("Length").GetGetMethod();

		static MethodInfo Math_Pow = typeof(Math).GetMethod("Pow");

		static ConstructorInfo List_EmptyConstructor = Types.List.GetConstructor(Type.EmptyTypes);
		
		static ConstructorInfo List_ArrayBoolConstructor = Types.List.GetConstructor(new Type[] { Types.ObjectArray, Types.Bool });
		
		static ConstructorInfo Hash_Constructor = Types.Hash.GetConstructor(Type.EmptyTypes);
		
		static ConstructorInfo Regex_Constructor = typeof(Regex).GetConstructor(new Type[] { Types.String });

		static ConstructorInfo Regex_Constructor_Options = typeof(Regex).GetConstructor(new Type[] { Types.String, typeof(RegexOptions) });

		static MethodInfo Hash_Add = Types.Hash.GetMethod("Add", new Type[] { typeof(object), typeof(object) });
		
		static ConstructorInfo TimeSpan_LongConstructor = Types.TimeSpan.GetConstructor(new Type[] { typeof(long) });
		
		static MethodInfo Type_GetTypeFromHandle = Types.Type.GetMethod("GetTypeFromHandle");

		static MethodInfo String_IsNullOrEmpty = Types.String.GetMethod("IsNullOrEmpty", new Type[] { Types.String });

		static MethodInfo RuntimeHelpers_InitializeArray = typeof(System.Runtime.CompilerServices.RuntimeHelpers).GetMethod("InitializeArray", new Type[] { Types.Array, typeof(System.RuntimeFieldHandle) });


		AssemblyBuilder _asmBuilder;
		
		ModuleBuilder _moduleBuilder;
		
		Hashtable _symbolDocWriters = new Hashtable();
		
		// IL generation state
		ILGenerator _il;
		Method _method;       //current method
		int _returnStatements;//number of return statements in current method
		bool _hasLeaveWithStoredValue;//has a explicit return inside a try block (a `leave')
		bool _returnImplicit; //method ends with an implicit return
		Label _returnLabel;   //label for `ret'
		Label _implicitLabel; //label for implicit return (with default value)
		Label _leaveLabel;    //label to load the stored return value (because of a `leave')
		IType _returnType;
		int _tryBlock; // are we in a try block?
		bool _checked = true;
		bool _rawArrayIndexing = false;
		Hashtable _typeCache = new Hashtable();
		
		// keeps track of types on the IL stack
		Stack _types = new Stack();
		
		Stack _loopInfoStack = new Stack();
		
		AttributeCollection _assemblyAttributes = new AttributeCollection();
		
		LoopInfo _currentLoopInfo;
		
		void EnterLoop(Label breakLabel, Label continueLabel)
		{
			_loopInfoStack.Push(_currentLoopInfo);
			_currentLoopInfo = new LoopInfo(breakLabel, continueLabel, _tryBlock);
		}
		
		bool InTryInLoop()
		{
			return _tryBlock > _currentLoopInfo.TryBlockDepth;
		}
		
		void LeaveLoop()
		{
			_currentLoopInfo = (LoopInfo)_loopInfoStack.Pop();
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
			return (IType)_types.Pop();
		}
		
		IType PeekTypeOnStack()
		{
			return (_types.Count != 0) ? (IType) _types.Peek() : null;
		}

		override public void Run()
		{
			if (Errors.Count > 0)
			{
				return;
			}
			
			GatherAssemblyAttributes();
			SetUpAssembly();
			
			DefineTypes();
			
			DefineResources();
			DefineAssemblyAttributes();
			DefineEntryPoint();

			_moduleBuilder.CreateGlobalFunctions(); //setup global .data
		}
		
		void GatherAssemblyAttributes()
		{
			foreach (Module module in CompileUnit.Modules)
			{
				foreach (Attribute attribute in module.AssemblyAttributes)
				{
					_assemblyAttributes.Add(attribute);
				}
			}
		}
		
		void DefineTypes()
		{
			if (CompileUnit.Modules.Count > 0)
			{
				List types = CollectTypes();
				
				foreach (TypeDefinition type in types)
				{
					DefineType(type);
				}
				
				foreach (TypeDefinition type in types)
				{
					DefineGenericParameters(type);
					DefineTypeMembers(type);
				}
				
				foreach (Module module in CompileUnit.Modules)
				{
					OnModule(module);
				}

				EmitAttributes();
				CreateTypes(types);
			}
		}
		
		sealed class AttributeEmitVisitor : DepthFirstVisitor
		{
			EmitAssembly _emitter;
			
			public AttributeEmitVisitor(EmitAssembly emitter)
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
				_emitter.EmitConstructorAttributes(node);
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
			
			public override void LeaveClassDefinition(ClassDefinition node)
			{
				_emitter.EmitTypeAttributes(node);
			}
			
			public override void LeaveInterfaceDefinition(InterfaceDefinition node)
			{
				_emitter.EmitTypeAttributes(node);
			}
			
			public override void LeaveEnumDefinition(EnumDefinition node)
			{
				_emitter.EmitTypeAttributes(node);
			}
		}
		
		delegate void CustomAttributeSetter(CustomAttributeBuilder attribute);
		
		void EmitAttributes(INodeWithAttributes node, CustomAttributeSetter setCustomAttribute)
		{
			foreach (Attribute attribute in node.Attributes)
			{
				setCustomAttribute(GetCustomAttributeBuilder(attribute));
			}
		}
		
		void EmitPropertyAttributes(Property node)
		{
			PropertyBuilder builder = GetPropertyBuilder(node);
			EmitAttributes(node, builder.SetCustomAttribute);
		}
		
		void EmitParameterAttributes(ParameterDeclaration node)
		{
			ParameterBuilder builder = (ParameterBuilder)GetBuilder(node);
			EmitAttributes(node, builder.SetCustomAttribute);
		}
		
		void EmitEventAttributes(Event node)
		{
			EventBuilder builder = (EventBuilder)GetBuilder(node);
			EmitAttributes(node, builder.SetCustomAttribute);
		}
		
		void EmitConstructorAttributes(Constructor node)
		{
			ConstructorBuilder builder = (ConstructorBuilder)GetBuilder(node);
			EmitAttributes(node, builder.SetCustomAttribute);
		}
		
		void EmitMethodAttributes(Method node)
		{
			MethodBuilder builder = GetMethodBuilder(node);
			EmitAttributes(node, builder.SetCustomAttribute);
		}
		
		void EmitTypeAttributes(TypeDefinition node)
		{
			TypeBuilder builder = GetTypeBuilder(node);
			EmitAttributes(node, builder.SetCustomAttribute);
		}

		void EmitTypeAttributes(EnumDefinition node)
		{
			EnumBuilder builder = GetBuilder(node) as EnumBuilder;
			if (null != builder)
			{
				EmitAttributes(node, builder.SetCustomAttribute);
			}
			else //nested enum
			{
				TypeBuilder typeBuilder = GetTypeBuilder(node);
				EmitAttributes(node, typeBuilder.SetCustomAttribute);
			}
		}

		void EmitFieldAttributes(TypeMember node)
		{
			FieldBuilder builder = GetFieldBuilder(node);
			EmitAttributes(node, new CustomAttributeSetter(builder.SetCustomAttribute));
		}
		
		void EmitAttributes()
		{
			AttributeEmitVisitor visitor = new AttributeEmitVisitor(this);
			foreach (Module module in CompileUnit.Modules)
			{
				module.Accept(visitor);
			}
		}
		
		void CreateTypes(List types)
		{
			new TypeCreator(this, types).Run();
		}
		
		/// <summary>
		/// Ensures that all types are created in the correct order.
		/// </summary>
		sealed class TypeCreator
		{
			EmitAssembly _emitter;
			
			Hashtable _created;
			
			List _types;
			
			TypeMember _current;
			
			public TypeCreator(EmitAssembly emitter, List types)
			{
				_emitter = emitter;
				_types = types;
				_created = new Hashtable();
			}
			
			public void Run()
			{
				ResolveEventHandler resolveHandler = new ResolveEventHandler(OnTypeResolve);
				AppDomain current = Thread.GetDomain();
				
				try
				{
					current.TypeResolve += resolveHandler;
					CreateTypes();
				}
				finally
				{
					current.TypeResolve -= resolveHandler;
				}
			}
			
			void CreateTypes()
			{
				foreach (TypeMember type in _types)
				{
					CreateType(type);
				}
			}
			
			void CreateType(TypeMember type)
			{
				if (!_created.ContainsKey(type))
				{
					TypeMember saved = _current;
					_current = type;
					
					_created.Add(type, type);
					
					Trace("creating type '{0}'", type);
					
					if (IsNestedType(type))
					{
						CreateType((TypeMember)type.ParentNode);
					}
					
					TypeDefinition typedef = type as TypeDefinition;
					if (null != typedef)
					{
						CreateRelatedTypes(typedef);
					}

					TypeBuilder typeBuilder = _emitter.GetBuilder(type) as TypeBuilder;
					if (null != typeBuilder)
					{
						typeBuilder.CreateType();
					}
					else
					{
						EnumBuilder enumBuilder = (EnumBuilder) _emitter.GetBuilder(type);
						enumBuilder.CreateType();
					}

					Trace("type '{0}' successfully created", type);
					
					_current = saved;
				}
			}

			private void CreateRelatedTypes(TypeDefinition typedef)
			{
				CreateRelatedTypes(typedef.BaseTypes);
				
				foreach (GenericParameterDeclaration gpd in typedef.GenericParameters)
				{
					CreateRelatedTypes(gpd.BaseTypes);
				}
			}

			private void CreateRelatedTypes(TypeReferenceCollection typerefs)
			{
				foreach (TypeReference typeref in typerefs)
				{
					IType type = _emitter.GetType(typeref);
					EnsureInternalDependency(type);
				}
			}
			
			private void EnsureInternalDependency(IType type)
			{
				AbstractInternalType internalType = type as AbstractInternalType;
				if (null != internalType)
				{
					CreateType(internalType.TypeDefinition);
					return;
				}

				if (type.ConstructedInfo != null)
				{
					EnsureInternalDependency(type.ConstructedInfo.GenericDefinition);
					foreach (IType typeArg in type.ConstructedInfo.GenericArguments)
					{
						EnsureInternalDependency(typeArg);
					}
				}
			}
			
			bool IsNestedType(TypeMember type)
			{
				NodeType parent = type.ParentNode.NodeType;
				return (NodeType.ClassDefinition == parent) ||
					(NodeType.InterfaceDefinition == parent);
			}
			
			Assembly OnTypeResolve(object sender, ResolveEventArgs args)
			{
				Trace("OnTypeResolve('{0}') during '{1}' creation.", args.Name, _current);
				
				// TypeResolve is generated whenever a type
				// contains fields of a value type not created yet.
				// All we need to do is look for value type fields
				// and create them all.
				ClassDefinition classdef = _current as ClassDefinition;
				foreach (TypeMember member in classdef.Members)
				{
					if (NodeType.Field == member.NodeType)
					{
						AbstractInternalType type = _emitter.GetType(((Field)member).Type) as AbstractInternalType;
						if (type != null && type.IsValueType)
						{
							CreateType(type.TypeDefinition);
						}
					}
				}

				return _emitter._asmBuilder;
			}
			
			void Trace(string format, params object[] args)
			{
				_emitter.Context.TraceVerbose(format, args);
			}
		}
		
		List CollectTypes()
		{
			List types = new List();
			foreach (Module module in CompileUnit.Modules)
			{
				CollectTypes(types, module.Members);
			}
			return types;
		}
		
		void CollectTypes(List types, TypeMemberCollection members)
		{
			foreach (TypeMember member in members)
			{
				switch (member.NodeType)
				{
					case NodeType.InterfaceDefinition:
					case NodeType.ClassDefinition:
						{
							types.Add(member);
							CollectTypes(types, ((TypeDefinition)member).Members);
							break;
						}
					case NodeType.EnumDefinition:
						{
							types.Add(member);
							break;
						}
				}
			}
		}
		
		override public void Dispose()
		{
			base.Dispose();
			
			_asmBuilder = null;
			_moduleBuilder = null;
			_symbolDocWriters.Clear();
			_il = null;
			_returnStatements = 0;
			_hasLeaveWithStoredValue = false;
			_returnImplicit = false;
			_returnType = null;
			_tryBlock = 0;
			_checked = true;
			_rawArrayIndexing = false;
			_types.Clear();
			_typeCache.Clear();
			_builders.Clear();
			_assemblyAttributes.Clear();
			_defaultValueHolders.Clear();
			_packedArrays.Clear();
		}
		
		override public void OnAttribute(Attribute node)
		{
		}
		
		override public void OnModule(Module module)
		{
			Visit(module.Members);
		}

		override public void OnEnumDefinition(EnumDefinition node)
		{
			EnumBuilder builder = GetBuilder(node) as EnumBuilder;
			if (null != builder)
			{
				foreach (EnumMember member in node.Members)
				{
					FieldBuilder field = builder.DefineLiteral(member.Name,
						Convert.ChangeType(((IntegerLiteralExpression) member.Initializer).Value,
							GetEnumUnderlyingType(node)));
					SetBuilder(member, field);
				}
			}
			else //nested enum (have to go through regular TypeBuilder
			{    //since there is no DefineNestedEnum in SRE :-/
				TypeBuilder typeBuilder = GetTypeBuilder(node);
				foreach (EnumMember member in node.Members)
				{
					FieldBuilder field = typeBuilder.DefineField(member.Name, typeBuilder,
														FieldAttributes.Public |
														FieldAttributes.Static |
														FieldAttributes.Literal);
					field.SetConstant(Convert.ChangeType(((IntegerLiteralExpression) member.Initializer).Value,
						GetEnumUnderlyingType(node)));
					SetBuilder(member, field);
				}
			}
		}
		
		override public void OnArrayTypeReference(ArrayTypeReference node)
		{
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{
			EmitTypeDefinition(node);
		}
		
		override public void OnField(Field node)
		{
			FieldBuilder builder = GetFieldBuilder(node);
			if (builder.IsLiteral)
			{
				builder.SetConstant(GetInternalFieldStaticValue((InternalField)node.Entity));
			}
		}
		
		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{
			TypeBuilder builder = GetTypeBuilder(node);
			foreach (TypeReference baseType in node.BaseTypes)
			{
				builder.AddInterfaceImplementation(GetSystemType(baseType));
			}
		}
		
		override public void OnMacroStatement(MacroStatement node)
		{
			NotImplemented(node, "Unexpected macro: " + node.ToCodeString());
		}
		
		override public void OnCallableDefinition(CallableDefinition node)
		{
			NotImplemented(node, "Unexpected callable definition!");
		}
		
		void EmitTypeDefinition(TypeDefinition node)
		{
			TypeBuilder current = GetTypeBuilder(node);
			EmitBaseTypesAndAttributes(node, current);
			Visit(node.Members);
		}
		
		override public void OnMethod(Method method)
		{
			if (method.IsRuntime) return;
			if (IsPInvoke(method)) return;
			
			MethodBuilder methodBuilder = GetMethodBuilder(method);
			DefineExplicitImplementationInfo(method);

			EmitMethod(method, methodBuilder.GetILGenerator());
		}

		private void DefineExplicitImplementationInfo(Method method)
		{
			if (null == method.ExplicitInfo)
				return;

			IMethod ifaceMethod = (IMethod)method.ExplicitInfo.Entity;
			MethodInfo ifaceInfo = GetMethodInfo(ifaceMethod);
			MethodInfo implInfo = GetMethodInfo((IMethod)method.Entity);

			TypeBuilder typeBuilder = GetTypeBuilder(method.DeclaringType);
			typeBuilder.DefineMethodOverride(implInfo, ifaceInfo);
		}

		void EmitMethod(Method method, ILGenerator generator)
		{
			_il = generator;
			_method = method;

			DefineLabels(method);
			Visit(method.Locals);

			BeginMethodBody(GetEntity(method).ReturnType);
			Visit(method.Body);
			EndMethodBody(method);
		}

		void BeginMethodBody(IType returnType)
		{
			_defaultValueHolders.Clear();

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
					_il.Emit(OpCodes.Br_S, _returnLabel);

				//load default return value for implicit return
				_il.MarkLabel(_implicitLabel);
				EmitDefaultValue(_returnType);
				PopType();
			}

			if (_hasLeaveWithStoredValue)
			{
				if (hasDefaultValueReturn || _returnStatements == -1)
					_il.Emit(OpCodes.Br_S, _returnLabel);

				//load the stored return value and `ret'
				_il.MarkLabel(_leaveLabel);
				_il.Emit(OpCodes.Ldloc, GetDefaultValueHolder(_returnType));
			}

			if (_returnImplicit || _returnStatements != 0)
			{
				_il.MarkLabel(_returnLabel);
				_il.Emit(OpCodes.Ret);
			}
		}

		private bool IsPInvoke(Method method)
		{
			return GetEntity(method).IsPInvoke;
		}

		override public void OnBlock(Block block)
		{
			bool currentChecked = _checked;
			_checked = AstAnnotations.IsChecked(block, Parameters.Checked);
			
			bool currentArrayIndexing = _rawArrayIndexing;
			_rawArrayIndexing = AstAnnotations.IsRawIndexing(block);

			Visit(block.Statements);

			_rawArrayIndexing = currentArrayIndexing;
			_checked = currentChecked;
		}

		void DefineLabels(Method method)
		{
			foreach (InternalLabel label in ((InternalMethod)method.Entity).Labels)
			{
				label.Label = _il.DefineLabel();
			}
		}
		
		override public void OnConstructor(Constructor constructor)
		{
			if (constructor.IsRuntime) return;
			
			ConstructorBuilder builder = GetConstructorBuilder(constructor);
			EmitMethod(constructor, builder.GetILGenerator());
		}
		
		override public void OnLocal(Local local)
		{
			InternalLocal info = GetInternalLocal(local);
			info.LocalBuilder = _il.DeclareLocal(GetSystemType(local), info.Type.IsPointer);
			if (Parameters.Debug)
			{
				info.LocalBuilder.SetLocalSymInfo(local.Name);
			}
		}
		
		override public void OnForStatement(ForStatement node)
		{
			NotImplemented("ForStatement");
		}
		
		override public void OnReturnStatement(ReturnStatement node)
		{
			EmitDebugInfo(node);
			OpCode retOpCode = _tryBlock > 0 ? OpCodes.Leave : OpCodes.Br;
			Label label = _returnLabel;

			if (null != node.Expression)
			{
				++_returnStatements;

				Visit(node.Expression);
				EmitCastIfNeeded(_returnType, PopType());

				if (retOpCode == OpCodes.Leave)
				{
					//`leave' clears the stack, so we have to store return value temporarily
					//we can use a default value holder for that since it won't be read afterwards
					//of course this is necessary only if return type is not void
					LocalBuilder temp = GetDefaultValueHolder(_returnType);
					_il.Emit(OpCodes.Stloc, temp);
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
				_il.Emit(retOpCode, label);
			else if (null != node.Expression)
				_returnStatements = -1; //instruct epilogue to branch last ret only if necessary
		}
		
		override public void OnRaiseStatement(RaiseStatement node)
		{
			EmitDebugInfo(node);
			if (node.Exception == null)
			{
				_il.Emit(OpCodes.Rethrow);
			}
			else
			{
				Visit(node.Exception); PopType();
				_il.Emit(OpCodes.Throw);
			}
		}

		override public void OnTryStatement(TryStatement node)
		{
			++_tryBlock;
			Label end = _il.BeginExceptionBlock();
			
			// The fault handler isn't very well supported by the
			// the ILGenerator. Thus, when there is a failure block
			// in the same try as an except or ensure block, we
			// need to do some special brute forcing with the exception
			// block context in the ILGenerator.
			if(null != node.FailureBlock && null != node.EnsureBlock)
			{
				++_tryBlock;
				_il.BeginExceptionBlock();
			}

			if(null != node.FailureBlock && node.ExceptionHandlers.Count > 0)
			{
				++_tryBlock;
				_il.BeginExceptionBlock();
			}

			Visit(node.ProtectedBlock);
						
			Visit(node.ExceptionHandlers);

			if(null != node.FailureBlock)
			{
				// Logic to back out of the manually forced blocks
				if(node.ExceptionHandlers.Count > 0)
				{
					_il.EndExceptionBlock();
					--_tryBlock;
				}
				
				_il.BeginFaultBlock();
				Visit(node.FailureBlock);
				
				// Logic to back out of the manually forced blocks once more
				if(null != node.EnsureBlock)
				{
					_il.EndExceptionBlock();
					--_tryBlock;
				}
			}
			
			if (null != node.EnsureBlock)
			{
				_il.BeginFinallyBlock();
				Visit(node.EnsureBlock);
			}

			_il.EndExceptionBlock();
			--_tryBlock;
		}
		
		override public void OnExceptionHandler(ExceptionHandler node)
		{
			if((node.Flags & ExceptionHandlerFlags.Filter) == ExceptionHandlerFlags.Filter)
			{
				_il.BeginExceptFilterBlock();
				
				Label endLabel = _il.DefineLabel();
			
				// If the filter is not untyped, then test the exception type
				// before testing the filter condition
				if((node.Flags & ExceptionHandlerFlags.Untyped) == ExceptionHandlerFlags.None)
				{
					Label filterCondition = _il.DefineLabel();
					
					// Test the type of the exception.
					_il.Emit(OpCodes.Isinst, GetSystemType(node.Declaration.Type));
					
					// Duplicate it.  If it is null, then it will be used to
					// skip the filter.
					_il.Emit(OpCodes.Dup);

					// If the exception is of the right type, branch
					// to test the filter condition.
					_il.Emit(OpCodes.Brtrue_S, filterCondition);

					// Otherwise, clean up the stack and prepare the stack
					// to skip the filter.
					EmitStoreOrPopException(node);
					
					_il.Emit(OpCodes.Ldc_I4_0);
					_il.Emit(OpCodes.Br, endLabel);
					_il.MarkLabel(filterCondition);

				}
				else if((node.Flags & ExceptionHandlerFlags.Anonymous) == ExceptionHandlerFlags.None)
				{
					// Cast the exception to the default except type
					_il.Emit(OpCodes.Isinst, GetSystemType(node.Declaration.Type));
				}
				
				EmitStoreOrPopException(node);
				
				// Test the condition and convert to boolean if needed.
				node.FilterCondition.Accept(this);
				PopType();
				EmitToBoolIfNeeded(node.FilterCondition);
				
				// If the type is right and the condition is true,
				// proceed with the handler.
				_il.MarkLabel(endLabel);
				_il.Emit(OpCodes.Ldc_I4_0);
				_il.Emit(OpCodes.Cgt_Un);
			
				_il.BeginCatchBlock(null);
			}
			else
			{
				// Begin a normal catch block of the appropriate type.
				_il.BeginCatchBlock(GetSystemType(node.Declaration.Type));
				
				// Clean up the stack or store the exception if not anonymous.
				EmitStoreOrPopException(node);
			}

			Visit(node.Block);
		}
		
		private void EmitStoreOrPopException(ExceptionHandler node)
		{
			if((node.Flags & ExceptionHandlerFlags.Anonymous) == ExceptionHandlerFlags.None)
			{
				_il.Emit(OpCodes.Stloc, GetLocalBuilder(node.Declaration));
			}
			else
			{
				_il.Emit(OpCodes.Pop);
			}
		}
				
		override public void OnUnpackStatement(UnpackStatement node)
		{
			NotImplemented("Unpacking");
		}
		
		override public bool EnterExpressionStatement(ExpressionStatement node)
		{
			EmitDebugInfo(node);
			return true;
		}
		
		override public void LeaveExpressionStatement(ExpressionStatement node)
		{
			// if the type of the inner expression is not
			// void we need to pop its return value to leave
			// the stack sane
			DiscardValueOnStack();
		}
		
		void DiscardValueOnStack()
		{
			if (!IsVoid(PopType()))
				_il.Emit(OpCodes.Pop);
		}

		bool IsVoid(IType type)
		{
			return type == TypeSystemServices.VoidType;
		}

		override public void OnUnlessStatement(UnlessStatement node)
		{
			Label endLabel = _il.DefineLabel();
			EmitDebugInfo(node);
			EmitBranchTrue(node.Condition, endLabel);
			node.Block.Accept(this);
			_il.MarkLabel(endLabel);
		}
		
		void OnSwitch(MethodInvocationExpression node)
		{
			ExpressionCollection args = node.Arguments;
			Visit(args[0]);
			EmitCastIfNeeded(TypeSystemServices.IntType, PopType());
			
			Label[] labels = new Label[args.Count-1];
			for (int i=0; i<labels.Length; ++i)
			{
				labels[i] = ((InternalLabel)args[i+1].Entity).Label;
			}
			_il.Emit(OpCodes.Switch, labels);
			
			PushVoid();
		}
		
		override public void OnGotoStatement(GotoStatement node)
		{
			EmitDebugInfo(node);
			
			InternalLabel label = (InternalLabel)GetEntity(node.Label);
			int gotoDepth = AstAnnotations.GetTryBlockDepth(node);
			int targetDepth = AstAnnotations.GetTryBlockDepth(label.LabelStatement);
			
			if (targetDepth == gotoDepth)
			{
				_il.Emit(OpCodes.Br, label.Label);
			}
			else
			{
				_il.Emit(OpCodes.Leave, label.Label);
			}
		}
		
		override public void OnLabelStatement(LabelStatement node)
		{
			EmitDebugInfo(node);
			_il.MarkLabel(((InternalLabel)node.Entity).Label);
		}
		
		override public void OnConditionalExpression(ConditionalExpression node)
		{
			IType type = GetExpressionType(node);
			
			Label endLabel = _il.DefineLabel();
			
			EmitBranchFalse(node.Condition, endLabel);
			node.TrueValue.Accept(this);
			EmitCastIfNeeded(type, PopType());
			
			Label elseEndLabel = _il.DefineLabel();
			_il.Emit(OpCodes.Br, elseEndLabel);
			_il.MarkLabel(endLabel);
			
			endLabel = elseEndLabel;
			node.FalseValue.Accept(this);
			EmitCastIfNeeded(type, PopType());
			
			_il.MarkLabel(endLabel);
			
			PushType(type);
		}
		
		override public void OnIfStatement(IfStatement node)
		{
			Label endLabel = _il.DefineLabel();
			
			EmitDebugInfo(node);
			EmitBranchFalse(node.Condition, endLabel);
			
			node.TrueBlock.Accept(this);
			if (null != node.FalseBlock)
			{
				Label elseEndLabel = _il.DefineLabel();
				if (!node.TrueBlock.EndsWith<ReturnStatement>() && !node.TrueBlock.EndsWith<RaiseStatement>())
					_il.Emit(OpCodes.Br, elseEndLabel);
				_il.MarkLabel(endLabel);
				
				endLabel = elseEndLabel;
				node.FalseBlock.Accept(this);
			}
			
			_il.MarkLabel(endLabel);
		}


		void EmitBranchTrue(Expression expression, Label label)
		{
			EmitBranch(true, expression, label);
		}

		void EmitBranchFalse(Expression expression, Label label)
		{
			EmitBranch(false, expression, label);
		}

		void EmitBranch(bool branch, BinaryExpression expression, Label label)
		{
			switch (expression.Operator)
			{
				case BinaryOperatorType.TypeTest:
					EmitTypeTest(expression);
					_il.Emit(branch ? OpCodes.Brtrue : OpCodes.Brfalse, label);
					break;

				case BinaryOperatorType.Or:
					if (branch)
					{
						EmitBranch(true, expression.Left, label);
						EmitBranch(true, expression.Right, label);
					}
					else
					{
						Label skipRhs = _il.DefineLabel();
						EmitBranch(true, expression.Left, skipRhs);
						EmitBranch(false, expression.Right, label);
						_il.MarkLabel(skipRhs);
					}
					break;

				case BinaryOperatorType.And:
					if (branch)
					{
						Label skipRhs = _il.DefineLabel();
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
					{
						EmitBranch(!branch, expression.Right, label);
					}
					else if (IsZeroEquivalent(expression.Right))
					{
						EmitBranch(!branch, expression.Left, label);
					}
					else
					{
						LoadCmpOperands(expression);
						_il.Emit(branch ? OpCodes.Beq : OpCodes.Bne_Un, label);
					}
					break;

				case BinaryOperatorType.Inequality:
					if (IsZeroEquivalent(expression.Left))
					{
						EmitBranch(branch, expression.Right, label);
					}
					else if (IsZeroEquivalent(expression.Right))
					{
						EmitBranch(branch, expression.Left, label);
					}
					else
					{
						LoadCmpOperands(expression);
						_il.Emit(branch ? OpCodes.Bne_Un : OpCodes.Beq, label);
					}
					break;

				case BinaryOperatorType.ReferenceEquality:
					if (IsNull(expression.Left))
					{
						EmitRawBranch(!branch, expression.Right, label);
						break;
					}
					if (IsNull(expression.Right))
					{
						EmitRawBranch(!branch, expression.Left, label);
						break;
					}
					Visit(expression.Left); PopType();
					Visit(expression.Right); PopType();
					_il.Emit(branch ? OpCodes.Beq : OpCodes.Bne_Un, label);
					break;

				case BinaryOperatorType.ReferenceInequality:
					if (IsNull(expression.Left))
					{
						EmitRawBranch(branch, expression.Right, label);
						break;
					}
					if (IsNull(expression.Right))
					{
						EmitRawBranch(branch, expression.Left, label);
						break;
					}
					Visit(expression.Left); PopType();
					Visit(expression.Right); PopType();
					_il.Emit(branch ? OpCodes.Bne_Un : OpCodes.Beq, label);
					break;

				case BinaryOperatorType.GreaterThan:
					LoadCmpOperands(expression);
					_il.Emit(branch ? OpCodes.Bgt : OpCodes.Ble, label);
					break;

				case BinaryOperatorType.GreaterThanOrEqual:
					LoadCmpOperands(expression);
					_il.Emit(branch ? OpCodes.Bge : OpCodes.Blt, label);
					break;

				case BinaryOperatorType.LessThan:
					LoadCmpOperands(expression);
					_il.Emit(branch ? OpCodes.Blt : OpCodes.Bge, label);
					break;

				case BinaryOperatorType.LessThanOrEqual:
					LoadCmpOperands(expression);
					_il.Emit(branch ? OpCodes.Ble : OpCodes.Bgt, label);
					break;

				default:
					EmitDefaultBranch(branch, expression, label);
					break;
			}
		}

		void EmitBranch(bool branch, UnaryExpression expression, Label label)
		{
			if (UnaryOperatorType.LogicalNot == expression.Operator)
			{
				EmitBranch(!branch, expression.Operand, label);
			}
			else
			{
				EmitDefaultBranch(branch, expression, label);
			}
		}

		void EmitBranch(bool branch, Expression expression, Label label)
		{
			switch (expression.NodeType)
			{
				case NodeType.BinaryExpression:
					{
						EmitBranch(branch, (BinaryExpression)expression, label);
						break;
					}

				case NodeType.UnaryExpression:
					{
						EmitBranch(branch, (UnaryExpression)expression, label);
						break;
					}

				default:
					{
						EmitDefaultBranch(branch, expression, label);
						break;
					}
			}
		}

		void EmitRawBranch(bool branch, Expression expression, Label label)
		{
			expression.Accept(this); PopType();
			_il.Emit(branch ? OpCodes.Brtrue : OpCodes.Brfalse, label);
		}

		void EmitDefaultBranch(bool branch, Expression expression, Label label)
		{
			expression.Accept(this);
			IType type = PopType();
			if (TypeSystemServices.DoubleType == type)
			{
				EmitDefaultValue(TypeSystemServices.DoubleType);
				_il.Emit(branch ? OpCodes.Bne_Un : OpCodes.Beq, label);
			}
			else if (TypeSystemServices.SingleType == type)
			{
				EmitDefaultValue(TypeSystemServices.SingleType);
				_il.Emit(branch ? OpCodes.Bne_Un : OpCodes.Beq, label);
			}
			else
			{
				EmitToBoolIfNeeded(expression);
				_il.Emit(branch ? OpCodes.Brtrue : OpCodes.Brfalse, label);
			}
		}


		private bool IsZeroEquivalent(Expression expression)
		{
			return (IsNull(expression) || IsZero(expression) || IsFalse(expression));
		}

		private bool IsNull(Expression expression)
		{
			return NodeType.NullLiteralExpression == expression.NodeType;
		}

		private bool IsFalse(Expression expression)
		{
			return NodeType.BoolLiteralExpression == expression.NodeType
				&& (false == ((BoolLiteralExpression)expression).Value);
		}

		private bool IsZero(Expression expression)
		{
			return (NodeType.IntegerLiteralExpression == expression.NodeType
			        && (0 == ((IntegerLiteralExpression)expression).Value))
			       ||
			       (NodeType.DoubleLiteralExpression == expression.NodeType
			        && (0 == ((DoubleLiteralExpression)expression).Value));
		}

		override public void OnBreakStatement(BreakStatement node)
		{
			EmitDebugInfo(node);
			if (InTryInLoop())
			{
				_il.Emit(OpCodes.Leave, _currentLoopInfo.BreakLabel);
			}
			else
			{
				_il.Emit(OpCodes.Br, _currentLoopInfo.BreakLabel);
			}
		}
		
		override public void OnContinueStatement(ContinueStatement node)
		{
			EmitDebugInfo(node);
			if (InTryInLoop())
			{
				_il.Emit(OpCodes.Leave, _currentLoopInfo.ContinueLabel);
			}
			else
			{
				_il.Emit(OpCodes.Br, _currentLoopInfo.ContinueLabel);
			}
		}
		
		override public void OnWhileStatement(WhileStatement node)
		{
			Label endLabel = _il.DefineLabel();
			Label bodyLabel = _il.DefineLabel();
			Label thenLabel = _il.DefineLabel();
			Label conditionLabel = _il.DefineLabel();
			LocalBuilder enteredLoop = null;
			
			if(null != node.OrBlock)
			{
				enteredLoop = _il.DeclareLocal(typeof(bool));
				_il.Emit(OpCodes.Ldc_I4_0);
				_il.Emit(OpCodes.Stloc, enteredLoop);
			}
			
			_il.Emit(OpCodes.Br, conditionLabel);
			_il.MarkLabel(bodyLabel);
			
			EnterLoop(endLabel, conditionLabel);
			if(null != node.OrBlock)
			{
				_il.Emit(OpCodes.Ldc_I4_1);
				_il.Emit(OpCodes.Stloc, enteredLoop);
			}
			node.Block.Accept(this);
			LeaveLoop();
			
			_il.MarkLabel(conditionLabel);
			EmitDebugInfo(node);
			EmitBranchTrue(node.Condition, bodyLabel);
			if(null != node.OrBlock)
			{
				_il.Emit(OpCodes.Ldloc, enteredLoop);
				_il.Emit(OpCodes.Brtrue, thenLabel);
				EnterLoop(endLabel, thenLabel);
				node.OrBlock.Accept(this);
				LeaveLoop();
				_il.MarkLabel(thenLabel);
			}
			if(null != node.ThenBlock)
			{
				node.ThenBlock.Accept(this);
			}
			_il.MarkLabel(endLabel);
		}
		
		void EmitIntNot()
		{
			_il.Emit(OpCodes.Ldc_I4_0);
			_il.Emit(OpCodes.Ceq);
		}
		
		void EmitGenericNot()
		{
			// bool codification:
			// value_on_stack ? 0 : 1
			Label wasTrue = _il.DefineLabel();
			Label wasFalse = _il.DefineLabel();
			_il.Emit(OpCodes.Brfalse_S, wasFalse);
			_il.Emit(OpCodes.Ldc_I4_0);
			_il.Emit(OpCodes.Br_S, wasTrue);
			_il.MarkLabel(wasFalse);
			_il.Emit(OpCodes.Ldc_I4_1);
			_il.MarkLabel(wasTrue);
		}
		
		override public void OnUnaryExpression(UnaryExpression node)
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
			bool isGenericParameter = GenericsServices.IsGenericParameter(type);

			if (!type.IsValueType && !isGenericParameter)
			{
				_il.Emit(OpCodes.Ldnull);
			}
			else if (type == TypeSystemServices.BoolType)
			{
				_il.Emit(OpCodes.Ldc_I4_0);
			}
			else if (TypeSystemServices.IsPrimitiveNumber(type) || type == TypeSystemServices.CharType)
			{
				EmitLoadLiteral(type, 0);
			}
			else if (isGenericParameter && TypeSystemServices.IsReferenceType(type))
			{
				_il.Emit(OpCodes.Ldnull);
				_il.Emit(OpCodes.Unbox_Any, GetSystemType(type));
			}
			else //valuetype or valuetype/unconstrained generic parameter
			{
				//TODO: if MethodBody.InitLocals is false
				//_il.Emit(OpCodes.Ldloca, GetDefaultValueHolder(type));
				//_il.Emit(OpCodes.Initobj, GetSystemType(type));
				_il.Emit(OpCodes.Ldloc, GetDefaultValueHolder(type));
			}

			PushType(type);
		}

		Dictionary<IType, LocalBuilder> _defaultValueHolders = new Dictionary<IType, LocalBuilder>();

		//get the default value holder (a local actually) for a given type
		//default value holder pool is cleared before each method body processing
		LocalBuilder GetDefaultValueHolder(IType type)
		{
			LocalBuilder holder;
			if (_defaultValueHolders.TryGetValue(type, out holder))
				return holder;

			holder = _il.DeclareLocal(GetSystemType(type));
			_defaultValueHolders.Add(type, holder);
			return holder;
		}


		private void EmitOnesComplement(UnaryExpression node)
		{
			node.Operand.Accept(this);
			_il.Emit(OpCodes.Not);
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
			IType operandType = GetExpressionType(node.Operand);
			if (!_checked || !TypeSystemServices.IsIntegerNumber(operandType))
			{
				//a single/double unary negation never overflow
				node.Operand.Accept(this);
				_il.Emit(OpCodes.Neg);
			}
			else
			{
				_il.Emit(OpCodes.Ldc_I4_0);
				if (operandType == TypeSystemServices.LongType || operandType == TypeSystemServices.ULongType)
					_il.Emit(OpCodes.Conv_I8);
				node.Operand.Accept(this);
				_il.Emit(TypeSystemServices.IsSignedNumber(operandType)
				         ? OpCodes.Sub_Ovf : OpCodes.Sub_Ovf_Un);
				if (operandType != TypeSystemServices.LongType && operandType != TypeSystemServices.ULongType)
					EmitCastIfNeeded(operandType, TypeSystemServices.IntType);
			}
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
				IType et = PeekTypeOnStack().GetElementType();
				OpCode code = GetLoadRefParamCode(et);
				if (code == OpCodes.Ldobj)
					_il.Emit(code, GetSystemType(et));
				else
					_il.Emit(code);

				PopType();
				PushType(et);
			}
		}

		bool ShouldLeaveValueOnStack(Expression node)
		{
			return node.ParentNode.NodeType != NodeType.ExpressionStatement;
		}
		
		void OnReferenceComparison(BinaryExpression node)
		{
			node.Left.Accept(this); PopType();
			node.Right.Accept(this); PopType();
			_il.Emit(OpCodes.Ceq);
			if (BinaryOperatorType.ReferenceInequality == node.Operator)
			{
				EmitIntNot();
			}
			PushBool();
		}
		
		void OnAssignmentToSlice(BinaryExpression node)
		{
			SlicingExpression slice = (SlicingExpression)node.Left;
			Visit(slice.Target);
			
			IArrayType arrayType = (IArrayType)PopType();
			IType elementType = arrayType.GetElementType();
			OpCode opcode = GetStoreEntityOpCode(elementType);
			
			Slice index = slice.Indices[0];
			EmitNormalizedArrayIndex(slice, index.Begin);
			
			bool stobj = IsStobj(opcode);
			if (stobj)
			{
				_il.Emit(OpCodes.Ldelema, GetSystemType(elementType));
			}
			
			Visit(node.Right);
			EmitCastIfNeeded(elementType, PopType());
			
			bool leaveValueOnStack = ShouldLeaveValueOnStack(node);
			LocalBuilder temp = null;
			if (leaveValueOnStack)
			{
				_il.Emit(OpCodes.Dup);
				temp = StoreTempLocal(elementType);
			}
			
			if (stobj)
			{
				_il.Emit(opcode, GetSystemType(elementType));
			}
			else
			{
				_il.Emit(opcode);
			}
			
			if (leaveValueOnStack)
			{
				LoadLocal(temp, elementType);
			}
			else
			{
				PushVoid();
			}
		}

		LocalBuilder _currentLocal = null;

		void LoadLocal(LocalBuilder local, IType localType)
		{
			_il.Emit(OpCodes.Ldloc, local);

			PushType(localType);
			_currentLocal = local;
		}

		void LoadLocal(InternalLocal local)
		{
			LoadLocal(local, false);
		}

		void LoadLocal(InternalLocal local, bool byAddress)
		{
			_il.Emit(IsByAddress(local.Type) ? OpCodes.Ldloca : OpCodes.Ldloc, local.LocalBuilder);

			PushType(local.Type);
			_currentLocal = local.LocalBuilder;
		}

		void LoadIndirectLocal(InternalLocal local)
		{
			LoadLocal(local);

			IType et = local.Type.GetElementType();
			PopType();
			PushType(et);
			OpCode code = GetLoadRefParamCode(et);
			if (code == OpCodes.Ldobj)
				_il.Emit(code, GetSystemType(et));
			else
				_il.Emit(code);
		}

		private LocalBuilder StoreTempLocal(IType elementType)
		{
			LocalBuilder temp;
			temp = _il.DeclareLocal(GetSystemType(elementType));
			_il.Emit(OpCodes.Stloc, temp);
			return temp;
		}

		void OnAssignment(BinaryExpression node)
		{
			if (NodeType.SlicingExpression == node.Left.NodeType)
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
						
						Visit(node.Right);
						EmitCastIfNeeded(param.Type, PopType());
						
						if (leaveValueOnStack)
						{
							_il.Emit(OpCodes.Dup);
							PushType(param.Type);
						}
						_il.Emit(OpCodes.Starg, param.Index);
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
						SetProperty(node, (IProperty)tag, node.Left, node.Right, leaveValueOnStack);
						break;
					}

				case EntityType.Event: //event=null (always internal in this context)
					{
						InternalEvent e = (InternalEvent) tag;
						OpCode opcode = e.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld;
						_il.Emit(OpCodes.Ldnull);
						_il.Emit(opcode, GetFieldBuilder(e.BackingField.Field));
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
			LocalBuilder temp = null;
			IType tempType = null;
			if (leaveValueOnStack)
			{
				Visit(right);
				tempType = PopType();
				temp = StoreTempLocal(tempType);
			}

			LoadParam(param);
			if (temp != null)
			{
				LoadLocal(temp, tempType);
			}
			else
			{
				Visit(right);
			}
			
			EmitCastIfNeeded(param.Type, PopType());
			
			OpCode storecode = GetStoreRefParamCode(param.Type);
			if (IsStobj(storecode)) //passing struct/decimal byref
			{
				_il.Emit(storecode, GetSystemType(param.Type));
			}
			else
			{
				_il.Emit(storecode);
			}

			if (null != temp)
			{
				LoadLocal(temp, tempType);
			}
		}

		void EmitTypeTest(BinaryExpression node)
		{
			IType orig;
			Visit(node.Left); orig = PopType();
			
			Type type = null;
			if (NodeType.TypeofExpression == node.Right.NodeType)
			{
				type = GetSystemType(((TypeofExpression)node.Right).Type);
			}
			else
			{
				type = GetSystemType(node.Right);
			}

			if (!TypeSystemServices.IsReferenceType(orig))
				_il.Emit(OpCodes.Box, GetSystemType(orig));

			_il.Emit(OpCodes.Isinst, type);
		}
		
		void OnTypeTest(BinaryExpression node)
		{
			EmitTypeTest(node);

			_il.Emit(OpCodes.Ldnull);
			_il.Emit(OpCodes.Cgt_Un);

			PushBool();
		}
		
		void LoadCmpOperands(BinaryExpression node)
		{
			IType lhs = node.Left.ExpressionType;
			IType rhs = node.Right.ExpressionType;

			if (lhs != rhs)
			{
				IType type = TypeSystemServices.GetPromotedNumberType(lhs, rhs);
				Visit(node.Left);
				EmitCastIfNeeded(type, PopType());
				Visit(node.Right);
				EmitCastIfNeeded(type, PopType());
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
			_il.Emit(OpCodes.Ceq);
			PushBool();
		}
		
		void OnInequality(BinaryExpression node)
		{
			LoadCmpOperands(node);
			_il.Emit(OpCodes.Ceq);
			EmitIntNot();
			PushBool();
		}
		
		void OnGreaterThan(BinaryExpression node)
		{
			LoadCmpOperands(node);
			_il.Emit(OpCodes.Cgt);
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
			_il.Emit(OpCodes.Clt);
			PushBool();
		}
		
		void OnLessThanOrEqual(BinaryExpression node)
		{
			OnGreaterThan(node);
			EmitIntNot();
		}
		
		void OnExponentiation(BinaryExpression node)
		{
			Visit(node.Left);
			EmitCastIfNeeded(TypeSystemServices.DoubleType, PopType());
			Visit(node.Right);
			EmitCastIfNeeded(TypeSystemServices.DoubleType, PopType());
			_il.EmitCall(OpCodes.Call, Math_Pow, null);
			PushType(TypeSystemServices.DoubleType);
		}
		
		void OnArithmeticOperator(BinaryExpression node)
		{
			IType type = node.ExpressionType;
			node.Left.Accept(this); EmitCastIfNeeded(type, PopType());
			node.Right.Accept(this); EmitCastIfNeeded(type, PopType());
			_il.Emit(GetArithmeticOpCode(type, node.Operator));

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

			//if an op_Implicit has been found already, give it priority!
			IMethod op_Implicit = expression["op_Implicit"] as IMethod;
			if (null != op_Implicit)
			{
				_il.EmitCall(OpCodes.Call, GetMethodInfo(op_Implicit), null);
				return true;
			}

			//use a builtin conversion operator just for the logical operator trueness test
			IType type = GetExpressionType(expression);
			if (TypeSystemServices.ObjectType == type || TypeSystemServices.DuckType == type)
			{
				_il.EmitCall(OpCodes.Call, RuntimeServices_ToBool_Object, null);
				return true;
			}
			else if (TypeSystemServices.IsNullable(type))
			{
				_il.Emit(OpCodes.Ldloca, _currentLocal);
				Type sType = GetSystemType(TypeSystemServices.GetNullableUnderlyingType(type));
				_il.EmitCall(OpCodes.Call, GetNullableHasValue(sType), null);
				LocalBuilder hasValue = StoreTempLocal(TypeSystemServices.BoolType);
				_il.Emit(OpCodes.Pop); //pop nullable address (ldloca)
				_il.Emit(OpCodes.Ldloc, hasValue);
				return true;
			}
			else if (TypeSystemServices.StringType == type)
			{
				_il.EmitCall(OpCodes.Call, String_IsNullOrEmpty, null);
				if (!inNotContext)
					EmitIntNot(); //reverse result (true for not empty)
				else
					notContext = true;
				return true;
			}
			else if (TypeSystemServices.IsIntegerNumber(type))
			{
				if (TypeSystemServices.LongType == type || TypeSystemServices.ULongType == type)
					_il.Emit(OpCodes.Conv_I4);
				return true;
			}
			else if (TypeSystemServices.SingleType == type)
			{
				EmitDefaultValue(TypeSystemServices.SingleType);
				_il.Emit(OpCodes.Ceq);
				if (!inNotContext)
					EmitIntNot();
				else
					notContext = true;
				return true;
			}
			else if (TypeSystemServices.DoubleType == type)
			{
				EmitDefaultValue(TypeSystemServices.DoubleType);
				_il.Emit(OpCodes.Ceq);
				if (!inNotContext)
					EmitIntNot();
				else
					notContext = true;
				return true;
			}
			else if (TypeSystemServices.DecimalType == type)
			{
				_il.EmitCall(OpCodes.Call, RuntimeServices_ToBool_Decimal, null);
				return true;
			}
			else if (!type.IsValueType)
			{
				if (null == expression.GetAncestor<BinaryExpression>()
				    && null != expression.GetAncestor<IfStatement>())
					return true; //use br(true|false) directly (most common case)

				_il.Emit(OpCodes.Ldnull);
				if (!inNotContext)
				{
					_il.Emit(OpCodes.Cgt_Un);
				}
				else
				{
					_il.Emit(OpCodes.Ceq);
					notContext = true;
				}
				return true;
			}
			return false;
		}
		
		void EmitAnd(BinaryExpression node)
		{
			EmitLogicalOperator(node, OpCodes.Brtrue, OpCodes.Brfalse);
		}
		
		void EmitOr(BinaryExpression node)
		{
			EmitLogicalOperator(node, OpCodes.Brfalse, OpCodes.Brtrue);
		}
		
		void EmitLogicalOperator(BinaryExpression node, OpCode brForValueType, OpCode brForRefType)
		{
			IType type = GetExpressionType(node);
			Visit(node.Left);
			
			IType lhsType = PopType();
			
			if (null != lhsType && lhsType.IsValueType && !type.IsValueType)
			{
				// if boxing, first evaluate the value
				// as it is and then box it...
				Label evalRhs = _il.DefineLabel();
				Label end = _il.DefineLabel();
				
				_il.Emit(OpCodes.Dup);
				EmitToBoolIfNeeded(node.Left);	// may need to convert decimal to bool
				_il.Emit(brForValueType, evalRhs);
				EmitCastIfNeeded(type, lhsType);
				_il.Emit(OpCodes.Br_S, end);
				
				_il.MarkLabel(evalRhs);
				_il.Emit(OpCodes.Pop);
				Visit(node.Right);
				EmitCastIfNeeded(type, PopType());
				
				_il.MarkLabel(end);
			}
			else
			{
				Label end = _il.DefineLabel();
				
				EmitCastIfNeeded(type, lhsType);
				_il.Emit(OpCodes.Dup);
				
				EmitToBoolIfNeeded(node.Left);
				
				_il.Emit(brForRefType, end);
				
				_il.Emit(OpCodes.Pop);
				Visit(node.Right);
				EmitCastIfNeeded(type, PopType());
				_il.MarkLabel(end);
			}
			
			PushType(type);
		}
		
		IType GetExpectedTypeForBitwiseRightOperand(BinaryExpression node)
		{
			switch (node.Operator)
			{
					// BOO-705
				case BinaryOperatorType.ShiftLeft:
				case BinaryOperatorType.ShiftRight:
					return TypeSystemServices.IntType;
			}
			return GetExpressionType(node);
		}
		
		void EmitBitwiseOperator(BinaryExpression node)
		{
			IType type = node.ExpressionType;

			Visit(node.Left);
			EmitCastIfNeeded(type, PopType());

			Visit(node.Right);
			EmitCastIfNeeded(
				GetExpectedTypeForBitwiseRightOperand(node),
				PopType());
			
			switch (node.Operator)
			{
				case BinaryOperatorType.BitwiseOr:
					{
						_il.Emit(OpCodes.Or);
						break;
					}
					
				case BinaryOperatorType.BitwiseAnd:
					{
						_il.Emit(OpCodes.And);
						break;
					}
					
				case BinaryOperatorType.ExclusiveOr:
					{
						_il.Emit(OpCodes.Xor);
						break;
					}

				case BinaryOperatorType.ShiftLeft:
					{
						_il.Emit(OpCodes.Shl);
						break;
					}
				case BinaryOperatorType.ShiftRight:
					{
						_il.Emit(TypeSystemServices.IsSignedNumber(type)
						         ? OpCodes.Shr : OpCodes.Shr_Un);
						break;
					}
			}

			PushType(type);
		}
		
		override public void OnBinaryExpression(BinaryExpression node)
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
		
		override public void OnTypeofExpression(TypeofExpression node)
		{
			EmitGetTypeFromHandle(GetSystemType(node.Type));
		}
		
		override public void OnCastExpression(CastExpression node)
		{
			IType type = GetType(node.Type);
			Visit(node.Target);
			EmitCastIfNeeded(type, PopType());
			PushType(type);
		}
		
		override public void OnTryCastExpression(TryCastExpression node)
		{
			Type type = GetSystemType(node.Type);
			
			node.Target.Accept(this); PopType();
			_il.Emit(OpCodes.Isinst, type);
			PushType(node.ExpressionType);
		}
		
		void InvokeMethod(IMethod method, MethodInvocationExpression node)
		{
			MethodInfo mi = GetMethodInfo(method);
			if (!InvokeOptimizedMethod(method, mi, node))
			{
				InvokeRegularMethod(method, mi, node);
			}
		}
		
		bool InvokeOptimizedMethod(IMethod method, MethodInfo mi, MethodInvocationExpression node)
		{
			if (Builtins_ArrayTypedConstructor == mi)
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
			else if (mi.IsGenericMethod && Builtins_ArrayGenericConstructor == mi.GetGenericMethodDefinition())
			{
				// optimize constructs such as:
				//		array[of int](2)
				IType type = method.ConstructedInfo.GenericArguments[0];
				if (null != type)
				{
					EmitNewArray(type, node.Arguments[0]);
					return true;
				}
			}
			else if (Builtins_ArrayTypedCollectionConstructor == mi)
			{
				// optimize constructs such as:
				//		array(int, (1, 2, 3))
				//		array(byte, [1, 2, 3, 4])
				IType type = TypeSystemServices.GetReferencedType(node.Arguments[0]);
				if (null != type)
				{
					ListLiteralExpression items = node.Arguments[1] as ListLiteralExpression;
					if (null != items)
					{
						EmitArray(type, items.Items);
						PushType(type.MakeArrayType(1));
						return true;
					}
				}
			}
			else if (Array_get_Length == mi)
			{
				// optimize constructs such as:
				//		len(anArray)
				//		anArray.Length
				Visit(node.Target);
				PopType();
				_il.Emit(OpCodes.Ldlen);
				PushType(TypeSystemServices.IntType);
				return true;
			}
			return false;
		}

		void EmitNewArray(IType type, Expression length)
		{
			Visit(length);
			EmitCastIfNeeded(TypeSystemServices.IntType, PopType());
			_il.Emit(OpCodes.Newarr, GetSystemType(type));
			PushType(type.MakeArrayType(1));
		}

		void InvokeRegularMethod(IMethod method, MethodInfo mi, MethodInvocationExpression node)
		{
			// Do not emit call if conditional attributes (if any) do not match defined symbols
			if (!CheckConditionalAttributes(method, mi))
			{
				EmitNop();
				PushType(method.ReturnType); // keep a valid state
				return;
			}

			IType targetType = null;
			Expression target = null;
			if (!mi.IsStatic)
			{
				target = GetTargetObject(node);
				targetType = target.ExpressionType;
				PushTargetObject(node, mi);
			}

			PushArguments(method, node.Arguments);

			// Emit a constrained call if target is a generic parameter
			if (targetType != null && targetType is IGenericParameter)
			{
				_il.Emit(OpCodes.Constrained, GetSystemType(targetType));
			}
			_il.EmitCall(GetCallOpCode(target, method), mi, null);

			PushType(method.ReturnType);
		}

		//returns true if no conditional attribute match the defined symbols
		//else return false (which means the method won't get emitted)
		private bool CheckConditionalAttributes(IMethod method, MethodInfo mi)
		{
			foreach (string conditionalSymbol in GetConditionalSymbols(method))
			{
				if (!Parameters.Defines.ContainsKey(conditionalSymbol))
				{
					_context.TraceInfo("call to method '{0}' not emitted because the symbol '{1}' is not defined.", method.ToString(), conditionalSymbol);
					return false;
				}
			}
			return true;
		}

		private IEnumerable<string> GetConditionalSymbols(IMethod method)
		{
			GenericMappedMethod mappedMethod = method as GenericMappedMethod;
			if (mappedMethod != null)
			{
				return GetConditionalSymbols(mappedMethod.SourceMember);
			}

			GenericConstructedMethod constructedMethod = method as GenericConstructedMethod;
			if (constructedMethod != null)
			{
				return GetConditionalSymbols(constructedMethod.GenericDefinition);
			}

			ExternalMethod externalMethod = method as ExternalMethod;
			if (externalMethod != null)
			{
				return GetConditionalSymbols(externalMethod);
			}

			InternalMethod internalMethod = method as InternalMethod;
			if (internalMethod != null)
			{
				return GetConditionalSymbols(internalMethod);
			}

			return new string[0];
		}

		private IEnumerable<string> GetConditionalSymbols(ExternalMethod method)
		{
			object[] attrs = method.MethodInfo.GetCustomAttributes(typeof(System.Diagnostics.ConditionalAttribute), false);
			foreach (System.Diagnostics.ConditionalAttribute attr in attrs)
			{
				yield return attr.ConditionString;
			}
			yield break;
		}

		private IEnumerable<string> GetConditionalSymbols(InternalMethod method)
		{
			Attribute[] attrs = MetadataUtil.GetCustomAttributes(method.Method, TypeSystemServices.ConditionalAttribute);
			foreach (Attribute attr in attrs)
			{
				if (1 != attr.Arguments.Count) continue;

				StringLiteralExpression sle = attr.Arguments[0] as StringLiteralExpression;
				if (sle == null) continue;

				yield return sle.Value;
			}
			yield break;
		}

		private void PushTargetObject(MethodInvocationExpression node, MethodInfo mi)
		{
			Expression target = GetTargetObject(node);
			IType targetType = target.ExpressionType;

			// If target is a generic parameter, its address must be loaded
			// to allow a constrained method call
			if (targetType is IGenericParameter)
			{
				LoadAddress(target);
			}
	
			else if (targetType.IsValueType)
			{
				if (mi.DeclaringType.IsValueType)
				{
					LoadAddress(target);					
				}
				else
				{
					Visit(node.Target);
					EmitBox(PopType());
				}
			}
			else
			{
				// pushes target reference
				Visit(node.Target);
				PopType();
			}
		}

		private static Expression GetTargetObject(MethodInvocationExpression node)
		{
			Expression target = node.Target;
			
			// Skip over generic reference expressions
			GenericReferenceExpression genericRef = target as GenericReferenceExpression;
			if (genericRef != null)
			{
				target = genericRef.Target;
			}
			
			MemberReferenceExpression memberRef = target as MemberReferenceExpression;			
			if (memberRef != null)
			{
				return memberRef.Target;
			}
			
			return null;
		}

		private OpCode GetCallOpCode(Expression target, IMethod method)
		{
			if (method.IsStatic) return OpCodes.Call;
			if (NodeType.SuperLiteralExpression == target.NodeType) return OpCodes.Call;
			if (IsValueTypeMethodCall(target, method)) return OpCodes.Call;
			return OpCodes.Callvirt;
		}
		
		private bool IsValueTypeMethodCall(Expression target, IMethod method)
		{
			IType type = target.ExpressionType;
			return type.IsValueType && method.DeclaringType == type;
		}

		void InvokeSuperMethod(IMethod methodInfo, MethodInvocationExpression node)
		{
			IMethod super = ((InternalMethod)methodInfo).Overriden;
			MethodInfo superMI = GetMethodInfo(super);
			if (methodInfo.DeclaringType.IsValueType)
			{
				_il.Emit(OpCodes.Ldarga_S, 0);
			}
			else
			{
				_il.Emit(OpCodes.Ldarg_0); // this
			}
			PushArguments(super, node.Arguments);
			_il.EmitCall(OpCodes.Call, superMI, null);
			PushType(super.ReturnType);
		}
		
		void EmitGetTypeFromHandle(Type type)
		{
			_il.Emit(OpCodes.Ldtoken, type);
			_il.EmitCall(OpCodes.Call, Type_GetTypeFromHandle, null);
			PushType(TypeSystemServices.TypeType);
		}
		
		void OnEval(MethodInvocationExpression node)
		{
			int allButLast = node.Arguments.Count-1;
			for (int i=0; i<allButLast; ++i)
			{
				Visit(node.Arguments[i]);
				DiscardValueOnStack();
			}
			
			Visit(node.Arguments[-1]);
		}
		
		void OnAddressOf(MethodInvocationExpression node)
		{
			MemberReferenceExpression methodRef = (MemberReferenceExpression)node.Arguments[0];
			MethodInfo method = GetMethodInfo((IMethod)GetEntity(methodRef));
			if (method.IsVirtual)
			{
				_il.Emit(OpCodes.Dup);
				_il.Emit(OpCodes.Ldvirtftn, method);
			}
			else
			{
				_il.Emit(OpCodes.Ldftn, method);
			}
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
			System.Type type = GetSystemType(GetExpressionType(argument));
			Debug.Assert(type.IsValueType);
			_il.Emit(OpCodes.Initobj, type);
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
						LocalBuilder builder = local.LocalBuilder;
						_il.Emit(OpCodes.Ldloca, builder);
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

		override public void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			IEntity tag = TypeSystemServices.GetEntity(node.Target);
			
			switch (tag.EntityType)
			{
				case EntityType.BuiltinFunction:
					{
						OnBuiltinFunction((BuiltinFunction)tag, node);
						break;
					}
					
				case EntityType.Method:
					{
						IMethod methodInfo = (IMethod)tag;
						
						if (node.Target.NodeType == NodeType.SuperLiteralExpression)
						{
							InvokeSuperMethod(methodInfo, node);
						}
						else
						{
							InvokeMethod(methodInfo, node);
						}
						
						break;
					}
					
				case EntityType.Constructor:
					{
						IConstructor constructorInfo = (IConstructor)tag;
						ConstructorInfo ci = GetConstructorInfo(constructorInfo);
						
						if (NodeType.SuperLiteralExpression == node.Target.NodeType || node.Target.NodeType == NodeType.SelfLiteralExpression)
						{
							// super constructor call
							_il.Emit(OpCodes.Ldarg_0);
							PushArguments(constructorInfo, node.Arguments);
							_il.Emit(OpCodes.Call, ci);
							PushVoid();
						}
						else
						{
							PushArguments(constructorInfo, node.Arguments);
							_il.Emit(OpCodes.Newobj, ci);
							
							// constructor invocation resulting type is
							PushType(constructorInfo.DeclaringType);
						}
						break;
					}
					
				default:
					{
						NotImplemented(node, tag.ToString());
						break;
					}
			}
		}
		
		override public void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node)
		{
			EmitLoadLiteral(node.Value.Ticks);
			_il.Emit(OpCodes.Newobj, TimeSpan_LongConstructor);
			PushType(TypeSystemServices.TimeSpanType);
		}
		
		override public void OnIntegerLiteralExpression(IntegerLiteralExpression node)
		{
			IType type = node.ExpressionType ?? TypeSystemServices.IntType;
			EmitLoadLiteral(type, node.Value);
			PushType(type);
		}

		override public void OnDoubleLiteralExpression(DoubleLiteralExpression node)
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

		void EmitLoadLiteral(IType type, long l)
		{
			EmitLoadLiteral(type, l, l);
		}

		void EmitLoadLiteral(IType type, double d)
		{
			EmitLoadLiteral(type, (long) d, d);
		}

		void EmitLoadLiteral(IType type, long l, double d)
		{
			if (type.IsEnum)
				type = TypeSystemServices.Map(GetEnumUnderlyingType(type));

			if (TypeSystemServices.IsIntegerNumber(type) || type == TypeSystemServices.CharType)
			{
				bool needsLongConv = true;
				switch (l)
				{
					case -1L:
						{
							if (type == TypeSystemServices.LongType || type == TypeSystemServices.ULongType)
							{
								_il.Emit(OpCodes.Ldc_I8, -1L);
								needsLongConv = false;
							}
							else
							{
								_il.Emit(OpCodes.Ldc_I4_M1);
							}
						}
						break;
					case 0L: _il.Emit(OpCodes.Ldc_I4_0); break;
					case 1L: _il.Emit(OpCodes.Ldc_I4_1); break;
					case 2L: _il.Emit(OpCodes.Ldc_I4_2); break;
					case 3L: _il.Emit(OpCodes.Ldc_I4_3); break;
					case 4L: _il.Emit(OpCodes.Ldc_I4_4); break;
					case 5L: _il.Emit(OpCodes.Ldc_I4_5); break;
					case 6L: _il.Emit(OpCodes.Ldc_I4_6); break;
					case 7L: _il.Emit(OpCodes.Ldc_I4_7); break;
					case 8L: _il.Emit(OpCodes.Ldc_I4_8); break;
					default:
						{
							if (l == (sbyte) l) //fits in an signed i1
							{
								_il.Emit(OpCodes.Ldc_I4_S, (sbyte) l);
							}
							else if (l == (int) l || l == (uint) l) //fits in an i4
							{
								if ((int) l == -1)
									_il.Emit(OpCodes.Ldc_I4_M1);
								else
									_il.Emit(OpCodes.Ldc_I4, (int) l);
							}
							else
							{
								_il.Emit(OpCodes.Ldc_I8, l);
								needsLongConv = false;
							}
						}
						break;
				}

				if (needsLongConv && type == TypeSystemServices.LongType)
					_il.Emit(OpCodes.Conv_I8);
				else if (type == TypeSystemServices.ULongType)
					_il.Emit(OpCodes.Conv_U8);
			}
			else if (type == TypeSystemServices.SingleType)
			{
				if (d != 0)
				{
					_il.Emit(OpCodes.Ldc_R4, (float) d);
				}
				else
				{
					_il.Emit(OpCodes.Ldc_I4_0);
					_il.Emit(OpCodes.Conv_R4);
				}
			}
			else if (type == TypeSystemServices.DoubleType)
			{
				if (d != 0)
				{
					_il.Emit(OpCodes.Ldc_R8, d);
				}
				else
				{
					_il.Emit(OpCodes.Ldc_I4_0);
					_il.Emit(OpCodes.Conv_R8);
				}
			}
			else
			{
				throw new InvalidOperationException(string.Format("`{0}' is not a literal", type));
			}
		}

		override public void OnBoolLiteralExpression(BoolLiteralExpression node)
		{
			if (node.Value)
			{
				_il.Emit(OpCodes.Ldc_I4_1);
			}
			else
			{
				_il.Emit(OpCodes.Ldc_I4_0);
			}
			PushBool();
		}
		
		override public void OnHashLiteralExpression(HashLiteralExpression node)
		{
			_il.Emit(OpCodes.Newobj, Hash_Constructor);
			
			IType objType = TypeSystemServices.ObjectType;
			foreach (ExpressionPair pair in node.Items)
			{
				_il.Emit(OpCodes.Dup);
				
				Visit(pair.First);
				EmitCastIfNeeded(objType, PopType());
				
				Visit(pair.Second);
				EmitCastIfNeeded(objType, PopType());
				_il.EmitCall(OpCodes.Callvirt, Hash_Add, null);
			}
			
			PushType(TypeSystemServices.HashType);
		}
		
		override public void OnGeneratorExpression(GeneratorExpression node)
		{
			NotImplemented(node, node.ToString());
		}
		
		override public void OnListLiteralExpression(ListLiteralExpression node)
		{
			if (node.Items.Count > 0)
			{
				EmitObjectArray(node.Items);
				_il.Emit(OpCodes.Ldc_I4_1);
				_il.Emit(OpCodes.Newobj, List_ArrayBoolConstructor);
			}
			else
			{
				_il.Emit(OpCodes.Newobj, List_EmptyConstructor);
			}
			PushType(TypeSystemServices.ListType);
		}
		
		override public void OnArrayLiteralExpression(ArrayLiteralExpression node)
		{
			IArrayType type = (IArrayType)node.ExpressionType;
			EmitArray(type.GetElementType(), node.Items);
			PushType(type);
		}
		
		override public void OnRELiteralExpression(RELiteralExpression node)
		{
			RegexOptions options = AstUtil.GetRegexOptions(node);

			_il.Emit(OpCodes.Ldstr, node.Pattern);
			if (options == RegexOptions.None)
			{
				_il.Emit(OpCodes.Newobj, Regex_Constructor);
			}
			else
			{
				EmitLoadLiteral((int) options);
				_il.Emit(OpCodes.Newobj, Regex_Constructor_Options);
			}

			PushType(node.ExpressionType);
		}
		
		override public void OnStringLiteralExpression(StringLiteralExpression node)
		{
			if (null == node.Value)
			{
				_il.Emit(OpCodes.Ldnull);
			}
			else if (0 != node.Value.Length)
			{ 
				_il.Emit(OpCodes.Ldstr, node.Value);
			}
			else /* force use of CLR-friendly string.Empty */
			{
				_il.Emit(OpCodes.Ldsfld, typeof(string).GetField("Empty"));
			}
			PushType(TypeSystemServices.StringType);
		}
		
		override public void OnCharLiteralExpression(CharLiteralExpression node)
		{
			EmitLoadLiteral((int) node.Value[0]);
			PushType(TypeSystemServices.CharType);
		}
		
		override public void OnSlicingExpression(SlicingExpression node)
		{
			if (AstUtil.IsLhsOfAssignment(node))
			{
				return;
			}
			
			Visit(node.Target);
			
			IArrayType type = (IArrayType)PopType();
			EmitNormalizedArrayIndex(node, node.Indices[0].Begin);
			
			IType elementType = type.GetElementType();
			OpCode opcode = GetLoadEntityOpCode(elementType);
			if (OpCodes.Ldelema.Value == opcode.Value)
			{
				Type systemType = GetSystemType(elementType);
				_il.Emit(opcode, systemType);
				if (!IsByAddress(elementType))
					_il.Emit(OpCodes.Ldobj, systemType);
			}
			else if (OpCodes.Ldelem.Value == opcode.Value)
			{
				_il.Emit(opcode, GetSystemType(elementType));
			}
			else
			{
				_il.Emit(opcode);
			}
			PushType(elementType);
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
					_il.Emit(OpCodes.Dup);
					_il.Emit(OpCodes.Ldlen);
					EmitLoadInt(index);
					_il.Emit(OpCodes.Add);
				}
				else
				{
					_il.Emit(OpCodes.Dup);
					EmitLoadInt(index);
					_il.EmitCall(OpCodes.Call, RuntimeServices_NormalizeArrayIndex, null);
				}
			}
			else
			{
				EmitLoadInt(index);
			}
		}
		
		bool CanBeNegative(Expression expression, ref bool isNegative)
		{
			IntegerLiteralExpression integer = expression as IntegerLiteralExpression;
			if (null != integer)
			{
				if (integer.Value >= 0)
				{
					return false;
				}
				isNegative = true;
			}
			return true;
		}
		
		void EmitLoadInt(Expression expression)
		{
			Visit(expression);
			EmitCastIfNeeded(TypeSystemServices.IntType, PopType());
		}
		
		override public void OnExpressionInterpolationExpression(ExpressionInterpolationExpression node)
		{
			Type stringBuilderType = typeof(StringBuilder);
			ConstructorInfo constructor = stringBuilderType.GetConstructor(Type.EmptyTypes);
			ConstructorInfo constructorString = stringBuilderType.GetConstructor(new Type[] { typeof(string) });
			MethodInfo appendObject = stringBuilderType.GetMethod("Append", new Type[] { typeof(object) });
			MethodInfo appendString = stringBuilderType.GetMethod("Append", new Type[] { typeof(string) });
			Expression arg0 = node.Expressions[0];
			IType argType = arg0.ExpressionType;

			/* if arg0 is a string, let's call StringBuilder constructor
			 * directly with the string */
			if ( ( typeof(StringLiteralExpression) == arg0.GetType()
				   && ((StringLiteralExpression) arg0).Value.Length > 0 )
				|| ( typeof(StringLiteralExpression) != arg0.GetType()
					 && TypeSystemServices.StringType == argType ) )
			{
				Visit(arg0);
				PopType();
				_il.Emit(OpCodes.Newobj, constructorString);
			}
			else
			{
				_il.Emit(OpCodes.Newobj, constructor);
				arg0 = null; /* arg0 is not a string so we want it to be appended below */
			}

			string formatString;
			foreach (Expression arg in node.Expressions)
			{
				/* we do not need to append literal string.Empty
				 * or arg0 if it has been handled by ctor */
				if ( ( typeof(StringLiteralExpression) == arg.GetType() 
					   && ((StringLiteralExpression) arg).Value.Length == 0 )
					|| arg == arg0 )
				{
					continue;
				}

				formatString = arg["formatString"] as string; //annotation
				if (!string.IsNullOrEmpty(formatString))
					_il.Emit(OpCodes.Ldstr, string.Format("{{0:{0}}}", formatString));

				Visit(arg);
				argType = PopType();

				if (!string.IsNullOrEmpty(formatString))
				{
					EmitCastIfNeeded(TypeSystemServices.ObjectType, argType);
					_il.EmitCall(OpCodes.Call, StringFormat, null);
				}

				if (TypeSystemServices.StringType == argType || !string.IsNullOrEmpty(formatString))
				{
					_il.EmitCall(OpCodes.Call, appendString, null);
				}
				else
				{
					EmitCastIfNeeded(TypeSystemServices.ObjectType, argType);
					_il.EmitCall(OpCodes.Call, appendObject, null);
				}
			}
			_il.EmitCall(OpCodes.Call, stringBuilderType.GetMethod("ToString", Type.EmptyTypes), null);
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
			if (field.IsStatic)
			{
				_il.Emit(OpCodes.Ldsflda, GetFieldInfo(field));
			}
			else
			{
				LoadMemberTarget(((MemberReferenceExpression)expression).Target, field);
				_il.Emit(OpCodes.Ldflda, GetFieldInfo(field));
			}
		}
		
		void EmitLoadField(Expression self, IField fieldInfo)
		{
			if (fieldInfo.IsStatic)
			{
				if (fieldInfo.IsLiteral)
				{
					EmitLoadLiteralField(self, fieldInfo);
				}
				else
				{
					if (fieldInfo.IsVolatile)
						_il.Emit(OpCodes.Volatile);
					_il.Emit(IsByAddress(fieldInfo.Type) ? OpCodes.Ldsflda : OpCodes.Ldsfld,
						GetFieldInfo(fieldInfo));
				}
			}
			else
			{
				LoadMemberTarget(self, fieldInfo);
				if (fieldInfo.IsVolatile)
					_il.Emit(OpCodes.Volatile);
				_il.Emit(IsByAddress(fieldInfo.Type) ? OpCodes.Ldflda : OpCodes.Ldfld,
					GetFieldInfo(fieldInfo));
			}
			PushType(fieldInfo.Type);
		}
		
		object GetStaticValue(IField field)
		{
			InternalField internalField = field as InternalField;
			if (null != internalField)
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
			if (type.IsEnum){
				Type underlyingType = GetEnumUnderlyingType(type);
				type = TypeSystemServices.Map(underlyingType);
				value = Convert.ChangeType(value, underlyingType);
			}

			if (null == value)
			{
				_il.Emit(OpCodes.Ldnull);
			}
			else if (type == TypeSystemServices.BoolType)
			{
				_il.Emit(((bool) value) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
			}
			else if (type == TypeSystemServices.StringType)
			{
				_il.Emit(OpCodes.Ldstr, (string) value);
			}
			else if (type == TypeSystemServices.CharType)
			{
				EmitLoadLiteral(type, (long)(char) value);
			}
			else if (type == TypeSystemServices.IntType)
			{
				EmitLoadLiteral(type, (long)(int) value);
			}
			else if (type == TypeSystemServices.UIntType)
			{
				EmitLoadLiteral(type, unchecked((long)(uint) value));
			}
			else if (type == TypeSystemServices.LongType)
			{
				EmitLoadLiteral(type, (long) value);
			}
			else if (type == TypeSystemServices.ULongType)
			{
				EmitLoadLiteral(type, unchecked((long)(ulong) value));
			}
			else if (type == TypeSystemServices.SingleType)
			{
				EmitLoadLiteral(type, (double)(float) value);
			}
			else if (type == TypeSystemServices.DoubleType)
			{
				EmitLoadLiteral(type, (double) value);
			}
			else if (type == TypeSystemServices.SByteType)
			{
				EmitLoadLiteral(type, (long)(sbyte) value);
			}
			else if (type == TypeSystemServices.ByteType)
			{
				EmitLoadLiteral(type, (long)(byte) value);
			}
			else if (type == TypeSystemServices.ShortType)
			{
				EmitLoadLiteral(type, (long)(short) value);
			}
			else if (type == TypeSystemServices.UShortType)
			{
				EmitLoadLiteral(type, (long)(ushort) value);
			}
			else
			{
				NotImplemented(node, "Literal field type: " + type.ToString());
			}
		}

		override public void OnGenericReferenceExpression(GenericReferenceExpression node)
		{
			IEntity tag = TypeSystem.TypeSystemServices.GetEntity(node);
			switch (tag.EntityType)
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
						NotImplemented(node, tag.ToString());
						break;
					}
			}
		}
		
		override public void OnMemberReferenceExpression(MemberReferenceExpression node)
		{
			IEntity tag = TypeSystem.TypeSystemServices.GetEntity(node);
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
			if (NodeType.SelfLiteralExpression == expression.NodeType)
			{
				if (expression.ExpressionType.IsValueType)
				{
					_il.Emit(OpCodes.Ldarg_0);
					return;
				}
			}
			
			IEntity tag = expression.Entity;
			if (null != tag)
			{
				switch (tag.EntityType)
				{
					case EntityType.Local:
						{
							InternalLocal local =  ((InternalLocal) tag);
							_il.Emit(!local.Type.IsPointer ? OpCodes.Ldloca : OpCodes.Ldloc,
							        local.LocalBuilder);
							return;
						}
						
					case EntityType.Parameter:
						{
							InternalParameter param = (InternalParameter)tag;
							if (param.Parameter.IsByRef)
							{
								LoadParam(param);
							}
							else
							{
								_il.Emit(OpCodes.Ldarga, param.Index);
							}
							return;
						}
						
					case EntityType.Field:
						{
							IField field = (IField)tag;
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
				SlicingExpression slicing = (SlicingExpression)expression;
				Visit(slicing.Target);
				IArrayType arrayType = (IArrayType)PopType();
				EmitNormalizedArrayIndex(slicing, slicing.Indices[0].Begin);
				_il.Emit(OpCodes.Ldelema, GetSystemType(arrayType.GetElementType()));
			}
			else
			{
				Visit(expression);

				if (!AstUtil.IsIndirection(expression))
				{
					// declare local to hold value type
					LocalBuilder temp = _il.DeclareLocal(GetSystemType(PopType()));
					_il.Emit(OpCodes.Stloc, temp);
					_il.Emit(OpCodes.Ldloca, temp);
				}
			}
		}
		
		bool IsValueTypeArraySlicing(Expression expression)
		{
			SlicingExpression slicing = expression as SlicingExpression;
			if (null != slicing)
			{
				IArrayType type = (IArrayType)slicing.Target.ExpressionType;
				return type.GetElementType().IsValueType;
			}
			return false;
		}
		
		override public void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			_il.Emit(OpCodes.Ldarg_0);
			if (node.ExpressionType.IsValueType)
			{
				_il.Emit(OpCodes.Ldobj, GetSystemType(node.ExpressionType));
			}
			PushType(node.ExpressionType);
		}
		
		override public void OnSuperLiteralExpression(SuperLiteralExpression node)
		{
			_il.Emit(OpCodes.Ldarg_0);
			if (node.ExpressionType.IsValueType)
			{
				_il.Emit(OpCodes.Ldobj, GetSystemType(node.ExpressionType));
			}
			PushType(node.ExpressionType);
		}
		
		override public void OnNullLiteralExpression(NullLiteralExpression node)
		{
			_il.Emit(OpCodes.Ldnull);
			PushType(null);
		}

		override public void OnReferenceExpression(ReferenceExpression node)
		{
			IEntity info = TypeSystem.TypeSystemServices.GetEntity(node);
			switch (info.EntityType)
			{
				case EntityType.Local:
					{
						if (!AstUtil.IsIndirection(node.ParentNode))
							LoadLocal((InternalLocal)info);
						else
							LoadIndirectLocal((InternalLocal)info);
						break;
					}

				case EntityType.Parameter:
					{
						InternalParameter param = (InternalParameter)info;
						LoadParam(param);
						
						if (param.Parameter.IsByRef)
						{
							OpCode code = GetLoadRefParamCode(param.Type);
							if (code.Value == OpCodes.Ldobj.Value)
							{
								_il.Emit(code, GetSystemType(param.Type));
							}
							else {
								_il.Emit(code);
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
						NotImplemented(node, info.ToString());
						break;
					}
					
			}
		}
		
		void LoadParam(InternalParameter param)
		{
			int index = param.Index;
			
			switch (index)
			{
				case 0:
					{
						_il.Emit(OpCodes.Ldarg_0);
						break;
					}
					
				case 1:
					{
						_il.Emit(OpCodes.Ldarg_1);
						break;
					}
					
				case 2:
					{
						_il.Emit(OpCodes.Ldarg_2);
						break;
					}
					
				case 3:
					{
						_il.Emit(OpCodes.Ldarg_3);
						break;
					}
					
				default:
					{
						if (index < 256)
						{
							_il.Emit(OpCodes.Ldarg_S, index);
						}
						else
						{
							_il.Emit(OpCodes.Ldarg, index);
						}
						break;
					}
			}
		}

		void SetLocal(BinaryExpression node, InternalLocal tag, bool leaveValueOnStack)
		{
			if (AstUtil.IsIndirection(node.Left))
				_il.Emit(OpCodes.Ldloc, tag.LocalBuilder);

			node.Right.Accept(this); // leaves type on stack
			
			IType typeOnStack = null;
			
			if (leaveValueOnStack)
			{
				typeOnStack = PeekTypeOnStack();
				_il.Emit(OpCodes.Dup);
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
			LocalBuilder local = tag.LocalBuilder;
			EmitCastIfNeeded(tag.Type, typeOnStack);
			_il.Emit(OpCodes.Stloc, local);
		}

		void EmitIndirectAssignment(InternalLocal tag, IType typeOnStack)
		{
			IType et = tag.Type.GetElementType();
			EmitCastIfNeeded(et, typeOnStack);

			OpCode code = GetStoreRefParamCode(et);
			if (code == OpCodes.Stobj)
				_il.Emit(code, GetSystemType(et));
			else
				_il.Emit(code);
		}

		void SetField(Node sourceNode, IField field, Expression reference, Expression value, bool leaveValueOnStack)
		{
			OpCode opSetField = OpCodes.Stsfld;
			if (!field.IsStatic)
			{
				opSetField = OpCodes.Stfld;
				if (null != reference)
				{
					LoadMemberTarget(
						((MemberReferenceExpression)reference).Target,
						field);
				}
			}
			
			value.Accept(this);
			EmitCastIfNeeded(field.Type, PopType());

			LocalBuilder local = null;
			if (leaveValueOnStack)
			{
				_il.Emit(OpCodes.Dup);
				local = _il.DeclareLocal(GetSystemType(field.Type));
				_il.Emit(OpCodes.Stloc, local);
			}

			if (field.IsVolatile)
				_il.Emit(OpCodes.Volatile);
			_il.Emit(opSetField, GetFieldInfo(field));

			if (leaveValueOnStack)
			{
				_il.Emit(OpCodes.Ldloc, local);
				PushType(field.Type);
			}
		}
		
		void SetProperty(Node sourceNode, IProperty property, Expression reference, Expression value, bool leaveValueOnStack)
		{
			OpCode callOpCode = OpCodes.Call;
			
			MethodInfo setMethod = GetMethodInfo(property.GetSetMethod());
			
			if (null != reference)
			{
				if (!setMethod.IsStatic)
				{
					Expression target = ((MemberReferenceExpression)reference).Target;
					if (setMethod.DeclaringType.IsValueType)
					{
						LoadAddress(target);
					}
					else
					{
						callOpCode = GetCallOpCode(target, property.GetSetMethod());
						
						target.Accept(this);
						PopType();
					}
				}
			}
			
			value.Accept(this);
			EmitCastIfNeeded(property.Type, PopType());
			
			LocalBuilder local = null;
			if (leaveValueOnStack)
			{
				_il.Emit(OpCodes.Dup);
				local = _il.DeclareLocal(GetSystemType(property.Type));
				_il.Emit(OpCodes.Stloc, local);
			}
			
			_il.EmitCall(callOpCode, setMethod, null);
			
			if (leaveValueOnStack)
			{
				_il.Emit(OpCodes.Ldloc, local);
				PushType(property.Type);
			}
		}
		
		bool EmitDebugInfo(Node node)
		{
			if (!Parameters.Debug)
				return false;
			return EmitDebugInfo(node, node);
		}

		private const int _DBG_SYMBOLS_QUEUE_CAPACITY = 5; 

		private System.Collections.Generic.Queue<LexicalInfo> _dbgSymbols = new System.Collections.Generic.Queue<LexicalInfo>(_DBG_SYMBOLS_QUEUE_CAPACITY);

		[ReflectionPermission(SecurityAction.Demand, ReflectionEmit=true)]
		bool EmitDebugInfo(Node startNode, Node endNode)
		{
			LexicalInfo start = startNode.LexicalInfo;
			if (!start.IsValid) return false;

			ISymbolDocumentWriter writer = GetDocumentWriter(start.FullPath);
			if (null == writer) return false;
			
			// ensure there is no duplicate emitted
			if (_dbgSymbols.Contains(start)) {
				_context.TraceInfo("duplicate symbol emit attempt for '{0}' : '{1}'.", start.ToString(), startNode.ToString()); 
				return false;
			}
			if (_dbgSymbols.Count >= _DBG_SYMBOLS_QUEUE_CAPACITY) _dbgSymbols.Dequeue();
			_dbgSymbols.Enqueue(start);

			try
			{
				_il.MarkSequencePoint(writer, start.Line, 0, start.Line+1, 0);
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
			_il.Emit(OpCodes.Nop);
		}

		private ISymbolDocumentWriter GetDocumentWriter(string fname)
		{
			ISymbolDocumentWriter writer = GetCachedDocumentWriter(fname);
			if (null != writer) return writer;
			
			writer = _moduleBuilder.DefineDocument(
				fname,
				Guid.Empty,
				Guid.Empty,
				SymDocumentType.Text);
			_symbolDocWriters.Add(fname, writer);
			
			return writer;
		}

		private ISymbolDocumentWriter GetCachedDocumentWriter(string fname)
		{
			return (ISymbolDocumentWriter) _symbolDocWriters[fname];
		}

		bool IsBoolOrInt(IType type)
		{
			return TypeSystemServices.BoolType == type ||
				TypeSystemServices.IntType == type;
		}
		
		void PushArguments(IMethodBase entity, ExpressionCollection args)
		{
			IParameter[] parameters = entity.GetParameters();
			for (int i=0; i<args.Count; ++i)
			{
				IType parameterType = parameters[i].Type;
				Expression arg = args[i];
				/*
				InternalParameter internalparam = parameters[i] as InternalParameter;
				if ((parameterType.IsByRef) ||
					(internalparam != null &&
					internalparam.Parameter.IsByRef)
					)
				 */
				if (parameters[i].IsByRef)
				{
					LoadAddress(arg);
				}
				else
				{
					arg.Accept(this);
					EmitCastIfNeeded(parameterType, PopType());
				}
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
			_il.Emit(OpCodes.Newarr, GetSystemType(type));

			if (0 == items.Count)
				return;

			int inlineStores = 0;
			if (items.Count > InlineArrayItemCountLimit
			    && TypeSystemServices.IsPrimitiveNumber(type))
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
			OpCode opcode = GetStoreEntityOpCode(type);
			for (int i=0; i<items.Count; ++i)
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
			byte[] ba = CreateByteArrayFromLiteralCollection(type, items);
			if (null == ba)
			{
				EmitInlineArrayInit(type, items);
				return;
			}

			FieldBuilder fb;
			if (!_packedArrays.TryGetValue(ba, out fb))
			{
				//there is no previously emitted bytearray to reuse, create it then
				fb = _moduleBuilder.DefineInitializedData(Context.GetUniqueName("newarr"), ba, FieldAttributes.Private);
				_packedArrays.Add(ba, fb);
			}

			_il.Emit(OpCodes.Dup); //dup (newarr)
			_il.Emit(OpCodes.Ldtoken, fb);
			_il.EmitCall(OpCodes.Call, RuntimeHelpers_InitializeArray, null);
		}

		Dictionary<byte[], FieldBuilder> _packedArrays = new Dictionary<byte[], FieldBuilder>(ArrayEqualityComparer<byte>.Default);

		byte[] CreateByteArrayFromLiteralCollection(IType type, ExpressionCollection items)
		{
			using (MemoryStream ms = new MemoryStream(items.Count * TypeSystemServices.SizeOf(type)))
			{
				using (BinaryWriter writer = new BinaryWriter(ms))
				{
					foreach (Expression item in items)
					{
						//TODO: BOO-1222 NumericLiteralExpression.GetValueAs<T>()
						if (item.NodeType == NodeType.IntegerLiteralExpression)
						{
							IntegerLiteralExpression literal = (IntegerLiteralExpression) item;
							if (type == TypeSystemServices.IntType)
								writer.Write(Convert.ToInt32(literal.Value));
							else if (type == TypeSystemServices.UIntType)
								writer.Write(Convert.ToUInt32(literal.Value));
							else if (type == TypeSystemServices.LongType)
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
							DoubleLiteralExpression literal = (DoubleLiteralExpression) item;
							if (type == TypeSystemServices.SingleType)
								writer.Write(Convert.ToSingle(literal.Value));
							else if (type == TypeSystemServices.DoubleType)
								writer.Write(Convert.ToDouble(literal.Value));
							else if (type == TypeSystemServices.IntType)
								writer.Write(Convert.ToInt32(literal.Value));
							else if (type == TypeSystemServices.UIntType)
								writer.Write(Convert.ToUInt32(literal.Value));
							else if (type == TypeSystemServices.LongType)
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
				}
				return ms.ToArray();
			}
		}

		bool IsInteger(IType type)
		{
			return type == TypeSystemServices.IntType ||
				type == TypeSystemServices.LongType ||
				type == TypeSystemServices.ShortType ||
				type == TypeSystemServices.ByteType;
		}
		
		MethodInfo GetToDecimalConversionMethod(IType type)
		{
			MethodInfo method =
				typeof(Decimal).GetMethod("op_Implicit", new Type[] { GetSystemType(type) });
			
			if (method == null)
			{
				method =
					typeof(Decimal).GetMethod("op_Explicit", new Type[] { GetSystemType(type) });
				if (method == null)
				{
					NotImplemented(string.Format("Numeric promotion for {0} to decimal not implemented!", type));
				}
			}
			return method;
		}
		
		MethodInfo GetFromDecimalConversionMethod(IType type)
		{
			string toType = "To" + type.Name;

			MethodInfo method =
				typeof(Decimal).GetMethod(toType, new Type[] { typeof(Decimal) });
			if (method == null)
			{
				NotImplemented(string.Format("Numeric promotion for decimal to {0} not implemented!", type));
			}
			return method;
		}
		
		OpCode GetArithmeticOpCode(IType type, BinaryOperatorType op)
		{
			if (IsInteger(type) && _checked)
			{
				switch (op)
				{
					case BinaryOperatorType.Addition: return OpCodes.Add_Ovf;
					case BinaryOperatorType.Subtraction: return OpCodes.Sub_Ovf;
					case BinaryOperatorType.Multiply: return OpCodes.Mul_Ovf;
					case BinaryOperatorType.Division: return OpCodes.Div;
					case BinaryOperatorType.Modulus: return OpCodes.Rem;
				}
			}
			else
			{
				switch (op)
				{
					case BinaryOperatorType.Addition: return OpCodes.Add;
					case BinaryOperatorType.Subtraction: return OpCodes.Sub;
					case BinaryOperatorType.Multiply: return OpCodes.Mul;
					case BinaryOperatorType.Division: return OpCodes.Div;
					case BinaryOperatorType.Modulus: return OpCodes.Rem;
				}
			}
			throw new ArgumentException("op");
		}
		
		OpCode GetLoadEntityOpCode(IType type)
		{
			if (IsByAddress(type))
				return OpCodes.Ldelema;

			if (!type.IsValueType)
			{
				return type is IGenericParameter
					? OpCodes.Ldelem
					: OpCodes.Ldelem_Ref;
			}

			if (type.IsEnum)
			{
				type = TypeSystemServices.Map(GetEnumUnderlyingType(type));
			}

			if (TypeSystemServices.IntType == type)
			{
				return OpCodes.Ldelem_I4;
			}
			if (TypeSystemServices.UIntType == type)
			{
				return OpCodes.Ldelem_U4;
			}				
			if (TypeSystemServices.LongType == type)
			{
				return OpCodes.Ldelem_I8;
			}	
			if (TypeSystemServices.SByteType == type)
			{
				return OpCodes.Ldelem_I1;
			}				
			if (TypeSystemServices.ByteType == type)
			{
				return OpCodes.Ldelem_U1;
			}
			if (TypeSystemServices.ShortType == type ||
			    TypeSystemServices.CharType == type)
			{
				return OpCodes.Ldelem_I2;
			}
			if (TypeSystemServices.UShortType == type)
			{
				return OpCodes.Ldelem_U2;
			}				
			if (TypeSystemServices.SingleType == type)
			{
				return OpCodes.Ldelem_R4;
			}
			if (TypeSystemServices.DoubleType == type)
			{
				return OpCodes.Ldelem_R8;
			}
			//NotImplemented("LoadEntityOpCode(" + tag + ")");
			return OpCodes.Ldelema;
		}
		
		OpCode GetStoreEntityOpCode(IType tag)
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
					return OpCodes.Stelem_I4;
				}
				else if (TypeSystemServices.LongType == tag ||
				    TypeSystemServices.ULongType == tag)
				{
					return OpCodes.Stelem_I8;
				}
				else if (TypeSystemServices.ShortType == tag ||
				    TypeSystemServices.CharType == tag)
				{
					return OpCodes.Stelem_I2;
				}
				else if (TypeSystemServices.ByteType == tag ||
				    TypeSystemServices.SByteType == tag)
				{
					return OpCodes.Stelem_I1;
				}
				else if (TypeSystemServices.SingleType == tag)
				{
					return OpCodes.Stelem_R4;
				}
				else if (TypeSystemServices.DoubleType == tag)
				{
					return OpCodes.Stelem_R8;
				}
				return OpCodes.Stobj;
			}
			
			return OpCodes.Stelem_Ref;
		}
		
		OpCode GetLoadRefParamCode(IType tag)
		{
			if (tag.IsValueType)
			{
				if (tag.IsEnum)
				{
					tag = TypeSystemServices.Map(GetEnumUnderlyingType(tag));
				}
				if (TypeSystemServices.IntType == tag)
				{
					return OpCodes.Ldind_I4;
				}
				if (TypeSystemServices.LongType == tag ||
				    TypeSystemServices.ULongType == tag)
				{
					return OpCodes.Ldind_I8;
				}
				if (TypeSystemServices.ByteType == tag)
				{
					return OpCodes.Ldind_U1;
				}
				if (TypeSystemServices.ShortType == tag ||
				    TypeSystemServices.CharType == tag)
				{
					return OpCodes.Ldind_I2;
				}
				if (TypeSystemServices.SingleType == tag)
				{
					return OpCodes.Ldind_R4;
				}
				if (TypeSystemServices.DoubleType == tag)
				{
					return OpCodes.Ldind_R8;
				}
				if (TypeSystemServices.UShortType == tag)
				{
					return OpCodes.Ldind_U2;
				}
				if (TypeSystemServices.UIntType == tag)
				{
					return OpCodes.Ldind_U4;
				}

				return OpCodes.Ldobj;
			}
			return OpCodes.Ldind_Ref;
		}
		
		OpCode GetStoreRefParamCode(IType tag)
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
					return OpCodes.Stind_I4;
				}
				if (TypeSystemServices.LongType == tag
				    || TypeSystemServices.ULongType == tag)
				{
					return OpCodes.Stind_I8;
				}
				if (TypeSystemServices.ByteType == tag)
				{
					return OpCodes.Stind_I1;
				}
				if (TypeSystemServices.ShortType == tag ||
				    TypeSystemServices.CharType == tag)
				{
					return OpCodes.Stind_I2;
				}
				if (TypeSystemServices.SingleType == tag)
				{
					return OpCodes.Stind_R4;
				}
				if (TypeSystemServices.DoubleType == tag)
				{
					return OpCodes.Stind_R8;
				}
				
				return OpCodes.Stobj;
			}
			return OpCodes.Stind_Ref;
		}
		
		bool IsAssignableFrom(IType expectedType, IType actualType)
		{
			return (IsPtr(expectedType) && IsPtr(actualType))
				|| expectedType.IsAssignableFrom(actualType);
		}
		
		bool IsPtr(IType type)
		{
			return (type == TypeSystemServices.IntPtrType)
				|| (type == TypeSystemServices.UIntPtrType);
		}
		
		void EmitCastIfNeeded(IType expectedType, IType actualType)
		{
			if (null == actualType) // see NullLiteralExpression
				return;
			if (expectedType.IsPointer || actualType.IsPointer) //no cast needed for addresses
				return;

			if (!IsAssignableFrom(expectedType, actualType))
			{
				IMethod method = TypeSystemServices.FindImplicitConversionOperator(actualType,expectedType);
				if (method != null)
				{
					EmitBoxIfNeeded(method.GetParameters()[0].Type, actualType);
					_il.EmitCall(OpCodes.Call, GetMethodInfo(method), null);
					return;
				}
				
				if (expectedType is IGenericParameter)
				{
					// Since expected type is a generic parameter, we don't know whether to emit 
					// an unbox opcode or a castclass opcode; so we emit an unbox.any opcode which
					// works as either of those at runtime
					_il.Emit(OpCodes.Unbox_Any, GetSystemType(expectedType));
				}
				else if (expectedType.IsValueType)
				{
					if (actualType.IsValueType)
					{
						// numeric promotion
						if (TypeSystemServices.DecimalType == expectedType)
						{
							_il.EmitCall(OpCodes.Call, GetToDecimalConversionMethod(actualType), null);
						}
						else if (TypeSystemServices.DecimalType == actualType)
						{
							_il.EmitCall(OpCodes.Call, GetFromDecimalConversionMethod(expectedType), null);
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
								_il.Emit(GetNumericPromotionOpCode(expectedType));
						}
					}
					else
					{
						// To get a value type out of a reference type we emit an unbox opcode
						EmitUnbox(expectedType);
					}
				}
				else
				{
					EmitRuntimeCoercionIfNeeded(expectedType, actualType);

					// In order to cast to a reference type we emit a castclass opcode
					_context.TraceInfo("castclass: expected type='{0}', type on stack='{1}'", expectedType, actualType);
					_il.Emit(OpCodes.Castclass, GetSystemType(expectedType));
				}
			}
			else
			{
				EmitBoxIfNeeded(expectedType, actualType);
			}
		}

		private void EmitRuntimeCoercionIfNeeded(IType expectedType, IType actualType)
		{
			if (TypeSystemServices.IsDuckType(actualType))
			{
				EmitGetTypeFromHandle(GetSystemType(expectedType)); PopType();
				_il.EmitCall(OpCodes.Call, RuntimeServices_Coerce, null);
			}
		}

		private MethodInfo _RuntimeServices_Coerce;
		
		private MethodInfo RuntimeServices_Coerce
		{
			get
			{
				if (_RuntimeServices_Coerce != null) return _RuntimeServices_Coerce;
				return _RuntimeServices_Coerce = Types.RuntimeServices.GetMethod("Coerce", new Type[] { Types.Object, Types.Type });
			}
		}

		private void EmitBoxIfNeeded(IType expectedType, IType actualType)
		{
			if ((actualType.IsValueType && !expectedType.IsValueType) ||
				(actualType is IGenericParameter && !(expectedType is IGenericParameter)))
			{
				EmitBox(actualType);
			}
		}

		void EmitBox(IType type)
		{
			_il.Emit(OpCodes.Box, GetSystemType(type));
		}
		
		void EmitUnbox(IType expectedType)
		{
			string unboxMethodName = GetUnboxMethodName(expectedType);
			if (null != unboxMethodName)
			{
				_il.EmitCall(OpCodes.Call, GetRuntimeMethod(unboxMethodName), null);
			}
			else
			{
				Type type = GetSystemType(expectedType);
				_il.Emit(OpCodes.Unbox, type);
				_il.Emit(OpCodes.Ldobj, type);
			}
		}
		
		MethodInfo GetRuntimeMethod(string methodName)
		{
			return Types.RuntimeServices.GetMethod(methodName);
		}
		
		string GetUnboxMethodName(IType type)
		{
			if (type == TypeSystemServices.ByteType) return "UnboxByte";
			if (type == TypeSystemServices.SByteType) return "UnboxSByte";
			if (type == TypeSystemServices.ShortType) return "UnboxInt16";
			if (type == TypeSystemServices.UShortType) return "UnboxUInt16";
			if (type == TypeSystemServices.IntType) return "UnboxInt32";
			if (type == TypeSystemServices.UIntType) return "UnboxUInt32";
			if (type == TypeSystemServices.LongType) return "UnboxInt64";
			if (type == TypeSystemServices.ULongType) return "UnboxUInt64";
			if (type == TypeSystemServices.SingleType) return "UnboxSingle";
			if (type == TypeSystemServices.DoubleType) return "UnboxDouble";
			if (type == TypeSystemServices.DecimalType) return "UnboxDecimal";
			if (type == TypeSystemServices.BoolType) return "UnboxBoolean";
			if (type == TypeSystemServices.CharType) return "UnboxChar";
			return null;
		}
		
		OpCode GetNumericPromotionOpCode(IType type)
		{
			if (type.IsEnum)
			{
				type = TypeSystemServices.Map(GetEnumUnderlyingType(type));
			}
			else if (type == TypeSystemServices.SByteType)
			{
				return _checked ? OpCodes.Conv_Ovf_I1 : OpCodes.Conv_I1;
			}
			else if (type == TypeSystemServices.ByteType)
			{
				return _checked ? OpCodes.Conv_Ovf_U1 : OpCodes.Conv_U1;
			}
			else if (type == TypeSystemServices.ShortType)
			{
				return _checked ? OpCodes.Conv_Ovf_I2 : OpCodes.Conv_I2;
			}
			else if (type == TypeSystemServices.UShortType ||
			         type == TypeSystemServices.CharType)
			{
				return _checked ? OpCodes.Conv_Ovf_U2 : OpCodes.Conv_U2;
			}
			if (type == TypeSystemServices.IntType)
			{
				return _checked ? OpCodes.Conv_Ovf_I4 : OpCodes.Conv_I4;
			}
			else if (type == TypeSystemServices.UIntType)
			{
				return _checked ? OpCodes.Conv_Ovf_U4 :OpCodes.Conv_U4;
			}
			else if (type == TypeSystemServices.LongType)
			{
				return _checked ? OpCodes.Conv_Ovf_I8 : OpCodes.Conv_I8;
			}
			else if (type == TypeSystemServices.ULongType)
			{
				return _checked ? OpCodes.Conv_Ovf_U8 :OpCodes.Conv_U8;
			}
			else if (type == TypeSystemServices.SingleType)
			{
				return OpCodes.Conv_R4;
			}
			else if (type == TypeSystemServices.DoubleType)
			{
				return OpCodes.Conv_R8;
			}
			else
			{
				throw new NotImplementedException(string.Format("Numeric promotion for {0} not implemented!", type));
			}
		}
		
		void StoreEntity(OpCode opcode, int index, Node value, IType elementType)
		{
			_il.Emit(OpCodes.Dup);	// array reference
			EmitLoadLiteral(index); // element index
			
			bool stobj = IsStobj(opcode); // value type sequence?
			if (stobj)
			{
				Type systemType = GetSystemType(elementType);
				_il.Emit(OpCodes.Ldelema, systemType);
				value.Accept(this);
				EmitCastIfNeeded(elementType, PopType());	// might need to cast to decimal
				_il.Emit(opcode, systemType);
			}
			else
			{
				value.Accept(this);
				EmitCastIfNeeded(elementType, PopType());
				_il.Emit(opcode);
			}
		}

		bool IsStobj(OpCode code)
		{
			return OpCodes.Stobj.Value == code.Value;
		}
		
		void DefineAssemblyAttributes()
		{
			foreach (Attribute attribute in _assemblyAttributes)
			{
				_asmBuilder.SetCustomAttribute(GetCustomAttributeBuilder(attribute));
			}
		}
		
		CustomAttributeBuilder CreateDebuggableAttribute()
		{
			return new CustomAttributeBuilder(
				DebuggableAttribute_Constructor,
				new object[] { true, true });
		}
		
		CustomAttributeBuilder CreateRuntimeCompatibilityAttribute()
		{
			return new CustomAttributeBuilder(
					RuntimeCompatibilityAttribute_Constructor, new object[0],
					RuntimeCompatibilityAttribute_Property, new object[] { true });
		}

		CustomAttributeBuilder CreateSerializableAttribute()
		{
			return new CustomAttributeBuilder(
				SerializableAttribute_Constructor,
				new object[0]);
		}

		CustomAttributeBuilder CreateUnverifiableCodeAttribute()
		{
			return new CustomAttributeBuilder(
				typeof(UnverifiableCodeAttribute).GetConstructor(Type.EmptyTypes), new object[0]);
		}

		CustomAttributeBuilder CreateSecurityPermissionAttribute(string permission)
		{
			return new CustomAttributeBuilder(
				typeof(SecurityPermissionAttribute).GetConstructor(new Type[] { typeof(SecurityAction) }),
				new object[] { SecurityAction.RequestMinimum },
				new PropertyInfo[] { typeof(SecurityPermissionAttribute).GetProperty("SkipVerification") },
				new object[] { true });
		}

		void DefineEntryPoint()
		{
			if (Context.Parameters.GenerateInMemory)
			{
				Context.GeneratedAssembly = _asmBuilder;
			}
			
			if (CompilerOutputType.Library != Parameters.OutputType)
			{
				Method method = ContextAnnotations.GetEntryPoint(Context);
				if (null != method)
				{
					MethodInfo entryPoint = Context.Parameters.GenerateInMemory
						? _asmBuilder.GetType(method.DeclaringType.FullName).GetMethod(method.Name, BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static)
						: GetMethodBuilder(method);
					_asmBuilder.SetEntryPoint(entryPoint, (PEFileKinds)Parameters.OutputType);
				}
				else
				{
					Errors.Add(CompilerErrorFactory.NoEntryPoint());
				}
			}
		}
		
		Type[] GetParameterTypes(ParameterDeclarationCollection parameters)
		{
			Type[] types = new Type[parameters.Count];
			for (int i=0; i<types.Length; ++i)
			{
				types[i] = GetSystemType(parameters[i].Type);
				if (parameters[i].IsByRef && !types[i].IsByRef)
				{
					types[i] = types[i].MakeByRefType();
				}
			}
			return types;
		}
		
		Hashtable _builders = new Hashtable();
		
		void SetBuilder(Node node, object builder)
		{
			if (null == builder)
			{
				throw new ArgumentNullException("type");
			}
			_builders[node] = builder;
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
		
		ConstructorBuilder GetConstructorBuilder(Method method)
		{
			return (ConstructorBuilder)_builders[method];
		}
		
		LocalBuilder GetLocalBuilder(Node local)
		{
			return GetInternalLocal(local).LocalBuilder;
		}
		
		PropertyInfo GetPropertyInfo(IEntity tag)
		{
			ExternalProperty external = tag as ExternalProperty;
			if (null != external)
			{
				return external.PropertyInfo;
			}
			return GetPropertyBuilder(((InternalProperty)tag).Property);
		}
		
		FieldInfo GetFieldInfo(IField tag)
		{
			// If field is external, get its existing FieldInfo
			ExternalField external = tag as ExternalField;
			if (null != external)
			{
				return external.FieldInfo;
			}

			// If field is mapped from a generic type, get its mapped FieldInfo
			// on the constructed type
            GenericMappedField mapped = tag as GenericMappedField;
            if (mapped != null)
            {
                return GetMappedFieldInfo(mapped.DeclaringType, mapped.SourceMember);
            }

            // If field is internal, get its FieldBuilder
			return GetFieldBuilder(((InternalField)tag).Field);
		}
		
		MethodInfo GetMethodInfo(IMethod entity)
		{
			// If method is external, get its existing MethodInfo
			ExternalMethod external = entity as ExternalMethod;
			if (null != external)
			{
				return (MethodInfo)external.MethodInfo;
			}

			// If method is a constructed generic method, get its MethodInfo from its definition
			if (entity is GenericConstructedMethod)
			{
				return GetConstructedMethodInfo(entity.ConstructedInfo);
			}

			// If method is mapped from a generic type, get its MethodInfo on the constructed type
			GenericMappedMethod mapped = entity as GenericMappedMethod;
			if (mapped != null)
			{
				return GetMappedMethodInfo(mapped.DeclaringType, mapped.SourceMember);
			}

			// If method is internal, get its MethodBuilder
			return GetMethodBuilder(((InternalMethod)entity).Method);
		}

		ConstructorInfo GetConstructorInfo(IConstructor entity)
		{
            // If constructor is external, get its existing ConstructorInfo
            ExternalConstructor external = entity as ExternalConstructor;
            if (null != external)
            {
                return external.ConstructorInfo;
            }

            // If constructor is mapped from a generic type, get its ConstructorInfo on the constructed type
            GenericMappedConstructor mapped = entity as GenericMappedConstructor;
            if (mapped != null)
            {
                return TypeBuilder.GetConstructor(GetSystemType(mapped.DeclaringType), GetConstructorInfo((IConstructor)mapped.SourceMember));
            }

            // If constructor is internal, get its MethodBuilder
			return GetConstructorBuilder(((InternalMethod)entity).Method);
		}
		
		/// <summary>
		/// Retrieves the MethodInfo for a generic constructed method.
		/// </summary>
		private MethodInfo GetConstructedMethodInfo(IConstructedMethodInfo constructedInfo)
		{
			Type[] arguments = Array.ConvertAll<IType, Type>(
				constructedInfo.GenericArguments,
				GetSystemType);
				
			return GetMethodInfo(constructedInfo.GenericDefinition).MakeGenericMethod(arguments);
		}

		/// <summary>
        /// Retrieves the FieldInfo for a field as mapped on a generic type.
		/// </summary>
		private FieldInfo GetMappedFieldInfo(IType targetType, IField source)
		{
            FieldInfo fi = GetFieldInfo(source);
			if (!fi.DeclaringType.IsGenericTypeDefinition)
			{
				// HACK: .NET Reflection doesn't allow calling TypeBuilder.GetField(Type, FieldInfo)
				// on types that aren't generic definitions (like open constructed types), so we have 
                // to manually find the corresponding FieldInfo on the declaring type's definition 
                // before mapping it
                Type definition = fi.DeclaringType.GetGenericTypeDefinition();
                fi = definition.GetField(fi.Name);
			}
            return TypeBuilder.GetField(GetSystemType(targetType), fi);
		}

        /// <summary>
        /// Retrieves the MethodInfo for a method as mapped on a generic type.
        /// </summary>
        private MethodInfo GetMappedMethodInfo(IType targetType, IMethod source)
        {
            MethodInfo mi = GetMethodInfo(source);
            if (!mi.DeclaringType.IsGenericTypeDefinition)
            {
                // HACK: .NET Reflection doesn't allow calling TypeBuilder.GetMethod(Type, MethodInfo) 
                // on types that aren't generic definitions (like open constructed types), so we have to 
                // manually find the corresponding MethodInfo on the declaring type's definition before 
                // mapping it
                Type definition = mi.DeclaringType.GetGenericTypeDefinition();
                mi = Array.Find<MethodInfo>(
                    definition.GetMethods(),
                    delegate(MethodInfo other) { return other.MetadataToken == mi.MetadataToken; });
            }

            return TypeBuilder.GetMethod(GetSystemType(targetType), mi);
        }
        
        /// <summary>
        /// Retrieves the ConstructorInfo for a constructor as mapped on a generic type.
        /// </summary>
		private ConstructorInfo GetMappedConstructorInfo(IType targetType, IConstructor source)
		{
            ConstructorInfo ci = GetConstructorInfo(source);
			if (!ci.DeclaringType.IsGenericTypeDefinition)
			{
				// HACK: .NET Reflection doesn't allow calling
				// TypeBuilder.GetConstructor(Type, ConstructorInfo) on types that aren't generic
				// definitions, so we have to manually find the corresponding ConstructorInfo on the
				// declaring type's definition before mapping it
				Type definition = ci.DeclaringType.GetGenericTypeDefinition();
				ci = Array.Find<ConstructorInfo>(
					definition.GetConstructors(),
					delegate(ConstructorInfo other) { return other.MetadataToken == ci.MetadataToken; });
			}

			return TypeBuilder.GetConstructor(GetSystemType(targetType), ci);
		}
		
		Type GetSystemType(Node node)
		{
			return GetSystemType(GetType(node));
		}
		
		Type GetSystemType(IType tag)
		{
			Type type = (Type)_typeCache[tag];
			if (type != null)
			{
				return type;
			}

			ExternalType external = tag as ExternalType;
			if (null != external)
			{
				type = external.ActualType;
			}
    		else if (tag.IsArray)
			{
				IArrayType arrayType = (IArrayType)tag;
				Type systemType = GetSystemType(arrayType.GetElementType());
				int rank = arrayType.GetArrayRank();
				
				if (rank == 1)
				{
					type = systemType.MakeArrayType();
				}
				else
				{
					type = systemType.MakeArrayType(rank);
				}
			}
			else if (Null.Default == tag)
			{
				type = Types.Object;
			}
            else if (tag.ConstructedInfo != null)
            {
                // Type is a constructed generic type - create it using its definition's system type
                Type[] arguments = Array.ConvertAll<IType, Type>(tag.ConstructedInfo.GenericArguments, GetSystemType);
                type = GetSystemType(tag.ConstructedInfo.GenericDefinition).MakeGenericType(arguments);
            }
            else if (tag is InternalGenericParameter)
            {
                return (Type)GetBuilder(((InternalGenericParameter)tag).Node);
            }
            else if (tag is AbstractInternalType)
            {
                TypeDefinition typedef = ((AbstractInternalType) tag).TypeDefinition;
                type = (Type)GetBuilder(typedef);

				if (null != tag.GenericInfo && !type.IsGenericType) //hu-oh, early-bound
					DefineGenericParameters(typedef);

				if (tag.IsPointer && null != type)
					type = type.MakePointerType();
            }

			if (null == type)
			{
				throw new InvalidOperationException(string.Format("Could not find a Type for {0}.", tag));
			}

			_typeCache.Add(tag, type);
			
			return type;
		}
		
		TypeAttributes GetNestedTypeAttributes(TypeMember type)
		{
			return GetExtendedTypeAttributes(GetNestedTypeAccessibility(type), type);
		}
		
		TypeAttributes GetNestedTypeAccessibility(TypeMember type)
		{
			if (type.IsPublic) return TypeAttributes.NestedPublic;
			if (type.IsInternal) return TypeAttributes.NestedAssembly;
			return TypeAttributes.NestedPrivate;
		}
		
		TypeAttributes GetTypeAttributes(TypeMember type)
		{
			return GetExtendedTypeAttributes(GetTypeVisibilityAttributes(type), type);
		}

		private TypeAttributes GetTypeVisibilityAttributes(TypeMember type)
		{
			return type.IsPublic ? TypeAttributes.Public : TypeAttributes.NotPublic;
		}

		TypeAttributes GetExtendedTypeAttributes(TypeAttributes attributes, TypeMember type)
		{
			switch (type.NodeType)
			{
				case NodeType.ClassDefinition:
					{
						attributes |= (TypeAttributes.AnsiClass | TypeAttributes.AutoLayout);
						attributes |= TypeAttributes.Class;

						if (!((ClassDefinition) type).HasDeclaredStaticConstructor)
						{
							attributes |= TypeAttributes.BeforeFieldInit;
						}
						if (!type.IsTransient)
						{
							attributes |= TypeAttributes.Serializable;
						}
						if (type.IsAbstract)
						{
							attributes |= TypeAttributes.Abstract;
						}
						if (type.IsFinal)
						{
							attributes |= TypeAttributes.Sealed;
						}
						if (type.IsStatic) //static type is Sealed+Abstract in SRE
						{
							attributes |= TypeAttributes.Sealed | TypeAttributes.Abstract;
						}
						if (((IType)type.Entity).IsValueType)
						{
							attributes |= TypeAttributes.SequentialLayout;
						}
						break;
					}
					
				case NodeType.EnumDefinition:
					{
						attributes |= TypeAttributes.Sealed;
						attributes |= TypeAttributes.Serializable;
						break;
					}
					
				case NodeType.InterfaceDefinition:
					{
						attributes |= (TypeAttributes.Interface | TypeAttributes.Abstract);
						break;
					}
					
				case NodeType.Module:
					{
						attributes |= TypeAttributes.Sealed;
						break;
					}
			}
			return attributes;
		}
		
		PropertyAttributes GetPropertyAttributes(Property property)
		{
			PropertyAttributes attributes = PropertyAttributes.None;

			if (property.ExplicitInfo != null)
			{
				attributes |= PropertyAttributes.SpecialName | PropertyAttributes.RTSpecialName;
			}
			return attributes;
		}
		
		MethodAttributes GetMethodAttributesFromTypeMember(TypeMember member)
		{
			MethodAttributes attributes = (MethodAttributes)0;
			if (member.IsPublic)
			{
				attributes = MethodAttributes.Public;
			}
			else if (member.IsProtected)
			{
				attributes = member.IsInternal
					? MethodAttributes.FamORAssem
					: MethodAttributes.Family;
			}
			else if (member.IsPrivate)
			{
				attributes = MethodAttributes.Private;
			}
			else if (member.IsInternal)
			{
				attributes = MethodAttributes.Assembly;
			}
			if (member.IsStatic)
			{
				attributes |= MethodAttributes.Static;
				
				if (member.Name.StartsWith("op_"))
				{
					attributes |= MethodAttributes.SpecialName;
				}
			}
			if (member.IsFinal)
			{
				attributes |= MethodAttributes.Final;
			}
			if (member.IsAbstract)
			{
				attributes |= (MethodAttributes.Abstract | MethodAttributes.Virtual);
			}
			if (member.IsVirtual || member.IsOverride)
			{
				attributes |= MethodAttributes.Virtual;
			}
			return attributes;
		}
		
		MethodAttributes GetPropertyMethodAttributes(TypeMember property)
		{
			MethodAttributes attributes = MethodAttributes.SpecialName | MethodAttributes.HideBySig;
			attributes |= GetMethodAttributesFromTypeMember(property);
			return attributes;
		}
		
		MethodAttributes GetMethodAttributes(Method method)
		{
			MethodAttributes attributes = MethodAttributes.HideBySig;
			if (method.ExplicitInfo != null)
			{
				attributes |= MethodAttributes.NewSlot;
			}
			if (IsPInvoke(method))
			{
				Debug.Assert(method.IsStatic);
				attributes |= MethodAttributes.PinvokeImpl;
			}
			attributes |= GetMethodAttributesFromTypeMember(method);
			return attributes;
		}
		
		FieldAttributes GetFieldAttributes(Field field)
		{
			FieldAttributes attributes = 0;
			if (field.IsProtected)
			{
				attributes |= FieldAttributes.Family;
			}
			else if (field.IsPublic)
			{
				attributes |= FieldAttributes.Public;
			}
			else if (field.IsPrivate)
			{
				attributes |= FieldAttributes.Private;
			}
			else if (field.IsInternal)
			{
				attributes |= FieldAttributes.Assembly;
			}
			if (field.IsStatic)
			{
				attributes |= FieldAttributes.Static;
			}
			if (field.IsTransient)
			{
				attributes |= FieldAttributes.NotSerialized;
			}
			if (field.IsFinal)
			{
				IField entity = (IField)field.Entity;
				if (entity.IsLiteral)
				{
					attributes |= FieldAttributes.Literal;
				}
				else
				{
					attributes |= FieldAttributes.InitOnly;
				}
			}
			return attributes;
		}

		static readonly Type IsVolatileType = typeof(System.Runtime.CompilerServices.IsVolatile);

		Type[] GetFieldRequiredCustomModifiers(Field field)
		{
			if (field.IsVolatile)
				return new Type[] { IsVolatileType };
			return Type.EmptyTypes;
		}

		ParameterAttributes GetParameterAttributes(ParameterDeclaration param)
		{
			return ParameterAttributes.None;
		}
		
		void DefineEvent(TypeBuilder typeBuilder, Event node)
		{
			EventBuilder builder = typeBuilder.DefineEvent(node.Name,
			                                               EventAttributes.None,
			                                               GetSystemType(node.Type));
			//MethodAttributes attribs = GetPropertyMethodAttributes(node);
			MethodAttributes baseAttributes = MethodAttributes.SpecialName;
			builder.SetAddOnMethod(DefineMethod(typeBuilder, node.Add, baseAttributes|GetMethodAttributes(node.Add)));
			builder.SetRemoveOnMethod(DefineMethod(typeBuilder, node.Remove, baseAttributes|GetMethodAttributes(node.Remove)));

			if (null != node.Raise)
			{
				builder.SetRaiseMethod(DefineMethod(typeBuilder, node.Raise, baseAttributes|GetMethodAttributes(node.Raise)));
			}

			SetBuilder(node, builder);
		}
		
		void DefineProperty(TypeBuilder typeBuilder, Property property)
		{
			string name;
			if (property.ExplicitInfo != null)
			{
				name = property.ExplicitInfo.InterfaceType.Name + "." + property.Name;
			}
			else
			{
				name = property.Name;
			}

			PropertyBuilder builder = typeBuilder.DefineProperty(name,
			                                                     GetPropertyAttributes(property),
			                                                     GetSystemType(property.Type),
			                                                     GetParameterTypes(property.Parameters));
			Method getter = property.Getter;
			Method setter = property.Setter;

			if (null != getter)
			{
				if (!getter.IsVisibilitySet)
					getter.Visibility = property.Visibility;

				MethodBuilder getterBuilder =
					DefineMethod(typeBuilder, getter, GetPropertyMethodAttributes(getter));
				builder.SetGetMethod(getterBuilder);
			}
			if (null != setter)
			{
				if (!setter.IsVisibilitySet)
					setter.Visibility = property.Visibility;

				MethodBuilder setterBuilder =
					DefineMethod(typeBuilder, setter, GetPropertyMethodAttributes(setter));
				builder.SetSetMethod(setterBuilder);
			}
			bool isDuckTyped = GetEntity(property).IsDuckTyped;
			if (isDuckTyped)
			{
				builder.SetCustomAttribute(CreateDuckTypedCustomAttribute());
			}
			
			SetBuilder(property, builder);
		}
		
		void DefineField(TypeBuilder typeBuilder, Field field)
		{
			FieldBuilder builder = typeBuilder.DefineField(field.Name,
			                                               GetSystemType(field),
			                                               GetFieldRequiredCustomModifiers(field),
			                                               Type.EmptyTypes,
			                                               GetFieldAttributes(field));
			SetBuilder(field, builder);
		}
		
		delegate ParameterBuilder ParameterFactory(int index, System.Reflection.ParameterAttributes attributes, string name);
		
		void DefineParameters(ConstructorBuilder builder, ParameterDeclarationCollection parameters)
		{
			DefineParameters(parameters, new ParameterFactory(builder.DefineParameter));
		}
		
		void DefineParameters(MethodBuilder builder, ParameterDeclarationCollection parameters)
		{
			DefineParameters(parameters, new ParameterFactory(builder.DefineParameter));
		}
		
		void DefineParameters(ParameterDeclarationCollection parameters, ParameterFactory defineParameter)
		{
			for (int i=0; i<parameters.Count; ++i)
			{
				ParameterBuilder paramBuilder = defineParameter(i+1, GetParameterAttributes(parameters[i]), parameters[i].Name);
				if (parameters[i].IsParamArray)
				{
					SetParamArrayAttribute(paramBuilder);
				}
				SetBuilder(parameters[i], paramBuilder);
			}
		}
		
		void SetParamArrayAttribute(ParameterBuilder builder)
		{
			builder.SetCustomAttribute(
				new CustomAttributeBuilder(
					ParamArrayAttribute_Constructor,
					new object[0]));
			
		}
		
		MethodImplAttributes GetImplementationFlags(Method method)
		{
			MethodImplAttributes flags = MethodImplAttributes.Managed;
			if (method.IsRuntime)
			{
				flags |= MethodImplAttributes.Runtime;
			}
			return flags;
		}

		MethodBuilder DefineMethod(TypeBuilder typeBuilder, Method method, MethodAttributes attributes)
		{
			ParameterDeclarationCollection parameters = method.Parameters;
			
			MethodAttributes methodAttributes = GetMethodAttributes(method) | attributes;
			
			string name;
			if (method.ExplicitInfo != null)
			{
				name = method.ExplicitInfo.InterfaceType.Name + "." + method.Name;
			}
			else
			{
				name = method.Name;
			}

			MethodBuilder builder = typeBuilder.DefineMethod(name,
			                                                 methodAttributes);

			if (method.GenericParameters.Count != 0)
			{
				DefineGenericParameters(builder, method.GenericParameters.ToArray());
			}
			
			builder.SetParameters(GetParameterTypes(parameters));

			IType returnType = GetEntity(method).ReturnType;
			if (IsPInvoke(method) && TypeSystemServices.IsUnknown(returnType))
			{
				returnType = TypeSystemServices.VoidType;
			}
			builder.SetReturnType(GetSystemType(returnType));

			builder.SetImplementationFlags(GetImplementationFlags(method));
			
			DefineParameters(builder, parameters);
			
			SetBuilder(method, builder);
			
			IMethod methodEntity = GetEntity(method);
			if (methodEntity.IsDuckTyped)
			{
				builder.SetCustomAttribute(CreateDuckTypedCustomAttribute());
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
			string[] names = Array.ConvertAll<GenericParameterDeclaration, string>(
				parameters,
				delegate(GenericParameterDeclaration gpd) { return gpd.Name; });

			GenericTypeParameterBuilder[] builders = builder.DefineGenericParameters(names);

			DefineGenericParameters(builders, parameters);
		}

		/// <summary>
		/// Defines the generic parameters of an internal generic method.
		/// </summary>
		void DefineGenericParameters(MethodBuilder builder, GenericParameterDeclaration[] parameters)
		{
			string[] names = Array.ConvertAll<GenericParameterDeclaration, string>(
				parameters,
				delegate(GenericParameterDeclaration gpd) { return gpd.Name; });

			GenericTypeParameterBuilder[] builders = builder.DefineGenericParameters(names);

			DefineGenericParameters(builders, parameters);
		}

		void DefineGenericParameters(GenericTypeParameterBuilder[] builders, GenericParameterDeclaration[] declarations)
		{
			for (int i = 0; i < builders.Length; i++)
			{
				SetBuilder(declarations[i], builders[i]);
				DefineGenericParameter(((InternalGenericParameter)declarations[i].Entity), builders[i]);
			}
		}

		private void DefineGenericParameter(InternalGenericParameter parameter, GenericTypeParameterBuilder builder)
		{
			// Set base type constraint
			if (parameter.BaseType != TypeSystemServices.ObjectType)
			{
				builder.SetBaseTypeConstraint(GetSystemType(parameter.BaseType));
			}

			// Set interface constraints
			Type[] interfaceTypes = Array.ConvertAll<IType, Type>(
				parameter.GetInterfaces(), GetSystemType);

			builder.SetInterfaceConstraints(interfaceTypes);

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

		private CustomAttributeBuilder CreateDuckTypedCustomAttribute()
		{
			return new CustomAttributeBuilder(DuckTypedAttribute_Constructor, new object[0]);
		}

		void DefineConstructor(TypeBuilder typeBuilder, Method constructor)
		{
			ConstructorBuilder builder = typeBuilder.DefineConstructor(GetMethodAttributes(constructor),
			                                                           CallingConventions.Standard,
			                                                           GetParameterTypes(constructor.Parameters));
			
			builder.SetImplementationFlags(GetImplementationFlags(constructor));
			DefineParameters(builder, constructor.Parameters);
			
			SetBuilder(constructor, builder);
		}
		
		bool IsEnumDefinition(TypeMember type)
		{
			return NodeType.EnumDefinition == type.NodeType;
		}

		void DefineType(TypeDefinition typeDefinition)
		{
			SetBuilder(typeDefinition, CreateTypeBuilder(typeDefinition));
		}
		
		bool IsValueType(TypeMember type)
		{
			IType entity = type.Entity as IType;
			return null != entity && entity.IsValueType;
		}

		object CreateTypeBuilder(TypeDefinition type)
		{
			Type baseType = null;
			if (IsEnumDefinition(type))
			{
				baseType = typeof(Enum);
			}
			else if (IsValueType(type))
			{
				baseType = Types.ValueType;
			}

			TypeBuilder typeBuilder = null;
			ClassDefinition enclosingType = type.ParentNode as ClassDefinition;
			EnumDefinition enumDef = type as EnumDefinition;

			if (null == enclosingType)
			{
				if (IsEnumDefinition(type))
				{
					//have to normalize TypeAttributes for EnumBuilder only
					EnumBuilder enumBuilder = _moduleBuilder.DefineEnum(
						type.QualifiedName,
						GetTypeVisibilityAttributes(type),
						GetEnumUnderlyingType(enumDef));
					enumBuilder.SetCustomAttribute(CreateSerializableAttribute());
					return enumBuilder;
				}
				else
				{
					typeBuilder = _moduleBuilder.DefineType(
						AnnotateGenericTypeName(type, type.QualifiedName),
						GetTypeAttributes(type),
						baseType);
				}
			}
			else
			{
				typeBuilder = GetTypeBuilder(enclosingType).DefineNestedType(
					AnnotateGenericTypeName(type, type.Name), 
					GetNestedTypeAttributes(type), 
					baseType);
			}

			if (IsEnumDefinition(type))
			{
				// Mono cant construct enum array types unless
				// the fields is already defined
				typeBuilder.DefineField("value__",
								GetEnumUnderlyingType(enumDef),
								FieldAttributes.Public |
								FieldAttributes.SpecialName |
								FieldAttributes.RTSpecialName);
			}

			return typeBuilder;
		}
		
		private string AnnotateGenericTypeName(TypeDefinition typeDef, string name)
		{
			if (typeDef.HasGenericParameters)
			{
				return name + "`" + typeDef.GenericParameters.Count;
			}
			return name;
		}

		void EmitBaseTypesAndAttributes(TypeDefinition typeDefinition, TypeBuilder typeBuilder)
		{
			foreach (TypeReference baseType in typeDefinition.BaseTypes)
			{
				Type type = GetSystemType(baseType);
				
				// For some reason you can't call IsClass on constructed types created at compile time,
				// so we'll ask the generic definition instead
				if ((type.IsGenericType && type.GetGenericTypeDefinition().IsClass) || (type.IsClass))
				{
					typeBuilder.SetParent(type);
				}
				else
				{
					typeBuilder.AddInterfaceImplementation(type);
				}
			}
		}
		
		void NotImplemented(string feature)
		{
			throw new NotImplementedException(feature);
		}
		
		CustomAttributeBuilder GetCustomAttributeBuilder(Attribute node)
		{
			IConstructor constructor = (IConstructor)GetEntity(node);
			ConstructorInfo constructorInfo = GetConstructorInfo(constructor);
			object[] constructorArgs = GetValues(constructor.GetParameters(),
			                                     node.Arguments);
			
			ExpressionPairCollection namedArgs = node.NamedArguments;
			if (namedArgs.Count > 0)
			{
				PropertyInfo[] namedProperties;
				object[] propertyValues;
				FieldInfo[] namedFields;
				object[] fieldValues;
				GetNamedValues(namedArgs,
				               out namedProperties, out propertyValues,
				               out namedFields, out fieldValues);
				return new CustomAttributeBuilder(
					constructorInfo, constructorArgs,
					namedProperties, propertyValues,
					namedFields, fieldValues);
			}
			return new CustomAttributeBuilder(constructorInfo, constructorArgs);
		}
		
		void GetNamedValues(ExpressionPairCollection values,
		                    out PropertyInfo[] outNamedProperties,
		                    out object[] outPropertyValues,
		                    out FieldInfo[] outNamedFields,
		                    out object[] outFieldValues)
		{
			List namedProperties = new List();
			List propertyValues = new List();
			List namedFields = new List();
			List fieldValues = new List();
			foreach (ExpressionPair pair in values)
			{
				ITypedEntity entity = (ITypedEntity)GetEntity(pair.First);
				object value = GetValue(entity.Type, pair.Second);
				if (EntityType.Property == entity.EntityType)
				{
					namedProperties.Add(GetPropertyInfo(entity));
					propertyValues.Add(value);
				}
				else
				{
					namedFields.Add(GetFieldInfo((IField)entity));
					fieldValues.Add(value);
				}
			}
			
			outNamedProperties = (PropertyInfo[])namedProperties.ToArray(typeof(PropertyInfo));
			outPropertyValues = propertyValues.ToArray();
			outNamedFields = (FieldInfo[])namedFields.ToArray(typeof(FieldInfo));
			outFieldValues = fieldValues.ToArray();
		}
		
		object[] GetValues(IParameter[] targetParameters, ExpressionCollection expressions)
		{
			object[] values = new object[expressions.Count];
			for (int i=0; i<values.Length; ++i)
			{
				values[i] = GetValue(targetParameters[i].Type, expressions[i]);
			}
			return values;
		}
		
		object GetValue(IType expectedType, Expression expression)
		{
			switch (expression.NodeType)
			{
				case NodeType.StringLiteralExpression:
					{
						return ((StringLiteralExpression)expression).Value;
					}
					
				case NodeType.CharLiteralExpression:
					{
						return ((CharLiteralExpression)expression).Value[0];
					}
					
				case NodeType.BoolLiteralExpression:
					{
						return ((BoolLiteralExpression)expression).Value;
					}
					
				case NodeType.IntegerLiteralExpression:
					{
						return ConvertValue(expectedType,
						                    ((IntegerLiteralExpression)expression).Value);
					}
					
				case NodeType.DoubleLiteralExpression:
					{
						return ConvertValue(expectedType,
						                    ((DoubleLiteralExpression)expression).Value);
					}
					
				case NodeType.TypeofExpression:
					{
						return GetSystemType(((TypeofExpression)expression).Type);
					}

				case NodeType.CastExpression:
					{
						return GetValue(expectedType, ((CastExpression)expression).Target);
					}
					
				default:
					{
						IEntity tag = GetEntity(expression);
						if (EntityType.Type == tag.EntityType)
						{
							return GetSystemType(expression);
						}
						else if (EntityType.Field == tag.EntityType)
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
								{
									return GetValue(expectedType, field.StaticValue as Expression);
								}
								return field.StaticValue;
							}
						}
						break;
					}
			}
			NotImplemented(expression, "Expression value: " + expression);
			return null;
		}
		
		object ConvertValue(IType expectedType, object value)
		{
			if (expectedType.IsEnum)
			{
				return Convert.ChangeType(value, GetEnumUnderlyingType(expectedType));
			}
			return Convert.ChangeType(value, GetSystemType(expectedType));
		}

		private Type GetEnumUnderlyingType(EnumDefinition node)
		{
			return ((InternalEnum) node.Entity).UnderlyingType;
		}

		private Type GetEnumUnderlyingType(IType enumType)
		{
			return enumType is IInternalEntity
				? ((InternalEnum) enumType).UnderlyingType
				: Enum.GetUnderlyingType(GetSystemType(enumType));
		}

		void DefineTypeMembers(TypeDefinition typeDefinition)
		{
			if (IsEnumDefinition(typeDefinition))
			{
				return;
			}
			TypeBuilder typeBuilder = GetTypeBuilder(typeDefinition);
			TypeMemberCollection members = typeDefinition.Members;
			foreach (TypeMember member in members)
			{
				switch (member.NodeType)
				{
					case NodeType.Method:
						{
							DefineMethod(typeBuilder, (Method)member, 0);
							break;
						}
						
					case NodeType.Constructor:
						{
							DefineConstructor(typeBuilder, (Constructor)member);
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
				}
			}
		}

		string GetAssemblySimpleName(string fname)
		{
			return Path.GetFileNameWithoutExtension(fname);
		}
		
		string GetTargetDirectory(string fname)
		{
			return Path.GetDirectoryName(Path.GetFullPath(fname));
		}
		
		string BuildOutputAssemblyName(string fname)
		{
			if (!Path.HasExtension(fname))
			{
				if (CompilerOutputType.Library == Parameters.OutputType)
				{
					fname += ".dll";
				}
				else
				{
					fname += ".exe";
					
				}
			}
			return Path.GetFullPath(fname);
		}
		
		void DefineResources()
		{
			foreach (ICompilerResource resource in Parameters.Resources)
			{
				resource.WriteResource(_sreResourceService);
			}
		}

		SREResourceService _sreResourceService;

		sealed class SREResourceService : IResourceService
		{
			AssemblyBuilder _asmBuilder;
			ModuleBuilder _moduleBuilder;

			public SREResourceService (AssemblyBuilder asmBuilder, ModuleBuilder modBuilder)
			{
				this._asmBuilder = asmBuilder;
				this._moduleBuilder = modBuilder;
			}

			public bool EmbedFile(string resourceName, string fname)
			{
				MethodInfo embed_res = typeof (AssemblyBuilder).GetMethod(
					"EmbedResourceFile", BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic,
					null, CallingConventions.Any, new Type[] { typeof(string), typeof(string) }, null);
				if (embed_res != null)
				{
					embed_res.Invoke(this._asmBuilder, new object[] { resourceName, fname });
					return true;
				}
				return false;
			}

			public IResourceWriter DefineResource(string resourceName, string resourceDescription)
			{
				return this._moduleBuilder.DefineResource(resourceName, resourceDescription);
			}
		}
		
		void SetUpAssembly()
		{
			string fname = Parameters.OutputAssembly;
			if (0 == fname.Length)
			{
				fname = CompileUnit.Modules[0].Name;
			}
			
			string outputFile = BuildOutputAssemblyName(fname);
			
			AssemblyName asmName = CreateAssemblyName(outputFile);
			_asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, GetAssemblyBuilderAccess(), GetTargetDirectory(outputFile));
			if (Parameters.Debug)
			{
				// ikvm tip:  Set DebuggableAttribute to assembly before
				// creating the module, to make sure Visual Studio (Whidbey)
				// picks up the attribute when debugging dynamically generated code.
				_asmBuilder.SetCustomAttribute(CreateDebuggableAttribute());
			}
			_asmBuilder.SetCustomAttribute(CreateRuntimeCompatibilityAttribute());

			_moduleBuilder = _asmBuilder.DefineDynamicModule(asmName.Name, Path.GetFileName(outputFile), Parameters.Debug);

			if (Parameters.Unsafe)
			{
				_asmBuilder.SetCustomAttribute(CreateSecurityPermissionAttribute("SkipVerification"));
				_moduleBuilder.SetCustomAttribute(CreateUnverifiableCodeAttribute());
			}

			_sreResourceService = new SREResourceService (_asmBuilder, _moduleBuilder);
			ContextAnnotations.SetAssemblyBuilder(Context, _asmBuilder);
			
			Context.GeneratedAssemblyFileName = outputFile;
		}
		
		AssemblyBuilderAccess GetAssemblyBuilderAccess()
		{
			return Parameters.GenerateInMemory ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Save;
		}
		
		AssemblyName CreateAssemblyName(string outputFile)
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = GetAssemblySimpleName(outputFile);
			assemblyName.Version = GetAssemblyVersion();
			if (Parameters.DelaySign)
			{
				assemblyName.SetPublicKey(GetAssemblyKeyPair(outputFile).PublicKey);
			}
			else
			{
				assemblyName.KeyPair = GetAssemblyKeyPair(outputFile);
			}
			return assemblyName;
		}
		
		StrongNameKeyPair GetAssemblyKeyPair(string outputFile)
		{
			Attribute attribute = GetAssemblyAttribute("System.Reflection.AssemblyKeyNameAttribute");
			if (Parameters.KeyContainer != null)
			{
				if (attribute != null)
				{
					Warnings.Add(CompilerWarningFactory.HaveBothKeyNameAndAttribute(attribute));
				}
				if (Parameters.KeyContainer.Length != 0)
				{
					return new StrongNameKeyPair(Parameters.KeyContainer);
				}
			}
			else if (attribute != null)
			{
				string asmName = ((StringLiteralExpression)attribute.Arguments[0]).Value;
				if (asmName.Length != 0) //ignore empty AssemblyKeyName values, like C# does
				{
					return new StrongNameKeyPair(asmName);
				}
			}
			
			string fname = null;
			string srcFile = null;
			attribute = GetAssemblyAttribute("System.Reflection.AssemblyKeyFileAttribute");
			
			if (Parameters.KeyFile != null)
			{
				fname = Parameters.KeyFile;
				if (attribute != null)
				{
					Warnings.Add(CompilerWarningFactory.HaveBothKeyFileAndAttribute(attribute));
				}
			}
			else if (attribute != null)
			{
				fname = ((StringLiteralExpression)attribute.Arguments[0]).Value;
				if (attribute.LexicalInfo != null)
				{
					srcFile = attribute.LexicalInfo.FileName;
				}
			}
			
			if (null != fname && fname.Length > 0)
			{
				if (!Path.IsPathRooted(fname))
				{
					fname = ResolveRelative(outputFile, srcFile, fname);
				}
				using (FileStream stream = File.OpenRead(fname))
				{
					//Parameters.DelaySign is ignored.
					return new StrongNameKeyPair(stream);
				}
			}
			return null;
		}
		
		string ResolveRelative(string targetFile, string srcFile, string relativeFile)
		{
			//relative to current directory:
			string fname = Path.GetFullPath(relativeFile);
			if (File.Exists(fname))
			{
				return fname;
			}
			
			//relative to source file:
			if (srcFile != null)
			{
				fname = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(srcFile),
				                                      relativeFile));
				if (File.Exists(fname))
				{
					return fname;
				}
			}
			
			//relative to output assembly:
			if (targetFile != null)
			{
				fname = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(targetFile),
				                                      relativeFile));
			}
			return fname;
		}
		
		Version GetAssemblyVersion()
		{
			string version = GetAssemblyAttributeValue("System.Reflection.AssemblyVersionAttribute");
			if (null == version)
			{
				version = "0.0.0.0";
			}
			/* 1.0.* -- BUILD -- based on days since January 1, 2000
			 * 1.0.0.* -- REVISION -- based on seconds since midnight, January 1, 2000, divided by 2			 *
			 */
			string[] sliced = version.Split('.');
			if (sliced.Length > 2)
			{
				DateTime baseTime = new DateTime(2000, 1, 1);
				TimeSpan mark = (DateTime.Now - baseTime);
				if (sliced[2].StartsWith("*"))
				{
					sliced[2] = Math.Round(mark.TotalDays).ToString();
				}
				if (sliced.Length > 3)
				{
					if (sliced[3].StartsWith("*"))
					{
						sliced[3] = Math.Round(mark.TotalSeconds).ToString();
					}
				}
				version = Boo.Lang.Builtins.join(sliced, ".");
			}
			return new Version(version);
		}
		
		string GetAssemblyAttributeValue(string name)
		{
			Attribute attribute = GetAssemblyAttribute(name);
			if (null != attribute)
			{
				return ((StringLiteralExpression)attribute.Arguments[0]).Value;
			}
			return null;
		}
		
		Attribute GetAssemblyAttribute(string name)
		{
			Attribute[] attributes = _assemblyAttributes.Get(name);
			if (attributes.Length > 0)
			{
				Debug.Assert(1 == attributes.Length);
				return attributes[0];
			}
			return null;
		}

		protected override IType GetExpressionType(Expression node)
		{
			IType type = base.GetExpressionType(node);
			if (TypeSystemServices.IsUnknown(type)) throw CompilerErrorFactory.InvalidNode(node);
			return type;
		}


		static private MethodInfo StringFormat
		{
			get {
				if (null != stringFormat)
					return stringFormat;
				stringFormat = Types.String.GetMethod("Format", new Type[] { typeof(string), typeof(object) });
				return stringFormat;
			}
		}
		static MethodInfo stringFormat;

		static Dictionary<Type,MethodInfo> _Nullable_HasValue = new Dictionary<Type,MethodInfo>();
		static MethodInfo GetNullableHasValue(Type type)
		{
			MethodInfo method;
			if (_Nullable_HasValue.TryGetValue(type, out method))
				return method;
			method = Types.Nullable.MakeGenericType(new Type[] {type}).GetProperty("HasValue").GetGetMethod();
			_Nullable_HasValue.Add(type, method);
			return method;
		}
	}
}
