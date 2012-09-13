using Boo.Lang.Compiler.Ast;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem.Services
{
	public class TypeChecker
	{
		private readonly EnvironmentProvision<TypeSystemServices> _typeSystemServices;
		private readonly EnvironmentProvision<CompilerErrorCollection> _errors;
		private readonly EnvironmentProvision<CompilerWarningCollection> _warnings;

		public bool AssertTypeCompatibility(Node sourceNode, IType expectedType, IType actualType)
		{
			if (IsError(expectedType) || IsError(actualType))
				return false;

			if (expectedType.IsPointer && actualType.IsPointer)
				return true; //if both types are unmanaged pointers casting is always possible

			if (TypeSystemServices.IsNullable(expectedType) && actualType.IsNull())
				return true;

			if (!CanBeReachedFrom(sourceNode, expectedType, actualType))
			{
				_errors.Instance.Add(CompilerErrorFactory.IncompatibleExpressionType(sourceNode, expectedType, actualType));
				return false;
			}
			return true;
		}

		public bool CanBeReachedFrom(Node anchor, IType expectedType, IType actualType)
		{
			bool byDowncast;
			if (!_typeSystemServices.Instance.CanBeReachedFrom(expectedType, actualType, out byDowncast))
				return false;
			if (byDowncast)
				_warnings.Instance.Add(CompilerWarningFactory.ImplicitDowncast(anchor, expectedType, actualType));
			return true;
		}

		private static bool IsError(IEntity entity)
		{
			return TypeSystemServices.IsError(entity);
		}
	}
}