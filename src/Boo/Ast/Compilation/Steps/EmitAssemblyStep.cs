using System;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Boo.Ast;
using Boo.Ast.Compilation;
using Boo.Ast.Compilation.Binding;

namespace Boo.Ast.Compilation.Steps
{
	public class EmitAssemblyStep : AbstractSwitcherCompilerStep
	{		
		public static MethodInfo GetEntryPoint(CompileUnit cu)
		{
			return (MethodInfo)cu[EntryPointKey];
		}
		
		static object EntryPointKey = new object();
		
		static MethodInfo String_Format = typeof(string).GetMethod("Format", new Type[] { Binding.Types.String, Binding.Types.ObjectArray });
		
		static MethodInfo RuntimeServices_MoveNext = Types.RuntimeServices.GetMethod("MoveNext");
		
		static MethodInfo RuntimeServices_CheckArrayUnpack = Types.RuntimeServices.GetMethod("CheckArrayUnpack");
		
		static MethodInfo RuntimeServices_GetEnumerable = Types.RuntimeServices.GetMethod("GetEnumerable");
		
		static MethodInfo IEnumerable_GetEnumerator = Types.IEnumerable.GetMethod("GetEnumerator");
		
		static MethodInfo IEnumerator_MoveNext = Types.IEnumerator.GetMethod("MoveNext");
		
		static MethodInfo IEnumerator_get_Current = Types.IEnumerator.GetProperty("Current").GetGetMethod();
		
		static ConstructorInfo List_EmptyConstructor = Types.List.GetConstructor(Type.EmptyTypes);
		
		static ConstructorInfo List_IntConstructor = Types.List.GetConstructor(new Type[] { typeof(int) });
		
		static MethodInfo List_Add = Types.List.GetMethod("Add", new Type[] { Types.Object });
		
		static Type[] DelegateConstructorTypes = new Type[] { Types.Object, Types.IntPtr };
		
		AssemblyBuilder _asmBuilder;
		
		ModuleBuilder _moduleBuilder;
		
		ISymbolDocumentWriter _symbolDocWriter;
		
		ILGenerator _il;
		
		// keeps track of types on the IL stack
		System.Collections.Stack _types = new System.Collections.Stack();
		
		void PushType(Type type)
		{
			_types.Push(type);
		}
		
		Type PopType()
		{
			return (Type)_types.Pop();
		}
		
		Type PeekTypeOnStack()
		{
			return (Type)_types.Peek();
		}
		
		public override void Run()
		{				
			if (Errors.Count > 0 || 0 == CompileUnit.Modules.Count)
			{
				return;				
			}
			
			_asmBuilder = AssemblySetupStep.GetAssemblyBuilder(CompilerContext);
			_moduleBuilder = AssemblySetupStep.GetModuleBuilder(CompilerContext);			
			
			foreach (Boo.Ast.Module module in CompileUnit.Modules)
			{
				OnModule(module);
			}
			
			DefineEntryPoint();			
		}
		
		public override void OnModule(Boo.Ast.Module module)
		{			
			_symbolDocWriter = _moduleBuilder.DefineDocument(module.LexicalInfo.FileName, Guid.Empty, Guid.Empty, Guid.Empty);
			
			EmitTypeDefinition(module);		
		}
		
		void EmitTypeDefinition(TypeDefinition node)
		{
			node.Members.Switch(this);
			
			TypeBuilder typeBuilder = GetTypeBuilder(node);
			typeBuilder.CreateType();
		}		
		
		public override void OnMethod(Method method)
		{			
			MethodBuilder methodBuilder = GetMethodBuilder(method);			
			_il = methodBuilder.GetILGenerator();
			method.Locals.Switch(this);
			method.Body.Switch(this);
			_il.Emit(OpCodes.Ret);			
		}
		
		public override void OnLocal(Local local)
		{			
			LocalBinding info = GetLocalBinding(local);
			info.LocalBuilder = _il.DeclareLocal(info.Type);
			info.LocalBuilder.SetLocalSymInfo(local.Name);			
		}
		
