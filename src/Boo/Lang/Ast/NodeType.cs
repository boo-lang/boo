﻿#region license
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

//
// DO NOT EDIT THIS FILE!
//
// This file was generated automatically by
// astgenerator.boo on 2/27/2004 1:11:12 AM
//

namespace Boo.Lang.Ast
{
	using System;
	
	[Serializable]
	public enum NodeType
	{
		CompileUnit, 
		SimpleTypeReference, 
		TupleTypeReference, 
		NamespaceDeclaration, 
		Import, 
		Module, 
		ClassDefinition, 
		InterfaceDefinition, 
		EnumDefinition, 
		EnumMember, 
		Field, 
		Property, 
		Local, 
		Method, 
		Constructor, 
		ParameterDeclaration, 
		Declaration, 
		Attribute, 
		StatementModifier, 
		Block, 
		DeclarationStatement, 
		AssertStatement, 
		MacroStatement, 
		TryStatement, 
		ExceptionHandler, 
		IfStatement, 
		UnlessStatement, 
		ForStatement, 
		WhileStatement, 
		GivenStatement, 
		WhenClause, 
		BreakStatement, 
		ContinueStatement, 
		RetryStatement, 
		ReturnStatement, 
		YieldStatement, 
		RaiseStatement, 
		UnpackStatement, 
		ExpressionStatement, 
		OmittedExpression, 
		ExpressionPair, 
		MethodInvocationExpression, 
		UnaryExpression, 
		BinaryExpression, 
		TernaryExpression, 
		ReferenceExpression, 
		MemberReferenceExpression, 
		StringLiteralExpression, 
		TimeSpanLiteralExpression, 
		IntegerLiteralExpression, 
		DoubleLiteralExpression, 
		NullLiteralExpression, 
		SelfLiteralExpression, 
		SuperLiteralExpression, 
		BoolLiteralExpression, 
		RELiteralExpression, 
		StringFormattingExpression, 
		HashLiteralExpression, 
		ListLiteralExpression, 
		TupleLiteralExpression, 
		IteratorExpression, 
		SlicingExpression, 
		AsExpression
	}
}