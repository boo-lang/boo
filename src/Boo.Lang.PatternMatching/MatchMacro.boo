#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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


namespace Boo.Lang.PatternMatching

import Boo.Lang.PatternMatching.Impl

# TODO: check for unreacheable patterns
macro match:
"""
Pattern matching facility:

	match <expression>:
		case <Pattern1>:
			<block1>
			.
			.
			.
		case <PatternN>:
			<blockN>
		otherwise:
			<blockOtherwise>

The following patterns are supported:
    
    Type() -- type test pattern
    Type(Property1: Pattern1, ...) -- object pattern
    Pattern1 | Pattern2 -- either pattern
	Pattern1 & Pattern2 -- both pattern (NOT IMPLEMENTED)
    Pattern1 and condition -- constrained pattern  (NOT IMPLEMENTED)
    Pattern1 or condition -- constrained pattern  (NOT IMPLEMENTED)
    (Pattern1, Pattern2) -- fixed size iteration pattern
    [Pattern1, Pattern2] -- arbitrary size iteration pattern (NOT IMPLEMENTED)
    x = Pattern -- variable binding
    x -- variable binding
    BinaryOperatorType.Assign -- constant pattern
    42 -- constant pattern
    "42" -- constant pattern
    null -- null test pattern
    
If no pattern matches MatchError is raised.
"""
	macro case:
		caseListFor(match).Add(case)

	macro otherwise:
		otherwiseNode = match["otherwise"] as Boo.Lang.Compiler.Ast.Node
		assert otherwiseNode is null, "`otherwise' is already defined at: ${otherwiseNode.LexicalInfo}"
		match["otherwise"] = otherwise

	assert 0 == len(match.Body.Statements), "Only `case' or `otherwise' are allowed in `match'. Offending statement at: ${match.Body.Statements[0].LexicalInfo}"
	assert 0 != len(caseListFor(match)), "`match' must contain at least one `case'"
	return MatchExpansion(Context, match).Value

