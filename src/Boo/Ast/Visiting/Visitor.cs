using System;
using Hashtable = System.Collections.Hashtable;

namespace Boo.Ast.Visiting
{
	/// <summary>
	/// Classe base para visitors.
	/// </summary>
	public class AbstractVisitor
	{
		#region despacho dinâmico de nós

		delegate void OnNodeDispatcher(AbstractVisitor visitor, Node node);

		static Hashtable _dispatchTable;

		static void Dispatch(AbstractVisitor visitor, Node node)
		{
			OnNodeDispatcher dispatcher = GetDispatcher(node);
			dispatcher(visitor, node);
		}

		static OnNodeDispatcher GetDispatcher(Node node)
		{
			OnNodeDispatcher dispatcher = (OnNodeDispatcher)_dispatchTable[node.GetType()];
			if (null == dispatcher)
			{
				throw new InvalidOperationException(string.Format("Não existe um dispatcher registrado para nós do tipo {0}!", node.GetType()));
			}
			return dispatcher;
		}
		
		#endregion

		#region Implementação automática
		/// Atenção! Este código foi gerado automaticamente pelo script D:\dev\boo\scripts\generate_visitor_dispatch.py!
		/// Não altere o código nesta região!

		static AbstractVisitor()
		{
			_dispatchTable = new Hashtable();
			_dispatchTable.Add(typeof(Boo.Ast.CompileUnit), new OnNodeDispatcher(AbstractVisitor.OnCompileUnit));
			_dispatchTable.Add(typeof(Boo.Ast.Module), new OnNodeDispatcher(AbstractVisitor.OnModule));
			_dispatchTable.Add(typeof(Boo.Ast.Package), new OnNodeDispatcher(AbstractVisitor.OnPackage));
			_dispatchTable.Add(typeof(Boo.Ast.Using), new OnNodeDispatcher(AbstractVisitor.OnImport));
			_dispatchTable.Add(typeof(Boo.Ast.ClassDefinition), new OnNodeDispatcher(AbstractVisitor.OnClassDefinition));
			_dispatchTable.Add(typeof(Boo.Ast.InterfaceDefinition), new OnNodeDispatcher(AbstractVisitor.OnInterfaceDefinition));
			_dispatchTable.Add(typeof(Boo.Ast.EnumDefinition), new OnNodeDispatcher(AbstractVisitor.OnEnumDefinition));
			_dispatchTable.Add(typeof(Boo.Ast.EnumMember), new OnNodeDispatcher(AbstractVisitor.OnEnumMember));
			_dispatchTable.Add(typeof(Boo.Ast.Constructor), new OnNodeDispatcher(AbstractVisitor.OnConstructor));
			_dispatchTable.Add(typeof(Boo.Ast.Method), new OnNodeDispatcher(AbstractVisitor.OnMethod));
			_dispatchTable.Add(typeof(Boo.Ast.Field), new OnNodeDispatcher(AbstractVisitor.OnField));
			_dispatchTable.Add(typeof(Boo.Ast.Property), new OnNodeDispatcher(AbstractVisitor.OnProperty));
			_dispatchTable.Add(typeof(Boo.Ast.ParameterDeclaration), new OnNodeDispatcher(AbstractVisitor.OnParameterDeclaration));
			_dispatchTable.Add(typeof(Boo.Ast.Attribute), new OnNodeDispatcher(AbstractVisitor.OnAttribute));
			_dispatchTable.Add(typeof(Boo.Ast.Block), new OnNodeDispatcher(AbstractVisitor.OnBlock));
			_dispatchTable.Add(typeof(Boo.Ast.ExpressionStatement), new OnNodeDispatcher(AbstractVisitor.OnExpressionStatement));
			_dispatchTable.Add(typeof(Boo.Ast.StatementModifier), new OnNodeDispatcher(AbstractVisitor.OnStatementModifier));
			_dispatchTable.Add(typeof(Boo.Ast.DeclarationStatement), new OnNodeDispatcher(AbstractVisitor.OnDeclarationStatement));
			_dispatchTable.Add(typeof(Boo.Ast.ReturnStatement), new OnNodeDispatcher(AbstractVisitor.OnReturnStatement));
			_dispatchTable.Add(typeof(Boo.Ast.RaiseStatement), new OnNodeDispatcher(AbstractVisitor.OnRaiseStatement));
			_dispatchTable.Add(typeof(Boo.Ast.YieldStatement), new OnNodeDispatcher(AbstractVisitor.OnYieldStatement));
			_dispatchTable.Add(typeof(Boo.Ast.UnpackStatement), new OnNodeDispatcher(AbstractVisitor.OnUnpackStatement));
			_dispatchTable.Add(typeof(Boo.Ast.SlicingExpression), new OnNodeDispatcher(AbstractVisitor.OnSlicingExpression));
			_dispatchTable.Add(typeof(Boo.Ast.BinaryExpression), new OnNodeDispatcher(AbstractVisitor.OnBinaryExpression));
			_dispatchTable.Add(typeof(Boo.Ast.TernaryExpression), new OnNodeDispatcher(AbstractVisitor.OnTernaryExpression));
			_dispatchTable.Add(typeof(Boo.Ast.ReferenceExpression), new OnNodeDispatcher(AbstractVisitor.OnReferenceExpression));
			_dispatchTable.Add(typeof(Boo.Ast.MemberReferenceExpression), new OnNodeDispatcher(AbstractVisitor.OnMemberReferenceExpression));
			_dispatchTable.Add(typeof(Boo.Ast.MethodInvocationExpression), new OnNodeDispatcher(AbstractVisitor.OnMethodInvocationExpression));
			_dispatchTable.Add(typeof(Boo.Ast.UnaryExpression), new OnNodeDispatcher(AbstractVisitor.OnUnaryExpression));
			_dispatchTable.Add(typeof(Boo.Ast.StringLiteralExpression), new OnNodeDispatcher(AbstractVisitor.OnStringLiteralExpression));
			_dispatchTable.Add(typeof(Boo.Ast.IntegerLiteralExpression), new OnNodeDispatcher(AbstractVisitor.OnIntegerLiteralExpression));
			_dispatchTable.Add(typeof(Boo.Ast.BoolLiteralExpression), new OnNodeDispatcher(AbstractVisitor.OnBoolLiteralExpression));
			_dispatchTable.Add(typeof(Boo.Ast.TimeSpanLiteralExpression), new OnNodeDispatcher(AbstractVisitor.OnTimeSpanLiteralExpression));
			_dispatchTable.Add(typeof(Boo.Ast.ListLiteralExpression), new OnNodeDispatcher(AbstractVisitor.OnListLiteralExpression));
			_dispatchTable.Add(typeof(Boo.Ast.TupleLiteralExpression), new OnNodeDispatcher(AbstractVisitor.OnTupleLiteralExpression));
			_dispatchTable.Add(typeof(Boo.Ast.HashLiteralExpression), new OnNodeDispatcher(AbstractVisitor.OnHashLiteralExpression));
			_dispatchTable.Add(typeof(Boo.Ast.SuperLiteralExpression), new OnNodeDispatcher(AbstractVisitor.OnSuperLiteralExpression));
			_dispatchTable.Add(typeof(Boo.Ast.SelfLiteralExpression), new OnNodeDispatcher(AbstractVisitor.OnSelfLiteralExpression));
			_dispatchTable.Add(typeof(Boo.Ast.StringFormattingExpression), new OnNodeDispatcher(AbstractVisitor.OnStringFormattingExpression));
			_dispatchTable.Add(typeof(Boo.Ast.IfStatement), new OnNodeDispatcher(AbstractVisitor.OnIfStatement));
			_dispatchTable.Add(typeof(Boo.Ast.ForStatement), new OnNodeDispatcher(AbstractVisitor.OnForStatement));
			_dispatchTable.Add(typeof(Boo.Ast.TypeReference), new OnNodeDispatcher(AbstractVisitor.OnTypeReference));
			_dispatchTable.Add(typeof(Boo.Ast.Declaration), new OnNodeDispatcher(AbstractVisitor.OnDeclaration));
			_dispatchTable.Add(typeof(Boo.Ast.ExpressionPair), new OnNodeDispatcher(AbstractVisitor.OnExpressionPair));
		}

