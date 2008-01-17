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

namespace Boo.Tests.Lang

import NUnit.Framework
import System
import System.Globalization
import Boo.Lang.Runtime

[TestFixture]
class RuntimeServicesTestFixture:

	static IC = CultureInfo.InvariantCulture
	
	static CNP = RuntimeServices.CheckNumericPromotion

	[Test]
	def CheckNumericPromotion():
		assert 3 == CNP(3).ToInt32(IC)
		assert 1024L == CNP(1024).ToInt64(IC)
		
		assert true == CNP(3).ToBoolean(IC)
		assert false == CNP(0).ToBoolean(IC)
		
		assert 0 == CNP(false).ToInt32(IC)
		assert 1 == CNP(true).ToInt32(IC)
		
	[Test]
	[ExpectedException(NullReferenceException)]
	def CheckNumericPromotionWithNull():
		CNP(null)
		
	[Test]
	[ExpectedException(InvalidCastException)]
	def CheckNumericPromotionWithString():
		CNP("")
		
	[Test]
	[ExpectedException(InvalidCastException)]
	def CheckNumericPromotionWithDate():
		CNP(date.Now)
			
	class Foo:
		def bar():
			raise ApplicationException()
		
