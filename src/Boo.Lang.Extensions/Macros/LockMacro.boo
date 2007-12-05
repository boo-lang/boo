#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.Lang.Extensions

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast


public class LockMacro(AbstractAstMacro):

	public static final MonitorLocalName = '__monitor{0}__'
	
	private static final Monitor_Enter as Expression = AstUtil.CreateReferenceExpression('System.Threading.Monitor.Enter')

	private static final Monitor_Exit as Expression = AstUtil.CreateReferenceExpression('System.Threading.Monitor.Exit')
	
	override def Expand(macro as MacroStatement):
		if 0 == macro.Arguments.Count:
			raise Boo.Lang.Compiler.CompilerErrorFactory.InvalidLockMacroArguments(macro)
		
		resulting as Block = macro.Block
		args as ExpressionCollection = macro.Arguments
		for i in range(args.Count, 0, -1):
			arg as Expression = args[(i - 1)]
			
			resulting = CreateLockedBlock(arg, resulting)
		
		return resulting
	
	private def CreateMonitorReference(lexicalInfo as LexicalInfo) as ReferenceExpression:
		localIndex as int = _context.AllocIndex()
		return ReferenceExpression(lexicalInfo, string.Format(MonitorLocalName, localIndex))
	
	internal def CreateLockedBlock(monitor as Expression, body as Block) as Block:
		monitorReference as ReferenceExpression = CreateMonitorReference(monitor.LexicalInfo)
		
		block = Block(body.LexicalInfo)
		
		// __monitorN__ = <expression>
		block.Add(BinaryExpression(BinaryOperatorType.Assign, monitorReference, monitor))
		
		// System.Threading.Monitor.Enter(__monitorN__)			
		block.Add(AstUtil.CreateMethodInvocationExpression(Monitor_Enter, monitorReference))
		
		// try:			
		// 		<the rest>
		// ensure:
		//		Monitor.Leave			
		stmt = TryStatement()
		stmt.ProtectedBlock = body
		stmt.EnsureBlock = Block()
		stmt.EnsureBlock.Add(AstUtil.CreateMethodInvocationExpression(Monitor_Exit, monitorReference))
		
		block.Add(stmt)
		
		return block

