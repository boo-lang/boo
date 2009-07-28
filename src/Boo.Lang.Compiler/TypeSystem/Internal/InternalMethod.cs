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

using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Core;

namespace Boo.Lang.Compiler.TypeSystem.Internal
{
	public class InternalMethod : InternalEntity<Method>, IMethod, INamespace
	{	
		protected InternalTypeSystemProvider _provider;

		protected IMethod _override;

		protected ExpressionCollection _returnExpressions;

		protected List _yieldStatements;

		private bool? _isBooExtension;
		private bool? _isClrExtension;

		internal InternalMethod(InternalTypeSystemProvider provider, Method method) : base(method)
		{
			_provider = provider;
		}

		public bool IsExtension
		{
			get { return IsBooExtension || IsClrExtension; }
		}

		public bool IsBooExtension
		{
			get
			{
				if (null == _isBooExtension)
				{
					_isBooExtension = IsAttributeDefined(Types.BooExtensionAttribute);
				}
				return _isBooExtension.Value;
			}
		}

		public bool IsClrExtension
		{
			get
			{
				if (null == _isClrExtension)
				{
					_isClrExtension = MetadataUtil.HasClrExtensions()
					                  && IsAttributeDefined(Types.ClrExtensionAttribute);
				}
				return _isClrExtension.Value;
			}
		}

		public bool IsDuckTyped
		{
			get
			{
				return My<TypeSystemServices>.Instance.IsDuckType(ReturnType);
			}
		}
		
		public bool IsPInvoke
		{
			get
			{
				return IsAttributeDefined(Types.DllImportAttribute);
			}
		}
		
		private bool IsAttributeDefined(System.Type attributeType)
		{
			return IsDefined(My<TypeSystemServices>.Instance.Map(attributeType));
		}

		public bool IsAbstract
		{
			get
			{
				return _node.IsAbstract;
			}
		}
		
		public bool IsVirtual
		{
			get
			{
				return _node.IsVirtual
				       || _node.IsAbstract
				       || _node.IsOverride;
			}
		}
		
		public bool IsSpecialName
		{
			get
			{
				return false;
			}
		}
		
		public bool AcceptVarArgs
		{
			get
			{
				return _node.Parameters.HasParamArray;
			}
		}
		
		override public EntityType EntityType
		{
			get { return EntityType.Method; }
		}
		
		public ICallableType CallableType
		{
			get
			{
				return My<TypeSystemServices>.Instance.GetCallableType(this);
			}
		}
		
		public IType Type
		{
			get
			{
				return CallableType;
			}
		}
		
		public Method Method
		{
			get
			{
				return _node;
			}
		}
		
		public IMethod Overriden
		{
			get
			{
				return _override;
			}
			
			set
			{
				_override = value;
			}
		}
		
		public IParameter[] GetParameters()
		{
			return _provider.Map(_node.Parameters);
		}
		
		public virtual IType ReturnType
		{
			get
			{
				if (null == _node.ReturnType)
					return _node.DeclaringType.NodeType == NodeType.ClassDefinition
					       	? Unknown.Default
					       	: (IType)_provider.VoidType;
				return TypeSystemServices.GetType(_node.ReturnType);
			}
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return DeclaringType;
			}
		}
		
		public bool IsGenerator
		{
			get
			{
				return null != _yieldStatements;
			}
		}
		
		public ExpressionCollection ReturnExpressions
		{
			get
			{
				return _returnExpressions;
			}
		}
		
		public ExpressionCollection YieldExpressions
		{
			get
			{
				ExpressionCollection expressions = new ExpressionCollection();
				foreach (YieldStatement stmt in _yieldStatements)
				{
					if (null != stmt.Expression) expressions.Add(stmt.Expression);
				}
				return expressions;
			}
		}
		
		
		sealed class LabelCollector : DepthFirstVisitor
		{
			public static readonly InternalLabel[] EmptyInternalLabelArray = new InternalLabel[0];

			List _labels;

			public override void OnLabelStatement(LabelStatement node)
			{
				if (null == _labels) _labels = new List();
				_labels.Add(node.Entity);
			}
			
			public InternalLabel[] Labels
			{
				get
				{
					if (null == _labels) return EmptyInternalLabelArray;
					return (InternalLabel[])_labels.ToArray(new InternalLabel[_labels.Count]);
				}
			}
		}
		
		public InternalLabel[] Labels
		{
			get
			{
				LabelCollector collector = new LabelCollector();
				_node.Accept(collector);
				return collector.Labels;
			}
		}
		
		public void AddYieldStatement(YieldStatement stmt)
		{
			if (null == _yieldStatements) _yieldStatements = new List();
			_yieldStatements.Add(stmt);
		}
		
		public void AddReturnExpression(Expression expression)
		{
			if (null == _returnExpressions) _returnExpressions = new ExpressionCollection();
			_returnExpressions.Add(expression);
		}

		public Local ResolveLocal(string name)
		{
			foreach (Local local in _node.Locals)
			{
				if (local.PrivateScope) continue;
				if (name == local.Name) return local;
			}
			return null;
		}
		
		public ParameterDeclaration ResolveParameter(string name)
		{
			foreach (ParameterDeclaration parameter in _node.Parameters)
			{
				if (name == parameter.Name) return parameter;
			}
			return null;
		}
		
		public virtual bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			if (Entities.IsFlagSet(typesToConsider, EntityType.Local))
			{
				Local local = ResolveLocal(name);
				if (null != local)
				{
					resultingSet.Add(TypeSystemServices.GetEntity(local));
					return true;
				}
			}
			
			if (Entities.IsFlagSet(typesToConsider, EntityType.Parameter))
			{
				ParameterDeclaration parameter = ResolveParameter(name);
				if (null != parameter)
				{
					resultingSet.Add(TypeSystemServices.GetEntity(parameter));
					return true;
				}
			}

			return false;
		}
		
		IEnumerable<IEntity> INamespace.GetMembers()
		{
			return NullNamespace.EmptyEntityArray;
		}
		
		override public string ToString()
		{
			return TypeSystemServices.GetSignature(this);
		}
		
		public virtual IConstructedMethodInfo ConstructedInfo
		{
			get { return null; }
		}

		public virtual IGenericMethodInfo GenericInfo
		{
			get { return null; }
		}
	}
}