		public override void OnForStatement(ForStatement node)
		{									
			EmitDebugInfo(node, node.Iterator);
			
			// iterator = <node.Iterator>;
			node.Iterator.Switch(this);		
			
			Type iteratorType = PopType();
			if (iteratorType.IsArray)
			{
				EmitArrayBasedFor(node, iteratorType);
			}
			else
			{
				EmitEnumerableBasedFor(node, iteratorType);
			}			
		}
		
		public override void OnUnpackStatement(UnpackStatement node)
		{
			DeclarationCollection decls = node.Declarations;
			
			ITypeBinding binding = GetTypeBinding(node.Expression);
			
			EmitDebugInfo(decls[0], node.Expression);						
			node.Expression.Switch(this);
			
			EmitUnpackForDeclarations(node.Declarations, PopType());			
		}	
		
		public override bool EnterExpressionStatement(ExpressionStatement node)
		{
			EmitDebugInfo(node);
			return true;
		}
		
		public override void LeaveExpressionStatement(ExpressionStatement node)
		{					
			// if the type of the inner expression is not
			// void we need to pop its return value to leave
			// the stack sane
			if (PopType() != Binding.Types.Void)
			{
				_il.Emit(OpCodes.Pop);
			}
		}
		
		public override void OnIfStatement(IfStatement node)
		{
			EmitDebugInfo(node, node.Expression);
			
			node.Expression.Switch(this); PopType();
			
			Label endLabel = _il.DefineLabel();
			_il.Emit(OpCodes.Brfalse, endLabel);			
			node.TrueBlock.Switch(this);
			_il.MarkLabel(endLabel);
		}
		
		public override void OnBinaryExpression(BinaryExpression node)
		{			
			if (BinaryOperatorType.Assign == node.Operator)
			{				
				IBinding binding = BindingManager.GetBinding(node.Left);
				switch (binding.BindingType)
				{
					case BindingType.Local:
					{
						node.Right.Switch(this); // leaves type on stack	
						_il.Emit(OpCodes.Dup);
						
						// todo: assignment result must be type on the left in the
						// case of casting
						LocalBuilder local = ((LocalBinding)binding).LocalBuilder;
						EmitCastIfNeeded(local.LocalType, PeekTypeOnStack());
						_il.Emit(OpCodes.Stloc, local);
						break;
					}
					
					case BindingType.Property:
					{
						IPropertyBinding property = (IPropertyBinding)binding;						
						SetProperty(node, property, node.Left, node.Right, true);
						break;
					}
						
					default:
					{
						Errors.NotImplemented(node, binding.ToString());
						break;
					}
				}				
			}
			else
			{				
				IBinding binding = BindingManager.GetBinding(node);
				if (BindingType.Method == binding.BindingType)
				{
					// operator
					IMethodBinding methodBinding = (IMethodBinding)binding;
					node.Left.Switch(this);
					EmitCastIfNeeded(methodBinding.GetParameterType(0), PopType());					
					node.Right.Switch(this);
					EmitCastIfNeeded(methodBinding.GetParameterType(1), PopType());
					_il.EmitCall(OpCodes.Call, (MethodInfo)methodBinding.MethodInfo, null);
					PushType(methodBinding.ReturnType.Type);
				}
				else
				{
					Errors.NotImplemented(node, binding.ToString());
				}
			}
		}
		
		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{				
			IBinding binding = BindingManager.GetBinding(node.Target);
			switch (binding.BindingType)
			{
				case BindingType.Method:
				{										
					IMethodBinding methodBinding = (IMethodBinding)binding;
					MethodInfo mi = (MethodInfo)methodBinding.MethodInfo;
					OpCode code = OpCodes.Call;
					if (!mi.IsStatic)
					{
						// pushes target reference
						node.Target.Switch(this);
						
						Type targetType = PopType();
						
						bool declaringTypeIsValueType = mi.DeclaringType.IsValueType;
						bool targetTypeIsValueType = targetType.IsValueType; 
						if (!declaringTypeIsValueType &&
						    !targetTypeIsValueType)
						{
							if (mi.IsVirtual)
							{
								code = OpCodes.Callvirt;
							}
						}
						else
						{
							if (declaringTypeIsValueType)
							{
								// declare local to hold value type
								LocalBuilder temp = _il.DeclareLocal(targetType);
								_il.Emit(OpCodes.Stloc, temp);
								_il.Emit(OpCodes.Ldloca, temp);
							}
							else
							{
								_il.Emit(OpCodes.Box, targetType);
							}
						}
					}
					PushArguments(methodBinding, node.Arguments);
					_il.EmitCall(code, mi, null);
					
					PushType(mi.ReturnType);
					break;
				}
				
				case BindingType.Constructor:
				{
					IConstructorBinding constructorBinding = (IConstructorBinding)binding;
					PushArguments(constructorBinding, node.Arguments);
					
					ConstructorInfo ci = constructorBinding.ConstructorInfo;
					_il.Emit(OpCodes.Newobj, ci);
					foreach (ExpressionPair pair in node.NamedArguments)
					{
						// object reference
						_il.Emit(OpCodes.Dup);
						
						IBinding memberBinding = BindingManager.GetBinding(pair.First);						
						// field/property reference						
						InitializeMember(node, memberBinding, pair.Second);
					}
					
					// constructor invocation resulting type is
					PushType(ci.DeclaringType);
					break;
				}
				
				default:
				{
					Errors.NotImplemented(node, binding.ToString());
					break;
				}
			}
		}
		