		static void OnCompileUnit(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnCompileUnit((Boo.Ast.CompileUnit)node);
		}

		protected virtual void OnCompileUnit(Boo.Ast.CompileUnit node)
		{
			if (EnterCompileUnit(node))
			{
				OnNodeCollection(node.Modules);
				LeaveCompileUnit(node);
			}
		}

		protected virtual bool EnterCompileUnit(Boo.Ast.CompileUnit node)
		{
			return true;
		}

		protected virtual void LeaveCompileUnit(Boo.Ast.CompileUnit node)
		{
		}

		static void OnModule(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnModule((Boo.Ast.Module)node);
		}

		protected virtual void OnModule(Boo.Ast.Module node)
		{
			if (EnterModule(node))
			{
				OnNodeCollection(node.Attributes);
				OnNode(node.Package);
				OnNodeCollection(node.Using);
				OnNodeCollection(node.Members);
				OnNode(node.Globals);
				LeaveModule(node);
			}
		}

		protected virtual bool EnterModule(Boo.Ast.Module node)
		{
			return true;
		}

		protected virtual void LeaveModule(Boo.Ast.Module node)
		{
		}

		static void OnPackage(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnPackage((Boo.Ast.Package)node);
		}

		protected virtual void OnPackage(Boo.Ast.Package node)
		{
		}

