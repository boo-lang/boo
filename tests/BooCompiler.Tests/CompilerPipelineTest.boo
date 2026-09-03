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

namespace BooCompiler.Tests

import NUnit.Framework
import Boo.Lang
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Steps

[TestFixture]
class CompilerPipelineTest:
	[Test]
	def EventSequence():
		calls = Boo.Lang.List[of string]()
		pipeline = CompilerPipeline()
		pipeline.Before += { sender, args | calls.Add("before") }
		pipeline.BeforeStep += { sender, args | calls.Add("before step") }
		pipeline.Add(ActionStep({ calls.Add("step") }))
		pipeline.AfterStep += { sender, args | calls.Add("after step") }
		pipeline.After += { sender, args | calls.Add("after") }
		pipeline.Run(CompilerContext())
		Assert.AreEqual(
			("before", "before step", "step", "after step", "after"),
			calls.ToArray())

	[Test]
	def CurrentStep():
		pipeline = CompilerPipeline()

		step1 = ActionStep(DoNothing)
		pipeline.Add(step1)

		step2 as ActionStep
		step2 = ActionStep({ Assert.AreSame(step2, pipeline.CurrentStep) })
		pipeline.Add(step2)

		currentSteps = Boo.Lang.List()
		pipeline.Before += { sender, args | currentSteps.Add(pipeline.CurrentStep) }
		pipeline.BeforeStep += { sender, args | currentSteps.Add(pipeline.CurrentStep) }
		pipeline.AfterStep += { sender, args | currentSteps.Add(pipeline.CurrentStep) }
		pipeline.After += { sender, args | currentSteps.Add(pipeline.CurrentStep) }

		pipeline.Run(CompilerContext())

		Assert.AreEqual(
			(null, step1, step1, step2, step2, null),
			currentSteps.ToArray())

	private def DoNothing():
		pass

	[Test]
	def PipelineIsEmptyByDefault():
		Assert.AreEqual(0, CompilerPipeline().Count)

	[Test]
	def ExecutionOrder():
		order = Boo.Lang.List[of string]()
		p1 = ActionStep({ order.Add("p1") })
		p2 = ActionStep({ order.Add("p2") })

		pipeline = CompilerPipeline()
		pipeline.Add(p1)
		pipeline.Add(p2)
		pipeline.Run(CompilerContext())

		Assert.AreEqual(("p1", "p2"), order.ToArray())