		public override void OnIntegerLiteralExpression(IntegerLiteralExpression node)
		{
			_il.Emit(OpCodes.Ldc_I4, int.Parse(node.Value));
			PushType(Types.Int);
		}
		
		public override void OnBoolLiteralExpression(BoolLiteralExpression node)
		{
			if (node.Value)
			{
				_il.Emit(OpCodes.Ldc_I4_1);
			}
			else
			{
				_il.Emit(OpCodes.Ldc_I4_0);
			}
			PushType(Types.Bool);
		}
		
		public override void OnListLiteralExpression(ListLiteralExpression node)
		{
			if (node.Items.Count > 0)
			{
				_il.Emit(OpCodes.Ldc_I4, node.Items.Count);
				_il.Emit(OpCodes.Newobj, List_IntConstructor);
				
				foreach (Expression item in node.Items)
				{					
					item.Switch(this);
					EmitCastIfNeeded(Types.Object, PopType());
					_il.EmitCall(OpCodes.Call, List_Add, null);
					// List_Add will return the list itself
				}
			}
			else
			{
				_il.Emit(OpCodes.Newobj, List_EmptyConstructor);			
			}
			PushType(Types.List);
		}
		
		public override void OnStringLiteralExpression(StringLiteralExpression node)
		{
			_il.Emit(OpCodes.Ldstr, node.Value);
			PushType(Types.String);
		}
		
		public override void OnStringFormattingExpression(StringFormattingExpression node)
		{			              
			_il.Emit(OpCodes.Ldstr, node.Template);
			
			// new object[node.Arguments.Count]
			_il.Emit(OpCodes.Ldc_I4, node.Arguments.Count);
			_il.Emit(OpCodes.Newarr, Types.Object);
			
			ExpressionCollection args = node.Arguments;
			for (int i=0; i<args.Count; ++i)
			{			
				StoreElementReference(i, args[i], Types.Object);				
			}
			
			_il.EmitCall(OpCodes.Call, String_Format, null);
			PushType(Types.String);
		}
		
