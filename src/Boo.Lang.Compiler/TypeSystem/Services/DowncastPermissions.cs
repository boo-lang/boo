namespace Boo.Lang.Compiler.TypeSystem.Services
{
    public class DowncastPermissions
    {
        private readonly CompilerParameters _parameters;

        public DowncastPermissions(CompilerContext context)
        {
            _parameters = context.Parameters;
        }

        public virtual bool IsDowncastAllowed()
        {
            return !_parameters.Strict || !_parameters.DisabledWarnings.Contains(CompilerWarningFactory.Codes.ImplicitDowncast);
        }
    }
}