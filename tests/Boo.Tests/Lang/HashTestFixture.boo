#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Tests

import NUnit.Framework

[TestFixture]
class HashTestFixture:
	
	[Test]
	def CreateHashFromEnumerable():
		h = Hash((i, i*2) for i in range(5))
		Assert.AreEqual(5, len(h))
		Assert.AreEqual([0, 1, 2, 3, 4], List(h.Keys).Sort())
		Assert.AreEqual([0, 2, 4, 6, 8], List(h.Values).Sort())
		
		for i in range(5):
			Assert.AreEqual(i*2, h[i])