		public override void OnMemberReferenceExpression(MemberReferenceExpression node)
		{			
			IBinding binding = BindingManager.GetBinding(node);
			switch (binding.BindingType)
			{
				case BindingType.Property:
				{
					OpCode code = OpCodes.Call;
					PropertyInfo property = ((IPropertyBinding)binding).PropertyInfo;
					MethodInfo getMethod = property.GetGetMethod();
					if (!getMethod.IsStatic)
					{
						node.Target.Switch(this);
						
						Type targetType = PopType();
						if (getMethod.IsVirtual)
						{
							if (targetType.IsValueType)
							{
								Errors.NotImplemented(node, "property access for value types");
							}
							else
							{
								code = OpCodes.Callvirt;
							}
						}
					}
					_il.EmitCall(code, getMethod, null);					
					PushType(getMethod.ReturnType);
					break;
				}
				
				case BindingType.Method:
				{
					node.Target.Switch(this);
					break;
				}
				
				case BindingType.Field:
				{
					IFieldBinding fieldBinding = (IFieldBinding)binding;
					FieldInfo fieldInfo = fieldBinding.FieldInfo;
					if (fieldBinding.IsStatic)
					{
						if (fieldInfo.DeclaringType.IsEnum)
						{
							_il.Emit(OpCodes.Ldc_I4, (int)fieldBinding.FieldInfo.GetValue(null));							
						}
						else
						{
							_il.Emit(OpCodes.Ldsfld, fieldInfo);							
						}
						PushType(fieldInfo.FieldType);
					}
					else
					{						
						Errors.NotImplemented(node, binding.ToString());
					}
					break;
				}
				
				default:
				{
					Errors.NotImplemented(node, binding.ToString());
					break;
				}
			}
		}
		
		public override void OnReferenceExpression(ReferenceExpression node)
		{	
			IBinding info = BindingManager.GetBinding(node);
			switch (info.BindingType)
			{
				case BindingType.Local:
				{
					LocalBinding local = (LocalBinding)info;
					LocalBuilder builder = local.LocalBuilder;
					_il.Emit(OpCodes.Ldloc, builder);
					PushType(builder.LocalType);
					break;
				}
				
				case BindingType.Parameter:
				{
					Binding.ParameterBinding param = (Binding.ParameterBinding)info;
					_il.Emit(OpCodes.Ldarg, param.Index);
					PushType(param.Type);
					break;
				}
				
				default:
				{
					Errors.NotImplemented(node, info.ToString());
					break;
				}
				
			}			
		}
		
		void SetProperty(Node sourceNode, IPropertyBinding property, Expression reference, Expression value, bool leaveValueOnStack)
		{
			PropertyInfo pi = property.PropertyInfo;			
			MethodInfo setMethod = pi.GetSetMethod();
			
			if (null != reference)
			{
				if (!setMethod.IsStatic)
				{
					((MemberReferenceExpression)reference).Target.Switch(this);
					PopType();
				}
			}
			
			value.Switch(this);
			EmitCastIfNeeded(pi.PropertyType, PopType());
			
			LocalBuilder local = null;
			if (leaveValueOnStack)
			{
				_il.Emit(OpCodes.Dup);
				local = _il.DeclareLocal(pi.PropertyType);
				_il.Emit(OpCodes.Stloc, local);
			}
			
			_il.EmitCall(OpCodes.Callvirt, setMethod, null);
			
			if (leaveValueOnStack)
			{
				_il.Emit(OpCodes.Ldloc, local);
				PushType(local.LocalType);
			}
		}
		
		void InitializeMember(Node sourceNode, IBinding binding, Expression value)
		{
			switch (binding.BindingType)
			{
				case BindingType.Property:
				{
					IPropertyBinding property = (IPropertyBinding)binding;
					SetProperty(sourceNode, property, null, value, false);					
					break;
				}
				
				case BindingType.Event:
				{
					MethodBase mi = ((IMethodBinding)GetBinding(value)).MethodInfo;
					if (!mi.IsStatic)
					{
						Errors.NotImplemented(sourceNode, "only static delegates for now");
					}
					else
					{
						_il.Emit(OpCodes.Ldnull);
						_il.Emit(OpCodes.Ldftn, (MethodInfo)mi);
					}
					
					EventInfo ei = ((ExternalEventBinding)binding).EventInfo;
					
					_il.Emit(OpCodes.Newobj, GetDelegateConstructor(ei.EventHandlerType));					
					_il.EmitCall(OpCodes.Callvirt, ei.GetAddMethod(true), null);
					
					break;
				}
					
				case BindingType.Field:
				{
					Errors.NotImplemented(sourceNode, binding.ToString());
					break;					
				}
				
				default:
				{
					throw new ArgumentException("binding");
				}				
			}
		}			
		
