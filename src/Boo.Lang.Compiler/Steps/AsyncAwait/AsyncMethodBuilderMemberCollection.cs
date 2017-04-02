using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps.AsyncAwait
{
    /// <summary>
    /// Async methods have both a return type (void, Task, or Task&lt;T&gt;) and a 'result' type, which is the
    /// operand type of any return expressions in the async method. The result type is void in the case of
    /// Task-returning and void-returning async methods, and T in the case of Task&lt;T&gt;-returning async
    /// methods.
    /// 
    /// System.Runtime.CompilerServices provides a collection of async method builders that are used in the
    /// generated code of async methods to create and manipulate the async method's task. There are three
    /// distinct async method builder types, one of each async return type: AsyncVoidMethodBuilder,
    /// AsyncTaskMethodBuilder, and AsyncTaskMethodBuilder&lt;T&gt;. 
    /// 
    /// AsyncMethodBuilderMemberCollection provides a common mechanism for accessing the well-known members of
    /// each async method builder type. This avoids having to inspect the return style of the current async method
    /// to pick the right async method builder member during async rewriting.
    /// 
    /// Adapted from Microsoft.CodeAnalysis.CSharp.AsyncMethodBuilderMemberCollection in the Roslyn codebase
    /// </summary>
    public struct AsyncMethodBuilderMemberCollection
    {
        /// <summary>
        /// The builder's constructed type.
        /// </summary>
        public readonly IType BuilderType;

        /// <summary>
        /// The result type of the constructed task: T for Task&lt;T&gt;, void otherwise.
        /// </summary>
        public readonly IType ResultType;

        /// <summary>
        /// Create an instance of the method builder.
        /// </summary>
        public readonly IMethod CreateBuilder;

        /// <summary>
        /// Binds an exception to the method builder.
        /// </summary>
        public readonly IMethod SetException;

        /// <summary>
        /// Marks the method builder as successfully completed, and sets the result if method is Task&lt;T&gt;-returning.
        /// </summary>
        public readonly IMethod SetResult;

        /// <summary>
        /// Schedules the state machine to proceed to the next action when the specified awaiter completes.
        /// </summary>
        public readonly IMethod AwaitOnCompleted;

        /// <summary>
        /// Schedules the state machine to proceed to the next action when the specified awaiter completes. This method can be called from partially trusted code.
        /// </summary>
        public readonly IMethod AwaitUnsafeOnCompleted;

        /// <summary>
        /// Begins running the builder with the associated state machine.
        /// </summary>
        public readonly IMethod Start;

        /// <summary>
        /// Associates the builder with the specified state machine.
        /// </summary>
        public readonly IMethod SetStateMachine;

        /// <summary>
        /// Get the constructed task for a Task-returning or Task&lt;T&gt;-returning async method.
        /// </summary>
        public readonly IProperty Task;

        private AsyncMethodBuilderMemberCollection(
            IType builderType,
            IType resultType,
            IMethod createBuilder,
            IMethod setException,
            IMethod setResult,
            IMethod awaitOnCompleted,
            IMethod awaitUnsafeOnCompleted,
            IMethod start,
            IMethod setStateMachine,
            IProperty task)
        {
            BuilderType = builderType;
            ResultType = resultType;
            CreateBuilder = createBuilder;
            SetException = setException;
            SetResult = setResult;
            AwaitOnCompleted = awaitOnCompleted;
            AwaitUnsafeOnCompleted = awaitUnsafeOnCompleted;
            Start = start;
            SetStateMachine = setStateMachine;
            Task = task;
        }

        public static bool TryCreate(TypeSystemServices tss, Method method, IType genericArg,
            out AsyncMethodBuilderMemberCollection collection)
        {
            if (ContextAnnotations.IsAsync(method))
            {
                var returnType = (IType) method.ReturnType.Entity;
                if (returnType == tss.VoidType)
                    return TryCreateVoid(tss, out collection);
                if (returnType == tss.TaskType)
                    return TryCreateTask(tss, out collection);
                if (returnType.ConstructedInfo != null &&
                    returnType.ConstructedInfo.GenericDefinition == tss.GenericTaskType)
                    return TryCreateGenericTask(tss, genericArg, out collection);
            }

            throw CompilerErrorFactory.InvalidAsyncType(method.ReturnType);
        }

        private static bool TryCreate(IType builderType, IType resultType,
            out AsyncMethodBuilderMemberCollection collection)
        {
            var members = builderType.GetMembers().OfType<IMember>().Where(m => m.IsPublic).ToDictionary(m => m.Name);
	        var task = members.ContainsKey("Task") ? (IProperty) members["Task"] : null;
            collection = new AsyncMethodBuilderMemberCollection(
                builderType,
                resultType,
                (IMethod) members["Create"],
                (IMethod) members["SetException"],
                (IMethod) members["SetResult"],
                (IMethod) members["AwaitOnCompleted"],
                (IMethod) members["AwaitUnsafeOnCompleted"],
                (IMethod) members["Start"],
                (IMethod) members["SetStateMachine"],
				task);

            return true;
        }

        private static bool TryCreateGenericTask(TypeSystemServices tss,
            IType genericArg, out AsyncMethodBuilderMemberCollection collection)
        {
            var builderType = tss.AsyncGenericTaskMethodBuilderType.GenericInfo.ConstructType(genericArg);
            return TryCreate(
                builderType,
                genericArg,
                out collection);
        }

        private static bool TryCreateTask(TypeSystemServices tss, 
            out AsyncMethodBuilderMemberCollection collection)
        {
            var builderType = tss.AsyncTaskMethodBuilderType;
            var resultType = tss.VoidType;
            return TryCreate(
                builderType,
                resultType,
                out collection);
        }

        private static bool TryCreateVoid(TypeSystemServices tss,
            out AsyncMethodBuilderMemberCollection collection)
        {
            var builderType = tss.AsyncVoidMethodBuilderType;
            var resultType = tss.VoidType;
            return TryCreate(
                builderType,
                resultType,
                out collection);
        }

    }
}
