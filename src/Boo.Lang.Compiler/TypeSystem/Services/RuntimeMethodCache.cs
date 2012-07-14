using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Runtime;

namespace Boo.Lang.Compiler.TypeSystem.Services
{
	public class RuntimeMethodCache : AbstractCompilerComponent
	{
		public IMethod Activator_CreateInstance
		{
			get { return CachedMethod("Activator_CreateInstance", () => Methods.Of<Type, object[], object>(Activator.CreateInstance)); }
		}

		public IConstructor Exception_StringConstructor
		{
			get { return CachedConstructor("Exception_StringConstructor", () => TypeSystemServices.GetStringExceptionConstructor()); }
		}

		public IMethod TextReaderEnumerator_lines
		{
			get { return CachedMethod("TextReaderEnumerator_lines", () => Methods.Of<TextReader, IEnumerable<string>>(TextReaderEnumerator.lines)); }
		}

		public IMethod List_GetRange1
		{
			get { return CachedMethod("List_GetRange1", () => Methods.InstanceFunctionOf<List<object>, int, List<object>>(l => l.GetRange)); }
		}

		public IMethod List_GetRange2
		{
			get { return CachedMethod("List_GetRange2", () => Methods.InstanceFunctionOf<List<object>, int, int, List<object>>(l => l.GetRange)); }
		}

		public IMethod RuntimeServices_GetRange1
		{
			get { return CachedRuntimeServicesMethod("GetRange1", () => Methods.Of<Array, int, Array>(RuntimeServices.GetRange1)); }
		}

		public IMethod RuntimeServices_GetRange2
		{
			get { return CachedRuntimeServicesMethod("GetRange2", () => Methods.Of<Array, int, int, Array>(RuntimeServices.GetRange2)); }
		}

		public IMethod RuntimeServices_GetMultiDimensionalRange1
		{
			get { return CachedRuntimeServicesMethod("GetMultiDimensionalRange1", () => Methods.Of<Array, int[], bool[], bool[], Array>(RuntimeServices.GetMultiDimensionalRange1)); }
		}

		public IMethod RuntimeServices_Len
		{
			get { return CachedRuntimeServicesMethod("Len", () => Methods.Of<object, int>(RuntimeServices.Len)); }
		}

		public IMethod RuntimeServices_Mid
		{
			get { return CachedRuntimeServicesMethod("Mid", () => Methods.Of<string, int, int, string>(RuntimeServices.Mid)); }
		}

		public IMethod RuntimeServices_NormalizeStringIndex
		{
			get { return CachedRuntimeServicesMethod("NormalizeStringIndex", () => Methods.Of<string, int, int>(RuntimeServices.NormalizeStringIndex)); }
		}

		public IMethod RuntimeServices_AddArrays
		{
			get { return CachedRuntimeServicesMethod("AddArrays", () => Methods.Of<Type, Array, Array, Array>(RuntimeServices.AddArrays)); }
		}

		public IMethod RuntimeServices_SetMultiDimensionalRange1
		{
			get { return CachedRuntimeServicesMethod("SetMultiDimensionalRange1", () => Methods.Of<Array, Array, int[], bool[]>(RuntimeServices.SetMultiDimensionalRange1)); }
		}

		public IMethod RuntimeServices_GetEnumerable
		{
			get { return CachedRuntimeServicesMethod("GetEnumerable", () => Methods.Of<object, IEnumerable>(RuntimeServices.GetEnumerable)); }
		}

		public IMethod RuntimeServices_EqualityOperator
		{
			get { return CachedMethod("RuntimeServices_EqualityOperator", () => Methods.Of<object, object, bool>(RuntimeServices.EqualityOperator)); }
		}

		public IMethod Array_get_Length
		{
			get { return CachedMethod("Array_get_Length", () => Methods.GetterOf<Array, int>(a => a.Length)); }
		}

		public IMethod Array_GetLength
		{
			get { return CachedMethod("Array_GetLength", () => Methods.InstanceFunctionOf<Array, int, int>(a => a.GetLength)); }
		}

		public IMethod String_get_Length
		{
			get { return CachedMethod("String_get_Length", () => Methods.GetterOf<string, int>(s => s.Length)); }
		}

		public IMethod String_Substring_Int
		{
			get { return CachedMethod("String_Substring_Int", () => Methods.InstanceFunctionOf<string, int, string>(s => s.Substring)); }
		}

		public IMethod ICollection_get_Count
		{
			get { return CachedMethod("ICollection_get_Count", () => Methods.GetterOf<ICollection, int>(c => c.Count)); }
		}

		public IMethod ICallable_Call
		{
			get { return CachedMethod("ICallable_Call", () => Methods.InstanceFunctionOf<ICallable, object[], object>(c => c.Call)); }
		}

		private IMethod CachedRuntimeServicesMethod(string methodName, Func<MethodInfo> producer)
		{
			return CachedMethod("RuntimeServices_" + methodName, producer);
		}

		IMethod CachedMethod(string key, Func<MethodInfo> producer)
		{
			return (IMethod)CachedMethodBase(key, () => TypeSystemServices.Map(producer()));
		}

		IConstructor CachedConstructor(string key, Func<IMethodBase> producer)
		{
			return (IConstructor)CachedMethodBase(key, producer);
		}

		private IMethodBase CachedMethodBase(string key, Func<IMethodBase> producer)
		{
			IMethodBase method;
			if (!_methodCache.TryGetValue(key, out method))
			{
				method = producer();
				_methodCache.Add(key, method);
			}
			return method;
		}

		private readonly Dictionary<string, IMethodBase> _methodCache = new Dictionary<string, IMethodBase>(StringComparer.Ordinal);
	}
}