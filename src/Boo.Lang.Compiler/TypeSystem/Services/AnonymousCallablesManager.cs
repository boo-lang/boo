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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem.Services
{	
	internal class AnonymousCallablesManager
	{
		private TypeSystemServices _tss;
		private IDictionary<CallableSignature, AnonymousCallableType> _cache = new Dictionary<CallableSignature, AnonymousCallableType>();
		
		public AnonymousCallablesManager(TypeSystemServices tss)
		{
			_tss = tss;
		}

		protected TypeSystemServices TypeSystemServices
		{
			get { return _tss; }
		}

		protected BooCodeBuilder CodeBuilder
		{
			get { return _tss.CodeBuilder; }
		}

		public ICallableType GetCallableType(CallableSignature signature)
		{
			AnonymousCallableType type = GetCachedCallableType(signature);
			if (type == null)
			{
				type = new AnonymousCallableType(TypeSystemServices, signature);
				_cache.Add(signature, type);
			}
			return type;
		}

		private AnonymousCallableType GetCachedCallableType(CallableSignature signature)
		{
			AnonymousCallableType result;
			_cache.TryGetValue(signature, out result);
			return result;
		}

		public IType GetConcreteCallableType(Node sourceNode, CallableSignature signature)
		{
			var type = GetCallableType(signature);
			var anonymous = type as AnonymousCallableType;
			return anonymous != null ? GetConcreteCallableType(sourceNode, anonymous) : type;
		}

		public IType GetConcreteCallableType(Node sourceNode, AnonymousCallableType anonymousType)
		{
			return anonymousType.ConcreteType ??
			       (anonymousType.ConcreteType = CreateConcreteCallableType(sourceNode, anonymousType));
		}

		private IType CreateConcreteCallableType(Node sourceNode, AnonymousCallableType anonymousType)
		{
			var module = TypeSystemServices.GetCompilerGeneratedTypesModule();
			
			var name = GenerateCallableTypeNameFrom(sourceNode, module);

			ClassDefinition cd = My<CallableTypeBuilder>.Instance.CreateEmptyCallableDefinition(name);

			cd.Annotate(AnonymousCallableTypeAnnotation);
			cd.Modifiers |= TypeMemberModifiers.Public;
			cd.LexicalInfo = sourceNode.LexicalInfo;

			cd.Members.Add(CreateInvokeMethod(anonymousType));

			Method beginInvoke = CreateBeginInvokeMethod(anonymousType);
			cd.Members.Add(beginInvoke);

			cd.Members.Add(CreateEndInvokeMethod(anonymousType));
			AddGenericTypes(cd, sourceNode.NodeType != NodeType.BlockExpression);
			module.Members.Add(cd);
			return (IType)cd.Entity;
		}

		private void AddGenericTypes(ClassDefinition cd, bool adaptInnerGenerics)
		{
			var collector = new GenericTypeCollector(this.CodeBuilder);
			collector.Process(cd);
		    if (!adaptInnerGenerics) return;

			var counter = cd.GenericParameters.Count;
			var innerCollector = new DetectInnerGenerics();
			cd.Accept(innerCollector);
			foreach (Node node in innerCollector.Values)
			{
				var param = (IGenericParameter)node.Entity;
				var gp = cd.GenericParameters.FirstOrDefault(gpd => gpd.Name.Equals(param.Name));
				if (gp == null)
				{
					gp = CodeBuilder.CreateGenericParameterDeclaration(counter, param.Name);
					cd.GenericParameters.Add(gp);
					++counter;
				}
				node.Entity = gp.Entity;
				gp["InternalGenericParent"] = (param as InternalGenericParameter).Node;
			}
		}

		private string GenerateCallableTypeNameFrom(Node sourceNode, Module module)
		{
			var enclosing = (sourceNode.GetAncestor(NodeType.ClassDefinition) ?? sourceNode.GetAncestor(NodeType.InterfaceDefinition) ?? sourceNode.GetAncestor(NodeType.EnumDefinition) ?? sourceNode.GetAncestor(NodeType.Module)) as TypeMember;
			string prefix = "";
			string postfix = "";
			if(enclosing != null)
			{
				prefix += enclosing.Name;
				enclosing = (sourceNode.GetAncestor(NodeType.Method) 
				             ?? sourceNode.GetAncestor(NodeType.Property) 
				             ?? sourceNode.GetAncestor(NodeType.Event) 
				             ?? sourceNode.GetAncestor(NodeType.Field)) as TypeMember;
				if(enclosing != null)
				{
					prefix += "_" + enclosing.Name;
				}
				prefix += "$";
			}
			else if (!sourceNode.LexicalInfo.Equals(LexicalInfo.Empty))
			{
				prefix += Path.GetFileNameWithoutExtension(sourceNode.LexicalInfo.FileName) + "$";
			}
			if(!sourceNode.LexicalInfo.Equals(LexicalInfo.Empty))
			{
				postfix = "$" + sourceNode.LexicalInfo.Line + "_" + sourceNode.LexicalInfo.Column + postfix;
			}
			return "__" + prefix + "callable" + module.Members.Count + postfix + "__";
		}

		Method CreateBeginInvokeMethod(ICallableType anonymousType)
		{
			Method method = CodeBuilder.CreateRuntimeMethod("BeginInvoke", TypeSystemServices.Map(typeof(IAsyncResult)),
															anonymousType.GetSignature().Parameters, false);

			int delta = method.Parameters.Count;
			method.Parameters.Add(
					CodeBuilder.CreateParameterDeclaration(delta + 1, "callback", TypeSystemServices.Map(typeof(AsyncCallback))));
			method.Parameters.Add(
					CodeBuilder.CreateParameterDeclaration(delta + 1, "asyncState", TypeSystemServices.ObjectType));
			return method;
		}

		public Method CreateEndInvokeMethod(ICallableType anonymousType)
		{
			CallableSignature signature = anonymousType.GetSignature();
			Method method = CodeBuilder.CreateRuntimeMethod("EndInvoke", signature.ReturnType);

			int delta = 1;
			foreach (IParameter p in signature.Parameters)
			{
				if (p.IsByRef)
				{
					method.Parameters.Add(
						CodeBuilder.CreateParameterDeclaration(++delta,
								p.Name,
								p.Type,
								true));
				}
			}
			delta = method.Parameters.Count;
			method.Parameters.Add(
				CodeBuilder.CreateParameterDeclaration(delta + 1, "result", TypeSystemServices.Map(typeof(IAsyncResult))));
			return method;
		}

		Method CreateInvokeMethod(AnonymousCallableType anonymousType)
		{
			CallableSignature signature = anonymousType.GetSignature();
			return CodeBuilder.CreateRuntimeMethod("Invoke", signature.ReturnType, signature.Parameters, signature.AcceptVarArgs);
		}

		public static readonly object AnonymousCallableTypeAnnotation = new object();
	}
}

