#region license
// Copyright (c) 2004, Daniel Grunwald (daniel@danielgrunwald.de)
// All rights reserved.
//
// BooBinding is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// BooBinding is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with BooBinding; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

namespace BooBinding

import System
import System.Reflection
import System.CodeDom
import System.Text
import System.Collections

import ICSharpCode.SharpRefactory.Parser
import ICSharpCode.SharpRefactory.Parser.AST
import ICSharpCode.SharpRefactory.PrettyPrinter

class BooVisitor(AbstractASTVisitor):
	_newLineSep = Environment.NewLine
	
	[Getter(SourceText)]
	_sourceText = StringBuilder()
	_indentLevel = 0
	_indentOpenPosition = 0
	_errors      = Errors()
	_currentType as TypeDeclaration = null
	_debugOutput = false
	_inmacro = false
	
	#region IASTVisitor interface implementation
	override def Visit(node as INode, data as object):
		_errors.Error(-1, -1, "Visited INode (should NEVER HAPPEN)")
		Console.WriteLine("Visitor was: " + self.GetType())
		Console.WriteLine("Node was : " + node.GetType())
		return node.AcceptChildren(self, data)
	
	def AppendIndentation():
		for i in range(_indentLevel):
			_sourceText.Append("\t")
	
	def AppendNewLine():
		_sourceText.Append(_newLineSep)
	
	def DebugOutput(o as INode):
		Console.WriteLine(o.ToString()) if _debugOutput
		return
	
	def AddIndentLevel():
		_indentLevel += 1
		_indentOpenPosition = _sourceText.Length
	
	def RemoveIndentLevel():
		if _indentOpenPosition == _sourceText.Length:
			// nothing was inserted in this block -> insert pass
			_indentOpenPosition = 0
			AppendIndentation()
			_sourceText.Append("pass")
			AppendNewLine()
		_indentLevel -= 1
	
	override def Visit(compilationUnit as CompilationUnit, data):
		DebugOutput(compilationUnit)
		BooRefactory().Refactor(compilationUnit)
		compilationUnit.AcceptChildren(self, data)
		return null
	
	override def Visit(namespaceDeclaration as NamespaceDeclaration, data):
		DebugOutput(namespaceDeclaration)
		AppendIndentation()
		_sourceText.Append("namespace ")
		_sourceText.Append(namespaceDeclaration.NameSpace)
		AppendNewLine()
		namespaceDeclaration.AcceptChildren(self, data)
		AppendNewLine()
		return null
	
	override def Visit(usingDeclaration as UsingDeclaration, data):
		DebugOutput(usingDeclaration)
		AppendIndentation()
		_sourceText.Append("import ")
		_sourceText.Append(usingDeclaration.Namespace)
		AppendNewLine()
		return null
	
	override def Visit(usingAliasDeclaration as UsingAliasDeclaration, data):
		DebugOutput(usingAliasDeclaration)
		AppendIndentation()
		_sourceText.Append("import ")
		_sourceText.Append(usingAliasDeclaration.Namespace)
		_sourceText.Append(" as ")
		_sourceText.Append(usingAliasDeclaration.Alias)
		AppendNewLine()
		return null
	
	override def Visit(attributeSection as AttributeSection, data):
		DebugOutput(attributeSection)
		AppendIndentation()
		_sourceText.Append("[")
		if (attributeSection.AttributeTarget != null and attributeSection.AttributeTarget.Length > 0):
			_sourceText.Append(attributeSection.AttributeTarget)
			_sourceText.Append(": ")
		for j in range(attributeSection.Attributes.Count):
			attr as ICSharpCode.SharpRefactory.Parser.AST.Attribute = attributeSection.Attributes[j]
			
			_sourceText.Append(attr.Name)
			_sourceText.Append("(")
			for i in range(attr.PositionalArguments.Count):
				expr as Expression = attr.PositionalArguments[i]
				_sourceText.Append(expr.AcceptVisitor(self, data).ToString())
				if (i + 1 < attr.PositionalArguments.Count):
					_sourceText.Append(", ")
			
			for i in range(attr.NamedArguments.Count):
				if (i > 0 or attr.PositionalArguments.Count > 0):
					_sourceText.Append(", ")
				named as NamedArgument = attr.NamedArguments[i]
				_sourceText.Append(named.Name)
				_sourceText.Append(": ")
				_sourceText.Append(named.Expr.AcceptVisitor(self, data).ToString())
			
			_sourceText.Append(")")
			if (j + 1 < attributeSection.Attributes.Count):
				_sourceText.Append(", ")
		
		_sourceText.Append("]")
		AppendNewLine()
		return null
	
	override def Visit(typeDeclaration as TypeDeclaration, data):
		DebugOutput(typeDeclaration)
		AppendIndentation()
		AppendNewLine()
		AppendAttributes(typeDeclaration.Attributes)	
		
		//Add a [Module] attribute if this is a class with a Main method
		if typeDeclaration.Type == Types.Class:
			for child in typeDeclaration.Children:
				method = child as MethodDeclaration
				if method is not null and method.Name == "Main":
					AppendIndentation()
					_sourceText.Append("[System.Runtime.CompilerServices.CompilerGlobalScope]")
					AppendNewLine()
			
		modifier = GetModifier(typeDeclaration.Modifier, Modifier.Public)

		typeString = "class "
		
		typeString = "enum " if typeDeclaration.Type == Types.Enum
		typeString = "interface " if typeDeclaration.Type == Types.Interface
		
		AppendIndentation()
		_sourceText.Append(modifier)
		_sourceText.Append(typeString)
		_sourceText.Append(typeDeclaration.Name)
		
		if typeDeclaration.BaseTypes == null:
			_sourceText.Append("(System.ValueType)") if typeDeclaration.Type == Types.Struct
		else:
			_sourceText.Append("(")
			first = true
			for baseType as string in typeDeclaration.BaseTypes:
				if first:
					first = false
				else:
					_sourceText.Append(", ")
				_sourceText.Append(baseType); 
			_sourceText.Append(")")
		_sourceText.Append(":")
		AppendNewLine()
		AddIndentLevel()
		oldType as TypeDeclaration = _currentType
		_currentType = typeDeclaration
		typeDeclaration.AcceptChildren(self, data)
		_currentType = oldType
		RemoveIndentLevel()
		AppendNewLine()
		return null
	
	override def Visit(delegateDeclaration as DelegateDeclaration, data):
		DebugOutput(delegateDeclaration)
		AppendNewLine()
		AppendAttributes(delegateDeclaration.Attributes)
		AppendIndentation()
		_sourceText.Append(GetModifier(delegateDeclaration.Modifier, Modifier.Public))
		_sourceText.Append("callable ")
		_sourceText.Append(delegateDeclaration.Name)
		_sourceText.Append("(")
		AppendParameters(delegateDeclaration.Parameters)
		_sourceText.Append(")")
		if delegateDeclaration.ReturnType.Type != "void":
			_sourceText.Append(" as ")
			_sourceText.Append(GetTypeString(delegateDeclaration.ReturnType))
		AppendNewLine()
		return null
	
	override def Visit(variableDeclaration as VariableDeclaration, data):
		AppendIndentation()
		_sourceText.Append(variableDeclaration.Name)
		if (variableDeclaration.Initializer != null):
			_sourceText.Append(" = ")
			_sourceText.Append(variableDeclaration.Initializer.AcceptVisitor(self, data))
		AppendNewLine()
		return null
	
	override def Visit(fieldDeclaration as FieldDeclaration, data):
		DebugOutput(fieldDeclaration)
		for field as VariableDeclaration in fieldDeclaration.Fields:
			AppendAttributes(fieldDeclaration.Attributes)
			AppendIndentation()
			// enum fields don't have a type or modifier
			if fieldDeclaration.TypeReference != null:
				_sourceText.Append(GetModifier(fieldDeclaration.Modifier, Modifier.Protected))
			_sourceText.Append(field.Name)
			if fieldDeclaration.TypeReference != null:
				_sourceText.Append(" as ")
				_sourceText.Append(GetTypeString(fieldDeclaration.TypeReference))
			if (field.Initializer != null):
				_sourceText.Append(" = ")
				_sourceText.Append(field.Initializer.AcceptVisitor(self, data).ToString())
			AppendNewLine()
		if fieldDeclaration.TypeReference != null:
			AppendIndentation()
			AppendNewLine()
		return null
	
	override def Visit(methodDeclaration as MethodDeclaration, data):
		DebugOutput(methodDeclaration)
		AppendAttributes(methodDeclaration.Attributes)
		AppendIndentation()
		isFunction as bool = methodDeclaration.TypeReference.Type != "void"
		_sourceText.Append(GetModifier(methodDeclaration.Modifier, Modifier.Public))
		_sourceText.Append("def ")
		_sourceText.Append(methodDeclaration.Name)
		_sourceText.Append("(")
		AppendParameters(methodDeclaration.Parameters)
		_sourceText.Append(")")
		if (isFunction):
			_sourceText.Append(" as ")
			_sourceText.Append(GetTypeString(methodDeclaration.TypeReference))
		if (_currentType.Type != Types.Interface):
			if (methodDeclaration.Body != null):
				_sourceText.Append(":")
				AppendNewLine()
				AddIndentLevel()
				methodDeclaration.Body.AcceptVisitor(self, data)
				RemoveIndentLevel()
				AppendIndentation()
			else:
				_sourceText.Append(":")
				AppendNewLine()
				AddIndentLevel()
				AppendIndentation()
				_sourceText.Append("pass")
				RemoveIndentLevel()
				AppendNewLine()
		AppendNewLine()
		return null
	
	override def Visit(propertyDeclaration as PropertyDeclaration, data):
		DebugOutput(propertyDeclaration)
		AppendAttributes(propertyDeclaration.Attributes)
		AppendIndentation()
		_sourceText.Append(GetModifier(propertyDeclaration.Modifier, Modifier.Public))
		_sourceText.Append(propertyDeclaration.Name)
		_sourceText.Append(" as ")
		_sourceText.Append(GetTypeString(propertyDeclaration.TypeReference))
		_sourceText.Append(":")
		AppendNewLine()
		
		AddIndentLevel()
		if (propertyDeclaration.GetRegion != null):
			propertyDeclaration.GetRegion.AcceptVisitor(self, data)
		
		if (propertyDeclaration.SetRegion != null):
			propertyDeclaration.SetRegion.AcceptVisitor(self, data)
		
		RemoveIndentLevel()
		AppendIndentation()
		AppendNewLine()
		return null
	
	override def Visit(propertyGetRegion as PropertyGetRegion, data):
		DebugOutput(propertyGetRegion)
		AppendAttributes(propertyGetRegion.Attributes)
		AppendIndentation()
		if propertyGetRegion.Block == null:
			_sourceText.Append("get")
			if (_currentType.Type != Types.Interface):
				_sourceText.Append(":")
				AppendNewLine()
				AddIndentLevel()
				AppendIndentation()
				_sourceText.Append("pass")
				RemoveIndentLevel()
			AppendNewLine()
		else:
			_sourceText.Append("get:")
			AppendNewLine()
			AddIndentLevel()
			propertyGetRegion.Block.AcceptVisitor(self, data)
			RemoveIndentLevel()
		return null
	
	override def Visit(propertySetRegion as PropertySetRegion, data):
		DebugOutput(propertySetRegion)
		AppendAttributes(propertySetRegion.Attributes)
		AppendIndentation()
		if propertySetRegion.Block == null:
			_sourceText.Append("set")
			if (_currentType.Type != Types.Interface):
				_sourceText.Append(":")
				AppendNewLine()
				AddIndentLevel()
				AppendIndentation()
				_sourceText.Append("pass")
				RemoveIndentLevel()
			AppendNewLine()
		else:
			_sourceText.Append("set:")
			AppendNewLine()
			AddIndentLevel()
			propertySetRegion.Block.AcceptVisitor(self, data)
			RemoveIndentLevel()
		
		return null
	
	override def Visit(eventDeclaration as EventDeclaration, data):
		DebugOutput(eventDeclaration)
		AppendNewLine()
		if (eventDeclaration.Name == null):
			for field as VariableDeclaration in eventDeclaration.VariableDeclarators:
				AppendAttributes(eventDeclaration.Attributes)
				AppendIndentation()
				_sourceText.Append(GetModifier(eventDeclaration.Modifier, Modifier.Public))
				_sourceText.Append("event ")
				_sourceText.Append(field.Name)
				_sourceText.Append(" as ")
				_sourceText.Append(GetTypeString(eventDeclaration.TypeReference))
				AppendNewLine()
		else:
			AppendAttributes(eventDeclaration.Attributes)
			AppendIndentation()
			_sourceText.Append(GetModifier(eventDeclaration.Modifier, Modifier.Public))
			_sourceText.Append("event ")
			_sourceText.Append(eventDeclaration.Name)
			_sourceText.Append(" as ")
			_sourceText.Append(GetTypeString(eventDeclaration.TypeReference))
			_sourceText.Append(":")
			AppendNewLine()
			AddIndentLevel()
			if (eventDeclaration.HasAddRegion):
				eventDeclaration.AddRegion.AcceptVisitor(self, data)
			
			if (eventDeclaration.HasRemoveRegion):
				eventDeclaration.RemoveRegion.AcceptVisitor(self, data)
			RemoveIndentLevel()
			AppendIndentation()
			AppendNewLine()
		
		return data
	
	override def Visit(eventAddRegion as EventAddRegion, data):
		AddIndentLevel()
		_sourceText.Append("add:")
		AppendNewLine()
		AddIndentLevel()
		eventAddRegion.Block.AcceptVisitor(self, data) if eventAddRegion.Block != null
		RemoveIndentLevel()
		_errors.Error(-1, -1, "Event add region can't be converted")
		return null
	
	override def Visit(eventRemoveRegion as EventRemoveRegion, data):
		AddIndentLevel()
		_sourceText.Append("remove:")
		AppendNewLine()
		AddIndentLevel()
		eventRemoveRegion.Block.AcceptVisitor(self, data) if eventRemoveRegion.Block != null
		RemoveIndentLevel()
		_errors.Error(-1, -1, "Event remove region can't be converted")
		return null
	
	override def Visit(constructorDeclaration as ConstructorDeclaration, data):
		DebugOutput(constructorDeclaration)
		AppendIndentation()
		_sourceText.Append(GetModifier(constructorDeclaration.Modifier, Modifier.Public))
		_sourceText.Append("def constructor")
		_sourceText.Append("(")
		AppendParameters(constructorDeclaration.Parameters)
		_sourceText.Append("):")
		AppendNewLine()
		
		AddIndentLevel()
		ci = constructorDeclaration.ConstructorInitializer
		if (ci != null):
			AppendIndentation()
			if (ci.ConstructorInitializerType == ConstructorInitializerType.Base):
				_sourceText.Append("super")
			else:
				_sourceText.Append("self")
			_sourceText.Append(GetParameters(ci.Arguments))
			AppendNewLine()
		
		DebugOutput(constructorDeclaration.Body)
		constructorDeclaration.Body.AcceptChildren(self, data)
		RemoveIndentLevel()
		
		AppendIndentation()
		AppendNewLine()
		return null
	
	override def Visit(destructorDeclaration as DestructorDeclaration, data):
		DebugOutput(destructorDeclaration)
		AppendNewLine()
		AppendIndentation()
		_sourceText.Append("def destructor():")
		AppendNewLine()
		
		AddIndentLevel()
		destructorDeclaration.Body.AcceptChildren(self, data)
		RemoveIndentLevel()
		
		return null
	
	def GetOperatorName(token as int, opType as OperatorType):
		if opType == OperatorType.Binary:
			return "op_Addition"           if token == Tokens.Plus
			return "op_Subtraction"        if token == Tokens.Minus
			return "op_Multiply"           if token == Tokens.Times
			return "op_Division"           if token == Tokens.Div
			return "op_Modulus"            if token == Tokens.Mod
			return "op_Equality"           if token == Tokens.Equal
			return "op_Inequality"           if token == Tokens.NotEqual
			return "op_LessThan"           if token == Tokens.LessThan
			return "op_LessThanOrEqual"    if token == Tokens.LessEqual
			return "op_GreaterThan"        if token == Tokens.GreaterThan
			return "op_GreaterThanOrEqual" if token == Tokens.GreaterEqual
			return "op_BitwiseOr"          if token == Tokens.BitwiseOr
			return "op_BitwiseAnd"         if token == Tokens.BitwiseAnd
		return "op_<unknown:${Tokens.GetTokenString(token)}>"
	
	override def Visit(operatorDeclaration as OperatorDeclaration, data):
		declarator = operatorDeclaration.OpratorDeclarator
		DebugOutput(operatorDeclaration)
		AppendAttributes(operatorDeclaration.Attributes)
		AppendIndentation()
		_sourceText.Append(GetModifier(operatorDeclaration.Modifier, Modifier.Public))
		_sourceText.Append("def ")
		_sourceText.Append(GetOperatorName(declarator.OverloadOperatorToken, declarator.OperatorType))
		_sourceText.Append("(")
		_sourceText.Append(declarator.FirstParameterName)
		_sourceText.Append(" as ")
		_sourceText.Append(GetTypeString(declarator.FirstParameterType))
		if (declarator.OperatorType == OperatorType.Binary):
			_sourceText.Append(", ")
			_sourceText.Append(declarator.FirstParameterName)
			_sourceText.Append(" as ")
			_sourceText.Append(GetTypeString(declarator.FirstParameterType))
		_sourceText.Append(") as ")
		_sourceText.Append(GetTypeString(declarator.TypeReference))
		
		if (operatorDeclaration.Body != null):
			_sourceText.Append(":")
			AppendNewLine()
			AddIndentLevel()
			operatorDeclaration.Body.AcceptChildren(self, data)
			RemoveIndentLevel()
			AppendIndentation()
		AppendNewLine()
		return null
	
	override def Visit(indexerDeclaration as IndexerDeclaration, data):
		DebugOutput(indexerDeclaration)
		AppendAttributes(indexerDeclaration.Attributes)
		AppendIndentation()
		_sourceText.Append(GetModifier(indexerDeclaration.Modifier, Modifier.Public))
		_sourceText.Append("Indexer(")
		AppendParameters(indexerDeclaration.Parameters)
		_sourceText.Append(") as ")
		_sourceText.Append(GetTypeString(indexerDeclaration.TypeReference))
		_sourceText.Append(":")
		AppendNewLine()
		
		AddIndentLevel()
		if (indexerDeclaration.GetRegion != null):
			indexerDeclaration.GetRegion.AcceptVisitor(self, data)
		
		if (indexerDeclaration.SetRegion != null):
			indexerDeclaration.SetRegion.AcceptVisitor(self, data)
		
		RemoveIndentLevel()
		
		AppendIndentation()
		AppendNewLine()
		return null
	
	override def Visit(blockStatement as BlockStatement, data):
		DebugOutput(blockStatement)
		blockStatement.AcceptChildren(self, data)
		return null
	
	override def Visit(statementExpression as StatementExpression, data):
		DebugOutput(statementExpression)
		AppendIndentation()
		_sourceText.Append(statementExpression.Expression.AcceptVisitor(self, statementExpression).ToString())
		AppendNewLine()
		return null
	
	override def Visit(localVariableDeclaration as LocalVariableDeclaration, data):
		DebugOutput(localVariableDeclaration)
		for localVar as VariableDeclaration in localVariableDeclaration.Variables:
			if not _inmacro:
				AppendIndentation()
			_sourceText.Append(GetModifier(localVariableDeclaration.Modifier, Modifier.Private))
			_sourceText.Append(localVar.Name)
			if not _inmacro:
				_sourceText.Append(" as ")
				_sourceText.Append(GetTypeString(localVariableDeclaration.Type))
			if (localVar.Initializer != null):
				_sourceText.Append(" = ")
				_sourceText.Append(localVar.Initializer.AcceptVisitor(self, data).ToString())
			
			AppendNewLine()
		return null
	
	override def Visit(emptyStatement as EmptyStatement, data):
		DebugOutput(emptyStatement)
		AppendNewLine()
		return null
	
	override def Visit(returnStatement as ReturnStatement, data):
		DebugOutput(returnStatement)
		AppendIndentation()
		_sourceText.Append("return")
		if (returnStatement.ReturnExpression != null):
			_sourceText.Append(" ")
			_sourceText.Append(returnStatement.ReturnExpression.AcceptVisitor(self, data).ToString())
		AppendNewLine()
		return null
	
	override def Visit(ifStatement as IfStatement, data):
		DebugOutput(ifStatement)
		AppendIndentation() unless data isa IfElseStatement
		ie as InvocationExpression = GetEventHandlerRaise(ifStatement)
		
		if ie == null or data isa IfElseStatement:
			_sourceText.Append("if ")
			_sourceText.Append(ifStatement.Condition.AcceptVisitor(self, null).ToString())
			_sourceText.Append(":")
			AppendNewLine()
			
			AddIndentLevel()
			ifStatement.EmbeddedStatement.AcceptVisitor(self, null)
			RemoveIndentLevel()
			
			AppendIndentation()
			AppendNewLine()
		else:
			_sourceText.Append(ie.AcceptVisitor(self, null))
			AppendNewLine()
		return null
	
	override def Visit(ifElseStatement as IfElseStatement, data):
		DebugOutput(ifElseStatement)
		AppendIndentation() unless data isa IfElseStatement
		_sourceText.Append("if ")
		_sourceText.Append(ifElseStatement.Condition.AcceptVisitor(self, null).ToString())
		_sourceText.Append(":")
		AppendNewLine()
		
		AddIndentLevel()
		ifElseStatement.EmbeddedStatement.AcceptVisitor(self, null)
		RemoveIndentLevel()
		
		elseStatement = ifElseStatement.EmbeddedElseStatement
		if elseStatement isa IfStatement or elseStatement isa IfElseStatement:
			// convert to elif
			AppendIndentation()
			_sourceText.Append("el")
			elseStatement.AcceptVisitor(self, ifElseStatement)
		else:
			AppendIndentation()
			_sourceText.Append("else:")
			AppendNewLine()
			AddIndentLevel()
			elseStatement.AcceptVisitor(self, null)
			RemoveIndentLevel()
			AppendIndentation()
			AppendNewLine()
		
		return null
	
	override def Visit(whileStatement as WhileStatement, data):
		DebugOutput(whileStatement)
		AppendIndentation()
		_sourceText.Append("while ")
		_sourceText.Append(whileStatement.Condition.AcceptVisitor(self, data).ToString())
		_sourceText.Append(":")
		AppendNewLine()
		
		AddIndentLevel()
		whileStatement.EmbeddedStatement.AcceptVisitor(self, data)
		RemoveIndentLevel()
		
		AppendIndentation()
		AppendNewLine()
		return null
	
	override def Visit(doWhileStatement as DoWhileStatement, data):
		DebugOutput(doWhileStatement)
		AppendIndentation()
		_sourceText.Append("while true:")
		AppendNewLine()
		
		AddIndentLevel()
		doWhileStatement.EmbeddedStatement.AcceptVisitor(self, data)
		AppendIndentation()
		_sourceText.Append("break unless ")
		_sourceText.Append(doWhileStatement.Condition.AcceptVisitor(self, data).ToString())
		AppendNewLine()
		RemoveIndentLevel()
		
		AppendIndentation()
		AppendNewLine()
		return null
	
	override def Visit(forStatement as ForStatement, data):
		DebugOutput(forStatement)
		// TODO: Simplify simple for(int * = *; * < *, *++) statements
		// if you do so, do it also in the C#->VB.NET converter
		
		if forStatement.Initializers != null:
			for o in forStatement.Initializers:
				if (o isa Expression):
					expr as Expression = o
					AppendIndentation()
					_sourceText.Append(expr.AcceptVisitor(self, data).ToString())
					AppendNewLine()
				if (o isa Statement):
					cast(Statement, o).AcceptVisitor(self, data)
		
		AppendIndentation()
		_sourceText.Append("while ")
		if (forStatement.Condition == null):
			_sourceText.Append("true")
		else:
			_sourceText.Append(forStatement.Condition.AcceptVisitor(self, data).ToString())
		_sourceText.Append(":")
		AppendNewLine()
		
		AddIndentLevel()
		forStatement.EmbeddedStatement.AcceptVisitor(self, data)
		if (forStatement.Iterator != null):
			for stmt as Statement in forStatement.Iterator:
				stmt.AcceptVisitor(self, data)
		
		RemoveIndentLevel()
		
		AppendIndentation()
		AppendNewLine()
		return null
	
	override def Visit(labelStatement as LabelStatement, data):
		DebugOutput(labelStatement)
		AppendIndentation()
		_sourceText.Append(":")
		_sourceText.Append(labelStatement.Label)
		AppendNewLine()
		return null
	
	override def Visit(gotoStatement as GotoStatement, data):
		DebugOutput(gotoStatement)
		AppendIndentation()
		_sourceText.Append("goto ")
		_sourceText.Append(gotoStatement.Label)
		AppendNewLine()
		return null
	
	override def Visit(switchStatement as SwitchStatement, data):
		
		DebugOutput(switchStatement)
		/*
		AppendIndentation()
		_sourceText.Append("given ")
		_sourceText.Append(switchStatement.SwitchExpression.AcceptVisitor(self, data).ToString())
		_sourceText.Append(":")
		AppendNewLine()
		AddIndentLevel()
		for section as SwitchSection in switchStatement.SwitchSections:
			AppendIndentation()
			_sourceText.Append("when ")
			
			for i in range(section.SwitchLabels.Count):
				label as  Expression = section.SwitchLabels[i]
				if (label == null):
					_sourceText.Append("default")
				else:
					_sourceText.Append(label.AcceptVisitor(self, data))
					if (i + 1 < section.SwitchLabels.Count):
						_sourceText.Append(", ")
			_sourceText.Append(":")
			AppendNewLine()
			
			AddIndentLevel()
			section.AcceptVisitor(self, data)
			RemoveIndentLevel()
		RemoveIndentLevel()
		AppendIndentation()
		AppendNewLine()
		*/
		AppendIndentation()
		_sourceText.Append("selector = ")
		_sourceText.Append(switchStatement.SwitchExpression.AcceptVisitor(self, data).ToString())
		AppendNewLine()
		first = true
		for section as SwitchSection in switchStatement.SwitchSections:
			AppendIndentation()
			if first:
				first = false
			else:
				_sourceText.Append("el")
			if section.SwitchLabels.Count == 1 and section.SwitchLabels[0] == null:
				_sourceText.Append("se")
			else:
				_sourceText.Append("if ")
				for i in range(section.SwitchLabels.Count):
					label as Expression = section.SwitchLabels[i]
					if (label == null):
						_sourceText.Append("true")
					else:
						_sourceText.Append("selector == ")
						_sourceText.Append(label.AcceptVisitor(self, data))
						if i + 1 < section.SwitchLabels.Count:
							_sourceText.Append(" or ")
			_sourceText.Append(":")
			AppendNewLine()
			AddIndentLevel()
			section.AcceptVisitor(self, data)
			RemoveIndentLevel()
		AppendIndentation()
		AppendNewLine()
		return null
	
	override def Visit(breakStatement as BreakStatement, data):
		DebugOutput(breakStatement)
		AppendIndentation()
		_sourceText.Append("break")
		AppendNewLine()
		return null
	
	override def Visit(continueStatement as ContinueStatement, data):
		DebugOutput(continueStatement)
		AppendIndentation()
		_sourceText.Append("continue  // WARNING !!! Please check if the converter made an endless loop")
		AppendNewLine()
		return null
	
	override def Visit(gotoCaseStatement as GotoCaseStatement, data):
		DebugOutput(gotoCaseStatement)
		AppendIndentation()
		_sourceText.Append("goto case ")
		if (gotoCaseStatement.CaseExpression == null):
			_sourceText.Append("default")
		else:
			_sourceText.Append(gotoCaseStatement.CaseExpression.AcceptVisitor(self, data))
		AppendNewLine()
		return null
	
	override def Visit(foreachStatement as ForeachStatement, data):
		DebugOutput(foreachStatement)
		AppendIndentation()
		_sourceText.Append("for ")
		_sourceText.Append(foreachStatement.VariableName)
		_sourceText.Append(" as ")
		_sourceText.Append(self.GetTypeString(foreachStatement.TypeReference))
		_sourceText.Append(" in ")
		_sourceText.Append(foreachStatement.Expression.AcceptVisitor(self, data))
		_sourceText.Append(":")
		AppendNewLine()
		
		AddIndentLevel()
		foreachStatement.EmbeddedStatement.AcceptVisitor(self, data)
		RemoveIndentLevel()
		
		AppendIndentation()
		AppendNewLine()
		return null
	
	override def Visit(lockStatement as LockStatement, data):
		DebugOutput(lockStatement)
		AppendIndentation()
		_sourceText.Append("lock ")
		_sourceText.Append(lockStatement.LockExpression.AcceptVisitor(self, data))
		_sourceText.Append(":")
		AppendNewLine()
		
		AddIndentLevel()
		lockStatement.EmbeddedStatement.AcceptVisitor(self, data)
		RemoveIndentLevel()
		
		AppendIndentation()
		AppendNewLine()
		return null
	
	override def Visit(usingStatement as UsingStatement, data):
		DebugOutput(usingStatement)
		AppendIndentation()
		_sourceText.Append("using ")
		_inmacro = true
		usingStatement.UsingStmnt.AcceptVisitor(self, data)
		_inmacro = false
		//HACK: [DH] chopped off trailing newline (crlf) here:
		_sourceText.Remove(_sourceText.Length - 2,2)
		_sourceText.Append(":")
		AppendNewLine()
		
		AddIndentLevel()
		usingStatement.EmbeddedStatement.AcceptVisitor(self, data)
		RemoveIndentLevel()
		
		AppendIndentation()
		AppendNewLine()
		return null
	
	override def Visit(tryCatchStatement as TryCatchStatement, data):
		DebugOutput(tryCatchStatement)
		AppendIndentation()
		_sourceText.Append("try:")
		AppendNewLine()
		
		AddIndentLevel()
		tryCatchStatement.StatementBlock.AcceptVisitor(self, data)
		RemoveIndentLevel()
		
		if (tryCatchStatement.CatchClauses != null):
			generated = 0
			for catchClause as CatchClause in tryCatchStatement.CatchClauses:
				AppendIndentation()
				_sourceText.Append("except")
				if (catchClause.Type != null):
					_sourceText.Append(" ")
					if (catchClause.VariableName == null):
						_sourceText.Append("exception")
						if (tryCatchStatement.CatchClauses.Count > 1):
							_sourceText.Append(generated.ToString())
							generated += 1
					else:
						_sourceText.Append(catchClause.VariableName)
					_sourceText.Append(" as ")
					_sourceText.Append(catchClause.Type)
				
				_sourceText.Append(":")
				AppendNewLine()
				AddIndentLevel()
				catchClause.StatementBlock.AcceptVisitor(self, data)
				RemoveIndentLevel()
		
		if (tryCatchStatement.FinallyBlock != null):
			AppendIndentation()
			_sourceText.Append("ensure:")
			AppendNewLine()
			
			AddIndentLevel()
			tryCatchStatement.FinallyBlock.AcceptVisitor(self, data)
			RemoveIndentLevel()
		AppendIndentation()
		AppendNewLine()
		return null
	
	override def Visit(throwStatement as ThrowStatement, data):
		DebugOutput(throwStatement)
		AppendIndentation()
		_sourceText.Append("raise")
		if throwStatement.ThrowExpression != null:
			_sourceText.Append(" ")
			_sourceText.Append(throwStatement.ThrowExpression.AcceptVisitor(self, data).ToString())
		AppendNewLine()
		return null
	
	override def Visit(fixedStatement as FixedStatement, data):
		DebugOutput(fixedStatement)
		_errors.Error(-1, -1, "fixed statement not supported by Boo")
		return null
	
	override def Visit(checkedStatement as CheckedStatement, data):
		DebugOutput(checkedStatement)
		AppendIndentation()
		_sourceText.Append("checked:")
		AppendNewLine()
		
		AddIndentLevel()
		checkedStatement.Block.AcceptVisitor(self, data)
		RemoveIndentLevel()
		return null
	
	override def Visit(uncheckedStatement as UncheckedStatement, data):
		DebugOutput(uncheckedStatement)
		AppendIndentation()
		_sourceText.Append("unchecked:")
		AppendNewLine()
		
		AddIndentLevel()
		uncheckedStatement.Block.AcceptVisitor(self, data)
		RemoveIndentLevel()
		return null
	
	def ConvertCharLiteral(ch as Char):
		b = StringBuilder("Char.Parse('")
		ConvertChar(ch, b)
		b.Append("')")
		return b.ToString()
	
	def ConvertChar(ch as Char, b as StringBuilder):
		// TODO: Are there any more char literals in Boo?
		if ch == char('\n'):
			b.Append("\\n")
		elif ch == char('\r'):
			b.Append("\\r")
		elif ch == char('\0'):
			b.Append("\\0")
		elif ch == char('\a'):
			b.Append("\\a")
		elif ch == char('\b'):
			b.Append("\\b")
		elif ch == char('\f'):
			b.Append("\\f")
		elif ch == char('\''):
			b.Append("\\'")
		elif ch == char('\\'):
			b.Append("\\\\")
		elif char.IsControl(ch):
			// TODO: Is this possible in boo?
			b.Append("\\u")
			b.Append(cast(int, ch))
		else:
			b.Append(ch)
	
	def ConvertString(str as string):
		b = StringBuilder()
		for c as Char in str:
			ConvertChar(c, b)
		return b.ToString()
	
	override def Visit(primitiveExpression as PrimitiveExpression, data):
		DebugOutput(primitiveExpression)
		if (primitiveExpression.Value == null):
			return "null"
		if (primitiveExpression.Value isa bool):
			if cast(bool, primitiveExpression.Value):
				return "true"
			else:
				return "false"
		
		val = primitiveExpression.Value
		if val isa string:
			return "'" + ConvertString(val) + "'"
		
		if val isa Char:
			return ConvertCharLiteral(cast(Char, val))
		
		if val isa double:
			return cast(double, val).ToString(System.Globalization.CultureInfo.InvariantCulture)
		
		if val isa single:
			return cast(single, val).ToString(System.Globalization.CultureInfo.InvariantCulture)
		
		// TODO: How to express decimals in Boo?
		/*if (primitiveExpression.Value isa decimal) {
			return String.Concat(primitiveExpression.Value.ToString(), "D")
		}
		*/
		
		return primitiveExpression.Value
	
	override def Visit(binaryOperatorExpression as BinaryOperatorExpression, data):
		DebugOutput(binaryOperatorExpression)
		left = binaryOperatorExpression.Left.AcceptVisitor(self, data).ToString()
		right = binaryOperatorExpression.Right.AcceptVisitor(self, data).ToString()
		opType = binaryOperatorExpression.Op
		
		op = " " + opType.ToString() + " "
		
		op = " + "    if opType == BinaryOperatorType.Add
		op = " - "    if opType == BinaryOperatorType.Subtract
		op = " * "    if opType == BinaryOperatorType.Multiply
		op = " / "    if opType == BinaryOperatorType.Divide
		op = " % "    if opType == BinaryOperatorType.Modulus
		
		// TODO: Bitshift and XOR doesn't work
		op = " << "   if opType == BinaryOperatorType.ShiftLeft
		op = " >> "   if opType == BinaryOperatorType.ShiftRight
		op = " & "    if opType == BinaryOperatorType.BitwiseAnd
		op = " | "    if opType == BinaryOperatorType.BitwiseOr
		op = " ^ "    if opType == BinaryOperatorType.ExclusiveOr
		
		op = " and "  if opType == BinaryOperatorType.LogicalAnd
		op = " or "   if opType == BinaryOperatorType.LogicalOr
		
		op = " as "   if opType == BinaryOperatorType.AS
		op = " isa "  if opType == BinaryOperatorType.IS
		
		op = " != "   if opType == BinaryOperatorType.InEquality
		op = " == "   if opType == BinaryOperatorType.Equality
		op = " > "    if opType == BinaryOperatorType.GreaterThan
		op = " >= "   if opType == BinaryOperatorType.GreaterThanOrEqual
		op = " < "    if opType == BinaryOperatorType.LessThan
		op = " <= "   if opType == BinaryOperatorType.LessThanOrEqual
		
		return left + op + right
	
	override def Visit(parenthesizedExpression as ParenthesizedExpression, data):
		DebugOutput(parenthesizedExpression)
		innerExpr = parenthesizedExpression.Expression.AcceptVisitor(self, data).ToString()
		
		// parenthesized cast expressions evaluate to a single 'method call' and don't need
		// to be parenthesized anymore like in C#.
		// C# "((Control)sender).Visible = false;" -> "cast(Control, sender).Visible = false"
		if (parenthesizedExpression.Expression isa CastExpression):
			return innerExpr
		else:
			return "(" + innerExpr + ")"
	
	override def Visit(invocationExpression as InvocationExpression, data):
		DebugOutput(invocationExpression)
		target = invocationExpression.TargetObject.AcceptVisitor(self, data)
		return target + GetParameters(invocationExpression.Parameters)
	
	override def Visit(identifierExpression as IdentifierExpression, data):
		DebugOutput(identifierExpression)
		return identifierExpression.Identifier
	
	override def Visit(typeReferenceExpression as TypeReferenceExpression, data):
		DebugOutput(typeReferenceExpression)
		return GetTypeString(typeReferenceExpression.TypeReference)
	
	override def Visit(unaryOperatorExpression as UnaryOperatorExpression, data):
		DebugOutput(unaryOperatorExpression)
		expr = unaryOperatorExpression.Expression.AcceptVisitor(self, data).ToString()
		opType = unaryOperatorExpression.Op
		op = opType.ToString() + " "
		
		// TODO: Bitwise not operator
		op = "~"    if opType == UnaryOperatorType.BitNot
		op = "--"   if opType == UnaryOperatorType.Decrement
		op = "++"   if opType == UnaryOperatorType.Increment
		op = "-"    if opType == UnaryOperatorType.Minus
		op = "not " if opType == UnaryOperatorType.Not
		op = ""     if opType == UnaryOperatorType.Plus
		// include these though they are not supported by boo
		op = "*"    if opType == UnaryOperatorType.Star
		op = "&"    if opType == UnaryOperatorType.BitWiseAnd
		if opType == UnaryOperatorType.PostDecrement:
			return "Math.Max(${expr}, --${expr})"
		if opType == UnaryOperatorType.PostIncrement:
			return "Math.Min(${expr}, ++${expr})"
		return op + expr
	
	override def Visit(assignmentExpression as AssignmentExpression, data):
		DebugOutput(assignmentExpression)
		left = assignmentExpression.Left.AcceptVisitor(self, data).ToString()
		right = assignmentExpression.Right.AcceptVisitor(self, data).ToString()
		op as string = null
		opType = assignmentExpression.Op
		op = " = "    if opType == AssignmentOperatorType.Assign
		op = " += "   if opType == AssignmentOperatorType.Add
		op = " -= "   if opType == AssignmentOperatorType.Subtract
		op = " *= "   if opType == AssignmentOperatorType.Multiply
		op = " /= "   if opType == AssignmentOperatorType.Divide
		return left + op + right if op != null
		// TODO: Bitshift operators don't work
		op = " << "  if opType == AssignmentOperatorType.ShiftLeft
		op = " >> "  if opType == AssignmentOperatorType.ShiftRight
		op = " % "   if opType == AssignmentOperatorType.Modulus
		op = " ^ "   if opType == AssignmentOperatorType.ExclusiveOr
		op = " & "   if opType == AssignmentOperatorType.BitwiseAnd
		op = " | "   if opType == AssignmentOperatorType.BitwiseOr
		return left + " = " + left + op + right if op != null
		return left + " " + opType.ToString() + " " + right
	
	override def Visit(sizeOfExpression as SizeOfExpression, data):
		DebugOutput(sizeOfExpression)
		_errors.Error(-1, -1, "sizeof expression not supported by Boo")
		return null
	
	override def Visit(typeOfExpression as TypeOfExpression, data):
		DebugOutput(typeOfExpression)
		return "typeof(" + GetTypeString(typeOfExpression.TypeReference) + ")"
	
	override def Visit(checkedExpression as CheckedExpression, data):
		DebugOutput(checkedExpression)
		_errors.Error(-1, -1, "checked expression not supported by Boo")
		return null
	
	override def Visit(uncheckedExpression as UncheckedExpression, data):
		DebugOutput(uncheckedExpression)
		_errors.Error(-1, -1, "unchecked expression not supported by Boo")
		return null
	
	override def Visit(pointerReferenceExpression as PointerReferenceExpression, data):
		_errors.Error(-1, -1, "pointer reference (->) not supported by Boo")
		return ""
	
	override def Visit(castExpression as CastExpression, data):
		DebugOutput(castExpression)
		expression = castExpression.Expression.AcceptVisitor(self, data).ToString()
		targetType = GetTypeString(castExpression.CastTo)
		return "cast(${targetType}, ${expression})"
	
	override def Visit(stackAllocExpression as StackAllocExpression, data):
		_errors.Error(-1, -1, "stack alloc expression not supported by Boo")
		return ""
	
	override def Visit(indexerExpression as IndexerExpression, data):
		DebugOutput(indexerExpression)
		target = indexerExpression.TargetObject.AcceptVisitor(self, data)
		parameters = GetExpressionList(indexerExpression.Indices)
		return "${target}[${parameters}]"
	
	override def Visit(thisReferenceExpression as ThisReferenceExpression, data):
		DebugOutput(thisReferenceExpression)
		return "self"
	
	override def Visit(baseReferenceExpression as BaseReferenceExpression, data):
		DebugOutput(baseReferenceExpression)
		return "super"
	
	override def Visit(objectCreateExpression as ObjectCreateExpression, data):
		DebugOutput(objectCreateExpression)
		if (IsEventHandlerCreation(objectCreateExpression)):
			expr as Expression = objectCreateExpression.Parameters[0]
			if expr isa FieldReferenceExpression:
				return cast(FieldReferenceExpression, expr).FieldName
			else:
				return expr.AcceptVisitor(self, data).ToString()
		else:
			targetType = GetTypeString(objectCreateExpression.CreateType)
			parameters = GetParameters(objectCreateExpression.Parameters)
			return targetType + parameters
	
	override def Visit(ace as ArrayCreateExpression, data):
		DebugOutput(ace)
		
		if (ace.ArrayInitializer != null and ace.ArrayInitializer.CreateExpressions != null):
			return ace.ArrayInitializer.AcceptVisitor(self, data)
		
		if (ace.Parameters != null and ace.Parameters.Count > 0):
			b = StringBuilder("array(")
			for i in range(ace.Parameters.Count - 1):
				b.Append("(")
			b.Append(GetTypeString(ace.CreateType))
			for i in range(ace.Parameters.Count - 1):
				b.Append(")")
			b.Append(", ")
			b.Append(GetExpressionList(cast(ArrayCreationParameter, ace.Parameters[0]).Expressions))
			b.Append(")")
			return b.ToString()
		else:
			return "(,)"
	
	override def Visit(parameterDeclarationExpression as ParameterDeclarationExpression, data):
		// should never be called:
		raise NotImplementedException()
	
	override def Visit(fieldReferenceExpression as FieldReferenceExpression, data):
		DebugOutput(fieldReferenceExpression)
		target = fieldReferenceExpression.TargetObject.AcceptVisitor(self, data)
		return "${target}.${fieldReferenceExpression.FieldName}"
	
	override def Visit(directionExpression as DirectionExpression, data):
		DebugOutput(directionExpression)
		// there is nothing in a Boo method call for out & ref
		return directionExpression.Expression.AcceptVisitor(self, data)
	
	override def Visit(arrayInitializerExpression as ArrayInitializerExpression, data):
		b = StringBuilder("(")
		b.Append(GetExpressionList(arrayInitializerExpression.CreateExpressions))
		b.Append(",") if arrayInitializerExpression.CreateExpressions.Count < 2
		b.Append(")")
		return b.ToString()
	
	override def Visit(conditionalExpression as ConditionalExpression, data):
		// TODO: Implement IIF for Boo
		condition = conditionalExpression.TestCondition.AcceptVisitor(self, data).ToString()
		trueExpression = conditionalExpression.TrueExpression.AcceptVisitor(self, data).ToString()
		falseExpression = conditionalExpression.FalseExpression.AcceptVisitor(self, data).ToString()
		return "iif(${condition}, ${trueExpression}, ${falseExpression})"
	#endregion
	
	def ConvertTypeString(typeString as string):
		return "single" if typeString == "float"
		if typeString.StartsWith("System."):
			convertedType = BooAmbience.TypeConversionTable[typeString]
		else:
			convertedType = BooAmbience.TypeConversionTable["System." + typeString]
		return convertedType if convertedType != null
		return typeString
	
	def GetTypeString(typeRef as TypeReference):
		if (typeRef == null):
			_errors.Error(-1, -1, "Got empty type string (internal error, check generated source code for empty types")
			return "!Got empty type string!"
		
		b = StringBuilder()
		if (typeRef.RankSpecifier != null):
			for i in range(typeRef.RankSpecifier.Length):
				//   b.Append("(")
				// Emulate multidimensional arrays as jagged arrays
				for j in range(typeRef.RankSpecifier[i]):
					b.Append("(")
		
		b.Append(ConvertTypeString(typeRef.Type))
		
		if (typeRef.RankSpecifier != null):
			for i in range(typeRef.RankSpecifier.Length):
				for j in range(typeRef.RankSpecifier[i]):
					b.Append(")")
		
		if (typeRef.PointerNestingLevel > 0):
			// append stars so the problem is visible in the generated source code
			for i in range(typeRef.PointerNestingLevel):
				b.Append("*")
			_errors.Error(-1, -1, "Pointer types are not supported by Boo")
		return b.ToString()
	
	def GetModifier(modifier as Modifier, default as Modifier):
		builder = StringBuilder()
		// TODO: Check if modifiers are called like this in Boo
		if ((modifier & Modifier.Public) == Modifier.Public):
			builder.Append("public ")    if default != Modifier.Public
		elif ((modifier & (Modifier.Protected | Modifier.Internal)) == (Modifier.Protected | Modifier.Internal)):
			builder.Append("protected internal ")
		elif ((modifier & Modifier.Internal) == Modifier.Internal):
			builder.Append("internal ")  if default != Modifier.Internal
		elif ((modifier & Modifier.Protected) == Modifier.Protected):
			builder.Append("protected ") if default != Modifier.Protected
		elif ((modifier & Modifier.Private) == Modifier.Private):
			builder.Append("private ")   if default != Modifier.Private
		
		builder.Append("static ")    if (modifier & Modifier.Static)   == Modifier.Static
		builder.Append("virtual ")   if (modifier & Modifier.Virtual)  == Modifier.Virtual
		builder.Append("abstract ")  if (modifier & Modifier.Abstract) == Modifier.Abstract
		builder.Append("override ")  if (modifier & Modifier.Override) == Modifier.Override
		//builder.Append("")         if (modifier & Modifier.New)      == Modifier.New
		builder.Append("final ")     if (modifier & Modifier.Sealed)   == Modifier.Sealed
		builder.Append("final ")  if (modifier & Modifier.Readonly) == Modifier.Readonly
		builder.Append("final ")     if (modifier & Modifier.Const)    == Modifier.Const
		builder.Append("extern ")    if (modifier & Modifier.Extern)   == Modifier.Extern
		builder.Append("volatile ")  if (modifier & Modifier.Volatile) == Modifier.Volatile
		builder.Append("unsafe ")    if (modifier & Modifier.Unsafe)   == Modifier.Unsafe
		
		if ((modifier & Modifier.Volatile) == Modifier.Volatile):
			_errors.Error(-1, -1, "'volatile' modifier not convertable")
		if ((modifier & Modifier.Unsafe) == Modifier.Unsafe):
			_errors.Error(-1, -1, "'unsafe' modifier not convertable")
		return builder.ToString()
	
	def GetParameters(l as ArrayList):
		return "(" + GetExpressionList(l) + ")"
	
	def GetExpressionList(l as ArrayList):
		if (l == null):
			return ""
		sb = StringBuilder()
		for exp as Expression in l:
			if sb.Length > 0:
				sb.Append(", ")
			sb.Append(exp.AcceptVisitor(self, null))
		return sb.ToString()
	
	def AppendParameters(parameters as ArrayList):
		return if parameters == null
		first = true
		for pde as ParameterDeclarationExpression in parameters:
			if first:
				first = false
			else:
				_sourceText.Append(", ")
			AppendAttributes(pde.Attributes)
			
			_sourceText.Append("ref ")    if pde.ParamModifiers == ParamModifiers.Ref
			_sourceText.Append("out ")    if pde.ParamModifiers == ParamModifiers.Out
			_sourceText.Append("*")       if pde.ParamModifiers == ParamModifiers.Params
			
			_sourceText.Append(pde.ParameterName)
			_sourceText.Append(" as ")
			_sourceText.Append(GetTypeString(pde.TypeReference))
	
	def AppendAttributes(attr as ArrayList):
		return if attr == null
		for section as AttributeSection in attr:
			section.AcceptVisitor(self, null)
	
	def GetEventHandlerRaise(ifStatement as IfStatement) as InvocationExpression:
		op = ifStatement.Condition as BinaryOperatorExpression
		if (op != null and op.Op == BinaryOperatorType.InEquality):
			if op.Left isa IdentifierExpression and op.Right isa PrimitiveExpression and (cast(PrimitiveExpression,op.Right).Value == null):
				identifier as string = cast(IdentifierExpression,op.Left).Identifier
				se as StatementExpression = null
				if (ifStatement.EmbeddedStatement isa StatementExpression):
					se = ifStatement.EmbeddedStatement
				elif (ifStatement.EmbeddedStatement.Children.Count == 1):
					se = ifStatement.EmbeddedStatement.Children[0] as StatementExpression
				
				if se != null:
					ie = se.Expression as InvocationExpression
					if ie != null:
						ex as Expression = ie.TargetObject
						methodName as string = null
						if (ex isa IdentifierExpression):
							methodName = cast(IdentifierExpression,ex).Identifier
						elif (ex isa FieldReferenceExpression):
							fre as FieldReferenceExpression = ex
							if (fre.TargetObject isa ThisReferenceExpression):
								methodName = fre.FieldName
						
						if methodName != null and methodName == identifier:
							for o in _currentType.Children:
								ed = o as EventDeclaration
								if ed != null:
									if (ed.Name == methodName):
										return ie
									for field as VariableDeclaration in ed.VariableDeclarators:
										if (field.Name == methodName):
											return ie
		return null
	
	def IsEventHandlerCreation(expr as Expression):
		if (expr isa ObjectCreateExpression):
			oce as ObjectCreateExpression = expr
			if (oce.Parameters.Count == 1):
				expr = oce.Parameters[0]
				methodName as string = null
				if (expr isa IdentifierExpression):
					methodName = cast(IdentifierExpression,expr).Identifier
				elif (expr isa FieldReferenceExpression):
					methodName = cast(FieldReferenceExpression,expr).FieldName
				
				if (methodName != null):
					for o in _currentType.Children:
						if (o isa MethodDeclaration and cast(MethodDeclaration,o).Name == methodName):
							return true
		return false;
