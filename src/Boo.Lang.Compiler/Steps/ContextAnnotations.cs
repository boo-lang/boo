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


using System;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps
{
	public static class ContextAnnotations
	{		
		private static readonly object EntryPointKey = new();
		
		private static readonly object AssemblyBuilderKey = new();

		private static readonly object MetadataBuilderKey = new(); 

		private static readonly object AsyncKey = new();

        private static readonly object AwaitInExceptionHandlerKey = new();

		private static readonly object FieldInvocationKey = new();

		public static Method GetEntryPoint(CompilerContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}
			return (Method)context.Properties[EntryPointKey];
		}
		
		public static void SetEntryPoint(CompilerContext context, Method method)
		{
			if (method == null)
			{
				throw new ArgumentNullException(nameof(method));
			}
			
			Method current = GetEntryPoint(context);
			if (null != current)
			{
				throw CompilerErrorFactory.MoreThanOneEntryPoint(method);
			}
			context.Properties[EntryPointKey] = method;
		}
		
		public static System.Reflection.Emit.AssemblyBuilder GetAssemblyBuilder(CompilerContext context)
		{
			var builder = (System.Reflection.Emit.AssemblyBuilder)context.Properties[AssemblyBuilderKey];
			if (builder == null)
			{
				throw CompilerErrorFactory.InvalidAssemblySetUp(context.CompileUnit);
			}
			return builder;
		}
		
		public static void SetAssemblyBuilder(CompilerContext context, System.Reflection.Emit.AssemblyBuilder builder)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

            context.Properties[AssemblyBuilderKey] = builder ?? throw new ArgumentNullException(nameof(builder));
		}

		public static System.Reflection.PortableExecutable.ManagedPEBuilder GetPEBuilder(CompilerContext context)
        {
			var builder = (System.Reflection.PortableExecutable.ManagedPEBuilder)context.Properties[MetadataBuilderKey];
			if (builder == null)
			{
				throw CompilerErrorFactory.InvalidAssemblySetUp(context.CompileUnit);
			}
			return builder;
		}

		public static void SetPEBuilder(CompilerContext context, System.Reflection.PortableExecutable.ManagedPEBuilder builder)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			context.Properties[MetadataBuilderKey] = builder ?? throw new ArgumentNullException(nameof(builder));
		}

		public static void MarkAsync(INodeWithBody node)
	    {
	        ((Node)node).Annotate(AsyncKey);
	    }

	    public static bool IsAsync(INodeWithBody node)
	    {
	        return ((Node) node).ContainsAnnotation(AsyncKey);
	    }

        public static void MarkAwaitInExceptionHandler(INodeWithBody node)
        {
            ((Node)node).Annotate(AwaitInExceptionHandlerKey);
        }

        public static bool AwaitInExceptionHandler(INodeWithBody node)
	    {
            return ((Node)node).ContainsAnnotation(AwaitInExceptionHandlerKey);
	    }

		public static void AddFieldInvocation(MethodInvocationExpression node)
		{
			var context = CompilerContext.Current;
            if (context[FieldInvocationKey] is not List<MethodInvocationExpression> list)
            {
                list = new List<MethodInvocationExpression>();
                context[FieldInvocationKey] = list;
            }
            list.Add(node);
		}

		public static List<MethodInvocationExpression> GetFieldInvocations()
		{
			var context = CompilerContext.Current;
			return context[FieldInvocationKey] as List<MethodInvocationExpression>;
		}
	}
}