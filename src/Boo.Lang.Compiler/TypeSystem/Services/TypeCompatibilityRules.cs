namespace Boo.Lang.Compiler.TypeSystem.Services
{
    public static class TypeCompatibilityRules
    {
        public static bool IsAssignableFrom(IType expectedType, IType actualType)
        {
            return expectedType.IsAssignableFrom(actualType);
        }
    }
}
