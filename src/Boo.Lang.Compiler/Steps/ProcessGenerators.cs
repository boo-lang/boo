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

using Boo.Lang.Compiler.Steps.Generators;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class ProcessGenerators : AbstractTransformerCompilerStep
	{
		public static System.Reflection.ConstructorInfo List_IEnumerableConstructor = Types.List.GetConstructor(new Type[] { Types.IEnumerable });
		
		Method _current;
		
		override public void Run()
		{
            if (Errors.Count > 0) return;
			Visit(CompileUnit.Modules);
		}
		
		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{
			// ignore
		}
		
		override public void OnEnumDefinition(EnumDefinition node)
		{
			// ignore
		}
		
		override public void OnField(Field node)
		{
			// ignore
		}
		
		override public void OnConstructor(Constructor method)
		{
			_current = method;
			Visit(_current.Body);
		}
		
		override public bool EnterMethod(Method method)
		{
			_current = method;
			return true;
		}
		
		override public void LeaveMethod(Method method)
		{
			InternalMethod entity = (InternalMethod)method.Entity;
			if (!entity.IsGenerator) return;
			
			GeneratorMethodProcessor processor = new GeneratorMethodProcessor(_context, entity);
			processor.Run();
		}

		override public void OnListLiteralExpression(ListLiteralExpression node)
		{
			bool generator = AstUtil.IsListGenerator(node);
			Visit(node.Items);
			if (generator)
			{
				ReplaceCurrentNode(
					CodeBuilder.CreateConstructorInvocation(
						TypeSystemServices.Map(List_IEnumerableConstructor),
						node.Items[0]));
			}
		}
		
		override public void LeaveGeneratorExpression(GeneratorExpression node)
		{
			using (ForeignReferenceCollector collector = new ForeignReferenceCollector())
			{
				collector.CurrentType = (IType)AstUtil.GetParentClass(node).Entity;
				collector.Initialize(_context);
				collector.Visit(node);

				GeneratorExpressionProcessor processor = new GeneratorExpressionProcessor(_context, collector, node);
				processor.Run();
				ReplaceCurrentNode(processor.CreateEnumerableConstructorInvocation());
			}
		}
	}
}