		static void OnImport(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnImport((Boo.Ast.Using)node);
		}

		protected virtual void OnImport(Boo.Ast.Using node)
		{
		}

		static void OnClassDefinition(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnClassDefinition((Boo.Ast.ClassDefinition)node);
		}

		protected virtual void OnClassDefinition(Boo.Ast.ClassDefinition node)
		{
			if (EnterClassDefinition(node))
			{
				OnNodeCollection(node.Attributes);
				OnNodeCollection(node.BaseTypes);
				OnNodeCollection(node.Members);
				LeaveClassDefinition(node);
			}
		}

		protected virtual bool EnterClassDefinition(Boo.Ast.ClassDefinition node)
		{
			return true;
		}

		protected virtual void LeaveClassDefinition(Boo.Ast.ClassDefinition node)
		{
		}

		static void OnInterfaceDefinition(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnInterfaceDefinition((Boo.Ast.InterfaceDefinition)node);
		}

		protected virtual void OnInterfaceDefinition(Boo.Ast.InterfaceDefinition node)
		{
			if (EnterInterfaceDefinition(node))
			{
				OnNodeCollection(node.Attributes);
				OnNodeCollection(node.BaseTypes);
				OnNodeCollection(node.Members);
				LeaveInterfaceDefinition(node);
			}
		}

		protected virtual bool EnterInterfaceDefinition(Boo.Ast.InterfaceDefinition node)
		{
			return true;
		}

		protected virtual void LeaveInterfaceDefinition(Boo.Ast.InterfaceDefinition node)
		{
		}

		static void OnEnumDefinition(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnEnumDefinition((Boo.Ast.EnumDefinition)node);
		}

		protected virtual void OnEnumDefinition(Boo.Ast.EnumDefinition node)
		{
			if (EnterEnumDefinition(node))
			{
				OnNodeCollection(node.Attributes);
				OnNodeCollection(node.Members);
				LeaveEnumDefinition(node);
			}
		}

		protected virtual bool EnterEnumDefinition(Boo.Ast.EnumDefinition node)
		{
			return true;
		}

