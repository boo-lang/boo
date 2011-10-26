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
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem.Services
{	
	internal class AnonymousCallablesManager
	{
		private readonly TypeSystemServices _tss;
		private readonly IDictionary<CallableSignature, AnonymousCallableType> _cache = new Dictionary<CallableSignature, AnonymousCallableType>();
		
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

			var mapping = CreateGenericParametersMappingForCallableType(anonymousType);
			if (mapping.Count > 0)
			{
				cd.GenericParameters = GenericParameterDeclarationCollection.FromArray(mapping.Values.ToArray());
				anonymousType = MapAnonymousType(anonymousType, mapping);
			}

			cd.Annotate(AnonymousCallableTypeAnnotation);
			cd.Modifiers |= TypeMemberModifiers.Public;
			cd.LexicalInfo = sourceNode.LexicalInfo;

			cd.Members.Add(CreateInvokeMethod(anonymousType));

			Method beginInvoke = CreateBeginInvokeMethod(anonymousType);
			cd.Members.Add(beginInvoke);

			cd.Members.Add(CreateEndInvokeMethod(anonymousType));
			module.Members.Add(cd);
            
			return (IType)cd.Entity;
		}

		private AnonymousCallableType MapAnonymousType(AnonymousCallableType anonymousType, Dictionary<InternalGenericParameter, GenericParameterDeclaration> mapping)
		{
			var mapper = new SimpleTypeMapper(mapping.Keys.ToArray(), Array.ConvertAll(mapping.Values.ToArray(), gpd => ((InternalGenericParameter) gpd.Entity).Type));
			var anonymousTypeSignatue = anonymousType.GetSignature();
			
			var newSignature = new CallableSignature(MapParameters(mapper, anonymousTypeSignatue.Parameters),
					mapper.MapType(anonymousTypeSignatue.ReturnType),
					anonymousTypeSignatue.AcceptVarArgs);

			var result = new AnonymousCallableType(TypeSystemServices, newSignature);
			return result;
		}

		private IParameter[] MapParameters(SimpleTypeMapper mapper, IEnumerable<IParameter> parameters)
		{
			var result = new IParameter[parameters.Count()];
			var i = 0;
			foreach (var parameter in parameters)
			{
				var mappedType = mapper.MapType(parameter.Type);
				var pd = CodeBuilder.CreateParameterDeclaration(i, "arg" + (i + 1), mappedType, parameter.IsByRef);
				result[i] = new InternalParameter(pd, i);
				i++;
			}
			return result;
		}

		private Dictionary<InternalGenericParameter, GenericParameterDeclaration> CreateGenericParametersMappingForCallableType(AnonymousCallableType anonymousType)
		{
			var mapping = new Dictionary<InternalGenericParameter, GenericParameterDeclaration>();
			var collector = new TypeCollector(t => t is InternalGenericParameter);

			collector.Visit(anonymousType);
			var i = 1;
			foreach (var igp in collector.Matches)
			{
				if (mapping.ContainsKey((InternalGenericParameter) igp)) continue;

				var gdp = new GenericParameterDeclaration("T" + i);
				gdp.Entity = new InternalGenericParameter(TypeSystemServices, gdp, i);
				mapping.Add((InternalGenericParameter) igp, gdp);
				i++;
			}
			return mapping;
		}

		private static string GenerateCallableTypeNameFrom(Node sourceNode, Module module)
		{
			var enclosing =
				(sourceNode.GetAncestor(NodeType.ClassDefinition) ??
				 sourceNode.GetAncestor(NodeType.InterfaceDefinition) ??
				 sourceNode.GetAncestor(NodeType.EnumDefinition) ?? sourceNode.GetAncestor(NodeType.Module)) as TypeMember;
			string prefix = "";
			string postfix = "";
			if (enclosing != null)
			{
				prefix += enclosing.Name;
				enclosing = (sourceNode.GetAncestor(NodeType.Method)
				             ?? sourceNode.GetAncestor(NodeType.Property)
				             ?? sourceNode.GetAncestor(NodeType.Event)
				             ?? sourceNode.GetAncestor(NodeType.Field)) as TypeMember;
				if (enclosing != null)
				{
					prefix += "_" + enclosing.Name;
				}
				prefix += "$";
			}
			else if (!sourceNode.LexicalInfo.Equals(LexicalInfo.Empty))
			{
				prefix += Path.GetFileNameWithoutExtension(sourceNode.LexicalInfo.FileName) + "$";
			}
			if (!sourceNode.LexicalInfo.Equals(LexicalInfo.Empty))
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

	/// <summary>
	/// Maps types, substituting type arguments for generic parameters using "IGenericParameter to IType" mapping.
	/// </summary>
	internal class SimpleTypeMapper : TypeMapper
	{
		readonly IDictionary<IGenericParameter, IType> _map = new Dictionary<IGenericParameter, IType>();

		/// <summary>
		/// Constrcuts a new GenericMapping for a specific mapping of generic parameters to type arguments.
		/// </summary>
		/// <param name="parameters">The generic parameters that should be mapped.</param>
		/// <param name="arguments">The type arguments to map generic parameters to.</param>
		public SimpleTypeMapper(IGenericParameter[] parameters, IType[] arguments)
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				_map.Add(parameters[i], arguments[i]);
			}
		}

		/// <summary>
		/// Maps a type involving generic parameters to the corresponding type after substituting concrete
		/// arguments for generic parameters.
		/// </summary>
		/// <remarks>
		/// If the source type is a generic parameter, it is mapped to the corresponding argument.
		/// </remarks>
		override public IType MapType(IType sourceType)
		{
			var gp = sourceType as IGenericParameter;
			if (gp != null)
			{
				// Map type parameters declared on our source
				if (_map.ContainsKey(gp)) return _map[gp];
			}
			return base.MapType(sourceType);
		}
	}

}

