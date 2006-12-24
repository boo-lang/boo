#region license
// Copyright (c) 2003, 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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


namespace TracePipeline

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps

class TracePipelineStep(AbstractVisitorCompilerStep):
"""
	Visits every method adding a trace statement at both its very
	beginning and end.
"""	
	override def Run():
		Visit(CompileUnit)
		
	override def LeaveMethod(method as Method):
		stmt = TryStatement()
		stmt.ProtectedBlock = method.Body
		stmt.ProtectedBlock.Insert(0,
				MethodStart("TRACE: Entering ${method.FullName}"))
		stmt.EnsureBlock = MethodEnd("TRACE: Leaving ${method.FullName}")
		method.Body = Block()
		method.Body.Add(stmt)
		
	def MethodStart(msg as string):
		// { print(msg)
		block = Block()
		mie = MethodInvocationExpression(ReferenceExpression("print"))
		mie.Arguments.Add(StringLiteralExpression(msg))
		block.Add(mie)
		//   __start = date.Now }
		dateNow = MemberReferenceExpression(
								ReferenceExpression("date"),
								"Now")
		block.Add(
			BinaryExpression(BinaryOperatorType.Assign,
			ReferenceExpression("__start"), dateNow))
		return block
		
	def MethodEnd(msg as string):
		// { __time = date.Now - __start
		block = Block()
		dateNow = MemberReferenceExpression(
							ReferenceExpression("date"),
							"Now")
		block.Add(BinaryExpression(BinaryOperatorType.Assign,
			ReferenceExpression("__time"),
			BinaryExpression(BinaryOperatorType.Subtraction,
				dateNow,
				ReferenceExpression("__start"))))
		//   print(msg + ": " + __time) }
		msgTime = BinaryExpression(BinaryOperatorType.Addition,
			StringLiteralExpression(msg + ": "),
			ReferenceExpression("__time"))
		mie = MethodInvocationExpression(ReferenceExpression("print"))
		mie.Arguments.Add(msgTime)
		block.Add(mie)
		return block
		

class TracePipeline(CompileToFile):
	
	def constructor():
		self.Insert(1, TracePipelineStep())