		protected virtual void LeaveEnumDefinition(Boo.Ast.EnumDefinition node)
		{
		}

		static void OnEnumMember(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnEnumMember((Boo.Ast.EnumMember)node);
		}

		protected virtual void OnEnumMember(Boo.Ast.EnumMember node)
		{
			if (EnterEnumMember(node))
			{
				OnNodeCollection(node.Attributes);
				LeaveEnumMember(node);
			}
		}

		protected virtual bool EnterEnumMember(Boo.Ast.EnumMember node)
		{
			return true;
		}

		protected virtual void LeaveEnumMember(Boo.Ast.EnumMember node)
		{
		}

		static void OnConstructor(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnConstructor((Boo.Ast.Constructor)node);
		}

		protected virtual void OnConstructor(Boo.Ast.Constructor node)
		{
			if (EnterConstructor(node))
			{
				OnNodeCollection(node.Attributes);
				OnNodeCollection(node.Parameters);
				LeaveConstructor(node);
			}
		}

		protected virtual bool EnterConstructor(Boo.Ast.Constructor node)
		{
			return true;
		}

		protected virtual void LeaveConstructor(Boo.Ast.Constructor node)
		{
		}

		static void OnMethod(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnMethod((Boo.Ast.Method)node);
		}

		protected virtual void OnMethod(Boo.Ast.Method node)
		{
			if (EnterMethod(node))
			{
				OnNodeCollection(node.Attributes);
				OnNodeCollection(node.Parameters);
				OnNode(node.ReturnType);
				OnNodeCollection(node.ReturnTypeAttributes);
				LeaveMethod(node);
			}
		}

		protected virtual bool EnterMethod(Boo.Ast.Method node)
		{
			return true;
		}

		protected virtual void LeaveMethod(Boo.Ast.Method node)
		{
		}

		static void OnField(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnField((Boo.Ast.Field)node);
		}

		protected virtual void OnField(Boo.Ast.Field node)
		{
			if (EnterField(node))
			{
				OnNodeCollection(node.Attributes);
				OnNode(node.Type);
				LeaveField(node);
			}
		}

		protected virtual bool EnterField(Boo.Ast.Field node)
		{
			return true;
		}

		protected virtual void LeaveField(Boo.Ast.Field node)
		{
		}

		static void OnProperty(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnProperty((Boo.Ast.Property)node);
		}

		protected virtual void OnProperty(Boo.Ast.Property node)
		{
			if (EnterProperty(node))
			{
				OnNodeCollection(node.Attributes);
				OnNode(node.Getter);
				OnNode(node.Setter);
				OnNode(node.Type);
				LeaveProperty(node);
			}
		}

		protected virtual bool EnterProperty(Boo.Ast.Property node)
		{
			return true;
		}

		protected virtual void LeaveProperty(Boo.Ast.Property node)
		{
		}

		static void OnParameterDeclaration(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnParameterDeclaration((Boo.Ast.ParameterDeclaration)node);
		}

		protected virtual void OnParameterDeclaration(Boo.Ast.ParameterDeclaration node)
		{
			if (EnterParameterDeclaration(node))
			{
				OnNodeCollection(node.Attributes);
				OnNode(node.Type);
				LeaveParameterDeclaration(node);
			}
		}

		protected virtual bool EnterParameterDeclaration(Boo.Ast.ParameterDeclaration node)
		{
			return true;
		}

		protected virtual void LeaveParameterDeclaration(Boo.Ast.ParameterDeclaration node)
		{
		}

		static void OnAttribute(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnAttribute((Boo.Ast.Attribute)node);
		}

		protected virtual void OnAttribute(Boo.Ast.Attribute node)
		{
		}

		static void OnBlock(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnBlock((Boo.Ast.Block)node);
		}

		protected virtual void OnBlock(Boo.Ast.Block node)
		{
			if (EnterBlock(node))
			{
				OnNodeCollection(node.Statements);
				LeaveBlock(node);
			}
		}

