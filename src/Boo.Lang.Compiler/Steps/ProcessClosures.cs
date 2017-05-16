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

using System.Diagnostics;
using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps.Generators;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps
{
	
	public class ProcessClosures : AbstractTransformerCompilerStep
	{
        public override void Run()
		{
			Visit(CompileUnit);
		}

	    public override void OnAsyncBlockExpression(AsyncBlockExpression node)
	    {
	        var result = Visit(node.Block);
	        ReplaceCurrentNode(result);
	    }

        private GeneratorTypeReplacer _mapper;

        public override void LeaveBlockExpression(BlockExpression node)
		{
			var closureEntity = GetEntity(node) as InternalMethod;
			if (closureEntity == null)
				return;

			var collector = new ForeignReferenceCollector();
			{
				collector.CurrentMethod = closureEntity.Method;
				collector.CurrentType = closureEntity.DeclaringType;
				closureEntity.Method.Body.Accept(collector);
				
				if (collector.ContainsForeignLocalReferences)
				{
					BooClassBuilder closureClass = CreateClosureClass(collector, closureEntity);
					if (closureEntity is InternalGenericMethod)
						closureEntity = GetEntity(closureEntity.Method) as InternalMethod;
					closureClass.ClassDefinition.LexicalInfo = node.LexicalInfo;
					collector.AdjustReferences();

                    if (_mapper != null)
                    {
                        closureClass.ClassDefinition.Accept(new GenericTypeMapper(_mapper));
                    }

                    ReplaceCurrentNode(
						CodeBuilder.CreateMemberReference(
							collector.CreateConstructorInvocationWithReferencedEntities(
								closureClass.Entity,
                                node.GetAncestor<Method>()),
							closureEntity));
				}
				else
				{
					_mapper = closureEntity.Method["GenericMapper"] as GeneratorTypeReplacer;
					if (_mapper != null)
						closureEntity.Method.Accept(new GenericTypeMapper(_mapper));
					IMethod entity = closureEntity;
					if (entity.GenericInfo != null)
					{
						entity = MapGenericMethod(entity, node.GetAncestor<Method>().GenericParameters);
					}
					Expression expression = CodeBuilder.CreateMemberReference(entity);
					expression.LexicalInfo = node.LexicalInfo;
					TypeSystemServices.GetConcreteExpressionType(expression);
					ReplaceCurrentNode(expression);
				}
			}
		}

		private static IMethod MapGenericMethod(IMethod method, GenericParameterDeclarationCollection genericArgs)
		{
			var args = method.GenericInfo.GenericParameters
				.Select(gp => (IType)genericArgs.First(ga => ga.Name == gp.Name).Entity).ToArray();
			return method.GenericInfo.ConstructMethod(args);
		}

		BooClassBuilder CreateClosureClass(ForeignReferenceCollector collector, InternalMethod closure)
		{
			Method method = closure.Method;
			TypeDefinition parent = method.DeclaringType;
			parent.Members.Remove(method);
			_mapper = method["GenericMapper"] as GeneratorTypeReplacer;
			
			BooClassBuilder builder = collector.CreateSkeletonClass(closure.Name, method.LexicalInfo);
			parent.Members.Add(builder.ClassDefinition);
			builder.ClassDefinition.Members.Add(method);
			method.Name = "Invoke";
			
			if (method.IsStatic)
			{
				// need to adjust paremeter indexes (parameter 0 is now self)
				foreach (ParameterDeclaration parameter in method.Parameters)
				{
					((InternalParameter)parameter.Entity).Index += 1;
				}
			}
			
			method.Modifiers = TypeMemberModifiers.Public;
			var coll = new GenericTypeCollector(CodeBuilder);
			coll.Process(builder.ClassDefinition);
			if (!method.GenericParameters.IsEmpty)
			{
				MapMethodGenerics(builder, method);
			}
			if (builder.ClassDefinition.GenericParameters.Count > 0)
                MapGenerics(builder.ClassDefinition);
			return builder;
		}

		private void MapMethodGenerics(BooClassBuilder builder, Method method)
		{
			Debug.Assert(_mapper != null);
			var classParams = builder.ClassDefinition.GenericParameters;
			foreach (var genParam in method.GenericParameters)
			{
				var replacement = classParams.FirstOrDefault(p => p.Name.Equals(genParam.Name));
				if (replacement != null && genParam != replacement.Entity)
					_mapper.Replace((IType) genParam.Entity, (IType) replacement.Entity);
			}
			method.GenericParameters.Clear();
			method.Entity = new InternalMethod(Environments.My<InternalTypeSystemProvider>.Instance, method);
		}

		private void MapGenerics(ClassDefinition cd)
	    {
	        var finder = new GenericTypeFinder();
	        foreach (var member in cd.Members)
                member.Accept(finder);

            _mapper = _mapper ?? new GeneratorTypeReplacer();
            var genParams = cd.GenericParameters;
	        foreach (var genType in finder.Results)
	        {
	            var replacement = genParams.FirstOrDefault(p => p.Name.Equals(genType.Name));
                if (replacement != null && genType != replacement.Entity)
                    _mapper.Replace(genType, (IType)replacement.Entity);
	        }
	    }
	}
}
