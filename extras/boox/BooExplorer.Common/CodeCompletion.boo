#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// This file is part of Boo Explorer.
//
// Boo Explorer is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Boo Explorer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

namespace BooExplorer.Common

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem
import System.IO

class CodeCompletionHunter(ProcessMethodBodiesWithDuckTyping):
	
	static def GetCompletion(source as string):
		
		hunter = CodeCompletionHunter()
		compiler = BooCompiler()		
		compiler.Parameters.OutputWriter = StringWriter()
		compiler.Parameters.Pipeline = MakePipeline(hunter)
		compiler.Parameters.Input.Add(StringInput("none", source))
		result = compiler.Run()
		print(result.Errors.ToString(true))
		
		return hunter.Members

	[getter(Members)]
	_members = array(IEntity, 0)
	
	override protected def ProcessMemberReferenceExpression(node as MemberReferenceExpression):
		if node.Name == '__codecomplete__':
			_members = TypeSystemServices.GetAllMembers(MyGetReferenceNamespace(node))
		else:
			super(node)
		
	protected def MyGetReferenceNamespace(expression as MemberReferenceExpression) as INamespace:		
		target as Expression = expression.Target
		if target.ExpressionType is not null:
			if target.ExpressionType.EntityType != EntityType.Error:
				return cast(INamespace, target.ExpressionType)
		return cast(INamespace, TypeSystemServices.GetOptionalEntity(target))
	
	protected static def MakePipeline(hunter):
		pipeline = ResolveExpressions(BreakOnErrors: false)
		index = pipeline.Find(Boo.Lang.Compiler.Steps.ProcessMethodBodiesWithDuckTyping)
		pipeline[index] = hunter
		return pipeline
		
