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