		ConstructorInfo GetDelegateConstructor(Type delegateType)
		{
			return delegateType.GetConstructor(DelegateConstructorTypes);
		}
		
		void EmitDebugInfo(Node node)
		{
			EmitDebugInfo(node, node);
		}
		
		void EmitDebugInfo(Node startNode, Node endNode)
		{
			LexicalInfo start = startNode.LexicalInfo;
			LexicalInfo end = endNode.LexicalInfo;
			_il.MarkSequencePoint(_symbolDocWriter, start.Line, start.StartColumn, end.Line, end.EndColumn);
		}
		
		void EmitEnumerableBasedFor(ForStatement node, Type iteratorType)
		{			
			Label labelTest = _il.DefineLabel();
			Label labelEnd = _il.DefineLabel();
			
			LocalBuilder localIterator = _il.DeclareLocal(Types.IEnumerator);
			EmitGetEnumerableIfNeeded(iteratorType);			
			_il.EmitCall(OpCodes.Callvirt, IEnumerable_GetEnumerator, null);
			_il.Emit(OpCodes.Stloc, localIterator);
			
			// iterator.MoveNext()			
			_il.MarkLabel(labelTest);
			_il.Emit(OpCodes.Ldloc, localIterator);
			_il.EmitCall(OpCodes.Callvirt, IEnumerator_MoveNext, null);
			_il.Emit(OpCodes.Brfalse, labelEnd);			
			
			_il.Emit(OpCodes.Ldloc, localIterator);
			_il.EmitCall(OpCodes.Callvirt, IEnumerator_get_Current, null);
			EmitUnpackForDeclarations(node.Declarations, Binding.Types.Object);
			
			node.Statements.Switch(this);
			_il.Emit(OpCodes.Br, labelTest);
			
			_il.MarkLabel(labelEnd);			
		}
		
		void EmitArrayBasedFor(ForStatement node, Type iteratorType)
		{				
			Label labelTest = _il.DefineLabel();
			Label labelEnd = _il.DefineLabel();
			
			LocalBuilder localIterator = _il.DeclareLocal(iteratorType);
			_il.Emit(OpCodes.Stloc, localIterator);
			
			// i = 0;
			LocalBuilder localIndex = _il.DeclareLocal(Types.Int);
			_il.Emit(OpCodes.Ldc_I4_0);
			_il.Emit(OpCodes.Stloc, localIndex);			
			
			// i<iterator.Length			
			_il.MarkLabel(labelTest);			
			_il.Emit(OpCodes.Ldloc, localIndex);
			_il.Emit(OpCodes.Ldloc, localIterator);
			_il.Emit(OpCodes.Ldlen);
			_il.Emit(OpCodes.Bge, labelEnd);		
			
			// value = iterator[i]
			_il.Emit(OpCodes.Ldloc, localIterator);
			_il.Emit(OpCodes.Ldloc, localIndex);
			_il.Emit(OpCodes.Ldelem_Ref);
			
			EmitUnpackForDeclarations(node.Declarations, iteratorType.GetElementType());
			
			node.Statements.Switch(this);
			
			// ++i
			_il.Emit(OpCodes.Ldc_I4_1);
			_il.Emit(OpCodes.Ldloc, localIndex);
			_il.Emit(OpCodes.Add);
			_il.Emit(OpCodes.Stloc, localIndex);
			_il.Emit(OpCodes.Br, labelTest);
			
			_il.MarkLabel(labelEnd);
		}
		