		protected virtual bool EnterBlock(Boo.Ast.Block node)
		{
			return true;
		}

		protected virtual void LeaveBlock(Boo.Ast.Block node)
		{
		}

		static void OnExpressionStatement(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnExpressionStatement((Boo.Ast.ExpressionStatement)node);
		}

		protected virtual void OnExpressionStatement(Boo.Ast.ExpressionStatement node)
		{
			if (EnterExpressionStatement(node))
			{
				OnNode(node.Expression);
				OnNode(node.Modifier);
				LeaveExpressionStatement(node);
			}
		}

		protected virtual bool EnterExpressionStatement(Boo.Ast.ExpressionStatement node)
		{
			return true;
		}

		protected virtual void LeaveExpressionStatement(Boo.Ast.ExpressionStatement node)
		{
		}

		static void OnStatementModifier(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnStatementModifier((Boo.Ast.StatementModifier)node);
		}

		protected virtual void OnStatementModifier(Boo.Ast.StatementModifier node)
		{
			if (EnterStatementModifier(node))
			{
				OnNode(node.Condition);
				LeaveStatementModifier(node);
			}
		}

		protected virtual bool EnterStatementModifier(Boo.Ast.StatementModifier node)
		{
			return true;
		}

		protected virtual void LeaveStatementModifier(Boo.Ast.StatementModifier node)
		{
		}

		static void OnDeclarationStatement(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnDeclarationStatement((Boo.Ast.DeclarationStatement)node);
		}

		protected virtual void OnDeclarationStatement(Boo.Ast.DeclarationStatement node)
		{
			if (EnterDeclarationStatement(node))
			{
				OnNode(node.Declaration);
				OnNode(node.Initializer);
				LeaveDeclarationStatement(node);
			}
		}

		protected virtual bool EnterDeclarationStatement(Boo.Ast.DeclarationStatement node)
		{
			return true;
		}

		protected virtual void LeaveDeclarationStatement(Boo.Ast.DeclarationStatement node)
		{
		}

		static void OnReturnStatement(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnReturnStatement((Boo.Ast.ReturnStatement)node);
		}

		protected virtual void OnReturnStatement(Boo.Ast.ReturnStatement node)
		{
			if (EnterReturnStatement(node))
			{
				OnNode(node.Expression);
				OnNode(node.Modifier);
				LeaveReturnStatement(node);
			}
		}

		protected virtual bool EnterReturnStatement(Boo.Ast.ReturnStatement node)
		{
			return true;
		}

		protected virtual void LeaveReturnStatement(Boo.Ast.ReturnStatement node)
		{
		}

		static void OnRaiseStatement(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnRaiseStatement((Boo.Ast.RaiseStatement)node);
		}

		protected virtual void OnRaiseStatement(Boo.Ast.RaiseStatement node)
		{
			if (EnterRaiseStatement(node))
			{
				OnNode(node.Exception);
				OnNode(node.Modifier);
				LeaveRaiseStatement(node);
			}
		}

		protected virtual bool EnterRaiseStatement(Boo.Ast.RaiseStatement node)
		{
			return true;
		}

		protected virtual void LeaveRaiseStatement(Boo.Ast.RaiseStatement node)
		{
		}

		static void OnYieldStatement(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnYieldStatement((Boo.Ast.YieldStatement)node);
		}

		protected virtual void OnYieldStatement(Boo.Ast.YieldStatement node)
		{
			if (EnterYieldStatement(node))
			{
				OnNode(node.Expression);
				OnNode(node.Modifier);
				LeaveYieldStatement(node);
			}
		}

		protected virtual bool EnterYieldStatement(Boo.Ast.YieldStatement node)
		{
			return true;
		}

		protected virtual void LeaveYieldStatement(Boo.Ast.YieldStatement node)
		{
		}

		static void OnUnpackStatement(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnUnpackStatement((Boo.Ast.UnpackStatement)node);
		}

