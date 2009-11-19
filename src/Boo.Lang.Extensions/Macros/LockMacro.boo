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



def createLockedBlock(context as CompilerContext, monitor as Expression, block as Block):
	temp = ReferenceExpression(monitor.LexicalInfo, context.GetUniqueName("lock", "monitor"))

	expireString as string = null
	expire as int = 5000 #expiration time in ms (default 5s)
	if context.Parameters.Defines.TryGetValue("LOCK_TIMEOUT", expireString):
		expire = int.Parse(expireString) if expireString
		_acquired = ReferenceExpression(monitor.LexicalInfo, context.GetUniqueName("lock", "acquired"))
		monitorEntry = [|
			block:
				$(_acquired) = System.Threading.Monitor.TryEnter($temp, $expire)
				if not $(_acquired):
					raise System.TimeoutException(string.Format("Lock at '{0}' could not be acquired within LOCK_TIMEOUT({1}ms) - possible deadlock", $(monitor.LexicalInfo.ToString()), $expire))
		|].Body
	else:
		monitorEntry = [|
			block:
				System.Threading.Monitor.Enter($temp)
		|].Body

	assignment = [| $temp = $monitor |].withLexicalInfoFrom(monitor)
	return [|
		$assignment
		$monitorEntry
		try:
			$block
		ensure:
			System.Threading.Monitor.Exit($temp)
	|]


macro lock:
	if 0 == len(lock.Arguments):
		raise CompilerErrorFactory.InvalidLockMacroArguments(lock)

	expansion = lock.Body
	for arg in reversed(lock.Arguments):
		expansion = createLockedBlock(Context, arg, expansion)
	return expansion

