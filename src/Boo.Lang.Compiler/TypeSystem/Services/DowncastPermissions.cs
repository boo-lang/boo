using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem.Services
{
    public class DowncastPermissions
    {
        private readonly CompilerParameters _parameters = My<CompilerParameters>.Instance;
        private readonly TypeSystemServices _typeSystem = My<TypeSystemServices>.Instance;

        public virtual bool CanBeReachedByDowncast(IType expectedType, IType actualType)
        {
            if (actualType.IsFinal)
                return false;

            if (IsDuckType(actualType))
                return true;

            if (!IsDowncastAllowed())
                return false;

            if (expectedType.IsInterface || actualType.IsInterface)
                return CanBeReachedByInterfaceDowncast(expectedType, actualType);
            
            return TypeCompatibilityRules.IsAssignableFrom(actualType, expectedType);
        }

    	private bool IsDuckType(IType actualType)
    	{
    		return _typeSystem.IsDuckType(actualType);
    	}

    	protected virtual bool CanBeReachedByInterfaceDowncast(IType expectedType, IType actualType)
        {
            //FIXME: currently interface downcast implements no type safety check at all (see BOO-1211)
            return true;
        }

        protected virtual bool IsDowncastAllowed()
        {
            return !_parameters.Strict || !_parameters.DisabledWarnings.Contains(CompilerWarningFactory.Codes.ImplicitDowncast);
        }
    }
}