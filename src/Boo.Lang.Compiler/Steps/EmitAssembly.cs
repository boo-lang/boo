#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Collections;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	class LoopInfo
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
		static MethodInfo RuntimeServices_MoveNext = Types.RuntimeServices.GetMethod("MoveNext");
		
		static MethodInfo RuntimeServices_CheckArrayUnpack = Types.RuntimeServices.GetMethod("CheckArrayUnpack");
		
		static MethodInfo RuntimeServices_NormalizeArrayIndex = Types.RuntimeServices.GetMethod("NormalizeArrayIndex");
		
		static MethodInfo RuntimeServices_GetEnumerable = Types.RuntimeServices.GetMethod("GetEnumerable");
		
		static MethodInfo RuntimeServices_ToBool = Types.RuntimeServices.GetMethod("ToBool");
		
		static MethodInfo IEnumerable_GetEnumerator = Types.IEnumerable.GetMethod("GetEnumerator");
		
		static MethodInfo IEnumerator_MoveNext = Types.IEnumerator.GetMethod("MoveNext");
		
		static MethodInfo IEnumerator_get_Current = Types.IEnumerator.GetProperty("Current").GetGetMethod();		
		
		static MethodInfo Math_Pow = typeof(Math).GetMethod("Pow");
		
		static ConstructorInfo List_EmptyConstructor = Types.List.GetConstructor(Type.EmptyTypes);
		
		static ConstructorInfo List_ArrayBoolConstructor = Types.List.GetConstructor(new Type[] { Types.ObjectArray, Types.Bool });
		
		static ConstructorInfo Hash_Constructor = Types.Hash.GetConstructor(new Type[0]);
		
		static ConstructorInfo Regex_Constructor = typeof(Regex).GetConstructor(new Type[] { Types.String });
		
		static MethodInfo Hash_Add = Types.Hash.GetMethod("Add", new Type[] { typeof(object), typeof(object) });
		
		static ConstructorInfo TimeSpan_LongConstructor = Types.TimeSpan.GetConstructor(new Type[] { typeof(long) });
		
		static ConstructorInfo Object_Constructor = Types.Object.GetConstructor(new Type[0]);
		
		static MethodInfo List_Add = Types.List.GetMethod("Add", new Type[] { Types.Object });
		
		static MethodInfo Type_GetTypeFromHandle = Types.Type.GetMethod("GetTypeFromHandle");
		
		static Type[] DelegateConstructorTypes = new Type[] { Types.Object, Types.IntPtr };
		
		AssemblyBuilder _asmBuilder;
		
		ModuleBuilder _moduleBuilder;
		
		ISymbolDocumentWriter _symbolDocWriter;
		
		TypeBuilder _typeBuilder;
		
		// IL generation state		
		ILGenerator _il;
		Label _returnLabel; // current label for method return
		LocalBuilder _returnValueLocal; // returnValueLocal
		IType _returnType;
		int _tryBlock; // are we in a try block?
		Hashtable _typeCache = new Hashtable();
		
		// keeps track of types on the IL stack
		System.Collections.Stack _types = new System.Collections.Stack();
		
		System.Collections.Stack _loopInfoStack = new System.Collections.Stack();
		
		LoopInfo _currentLoopInfo;
		
		void EnterLoop(Label breakLabel, Label continueLabel)
		{
			_loopInfoStack.Push(_currentLoopInfo = new LoopInfo(breakLabel, continueLabel, _tryBlock));
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
			return (IType)_types.Peek();
		}
		
		void AssertStackIsEmpty(string message)
		{
			if (0 != _types.Count)
			{
				throw new ApplicationException(
						string.Format("{0}: {1} items still on the stack.", message, _types.Count)
						);
			}
		}
		
		override public void Run()
		{				
			if (Errors.Count > 0)
			{
				return;				
			}
			
			SetUpAssembly();
			
			DefineTypes();			
			DefineResources();
			DefineEntryPoint();			
		}
		
		void DefineTypes()
		{
			if (CompileUnit.Modules.Count > 0)
			{			
				Boo.Lang.List types = CollectTypes();
				
				foreach (TypeDefinition type in types)
				{
					DefineType(type);
				}
				
				foreach (TypeDefinition type in types)
				{
					DefineTypeMembers(type);
				}
				
				foreach (Boo.Lang.Compiler.Ast.Module module in CompileUnit.Modules)
				{
					OnModule(module);
				}
				
				CreateTypes(types);
			}
		}
		
		void CreateTypes(Boo.Lang.List types)
		{
			Hashtable created = new Hashtable();
			foreach (TypeMember type in types)
			{
				CreateType(created, type);
			}
		}
		
		void CreateType(Hashtable created, TypeMember type)
		{	
			if (!created.ContainsKey(type))
			{
				created.Add(type, type);
				
				if (IsEnumDefinition(type))
				{				
					GetEnumBuilder(type).CreateType();
				}
				else
				{
					TypeDefinition typedef = type as TypeDefinition;
					if (null != typedef)
					{
						foreach (TypeReference baseTypeRef in typedef.BaseTypes)
						{
							InternalType tag = GetType(baseTypeRef) as InternalType;
							if (null != tag)
							{
								CreateType(created, tag.TypeDefinition);
							}
						}
					}
					GetTypeBuilder(type).CreateType();
				}
			}
		}
		
		Boo.Lang.List CollectTypes()
		{
			Boo.Lang.List types = new Boo.Lang.List();
			foreach (Boo.Lang.Compiler.Ast.Module module in CompileUnit.Modules)
			{				 
				CollectTypes(types, module.Members);
			}
			return types;
		}
		
		void CollectTypes(Boo.Lang.List types, TypeMemberCollection members)
		{
			foreach (TypeMember member in members)
			{
				switch (member.NodeType)
				{
					case NodeType.ClassDefinition:
					{
						types.Add(member);
						CollectTypes(types, ((TypeDefinition)member).Members);
						break;
					}
					
					case NodeType.InterfaceDefinition:
					{
						types.Add(member);
						CollectTypes(types, ((TypeDefinition)member).Members);
						break;
					}
					
					case NodeType.EnumDefinition:
					{
						types.Insert(0, member);
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
			_symbolDocWriter = null;
			_typeBuilder = null;
			_il = null;		
			_returnValueLocal = null;
			_returnType = null;
			_tryBlock = 0;
			_types.Clear();
			_typeCache.Clear();
		}
		
		override public void OnAttribute(Boo.Lang.Compiler.Ast.Attribute node)
		{
		}
		
		override public void OnModule(Boo.Lang.Compiler.Ast.Module module)
		{			
			string fname = module.LexicalInfo.FileName;
			if (null != fname)
			{
				_symbolDocWriter = _moduleBuilder.DefineDocument(fname, Guid.Empty, Guid.Empty, Guid.Empty);
			}
			else
			{
				_symbolDocWriter = null;
			}
			Visit(module.Members);
		}
		
		override public void OnEnumDefinition(EnumDefinition node)
		{
			EnumBuilder builder = GetEnumBuilder(node);
			foreach (Boo.Lang.Compiler.Ast.Attribute attribute in node.Attributes)
			{
				builder.SetCustomAttribute(GetCustomAttributeBuilder(attribute));
			}
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{
			EmitTypeDefinition(node);
		}
		
		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{
			TypeBuilder builder = GetTypeBuilder(node);
			foreach (TypeReference baseType in node.BaseTypes)
			{
				builder.AddInterfaceImplementation(GetSystemType(baseType));
			}
			EmitAttributes(builder, node);
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
			
			_typeBuilder = current;
		}		
		
		override public void OnMethod(Method method)
		{			
			if (method.IsRuntime)
			{
				return;				
			}
			
			MethodBuilder methodBuilder = GetMethodBuilder(method);			
			_il = methodBuilder.GetILGenerator();
			_returnLabel = _il.DefineLabel();
			
			_returnType = ((IMethod)GetEntity(method)).ReturnType;
			if (TypeSystemServices.VoidType != _returnType)
			{
				_returnValueLocal = _il.DeclareLocal(GetSystemType(_returnType));
			}
			
			Visit(method.Locals);
			Visit(method.Body);
			
			_il.MarkLabel(_returnLabel);
			
			if (null != _returnValueLocal)
			{
				_il.Emit(OpCodes.Ldloc, _returnValueLocal);
				_returnValueLocal = null;
			}
			_il.Emit(OpCodes.Ret);			
		}
		
		override public void OnConstructor(Constructor constructor)
		{
			if (constructor.IsRuntime)
			{
				return;
			}
			
			ConstructorBuilder builder = GetConstructorBuilder(constructor);
			_il = builder.GetILGenerator();

			InternalConstructor tag = (InternalConstructor)GetEntity(constructor);
			Visit(constructor.Locals);
			Visit(constructor.Body);
			_il.Emit(OpCodes.Ret);
		}
		
		override public void OnLocal(Local local)
		{			
			LocalVariable info = GetLocalVariable(local);
			info.LocalBuilder = _il.DeclareLocal(GetSystemType(local));
			info.LocalBuilder.SetLocalSymInfo(local.Name);			
		}
		
		override public void OnForStatement(ForStatement node)
		{									
			EmitDebugInfo(node, node.Iterator);
			
			// iterator = <node.Iterator>;
			node.Iterator.Accept(this);		
			
			IType iteratorType = PopType();
			if (iteratorType.IsArray)
			{
				EmitArrayBasedFor(node, (IArrayType)iteratorType);
			}
			else
			{
				EmitEnumerableBasedFor(node, iteratorType);
			}			
		}
		
		override public void OnReturnStatement(ReturnStatement node)
		{
			OpCode retOpCode = _tryBlock > 0 ? OpCodes.Leave : OpCodes.Br;
			
			if (null != node.Expression)
			{
				Visit(node.Expression);
				EmitCastIfNeeded(_returnType, PopType());
				_il.Emit(OpCodes.Stloc, _returnValueLocal);
			}
			_il.Emit(retOpCode, _returnLabel);
		}
		
		override public void OnRaiseStatement(RaiseStatement node)
		{
			Visit(node.Exception); PopType();
			_il.Emit(OpCodes.Throw);
		}
		
		override public void OnTryStatement(TryStatement node)
		{
			++_tryBlock;
			
			Label endLabel = _il.BeginExceptionBlock();
			Visit(node.ProtectedBlock);
			Visit(node.ExceptionHandlers);
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
			_il.BeginCatchBlock(GetSystemType(node.Declaration));
			_il.Emit(OpCodes.Stloc, GetLocalBuilder(node.Declaration));
			Visit(node.Block);
		}
		
		override public void OnUnpackStatement(UnpackStatement node)
		{
			DeclarationCollection decls = node.Declarations;
			
			EmitDebugInfo(decls[0], node.Expression);						
			node.Expression.Accept(this);
			
			EmitUnpackForDeclarations(node.Declarations, PopType());			
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
			if (PopType() != TypeSystemServices.VoidType)
			{				
				_il.Emit(OpCodes.Pop);				
			}
			AssertStackIsEmpty("stack must be empty after a statement!");
		}
		
		override public void OnUnlessStatement(UnlessStatement node)
		{
			EmitDebugInfo(node);
			
			Label endLabel = _il.DefineLabel();
			EmitBranchTrue(node.Condition, endLabel);
			node.Block.Accept(this);
			_il.MarkLabel(endLabel);
		}
		
		override public void OnIfStatement(IfStatement node)
		{
			EmitDebugInfo(node);
			
			Label endLabel = _il.DefineLabel();
			
			EmitBranchFalse(node.Condition, endLabel);
			node.TrueBlock.Accept(this);
			if (null != node.FalseBlock)
			{
				Label elseEndLabel = _il.DefineLabel();
				_il.Emit(OpCodes.Br, elseEndLabel);
				_il.MarkLabel(endLabel);
				
				endLabel = elseEndLabel;
				node.FalseBlock.Accept(this);
			}
			
			_il.MarkLabel(endLabel);
		}
		
		void EmitBranchTrue(UnaryExpression expression, Label label)
		{
			if (UnaryOperatorType.LogicalNot == expression.Operator)
			{
				EmitBranchFalse(expression.Operand, label);
			}
			else
			{
				DefaultBranchTrue(expression, label);
			}
		}
		
		void EmitBranchTrue(BinaryExpression expression, Label label)
		{
			switch (expression.Operator)
			{
				case BinaryOperatorType.TypeTest:
				{
					EmitTypeTest(expression);
					_il.Emit(OpCodes.Brtrue, label);
					break;
				}
				
				case BinaryOperatorType.Or:
				{
					EmitBranchTrue(expression.Left, label);
					EmitBranchTrue(expression.Right, label);
					break;
				}
				
				case BinaryOperatorType.And:
				{
					Label skipRhs = _il.DefineLabel();
					EmitBranchFalse(expression.Left, skipRhs);
					EmitBranchTrue(expression.Right, label);
					_il.MarkLabel(skipRhs);
					break;
				}
				
				case BinaryOperatorType.Equality:
				{
					LoadCmpOperands(expression);
					_il.Emit(OpCodes.Beq, label);
					break;
				}
				
				case BinaryOperatorType.ReferenceEquality:
				{
					Visit(expression.Left); PopType();
					Visit(expression.Right); PopType();
					_il.Emit(OpCodes.Beq, label);
					break;
				}
				
				case BinaryOperatorType.ReferenceInequality:
				{
					Visit(expression.Left); PopType();
					Visit(expression.Right); PopType();
					_il.Emit(OpCodes.Ceq);
					_il.Emit(OpCodes.Brfalse, label);
					break;
				}
				
				case BinaryOperatorType.GreaterThan:
				{
					LoadCmpOperands(expression);
					_il.Emit(OpCodes.Bgt, label);
					break;
				}
				
				case BinaryOperatorType.GreaterThanOrEqual:
				{
					LoadCmpOperands(expression);
					_il.Emit(OpCodes.Bge, label);
					break;
				}
				
				case BinaryOperatorType.LessThan:
				{
					LoadCmpOperands(expression);
					_il.Emit(OpCodes.Blt, label);
					break;
				}
				
				case BinaryOperatorType.LessThanOrEqual:
				{
					LoadCmpOperands(expression);
					_il.Emit(OpCodes.Ble, label);
					break;
				}
				
				default:
				{
					DefaultBranchTrue(expression, label);
					break;
				}
			}
		}
		
		void EmitBranchTrue(Expression expression, Label label)
		{
			switch (expression.NodeType)
			{
				case NodeType.BinaryExpression:
				{
					EmitBranchTrue((BinaryExpression)expression, label);
					break;
				}
				
				case NodeType.UnaryExpression:
				{
					EmitBranchTrue((UnaryExpression)expression, label);
					break;
				}
				
				default:
				{
					DefaultBranchTrue(expression, label);
					break;
				}
			}
		}
		
		void DefaultBranchTrue(Expression expression, Label label)
		{
			expression.Accept(this);
			EmitToBoolIfNeeded(PopType());
			_il.Emit(OpCodes.Brtrue, label);
		}
		
		void EmitBranchFalse(BinaryExpression expression, Label label)
		{
			switch (expression.Operator)
			{
				case BinaryOperatorType.TypeTest:
				{
					EmitTypeTest(expression);
					_il.Emit(OpCodes.Brfalse, label);
					break;
				}
				
				case BinaryOperatorType.Or:
				{					
					Label end = _il.DefineLabel();
					EmitBranchTrue(expression.Left, end);
					EmitBranchFalse(expression.Right, label);
					_il.MarkLabel(end);	
					break;
				}
				
				case BinaryOperatorType.And:
				{
					EmitBranchFalse(expression.Left, label);
					EmitBranchFalse(expression.Right, label);
					break;
				}
				
				default:
				{
					DefaultBranchFalse(expression, label);
					break;
				}
			}
		}
		
		void EmitBranchFalse(Expression expression, Label label)
		{
			switch (expression.NodeType)
			{
				case NodeType.UnaryExpression:
				{
					EmitBranchFalse((UnaryExpression)expression, label);
					break;
				}
				
				case NodeType.BinaryExpression:
				{
					EmitBranchFalse((BinaryExpression)expression, label);
					break;
				}
				
				default:
				{
					DefaultBranchFalse(expression, label);
					break;
				}
			}
		}
		
		void EmitBranchFalse(UnaryExpression expression, Label label)
		{
			switch (expression.Operator)
			{
				case UnaryOperatorType.LogicalNot:
				{
					EmitBranchTrue(expression.Operand, label);
					break;
				}
				
				default:					
				{		
					DefaultBranchFalse(expression, label);
					break;
				}
			}
		}
		
		void DefaultBranchFalse(Expression expression, Label label)
		{
			expression.Accept(this);
			EmitToBoolIfNeeded(PopType());
			_il.Emit(OpCodes.Brfalse, label);
		}
		
		override public void OnBreakStatement(BreakStatement node)
		{
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
			EmitDebugInfo(node);
			
			Label endLabel = _il.DefineLabel();			
			Label bodyLabel = _il.DefineLabel();
			Label conditionLabel = _il.DefineLabel();
			
			_il.Emit(OpCodes.Br, conditionLabel);
			
			_il.MarkLabel(bodyLabel);
			
			EnterLoop(endLabel, conditionLabel);
			node.Block.Accept(this);
			LeaveLoop();
			
			_il.MarkLabel(conditionLabel);
			EmitBranchTrue(node.Condition, bodyLabel);			
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
			// value_on_stack ? 1 : 0
			Label wasTrue = _il.DefineLabel();
			Label wasFalse = _il.DefineLabel();
			_il.Emit(OpCodes.Brfalse, wasFalse);
			_il.Emit(OpCodes.Ldc_I4_0);
			_il.Emit(OpCodes.Br, wasTrue);
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
					node.Operand.Accept(this);
					IType typeOnStack = PopType();
					if (IsBoolOrInt(typeOnStack))
					{
						EmitIntNot();
					}
					else
					{
						EmitGenericNot();
					}
					PushBool();
					break;
				}
				
				case UnaryOperatorType.UnaryNegation:
				{					
					node.Operand.Accept(this);
					IType type = PopType();
					_il.Emit(OpCodes.Ldc_I4, -1);
					EmitCastIfNeeded(type, TypeSystemServices.IntType);
					_il.Emit(OpCodes.Mul);
					PushType(type);
					break;
				}
				
				default:
				{
					NotImplemented(node, "unary operator not supported");
					break;
				}
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
			EmitNormalizedArrayIndex(slice.Begin);			
			
			Visit(node.Right);
			EmitCastIfNeeded(elementType, PopType());
			
			bool leaveValueOnStack = ShouldLeaveValueOnStack(node);
			LocalBuilder temp = null;
			if (leaveValueOnStack)
			{
				_il.Emit(OpCodes.Dup);
				temp = _il.DeclareLocal(GetSystemType(elementType));
				_il.Emit(OpCodes.Stloc, temp);				
			}
			
			_il.Emit(GetStoreEntityOpCode(elementType));
			
			if (leaveValueOnStack)
			{
				_il.Emit(OpCodes.Ldloc, temp);
				PushType(elementType);
			}
			else
			{
				PushVoid();
			}
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
					SetLocal(node, (LocalVariable)tag, leaveValueOnStack);
					break;
				}
				
				case EntityType.Parameter:
				{
					InternalParameter param = (InternalParameter)tag;
					
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
		
		void EmitTypeTest(BinaryExpression node)
		{
			Visit(node.Left); PopType();
			_il.Emit(OpCodes.Isinst, GetSystemType(node.Right));
		}
		
		void OnTypeTest(BinaryExpression node)
		{
			EmitTypeTest(node);
			
			Label isTrue = _il.DefineLabel();
			Label isFalse = _il.DefineLabel();
			_il.Emit(OpCodes.Brtrue, isTrue);
			_il.Emit(OpCodes.Ldc_I4_0);
			_il.Emit(OpCodes.Br, isFalse);
			_il.MarkLabel(isTrue);
			_il.Emit(OpCodes.Ldc_I4_1);
			_il.MarkLabel(isFalse);
			
			PushBool();
		}
		
		void LoadCmpOperands(BinaryExpression node)
		{
			IType lhs = node.Left.ExpressionType;
			IType rhs = node.Right.ExpressionType;
			
			IType type = TypeSystemServices.GetPromotedNumberType(lhs, rhs);
			Visit(node.Left);
			EmitCastIfNeeded(type, PopType());
			Visit(node.Right);
			EmitCastIfNeeded(type, PopType());
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
		
		void EmitToBoolIfNeeded(IType topOfStack)
		{
			if (TypeSystemServices.ObjectType == topOfStack)
			{
				_il.EmitCall(OpCodes.Call, RuntimeServices_ToBool, null);
			}
		}
		
		void EmitAnd(BinaryExpression node)
		{
			EmitLogicalOperator(node, OpCodes.Brtrue, OpCodes.Brfalse); 
			/*
			IType type = GetType(node);
			Visit(node.Left);
			
			IType lhsType = PopType();
			
			IType lhsType = PopType();
			if (null != lhsType && lhsType.IsValueType && !type.IsValueType)
			{
				Label lhsWasTrue = _il.DefineLabel();
				Label end = _il.DefineLabel();
				
				_il.Emit(OpCodes.Dup);
				_il.Emit(OpCodes.Brtrue, lhsWasTrue);
				EmitCastIfNeeded(type, lhsType);
				_il.Emit(OpCodes.Br, end);
				
				_il.MarkLabel(lhsWasTrue);
				_il.Emit(OpCodes.Pop);
				Visit(node.Right);
				EmitCastIfNeeded(type, PopType());
				
				_il.MarkLabel(end);
			}
			else
			{
				EmitCastIfNeeded(type, lhsType);
				
				_il.Emit(OpCodes.Dup);
				EmitToBoolIfNeeded(type);
				
				_il.Emit(OpCodes.Brfalse, end);
				
				_il.Emit(OpCodes.Pop);
				Visit(node.Right);
				EmitCastIfNeeded(type, PopType());
				_il.MarkLabel(end);				
			}
			
			PushType(type);*/
		}
		
		void EmitOr(BinaryExpression node)
		{			
			EmitLogicalOperator(node, OpCodes.Brfalse, OpCodes.Brtrue);
		}
		
		void EmitLogicalOperator(BinaryExpression node, OpCode brForValueType, OpCode brForRefType)
		{
			IType type = node.ExpressionType;
			Visit(node.Left);
			
			IType lhsType = PopType();
			
			if (null != lhsType && lhsType.IsValueType && !type.IsValueType)
			{
				// if boxing, first evaluate the value
				// as it is and then box it...
				Label evalRhs = _il.DefineLabel();
				Label end = _il.DefineLabel();
				
				_il.Emit(OpCodes.Dup);
				_il.Emit(brForValueType, evalRhs);
				EmitCastIfNeeded(type, lhsType);				
				_il.Emit(OpCodes.Br, end);			
				
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
				
				EmitToBoolIfNeeded(lhsType);
				
				_il.Emit(brForRefType, end);
				
				_il.Emit(OpCodes.Pop);
				Visit(node.Right);
				EmitCastIfNeeded(type, PopType());
				_il.MarkLabel(end);
			}
			
			PushType(type);
		}
		
		void EmitBitwiseOperator(BinaryExpression node)
		{
			IType type = node.ExpressionType;
			
			Visit(node.Left);
			EmitCastIfNeeded(type, PopType());
			
			Visit(node.Right);
			EmitCastIfNeeded(type, PopType());
			
			_il.Emit(OpCodes.Or);
			
			PushType(type);
		}
		
		override public void OnBinaryExpression(BinaryExpression node)
		{				
			switch (node.Operator)
			{
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
				{
					OnArithmeticOperator(node);
					break;
				}
				
				case BinaryOperatorType.Subtraction:
				{
					OnArithmeticOperator(node);
					break;
				}
				
				case BinaryOperatorType.Multiply:
				{
					OnArithmeticOperator(node);
					break;
				}
				
				case BinaryOperatorType.Division:
				{
					OnArithmeticOperator(node);
					break;
				}
				
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
				
				case BinaryOperatorType.InPlaceAdd:
				{
					Visit(((MemberReferenceExpression)node.Left).Target); PopType();
					SubscribeEvent(node, GetEntity(node.Left), node.Right);
					PushVoid();
					break;
				}
				
				case BinaryOperatorType.InPlaceSubtract:
				{
					Visit(((MemberReferenceExpression)node.Left).Target); PopType();
					UnsubscribeEvent(node, GetEntity(node.Left), node.Right);
					PushVoid();
					break;
				}
				
				default:
				{				
					NotImplemented(node, node.Operator.ToString());
					break;
				}
			}
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
		
		override public void OnAsExpression(AsExpression node)
		{
			Type type = GetSystemType(node.Type);
			
			node.Target.Accept(this); PopType();			
			_il.Emit(OpCodes.Isinst, type);
			PushType(node.ExpressionType);
		}
		
		void InvokeMethod(IMethod methodInfo, MethodInvocationExpression node)
		{			
			MethodInfo mi = GetMethodInfo(methodInfo);
			OpCode code = OpCodes.Call;
			if (!mi.IsStatic)
			{				
				Expression target = ((MemberReferenceExpression)node.Target).Target;
				IType targetType = target.ExpressionType;
				if (targetType.IsValueType)
				{				
					if (mi.DeclaringType == Types.Object)
					{
						Visit(node.Target); 
						_il.Emit(OpCodes.Box, GetSystemType(PopType()));
					}
					else
					{
						LoadAddress(target);
					}
				}
				else
				{
					// pushes target reference
					Visit(node.Target); PopType();
					if (mi.IsVirtual)
					{
						code = OpCodes.Callvirt;
					}
				}
			}
			PushArguments(methodInfo, node.Arguments);
			_il.EmitCall(code, mi, null);
			
			PushType(methodInfo.ReturnType);
		}
		
		void InvokeSuperMethod(IMethod methodInfo, MethodInvocationExpression node)
		{
			IMethod super = ((InternalMethod)methodInfo).Override;
			MethodInfo superMI = GetMethodInfo(super);
			_il.Emit(OpCodes.Ldarg_0); // this
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
		
		override public void OnMethodInvocationExpression(MethodInvocationExpression node)
		{				
			IEntity tag = TypeSystemServices.GetEntity(node.Target);
			switch (tag.EntityType)
			{
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
					
					if (NodeType.SuperLiteralExpression == node.Target.NodeType)
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
						foreach (ExpressionPair pair in node.NamedArguments)
						{
							// object reference
							_il.Emit(OpCodes.Dup);
							
							IEntity memberInfo = TypeSystemServices.GetEntity(pair.First);						
							// field/property reference						
							InitializeMember(node, memberInfo, pair.Second);
						}
						
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
			_il.Emit(OpCodes.Ldc_I8, node.Value.Ticks);
			_il.Emit(OpCodes.Newobj, TimeSpan_LongConstructor);
			PushType(TypeSystemServices.TimeSpanType);
		}
		
		override public void OnIntegerLiteralExpression(IntegerLiteralExpression node)
		{
			if (node.IsLong)
			{
				_il.Emit(OpCodes.Ldc_I8, node.Value);
				PushType(TypeSystemServices.LongType);
			}
			else
			{
				switch (node.Value)
				{
					case 0L:
					{
						_il.Emit(OpCodes.Ldc_I4_0);
						break;
					}
					
					case 1L:
					{
						_il.Emit(OpCodes.Ldc_I4_1);
						break;
					}
					
					default:
					{
						_il.Emit(OpCodes.Ldc_I4, (int)node.Value);
						break;
					}
				}				
				PushType(TypeSystemServices.IntType);
			}			
		}
		
		override public void OnDoubleLiteralExpression(DoubleLiteralExpression node)
		{
			_il.Emit(OpCodes.Ldc_R8, node.Value);
			PushType(TypeSystemServices.DoubleType);
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
				_il.EmitCall(OpCodes.Call, Hash_Add, null);
			}
			PushType(TypeSystemServices.HashType);
		}
		
		bool IsListGenerator(ListLiteralExpression node)
		{
			return 1 == node.Items.Count &&
				NodeType.GeneratorExpression == node.Items[0].NodeType;
		}
		
		override public void OnGeneratorExpression(GeneratorExpression node)
		{
			NotImplemented(node, node.ToString());
		}
		
		override public void OnListLiteralExpression(ListLiteralExpression node)
		{
			if (node.Items.Count > 0)
			{
				if (IsListGenerator(node))
				{
					EmitListDisplay(node);
				}
				else
				{
					EmitObjectArray(node.Items);
					_il.Emit(OpCodes.Ldc_I4_1);
					_il.Emit(OpCodes.Newobj, List_ArrayBoolConstructor);
				}
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
			_il.Emit(OpCodes.Ldstr, RuntimeServices.Mid(node.Value, 1, -1));
			_il.Emit(OpCodes.Newobj, Regex_Constructor);
			PushType(node.ExpressionType);
		}
		
		override public void OnStringLiteralExpression(StringLiteralExpression node)
		{
			_il.Emit(OpCodes.Ldstr, node.Value);
			PushType(TypeSystemServices.StringType);
		}
		
		override public void OnSlicingExpression(SlicingExpression node)
		{			
			if (AstUtil.IsLhsOfAssignment(node))
			{
				return;
			}
			
			Visit(node.Target); 			
			IArrayType type = (IArrayType)PopType();

			EmitNormalizedArrayIndex(node.Begin);
			_il.Emit(GetLoadEntityOpCode(type.GetElementType()));			
			
			PushType(type.GetElementType());
		}
		
		void EmitNormalizedArrayIndex(Expression index)
		{
			bool isNegative = false;
			if (CanBeNegative(index, ref isNegative))
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
		
		static Regex _interpolatedExpression = new Regex(@"\{(\d+)\}", RegexOptions.Compiled|RegexOptions.CultureInvariant);
		
		override public void OnExpressionInterpolationExpression(ExpressionInterpolationExpression node)
		{	
			Type stringBuilderType = typeof(StringBuilder);
			ConstructorInfo constructor =  stringBuilderType.GetConstructor(new Type[0]);
			MethodInfo appendObject = stringBuilderType.GetMethod("Append", new Type[] { typeof(object) });
			MethodInfo appendString = stringBuilderType.GetMethod("Append", new Type[] { typeof(string) });
			
			_il.Emit(OpCodes.Newobj, constructor);
			
			foreach (Expression arg in node.Expressions)
			{	
				Visit(arg);
				
				IType argType = PopType();
				if (TypeSystemServices.StringType == argType)
				{
					_il.EmitCall(OpCodes.Call, appendString, null);
				}
				else
				{
					EmitCastIfNeeded(TypeSystemServices.ObjectType, argType);
					_il.EmitCall(OpCodes.Call, appendObject, null);
				}
			}
			_il.EmitCall(OpCodes.Call, stringBuilderType.GetMethod("ToString", new Type[0]), null);
			PushType(TypeSystemServices.StringType);
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
					_il.Emit(OpCodes.Ldsfld, GetFieldInfo(fieldInfo));							
				}
			}
			else
			{						
				Visit(self); PopType();
				_il.Emit(OpCodes.Ldfld, GetFieldInfo(fieldInfo));						
			}
			PushType(fieldInfo.Type);
		}
		
		void EmitLoadLiteralField(Node node, IField fieldInfo)
		{
			object value = fieldInfo.StaticValue;
			if (null == value)
			{
				_il.Emit(OpCodes.Ldnull);
			}
			else
			{			
				TypeCode type = Type.GetTypeCode(value.GetType());
				switch (type)
				{
					case TypeCode.Int32:
					{
						_il.Emit(OpCodes.Ldc_I4, (int)value);
						break;
					}
					
					case TypeCode.Int64:
					{
						_il.Emit(OpCodes.Ldc_I8, (long)value);
						break;
					}
					
					case TypeCode.Single:
					{
						_il.Emit(OpCodes.Ldc_R4, (float)value);
						break;
					}
					
					case TypeCode.Double:
					{
						_il.Emit(OpCodes.Ldc_R8, (double)value);
						break;
					}
					
					case TypeCode.String:
					{
						_il.Emit(OpCodes.Ldstr, (string)value);
						break;
					}
					
					default:
					{
						NotImplemented(node, "Literal: " + type.ToString());
						break;
					}
				}
			}
		}
		
		override public void OnMemberReferenceExpression(MemberReferenceExpression node)
		{			
			IEntity tag = TypeSystemServices.GetEntity(node);
			switch (tag.EntityType)
			{				
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
			IEntity tag = expression.Entity;
			if (null != tag)
			{
				switch (tag.EntityType)
				{
					case EntityType.Local:
					{				
						_il.Emit(OpCodes.Ldloca, ((LocalVariable)tag).LocalBuilder);
						return;
					}
				
					case EntityType.Parameter:
					{
						_il.Emit(OpCodes.Ldarga, ((InternalParameter)tag).Index);
						return;
					}
				}
			}
			
			// declare local to hold value type
			Visit(expression);
			LocalBuilder temp = _il.DeclareLocal(GetSystemType(PopType()));
			_il.Emit(OpCodes.Stloc, temp);
			_il.Emit(OpCodes.Ldloca, temp);
		}
		
		override public void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			_il.Emit(OpCodes.Ldarg_0);
			PushType(node.ExpressionType);
		}
		
		override public void OnNullLiteralExpression(NullLiteralExpression node)
		{
			_il.Emit(OpCodes.Ldnull);
			PushType(null);
		}
		
		override public void OnReferenceExpression(ReferenceExpression node)
		{	
			IEntity info = TypeSystemServices.GetEntity(node);
			switch (info.EntityType)
			{
				case EntityType.Local:
				{
					LocalVariable local = (LocalVariable)info;
					LocalBuilder builder = local.LocalBuilder;
					_il.Emit(OpCodes.Ldloc, builder);
					PushType(local.Type);
					break;
				}
				
				case EntityType.Parameter:
				{
					TypeSystem.InternalParameter param = (TypeSystem.InternalParameter)info;
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
					PushType(param.Type);
					break;
				}
				
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
		
		void SetLocal(BinaryExpression node, LocalVariable tag, bool leaveValueOnStack)
		{
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
			EmitAssignment(tag, typeOnStack);
		}
		
		void EmitAssignment(LocalVariable tag, IType typeOnStack)
		{			
			// todo: assignment result must be type on the left in the
			// case of casting
			LocalBuilder local = tag.LocalBuilder;
			EmitCastIfNeeded(tag.Type, typeOnStack);
			_il.Emit(OpCodes.Stloc, local);
		}
		
		void SetField(Node sourceNode, IField field, Expression reference, Expression value, bool leaveValueOnStack)
		{
			OpCode opSetField = OpCodes.Stsfld;
			if (!field.IsStatic)				
			{
				opSetField = OpCodes.Stfld;
				if (null != reference)
				{
					((MemberReferenceExpression)reference).Target.Accept(this);
					PopType();
				}
			}
			
			value.Accept(this);
			EmitCastIfNeeded(field.Type, PopType());
			
			FieldInfo fi = GetFieldInfo(field);
			LocalBuilder local = null;
			if (leaveValueOnStack)
			{
				_il.Emit(OpCodes.Dup);
				local = _il.DeclareLocal(fi.FieldType);
				_il.Emit(OpCodes.Stloc, local);
			}
			
			_il.Emit(opSetField, fi);
			
			if (leaveValueOnStack)
			{
				_il.Emit(OpCodes.Ldloc, local);
				PushType(field.Type);
			}
		}
		
		void SetProperty(Node sourceNode, IProperty property, Expression reference, Expression value, bool leaveValueOnStack)
		{
			PropertyInfo pi = GetPropertyInfo(property);			
			MethodInfo setMethod = pi.GetSetMethod(true);
			
			if (null != reference)
			{
				if (!setMethod.IsStatic)
				{
					((MemberReferenceExpression)reference).Target.Accept(this);
					PopType();
				}
			}
			
			value.Accept(this);
			EmitCastIfNeeded(property.Type, PopType());
			
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
				PushType(property.Type);
			}
		}
		
		void EmitCreateDelegate(Type delegateType, Expression value)
		{
			MethodBase mi = GetMethodInfo((IMethod)GetEntity(value));
			if (mi.IsStatic)
			{
				_il.Emit(OpCodes.Ldnull);
			}
			else
			{
				Visit(((MemberReferenceExpression)value).Target); PopType();
			}
			_il.Emit(OpCodes.Ldftn, (MethodInfo)mi);
			_il.Emit(OpCodes.Newobj, GetDelegateConstructor(delegateType));
		}
		
		void SubscribeEvent(Node sourceNode, IEntity eventInfo, Expression value)
		{
			EventInfo ei = ((ExternalEvent)eventInfo).EventInfo;
			EmitCreateDelegate(ei.EventHandlerType, value);								
			_il.EmitCall(OpCodes.Callvirt, ei.GetAddMethod(true), null);
		}
		
		void UnsubscribeEvent(Node sourceNode, IEntity eventInfo, Expression value)
		{
			EventInfo ei = ((ExternalEvent)eventInfo).EventInfo;
			EmitCreateDelegate(ei.EventHandlerType, value);								
			_il.EmitCall(OpCodes.Callvirt, ei.GetRemoveMethod(true), null);
		}
		
		void InitializeMember(Node sourceNode, IEntity tag, Expression value)
		{
			switch (tag.EntityType)
			{
				case EntityType.Property:
				{
					IProperty property = (IProperty)tag;
					SetProperty(sourceNode, property, null, value, false);					
					break;
				}
				
				case EntityType.Event:
				{
					SubscribeEvent(sourceNode, tag, value);
					break;
				}
					
				case EntityType.Field:
				{
					SetField(sourceNode, (IField)tag, null, value, false);
					break;					
				}
				
				default:
				{
					throw new ArgumentException("tag");
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
			/*
			LexicalInfo start = startNode.LexicalInfo;
			LexicalInfo end = endNode.LexicalInfo;
			if (start != LexicalInfo.Empty && end != LexicalInfo.Empty)
			{
				_il.MarkSequencePoint(_symbolDocWriter, start.Line, start.StartColumn, end.Line, end.EndColumn);
			}
			*/
		}
		
		void EmitListDisplay(ListLiteralExpression node)
		{
			GeneratorExpression display = (GeneratorExpression)node.Items[0]; 
			
			// list = List()
			LocalBuilder list = _il.DeclareLocal(Types.List);			
			_il.Emit(OpCodes.Newobj, List_EmptyConstructor);
			_il.Emit(OpCodes.Stloc, list);
			
			Label labelTest = _il.DefineLabel();
			Label labelBody = _il.DefineLabel();
			
			LocalBuilder localIterator = _il.DeclareLocal(Types.IEnumerator);
			
			Visit(display.Iterator); PopType();
			_il.EmitCall(OpCodes.Callvirt, IEnumerable_GetEnumerator, null);
			_il.Emit(OpCodes.Stloc, localIterator);
			_il.Emit(OpCodes.Br, labelTest);
			
			_il.MarkLabel(labelBody);
			_il.Emit(OpCodes.Ldloc, localIterator);
			_il.EmitCall(OpCodes.Callvirt, IEnumerator_get_Current, null);
			EmitUnpackForDeclarations(display.Declarations, TypeSystemServices.ObjectType);			
			
			StatementModifier filter = display.Filter; 
			if (null != filter)
			{
				if (StatementModifierType.If == filter.Type)
				{
					EmitBranchFalse(filter.Condition, labelTest);
				}
				else
				{
					EmitBranchTrue(filter.Condition, labelTest);
				}
			}
			
			_il.Emit(OpCodes.Ldloc, list);
			Visit(display.Expression);
			EmitCastIfNeeded(TypeSystemServices.ObjectType, PopType());
			_il.EmitCall(OpCodes.Call, List_Add, null);
			_il.Emit(OpCodes.Pop);
			
			_il.MarkLabel(labelTest);
			_il.Emit(OpCodes.Ldloc, localIterator);
			_il.EmitCall(OpCodes.Callvirt, IEnumerator_MoveNext, null);
			_il.Emit(OpCodes.Brtrue, labelBody);
			
			_il.Emit(OpCodes.Ldloc, list);
		}
		
		void EmitEnumerableBasedFor(ForStatement node, IType iteratorType)
		{			
			Label labelTest = _il.DefineLabel();
			Label labelBody = _il.DefineLabel();
			Label breakLabel = _il.DefineLabel();
			
			LocalBuilder localIterator = _il.DeclareLocal(Types.IEnumerator);
			_il.EmitCall(OpCodes.Callvirt, IEnumerable_GetEnumerator, null);
			_il.Emit(OpCodes.Stloc, localIterator);
			_il.Emit(OpCodes.Br, labelTest);
			
			_il.MarkLabel(labelBody);
			_il.Emit(OpCodes.Ldloc, localIterator);
			_il.EmitCall(OpCodes.Callvirt, IEnumerator_get_Current, null);
			EmitUnpackForDeclarations(node.Declarations, TypeSystemServices.ObjectType);
			
			EnterLoop(breakLabel, labelTest);
			Visit(node.Block);
			LeaveLoop();
			
			// iterator.MoveNext()			
			_il.MarkLabel(labelTest);
			_il.Emit(OpCodes.Ldloc, localIterator);
			_il.EmitCall(OpCodes.Callvirt, IEnumerator_MoveNext, null);
			_il.Emit(OpCodes.Brtrue, labelBody);
			
			_il.MarkLabel(breakLabel);
		}
		
		void EmitArrayBasedFor(ForStatement node, IArrayType iteratorTypeInfo)
		{				
			Label labelTest = _il.DefineLabel();
			Label labelBody = _il.DefineLabel();
			Label continueLabel = _il.DefineLabel();
			Label breakLabel = _il.DefineLabel();
			
			OpCode ldelem = GetLoadEntityOpCode(iteratorTypeInfo.GetElementType());
			
			Type iteratorType = GetSystemType(iteratorTypeInfo);
			LocalBuilder localIterator = _il.DeclareLocal(iteratorType);
			_il.Emit(OpCodes.Stloc, localIterator);
			
			// i = 0;
			LocalBuilder localIndex = _il.DeclareLocal(Types.Int);
			_il.Emit(OpCodes.Ldc_I4_0);
			_il.Emit(OpCodes.Stloc, localIndex);

			_il.Emit(OpCodes.Br, labelTest);
			
			_il.MarkLabel(labelBody);
			
			// value = iterator[i]
			_il.Emit(OpCodes.Ldloc, localIterator);
			_il.Emit(OpCodes.Ldloc, localIndex);
			_il.Emit(ldelem);
			
			EmitUnpackForDeclarations(node.Declarations, iteratorTypeInfo.GetElementType());
			
			EnterLoop(breakLabel, continueLabel);
			Visit(node.Block);
			LeaveLoop();
			
			_il.MarkLabel(continueLabel);
			
			// ++i			
			_il.Emit(OpCodes.Ldloc, localIndex);
			_il.Emit(OpCodes.Ldc_I4_1);
			_il.Emit(OpCodes.Add);
			_il.Emit(OpCodes.Stloc, localIndex);
			
			// i<iterator.Length			
			_il.MarkLabel(labelTest);			
			_il.Emit(OpCodes.Ldloc, localIndex);
			_il.Emit(OpCodes.Ldloc, localIterator);
			_il.Emit(OpCodes.Ldlen);
			_il.Emit(OpCodes.Conv_I4);
			_il.Emit(OpCodes.Blt, labelBody);
			
			_il.MarkLabel(breakLabel);
		}
		
		void EmitUnpackForDeclarations(DeclarationCollection decls, IType topOfStack)
		{
			if (1 == decls.Count)
			{
				// for arg in iterator				
				StoreLocal(topOfStack, GetLocalVariable(decls[0]));
			}
			else
			{
				if (topOfStack.IsArray)
				{						
					IType elementTypeInfo = ((IArrayType)topOfStack).GetElementType();
					
					// RuntimeServices.CheckArrayUnpack(array, decls.Count);					
					_il.Emit(OpCodes.Dup);
					_il.Emit(OpCodes.Ldc_I4, decls.Count);					
					_il.EmitCall(OpCodes.Call, RuntimeServices_CheckArrayUnpack, null);
					
					OpCode ldelem = GetLoadEntityOpCode(elementTypeInfo);
					for (int i=0; i<decls.Count; ++i)
					{
						// local = array[i]
						_il.Emit(OpCodes.Dup);
						_il.Emit(OpCodes.Ldc_I4, i); // element index			
						_il.Emit(ldelem);
						
						StoreLocal(elementTypeInfo, GetLocalVariable(decls[i]));					
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
						StoreLocal(TypeSystemServices.ObjectType, GetLocalVariable(d));				
					}					
				}
				_il.Emit(OpCodes.Pop);
			}
		}
		
		void EmitGetEnumerableIfNeeded(IType topOfStack)
		{
			if (!IsIEnumerableCompatible(topOfStack))
			{
				_il.EmitCall(OpCodes.Call, RuntimeServices_GetEnumerable, null);
			}
		}
		
		bool IsBoolOrInt(IType type)
		{
			return TypeSystemServices.BoolType == type ||
				TypeSystemServices.IntType == type;
		}
		
		bool IsIEnumerableCompatible(IType type)
		{
			return TypeSystemServices.IEnumerableType.IsAssignableFrom(type);
		}
		
		void PushArguments(IMethod tag, ExpressionCollection args)
		{
			IParameter[] parameters = tag.GetParameters();
			for (int i=0; i<args.Count; ++i)
			{
				Expression arg = args[i];
				arg.Accept(this);
				EmitCastIfNeeded(parameters[i].Type, PopType());
			}
		}
		
		void EmitObjectArray(ExpressionCollection items)
		{
			EmitArray(TypeSystemServices.ObjectType, items);
		}
		
		void EmitArray(IType type, ExpressionCollection items)
		{
			_il.Emit(OpCodes.Ldc_I4, items.Count);
			_il.Emit(OpCodes.Newarr, GetSystemType(type));
			
			OpCode opcode = GetStoreEntityOpCode(type);
			for (int i=0; i<items.Count; ++i)
			{			
				StoreEntity(opcode, i, items[i], type);				
			}
		}
		
		bool IsInteger(IType type)
		{
			return type == TypeSystemServices.IntType ||
				type == TypeSystemServices.LongType ||
				type == TypeSystemServices.ByteType;
		}
		
		OpCode GetArithmeticOpCode(IType type, BinaryOperatorType op)
		{
			if (IsInteger(type))
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
		
		OpCode GetLoadEntityOpCode(IType tag)
		{
			if (tag.IsValueType)
			{
				if (TypeSystemServices.IntType == tag)
				{
					return OpCodes.Ldelem_I4;
				}
				if (TypeSystemServices.LongType == tag)
				{
					return OpCodes.Ldelem_I8;
				}
				if (TypeSystemServices.SingleType == tag)
				{
					return OpCodes.Ldelem_R4;
				}
				if (TypeSystemServices.DoubleType == tag)
				{
					return OpCodes.Ldelem_R8;
				}
				NotImplemented("LoadEntityOpCode(" + tag + ")");
			}
			return OpCodes.Ldelem_Ref;
		}		
		
		OpCode GetStoreEntityOpCode(IType tag)
		{
			if (tag.IsValueType)
			{
				if (TypeSystemServices.IntType == tag)
				{
					return OpCodes.Stelem_I4;
				}
				if (TypeSystemServices.LongType == tag)
				{
					return OpCodes.Stelem_I8;
				}
				if (TypeSystemServices.SingleType == tag)
				{
					return OpCodes.Stelem_R4;
				}
				if (TypeSystemServices.DoubleType == tag)
				{
					return OpCodes.Stelem_R8;
				}
				NotImplemented("GetStoreEntityOpCode(" + tag + ")");				
			}
			return OpCodes.Stelem_Ref;
		}
		
		void EmitCastIfNeeded(IType expectedType, IType actualType)
		{			
			if (null == actualType) // see NullLiteralExpression
			{
				return;
			}
			
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
						Type type = GetSystemType(expectedType);
						_il.Emit(OpCodes.Unbox, type);
						_il.Emit(OpCodes.Ldobj, type);
					}
				}
				else
				{
					_context.TraceInfo("castclass: expected type='{0}', type on stack='{1}'", expectedType, actualType);
					_il.Emit(OpCodes.Castclass, GetSystemType(expectedType));
				}
			}
			else
			{
				if (expectedType == TypeSystemServices.ObjectType)
				{
					if (actualType.IsValueType)
					{
						_il.Emit(OpCodes.Box, GetSystemType(actualType));
					}
				}
			}
		}
		
		OpCode GetNumericPromotionOpCode(IType type)
		{
			if (type == TypeSystemServices.IntType)
			{
				return OpCodes.Conv_I4;
			}
			else if (type == TypeSystemServices.LongType)
			{
				return OpCodes.Conv_I8;
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
		
		void StoreLocal(IType topOfStack, LocalVariable local)
		{
			EmitCastIfNeeded(local.Type, topOfStack);
			_il.Emit(OpCodes.Stloc, local.LocalBuilder);
		}
		
		void StoreEntity(OpCode opcode, int index, Node value, IType elementType)
		{
			_il.Emit(OpCodes.Dup);	// array reference
			_il.Emit(OpCodes.Ldc_I4, index); // element index
			value.Accept(this); // value
			EmitCastIfNeeded(elementType, PopType());
			_il.Emit(opcode);
		}		
		
		void DefineEntryPoint()
		{
			Context.GeneratedAssembly = _asmBuilder;
			
			if (CompilerOutputType.Library != Parameters.OutputType)
			{				
				Method method = ContextAnnotations.GetEntryPoint(Context);
				if (null != method)
				{
					Type type = _asmBuilder.GetType(method.DeclaringType.FullName, true);
					MethodInfo createdMethod = type.GetMethod(method.Name, BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic);
					MethodInfo methodBuilder = GetMethodInfo((IMethod)GetEntity(method));
					
					// the mono implementation expects the first argument to 
					// SetEntryPoint to be a MethodBuilder, otherwise it generates
					// bogus assemblies
					_asmBuilder.SetEntryPoint(createdMethod, (PEFileKinds)Parameters.OutputType);
					
					// for the rest of the world (like RunAssembly)
					// the created method is the way to go
					Context.GeneratedAssemblyEntryPoint = createdMethod;
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
		
		EnumBuilder GetEnumBuilder(Node node)
		{
			return (EnumBuilder)_builders[node];
		}
		
		TypeBuilder GetTypeBuilder(Node node)
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
			return GetLocalVariable(local).LocalBuilder;
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
			ExternalField external = tag as ExternalField;
			if (null != external)
			{
				return external.FieldInfo;
			}
			return GetFieldBuilder(((InternalField)tag).Field);
		}
		
		MethodInfo GetMethodInfo(IMethod tag)
		{
			ExternalMethod external = tag as ExternalMethod;
			if (null != external)
			{
				return (MethodInfo)external.MethodInfo;
			}
			return GetMethodBuilder(((InternalMethod)tag).Method);
		}	
		
		ConstructorInfo GetConstructorInfo(IConstructor tag)
		{
			ExternalConstructor external = tag as ExternalConstructor;
			if (null != external)
			{
				return external.ConstructorInfo;
			}
			return GetConstructorBuilder(((InternalMethod)tag).Method);
		}
		
		IType Map(Type type)
		{
			return TypeSystemServices.Map(type);
		}
		
		Type GetSystemType(Node node)
		{
			return GetSystemType(GetType(node));
		}
		
		Type GetSystemType(IType tag)
		{
			Type type = (Type)_typeCache[tag];
			if (null == type)
			{
				ExternalType external = tag as ExternalType;
				if (null != external)
				{
					type = external.ActualType;
				}
				else
				{
					if (tag.IsArray)
					{				
						IArrayType arrayType = (IArrayType)tag;
						IType elementType = GetSimpleEntityType(arrayType);						
						if (elementType is IInternalEntity)
						{
							string typeName = GetArrayTypeName(arrayType);
							type = _moduleBuilder.GetType(typeName, true);
						}
						else
						{
							//type = Type.GetType(typeName, true);
							type = Array.CreateInstance(GetSystemType(arrayType.GetElementType()), 0).GetType();
						}
					}
					else
					{
						if (Null.Default == tag)
						{
							type = Types.Object;
						}
						else
						{
							type = (Type)GetBuilder(((AbstractInternalType)tag).TypeDefinition);
						}
					}
				}
				if (null == type)
				{
					throw new InvalidOperationException(string.Format("Could not find a Type for {0}.", tag));
				}
				_typeCache.Add(tag, type);
			}
			return type;
		}
		
		IType GetSimpleEntityType(IArrayType tag)
		{
			return GetSimpleEntityType(tag.GetElementType());
		}
		
		IType GetSimpleEntityType(IType tag)
		{
			if (tag.IsArray)
			{
				return GetSimpleEntityType(((IArrayType)tag).GetElementType());
			}
			return tag;
		}
		
		string GetArrayTypeName(IType tag)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			GetArrayTypeName(builder, tag);
			return builder.ToString();			
		}
		
		void GetArrayTypeName(System.Text.StringBuilder buffer, IType tag)
		{
			if (tag.IsArray)
			{
				GetArrayTypeName(buffer, ((IArrayType)tag).GetElementType());
				buffer.Append("[]");
			}
			else
			{
				buffer.Append(tag.FullName);
			}
		}
		
		TypeAttributes GetNestedTypeAttributes(TypeMember type)
		{
			TypeAttributes attributes = type.IsPublic ? TypeAttributes.NestedPublic : TypeAttributes.NestedPrivate;
			return GetExtendedTypeAttributes(attributes, type);			
		}
		
		TypeAttributes GetTypeAttributes(TypeMember type)
		{
			TypeAttributes attributes = type.IsPublic ? TypeAttributes.Public : TypeAttributes.NotPublic;
			return GetExtendedTypeAttributes(attributes, type);
		}
		
		TypeAttributes GetExtendedTypeAttributes(TypeAttributes attributes, TypeMember type)
		{
			
			switch (type.NodeType)
			{				
				case NodeType.ClassDefinition:
				{
					attributes |= (TypeAttributes.AnsiClass | TypeAttributes.AutoLayout);
					attributes |= TypeAttributes.Class;
					attributes |= TypeAttributes.Serializable;
					if (type.IsAbstract)
					{
						attributes |= TypeAttributes.Abstract;
					}
					if (type.IsFinal)
					{
						attributes |= TypeAttributes.Sealed;
					}
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
			return PropertyAttributes.None;
		}
		
		MethodAttributes GetMethodAttributes(Method method)
		{
			MethodAttributes attributes = MethodAttributes.HideBySig;
			if (method.IsPublic)
			{
				attributes |= MethodAttributes.Public;			
			}
			else if (method.IsProtected)
			{
				attributes |= MethodAttributes.Family;
			}
			else if (method.IsPrivate)
			{
				attributes |= MethodAttributes.Private;
			}
			
			if (method.IsStatic)
			{
				attributes |= MethodAttributes.Static;
			}
			else if (method.IsOverride || method.IsVirtual)
			{
				attributes |= MethodAttributes.Virtual;
			}
			if (method.IsAbstract)
			{
				attributes |= (MethodAttributes.Abstract | MethodAttributes.Virtual);
			}
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
			return attributes;
		}
		
		void DefineProperty(TypeBuilder typeBuilder, Property property)
		{
			PropertyBuilder builder = typeBuilder.DefineProperty(property.Name, 
			                                            GetPropertyAttributes(property),
			                                            GetSystemType(property.Type),
			                                            GetParameterTypes(property.Parameters));
			Method getter = property.Getter;
			Method setter = property.Setter;
			
			if (null != getter)
			{
				MethodBuilder getterBuilder = DefineMethod(typeBuilder, getter, MethodAttributes.SpecialName);
				builder.SetGetMethod(getterBuilder);
			}
			if (null != setter)
			{
				MethodBuilder setterBuilder = DefineMethod(typeBuilder, setter, MethodAttributes.SpecialName);
				builder.SetSetMethod(setterBuilder);
			}
			
			foreach (Boo.Lang.Compiler.Ast.Attribute attribute in property.Attributes)
			{
				builder.SetCustomAttribute(GetCustomAttributeBuilder(attribute));
			}
			
			SetBuilder(property, builder);
		}
		
		void DefineField(TypeBuilder typeBuilder, Field field)
		{
			FieldBuilder builder = typeBuilder.DefineField(field.Name, 
			                                               GetSystemType(field), 
			                                               GetFieldAttributes(field));
			SetBuilder(field, builder);
		}
		
		void DefineParameters(MethodBuilder builder, ParameterDeclarationCollection parameters)
		{
			for (int i=0; i<parameters.Count; ++i)
			{
				builder.DefineParameter(i+1, ParameterAttributes.None, parameters[i].Name);
			}
		}
		
		void DefineParameters(ConstructorBuilder builder, ParameterDeclarationCollection parameters)
		{
			for (int i=0; i<parameters.Count; ++i)
			{
				builder.DefineParameter(i+1, ParameterAttributes.None, parameters[i].Name);
			}
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
			if (typeBuilder.IsInterface)
			{
				methodAttributes |= (MethodAttributes.Virtual | MethodAttributes.Abstract);
			}
			
			MethodBuilder builder = typeBuilder.DefineMethod(method.Name, 
                                        methodAttributes,
                                        GetSystemType(method.ReturnType),                             
										GetParameterTypes(parameters));

			builder.SetImplementationFlags(GetImplementationFlags(method));										
			
			DefineParameters(builder, parameters);				
			
			SetBuilder(method, builder);
			foreach (Boo.Lang.Compiler.Ast.Attribute attribute in method.Attributes)
			{
				builder.SetCustomAttribute(GetCustomAttributeBuilder(attribute));
			}			
			return builder;
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
			if (IsEnumDefinition(typeDefinition))
			{				
				EnumBuilder enumBuilder = _moduleBuilder.DefineEnum(typeDefinition.FullName,
											GetTypeAttributes(typeDefinition),
											typeof(long));
											
				
				foreach (EnumMember member in typeDefinition.Members)
				{
					enumBuilder.DefineLiteral(member.Name, (long)member.Initializer.Value);
				}				
				SetBuilder(typeDefinition, enumBuilder);
			}
			else
			{					
				TypeBuilder typeBuilder = CreateTypeBuilder(typeDefinition);
				SetBuilder(typeDefinition, typeBuilder);
			}
		}
		
		TypeBuilder CreateTypeBuilder(TypeMember type)
		{
			TypeBuilder typeBuilder = null;
			ClassDefinition  enclosingType = type.ParentNode as ClassDefinition;
			if (null == enclosingType)
			{
				typeBuilder = _moduleBuilder.DefineType(type.FullName,
										GetTypeAttributes(type));
			}
			else
			{
				typeBuilder = GetTypeBuilder(enclosingType).DefineNestedType(type.Name,
																GetNestedTypeAttributes(type));
			}
			return typeBuilder;
		}
		
		void EmitBaseTypesAndAttributes(TypeDefinition typeDefinition, TypeBuilder typeBuilder)
		{			
			foreach (TypeReference baseType in typeDefinition.BaseTypes)
			{
				Type type = GetSystemType(baseType);
				if (type.IsClass)
				{					
					typeBuilder.SetParent(type);
				}
				else
				{
					typeBuilder.AddInterfaceImplementation(type);
				}
			}
			EmitAttributes(typeBuilder, typeDefinition);
		}
		
		void EmitAttributes(TypeBuilder typeBuilder, TypeDefinition typeDefinition)
		{			
			foreach (Boo.Lang.Compiler.Ast.Attribute attribute in typeDefinition.Attributes)
			{
				typeBuilder.SetCustomAttribute(GetCustomAttributeBuilder(attribute));
			}
		}
		
		void NotImplemented(string feature)
		{
			throw new NotImplementedException(feature);
		}
		
		CustomAttributeBuilder GetCustomAttributeBuilder(Boo.Lang.Compiler.Ast.Attribute node)
		{
			IConstructor constructor = (IConstructor)GetEntity(node);
			ConstructorInfo constructorInfo = GetConstructorInfo(constructor);
			object[] constructorArgs = GetValues(node.Arguments);
			
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
			Boo.Lang.List namedProperties = new Boo.Lang.List();
			Boo.Lang.List propertyValues = new Boo.Lang.List();
			Boo.Lang.List namedFields = new Boo.Lang.List();
			Boo.Lang.List fieldValues = new Boo.Lang.List();
			foreach (ExpressionPair pair in values)
			{
				IEntity tag = GetEntity(pair.First);
				if (EntityType.Property == tag.EntityType)
				{
					namedProperties.Add(GetPropertyInfo(tag));
					propertyValues.Add(GetValue(pair.Second));
				}
				else
				{
					namedFields.Add(GetFieldInfo((IField)tag));
					fieldValues.Add(GetValue(pair.Second));
				}
			}
			
			outNamedProperties = (PropertyInfo[])namedProperties.ToArray(typeof(PropertyInfo));
			outPropertyValues = (object[])propertyValues.ToArray();
			outNamedFields = (FieldInfo[])namedFields.ToArray(typeof(FieldInfo));
			outFieldValues = (object[])fieldValues.ToArray();
		}
		
		object[] GetValues(ExpressionCollection expressions)
		{
			object[] values = new object[expressions.Count];
			for (int i=0; i<values.Length; ++i)
			{
				values[i] = GetValue(expressions[i]);
			}
			return values;
		}
		
		object GetValue(Expression expression)
		{
			switch (expression.NodeType)
			{
				case NodeType.StringLiteralExpression:
				{
					return ((StringLiteralExpression)expression).Value;
				}
				
				case NodeType.BoolLiteralExpression:
				{
					return ((BoolLiteralExpression)expression).Value;
				}
				
				case NodeType.TypeofExpression:
				{
					return GetSystemType(((TypeofExpression)expression).Type);
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
							return field.StaticValue;
						}
					}
					break;
				}
			}
			NotImplemented(expression, "Expression value");
			return null;
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
				}
			}
		}
		
		string GetAssemblyName(string fname)
		{
			return Path.GetFileNameWithoutExtension(fname);
		}
		
		string GetTargetDirectory(string fname)
		{
			return Path.GetDirectoryName(Path.GetFullPath(fname));
		}
		
		string BuildOutputAssemblyName()
		{				
			string fname = Parameters.OutputAssembly;
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
				IResourceWriter writer = _moduleBuilder.DefineResource(resource.Name, resource.Description);
				resource.WriteResources(writer);
			}
		}
		
		void SetUpAssembly()
		{
			if (0 == Parameters.OutputAssembly.Length)
			{				
				Parameters.OutputAssembly = CompileUnit.Modules[0].Name;			
			}
			
			Parameters.OutputAssembly = BuildOutputAssemblyName();
			
			AssemblyName asmName = new AssemblyName();
			asmName.Name = GetAssemblyName(Parameters.OutputAssembly);
			
			_asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave, GetTargetDirectory(Parameters.OutputAssembly));
			_moduleBuilder = _asmBuilder.DefineDynamicModule(asmName.Name, Path.GetFileName(Parameters.OutputAssembly), true);			
			ContextAnnotations.SetAssemblyBuilder(Context, _asmBuilder);
		}
	}
}
