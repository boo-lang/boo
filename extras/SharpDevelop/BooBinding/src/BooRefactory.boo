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

class BooRefactory:
	// This class can refactor the c# parse tree before converting to boo
	// to simplify some things
	
	// NOTE: While writing this, I discovered many bugs in SharpRefactory I worked around
	// by overriding the buggy methods.
	// Since SharpRefactory will be replaced by NRefactory soon, the bugs won't be fixed
	// in SharpRefactory but only in NRefactory (but many bugs don't exist in NRefactory anyhow)
	
	def Refactor(compilationUnit as CompilationUnit):
		RefactorNamespaces(compilationUnit)
		compilationUnit.AcceptChildren(RefactoryVisitor(), null);
	
	def RefactorNamespaces(compilationUnit as CompilationUnit):
		namespacecount = CountNamespaces(compilationUnit.Children)
		if namespacecount == 1:
			// put the namespace definition to the beginning
			for o in compilationUnit.Children:
				n = o as NamespaceDeclaration
				if n != null:
					// get using definitions into the namespace
					num = 0
					i = 0
					while i < compilationUnit.Children.Count:
						child = compilationUnit.Children[i]
						print("i = " + i.ToString() + "; child =" + child.ToString())
						if child isa UsingDeclaration or child isa UsingAliasDeclaration:
							compilationUnit.Children.RemoveAt(i)
							i -= 1
							n.Children.Insert(num, child)
							num += 1
						i += 1
					return
	
	def CountNamespaces(l as ArrayList) as int:
		count as int = 0
		for o in l:
			n = o as NamespaceDeclaration
			if n != null:
				count += 1 + CountNamespaces(n.Children)
		return count

class RefactoryVisitor(AbstractASTVisitor):
	override def Visit(typeDeclaration as TypeDeclaration, data):
		// create DefaultMemberAttribute for indexers
		if typeDeclaration.Type == Types.Class and HasIndexer(typeDeclaration.Children):
			expression = PrimitiveExpression('Indexer', 'Indexer')
			attribute = ICSharpCode.SharpRefactory.Parser.AST.Attribute("DefaultMember", MakeArray(expression), ArrayList())
			typeDeclaration.Attributes.Add(AttributeSection(null, MakeArray(attribute)))
		
		field as FieldDeclaration = null
		removed = ArrayList() // list of all elements that should be removed from typeDeclaration
		
		// prefix fields with underscore
		fields = ArrayList()
		for o in typeDeclaration.Children:
			field = o as FieldDeclaration
			if field != null:
				for var as VariableDeclaration in field.Fields:
					if var.Name.Length < 2 or not var.Name[1:2] == "_":
						if Char.IsLower(var.Name, 0) and IsPrivate(field.Modifier):
							fields.Add(var)
		typeDeclaration.AcceptChildren(RenameFieldVisitor(fields), data)
		for var as VariableDeclaration in fields:
			var.Name = "_" + var.Name
		
		// convert Getter/Setter into attributes if possible
		// -> therefore create a new field list (containing the fields, not VarDecs)
		fieldHash = Hashtable()
		for o in typeDeclaration.Children:
			field = o as FieldDeclaration
			if field != null:
				if field.Fields.Count == 1:
					fieldHash[cast(VariableDeclaration, field.Fields[0]).Name] = field
		// now look for properties
		for o in typeDeclaration.Children:
			property = o as PropertyDeclaration
			if property != null and property.Modifier == Modifier.Public:
				ok = true
				field = null
				// check if the property has correct getters/setters
				if property.HasGetRegion:
					block = property.GetRegion.Block
					if block != null and block.Children.Count == 1:
						child = block.Children[0] as ReturnStatement
						if child != null:
							retExpr = child.ReturnExpression as IdentifierExpression
							if retExpr != null:
								field = fieldHash[retExpr.Identifier]
					ok = false if field == null
				if ok and property.HasSetRegion:
					ok = false
					setterField as FieldDeclaration = null
					block = property.SetRegion.Block
					if block != null and block.Children.Count == 1:
						childStatement = block.Children[0] as StatementExpression
						if child != null:
							expr = childStatement.Expression as AssignmentExpression
							if expr != null and expr.Op == AssignmentOperatorType.Assign:
								leftExpr = expr.Left as IdentifierExpression
								rightExpr = expr.Right as IdentifierExpression
								if leftExpr != null and rightExpr != null and rightExpr.Identifier == "value":
									setterField = fieldHash[leftExpr.Identifier]
					ok = false if setterField == null or setterField != field
				
				
				if ok and property.HasGetRegion:
					removed.Add(o)
					print("Replacing property ${property.Name}")
					identifier = IdentifierExpression(property.Name)
					attributes = ArrayList()
					if property.HasSetRegion:
						attributes.Add(ICSharpCode.SharpRefactory.Parser.AST.Attribute("Property", MakeArray(identifier), ArrayList()))
					else:
						attributes.Add(ICSharpCode.SharpRefactory.Parser.AST.Attribute("Getter", MakeArray(identifier), ArrayList()))
					field.Attributes.Add(AttributeSection(null, attributes))
		
		for o in removed:
			typeDeclaration.Children.Remove(o)
		
		return super(typeDeclaration, data)
	
	def IsPrivate(m as Modifier) as bool:
		// fields without any modifier are also private
		return false if (m & Modifier.Public) == Modifier.Public
		return false if (m & Modifier.Protected) == Modifier.Protected
		return false if (m & Modifier.Internal) == Modifier.Internal
		return true
	
	override def Visit(statementExpression as StatementExpression, data):
		expr = statementExpression.Expression as UnaryOperatorExpression
		if expr != null:
			// found unary operator as single statement
			expr.Op = UnaryOperatorType.Increment if expr.Op == UnaryOperatorType.PostIncrement
			expr.Op = UnaryOperatorType.Decrement if expr.Op == UnaryOperatorType.PostDecrement
		
		return super(statementExpression, data)
	
	override def Visit(localVariableDeclaration as LocalVariableDeclaration, data):
		/* Check for this structure:
		[LocalVariableDeclaration: Type=[TypeReference: Type=EmptyClass, PointerNestingLevel=0, RankSpecifier=System.Int32[]], Modifier =None
		Variables={[VariableDeclaration: Name=c, Initializer=[CastExpression:
			CastTo=[TypeReference: Type=EmptyClass, PointerNestingLevel=0, RankSpecifier=System.Int32[]],
			Expression=***]]}]
		*/
		if localVariableDeclaration.Variables.Count == 1:
			var as VariableDeclaration = localVariableDeclaration.Variables[0]
			castExpr = var.Initializer as CastExpression
			if castExpr != null:
				if castExpr.CastTo.Type == localVariableDeclaration.Type.Type:
					// remove redundant cast
					var.Initializer = castExpr.Expression
		
		return super(localVariableDeclaration, data)
	
	override def Visit(blockStatement as BlockStatement, data):
		// work around SharpRefactory bug (that won't be fixed before NRefactory)
		switchSection = blockStatement as SwitchSection
		return Visit(switchSection, data) if switchSection != null
		return super(blockStatement, data)
	
	override def Visit(switchSection as SwitchSection, data):
		if switchSection.Children != null and switchSection.Children.Count > 0:
			lastNum = switchSection.Children.Count - 1
			lastChild = switchSection.Children[lastNum]
			if lastChild isa BreakStatement:
				switchSection.Children.RemoveAt(lastNum)
		
		// can't use super because of another SharpRefactory bug
		for label as Expression in switchSection.SwitchLabels:
			label.AcceptVisitor(self, data) if label != null
		return switchSection.AcceptChildren(self, data)
	
	def HasIndexer(l as ArrayList) as bool:
		for o in l:
			return true if o isa IndexerDeclaration
		return false
	
	def MakeArray(val as object) as ArrayList:
		a = ArrayList()
		a.Add(val)
		return a
	

