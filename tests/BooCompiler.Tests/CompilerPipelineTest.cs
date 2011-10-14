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

using NUnit.Framework;
using Boo.Lang;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Steps;

namespace BooCompiler.Tests
{
	/// <summary>	
	/// </summary>
	[TestFixture]
	public class CompilerPipelineTest
	{	
		[Test]
		public void EventSequence()
		{
			var calls = new List<string>();
			var pipeline = new CompilerPipeline();
			pipeline.Before += delegate { calls.Add("before"); };
			pipeline.BeforeStep += delegate { calls.Add("before step"); };
			pipeline.Add(new ActionStep(() => calls.Add("step")));
			pipeline.AfterStep += delegate { calls.Add("after step"); };
			pipeline.After += delegate { calls.Add("after"); };
			pipeline.Run(new CompilerContext());
			Assert.AreEqual(
				new string[] {"before", "before step", "step", "after step", "after"},
				calls.ToArray());
		}

		[Test]
		public void CurrentStep()
		{
			var pipeline = new CompilerPipeline();

			var step1 = new ActionStep(delegate {});
			pipeline.Add(step1);

			ActionStep step2 = null;
			step2 = new ActionStep(() => Assert.AreSame(step2, pipeline.CurrentStep));
			pipeline.Add(step2);

			var currentSteps = new Boo.Lang.List();
			pipeline.Before += (sender, args) => currentSteps.Add(pipeline.CurrentStep);
			pipeline.BeforeStep += (sender, args) => currentSteps.Add(pipeline.CurrentStep);
			pipeline.AfterStep += (sender, args) => currentSteps.Add(pipeline.CurrentStep);
			pipeline.After += (sender, args) => currentSteps.Add(pipeline.CurrentStep);

			pipeline.Run(new CompilerContext());

			Assert.AreEqual(
				new object[] { null, step1, step1, step2, step2, null },
				currentSteps.ToArray());
		}

		[Test]
		public void PipelineIsEmptyByDefault()
		{
			Assert.AreEqual(0, new CompilerPipeline().Count);
		}

		[Test]
		public void ExecutionOrder()
		{
			var order = new List<string>();
			var p1 = new ActionStep(() => order.Add("p1"));
			var p2 = new ActionStep(() => order.Add("p2"));

			var pipeline = new CompilerPipeline { p1, p2 };
			pipeline.Run(new CompilerContext());

			Assert.AreEqual(new[] { "p1", "p2" }, order.ToArray());
		}
	}
}