		protected virtual void OnUnpackStatement(Boo.Ast.UnpackStatement node)
		{
			if (EnterUnpackStatement(node))
			{
				OnNodeCollection(node.Declarations);
				OnNode(node.Expression);
				OnNode(node.Modifier);
				LeaveUnpackStatement(node);
			}
		}

		protected virtual bool EnterUnpackStatement(Boo.Ast.UnpackStatement node)
		{
			return true;
		}

		protected virtual void LeaveUnpackStatement(Boo.Ast.UnpackStatement node)
		{
		}

		static void OnSlicingExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnSlicingExpression((Boo.Ast.SlicingExpression)node);
		}

		protected virtual void OnSlicingExpression(Boo.Ast.SlicingExpression node)
		{
			if (EnterSlicingExpression(node))
			{
				OnNode(node.Target);
				OnNode(node.Begin);
				OnNode(node.End);
				OnNode(node.Step);
				LeaveSlicingExpression(node);
			}
		}

		protected virtual bool EnterSlicingExpression(Boo.Ast.SlicingExpression node)
		{
			return true;
		}

		protected virtual void LeaveSlicingExpression(Boo.Ast.SlicingExpression node)
		{
		}

		static void OnBinaryExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnBinaryExpression((Boo.Ast.BinaryExpression)node);
		}

		protected virtual void OnBinaryExpression(Boo.Ast.BinaryExpression node)
		{
			if (EnterBinaryExpression(node))
			{
				OnNode(node.Left);
				OnNode(node.Right);
				LeaveBinaryExpression(node);
			}
		}

		protected virtual bool EnterBinaryExpression(Boo.Ast.BinaryExpression node)
		{
			return true;
		}

		protected virtual void LeaveBinaryExpression(Boo.Ast.BinaryExpression node)
		{
		}

		static void OnTernaryExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnTernaryExpression((Boo.Ast.TernaryExpression)node);
		}

		protected virtual void OnTernaryExpression(Boo.Ast.TernaryExpression node)
		{
			if (EnterTernaryExpression(node))
			{
				OnNode(node.Condition);
				OnNode(node.TrueExpression);
				OnNode(node.FalseExpression);
				LeaveTernaryExpression(node);
			}
		}

		protected virtual bool EnterTernaryExpression(Boo.Ast.TernaryExpression node)
		{
			return true;
		}

		protected virtual void LeaveTernaryExpression(Boo.Ast.TernaryExpression node)
		{
		}

		static void OnReferenceExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnReferenceExpression((Boo.Ast.ReferenceExpression)node);
		}

		protected virtual void OnReferenceExpression(Boo.Ast.ReferenceExpression node)
		{
		}

		static void OnMemberReferenceExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnMemberReferenceExpression((Boo.Ast.MemberReferenceExpression)node);
		}

		protected virtual void OnMemberReferenceExpression(Boo.Ast.MemberReferenceExpression node)
		{
			if (EnterMemberReferenceExpression(node))
			{
				OnNode(node.Target);
				LeaveMemberReferenceExpression(node);
			}
		}

		protected virtual bool EnterMemberReferenceExpression(Boo.Ast.MemberReferenceExpression node)
		{
			return true;
		}

		protected virtual void LeaveMemberReferenceExpression(Boo.Ast.MemberReferenceExpression node)
		{
		}

		static void OnMethodInvocationExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnMethodInvocationExpression((Boo.Ast.MethodInvocationExpression)node);
		}

		protected virtual void OnMethodInvocationExpression(Boo.Ast.MethodInvocationExpression node)
		{
			if (EnterMethodInvocationExpression(node))
			{
				OnNode(node.Target);
				OnNodeCollection(node.Arguments);
				OnNodeCollection(node.NamedArguments);
				LeaveMethodInvocationExpression(node);
			}
		}

		protected virtual bool EnterMethodInvocationExpression(Boo.Ast.MethodInvocationExpression node)
		{
			return true;
		}

		protected virtual void LeaveMethodInvocationExpression(Boo.Ast.MethodInvocationExpression node)
		{
		}

		static void OnUnaryExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnUnaryExpression((Boo.Ast.UnaryExpression)node);
		}

		protected virtual void OnUnaryExpression(Boo.Ast.UnaryExpression node)
		{
			if (EnterUnaryExpression(node))
			{
				OnNode(node.Operand);
				LeaveUnaryExpression(node);
			}
		}

		protected virtual bool EnterUnaryExpression(Boo.Ast.UnaryExpression node)
		{
			return true;
		}

		protected virtual void LeaveUnaryExpression(Boo.Ast.UnaryExpression node)
		{
		}

		static void OnStringLiteralExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnStringLiteralExpression((Boo.Ast.StringLiteralExpression)node);
		}

		protected virtual void OnStringLiteralExpression(Boo.Ast.StringLiteralExpression node)
		{
		}

		static void OnIntegerLiteralExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnIntegerLiteralExpression((Boo.Ast.IntegerLiteralExpression)node);
		}

		protected virtual void OnIntegerLiteralExpression(Boo.Ast.IntegerLiteralExpression node)
		{
		}

		static void OnBoolLiteralExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnBoolLiteralExpression((Boo.Ast.BoolLiteralExpression)node);
		}

		protected virtual void OnBoolLiteralExpression(Boo.Ast.BoolLiteralExpression node)
		{
		}

		static void OnTimeSpanLiteralExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnTimeSpanLiteralExpression((Boo.Ast.TimeSpanLiteralExpression)node);
		}

		protected virtual void OnTimeSpanLiteralExpression(Boo.Ast.TimeSpanLiteralExpression node)
		{
		}

		static void OnListLiteralExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnListLiteralExpression((Boo.Ast.ListLiteralExpression)node);
		}

		protected virtual void OnListLiteralExpression(Boo.Ast.ListLiteralExpression node)
		{
			if (EnterListLiteralExpression(node))
			{
				OnNodeCollection(node.Items);
				LeaveListLiteralExpression(node);
			}
		}

		protected virtual bool EnterListLiteralExpression(Boo.Ast.ListLiteralExpression node)
		{
			return true;
		}

		protected virtual void LeaveListLiteralExpression(Boo.Ast.ListLiteralExpression node)
		{
		}

		static void OnTupleLiteralExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnTupleLiteralExpression((Boo.Ast.TupleLiteralExpression)node);
		}

		protected virtual void OnTupleLiteralExpression(Boo.Ast.TupleLiteralExpression node)
		{
			if (EnterTupleLiteralExpression(node))
			{
				OnNodeCollection(node.Items);
				LeaveTupleLiteralExpression(node);
			}
		}

		protected virtual bool EnterTupleLiteralExpression(Boo.Ast.TupleLiteralExpression node)
		{
			return true;
		}

		protected virtual void LeaveTupleLiteralExpression(Boo.Ast.TupleLiteralExpression node)
		{
		}

		static void OnHashLiteralExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnHashLiteralExpression((Boo.Ast.HashLiteralExpression)node);
		}

		protected virtual void OnHashLiteralExpression(Boo.Ast.HashLiteralExpression node)
		{
			if (EnterHashLiteralExpression(node))
			{
				OnNodeCollection(node.Items);
				LeaveHashLiteralExpression(node);
			}
		}

		protected virtual bool EnterHashLiteralExpression(Boo.Ast.HashLiteralExpression node)
		{
			return true;
		}

		protected virtual void LeaveHashLiteralExpression(Boo.Ast.HashLiteralExpression node)
		{
		}

		static void OnSuperLiteralExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnSuperLiteralExpression((Boo.Ast.SuperLiteralExpression)node);
		}

		protected virtual void OnSuperLiteralExpression(Boo.Ast.SuperLiteralExpression node)
		{
		}

		static void OnSelfLiteralExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnSelfLiteralExpression((Boo.Ast.SelfLiteralExpression)node);
		}

		protected virtual void OnSelfLiteralExpression(Boo.Ast.SelfLiteralExpression node)
		{
		}

		static void OnStringFormattingExpression(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnStringFormattingExpression((Boo.Ast.StringFormattingExpression)node);
		}

		protected virtual void OnStringFormattingExpression(Boo.Ast.StringFormattingExpression node)
		{
			if (EnterStringFormattingExpression(node))
			{
				OnNodeCollection(node.Arguments);
				LeaveStringFormattingExpression(node);
			}
		}

		protected virtual bool EnterStringFormattingExpression(Boo.Ast.StringFormattingExpression node)
		{
			return true;
		}

		protected virtual void LeaveStringFormattingExpression(Boo.Ast.StringFormattingExpression node)
		{
		}

		static void OnIfStatement(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnIfStatement((Boo.Ast.IfStatement)node);
		}

		protected virtual void OnIfStatement(Boo.Ast.IfStatement node)
		{
			if (EnterIfStatement(node))
			{
				OnNode(node.Expression);
				OnNode(node.TrueBlock);
				OnNode(node.FalseBlock);
				LeaveIfStatement(node);
			}
		}

		protected virtual bool EnterIfStatement(Boo.Ast.IfStatement node)
		{
			return true;
		}

		protected virtual void LeaveIfStatement(Boo.Ast.IfStatement node)
		{
		}

		static void OnForStatement(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnForStatement((Boo.Ast.ForStatement)node);
		}

		protected virtual void OnForStatement(Boo.Ast.ForStatement node)
		{
			if (EnterForStatement(node))
			{
				OnNodeCollection(node.Declarations);
				OnNode(node.Iterator);
				OnNodeCollection(node.Statements);
				LeaveForStatement(node);
			}
		}

		protected virtual bool EnterForStatement(Boo.Ast.ForStatement node)
		{
			return true;
		}

		protected virtual void LeaveForStatement(Boo.Ast.ForStatement node)
		{
		}

		static void OnTypeReference(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnTypeReference((Boo.Ast.TypeReference)node);
		}

		protected virtual void OnTypeReference(Boo.Ast.TypeReference node)
		{
		}

		static void OnDeclaration(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnDeclaration((Boo.Ast.Declaration)node);
		}

		protected virtual void OnDeclaration(Boo.Ast.Declaration node)
		{
			if (EnterDeclaration(node))
			{
				OnNode(node.Type);
				LeaveDeclaration(node);
			}
		}

		protected virtual bool EnterDeclaration(Boo.Ast.Declaration node)
		{
			return true;
		}

		protected virtual void LeaveDeclaration(Boo.Ast.Declaration node)
		{
		}

		static void OnExpressionPair(AbstractVisitor visitor, Boo.Ast.Node node)
		{
			visitor.OnExpressionPair((Boo.Ast.ExpressionPair)node);
		}

		protected virtual void OnExpressionPair(Boo.Ast.ExpressionPair node)
		{
			if (EnterExpressionPair(node))
			{
				OnNode(node.First);
				OnNode(node.Second);
				LeaveExpressionPair(node);
			}
		}

		protected virtual bool EnterExpressionPair(Boo.Ast.ExpressionPair node)
		{
			return true;
		}

		protected virtual void LeaveExpressionPair(Boo.Ast.ExpressionPair node)
		{
		}
		#endregion

		#region Implementação late bound
		protected virtual void OnNode(Node node)
		{
			if (null != node)
			{
				Dispatch(this, node);
			}
		}

		protected virtual void OnNodeCollection(NodeCollection nodes)
		{
			if (null != nodes)
			{
				foreach (Node node in nodes)
				{
					Dispatch(this, node);
				}
			}
		}
		#endregion
	}
}