		void EmitUnpackForDeclarations(DeclarationCollection decls, Type topOfStack)
		{
			if (1 == decls.Count)
			{
				// for arg in iterator
				LocalBuilder localValue = GetLocalBuilder(decls[0]);
				StoreLocal(topOfStack, localValue);
			}
			else
			{
				if (topOfStack.IsArray)
				{	
					Type elementType = topOfStack.GetElementType();
					
					// RuntimeServices.CheckArrayUnpack(array, decls.Count);					
					_il.Emit(OpCodes.Dup);
					_il.Emit(OpCodes.Ldc_I4, decls.Count);					
					_il.EmitCall(OpCodes.Call, RuntimeServices_CheckArrayUnpack, null);
					
					for (int i=0; i<decls.Count; ++i)
					{
						// local = array[i]
						_il.Emit(OpCodes.Dup);
						_il.Emit(OpCodes.Ldc_I4, i); // element index			
						_il.Emit(OpCodes.Ldelem_Ref);
						
						StoreLocal(elementType, GetLocalBuilder(decls[i]));					
					}
				}
				else
				{
					EmitGetEnumerableIfNeeded(topOfStack);
					_il.EmitCall(OpCodes.Callvirt, IEnumerable_GetEnumerator, null);
					
					foreach (Declaration d in decls)
					{
						_il.Emit(OpCodes.Dup);
						_il.EmitCall(OpCodes.Call, RuntimeServices_MoveNext, null);				
						StoreLocal(Types.Object, GetLocalBuilder(d));				
					}					
				}
				_il.Emit(OpCodes.Pop);
			}
		}
		
		void EmitGetEnumerableIfNeeded(Type topOfStack)
		{
			if (!IsIEnumerableCompatible(topOfStack))
			{
				_il.EmitCall(OpCodes.Call, RuntimeServices_GetEnumerable, null);
			}
		}
		
		bool IsIEnumerableCompatible(Type type)
		{
			return Types.IEnumerable.IsAssignableFrom(type);
		}
		
		void PushArguments(IMethodBinding binding, ExpressionCollection args)
		{
			for (int i=0; i<args.Count; ++i)
			{
				Expression arg = args[i];
				
				Type expectedType = binding.GetParameterType(i);
				arg.Switch(this);
				EmitCastIfNeeded(expectedType, PopType());
			}
		}
		
		void EmitCastIfNeeded(Type expectedType, Type actualType)
		{
			if (!expectedType.IsAssignableFrom(actualType))
			{
				if (expectedType.IsValueType)
				{
					if (actualType.IsValueType)
					{
						// numeric promotion
						_il.Emit(GetNumericPromotionOpCode(expectedType));
					}
					else
					{
						_il.Emit(OpCodes.Unbox, expectedType);
						_il.Emit(OpCodes.Ldobj, expectedType);
					}
				}
				else
				{
					_il.Emit(OpCodes.Castclass, expectedType);
				}
			}
			else
			{
				if (expectedType == Types.Object)
				{
					if (actualType.IsValueType)
					{
						_il.Emit(OpCodes.Box, actualType);
					}
				}
			}
		}
		
		OpCode GetNumericPromotionOpCode(Type type)
		{
			if (type == Types.Int)
			{
				return OpCodes.Conv_I4;
			}
			else if (type == Types.Single)
			{
				return OpCodes.Conv_R4;
			}
			else
			{
				throw new NotImplementedException(string.Format("Numeric promotion for {0} not implemented!", type));
			}
		}
		
		void StoreLocal(Type topOfStack, LocalBuilder localBuilder)
		{
			EmitCastIfNeeded(localBuilder.LocalType, topOfStack);
			_il.Emit(OpCodes.Stloc, localBuilder);
		}
		
		void StoreElementReference(int index, Node value, Type elementType)
		{
			_il.Emit(OpCodes.Dup);	// array reference
			_il.Emit(OpCodes.Ldc_I4, index); // element index
			value.Switch(this); // value
			EmitCastIfNeeded(elementType, GetBoundType(value));
			_il.Emit(OpCodes.Stelem_Ref);
		}		
		
		void DefineEntryPoint()
		{
			if (CompilerOutputType.Library != CompilerParameters.OutputType)
			{
				Module main = CompileUnit.Modules[0];
				Method method = ModuleStep.GetMainMethod(main);
				if (null != method)
				{
					Type type = _asmBuilder.GetType(main.FullyQualifiedName, true);
					MethodInfo mi = type.GetMethod(method.Name, BindingFlags.Static|BindingFlags.NonPublic);
					
					_asmBuilder.SetEntryPoint(mi, (PEFileKinds)CompilerParameters.OutputType);
					CompileUnit[EntryPointKey] = mi;
				}
				else
				{
					Errors.NoEntryPoint(main);
				}
			}
		}	

	}
}
