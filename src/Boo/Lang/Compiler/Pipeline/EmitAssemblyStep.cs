#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Collections;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using Boo.Lang.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Bindings;

namespace Boo.Lang.Compiler.Pipeline
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
	
	public class EmitAssemblyStep : AbstractSwitcherCompilerStep
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
		
		static ConstructorInfo List_IntConstructor = Types.List.GetConstructor(new Type[] { typeof(int) });
		
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
		ITypeBinding _returnType;
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
		
		void PushType(ITypeBinding type)
		{
			_types.Push(type);
		}
		
		void PushBool()
		{
			PushType(BindingManager.BoolTypeBinding);
		}
		
		void PushVoid()
		{
			PushType(BindingManager.VoidTypeBinding);
		}
		
		ITypeBinding PopType()
		{
			return (ITypeBinding)_types.Pop();
		}
		
		ITypeBinding PeekTypeOnStack()
		{
			return (ITypeBinding)_types.Peek();
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
			if (Errors.Count > 0 || 0 == CompileUnit.Modules.Count)
			{
				return;				
			}
			
			SetUpAssembly();
			
			ArrayList types = CollectTypes();
			
			foreach (TypeDefinition type in types)
			{
				DefineType(type);
			}
			
			foreach (TypeDefinition type in types)
			{
				DefineTypeMembers(type);
			}
			
			foreach (Boo.Lang.Ast.Module module in CompileUnit.Modules)
			{
				OnModule(module);
			}
			
			CreateTypes(types);
			
			DefineEntryPoint();			
		}
		
		void CreateTypes(ArrayList types)
		{
			Hashtable created = new Hashtable();
			foreach (TypeDefinition type in types)
			{
				CreateType(created, type);
			}
		}
		
		void CreateType(Hashtable created, TypeDefinition type)
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
					foreach (TypeReference baseTypeRef in type.BaseTypes)
					{
						InternalTypeBinding binding = GetBoundType(baseTypeRef) as InternalTypeBinding;
						if (null != binding)
						{
							CreateType(created, binding.TypeDefinition);
						}
					}				
					GetTypeBuilder(type).CreateType();
				}
			}
		}
		
		ArrayList CollectTypes()
		{
			ArrayList types = new ArrayList();
			foreach (Boo.Lang.Ast.Module module in CompileUnit.Modules)
			{				 
				CollectTypes(types, module.Members);
			}
			return types;
		}
		
		void CollectTypes(ArrayList types, TypeMemberCollection members)
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
			_symbolDocWriter = null;
			_typeBuilder = null;
			_il = null;		
			_returnValueLocal = null;
			_returnType = null;
			_tryBlock = 0;
			_types.Clear();
			_typeCache.Clear();
		}
		
		override public void OnAttribute(Boo.Lang.Ast.Attribute node)
		{
		}
		
		override public void OnModule(Boo.Lang.Ast.Module module)
		{			
			_symbolDocWriter = _moduleBuilder.DefineDocument(module.LexicalInfo.FileName, Guid.Empty, Guid.Empty, Guid.Empty);			
			Switch(module.Members);
		}
		
		override public void OnEnumDefinition(EnumDefinition node)
		{
			EnumBuilder builder = GetEnumBuilder(node);
			foreach (Boo.Lang.Ast.Attribute attribute in node.Attributes)
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
				builder.AddInterfaceImplementation(GetType(baseType));
			}
			EmitAttributes(builder, node);
		}
		
		void EmitTypeDefinition(TypeDefinition node)
		{
			TypeBuilder current = GetTypeBuilder(node);
			EmitBaseTypesAndAttributes(node, current);			
			Switch(node.Members);
			
			_typeBuilder = current;
		}		
		
		override public void OnMethod(Method method)
		{			
			MethodBuilder methodBuilder = GetMethodBuilder(method);			
			_il = methodBuilder.GetILGenerator();
			_returnLabel = _il.DefineLabel();
			
			_returnType = ((IMethodBinding)GetBinding(method)).ReturnType;
			if (BindingManager.VoidTypeBinding != _returnType)
			{
				_returnValueLocal = _il.DeclareLocal(GetType(_returnType));
			}
			
			Switch(method.Locals);
			Switch(method.Body);
			
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
			ConstructorBuilder builder = GetConstructorBuilder(constructor);
			_il = builder.GetILGenerator();

			InternalConstructorBinding binding = (InternalConstructorBinding)GetBinding(constructor);
			Switch(constructor.Locals);
			Switch(constructor.Body);
			_il.Emit(OpCodes.Ret);
		}
		
		override public void OnLocal(Local local)
		{			
			LocalBinding info = GetLocalBinding(local);
			info.LocalBuilder = _il.DeclareLocal(GetType(local));
			info.LocalBuilder.SetLocalSymInfo(local.Name);			
		}
		
		override public void OnForStatement(ForStatement node)
		{									
			EmitDebugInfo(node, node.Iterator);
			
			// iterator = <node.Iterator>;
			node.Iterator.Switch(this);		
			
			ITypeBinding iteratorType = PopType();
			if (iteratorType.IsArray)
			{
				EmitArrayBasedFor(node, iteratorType);
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
				Switch(node.Expression);
				EmitCastIfNeeded(_returnType, PopType());
				_il.Emit(OpCodes.Stloc, _returnValueLocal);
			}
			_il.Emit(retOpCode, _returnLabel);
		}
		
		override public void OnRaiseStatement(RaiseStatement node)
		{
			Switch(node.Exception); PopType();
			_il.Emit(OpCodes.Throw);
		}
		
		override public void OnTryStatement(TryStatement node)
		{
			++_tryBlock;
			
			Label endLabel = _il.BeginExceptionBlock();
			Switch(node.ProtectedBlock);
			Switch(node.ExceptionHandlers);
			if (null != node.EnsureBlock)
			{
				_il.BeginFinallyBlock();
				Switch(node.EnsureBlock);
			}
			_il.EndExceptionBlock();
			
			--_tryBlock;
		}
		
		override public void OnExceptionHandler(ExceptionHandler node)
		{
			_il.BeginCatchBlock(GetType(node.Declaration));
			_il.Emit(OpCodes.Stloc, GetLocalBuilder(node.Declaration));
			Switch(node.Block);
		}
		
		override public void OnUnpackStatement(UnpackStatement node)
		{
			DeclarationCollection decls = node.Declarations;
			
			EmitDebugInfo(decls[0], node.Expression);						
			node.Expression.Switch(this);
			
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
			if (PopType() != BindingManager.VoidTypeBinding)
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
			node.Block.Switch(this);
			_il.MarkLabel(endLabel);
		}
		
		override public void OnIfStatement(IfStatement node)
		{
			EmitDebugInfo(node);
			
			Label endLabel = _il.DefineLabel();
			
			EmitBranchFalse(node.Condition, endLabel);
			node.TrueBlock.Switch(this);
			if (null != node.FalseBlock)
			{
				Label elseEndLabel = _il.DefineLabel();
				_il.Emit(OpCodes.Br, elseEndLabel);
				_il.MarkLabel(endLabel);
				
				endLabel = elseEndLabel;
				node.FalseBlock.Switch(this);
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
					Switch(expression.Left); PopType();
					Switch(expression.Right); PopType();
					_il.Emit(OpCodes.Beq, label);
					break;
				}
				
				case BinaryOperatorType.ReferenceInequality:
				{
					Switch(expression.Left); PopType();
					Switch(expression.Right); PopType();
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
			expression.Switch(this);
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
			expression.Switch(this);
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
			node.Block.Switch(this);
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
					node.Operand.Switch(this);
					ITypeBinding typeOnStack = PopType();
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
			node.Left.Switch(this); PopType();
			node.Right.Switch(this); PopType();
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
			Switch(slice.Target); 
			
			ITypeBinding arrayType = PopType();
			ITypeBinding elementType = arrayType.GetElementType();
			EmitNormalizedArrayIndex(slice.Begin);			
			
			Switch(node.Right);
			EmitCastIfNeeded(elementType, PopType());
			
			bool leaveValueOnStack = ShouldLeaveValueOnStack(node);
			LocalBuilder temp = null;
			if (leaveValueOnStack)
			{
				_il.Emit(OpCodes.Dup);
				temp = _il.DeclareLocal(GetType(elementType));
				_il.Emit(OpCodes.Stloc, temp);				
			}
			
			_il.Emit(GetStoreElementOpCode(elementType));
			
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
			IBinding binding = BindingManager.GetBinding(node.Left);
			switch (binding.BindingType)
			{
				case BindingType.Local:
				{
					SetLocal(node, (LocalBinding)binding, leaveValueOnStack);
					break;
				}
				
				case BindingType.Parameter:
				{
					ParameterBinding param = (ParameterBinding)binding;
					
					Switch(node.Right);
					EmitCastIfNeeded(param.BoundType, PopType());
					
					if (leaveValueOnStack)
					{
						_il.Emit(OpCodes.Dup);
						PushType(param.BoundType);
					}
					_il.Emit(OpCodes.Starg, param.Index);
					break;
				}
				
				case BindingType.Field:
				{
					IFieldBinding field = (IFieldBinding)binding;
					SetField(node, field, node.Left, node.Right, leaveValueOnStack);
					break;
				}
				
				case BindingType.Property:
				{
					SetProperty(node, (IPropertyBinding)binding, node.Left, node.Right, leaveValueOnStack);
					break;
				}
					
				default:
				{
					NotImplemented(node, binding.ToString());
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
			Switch(node.Left); PopType();
			_il.Emit(OpCodes.Isinst, GetType(node.Right));
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
			ITypeBinding lhs = GetBoundType(node.Left);
			ITypeBinding rhs = GetBoundType(node.Right);
			
			ITypeBinding type = BindingManager.GetPromotedNumberType(lhs, rhs);
			Switch(node.Left);
			EmitCastIfNeeded(type, PopType());
			Switch(node.Right);
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
			Switch(node.Left);
			EmitCastIfNeeded(BindingManager.DoubleTypeBinding, PopType());
			Switch(node.Right);
			EmitCastIfNeeded(BindingManager.DoubleTypeBinding, PopType());
			_il.EmitCall(OpCodes.Call, Math_Pow, null);
			PushType(BindingManager.DoubleTypeBinding);			
		}
		
		void OnArithmeticOperator(BinaryExpression node)
		{
			ITypeBinding type = GetBoundType(node);
			node.Left.Switch(this); EmitCastIfNeeded(type, PopType());
			node.Right.Switch(this); EmitCastIfNeeded(type, PopType());
			_il.Emit(GetArithmeticOpCode(type, node.Operator));
			PushType(type);
		}
		
		void EmitToBoolIfNeeded(ITypeBinding topOfStack)
		{
			if (BindingManager.ObjectTypeBinding == topOfStack)
			{
				_il.EmitCall(OpCodes.Call, RuntimeServices_ToBool, null);
			}
		}
		
		void EmitAnd(BinaryExpression node)
		{
			EmitLogicalOperator(node, OpCodes.Brtrue, OpCodes.Brfalse); 
			/*
			ITypeBinding type = GetBoundType(node);
			Switch(node.Left);
			
			ITypeBinding lhsType = PopType();
			
			ITypeBinding lhsType = PopType();
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
				Switch(node.Right);
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
				Switch(node.Right);
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
			ITypeBinding type = GetBoundType(node);
			Switch(node.Left);
			
			ITypeBinding lhsType = PopType();
			
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
				Switch(node.Right);
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
				Switch(node.Right);
				EmitCastIfNeeded(type, PopType());
				_il.MarkLabel(end);
			}
			
			PushType(type);
		}
		
		void EmitBitwiseOperator(BinaryExpression node)
		{
			ITypeBinding type = GetBoundType(node);
			
			Switch(node.Left);
			EmitCastIfNeeded(type, PopType());
			
			Switch(node.Right);
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
					Switch(((MemberReferenceExpression)node.Left).Target); PopType();
					AddDelegate(node, GetBinding(node.Left), node.Right);
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
		
		override public void OnAsExpression(AsExpression node)
		{
			Type type = GetType(node.Type);
			
			node.Target.Switch(this); PopType();			
			_il.Emit(OpCodes.Isinst, type);
			PushType(GetBoundType(node));
		}
		
		void InvokeMethod(IMethodBinding methodBinding, MethodInvocationExpression node)
		{			
			MethodInfo mi = GetMethodInfo(methodBinding);
			OpCode code = OpCodes.Call;
			if (!mi.IsStatic)
			{				
				Expression target = ((MemberReferenceExpression)node.Target).Target;
				ITypeBinding targetType = GetBoundType(target);
				if (targetType.IsValueType)
				{				
					if (mi.DeclaringType == Types.Object)
					{
						Switch(node.Target); 
						_il.Emit(OpCodes.Box, GetType(PopType()));
					}
					else
					{
						LoadAddress(target);
					}
				}
				else
				{
					// pushes target reference
					Switch(node.Target); PopType();
					if (mi.IsVirtual)
					{
						code = OpCodes.Callvirt;
					}
				}
			}
			PushArguments(methodBinding, node.Arguments);
			_il.EmitCall(code, mi, null);
			
			PushType(methodBinding.ReturnType);
		}
		
		void InvokeSuperMethod(IMethodBinding methodBinding, MethodInvocationExpression node)
		{
			IMethodBinding super = ((InternalMethodBinding)methodBinding).Override;
			MethodInfo superMI = GetMethodInfo(super);
			_il.Emit(OpCodes.Ldarg_0); // this
			PushArguments(super, node.Arguments);
			_il.EmitCall(OpCodes.Call, superMI, null);
			PushType(super.ReturnType);
		}
		
		void OnSpecialFunction(IBinding binding, MethodInvocationExpression node)
		{
			EmitGetTypeFromHandle(GetType(node.Arguments[0]));
		}
		
		void EmitGetTypeFromHandle(Type type)
		{
			_il.Emit(OpCodes.Ldtoken, type);
			_il.EmitCall(OpCodes.Call, Type_GetTypeFromHandle, null);
			PushType(BindingManager.TypeTypeBinding);
		}
		
		override public void OnMethodInvocationExpression(MethodInvocationExpression node)
		{				
			IBinding binding = BindingManager.GetBinding(node.Target);
			switch (binding.BindingType)
			{
				case BindingType.SpecialFunction:
				{
					OnSpecialFunction(binding, node);
					break;
				}
				
				case BindingType.Method:
				{	
					IMethodBinding methodBinding = (IMethodBinding)binding;
					
					if (node.Target.NodeType == NodeType.SuperLiteralExpression)
					{
						InvokeSuperMethod(methodBinding, node);
					}
					else
					{						
						InvokeMethod(methodBinding, node);
					}
					
					break;
				}
				
				case BindingType.Constructor:
				{
					IConstructorBinding constructorBinding = (IConstructorBinding)binding;
					ConstructorInfo ci = GetConstructorInfo(constructorBinding);
					
					if (NodeType.SuperLiteralExpression == node.Target.NodeType)
					{
						// super constructor call
						_il.Emit(OpCodes.Ldarg_0);
						PushArguments(constructorBinding, node.Arguments);
						_il.Emit(OpCodes.Call, ci);
						PushVoid();
					}
					else
					{
						PushArguments(constructorBinding, node.Arguments);
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
						PushType(constructorBinding.DeclaringType);
					}
					break;
				}
				
				default:
				{
					NotImplemented(node, binding.ToString());
					break;
				}
			}
		}
		
		override public void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node)
		{
			_il.Emit(OpCodes.Ldc_I8, node.Value.Ticks);
			_il.Emit(OpCodes.Newobj, TimeSpan_LongConstructor);
			PushType(BindingManager.TimeSpanTypeBinding);
		}
		
		override public void OnIntegerLiteralExpression(IntegerLiteralExpression node)
		{
			if (node.IsLong)
			{
				_il.Emit(OpCodes.Ldc_I8, node.Value);
				PushType(BindingManager.LongTypeBinding);
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
				PushType(BindingManager.IntTypeBinding);
			}			
		}
		
		override public void OnDoubleLiteralExpression(DoubleLiteralExpression node)
		{
			_il.Emit(OpCodes.Ldc_R8, node.Value);
			PushType(BindingManager.DoubleTypeBinding);
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
			
			ITypeBinding objType = BindingManager.ObjectTypeBinding;
			foreach (ExpressionPair pair in node.Items)
			{
				_il.Emit(OpCodes.Dup);
				Switch(pair.First);
				EmitCastIfNeeded(objType, PopType());
				
				Switch(pair.Second);
				EmitCastIfNeeded(objType, PopType());
				_il.EmitCall(OpCodes.Call, Hash_Add, null);
			}
			PushType(BindingManager.HashTypeBinding);
		}
		
		override public void OnListLiteralExpression(ListLiteralExpression node)
		{
			if (node.Items.Count > 0)
			{
				_il.Emit(OpCodes.Ldc_I4, node.Items.Count);
				_il.Emit(OpCodes.Newobj, List_IntConstructor);
				
				foreach (Expression item in node.Items)
				{					
					item.Switch(this);
					EmitCastIfNeeded(BindingManager.ObjectTypeBinding, PopType());
					_il.EmitCall(OpCodes.Call, List_Add, null);
					// List_Add will return the list itself
				}
			}
			else
			{
				_il.Emit(OpCodes.Newobj, List_EmptyConstructor);			
			}
			PushType(BindingManager.ListTypeBinding);
		}
		
		override public void OnTupleLiteralExpression(TupleLiteralExpression node)
		{
			ITypeBinding type = GetBoundType(node);
			EmitArray(type.GetElementType(), node.Items);
			PushType(type);
		}
		
		override public void OnRELiteralExpression(RELiteralExpression node)
		{
			_il.Emit(OpCodes.Ldstr, RuntimeServices.Mid(node.Value, 1, -1));
			_il.Emit(OpCodes.Newobj, Regex_Constructor);
			PushType(GetBoundType(node));
		}
		
		override public void OnStringLiteralExpression(StringLiteralExpression node)
		{
			_il.Emit(OpCodes.Ldstr, node.Value);
			PushType(BindingManager.StringTypeBinding);
		}
		
		override public void OnSlicingExpression(SlicingExpression node)
		{			
			if (AstUtil.IsLhsOfAssignment(node))
			{
				return;
			}
			
			Switch(node.Target); 			
			ITypeBinding type = PopType();

			EmitNormalizedArrayIndex(node.Begin);
			_il.Emit(GetLoadElementOpCode(type.GetElementType()));			
			
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
			Switch(expression);
			EmitCastIfNeeded(BindingManager.IntTypeBinding, PopType());
		}
		
		static Regex _interpolatedExpression = new Regex(@"\{(\d+)\}", RegexOptions.Compiled|RegexOptions.CultureInvariant);
		
		void EmitAppendString(MethodInfo appendString, string template, int current, int matchIndex)
		{
			string part = template.Substring(current, matchIndex-current);
			if (part.Length > 0)
			{
				_il.Emit(OpCodes.Ldstr, part);
				_il.EmitCall(OpCodes.Call, appendString, null);
			}
		}
		
		override public void OnStringFormattingExpression(StringFormattingExpression node)
		{	
			Type stringBuilderType = typeof(StringBuilder);
			ConstructorInfo constructor =  stringBuilderType.GetConstructor(new Type[] { typeof(int) });
			MethodInfo appendObject = stringBuilderType.GetMethod("Append", new Type[] { typeof(object) });
			MethodInfo appendString = stringBuilderType.GetMethod("Append", new Type[] { typeof(string) });
			
			string template = node.Template;
			
			_il.Emit(OpCodes.Ldc_I4, (int)template.Length*2);
			_il.Emit(OpCodes.Newobj, constructor);
			
			int current = 0;
			Match m = _interpolatedExpression.Match(template);
			foreach (Expression arg in node.Arguments)
			{	
				EmitAppendString(appendString, template, current, m.Index);				
				current = m.Index + m.Length;

				Switch(arg);
				
				ITypeBinding argType = PopType();
				if (BindingManager.StringTypeBinding == argType)
				{
					_il.EmitCall(OpCodes.Call, appendString, null);
				}
				else
				{
					EmitCastIfNeeded(BindingManager.ObjectTypeBinding, argType);
					_il.EmitCall(OpCodes.Call, appendObject, null);
				}

				m = m.NextMatch();
			}
			EmitAppendString(appendString, template, current, template.Length);
			_il.EmitCall(OpCodes.Call, stringBuilderType.GetMethod("ToString", new Type[0]), null);
			PushType(BindingManager.StringTypeBinding);
		}
		
		void EmitLoadField(Expression self, IFieldBinding fieldBinding)
		{
			if (fieldBinding.IsStatic)
			{
				if (fieldBinding.DeclaringType.IsEnum)
				{
					_il.Emit(OpCodes.Ldc_I4, Convert.ToInt32(fieldBinding.StaticValue));							
				}
				else
				{
					_il.Emit(OpCodes.Ldsfld, GetFieldInfo(fieldBinding));							
				}
			}
			else
			{						
				Switch(self); PopType();
				_il.Emit(OpCodes.Ldfld, GetFieldInfo(fieldBinding));						
			}
			PushType(fieldBinding.BoundType);
		}
		
		override public void OnMemberReferenceExpression(MemberReferenceExpression node)
		{			
			IBinding binding = BindingManager.GetBinding(node);
			switch (binding.BindingType)
			{				
				case BindingType.Method:
				{
					node.Target.Switch(this);
					break;
				}
				
				case BindingType.Field:
				{
					EmitLoadField(node.Target, (IFieldBinding)binding);
					break;
				}
				
				case BindingType.TypeReference:
				{
					EmitGetTypeFromHandle(GetType(node));
					break;
				}
				
				default:
				{
					NotImplemented(node, binding.ToString());
					break;
				}
			}
		}
		
		void LoadAddress(Expression expression)
		{
			IBinding binding = GetBinding(expression);
			switch (binding.BindingType)
			{
				case BindingType.Local:
				{				
					_il.Emit(OpCodes.Ldloca, ((LocalBinding)binding).LocalBuilder);
					break;
				}
				
				case BindingType.Parameter:
				{
					_il.Emit(OpCodes.Ldarga, ((ParameterBinding)binding).Index);
					break;
				}
				
				default:
				{
					// declare local to hold value type
					Switch(expression); 
					LocalBuilder temp = _il.DeclareLocal(GetType(PopType()));
					_il.Emit(OpCodes.Stloc, temp);
					_il.Emit(OpCodes.Ldloca, temp);
					break;
				}
			}
		}
		
		override public void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			_il.Emit(OpCodes.Ldarg_0);
			PushType(GetBoundType(node));
		}
		
		override public void OnNullLiteralExpression(NullLiteralExpression node)
		{
			_il.Emit(OpCodes.Ldnull);
			PushType(null);
		}
		
		override public void OnReferenceExpression(ReferenceExpression node)
		{	
			IBinding info = BindingManager.GetBinding(node);
			switch (info.BindingType)
			{
				case BindingType.Local:
				{
					LocalBinding local = (LocalBinding)info;
					LocalBuilder builder = local.LocalBuilder;
					_il.Emit(OpCodes.Ldloc, builder);
					PushType(local.BoundType);
					break;
				}
				
				case BindingType.Parameter:
				{
					Bindings.ParameterBinding param = (Bindings.ParameterBinding)info;
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
					PushType(param.BoundType);
					break;
				}
				
				case BindingType.TypeReference:
				{
					EmitGetTypeFromHandle(GetType(node));
					break;
				}
				
				default:
				{
					NotImplemented(node, info.ToString());
					break;
				}
				
			}			
		}
		
		void SetLocal(BinaryExpression node, LocalBinding binding, bool leaveValueOnStack)
		{
			node.Right.Switch(this); // leaves type on stack
					
			ITypeBinding typeOnStack = null;
			
			if (leaveValueOnStack)
			{	
				typeOnStack = PeekTypeOnStack();
				_il.Emit(OpCodes.Dup);
			}
			else
			{
				typeOnStack = PopType();
			}
			EmitAssignment(binding, typeOnStack);
		}
		
		void EmitAssignment(LocalBinding binding, ITypeBinding typeOnStack)
		{			
			// todo: assignment result must be type on the left in the
			// case of casting
			LocalBuilder local = binding.LocalBuilder;
			EmitCastIfNeeded(binding.BoundType, typeOnStack);
			_il.Emit(OpCodes.Stloc, local);
		}
		
		void SetField(Node sourceNode, IFieldBinding field, Expression reference, Expression value, bool leaveValueOnStack)
		{
			OpCode opSetField = OpCodes.Stsfld;
			if (!field.IsStatic)				
			{
				opSetField = OpCodes.Stfld;
				if (null != reference)
				{
					((MemberReferenceExpression)reference).Target.Switch(this);
					PopType();
				}
			}
			
			value.Switch(this);
			EmitCastIfNeeded(field.BoundType, PopType());
			
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
				PushType(field.BoundType);
			}
		}
		
		void SetProperty(Node sourceNode, IPropertyBinding property, Expression reference, Expression value, bool leaveValueOnStack)
		{
			PropertyInfo pi = GetPropertyInfo(property);			
			MethodInfo setMethod = pi.GetSetMethod(true);
			
			if (null != reference)
			{
				if (!setMethod.IsStatic)
				{
					((MemberReferenceExpression)reference).Target.Switch(this);
					PopType();
				}
			}
			
			value.Switch(this);
			EmitCastIfNeeded(property.BoundType, PopType());
			
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
				PushType(property.BoundType);
			}
		}
		
		void AddDelegate(Node sourceNode, IBinding eventBinding, Expression value)
		{
			MethodBase mi = GetMethodInfo((IMethodBinding)GetBinding(value));
			if (mi.IsStatic)
			{
				_il.Emit(OpCodes.Ldnull);
			}
			else
			{
				Switch(((MemberReferenceExpression)value).Target); PopType();
			}
			_il.Emit(OpCodes.Ldftn, (MethodInfo)mi);
			
			EventInfo ei = ((ExternalEventBinding)eventBinding).EventInfo;
			
			_il.Emit(OpCodes.Newobj, GetDelegateConstructor(ei.EventHandlerType));					
			_il.EmitCall(OpCodes.Callvirt, ei.GetAddMethod(true), null);
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
					AddDelegate(sourceNode, binding, value);
					break;
				}
					
				case BindingType.Field:
				{
					SetField(sourceNode, (IFieldBinding)binding, null, value, false);
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
			/*
			LexicalInfo start = startNode.LexicalInfo;
			LexicalInfo end = endNode.LexicalInfo;
			if (start != LexicalInfo.Empty && end != LexicalInfo.Empty)
			{
				_il.MarkSequencePoint(_symbolDocWriter, start.Line, start.StartColumn, end.Line, end.EndColumn);
			}
			*/
		}
		
		void EmitEnumerableBasedFor(ForStatement node, ITypeBinding iteratorType)
		{			
			Label labelTest = _il.DefineLabel();
			Label labelBody = _il.DefineLabel();
			Label breakLabel = _il.DefineLabel();
			
			LocalBuilder localIterator = _il.DeclareLocal(Types.IEnumerator);
			EmitGetEnumerableIfNeeded(iteratorType);			
			_il.EmitCall(OpCodes.Callvirt, IEnumerable_GetEnumerator, null);
			_il.Emit(OpCodes.Stloc, localIterator);
			_il.Emit(OpCodes.Br, labelTest);
			
			_il.MarkLabel(labelBody);
			_il.Emit(OpCodes.Ldloc, localIterator);
			_il.EmitCall(OpCodes.Callvirt, IEnumerator_get_Current, null);
			EmitUnpackForDeclarations(node.Declarations, BindingManager.ObjectTypeBinding);
			
			EnterLoop(breakLabel, labelTest);
			Switch(node.Block);
			LeaveLoop();
			
			// iterator.MoveNext()			
			_il.MarkLabel(labelTest);
			_il.Emit(OpCodes.Ldloc, localIterator);
			_il.EmitCall(OpCodes.Callvirt, IEnumerator_MoveNext, null);
			_il.Emit(OpCodes.Brtrue, labelBody);
			
			_il.MarkLabel(breakLabel);
		}
		
		void EmitArrayBasedFor(ForStatement node, ITypeBinding iteratorTypeBinding)
		{				
			Label labelTest = _il.DefineLabel();
			Label labelBody = _il.DefineLabel();
			Label continueLabel = _il.DefineLabel();
			Label breakLabel = _il.DefineLabel();
			
			OpCode ldelem = GetLoadElementOpCode(iteratorTypeBinding.GetElementType());
			
			Type iteratorType = GetType(iteratorTypeBinding);
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
			
			EmitUnpackForDeclarations(node.Declarations, iteratorTypeBinding.GetElementType());
			
			EnterLoop(breakLabel, continueLabel);
			Switch(node.Block);
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
		
		void EmitUnpackForDeclarations(DeclarationCollection decls, ITypeBinding topOfStack)
		{
			if (1 == decls.Count)
			{
				// for arg in iterator				
				StoreLocal(topOfStack, GetLocalBinding(decls[0]));
			}
			else
			{
				if (topOfStack.IsArray)
				{						
					ITypeBinding elementTypeBinding = topOfStack.GetElementType();
					
					// RuntimeServices.CheckArrayUnpack(array, decls.Count);					
					_il.Emit(OpCodes.Dup);
					_il.Emit(OpCodes.Ldc_I4, decls.Count);					
					_il.EmitCall(OpCodes.Call, RuntimeServices_CheckArrayUnpack, null);
					
					OpCode ldelem = GetLoadElementOpCode(elementTypeBinding);
					for (int i=0; i<decls.Count; ++i)
					{
						// local = array[i]
						_il.Emit(OpCodes.Dup);
						_il.Emit(OpCodes.Ldc_I4, i); // element index			
						_il.Emit(ldelem);
						
						StoreLocal(elementTypeBinding, GetLocalBinding(decls[i]));					
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
						StoreLocal(BindingManager.ObjectTypeBinding, GetLocalBinding(d));				
					}					
				}
				_il.Emit(OpCodes.Pop);
			}
		}
		
		void EmitGetEnumerableIfNeeded(ITypeBinding topOfStack)
		{
			if (!IsIEnumerableCompatible(topOfStack))
			{
				_il.EmitCall(OpCodes.Call, RuntimeServices_GetEnumerable, null);
			}
		}
		
		bool IsBoolOrInt(ITypeBinding type)
		{
			return BindingManager.BoolTypeBinding == type ||
				BindingManager.IntTypeBinding == type;
		}
		
		bool IsIEnumerableCompatible(ITypeBinding type)
		{
			return BindingManager.IEnumerableTypeBinding.IsAssignableFrom(type);
		}
		
		void PushArguments(IMethodBinding binding, ExpressionCollection args)
		{
			for (int i=0; i<args.Count; ++i)
			{
				Expression arg = args[i];
				arg.Switch(this);
				EmitCastIfNeeded(binding.GetParameterType(i), PopType());
			}
		}
		
		void EmitObjectArray(ExpressionCollection items)
		{
			EmitArray(BindingManager.ObjectTypeBinding, items);
		}
		
		void EmitArray(ITypeBinding type, ExpressionCollection items)
		{
			_il.Emit(OpCodes.Ldc_I4, items.Count);
			_il.Emit(OpCodes.Newarr, GetType(type));
			
			OpCode opcode = GetStoreElementOpCode(type);
			for (int i=0; i<items.Count; ++i)
			{			
				StoreElement(opcode, i, items[i], type);				
			}
		}
		
		bool IsInteger(ITypeBinding type)
		{
			return type == BindingManager.IntTypeBinding ||
				type == BindingManager.LongTypeBinding ||
				type == BindingManager.ByteTypeBinding;
		}
		
		OpCode GetArithmeticOpCode(ITypeBinding type, BinaryOperatorType op)
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
		
		OpCode GetLoadElementOpCode(ITypeBinding binding)
		{
			if (binding.IsValueType)
			{
				if (BindingManager.IntTypeBinding == binding)
				{
					return OpCodes.Ldelem_I4;
				}
				if (BindingManager.LongTypeBinding == binding)
				{
					return OpCodes.Ldelem_I8;
				}
				NotImplemented("LoadElementOpCode(" + binding + ")");
			}
			return OpCodes.Ldelem_Ref;
		}		
		
		OpCode GetStoreElementOpCode(ITypeBinding binding)
		{
			if (binding.IsValueType)
			{
				if (BindingManager.IntTypeBinding == binding)
				{
					return OpCodes.Stelem_I4;
				}
				if (BindingManager.LongTypeBinding == binding)
				{
					return OpCodes.Stelem_I8;
				}
				NotImplemented("GetStoreElementOpCode(" + binding + ")");				
			}
			return OpCodes.Stelem_Ref;
		}
		
		void EmitCastIfNeeded(ITypeBinding expectedType, ITypeBinding actualType)
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
						Type type = GetType(expectedType);
						_il.Emit(OpCodes.Unbox, type);
						_il.Emit(OpCodes.Ldobj, type);
					}
				}
				else
				{
					_context.TraceInfo("castclass: expected type='{0}', type on stack='{1}'", expectedType, actualType);
					_il.Emit(OpCodes.Castclass, GetType(expectedType));
				}
			}
			else
			{
				if (expectedType == BindingManager.ObjectTypeBinding)
				{
					if (actualType.IsValueType)
					{
						_il.Emit(OpCodes.Box, GetType(actualType));
					}
				}
			}
		}
		
		OpCode GetNumericPromotionOpCode(ITypeBinding type)
		{
			if (type == BindingManager.IntTypeBinding)
			{
				return OpCodes.Conv_I4;
			}
			else if (type == BindingManager.LongTypeBinding)
			{
				return OpCodes.Conv_I8;
			}
			else if (type == BindingManager.SingleTypeBinding)
			{
				return OpCodes.Conv_R4;
			}
			else if (type == BindingManager.DoubleTypeBinding)
			{
				return OpCodes.Conv_R8;
			}
			else
			{
				throw new NotImplementedException(string.Format("Numeric promotion for {0} not implemented!", type));				
			}
		}
		
		void StoreLocal(ITypeBinding topOfStack, LocalBinding local)
		{
			EmitCastIfNeeded(local.BoundType, topOfStack);
			_il.Emit(OpCodes.Stloc, local.LocalBuilder);
		}
		
		void StoreElement(OpCode opcode, int index, Node value, ITypeBinding elementType)
		{
			_il.Emit(OpCodes.Dup);	// array reference
			_il.Emit(OpCodes.Ldc_I4, index); // element index
			value.Switch(this); // value
			EmitCastIfNeeded(elementType, PopType());
			_il.Emit(opcode);
		}		
		
		void DefineEntryPoint()
		{
			if (CompilerOutputType.Library != CompilerParameters.OutputType)
			{				
				Method method = AstAnnotations.GetEntryPoint(CompileUnit);
				if (null != method)
				{
					Type type = _asmBuilder.GetType(method.DeclaringType.FullName, true);
					MethodInfo createdMethod = type.GetMethod(method.Name, BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic);
					MethodInfo methodBuilder = GetMethodInfo((IMethodBinding)GetBinding(method));
					
					// the mono implementation expects the first argument to 
					// SetEntryPoint to be a MethodBuilder, otherwise it generates
					// bogus assemblies
					_asmBuilder.SetEntryPoint(methodBuilder, (PEFileKinds)CompilerParameters.OutputType);
					
					// for the rest of the world (like RunAssemblyStep)
					// the created method is the way to go
					AstAnnotations.SetAssemblyEntryPoint(CompileUnit, createdMethod);
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
				types[i] = GetType(parameters[i].Type);
			}
			return types;
		}
		
		static object EmitInfoKey = new object();
		
		void SetBuilder(Node node, object builder)
		{
			if (null == builder)
			{
				throw new ArgumentNullException("type");
			}
			node[EmitInfoKey] = builder;
		}
		
		EnumBuilder GetEnumBuilder(Node node)
		{
			return (EnumBuilder)node[EmitInfoKey];
		}
		
		TypeBuilder GetTypeBuilder(Node node)
		{
			return (TypeBuilder)node[EmitInfoKey];
		}
		
		PropertyBuilder GetPropertyBuilder(Node node)
		{
			return (PropertyBuilder)node[EmitInfoKey];
		}
		
		FieldBuilder GetFieldBuilder(Node node)
		{
			return (FieldBuilder)node[EmitInfoKey];
		}
		
		MethodBuilder GetMethodBuilder(Method method)
		{
			return (MethodBuilder)method[EmitInfoKey];
		}
		
		ConstructorBuilder GetConstructorBuilder(Method method)
		{
			return (ConstructorBuilder)method[EmitInfoKey];
		}
		
		LocalBuilder GetLocalBuilder(Node local)
		{
			return GetLocalBinding(local).LocalBuilder;
		}
		
		PropertyInfo GetPropertyInfo(IBinding binding)
		{
			ExternalPropertyBinding external = binding as ExternalPropertyBinding;
			if (null != external)
			{
				return external.PropertyInfo;
			}
			return GetPropertyBuilder(((InternalPropertyBinding)binding).Property);
		}
		
		FieldInfo GetFieldInfo(IFieldBinding binding)
		{
			ExternalFieldBinding external = binding as ExternalFieldBinding;
			if (null != external)
			{
				return external.FieldInfo;
			}
			return GetFieldBuilder(((InternalFieldBinding)binding).Field);
		}
		
		MethodInfo GetMethodInfo(IMethodBinding binding)
		{
			ExternalMethodBinding external = binding as ExternalMethodBinding;
			if (null != external)
			{
				return (MethodInfo)external.MethodInfo;
			}
			return GetMethodBuilder(((InternalMethodBinding)binding).Method);
		}	
		
		ConstructorInfo GetConstructorInfo(IConstructorBinding binding)
		{
			ExternalConstructorBinding external = binding as ExternalConstructorBinding;
			if (null != external)
			{
				return external.ConstructorInfo;
			}
			return GetConstructorBuilder(((InternalMethodBinding)binding).Method);
		}
		
		ITypeBinding AsTypeBinding(Type type)
		{
			return BindingManager.AsTypeBinding(type);
		}
		
		Type GetType(ITypeBinding binding)
		{
			Type type = (Type)_typeCache[binding];
			if (null == type)
			{
				ExternalTypeBinding external = binding as ExternalTypeBinding;
				if (null != external)
				{
					type = external.Type;
				}
				else
				{
					if (binding.IsArray)
					{												
						ITypeBinding elementType = GetSimpleElementType(binding);						
						if (elementType is IInternalBinding)
						{
							string typeName = GetArrayTypeName(binding);
							type = _moduleBuilder.GetType(typeName, true);
						}
						else
						{
							//type = Type.GetType(typeName, true);
							type = Array.CreateInstance(GetType(binding.GetElementType()), 0).GetType();
						}
					}
					else
					{
						if (NullBinding.Default == binding)
						{
							type = Types.Object;
						}
						else
						{
							type = (Type)((AbstractInternalTypeBinding)binding).TypeDefinition[EmitInfoKey];
						}
					}
				}
				if (null == type)
				{
					throw new InvalidOperationException(string.Format("Could not find a Type for {0}.", binding));
				}
				_typeCache.Add(binding, type);
			}
			return type;
		}
		
		ITypeBinding GetSimpleElementType(ITypeBinding binding)
		{
			if (binding.IsArray)
			{
				return GetSimpleElementType(binding.GetElementType());
			}
			return binding;
		}
		
		string GetArrayTypeName(ITypeBinding binding)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			GetArrayTypeName(builder, binding);
			return builder.ToString();			
		}
		
		void GetArrayTypeName(System.Text.StringBuilder buffer, ITypeBinding binding)
		{
			if (binding.IsArray)
			{
				GetArrayTypeName(buffer, binding.GetElementType());
				buffer.Append("[]");
			}
			else
			{
				buffer.Append(binding.FullName);
			}
		}
		
		Type GetType(Node node)
		{
			return GetType(GetBoundType(node));
		}
		
		TypeAttributes GetNestedTypeAttributes(TypeDefinition type)
		{
			TypeAttributes attributes = type.IsPublic ? TypeAttributes.NestedPublic : TypeAttributes.NestedPrivate;
			return GetExtendedTypeAttributes(attributes, type);
			
		}
		
		TypeAttributes GetTypeAttributes(TypeDefinition type)
		{
			TypeAttributes attributes = type.IsPublic ? TypeAttributes.Public : TypeAttributes.NotPublic;
			return GetExtendedTypeAttributes(attributes, type);
		}
		
		TypeAttributes GetExtendedTypeAttributes(TypeAttributes attributes, TypeDefinition type)
		{
			
			switch (type.NodeType)
			{
				case NodeType.ClassDefinition:
				{
					attributes |= (TypeAttributes.AnsiClass | TypeAttributes.AutoLayout);
					attributes |= TypeAttributes.Class;
					attributes |= TypeAttributes.Serializable;
					if (type.IsModifierSet(TypeMemberModifiers.Abstract))
					{
						attributes |= TypeAttributes.Abstract;
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
			                                            GetType(property.Type),
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
			
			foreach (Boo.Lang.Ast.Attribute attribute in property.Attributes)
			{
				builder.SetCustomAttribute(GetCustomAttributeBuilder(attribute));
			}
			
			SetBuilder(property, builder);
		}
		
		void DefineField(TypeBuilder typeBuilder, Field field)
		{
			FieldBuilder builder = typeBuilder.DefineField(field.Name, 
			                                               GetType(field), 
			                                               GetFieldAttributes(field));
			SetBuilder(field, builder);
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
                                        GetType(method.ReturnType),                             
										GetParameterTypes(parameters));
			for (int i=0; i<parameters.Count; ++i)
			{
				builder.DefineParameter(i+1, ParameterAttributes.None, parameters[i].Name);
			}			
			
			SetBuilder(method, builder);
			foreach (Boo.Lang.Ast.Attribute attribute in method.Attributes)
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
			SetBuilder(constructor, builder);
		}
		
		bool IsEnumDefinition(TypeDefinition type)
		{
			return NodeType.EnumDefinition == type.NodeType;
		}
		
		void DefineType(TypeDefinition typeDefinition)
		{
			if (IsEnumDefinition(typeDefinition))
			{
				EnumBuilder enumBuilder = _moduleBuilder.DefineEnum(typeDefinition.FullName,
											GetTypeAttributes(typeDefinition),
											typeof(int));
											
				
				foreach (EnumMember member in typeDefinition.Members)
				{
					enumBuilder.DefineLiteral(member.Name, (int)member.Initializer.Value);
				}				
				SetBuilder(typeDefinition, enumBuilder);
			}
			else
			{					
				TypeBuilder typeBuilder = null;
				
				ClassDefinition  enclosingType = typeDefinition.ParentNode as ClassDefinition;
				if (null == enclosingType)
				{
					typeBuilder = _moduleBuilder.DefineType(typeDefinition.FullName,
											GetTypeAttributes(typeDefinition));
				}
				else
				{
					typeBuilder = GetTypeBuilder(enclosingType).DefineNestedType(typeDefinition.Name,
																	GetNestedTypeAttributes(typeDefinition));
				}
				SetBuilder(typeDefinition, typeBuilder);
			}
		}
		
		void EmitBaseTypesAndAttributes(TypeDefinition typeDefinition, TypeBuilder typeBuilder)
		{			
			foreach (TypeReference baseType in typeDefinition.BaseTypes)
			{
				Type type = GetType(baseType);
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
			foreach (Boo.Lang.Ast.Attribute attribute in typeDefinition.Attributes)
			{
				typeBuilder.SetCustomAttribute(GetCustomAttributeBuilder(attribute));
			}
		}
		
		void NotImplemented(Node node, string feature)
		{
			throw CompilerErrorFactory.NotImplemented(node, feature);
		}
		
		void NotImplemented(string feature)
		{
			throw new NotImplementedException(feature);
		}
		
		CustomAttributeBuilder GetCustomAttributeBuilder(Boo.Lang.Ast.Attribute node)
		{
			IConstructorBinding constructor = (IConstructorBinding)GetBinding(node);
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
			ArrayList namedProperties = new ArrayList();
			ArrayList propertyValues = new ArrayList();
			ArrayList namedFields = new ArrayList();
			ArrayList fieldValues = new ArrayList();
			foreach (ExpressionPair pair in values)
			{
				IBinding binding = GetBinding(pair.First);
				if (BindingType.Property == binding.BindingType)
				{
					namedProperties.Add(GetPropertyInfo(binding));
					propertyValues.Add(GetValue(pair.Second));
				}
				else
				{
					namedFields.Add(GetFieldInfo((IFieldBinding)binding));
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
				
				default:
				{
					IBinding binding = GetBinding(expression);
					if (BindingType.TypeReference == binding.BindingType)
					{
						return GetType(expression);
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
			CompilerParameters parameters = CompilerParameters;
			string fname = parameters.OutputAssembly;
			if (!Path.HasExtension(fname))
			{
				if (CompilerOutputType.Library == parameters.OutputType)
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
		
		void SetUpAssembly()
		{
			if (0 == CompilerParameters.OutputAssembly.Length)
			{				
				CompilerParameters.OutputAssembly = CompileUnit.Modules[0].Name;			
			}
			
			CompilerParameters.OutputAssembly = BuildOutputAssemblyName();
			
			AssemblyName asmName = new AssemblyName();
			asmName.Name = GetAssemblyName(CompilerParameters.OutputAssembly);
			
			_asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave, GetTargetDirectory(CompilerParameters.OutputAssembly));
			_moduleBuilder = _asmBuilder.DefineDynamicModule(asmName.Name, Path.GetFileName(CompilerParameters.OutputAssembly), true);			
			AstAnnotations.SetAssemblyBuilder(CompileUnit, _asmBuilder);
		}
	}
}
