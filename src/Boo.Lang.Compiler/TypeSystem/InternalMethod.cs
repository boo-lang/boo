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

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using Boo.Lang.Compiler.Ast;

	public class InternalMethod : IInternalEntity, IMethod, INamespace
	{	
		protected TypeSystemServices _typeSystemServices;
		
		protected Method _method;
		
		protected IMethod _override;
		
		protected IType _declaringType;
		
		protected IParameter[] _parameters;
		
		protected ExpressionCollection _returnExpressions;

		protected List _yieldStatements;

		private bool? _isBooExtension;
		private bool? _isClrExtension;

		internal InternalMethod(TypeSystemServices typeSystemServices, Method method)
		{
			_typeSystemServices = typeSystemServices;
			_method = method;
			if (method.NodeType != NodeType.Constructor && method.NodeType != NodeType.Destructor)
			{
				if (null == _method.ReturnType)
				{
					IType returnType = _method.DeclaringType.NodeType == NodeType.ClassDefinition
						? Unknown.Default
						: (IType)_typeSystemServices.VoidType;
					_method.ReturnType = _typeSystemServices.CodeBuilder.CreateTypeReference(method.LexicalInfo, returnType);
				}
			}
		}

		public bool IsExtension
		{
			get
			{
				return IsBooExtension || IsClrExtension;
			}
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
				return this.ReturnType == _typeSystemServices.DuckType;
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
			return MetadataUtil.IsAttributeDefined(_method, _typeSystemServices.Map(attributeType));
		}
		
		public IType DeclaringType
		{
			get
			{
				if (null == _declaringType)
				{
					_declaringType = (IType)TypeSystemServices.GetEntity(_method.DeclaringType);
				}
				return _declaringType;
			}
		}
		
		public bool IsStatic
		{
			get
			{
				return _method.IsStatic;
			}
		}
		
		public bool IsPublic
		{
			get
			{
				return _method.IsPublic;
			}
		}
		
		public bool IsProtected
		{
			get
			{
				return _method.IsProtected;
			}
		}

		public bool IsPrivate
		{
			get
			{
				return _method.IsPrivate;
			}
		}

		public bool IsInternal
		{
			get
			{
				return _method.IsInternal;
			}
		}
		
		public bool IsAbstract
		{
			get
			{
				return _method.IsAbstract;
			}
		}
		
		public bool IsVirtual
		{
			get
			{
				return _method.IsVirtual
					|| _method.IsAbstract
					|| _method.IsOverride;
			}
		}
		
		public bool IsSpecialName
		{
			get
			{
				return false;
			}
		}
		
		public string Name
		{
			get
			{	
				return _method.Name;
			}
		}

		public bool AcceptVarArgs
		{
			get
			{
				return _method.Parameters.VariableNumber;
			}
		}
		
		public virtual string FullName
		{
			get
			{
				return _method.DeclaringType.FullName + "." + _method.Name;
			}
		}
		
		public virtual EntityType EntityType
		{
			get
			{
				return EntityType.Method;
			}
		}
		
		public ICallableType CallableType
		{
			get
			{
				return _typeSystemServices.GetCallableType(this);
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
				return _method;
			}
		}
		
		public Node Node
		{
			get
			{
				return _method;
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
			if (null == _parameters)
			{
				_parameters = _typeSystemServices.Map(_method.Parameters);
			}
			return _parameters;
		}
		
		public virtual IType ReturnType
		{
			get
			{
				return TypeSystemServices.GetType(_method.ReturnType);
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
		
		
		class LabelCollector : DepthFirstVisitor
		{
			public static readonly InternalLabel[] EmptyInternalLabelArray = new InternalLabel[0];

			protected List _labels;
		
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
				_method.Accept(collector);
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
			foreach (Local local in _method.Locals)
			{
				if (local.PrivateScope) continue;
				if (name == local.Name) return local;
			}
			return null;
		}
		
		public ParameterDeclaration ResolveParameter(string name)
		{
			foreach (ParameterDeclaration parameter in _method.Parameters)
			{
				if (name == parameter.Name) return parameter;
			}
			return null;
		}
		
		public virtual bool Resolve(List targetList, string name, EntityType flags)
		{
			if (NameResolutionService.IsFlagSet(flags, EntityType.Local))
			{
				Local local = ResolveLocal(name);
				if (null != local)
				{
					targetList.Add(TypeSystemServices.GetEntity(local));
					return true;
				}
			}
			
			if (NameResolutionService.IsFlagSet(flags, EntityType.Parameter))
			{
				ParameterDeclaration parameter = ResolveParameter(name);
				if (null != parameter)
				{
					targetList.Add(TypeSystemServices.GetEntity(parameter));
					return true;
				}
			}

			return false;
		}
		
		IEntity[] INamespace.GetMembers()
		{
			return NullNamespace.EmptyEntityArray;
		}
		
		override public string ToString()
		{
			return _typeSystemServices.GetSignature(this);
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
