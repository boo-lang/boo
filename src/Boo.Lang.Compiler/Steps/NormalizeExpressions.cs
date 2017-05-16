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

namespace Boo.Lang.Compiler.Steps
{
	using Boo.Lang.Compiler.Ast;

	public class NormalizeExpressions : AbstractTransformerCompilerStep
	{
		Method _current;

		override public void Run()
		{
			Visit(CompileUnit);
		}

		override public void OnMethod(Method node)
		{
			_current = node;
			Visit(node.Body);
		}

		override public void OnConstructor(Constructor node)
		{
			OnMethod(node);
		}

		override public void OnDestructor(Destructor node)
		{
			OnMethod(node);
		}

		public override void OnCollectionInitializationExpression(CollectionInitializationExpression node)
		{
			var temp = new ReferenceExpression(node.LexicalInfo, Context.GetUniqueName("collection"));

			var initialization = CodeBuilder.CreateEvalInvocation(node.LexicalInfo);

			// temp = $(node.Collection)
			initialization.Arguments.Add(new BinaryExpression(BinaryOperatorType.Assign, temp, node.Collection));

			if (node.Initializer is ListLiteralExpression)
				foreach (var item in ((ListLiteralExpression)node.Initializer).Items)
					// temp.Add(item)
					initialization.Arguments.Add(NewAddInvocation(item.LexicalInfo, temp.CloneNode(), item));
			else
				foreach (var pair in ((HashLiteralExpression)node.Initializer).Items)
					// temp.Add(key, value)
					initialization.Arguments.Add(NewAddInvocation(pair.LexicalInfo, temp.CloneNode(), pair.First, pair.Second));

			// return temp
			initialization.Arguments.Add(temp.CloneNode());

			ReplaceCurrentNode(initialization);
		}

		private static MethodInvocationExpression NewAddInvocation(LexicalInfo location, ReferenceExpression target, params Expression[] args)
		{
			return new MethodInvocationExpression(location, new MemberReferenceExpression(target.CloneNode(), "Add"), args);
		}

		override public void OnMemberReferenceExpression(MemberReferenceExpression node)
		{
			if (node.Target.NodeType != NodeType.OmittedExpression)
				return;

			if (_current.IsStatic)
				node.Target = new ReferenceExpression(node.Target.LexicalInfo, _current.DeclaringType.Name);
			else
				node.Target = new SelfLiteralExpression(node.Target.LexicalInfo);
		}
	}
}

