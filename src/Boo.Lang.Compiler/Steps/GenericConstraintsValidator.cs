#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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


using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	public class GenericConstraintsValidator
	{
		private readonly CompilerContext _context;
		private readonly Node _node;
		private readonly GenericParameterDeclarationCollection _parameters;

		public GenericConstraintsValidator(CompilerContext ctx, Node node, GenericParameterDeclarationCollection parameters)
		{
			_context = ctx;
			_node = node;
			_parameters = parameters;
		}

		public void Validate()
		{
			foreach (GenericParameterDeclaration parameter in _parameters)
			{
				new GenericConstraintValidator(_context, parameter).Validate();
			}
		}
	}

	public class GenericConstraintValidator
	{
		private readonly CompilerContext _context;
		private readonly GenericParameterDeclaration _gpd;

		private bool? _hasClassConstraint = null;
		private bool? _hasStructConstraint = null;
		private bool? _hasConstructorConstraint = null;
		private TypeReference _baseType = null;

		public GenericConstraintValidator(CompilerContext context, GenericParameterDeclaration gpd)
		{
			_context = context;
			_gpd = gpd;
		}

		protected CompilerContext Context
		{
			get { return _context; }
		}

		protected bool HasClassConstraint
		{
			get
			{
				if (_hasClassConstraint == null)
				{
					_hasClassConstraint = HasConstraint(_gpd.Constraints, GenericParameterConstraints.ReferenceType);
				}
				return (bool)_hasClassConstraint;
			}
		}

		protected bool HasStructConstraint
		{
			get
			{
				if (_hasStructConstraint == null)
				{
					_hasStructConstraint = HasConstraint(_gpd.Constraints, GenericParameterConstraints.ValueType);
				}
				return (bool)_hasStructConstraint;
			}
		}

		protected bool HasConstructorConstraint
		{
			get
			{
				if (_hasConstructorConstraint == null)
				{
					_hasConstructorConstraint = HasConstraint(_gpd.Constraints, GenericParameterConstraints.Constructable);
				}
				return (bool)_hasConstructorConstraint;
			}
		}

		protected TypeSystemServices TypeSystemServices
		{
			get { return Context.TypeSystemServices; }
		}

		protected void Error(CompilerError error)
		{
			Context.Errors.Add(error);
		}

		public void Validate()
		{
			CheckAttributes();
			CheckTypeConstraints();
		}

		private void CheckAttributes()
		{
			// Check for consistency
			if (HasClassConstraint && HasStructConstraint)
			{
				Error(CompilerErrorFactory.StructAndClassConstraintsConflict(_gpd));
			}

			// Check for redundancy
			if (HasStructConstraint && HasConstructorConstraint)
			{
				Error(CompilerErrorFactory.StructAndConstructorConstraintsConflict(_gpd));
			}
		}

		private void CheckTypeConstraints()
		{
			foreach (TypeReference tr in _gpd.BaseTypes)
			{
				CheckTypeConstraint(tr);
			}
		}

		private void CheckTypeConstraint(TypeReference tr)
		{
			IType type = (IType)tr.Entity;
			
			// Check for validity as type constraint (class or interface; not sealed; not special)
			if (!IsValidTypeConstraint(type))
			{
				Error(CompilerErrorFactory.InvalidTypeConstraint(_gpd, tr));
			}

			// Check for a class basetype
			if (type.IsClass)
			{
				// Ensure no more than a single basetype 
				if (_baseType == null)
				{
					_baseType = tr;
				}
				else
				{
					Error(CompilerErrorFactory.MultipleBaseTypeConstraints(_gpd, tr, _baseType));
				}

				// Class basetype cannot be used with struct or class constraints
				if (HasStructConstraint)
				{
					Error(CompilerErrorFactory.TypeConstraintConflictsWithSpecialConstraint(_gpd, tr, "struct"));
				}
				if (HasClassConstraint)
				{
					Error(CompilerErrorFactory.TypeConstraintConflictsWithSpecialConstraint(_gpd, tr, "class"));
				}
			}

			// Check for 'naked dependency' to another generic parameter
			if (type is IGenericParameter)
			{
				// TODO: Inherit attributes and interface constraints, check consistency with basetype
				// TODO: Check for cycles
			}	

			// TODO: Check for accessibility - at least as accessible as definition
			if (LessAccessibleThan(type, _gpd.ParentNode.Entity))
			{
				// TODO: Error "type constraint '...' is less accessible than definition '...'"
			}
		}

		private bool IsValidTypeConstraint(IType type)
		{
			// Type constraints must be interfaces or classes
			if (!(type.IsInterface || type.IsClass))
			{
				return false;
			}

			// Type constraint cannot be final
			if (type.IsFinal)
			{
				return false;
			}

			// Type constraint cannot be one of the special types
			if (type == TypeSystemServices.ArrayType ||
				type == TypeSystemServices.ObjectType ||
				type == TypeSystemServices.DelegateType ||
				type == TypeSystemServices.ValueTypeType)
			{
				return false;
			}

			return true;
		}

		private bool LessAccessibleThan(IType left, IEntity right)
		{
			// TODO: return true if left is less accessible than right
			return false;
		}

		private bool HasConstraint(GenericParameterConstraints flags, GenericParameterConstraints flag)
		{
			return ((flags & flag) == flag);
		}
	}
}