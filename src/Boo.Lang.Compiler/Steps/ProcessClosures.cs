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
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class ProcessClosures : AbstractTransformerCompilerStep
	{
		Method _currentMethod;
		
		override public void Run()
		{
			if (0 == Errors.Count)
			{
				Visit(CompileUnit);
			}
		}
		
		override public void OnMethod(Method node)
		{
			_currentMethod = node;
			Visit(node.Body);
		}
		
		override public void OnCallableBlockExpression(CallableBlockExpression node)
		{
			InternalMethod closureEntity = (InternalMethod)GetEntity(node);
			ReplaceCurrentNode(CreateClosureReference(closureEntity));
		}
		
		Expression CreateClosureReference(InternalMethod closure)
		{
			using (ForeignReferenceCollector collector = new ForeignReferenceCollector())
			{
				collector.ForeignMethod = _currentMethod;
				collector.Initialize(_context);
				collector.Visit(closure.Method.Body);
				
				if (collector.ContainsForeignLocalReferences)
				{	
					return CreateClosureClass(collector, closure);					
				}
			}
			return CodeBuilder.CreateMemberReference(closure);
		}
		
		Expression CreateClosureClass(ForeignReferenceCollector collector, InternalMethod closure)
		{
			Method method = closure.Method;
			TypeDefinition parent = method.DeclaringType;
			parent.Members.Remove(method);
			
			BooClassBuilder builder = collector.CreateSkeletonClass(method.Name);					
			builder.ClassDefinition.Members.Add(method);			
			method.Name = "Invoke";			
			parent.Members.Add(builder.ClassDefinition);	
			
			if (method.IsStatic)
			{	
				// need to adjust paremeter indexes (parameter 0 is now self)
				foreach (ParameterDeclaration parameter in method.Parameters)
				{
					((InternalParameter)parameter.Entity).Index += 1;
				}
			}
			
			method.Modifiers = TypeMemberModifiers.Public;
			
			collector.AdjustReferences();
			return CodeBuilder.CreateMemberReference(
					collector.CreateConstructorInvocationWithReferencedEntities(
							builder.Entity),
					closure);
		}
	}
}
