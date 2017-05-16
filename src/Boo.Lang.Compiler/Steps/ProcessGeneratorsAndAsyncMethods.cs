using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps.AsyncAwait;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps
{
    public class ProcessGeneratorsAndAsyncMethods : ProcessGenerators
    {
        public override void LeaveMethod(Method method)
        {
            if (ContextAnnotations.IsAsync(method))
            {
                var entity = (InternalMethod)method.Entity;
                var processor = new AsyncMethodProcessor(Context, entity);
                processor.Run();
            }
            else base.LeaveMethod(method);
        }
    }
}
