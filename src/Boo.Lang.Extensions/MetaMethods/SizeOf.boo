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

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Services

[meta]
def sizeof(e as Expression):
	ue = e as UnaryExpression
	e = ue.Operand if ue and ue.Operator == UnaryOperatorType.Indirection

	re = e as ReferenceExpression
	if not re:
		goto invalid

	entity = my(NameResolutionService).Resolve(re.Name, EntityType.Type | EntityType.Local)
	if not entity:
		goto invalid
	if entity.EntityType == EntityType.Local:
		entity = cast(InternalLocal, entity).Type

	if entity.EntityType == EntityType.Type or entity.EntityType == EntityType.Array:
		type = entity as IType
		size = my(TypeSystemServices).SizeOf(type)

	if not size:
		my(CompilerErrorCollection).Add(CompilerErrorFactory.PointerIncompatibleType(e, type))
		return IntegerLiteralExpression(0)

	return IntegerLiteralExpression(size)

	:invalid
	my(CompilerErrorCollection).Add(CompilerErrorFactory.NameNotType(e, e.ToCodeString(), null, null));
	return IntegerLiteralExpression(0)

