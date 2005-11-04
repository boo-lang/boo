#region license
// Copyright (c) 2005 Arron Washington (l33ts0n@gmail.com)
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
//     * Neither the name of Arron Washington nor the names of its
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
namespace Boo.Lang.Useful.Attributes

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class AutoFlagsAttribute(AbstractAstAttribute):
"""
Automatically increment enum values using base 2.
Makes bitflags a snap, but beware: 
assign your own values with care!
By default, AutoFlags starts with 1 - you can make a "none" enum by specifying it directly:
enum Ninjas:
	None = 0
	Black #assigned 1
	White #assigned 2
	Grey = Black | White #assigned 3, 1 + 2
"""
	override def Apply(node as Node):
		unless node isa EnumDefinition:
			InvalidNodeForAttribute("Enum")
			return
		#Set the values of the enum members to base 2 values.
		enumDef = node as EnumDefinition
		members = array(EnumMember, enumDef.Members.Select(NodeType.EnumMember))
		lastVal = 1
		for member in members:
			continue unless member.Initializer is null
			member.Initializer = IntegerLiteralExpression(lastVal)
			lastVal = 2 * lastVal		
		#Finally, apply the 'Flags' attribute - for reflection / interop with other languages.		
		enumDef.Attributes.Add(Attribute("System.FlagsAttribute"))