using System;
using System.Collections.Generic;
using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem
{
    public static class AsyncHelper
    {
        public static bool ValidAsyncTypeUnbound(Method value)
        {
            var ret = value.ReturnType;
            if (ret == null)
                return true;
            var typeRef = ret as SimpleTypeReference;
            return typeRef != null; //trying to resolve this too early has too many edge cases
        }

        internal static bool ValidAsyncTypeBound(Method value)
        {
            var ret = (IType)value.ReturnType.Entity;
            var tss = My<TypeSystemServices>.Instance;
            return ret == tss.VoidType ||
                   ret == tss.TaskType ||
                   TypeCompatibilityRules.IsAssignableFrom(tss.GenericTaskType, ret);
        }

        private static IMethod GetNoArgs(IEnumerable<IEntity> value, TypeSystemServices tss)
        {
            return value.Cast<IMethod>().SingleOrDefault(m => m.GetParameters().Length == 0);
        }

        private static IMethod GetNoArgsNoVoid(IEnumerable<IEntity> value, TypeSystemServices tss)
        {
			return value.Cast<IMethod>().SingleOrDefault(m => 
				((m.GetParameters().Length == 0) || (m.IsExtension && m.GetParameters().Length == 1)) && 
				m.ReturnType != tss.VoidType);
        }

        public static IType GetAwaitType(Expression value)
        {
            var type = value.ExpressionType;
            var tss = My<TypeSystemServices>.Instance;
            if (type == tss.TaskType)
                return type;
            if (type.ConstructedInfo != null && type.ConstructedInfo.GenericDefinition == tss.GenericTaskType)
                return type.ConstructedInfo.GenericArguments[0];

            var awaiterSet = new List<IEntity>();
			IEntity[] candidates = type.Resolve(awaiterSet, "GetAwaiter", EntityType.Method) ?
				awaiterSet.ToArray() : 
				tss.FindExtension(type, "GetAwaiter");

            var awaiter = GetNoArgsNoVoid(candidates , tss);
            if (awaiter == null)
                return null;
			value["$GetAwaiter"] = awaiter;
			var awaiterType = awaiter.ReturnType;
            if (awaiterType == null || awaiterType == tss.VoidType)
                return null;
            awaiterSet.Clear();
            if (awaiterType.Resolve(awaiterSet, "GetResult", EntityType.Method))
            {
                var getResult = GetNoArgs(awaiterSet, tss);
                if (getResult == null)
                    return null;
				value["$GetResult"] = getResult;
                if (getResult.ReturnType == tss.VoidType)
                    return tss.TaskType;
                return getResult.ReturnType;
            }
            return null;
        }

        internal static bool InAsyncMethod(Expression value)
        {
			INodeWithBody ancestor = value.GetAncestor<BlockExpression>();
			if (ancestor == null) ancestor = value.GetAncestor<Method>();
            return ContextAnnotations.IsAsync(ancestor);
        }
    }
}
