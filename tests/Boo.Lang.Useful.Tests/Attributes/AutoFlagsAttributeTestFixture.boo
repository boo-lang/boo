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
namespace Boo.Lang.Useful.Tests.Attributes

import Boo.Lang.Useful.Attributes
import NUnit.Framework

[TestFixture]
class AutoFlagsAttributeTestFixture:
	
	[Test]
	def BitwiseOperations():
		var = Ninja.Grey
		assert var & Ninja.White != Ninja.None
		assert var & Ninja.Black != Ninja.None
		assert var & Ninja.Grey != Ninja.None
		assert var & Ninja.None == Ninja.None
		var = Ninja.Black | Ninja.Stylish
		assert var & Ninja.Black != Ninja.None
		assert var & Ninja.Stylish != Ninja.None
		assert var & Ninja.White == Ninja.None
		
	[Test]
	def FlagsAttribute():
		assert System.Attribute.IsDefined(Ninja, System.FlagsAttribute)
		
[AutoFlags]
enum Ninja:
	None = 0
	White
	Black
	Grey = 3 # White | Black
	Stylish