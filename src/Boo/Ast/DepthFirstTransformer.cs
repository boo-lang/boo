using System;

namespace Boo.Ast
{
	public class DepthFirstTransformer : IAstTransformer
	{
		public bool Switch(Node node, out Node resultingNode)
		{
			resultingNode = node;
			if (null != node)
			{			
				node.Switch(this, out resultingNode);
				return true;
			}
			return false;
		}
		
		public bool Switch(NodeCollection collection)
		{
			if (null != collection)
			{
				int removed = 0;
				
				Node[] nodes = collection.ToArray();
				for (int i=0; i<nodes.Length; ++i)
				{
					Node resultingNode;
					Node currentNode = nodes[i];
					currentNode.Switch(this, out resultingNode);
					if (currentNode != resultingNode)
					{
						int actualIndex = i-removed;
						if (null == resultingNode)
						{
							collection.RemoveAt(actualIndex);
						}
						else
						{
							collection.ReplaceAt(actualIndex, resultingNode);
						}
					}
				}
				return true;
			}
			return false;
		}
		
		public virtual void OnCompileUnit(CompileUnit node, out CompileUnit resultingNode)
		{				
			CompileUnit result = node;
			
			if (EnterCompileUnit(node, ref result))
			{		
				Switch(node.Modules);
			}
			LeaveCompileUnit(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterCompileUnit(CompileUnit node, ref CompileUnit resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveCompileUnit(CompileUnit node, ref CompileUnit resultingNode)
		{
		}
		
		public virtual void OnTypeReference(TypeReference node, out TypeReference resultingNode)
		{				
			TypeReference result = node;
			
				
			resultingNode = result;
		}
		
		public virtual void OnPackage(Package node, out Package resultingNode)
		{				
			Package result = node;
			
				
			resultingNode = result;
		}
		
		public virtual void OnUsing(Using node, out Using resultingNode)
		{				
			Using result = node;
			
			if (EnterUsing(node, ref result))
			{		
				ReferenceExpression currentAssemblyReferenceValue = node.AssemblyReference;
				Node resultingAssemblyReferenceValue;
				Switch(currentAssemblyReferenceValue, out resultingAssemblyReferenceValue);
				if (currentAssemblyReferenceValue != resultingAssemblyReferenceValue)
				{
					node.AssemblyReference = (ReferenceExpression)resultingAssemblyReferenceValue;
				}
				ReferenceExpression currentAliasValue = node.Alias;
				Node resultingAliasValue;
				Switch(currentAliasValue, out resultingAliasValue);
				if (currentAliasValue != resultingAliasValue)
				{
					node.Alias = (ReferenceExpression)resultingAliasValue;
				}
			}
			LeaveUsing(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterUsing(Using node, ref Using resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveUsing(Using node, ref Using resultingNode)
		{
		}
		
		public virtual void OnModule(Module node, out Module resultingNode)
		{				
			Module result = node;
			
			if (EnterModule(node, ref result))
			{		
				Package currentPackageValue = node.Package;
				Node resultingPackageValue;
				Switch(currentPackageValue, out resultingPackageValue);
				if (currentPackageValue != resultingPackageValue)
				{
					node.Package = (Package)resultingPackageValue;
				}
				Switch(node.Using);
				Block currentGlobalsValue = node.Globals;
				Node resultingGlobalsValue;
				Switch(currentGlobalsValue, out resultingGlobalsValue);
				if (currentGlobalsValue != resultingGlobalsValue)
				{
					node.Globals = (Block)resultingGlobalsValue;
				}
				Switch(node.Attributes);
				Switch(node.Members);
				Switch(node.BaseTypes);
			}
			LeaveModule(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterModule(Module node, ref Module resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveModule(Module node, ref Module resultingNode)
		{
		}
		
		public virtual void OnClassDefinition(ClassDefinition node, out ClassDefinition resultingNode)
		{				
			ClassDefinition result = node;
			
			if (EnterClassDefinition(node, ref result))
			{		
				Switch(node.Attributes);
				Switch(node.Members);
				Switch(node.BaseTypes);
			}
			LeaveClassDefinition(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterClassDefinition(ClassDefinition node, ref ClassDefinition resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveClassDefinition(ClassDefinition node, ref ClassDefinition resultingNode)
		{
		}
		
		public virtual void OnInterfaceDefinition(InterfaceDefinition node, out InterfaceDefinition resultingNode)
		{				
			InterfaceDefinition result = node;
			
			if (EnterInterfaceDefinition(node, ref result))
			{		
				Switch(node.Attributes);
				Switch(node.Members);
				Switch(node.BaseTypes);
			}
			LeaveInterfaceDefinition(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterInterfaceDefinition(InterfaceDefinition node, ref InterfaceDefinition resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveInterfaceDefinition(InterfaceDefinition node, ref InterfaceDefinition resultingNode)
		{
		}
		
		public virtual void OnEnumDefinition(EnumDefinition node, out EnumDefinition resultingNode)
		{				
			EnumDefinition result = node;
			
			if (EnterEnumDefinition(node, ref result))
			{		
				Switch(node.Attributes);
				Switch(node.Members);
				Switch(node.BaseTypes);
			}
			LeaveEnumDefinition(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterEnumDefinition(EnumDefinition node, ref EnumDefinition resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveEnumDefinition(EnumDefinition node, ref EnumDefinition resultingNode)
		{
		}
		
		public virtual void OnEnumMember(EnumMember node, out EnumMember resultingNode)
		{				
			EnumMember result = node;
			
			if (EnterEnumMember(node, ref result))
			{		
				IntegerLiteralExpression currentInitializerValue = node.Initializer;
				Node resultingInitializerValue;
				Switch(currentInitializerValue, out resultingInitializerValue);
				if (currentInitializerValue != resultingInitializerValue)
				{
					node.Initializer = (IntegerLiteralExpression)resultingInitializerValue;
				}
				Switch(node.Attributes);
			}
			LeaveEnumMember(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterEnumMember(EnumMember node, ref EnumMember resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveEnumMember(EnumMember node, ref EnumMember resultingNode)
		{
		}
		
		public virtual void OnField(Field node, out Field resultingNode)
		{				
			Field result = node;
			
			if (EnterField(node, ref result))
			{		
				TypeReference currentTypeValue = node.Type;
				Node resultingTypeValue;
				Switch(currentTypeValue, out resultingTypeValue);
				if (currentTypeValue != resultingTypeValue)
				{
					node.Type = (TypeReference)resultingTypeValue;
				}
				Switch(node.Attributes);
			}
			LeaveField(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterField(Field node, ref Field resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveField(Field node, ref Field resultingNode)
		{
		}
		
		public virtual void OnProperty(Property node, out Property resultingNode)
		{				
			Property result = node;
			
			if (EnterProperty(node, ref result))
			{		
				Method currentGetterValue = node.Getter;
				Node resultingGetterValue;
				Switch(currentGetterValue, out resultingGetterValue);
				if (currentGetterValue != resultingGetterValue)
				{
					node.Getter = (Method)resultingGetterValue;
				}
				Method currentSetterValue = node.Setter;
				Node resultingSetterValue;
				Switch(currentSetterValue, out resultingSetterValue);
				if (currentSetterValue != resultingSetterValue)
				{
					node.Setter = (Method)resultingSetterValue;
				}
				TypeReference currentTypeValue = node.Type;
				Node resultingTypeValue;
				Switch(currentTypeValue, out resultingTypeValue);
				if (currentTypeValue != resultingTypeValue)
				{
					node.Type = (TypeReference)resultingTypeValue;
				}
				Switch(node.Attributes);
			}
			LeaveProperty(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterProperty(Property node, ref Property resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveProperty(Property node, ref Property resultingNode)
		{
		}
		
		public virtual void OnLocal(Local node, out Local resultingNode)
		{				
			Local result = node;
			
				
			resultingNode = result;
		}
		
		public virtual void OnMethod(Method node, out Method resultingNode)
		{				
			Method result = node;
			
			if (EnterMethod(node, ref result))
			{		
				Switch(node.Parameters);
				TypeReference currentReturnTypeValue = node.ReturnType;
				Node resultingReturnTypeValue;
				Switch(currentReturnTypeValue, out resultingReturnTypeValue);
				if (currentReturnTypeValue != resultingReturnTypeValue)
				{
					node.ReturnType = (TypeReference)resultingReturnTypeValue;
				}
				Switch(node.ReturnTypeAttributes);
				Block currentBodyValue = node.Body;
				Node resultingBodyValue;
				Switch(currentBodyValue, out resultingBodyValue);
				if (currentBodyValue != resultingBodyValue)
				{
					node.Body = (Block)resultingBodyValue;
				}
				Switch(node.Locals);
				Switch(node.Attributes);
			}
			LeaveMethod(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterMethod(Method node, ref Method resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveMethod(Method node, ref Method resultingNode)
		{
		}
		
		public virtual void OnConstructor(Constructor node, out Constructor resultingNode)
		{				
			Constructor result = node;
			
			if (EnterConstructor(node, ref result))
			{		
				Switch(node.Attributes);
				Switch(node.Parameters);
				TypeReference currentReturnTypeValue = node.ReturnType;
				Node resultingReturnTypeValue;
				Switch(currentReturnTypeValue, out resultingReturnTypeValue);
				if (currentReturnTypeValue != resultingReturnTypeValue)
				{
					node.ReturnType = (TypeReference)resultingReturnTypeValue;
				}
				Switch(node.ReturnTypeAttributes);
				Block currentBodyValue = node.Body;
				Node resultingBodyValue;
				Switch(currentBodyValue, out resultingBodyValue);
				if (currentBodyValue != resultingBodyValue)
				{
					node.Body = (Block)resultingBodyValue;
				}
				Switch(node.Locals);
			}
			LeaveConstructor(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterConstructor(Constructor node, ref Constructor resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveConstructor(Constructor node, ref Constructor resultingNode)
		{
		}
		
		public virtual void OnParameterDeclaration(ParameterDeclaration node, out ParameterDeclaration resultingNode)
		{				
			ParameterDeclaration result = node;
			
			if (EnterParameterDeclaration(node, ref result))
			{		
				TypeReference currentTypeValue = node.Type;
				Node resultingTypeValue;
				Switch(currentTypeValue, out resultingTypeValue);
				if (currentTypeValue != resultingTypeValue)
				{
					node.Type = (TypeReference)resultingTypeValue;
				}
				Switch(node.Attributes);
			}
			LeaveParameterDeclaration(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterParameterDeclaration(ParameterDeclaration node, ref ParameterDeclaration resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveParameterDeclaration(ParameterDeclaration node, ref ParameterDeclaration resultingNode)
		{
		}
		
		public virtual void OnDeclaration(Declaration node, out Declaration resultingNode)
		{				
			Declaration result = node;
			
			if (EnterDeclaration(node, ref result))
			{		
				TypeReference currentTypeValue = node.Type;
				Node resultingTypeValue;
				Switch(currentTypeValue, out resultingTypeValue);
				if (currentTypeValue != resultingTypeValue)
				{
					node.Type = (TypeReference)resultingTypeValue;
				}
			}
			LeaveDeclaration(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterDeclaration(Declaration node, ref Declaration resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveDeclaration(Declaration node, ref Declaration resultingNode)
		{
		}
		
		public virtual void OnBlock(Block node, out Block resultingNode)
		{				
			Block result = node;
			
			if (EnterBlock(node, ref result))
			{		
				Switch(node.Statements);
			}
			LeaveBlock(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterBlock(Block node, ref Block resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveBlock(Block node, ref Block resultingNode)
		{
		}
		
		public virtual void OnAttribute(Attribute node, out Attribute resultingNode)
		{				
			Attribute result = node;
			
			if (EnterAttribute(node, ref result))
			{		
				Switch(node.Arguments);
				Switch(node.NamedArguments);
			}
			LeaveAttribute(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterAttribute(Attribute node, ref Attribute resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveAttribute(Attribute node, ref Attribute resultingNode)
		{
		}
		
		public virtual void OnStatementModifier(StatementModifier node, out StatementModifier resultingNode)
		{				
			StatementModifier result = node;
			
			if (EnterStatementModifier(node, ref result))
			{		
				Expression currentConditionValue = node.Condition;
				Node resultingConditionValue;
				Switch(currentConditionValue, out resultingConditionValue);
				if (currentConditionValue != resultingConditionValue)
				{
					node.Condition = (Expression)resultingConditionValue;
				}
			}
			LeaveStatementModifier(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterStatementModifier(StatementModifier node, ref StatementModifier resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveStatementModifier(StatementModifier node, ref StatementModifier resultingNode)
		{
		}
		
		public virtual void OnDeclarationStatement(DeclarationStatement node, out Statement resultingNode)
		{				
			Statement result = node;
			
			if (EnterDeclarationStatement(node, ref result))
			{		
				Declaration currentDeclarationValue = node.Declaration;
				Node resultingDeclarationValue;
				Switch(currentDeclarationValue, out resultingDeclarationValue);
				if (currentDeclarationValue != resultingDeclarationValue)
				{
					node.Declaration = (Declaration)resultingDeclarationValue;
				}
				Expression currentInitializerValue = node.Initializer;
				Node resultingInitializerValue;
				Switch(currentInitializerValue, out resultingInitializerValue);
				if (currentInitializerValue != resultingInitializerValue)
				{
					node.Initializer = (Expression)resultingInitializerValue;
				}
				StatementModifier currentModifierValue = node.Modifier;
				Node resultingModifierValue;
				Switch(currentModifierValue, out resultingModifierValue);
				if (currentModifierValue != resultingModifierValue)
				{
					node.Modifier = (StatementModifier)resultingModifierValue;
				}
			}
			LeaveDeclarationStatement(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterDeclarationStatement(DeclarationStatement node, ref Statement resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveDeclarationStatement(DeclarationStatement node, ref Statement resultingNode)
		{
		}
		
		public virtual void OnAssertStatement(AssertStatement node, out Statement resultingNode)
		{				
			Statement result = node;
			
			if (EnterAssertStatement(node, ref result))
			{		
				Expression currentConditionValue = node.Condition;
				Node resultingConditionValue;
				Switch(currentConditionValue, out resultingConditionValue);
				if (currentConditionValue != resultingConditionValue)
				{
					node.Condition = (Expression)resultingConditionValue;
				}
				Expression currentMessageValue = node.Message;
				Node resultingMessageValue;
				Switch(currentMessageValue, out resultingMessageValue);
				if (currentMessageValue != resultingMessageValue)
				{
					node.Message = (Expression)resultingMessageValue;
				}
				StatementModifier currentModifierValue = node.Modifier;
				Node resultingModifierValue;
				Switch(currentModifierValue, out resultingModifierValue);
				if (currentModifierValue != resultingModifierValue)
				{
					node.Modifier = (StatementModifier)resultingModifierValue;
				}
			}
			LeaveAssertStatement(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterAssertStatement(AssertStatement node, ref Statement resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveAssertStatement(AssertStatement node, ref Statement resultingNode)
		{
		}
		
		public virtual void OnTryStatement(TryStatement node, out Statement resultingNode)
		{				
			Statement result = node;
			
			if (EnterTryStatement(node, ref result))
			{		
				Block currentProtectedBlockValue = node.ProtectedBlock;
				Node resultingProtectedBlockValue;
				Switch(currentProtectedBlockValue, out resultingProtectedBlockValue);
				if (currentProtectedBlockValue != resultingProtectedBlockValue)
				{
					node.ProtectedBlock = (Block)resultingProtectedBlockValue;
				}
				Switch(node.ExceptionHandlers);
				Block currentSuccessBlockValue = node.SuccessBlock;
				Node resultingSuccessBlockValue;
				Switch(currentSuccessBlockValue, out resultingSuccessBlockValue);
				if (currentSuccessBlockValue != resultingSuccessBlockValue)
				{
					node.SuccessBlock = (Block)resultingSuccessBlockValue;
				}
				Block currentEnsureBlockValue = node.EnsureBlock;
				Node resultingEnsureBlockValue;
				Switch(currentEnsureBlockValue, out resultingEnsureBlockValue);
				if (currentEnsureBlockValue != resultingEnsureBlockValue)
				{
					node.EnsureBlock = (Block)resultingEnsureBlockValue;
				}
				StatementModifier currentModifierValue = node.Modifier;
				Node resultingModifierValue;
				Switch(currentModifierValue, out resultingModifierValue);
				if (currentModifierValue != resultingModifierValue)
				{
					node.Modifier = (StatementModifier)resultingModifierValue;
				}
			}
			LeaveTryStatement(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterTryStatement(TryStatement node, ref Statement resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveTryStatement(TryStatement node, ref Statement resultingNode)
		{
		}
		
		public virtual void OnExceptionHandler(ExceptionHandler node, out ExceptionHandler resultingNode)
		{				
			ExceptionHandler result = node;
			
			if (EnterExceptionHandler(node, ref result))
			{		
				Declaration currentDeclarationValue = node.Declaration;
				Node resultingDeclarationValue;
				Switch(currentDeclarationValue, out resultingDeclarationValue);
				if (currentDeclarationValue != resultingDeclarationValue)
				{
					node.Declaration = (Declaration)resultingDeclarationValue;
				}
				Switch(node.Statements);
			}
			LeaveExceptionHandler(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterExceptionHandler(ExceptionHandler node, ref ExceptionHandler resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveExceptionHandler(ExceptionHandler node, ref ExceptionHandler resultingNode)
		{
		}
		
		public virtual void OnIfStatement(IfStatement node, out Statement resultingNode)
		{				
			Statement result = node;
			
			if (EnterIfStatement(node, ref result))
			{		
				Expression currentExpressionValue = node.Expression;
				Node resultingExpressionValue;
				Switch(currentExpressionValue, out resultingExpressionValue);
				if (currentExpressionValue != resultingExpressionValue)
				{
					node.Expression = (Expression)resultingExpressionValue;
				}
				Block currentTrueBlockValue = node.TrueBlock;
				Node resultingTrueBlockValue;
				Switch(currentTrueBlockValue, out resultingTrueBlockValue);
				if (currentTrueBlockValue != resultingTrueBlockValue)
				{
					node.TrueBlock = (Block)resultingTrueBlockValue;
				}
				Block currentFalseBlockValue = node.FalseBlock;
				Node resultingFalseBlockValue;
				Switch(currentFalseBlockValue, out resultingFalseBlockValue);
				if (currentFalseBlockValue != resultingFalseBlockValue)
				{
					node.FalseBlock = (Block)resultingFalseBlockValue;
				}
				StatementModifier currentModifierValue = node.Modifier;
				Node resultingModifierValue;
				Switch(currentModifierValue, out resultingModifierValue);
				if (currentModifierValue != resultingModifierValue)
				{
					node.Modifier = (StatementModifier)resultingModifierValue;
				}
			}
			LeaveIfStatement(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterIfStatement(IfStatement node, ref Statement resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveIfStatement(IfStatement node, ref Statement resultingNode)
		{
		}
		
		public virtual void OnForStatement(ForStatement node, out Statement resultingNode)
		{				
			Statement result = node;
			
			if (EnterForStatement(node, ref result))
			{		
				Switch(node.Declarations);
				Expression currentIteratorValue = node.Iterator;
				Node resultingIteratorValue;
				Switch(currentIteratorValue, out resultingIteratorValue);
				if (currentIteratorValue != resultingIteratorValue)
				{
					node.Iterator = (Expression)resultingIteratorValue;
				}
				Switch(node.Statements);
				StatementModifier currentModifierValue = node.Modifier;
				Node resultingModifierValue;
				Switch(currentModifierValue, out resultingModifierValue);
				if (currentModifierValue != resultingModifierValue)
				{
					node.Modifier = (StatementModifier)resultingModifierValue;
				}
			}
			LeaveForStatement(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterForStatement(ForStatement node, ref Statement resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveForStatement(ForStatement node, ref Statement resultingNode)
		{
		}
		
		public virtual void OnWhileStatement(WhileStatement node, out Statement resultingNode)
		{				
			Statement result = node;
			
			if (EnterWhileStatement(node, ref result))
			{		
				Expression currentConditionValue = node.Condition;
				Node resultingConditionValue;
				Switch(currentConditionValue, out resultingConditionValue);
				if (currentConditionValue != resultingConditionValue)
				{
					node.Condition = (Expression)resultingConditionValue;
				}
				Switch(node.Statements);
				StatementModifier currentModifierValue = node.Modifier;
				Node resultingModifierValue;
				Switch(currentModifierValue, out resultingModifierValue);
				if (currentModifierValue != resultingModifierValue)
				{
					node.Modifier = (StatementModifier)resultingModifierValue;
				}
			}
			LeaveWhileStatement(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterWhileStatement(WhileStatement node, ref Statement resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveWhileStatement(WhileStatement node, ref Statement resultingNode)
		{
		}
		
		public virtual void OnGivenStatement(GivenStatement node, out Statement resultingNode)
		{				
			Statement result = node;
			
			if (EnterGivenStatement(node, ref result))
			{		
				Expression currentExpressionValue = node.Expression;
				Node resultingExpressionValue;
				Switch(currentExpressionValue, out resultingExpressionValue);
				if (currentExpressionValue != resultingExpressionValue)
				{
					node.Expression = (Expression)resultingExpressionValue;
				}
				Switch(node.WhenClauses);
				Block currentOtherwiseBlockValue = node.OtherwiseBlock;
				Node resultingOtherwiseBlockValue;
				Switch(currentOtherwiseBlockValue, out resultingOtherwiseBlockValue);
				if (currentOtherwiseBlockValue != resultingOtherwiseBlockValue)
				{
					node.OtherwiseBlock = (Block)resultingOtherwiseBlockValue;
				}
				StatementModifier currentModifierValue = node.Modifier;
				Node resultingModifierValue;
				Switch(currentModifierValue, out resultingModifierValue);
				if (currentModifierValue != resultingModifierValue)
				{
					node.Modifier = (StatementModifier)resultingModifierValue;
				}
			}
			LeaveGivenStatement(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterGivenStatement(GivenStatement node, ref Statement resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveGivenStatement(GivenStatement node, ref Statement resultingNode)
		{
		}
		
		public virtual void OnWhenClause(WhenClause node, out WhenClause resultingNode)
		{				
			WhenClause result = node;
			
			if (EnterWhenClause(node, ref result))
			{		
				Expression currentConditionValue = node.Condition;
				Node resultingConditionValue;
				Switch(currentConditionValue, out resultingConditionValue);
				if (currentConditionValue != resultingConditionValue)
				{
					node.Condition = (Expression)resultingConditionValue;
				}
				Switch(node.Statements);
			}
			LeaveWhenClause(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterWhenClause(WhenClause node, ref WhenClause resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveWhenClause(WhenClause node, ref WhenClause resultingNode)
		{
		}
		
		public virtual void OnBreakStatement(BreakStatement node, out Statement resultingNode)
		{				
			Statement result = node;
			
			if (EnterBreakStatement(node, ref result))
			{		
				StatementModifier currentModifierValue = node.Modifier;
				Node resultingModifierValue;
				Switch(currentModifierValue, out resultingModifierValue);
				if (currentModifierValue != resultingModifierValue)
				{
					node.Modifier = (StatementModifier)resultingModifierValue;
				}
			}
			LeaveBreakStatement(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterBreakStatement(BreakStatement node, ref Statement resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveBreakStatement(BreakStatement node, ref Statement resultingNode)
		{
		}
		
		public virtual void OnContinueStatement(ContinueStatement node, out Statement resultingNode)
		{				
			Statement result = node;
			
			if (EnterContinueStatement(node, ref result))
			{		
				StatementModifier currentModifierValue = node.Modifier;
				Node resultingModifierValue;
				Switch(currentModifierValue, out resultingModifierValue);
				if (currentModifierValue != resultingModifierValue)
				{
					node.Modifier = (StatementModifier)resultingModifierValue;
				}
			}
			LeaveContinueStatement(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterContinueStatement(ContinueStatement node, ref Statement resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveContinueStatement(ContinueStatement node, ref Statement resultingNode)
		{
		}
		
		public virtual void OnRetryStatement(RetryStatement node, out Statement resultingNode)
		{				
			Statement result = node;
			
			if (EnterRetryStatement(node, ref result))
			{		
				StatementModifier currentModifierValue = node.Modifier;
				Node resultingModifierValue;
				Switch(currentModifierValue, out resultingModifierValue);
				if (currentModifierValue != resultingModifierValue)
				{
					node.Modifier = (StatementModifier)resultingModifierValue;
				}
			}
			LeaveRetryStatement(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterRetryStatement(RetryStatement node, ref Statement resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveRetryStatement(RetryStatement node, ref Statement resultingNode)
		{
		}
		
		public virtual void OnReturnStatement(ReturnStatement node, out Statement resultingNode)
		{				
			Statement result = node;
			
			if (EnterReturnStatement(node, ref result))
			{		
				Expression currentExpressionValue = node.Expression;
				Node resultingExpressionValue;
				Switch(currentExpressionValue, out resultingExpressionValue);
				if (currentExpressionValue != resultingExpressionValue)
				{
					node.Expression = (Expression)resultingExpressionValue;
				}
				StatementModifier currentModifierValue = node.Modifier;
				Node resultingModifierValue;
				Switch(currentModifierValue, out resultingModifierValue);
				if (currentModifierValue != resultingModifierValue)
				{
					node.Modifier = (StatementModifier)resultingModifierValue;
				}
			}
			LeaveReturnStatement(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterReturnStatement(ReturnStatement node, ref Statement resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveReturnStatement(ReturnStatement node, ref Statement resultingNode)
		{
		}
		
		public virtual void OnYieldStatement(YieldStatement node, out Statement resultingNode)
		{				
			Statement result = node;
			
			if (EnterYieldStatement(node, ref result))
			{		
				Expression currentExpressionValue = node.Expression;
				Node resultingExpressionValue;
				Switch(currentExpressionValue, out resultingExpressionValue);
				if (currentExpressionValue != resultingExpressionValue)
				{
					node.Expression = (Expression)resultingExpressionValue;
				}
				StatementModifier currentModifierValue = node.Modifier;
				Node resultingModifierValue;
				Switch(currentModifierValue, out resultingModifierValue);
				if (currentModifierValue != resultingModifierValue)
				{
					node.Modifier = (StatementModifier)resultingModifierValue;
				}
			}
			LeaveYieldStatement(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterYieldStatement(YieldStatement node, ref Statement resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveYieldStatement(YieldStatement node, ref Statement resultingNode)
		{
		}
		
		public virtual void OnRaiseStatement(RaiseStatement node, out Statement resultingNode)
		{				
			Statement result = node;
			
			if (EnterRaiseStatement(node, ref result))
			{		
				Expression currentExceptionValue = node.Exception;
				Node resultingExceptionValue;
				Switch(currentExceptionValue, out resultingExceptionValue);
				if (currentExceptionValue != resultingExceptionValue)
				{
					node.Exception = (Expression)resultingExceptionValue;
				}
				StatementModifier currentModifierValue = node.Modifier;
				Node resultingModifierValue;
				Switch(currentModifierValue, out resultingModifierValue);
				if (currentModifierValue != resultingModifierValue)
				{
					node.Modifier = (StatementModifier)resultingModifierValue;
				}
			}
			LeaveRaiseStatement(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterRaiseStatement(RaiseStatement node, ref Statement resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveRaiseStatement(RaiseStatement node, ref Statement resultingNode)
		{
		}
		
		public virtual void OnUnpackStatement(UnpackStatement node, out Statement resultingNode)
		{				
			Statement result = node;
			
			if (EnterUnpackStatement(node, ref result))
			{		
				Switch(node.Declarations);
				Expression currentExpressionValue = node.Expression;
				Node resultingExpressionValue;
				Switch(currentExpressionValue, out resultingExpressionValue);
				if (currentExpressionValue != resultingExpressionValue)
				{
					node.Expression = (Expression)resultingExpressionValue;
				}
				StatementModifier currentModifierValue = node.Modifier;
				Node resultingModifierValue;
				Switch(currentModifierValue, out resultingModifierValue);
				if (currentModifierValue != resultingModifierValue)
				{
					node.Modifier = (StatementModifier)resultingModifierValue;
				}
			}
			LeaveUnpackStatement(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterUnpackStatement(UnpackStatement node, ref Statement resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveUnpackStatement(UnpackStatement node, ref Statement resultingNode)
		{
		}
		
		public virtual void OnExpressionStatement(ExpressionStatement node, out Statement resultingNode)
		{				
			Statement result = node;
			
			if (EnterExpressionStatement(node, ref result))
			{		
				Expression currentExpressionValue = node.Expression;
				Node resultingExpressionValue;
				Switch(currentExpressionValue, out resultingExpressionValue);
				if (currentExpressionValue != resultingExpressionValue)
				{
					node.Expression = (Expression)resultingExpressionValue;
				}
				StatementModifier currentModifierValue = node.Modifier;
				Node resultingModifierValue;
				Switch(currentModifierValue, out resultingModifierValue);
				if (currentModifierValue != resultingModifierValue)
				{
					node.Modifier = (StatementModifier)resultingModifierValue;
				}
			}
			LeaveExpressionStatement(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterExpressionStatement(ExpressionStatement node, ref Statement resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveExpressionStatement(ExpressionStatement node, ref Statement resultingNode)
		{
		}
		
		public virtual void OnOmittedExpression(OmittedExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
				
			resultingNode = result;
		}
		
		public virtual void OnExpressionPair(ExpressionPair node, out ExpressionPair resultingNode)
		{				
			ExpressionPair result = node;
			
			if (EnterExpressionPair(node, ref result))
			{		
				Expression currentFirstValue = node.First;
				Node resultingFirstValue;
				Switch(currentFirstValue, out resultingFirstValue);
				if (currentFirstValue != resultingFirstValue)
				{
					node.First = (Expression)resultingFirstValue;
				}
				Expression currentSecondValue = node.Second;
				Node resultingSecondValue;
				Switch(currentSecondValue, out resultingSecondValue);
				if (currentSecondValue != resultingSecondValue)
				{
					node.Second = (Expression)resultingSecondValue;
				}
			}
			LeaveExpressionPair(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterExpressionPair(ExpressionPair node, ref ExpressionPair resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveExpressionPair(ExpressionPair node, ref ExpressionPair resultingNode)
		{
		}
		
		public virtual void OnMethodInvocationExpression(MethodInvocationExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
			if (EnterMethodInvocationExpression(node, ref result))
			{		
				Expression currentTargetValue = node.Target;
				Node resultingTargetValue;
				Switch(currentTargetValue, out resultingTargetValue);
				if (currentTargetValue != resultingTargetValue)
				{
					node.Target = (Expression)resultingTargetValue;
				}
				Switch(node.Arguments);
				Switch(node.NamedArguments);
			}
			LeaveMethodInvocationExpression(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterMethodInvocationExpression(MethodInvocationExpression node, ref Expression resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveMethodInvocationExpression(MethodInvocationExpression node, ref Expression resultingNode)
		{
		}
		
		public virtual void OnUnaryExpression(UnaryExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
			if (EnterUnaryExpression(node, ref result))
			{		
				Expression currentOperandValue = node.Operand;
				Node resultingOperandValue;
				Switch(currentOperandValue, out resultingOperandValue);
				if (currentOperandValue != resultingOperandValue)
				{
					node.Operand = (Expression)resultingOperandValue;
				}
			}
			LeaveUnaryExpression(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterUnaryExpression(UnaryExpression node, ref Expression resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveUnaryExpression(UnaryExpression node, ref Expression resultingNode)
		{
		}
		
		public virtual void OnBinaryExpression(BinaryExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
			if (EnterBinaryExpression(node, ref result))
			{		
				Expression currentLeftValue = node.Left;
				Node resultingLeftValue;
				Switch(currentLeftValue, out resultingLeftValue);
				if (currentLeftValue != resultingLeftValue)
				{
					node.Left = (Expression)resultingLeftValue;
				}
				Expression currentRightValue = node.Right;
				Node resultingRightValue;
				Switch(currentRightValue, out resultingRightValue);
				if (currentRightValue != resultingRightValue)
				{
					node.Right = (Expression)resultingRightValue;
				}
			}
			LeaveBinaryExpression(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterBinaryExpression(BinaryExpression node, ref Expression resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveBinaryExpression(BinaryExpression node, ref Expression resultingNode)
		{
		}
		
		public virtual void OnTernaryExpression(TernaryExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
			if (EnterTernaryExpression(node, ref result))
			{		
				Expression currentConditionValue = node.Condition;
				Node resultingConditionValue;
				Switch(currentConditionValue, out resultingConditionValue);
				if (currentConditionValue != resultingConditionValue)
				{
					node.Condition = (Expression)resultingConditionValue;
				}
				Expression currentTrueExpressionValue = node.TrueExpression;
				Node resultingTrueExpressionValue;
				Switch(currentTrueExpressionValue, out resultingTrueExpressionValue);
				if (currentTrueExpressionValue != resultingTrueExpressionValue)
				{
					node.TrueExpression = (Expression)resultingTrueExpressionValue;
				}
				Expression currentFalseExpressionValue = node.FalseExpression;
				Node resultingFalseExpressionValue;
				Switch(currentFalseExpressionValue, out resultingFalseExpressionValue);
				if (currentFalseExpressionValue != resultingFalseExpressionValue)
				{
					node.FalseExpression = (Expression)resultingFalseExpressionValue;
				}
			}
			LeaveTernaryExpression(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterTernaryExpression(TernaryExpression node, ref Expression resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveTernaryExpression(TernaryExpression node, ref Expression resultingNode)
		{
		}
		
		public virtual void OnReferenceExpression(ReferenceExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
				
			resultingNode = result;
		}
		
		public virtual void OnMemberReferenceExpression(MemberReferenceExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
			if (EnterMemberReferenceExpression(node, ref result))
			{		
				Expression currentTargetValue = node.Target;
				Node resultingTargetValue;
				Switch(currentTargetValue, out resultingTargetValue);
				if (currentTargetValue != resultingTargetValue)
				{
					node.Target = (Expression)resultingTargetValue;
				}
			}
			LeaveMemberReferenceExpression(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterMemberReferenceExpression(MemberReferenceExpression node, ref Expression resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveMemberReferenceExpression(MemberReferenceExpression node, ref Expression resultingNode)
		{
		}
		
		public virtual void OnLiteralExpression(LiteralExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
				
			resultingNode = result;
		}
		
		public virtual void OnStringLiteralExpression(StringLiteralExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
				
			resultingNode = result;
		}
		
		public virtual void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
				
			resultingNode = result;
		}
		
		public virtual void OnIntegerLiteralExpression(IntegerLiteralExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
				
			resultingNode = result;
		}
		
		public virtual void OnNullLiteralExpression(NullLiteralExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
				
			resultingNode = result;
		}
		
		public virtual void OnSelfLiteralExpression(SelfLiteralExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
				
			resultingNode = result;
		}
		
		public virtual void OnSuperLiteralExpression(SuperLiteralExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
				
			resultingNode = result;
		}
		
		public virtual void OnBoolLiteralExpression(BoolLiteralExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
				
			resultingNode = result;
		}
		
		public virtual void OnRELiteralExpression(RELiteralExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
				
			resultingNode = result;
		}
		
		public virtual void OnStringFormattingExpression(StringFormattingExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
			if (EnterStringFormattingExpression(node, ref result))
			{		
				Switch(node.Arguments);
			}
			LeaveStringFormattingExpression(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterStringFormattingExpression(StringFormattingExpression node, ref Expression resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveStringFormattingExpression(StringFormattingExpression node, ref Expression resultingNode)
		{
		}
		
		public virtual void OnHashLiteralExpression(HashLiteralExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
			if (EnterHashLiteralExpression(node, ref result))
			{		
				Switch(node.Items);
			}
			LeaveHashLiteralExpression(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterHashLiteralExpression(HashLiteralExpression node, ref Expression resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveHashLiteralExpression(HashLiteralExpression node, ref Expression resultingNode)
		{
		}
		
		public virtual void OnListLiteralExpression(ListLiteralExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
			if (EnterListLiteralExpression(node, ref result))
			{		
				Switch(node.Items);
			}
			LeaveListLiteralExpression(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterListLiteralExpression(ListLiteralExpression node, ref Expression resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveListLiteralExpression(ListLiteralExpression node, ref Expression resultingNode)
		{
		}
		
		public virtual void OnTupleLiteralExpression(TupleLiteralExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
			if (EnterTupleLiteralExpression(node, ref result))
			{		
				Switch(node.Items);
			}
			LeaveTupleLiteralExpression(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterTupleLiteralExpression(TupleLiteralExpression node, ref Expression resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveTupleLiteralExpression(TupleLiteralExpression node, ref Expression resultingNode)
		{
		}
		
		public virtual void OnListDisplayExpression(ListDisplayExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
			if (EnterListDisplayExpression(node, ref result))
			{		
				Expression currentExpressionValue = node.Expression;
				Node resultingExpressionValue;
				Switch(currentExpressionValue, out resultingExpressionValue);
				if (currentExpressionValue != resultingExpressionValue)
				{
					node.Expression = (Expression)resultingExpressionValue;
				}
				Switch(node.Declarations);
				Expression currentIteratorValue = node.Iterator;
				Node resultingIteratorValue;
				Switch(currentIteratorValue, out resultingIteratorValue);
				if (currentIteratorValue != resultingIteratorValue)
				{
					node.Iterator = (Expression)resultingIteratorValue;
				}
				StatementModifier currentFilterValue = node.Filter;
				Node resultingFilterValue;
				Switch(currentFilterValue, out resultingFilterValue);
				if (currentFilterValue != resultingFilterValue)
				{
					node.Filter = (StatementModifier)resultingFilterValue;
				}
			}
			LeaveListDisplayExpression(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterListDisplayExpression(ListDisplayExpression node, ref Expression resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveListDisplayExpression(ListDisplayExpression node, ref Expression resultingNode)
		{
		}
		
		public virtual void OnSlicingExpression(SlicingExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
			if (EnterSlicingExpression(node, ref result))
			{		
				Expression currentTargetValue = node.Target;
				Node resultingTargetValue;
				Switch(currentTargetValue, out resultingTargetValue);
				if (currentTargetValue != resultingTargetValue)
				{
					node.Target = (Expression)resultingTargetValue;
				}
				Expression currentBeginValue = node.Begin;
				Node resultingBeginValue;
				Switch(currentBeginValue, out resultingBeginValue);
				if (currentBeginValue != resultingBeginValue)
				{
					node.Begin = (Expression)resultingBeginValue;
				}
				Expression currentEndValue = node.End;
				Node resultingEndValue;
				Switch(currentEndValue, out resultingEndValue);
				if (currentEndValue != resultingEndValue)
				{
					node.End = (Expression)resultingEndValue;
				}
				Expression currentStepValue = node.Step;
				Node resultingStepValue;
				Switch(currentStepValue, out resultingStepValue);
				if (currentStepValue != resultingStepValue)
				{
					node.Step = (Expression)resultingStepValue;
				}
			}
			LeaveSlicingExpression(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterSlicingExpression(SlicingExpression node, ref Expression resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveSlicingExpression(SlicingExpression node, ref Expression resultingNode)
		{
		}
		
		public virtual void OnAsExpression(AsExpression node, out Expression resultingNode)
		{				
			Expression result = node;
			
			if (EnterAsExpression(node, ref result))
			{		
				Expression currentTargetValue = node.Target;
				Node resultingTargetValue;
				Switch(currentTargetValue, out resultingTargetValue);
				if (currentTargetValue != resultingTargetValue)
				{
					node.Target = (Expression)resultingTargetValue;
				}
				TypeReference currentTypeValue = node.Type;
				Node resultingTypeValue;
				Switch(currentTypeValue, out resultingTypeValue);
				if (currentTypeValue != resultingTypeValue)
				{
					node.Type = (TypeReference)resultingTypeValue;
				}
			}
			LeaveAsExpression(node, ref result);
				
			resultingNode = result;
		}
		
		public virtual bool EnterAsExpression(AsExpression node, ref Expression resultingNode)
		{
			return true;
		}
		
		public virtual void LeaveAsExpression(AsExpression node, ref Expression resultingNode)
		{
		}
		
	}
}