class RenameFieldVisitor(AbstractASTVisitor):
	_fields as ArrayList
	_curBlock = ArrayList()
	_blocks = Stack()
	
	def constructor(fields as ArrayList):
		_fields = fields
	
	override def Visit(typeDeclaration as TypeDeclaration, data):
		// ignore sub-types
		return null
	
	override def Visit(blockStatement as BlockStatement, data):
		Push()
		result = super(blockStatement, data)
		Pop()
		return result
	
	override def Visit(methodDeclaration as MethodDeclaration, data):
		Push()
		result = super(methodDeclaration, data)
		Pop()
		return result
	
	override def Visit(constructorDeclaration as ConstructorDeclaration, data):
		Push()
		result = super(constructorDeclaration, data)
		Pop()
		return result
	
	private def Push():
		//print("PUSH block")
		_blocks.Push(_curBlock)
		_curBlock = ArrayList()
	
	private def Pop():
		_curBlock = _blocks.Pop()
		//print("POP block")
	
	override def Visit(localVariableDeclaration as LocalVariableDeclaration, data):
		for decl as VariableDeclaration in localVariableDeclaration.Variables:
			//print("add variable ${decl.Name} to block")
			_curBlock.Add(decl.Name)
		return super(localVariableDeclaration, data)
	
	override def Visit(parameterDeclarationExpression as ParameterDeclarationExpression, data):
		_curBlock.Add(parameterDeclarationExpression.ParameterName)
		//print("add parameter ${parameterDeclarationExpression.ParameterName} to block")
		return super(parameterDeclarationExpression, data)
	
	override def Visit(identifierExpression as IdentifierExpression, data):
		name = identifierExpression.Identifier
		for var as VariableDeclaration in _fields:
			if var.Name == name and not IsLocal(name):
				identifierExpression.Identifier = "_" + name
				return null
		return super(identifierExpression, data)
	
	override def Visit(fieldReferenceExpression as FieldReferenceExpression, data):
		if fieldReferenceExpression.TargetObject isa ThisReferenceExpression:
			name = fieldReferenceExpression.FieldName
			for var as VariableDeclaration in _fields:
				if var.Name == fieldReferenceExpression.FieldName:
					fieldReferenceExpression.FieldName = "_" + name
					return null
		return super(fieldReferenceExpression, data)
	
	def IsLocal(name as string) as bool:
		for block as ArrayList in _blocks:
			for n as string in block:
				return true if name == n
		for n as string in _curBlock:
			return true if name == n
		return false
	
	override def Visit(invocationExpression as InvocationExpression, data):
		// this method is a workaround for a bug in SharpRefactory
		result = data
		if invocationExpression.TargetObject != null:
			result = invocationExpression.TargetObject.AcceptVisitor(self, data)
		if invocationExpression.Parameters != null:
			for n as INode in invocationExpression.Parameters:
				n.AcceptVisitor(self, data)
		return result
	
	override def Visit(indexerExpression as IndexerExpression, data):
		// this method is a workaround for a bug in SharpRefactory
		result = indexerExpression.TargetObject.AcceptVisitor(self, data)
		for n as INode in indexerExpression.Indices:
			n.AcceptVisitor(self, data)
		return result
	
