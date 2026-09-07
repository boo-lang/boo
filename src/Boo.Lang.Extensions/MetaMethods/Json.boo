#region license
// Copyright (c) 2009, Rodrigo B. de Oliveira (rbo@acm.org)
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

import System.Linq.Enumerable
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Environments

[Meta(ResolveArgs: true)]
def Json(value as Expression) as Expression:
"""
Writes a Boo literal as the System.Text.Json tree it describes.

A hash literal becomes a JsonObject and a list literal a JsonArray. A node
built elsewhere is taken as it stands, and anything else converts itself.
"""
	return JsonObj(value) if value isa HashLiteralExpression
	return JsonArr(value) if value isa ListLiteralExpression
	return value if IsNode(value)
	return [| System.Text.Json.Nodes.JsonValue.Create($value) |].withLexicalInfoFrom(value)

private def IsNode(value as Expression) as bool:
	# JsonValue.Create takes a value, not a node.
	type = value.ExpressionType
	return false if type is null
	return my(TypeSystemServices).Map(typeof(System.Text.Json.Nodes.JsonNode)).IsAssignableFrom(type)

private def JsonArr(value as ListLiteralExpression) as Expression:
	# JsonArray takes its items as a params array, so they are its arguments.
	call as MethodInvocationExpression = [| System.Text.Json.Nodes.JsonArray() |].withLexicalInfoFrom(value)
	call.Arguments.AddRange(value.Items.Select(Json))
	return call

private def JsonObj(value as HashLiteralExpression) as Expression:
	# JsonObject takes one IEnumerable of pairs, not a pair per argument.
	if value.Items.Count == 0:
		empty = [| array(System.Collections.Generic.KeyValuePair[of string, System.Text.Json.Nodes.JsonNode], 0) |]
		return [| System.Text.Json.Nodes.JsonObject($empty) |].withLexicalInfoFrom(value)

	pairs = ArrayLiteralExpression()
	pairs.Items.AddRange(value.Items.Select(JsonPair))
	return [| System.Text.Json.Nodes.JsonObject($pairs) |].withLexicalInfoFrom(value)

private def JsonPair(pair as ExpressionPair) as Expression:
	if pair.First isa LiteralExpression and not pair.First isa StringLiteralExpression:
		# What a meta method raises reaches the reader as an internal error.
		my(CompilerErrorCollection).Add(
			CompilerErrorFactory.CustomError(pair.First, "A JSON key has to be a string."))
		return [| null |].withLexicalInfoFrom(pair)
	written = Json(pair.Second)
	return [| System.Collections.Generic.KeyValuePair[of string, System.Text.Json.Nodes.JsonNode]($(pair.First), $written) |].withLexicalInfoFrom(pair)
