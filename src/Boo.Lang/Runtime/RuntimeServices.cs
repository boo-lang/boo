#region license
// Copyright (c) 2004, 2007 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//	 * Redistributions of source code must retain the above copyright notice,
//	 this list of conditions and the following disclaimer.
//	 * Redistributions in binary form must reproduce the above copyright notice,
//	 this list of conditions and the following disclaimer in the documentation
//	 and/or other materials provided with the distribution.
//	 * Neither the name of Rodrigo B. de Oliveira nor the names of its
//	 contributors may be used to endorse or promote products derived from this
//	 software without specific prior written permission.
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
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Reflection.Emit;
using System.Text;

namespace Boo.Lang.Runtime
{
	public class RuntimeServices
	{
		private static readonly object[] NoArguments = new object[0];

		private static readonly Type RuntimeServicesType = typeof(RuntimeServices);

		internal const BindingFlags InstanceMemberFlags = BindingFlags.Public |
														 BindingFlags.NonPublic |
														 BindingFlags.Instance;

		internal const BindingFlags DefaultBindingFlags = InstanceMemberFlags |
												BindingFlags.OptionalParamBinding |
												BindingFlags.Static |
												BindingFlags.FlattenHierarchy;

		private const BindingFlags InvokeBindingFlags = DefaultBindingFlags |
												BindingFlags.InvokeMethod;

		private const BindingFlags StaticMemberBindingFlags = BindingFlags.Public |
												BindingFlags.Static |
												BindingFlags.FlattenHierarchy;

		private const BindingFlags InvokeOperatorBindingFlags = StaticMemberBindingFlags |
												BindingFlags.InvokeMethod;

		private const BindingFlags SetPropertyBindingFlags = DefaultBindingFlags |
												BindingFlags.SetProperty |
												BindingFlags.SetField;

		private const BindingFlags GetPropertyBindingFlags = DefaultBindingFlags |
												BindingFlags.GetProperty |
												BindingFlags.GetField;

		private static DispatcherCache _cache = new DispatcherCache();
		private static ExtensionRegistry _extensions = new ExtensionRegistry();

		public delegate void CodeBlock();

		public static void WithExtensions(System.Type extensions, CodeBlock block)
		{
			RegisterExtensions(extensions);
			try
			{
				block();
			}
			finally
			{
				UnRegisterExtensions(extensions);
			}
		}

		public static void RegisterExtensions(System.Type extensions)
		{
			_extensions.Register(extensions);
		}

		public static void UnRegisterExtensions(System.Type extensions)
		{
			_extensions.UnRegister(extensions);
		}

		public static object Invoke(object target, string name, object[] args)
		{	
			return Dispatch(target, name, args, delegate { return CreateMethodDispatcher(target, name, args); });
		}

		private static Dispatcher CreateMethodDispatcher(object target, string name, object[] args)
		{
			IQuackFu duck = target as IQuackFu;
			if (null != duck)
			{
				return delegate(object o, object[] arguments) { return ((IQuackFu) o).QuackInvoke(name, arguments); };
			}

			Type type = target as Type;
			if (null != type)
			{
				// static method
				return DoCreateMethodDispatcher(null, type, name, args);
			}

			Type targetType = target.GetType();
			if (targetType.IsCOMObject)
			{
				// COM methods cant be seen through reflection
				// so maybe the proper way to support parameter conversions
				// is indeed by creating a specific Binder object
				// which knows the boo conversion rules
				return
					delegate(object o, object[] arguments) { return o.GetType().InvokeMember(name, InvokeBindingFlags, null, target, arguments); };
			}
			
			return DoCreateMethodDispatcher(target, targetType, name, args);
		}

		private static Dispatcher DoCreateMethodDispatcher(object target, Type targetType, string name, object[] args)
		{
			return new MethodDispatcherFactory(_extensions, target, targetType, name, args).Create();
		}

		private static object Dispatch(object target, string cacheKeyName, object[] args, DispatcherCache.DispatcherFactory factory)
		{
			Type[] cacheKeyTypes = MethodResolver.GetArgumentTypes(args);
			return Dispatch(target, cacheKeyName, cacheKeyTypes, args, factory);
		}

		private static object Dispatch(object target, string cacheKeyName, Type[] cacheKeyTypes, object[] args, DispatcherCache.DispatcherFactory factory)
		{
			Type targetType = (target as Type) ?? target.GetType();
			DispatcherKey key = new DispatcherKey(targetType, cacheKeyName, cacheKeyTypes);
			Dispatcher dispatcher = _cache.Get(key, factory);
			return dispatcher(target, args);
		}

		public static object GetProperty(object target, string name)
		{
			return Dispatch(target, name, NoArguments, delegate { return CreatePropGetDispatcher(target, name);  });
		}

		private static Dispatcher CreatePropGetDispatcher(object target, string name)
		{
			IQuackFu duck = target as IQuackFu;
			if (null != duck)
			{
				return delegate(object o, object[] args) { return ((IQuackFu)o).QuackGet(name, null); };
			}

			Type type = target as Type;
			if (null != type) return DoCreatePropGetDispatcher(null, type, name);

			Type targetType = target.GetType();
			if (targetType.IsCOMObject)
			{
				return delegate(object o, object[] args)
		       	{
		       		return o.GetType().InvokeMember(name, GetPropertyBindingFlags, null, o, null);
		       	};
			}

			return DoCreatePropGetDispatcher(target, target.GetType(), name);
		}

		private static Dispatcher DoCreatePropGetDispatcher(object target, Type type, string name)
		{
			return new PropertyDispatcherFactory(_extensions, target, type, name).CreateGetter();
		}

		public static object SetProperty(object target, string name, object value)
		{
			return Dispatch(target, name, new object[] { value }, delegate { return CreatePropSetDispatcher(target, name, value); });
		}

		private static Dispatcher CreatePropSetDispatcher(object target, string name, object value)
		{
			IQuackFu duck = target as IQuackFu;
			if (null != duck)
			{
				return delegate(object o, object[] args) { return ((IQuackFu) o).QuackSet(name, null, args[0]); };
			}

			Type type = target as Type;
			if (null != type) return DoCreatePropSetDispatcher(null, type, name, value);

			Type targetType = target.GetType();
			if (targetType.IsCOMObject)
			{
				return delegate(object o, object[] args)
		       	{
		       		return o.GetType().InvokeMember(name,
		       		                                SetPropertyBindingFlags,
		       		                                null,
		       		                                o,
		       		                                args);
		       	};
			}

			return DoCreatePropSetDispatcher(target, targetType, name, value);
		}

		private static Dispatcher DoCreatePropSetDispatcher(object target, Type type, string name, object value)
		{
			return new PropertyDispatcherFactory(_extensions, target, type, name, value).CreateSetter();
		}

		public struct ValueTypeChange
		{
			public object Target;
			public string Member;
			public object Value;

			public ValueTypeChange(object target, string member, object value)
			{
				this.Target = target;
				this.Member = member;
				this.Value = value;
			}
		}

		public static void PropagateValueTypeChanges(ValueTypeChange[] changes)
		{
			foreach (ValueTypeChange change in changes)
			{
				if (!(change.Value is ValueType)) break;
				try
				{
					SetProperty(change.Target, change.Member, change.Value);
				}
				catch (System.MissingFieldException)
				{
					// hit a readonly property
					break;
				}
			}
		}

		[Obsolete("Use Coerce instead.")]
		public static object DuckImplicitCast(object value, Type toType)
		{
			return Coerce(value, toType);
		}

		public static object Coerce(object value, Type toType)
		{
			if (value == null) return null;

			return Dispatch(value, "$Coerce$", new Type[] {toType}, new object[] {toType}, delegate { return CreateCoerceDispatcher(value, toType); });
		}

		private static Dispatcher CreateCoerceDispatcher(object value, Type toType)
		{
			if (toType.IsInstanceOfType(value)) return IdentityDispatcher;
			if (value is ICoercible) return CoercibleDispatcher;

			Type fromType = value.GetType();
			if (IsPromotableNumeric(fromType) && IsPromotableNumeric(toType))
			{
				return new PromotionEmitter(toType).Emit();
			}

			MethodInfo method = FindImplicitConversionOperator(fromType, toType);
			if (null == method) return IdentityDispatcher;

			return EmitImplicitConversionDispatcher(method);
		}

		private static bool IsPromotableNumeric(Type fromType)
		{
			return IsPromotableNumeric(Type.GetTypeCode(fromType));
		}

		private static Dispatcher EmitImplicitConversionDispatcher(MethodInfo method)
		{
			return new ImplicitConversionEmitter(method).Emit();
		}

		private static object CoercibleDispatcher(object o, object[] args)
		{
			return ((ICoercible) o).Coerce((Type)args[0]);
		}

		private static object IdentityDispatcher(object o, object[] args)
		{
			return o;
		}

		public static object GetSlice(object target, string name, object[] args)
		{
			return Dispatch(target, name + "[]", args, delegate { return CreateGetSliceDispatcher(target, name, args); });
		}

		private static Dispatcher CreateGetSliceDispatcher(object target, string name, object[] args)
		{
			IQuackFu duck = target as IQuackFu;
			if (null != duck)
			{
				return delegate(object o, object[] arguments) { return ((IQuackFu) o).QuackGet(name, arguments); };
			}

			if ("" == name
				&& args.Length == 1
				&& target is System.Array) return GetArraySlice;
			
			return new SliceDispatcherFactory(_extensions, target, target.GetType(), name, args).CreateGetter();
		}

		private static object GetArraySlice(object target, object[] args)
		{
			IList list = (IList)target;
			return list[NormalizeIndex(list.Count, (int)args[0])];
		}

		public static object SetSlice(object target, string name, object[] args)
		{
			return Dispatch(target, name + "[]=", args, delegate { return CreateSetSliceDispatcher(target, name, args); });
		}

		static Dispatcher CreateSetSliceDispatcher(object target, string name, object[] args)
		{
			IQuackFu duck = target as IQuackFu;
			if (null != duck)
			{
				return delegate(object o, object[] arguments)
				{
					return ((IQuackFu) o).QuackSet(name, (object[]) GetRange2(arguments, 0, arguments.Length - 1), arguments[arguments.Length - 1]);
				};
			}

			if ("" == name
				&& 2 == args.Length
				&& target is System.Array) return SetArraySlice;

			return new SliceDispatcherFactory(_extensions, target, target.GetType(), name, args).CreateSetter();
		}

		private static object SetArraySlice(object target, object[] args)
		{
			IList list = (IList)target;
			list[NormalizeIndex(list.Count, (int)args[0])] = args[1];
			return args[1];
		}

		internal static String GetDefaultMemberName(Type type)
		{
			DefaultMemberAttribute attribute = (DefaultMemberAttribute)Attribute.GetCustomAttribute(type, typeof(DefaultMemberAttribute));
			return attribute != null ? attribute.MemberName : "";
		}

		public static object InvokeCallable(object target, object[] args)
		{
			if (null == target) throw new ArgumentNullException("target");
			if (null == args) throw new ArgumentNullException("args");

			ICallable c = target as ICallable;
			if (null != c) return c.Call(args);

			Delegate d = target as Delegate;
			if (null != d) return d.DynamicInvoke(args);
			
			Type type = target as Type;
			if (null != type) return Activator.CreateInstance(type, args);
			
			return ((MethodInfo)target).Invoke(null, args);
		}

		private static bool IsNumeric(TypeCode code)
		{
			switch (code)
			{
				case TypeCode.Byte: return true;
				case TypeCode.SByte: return true;
				case TypeCode.Int16: return true;
				case TypeCode.Int32: return true;
				case TypeCode.Int64: return true;
				case TypeCode.UInt16: return true;
				case TypeCode.UInt32: return true;
				case TypeCode.UInt64: return true;
				case TypeCode.Single: return true;
				case TypeCode.Double: return true;
				case TypeCode.Decimal: return true;
			}
			return false;
		}

		public static object InvokeBinaryOperator(string operatorName, object lhs, object rhs)
		{
			Type lhsType = lhs.GetType();
			Type rhsType = rhs.GetType();
			TypeCode lhsTypeCode = Type.GetTypeCode(lhsType);
			TypeCode rhsTypeCode = Type.GetTypeCode(rhsType);

			if (IsNumeric(lhsTypeCode) && IsNumeric(rhsTypeCode))
			{
				// HACK: optimization to get to the correct operators faster
				// is it worthy?
				switch (((int)operatorName[3] << 8) + (int)operatorName[operatorName.Length - 1])
				{
					case ((int)'A' << 8) + (int)'n':			// op_Addition
						return op_Addition(lhs, lhsTypeCode, rhs, rhsTypeCode);
					case ((int)'S' << 8) + (int)'n':			// op_Subtraction
						return op_Subtraction(lhs, lhsTypeCode, rhs, rhsTypeCode);
					case ((int)'M' << 8) + (int)'y':			// op_Multiply
						return op_Multiply(lhs, lhsTypeCode, rhs, rhsTypeCode);
					case ((int)'D' << 8) + (int)'n':			// op_Division
						return op_Division(lhs, lhsTypeCode, rhs, rhsTypeCode);
					case ((int)'M' << 8) + (int)'s':			// op_Modulus
						return op_Modulus(lhs, lhsTypeCode, rhs, rhsTypeCode);
					case ((int)'E' << 8) + (int)'n':			// op_Exponentiation
						return op_Exponentiation(lhs, lhsTypeCode, rhs, rhsTypeCode);
					case ((int)'L' << 8) + (int)'n':			// op_LessThan
						return op_LessThan(lhs, lhsTypeCode, rhs, rhsTypeCode);
					case ((int)'L' << 8) + (int)'l':			// op_LessThanOrEqual
						return op_LessThanOrEqual(lhs, lhsTypeCode, rhs, rhsTypeCode);
					case ((int)'G' << 8) + (int)'n':			// op_GreaterThan
						return op_GreaterThan(lhs, lhsTypeCode, rhs, rhsTypeCode);
					case ((int)'G' << 8) + (int)'l':			// op_GreaterThanOrEqual
						return op_GreaterThanOrEqual(lhs, lhsTypeCode, rhs, rhsTypeCode);
					case ((int)'B' << 8) + (int)'r':			// op_BitwiseOr
						return op_BitwiseOr(lhs, lhsTypeCode, rhs, rhsTypeCode);
					case ((int)'B' << 8) + (int)'d':			// op_BitwiseAnd
						return op_BitwiseAnd(lhs, lhsTypeCode, rhs, rhsTypeCode);
					case ((int)'E' << 8) + (int)'r':			// op_ExclusiveOr
						return op_ExclusiveOr(lhs, lhsTypeCode, rhs, rhsTypeCode);
					case ((int)'S' << 8) + (int)'t':			// op_ShiftLeft/Right
						return operatorName[8] == 'L' ?
							op_ShiftLeft(lhs, lhsTypeCode, rhs, rhsTypeCode) :
							op_ShiftRight(lhs, lhsTypeCode, rhs, rhsTypeCode);
					case ((int)'M' << 8) + (int)'h':			// op_Match
					case ((int)'N' << 8) + (int)'h':			// op_NotMatch
					case ((int)'M' << 8) + (int)'r':			// op_Member
					case ((int)'N' << 8) + (int)'r':			// op_NotMember
					default:
						throw new ArgumentException(lhs + " " + operatorName + " " + rhs);
				}
			}
			else
			{
				object[] args = new object[] { lhs, rhs };
				IQuackFu duck = lhs as IQuackFu;
				if (null != duck)
				{
					return duck.QuackInvoke(operatorName, args);
				}
				else
				{
					duck = rhs as IQuackFu;
					if (null != duck)
					{
						return duck.QuackInvoke(operatorName, args);
					}
				}

				try
				{
					// TODO: first resolve the right method on either
					// lhs and rhs
					// and then cache the final information
					return Invoke(lhsType, operatorName, args);
				}
				catch (MissingMethodException)
				{
					try
					{
						return Invoke(rhsType, operatorName, args);
					}
					catch (MissingMethodException)
					{
						try
						{
							return InvokeRuntimeServicesOperator(operatorName, args);
						}
						catch (MissingMethodException)
						{
						}
					}

					throw; // always throw the original exception
				}
			}
		}

		public static object InvokeUnaryOperator(string operatorName, object operand)
		{
			Type operandType = operand.GetType();
			TypeCode operandTypeCode = Type.GetTypeCode(operandType);

			if (IsNumeric(operandTypeCode))
			{
				// HACK: optimization to get to the correct operators faster
				// is it worthy?
				switch (((int)operatorName[3] << 8) + (int)operatorName[operatorName.Length - 1])
				{
					case ((int)'U' << 8) + (int)'n':			// op_UnaryNegation
						return op_UnaryNegation(operand, operandTypeCode);
					default:
						throw new ArgumentException(operatorName + " " + operand);
				}
			}
			else
			{
				object[] args = new object[] { operand };
				IQuackFu duck = operand as IQuackFu;
				if (null != duck)
				{
					return duck.QuackInvoke(operatorName, args);
				}

				try
				{
					return Invoke(operandType, operatorName, args);
				}
				catch (MissingMethodException)
				{
					try
					{
						return InvokeRuntimeServicesOperator(operatorName, args);
					}
					catch (MissingMethodException)
					{
					}

					throw; // always throw the original exception
				}
			}
		}

		private static object InvokeRuntimeServicesOperator(string operatorName, object[] args)
		{
			return Invoke(RuntimeServicesType, operatorName, args);
		}

		public static object MoveNext(IEnumerator enumerator)
		{
			if (null == enumerator) Error("CantUnpackNull");
			if (!enumerator.MoveNext()) Error("UnpackListOfWrongSize");
			return enumerator.Current;
		}

		public static int Len(object obj)
		{
			if (null != obj)
			{
				ICollection collection = obj as ICollection;
				if (null != collection) return collection.Count;

				string s = obj as string;
				if (null != s) return s.Length;
			}
			throw new ArgumentException();
		}

		public static string Mid(string s, int begin, int end)
		{
			begin = NormalizeStringIndex(s, begin);
			end = NormalizeStringIndex(s, end);
			return s.Substring(begin, end - begin);
		}

		public static Array GetRange1(Array source, int begin)
		{
			return GetRange2(source, begin, source.Length);
		}

		public static Array GetRange2(Array source, int begin, int end)
		{
			int sourceLen = source.Length;
			begin = NormalizeIndex(sourceLen, begin);
			end = NormalizeIndex(sourceLen, end);
			int targetLen = Math.Max(0, end - begin);
			Array target = Array.CreateInstance(source.GetType().GetElementType(), targetLen);
			Array.Copy(source, begin, target, 0, targetLen);
			return target;
		}

		public static void SetMultiDimensionalRange1(Array source, Array dest, int[] ranges, bool[] collapse)
		{
			if (dest.Rank != ranges.Length / 2)
			{
				throw new Exception("invalid range passed: " + ranges.Length / 2 + ", expected " + dest.Rank * 2);
			}

			for (int i = 0; i < dest.Rank; i++)
			{
				if (ranges[2 * i] > 0 ||
					ranges[2 * i] > dest.GetLength(i) ||
					ranges[2 * i + 1] > dest.GetLength(i) ||
					ranges[2 * i + 1] < ranges[2 * i])
				{
					// FIXME: Better error reporting
					Error("InvalidArray");
				}
			}

			int sourceRank = 0;
			foreach (bool val in collapse)
			{
				if (!val)
				{
					sourceRank++;
				}
			}

			if (source.Rank != sourceRank)
			{
				// FIXME: Better error reporting
				Error("InvalidArray");
			}

			int[] lensDest = new int[dest.Rank];
			int[] lensSrc = new int[sourceRank];
			int rankIndex = 0;
			for (int i = 0; i < dest.Rank; i++)
			{
				lensDest[i] = ranges[2 * i + 1] - ranges[2 * i];
				if (!collapse[i])
				{
					lensSrc[rankIndex] = lensDest[i] - ranges[2 * i];
					if (lensSrc[rankIndex] != source.GetLength(rankIndex))
					{
						// FIXME: Better error reporting
						Error("InvalidArray");
					}
					rankIndex++;
				}
			}

			int[] modInd = new int[dest.Rank];
			for (int i = 0; i < dest.Rank; i++)
			{
				if (i == 0)
				{
					modInd[i] = source.Length / lensDest[lensDest.Length - 1];
				}
				else
				{
					modInd[i] = modInd[i - 1] / lensDest[i - 1];
				}
			}

			int counter;
			int[] indexDest = new int[dest.Rank];
			int[] indexSrc = new int[sourceRank];
			for (int i = 0; i < source.Length; i++)
			{
				counter = 0;
				for (int j = 0; j < dest.Rank; j++)
				{
					int index = (i % modInd[j]) / (modInd[j] / lensDest[j]);
					indexDest[j] = index;
					if (!collapse[j])
					{
						indexSrc[counter] = indexDest[j] + ranges[2 * j];
						counter++;
					}
					dest.SetValue(source.GetValue(indexSrc), indexDest);
				}
			}
		}

		public static Array GetMultiDimensionalRange1(Array source, int[] ranges, bool[] collapse)
		{
			int rankSrc = source.Rank;
			int collapseSize = 0;

			foreach (bool val in collapse)
			{
				if (val)
				{
					collapseSize++;
				}
			}

			int rankDest = rankSrc - collapseSize;
			int[] lensDest = new int[rankDest];
			int[] lensSrc = new int[rankSrc];

			int rankIndex = 0;
			for (int i = 0; i < rankSrc; i++)
			{
				ranges[2 * i] = NormalizeIndex(source.GetLength(i), ranges[2 * i]);
				ranges[2 * i + 1] = NormalizeIndex(source.GetLength(i), ranges[2 * i + 1]);

				lensSrc[i] = ranges[2 * i + 1] - ranges[2 * i];
				if (!collapse[i])
				{
					lensDest[rankIndex] = ranges[2 * i + 1] - ranges[2 * i];
					rankIndex++;
				}
			}

			Array dest = Array.CreateInstance(source.GetType().GetElementType(), lensDest);

			int[] modInd = new int[rankSrc];
			int[] indicesDest = new int[rankDest];
			int[] indicesSrc = new int[rankSrc];

			for (int i = 0; i < rankSrc; i++)
			{
				if (i == 0)
				{
					modInd[i] = dest.Length;
				}
				else
				{
					modInd[i] = modInd[i - 1] / lensSrc[i - 1];
				}
			}

			for (int i = 0; i < dest.Length; i++)
			{
				int destIndex = 0;
				for (int j = 0; j < rankSrc; j++)
				{
					int index = (i % modInd[j]) / (modInd[j] / lensSrc[j]);
					indicesSrc[j] = ranges[2 * j] + index;
					if (!collapse[j])
					{
						indicesDest[destIndex] = indicesSrc[j] - ranges[2 * j];
						destIndex++;
					}
				}
				dest.SetValue(source.GetValue(indicesSrc), indicesDest);
			}

			return dest;
		}

		public static void CheckArrayUnpack(Array array, int expected)
		{
			if (null == array)
			{
				Error("CantUnpackNull");
			}
			if (expected > array.Length)
			{
				Error("UnpackArrayOfWrongSize", expected, array.Length);
			}
		}

		/*
		 * Since index normalization is quite performance-sensitive, the
		 * method body is duplicated on Normalize* methods to increase
		 * likelihood that the JIT will inline the call in the caller.
		 * On Mono it gives ~30% improvement since it triggers inlining On.
		 */
		public static int NormalizeIndex(int len, int index)
		{
			if (index < 0)
				return Math.Max(0, index + len);
			return Math.Min(index, len);
		}

		public static int NormalizeArrayIndex(Array array, int index)
		{
			if (index < 0)
				return Math.Max(0, index + array.Length);
			return Math.Min(index, array.Length);
		}

		public static int NormalizeStringIndex(string s, int index)
		{
			if (index < 0)
				return Math.Max(0, index + s.Length);
			return Math.Min(index, s.Length);
		}

		public static IEnumerable GetEnumerable(object enumerable)
		{
			if (null == enumerable) Error("CantEnumerateNull");

			IEnumerable iterator = enumerable as IEnumerable;
			if (null != iterator) return iterator;

			TextReader reader = enumerable as TextReader;
			if (null != reader) return TextReaderEnumerator.lines(reader);

			Error("ArgumentNotEnumerable");
			return null;
		}

		#region global operators

		public static Array AddArrays(Type resultingElementType, Array lhs, Array rhs)
		{
			int resultingLen = lhs.Length + rhs.Length;
			Array result = Array.CreateInstance(resultingElementType, resultingLen);
			Array.Copy(lhs, 0, result, 0, lhs.Length);
			Array.Copy(rhs, 0, result, lhs.Length, rhs.Length);
			return result;
		}

		public static string op_Addition(string lhs, string rhs)
		{
			return string.Concat(lhs, rhs);
		}

		public static string op_Addition(string lhs, object rhs)
		{
			return string.Concat(lhs, rhs);
		}

		public static string op_Addition(object lhs, string rhs)
		{
			return string.Concat(lhs, rhs);
		}

		public static Array op_Multiply(Array lhs, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			Type type = lhs.GetType();
			if (1 != type.GetArrayRank())
			{
				throw new ArgumentException("lhs");
			}

			int length = lhs.Length;
			Array result = Array.CreateInstance(type.GetElementType(), length * count);
			int destinationIndex = 0;
			for (int i = 0; i < count; ++i)
			{
				Array.Copy(lhs, 0, result, destinationIndex, length);
				destinationIndex += length;
			}
			return result;
		}

		public static Array op_Multiply(int count, Array rhs)
		{
			return op_Multiply(rhs, count);
		}

		public static string op_Multiply(string lhs, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			string result = null;
			if (null != lhs)
			{
				StringBuilder builder = new StringBuilder(lhs.Length * count);
				for (int i = 0; i < count; ++i)
				{
					builder.Append(lhs);
				}
				result = builder.ToString();
			}
			return result;
		}

		public static string op_Multiply(int count, string rhs)
		{
			return op_Multiply(rhs, count);
		}

		public static bool op_NotMember(string lhs, string rhs)
		{
			return !op_Member(lhs, rhs);
		}

		public static bool op_Member(string lhs, string rhs)
		{
			if (null == lhs || null == rhs)
			{
				return false;
			}
			return rhs.IndexOf(lhs) > -1;
		}

#if !NO_SYSTEM_DLL
		public static bool op_Match(string input, System.Text.RegularExpressions.Regex pattern)
		{
			return pattern.IsMatch(input);
		}

		public static bool op_Match(string input, string pattern)
		{
			return System.Text.RegularExpressions.Regex.IsMatch(input, pattern);
		}

		public static bool op_NotMatch(string input, System.Text.RegularExpressions.Regex pattern)
		{
			return !op_Match(input, pattern);
		}
	
		public static bool op_NotMatch(string input, string pattern)
		{
			return !op_Match(input, pattern);
		}
#endif

		public static string op_Modulus(string lhs, IEnumerable rhs)
		{
			return string.Format(lhs, Boo.Lang.Builtins.array(rhs));
		}

		public static string op_Modulus(string lhs, object[] rhs)
		{
			return string.Format(lhs, rhs);
		}

		public static bool op_Member(object lhs, IList rhs)
		{
			if (null == rhs)
			{
				return false;
			}
			return rhs.Contains(lhs);
		}

		public static bool op_NotMember(object lhs, IList rhs)
		{
			return !op_Member(lhs, rhs);
		}

		public static bool op_Member(object lhs, IDictionary rhs)
		{
			if (null == rhs)
			{
				return false;
			}
			return rhs.Contains(lhs);
		}

		public static bool op_NotMember(object lhs, IDictionary rhs)
		{
			return !op_Member(lhs, rhs);
		}

		public static bool op_Member(object lhs, IEnumerable rhs)
		{
			if (null == rhs)
			{
				return false;
			}
			foreach (object item in rhs)
			{
				if (EqualityOperator(lhs, item))
				{
					return true;
				}
			}
			return false;
		}

		public static bool op_NotMember(object lhs, IEnumerable rhs)
		{
			return !op_Member(lhs, rhs);
		}

		public static bool EqualityOperator(object lhs, object rhs)
		{
			if (lhs == rhs) return true;

			// Some types do overload Equals to compare
			// against null values
			if (null == lhs) return rhs.Equals(lhs);
			if (null == rhs) return lhs.Equals(rhs);

			TypeCode lhsTypeCode = Type.GetTypeCode(lhs.GetType());
			TypeCode rhsTypeCode = Type.GetTypeCode(rhs.GetType());
			if (IsNumeric(lhsTypeCode) && IsNumeric(rhsTypeCode))
			{
				return EqualityOperator(lhs, lhsTypeCode, rhs, rhsTypeCode);
			}

			Array lhsa = lhs as Array;
			if (null != lhsa)
			{
				Array rhsa = rhs as Array;
				if (null != rhsa)
				{
					return ArrayEqualityImpl(lhsa, rhsa);
				}
			}
			return lhs.Equals(rhs) || rhs.Equals(lhs);
		}

		public static bool op_Equality(Array lhs, Array rhs)
		{
			if (lhs == rhs)
			{
				return true;
			}

			if (null == lhs || null == rhs)
			{
				return false;
			}
			return ArrayEqualityImpl(lhs, rhs);
		}

		static bool ArrayEqualityImpl(Array lhs, Array rhs)
		{
			if (1 != lhs.Rank || 1 != rhs.Rank)
			{
				throw new ArgumentException("array rank must be 1");
			}

			if (lhs.Length != rhs.Length)
			{
				return false;
			}

			for (int i = 0; i < lhs.Length; ++i)
			{
				if (!EqualityOperator(lhs.GetValue(i), rhs.GetValue(i)))
				{
					return false;
				}
			}
			return true;
		}
		#endregion

		#region dynamic operator for primitive types
		private static TypeCode GetConvertTypeCode(TypeCode lhsTypeCode, TypeCode rhsTypeCode)
		{
			/* C# ECMA Spec V2
			 * 14.2.6.2 Binary numeric promotions
			 * This clause is informative.
			 * Binary numeric promotion occurs for the operands of the predefined +, ?, *, /, %, &, |, ^, ==, !=, >, <, >=,
			 * and <= binary operators. Binary numeric promotion implicitly converts both operands to a common type
			 * which, in case of the non-relational operators, also becomes the result type of the operation. Binary numeric
			 * promotion consists of applying the following rules, in the order they appear here:
			 * � If either operand is of type decimal, the other operand is converted to type decimal, or a compiletime
			 *	 error occurs if the other operand is of type float or double.
			 * � Otherwise, if either operand is of type double, the other operand is converted to type double.
			 * � Otherwise, if either operand is of type float, the other operand is converted to type float.
			 * � Otherwise, if either operand is of type ulong, the other operand is converted to type ulong, or a
			 *	 compile-time error occurs if the other operand is of type sbyte, short, int, or long.
			 * � Otherwise, if either operand is of type long, the other operand is converted to type long.
			 * � Otherwise, if either operand is of type uint and the other operand is of type sbyte, short, or int,
			 *	 both operands are converted to type long.
			 * � Otherwise, if either operand is of type uint, the other operand is converted to type uint.
			 * � Otherwise, both operands are converted to type int.
			 * [Note: The first rule disallows any operations that mix the decimal type with the double and float types.
			 *	The rule follows from the fact that there are no implicit conversions between the decimal type and the
			 *	double and float types. end note]
			 * [Note: Also note that it is not possible for an operand to be of type ulong when the other operand is of a
			 *	signed integral type. The reason is that no integral type exists that can represent the full range of ulong as
			 *	well as the signed integral types. end note]
			 * In both of the above cases, a cast expression can be used to explicitly convert one operand to a type that is
			 * compatible with the other operand.
			 */
			if (TypeCode.Decimal == lhsTypeCode || TypeCode.Decimal == rhsTypeCode)
			{
				return TypeCode.Decimal;	// not per ECMA spec
			}
			if (TypeCode.Double == lhsTypeCode || TypeCode.Double == rhsTypeCode)
			{
				return TypeCode.Double;
			}
			if (TypeCode.Single == lhsTypeCode || TypeCode.Single == rhsTypeCode)
			{
				return TypeCode.Single;
			}
			if (TypeCode.UInt64 == lhsTypeCode)
			{
				if (TypeCode.SByte == rhsTypeCode || TypeCode.Int16 == rhsTypeCode ||
					TypeCode.Int32 == rhsTypeCode || TypeCode.Int64 == rhsTypeCode)
				{
					//					throw new ArgumentException("ulong <op> " + rhsTypeCode);
					return TypeCode.Int64;	// not per Ecma spec
				}
				return TypeCode.UInt64;
			}
			if (TypeCode.UInt64 == rhsTypeCode)
			{
				if (TypeCode.SByte == lhsTypeCode || TypeCode.Int16 == lhsTypeCode ||
					TypeCode.Int32 == lhsTypeCode || TypeCode.Int64 == lhsTypeCode)
				{
					//					throw new ArgumentException(lhsTypeCode + " <op> ulong");
					return TypeCode.Int64;	// not per Ecma spec
				}
				return TypeCode.UInt64;
			}
			if (TypeCode.Int64 == lhsTypeCode || TypeCode.Int64 == rhsTypeCode)
			{
				return TypeCode.Int64;
			}
			if (TypeCode.UInt32 == lhsTypeCode)
			{
				if (TypeCode.SByte == rhsTypeCode || TypeCode.Int16 == rhsTypeCode ||
					TypeCode.Int32 == rhsTypeCode)
				{
					return TypeCode.Int64;
				}
				return TypeCode.UInt32;
			}
			if (TypeCode.UInt32 == rhsTypeCode)
			{
				if (TypeCode.SByte == lhsTypeCode || TypeCode.Int16 == lhsTypeCode ||
					TypeCode.Int32 == lhsTypeCode)
				{
					return TypeCode.Int64;
				}
				return TypeCode.UInt32;
			}
			return TypeCode.Int32;
		}

		private static object op_Multiply(object lhs, TypeCode lhsTypeCode,
										  object rhs, TypeCode rhsTypeCode)
		{
			IConvertible lhsConvertible = (IConvertible)lhs;
			IConvertible rhsConvertible = (IConvertible)rhs;

			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
				case TypeCode.Decimal:
					return lhsConvertible.ToDecimal(null) * rhsConvertible.ToDecimal(null);
				case TypeCode.Double:
					return lhsConvertible.ToDouble(null) * rhsConvertible.ToDouble(null);
				case TypeCode.Single:
					return lhsConvertible.ToSingle(null) * rhsConvertible.ToSingle(null);
				case TypeCode.UInt64:
					return lhsConvertible.ToUInt64(null) * rhsConvertible.ToUInt64(null);
				case TypeCode.Int64:
					return lhsConvertible.ToInt64(null) * rhsConvertible.ToInt64(null);
				case TypeCode.UInt32:
					return lhsConvertible.ToUInt32(null) * rhsConvertible.ToUInt32(null);
				case TypeCode.Int32:
				default:
					return lhsConvertible.ToInt32(null) * rhsConvertible.ToInt32(null);
			}
		}

		private static object op_Division(object lhs, TypeCode lhsTypeCode,
										  object rhs, TypeCode rhsTypeCode)
		{
			IConvertible lhsConvertible = (IConvertible)lhs;
			IConvertible rhsConvertible = (IConvertible)rhs;

			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
				case TypeCode.Decimal:
					return lhsConvertible.ToDecimal(null) / rhsConvertible.ToDecimal(null);
				case TypeCode.Double:
					return lhsConvertible.ToDouble(null) / rhsConvertible.ToDouble(null);
				case TypeCode.Single:
					return lhsConvertible.ToSingle(null) / rhsConvertible.ToSingle(null);
				case TypeCode.UInt64:
					return lhsConvertible.ToUInt64(null) / rhsConvertible.ToUInt64(null);
				case TypeCode.Int64:
					return lhsConvertible.ToInt64(null) / rhsConvertible.ToInt64(null);
				case TypeCode.UInt32:
					return lhsConvertible.ToUInt32(null) / rhsConvertible.ToUInt32(null);
				case TypeCode.Int32:
				default:
					return lhsConvertible.ToInt32(null) / rhsConvertible.ToInt32(null);
			}
		}

		private static object op_Addition(object lhs, TypeCode lhsTypeCode,
										  object rhs, TypeCode rhsTypeCode)
		{
			IConvertible lhsConvertible = (IConvertible)lhs;
			IConvertible rhsConvertible = (IConvertible)rhs;

			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
				case TypeCode.Decimal:
					return lhsConvertible.ToDecimal(null) + rhsConvertible.ToDecimal(null);
				case TypeCode.Double:
					return lhsConvertible.ToDouble(null) + rhsConvertible.ToDouble(null);
				case TypeCode.Single:
					return lhsConvertible.ToSingle(null) + rhsConvertible.ToSingle(null);
				case TypeCode.UInt64:
					return lhsConvertible.ToUInt64(null) + rhsConvertible.ToUInt64(null);
				case TypeCode.Int64:
					return lhsConvertible.ToInt64(null) + rhsConvertible.ToInt64(null);
				case TypeCode.UInt32:
					return lhsConvertible.ToUInt32(null) + rhsConvertible.ToUInt32(null);
				case TypeCode.Int32:
				default:
					return lhsConvertible.ToInt32(null) + rhsConvertible.ToInt32(null);
			}
		}

		private static object op_Subtraction(object lhs, TypeCode lhsTypeCode,
											 object rhs, TypeCode rhsTypeCode)
		{
			IConvertible lhsConvertible = (IConvertible)lhs;
			IConvertible rhsConvertible = (IConvertible)rhs;

			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
				case TypeCode.Decimal:
					return lhsConvertible.ToDecimal(null) - rhsConvertible.ToDecimal(null);
				case TypeCode.Double:
					return lhsConvertible.ToDouble(null) - rhsConvertible.ToDouble(null);
				case TypeCode.Single:
					return lhsConvertible.ToSingle(null) - rhsConvertible.ToSingle(null);
				case TypeCode.UInt64:
					return lhsConvertible.ToUInt64(null) - rhsConvertible.ToUInt64(null);
				case TypeCode.Int64:
					return lhsConvertible.ToInt64(null) - rhsConvertible.ToInt64(null);
				case TypeCode.UInt32:
					return lhsConvertible.ToUInt32(null) - rhsConvertible.ToUInt32(null);
				case TypeCode.Int32:
				default:
					return lhsConvertible.ToInt32(null) - rhsConvertible.ToInt32(null);
			}
		}

		private static bool EqualityOperator(object lhs, TypeCode lhsTypeCode,
										  object rhs, TypeCode rhsTypeCode)
		{
			IConvertible lhsConvertible = (IConvertible)lhs;
			IConvertible rhsConvertible = (IConvertible)rhs;

			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
				case TypeCode.Decimal:
					return lhsConvertible.ToDecimal(null) == rhsConvertible.ToDecimal(null);
				case TypeCode.Double:
					return lhsConvertible.ToDouble(null) == rhsConvertible.ToDouble(null);
				case TypeCode.Single:
					return lhsConvertible.ToSingle(null) == rhsConvertible.ToSingle(null);
				case TypeCode.UInt64:
					return lhsConvertible.ToUInt64(null) == rhsConvertible.ToUInt64(null);
				case TypeCode.Int64:
					return lhsConvertible.ToInt64(null) == rhsConvertible.ToInt64(null);
				case TypeCode.UInt32:
					return lhsConvertible.ToUInt32(null) == rhsConvertible.ToUInt32(null);
				case TypeCode.Int32:
				default:
					return lhsConvertible.ToInt32(null) == rhsConvertible.ToInt32(null);
			}
		}

		private static bool op_GreaterThan(object lhs, TypeCode lhsTypeCode,
										  object rhs, TypeCode rhsTypeCode)
		{
			IConvertible lhsConvertible = (IConvertible)lhs;
			IConvertible rhsConvertible = (IConvertible)rhs;

			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
				case TypeCode.Decimal:
					return lhsConvertible.ToDecimal(null) > rhsConvertible.ToDecimal(null);
				case TypeCode.Double:
					return lhsConvertible.ToDouble(null) > rhsConvertible.ToDouble(null);
				case TypeCode.Single:
					return lhsConvertible.ToSingle(null) > rhsConvertible.ToSingle(null);
				case TypeCode.UInt64:
					return lhsConvertible.ToUInt64(null) > rhsConvertible.ToUInt64(null);
				case TypeCode.Int64:
					return lhsConvertible.ToInt64(null) > rhsConvertible.ToInt64(null);
				case TypeCode.UInt32:
					return lhsConvertible.ToUInt32(null) > rhsConvertible.ToUInt32(null);
				case TypeCode.Int32:
				default:
					return lhsConvertible.ToInt32(null) > rhsConvertible.ToInt32(null);
			}
		}

		private static bool op_GreaterThanOrEqual(object lhs, TypeCode lhsTypeCode,
										  object rhs, TypeCode rhsTypeCode)
		{
			IConvertible lhsConvertible = (IConvertible)lhs;
			IConvertible rhsConvertible = (IConvertible)rhs;

			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
				case TypeCode.Decimal:
					return lhsConvertible.ToDecimal(null) >= rhsConvertible.ToDecimal(null);
				case TypeCode.Double:
					return lhsConvertible.ToDouble(null) >= rhsConvertible.ToDouble(null);
				case TypeCode.Single:
					return lhsConvertible.ToSingle(null) >= rhsConvertible.ToSingle(null);
				case TypeCode.UInt64:
					return lhsConvertible.ToUInt64(null) >= rhsConvertible.ToUInt64(null);
				case TypeCode.Int64:
					return lhsConvertible.ToInt64(null) >= rhsConvertible.ToInt64(null);
				case TypeCode.UInt32:
					return lhsConvertible.ToUInt32(null) >= rhsConvertible.ToUInt32(null);
				case TypeCode.Int32:
				default:
					return lhsConvertible.ToInt32(null) >= rhsConvertible.ToInt32(null);
			}
		}

		private static bool op_LessThan(object lhs, TypeCode lhsTypeCode,
										  object rhs, TypeCode rhsTypeCode)
		{
			IConvertible lhsConvertible = (IConvertible)lhs;
			IConvertible rhsConvertible = (IConvertible)rhs;

			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
				case TypeCode.Decimal:
					return lhsConvertible.ToDecimal(null) < rhsConvertible.ToDecimal(null);
				case TypeCode.Double:
					return lhsConvertible.ToDouble(null) < rhsConvertible.ToDouble(null);
				case TypeCode.Single:
					return lhsConvertible.ToSingle(null) < rhsConvertible.ToSingle(null);
				case TypeCode.UInt64:
					return lhsConvertible.ToUInt64(null) < rhsConvertible.ToUInt64(null);
				case TypeCode.Int64:
					return lhsConvertible.ToInt64(null) < rhsConvertible.ToInt64(null);
				case TypeCode.UInt32:
					return lhsConvertible.ToUInt32(null) < rhsConvertible.ToUInt32(null);
				case TypeCode.Int32:
				default:
					return lhsConvertible.ToInt32(null) < rhsConvertible.ToInt32(null);
			}
		}

		private static bool op_LessThanOrEqual(object lhs, TypeCode lhsTypeCode,
										  object rhs, TypeCode rhsTypeCode)
		{
			IConvertible lhsConvertible = (IConvertible)lhs;
			IConvertible rhsConvertible = (IConvertible)rhs;

			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
				case TypeCode.Decimal:
					return lhsConvertible.ToDecimal(null) <= rhsConvertible.ToDecimal(null);
				case TypeCode.Double:
					return lhsConvertible.ToDouble(null) <= rhsConvertible.ToDouble(null);
				case TypeCode.Single:
					return lhsConvertible.ToSingle(null) <= rhsConvertible.ToSingle(null);
				case TypeCode.UInt64:
					return lhsConvertible.ToUInt64(null) <= rhsConvertible.ToUInt64(null);
				case TypeCode.Int64:
					return lhsConvertible.ToInt64(null) <= rhsConvertible.ToInt64(null);
				case TypeCode.UInt32:
					return lhsConvertible.ToUInt32(null) <= rhsConvertible.ToUInt32(null);
				case TypeCode.Int32:
				default:
					return lhsConvertible.ToInt32(null) <= rhsConvertible.ToInt32(null);
			}
		}

		private static object op_Modulus(object lhs, TypeCode lhsTypeCode,
										  object rhs, TypeCode rhsTypeCode)
		{
			IConvertible lhsConvertible = (IConvertible)lhs;
			IConvertible rhsConvertible = (IConvertible)rhs;

			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
				case TypeCode.Decimal:
					return lhsConvertible.ToDecimal(null) % rhsConvertible.ToDecimal(null);
				case TypeCode.Double:
					return lhsConvertible.ToDouble(null) % rhsConvertible.ToDouble(null);
				case TypeCode.Single:
					return lhsConvertible.ToSingle(null) % rhsConvertible.ToSingle(null);
				case TypeCode.UInt64:
					return lhsConvertible.ToUInt64(null) % rhsConvertible.ToUInt64(null);
				case TypeCode.Int64:
					return lhsConvertible.ToInt64(null) % rhsConvertible.ToInt64(null);
				case TypeCode.UInt32:
					return lhsConvertible.ToUInt32(null) % rhsConvertible.ToUInt32(null);
				case TypeCode.Int32:
				default:
					return lhsConvertible.ToInt32(null) % rhsConvertible.ToInt32(null);
			}
		}

		private static double op_Exponentiation(object lhs, TypeCode lhsTypeCode,
										  object rhs, TypeCode rhsTypeCode)
		{
			IConvertible lhsConvertible = (IConvertible)lhs;
			IConvertible rhsConvertible = (IConvertible)rhs;

			return Math.Pow(lhsConvertible.ToDouble(null), rhsConvertible.ToDouble(null));
		}

		private static object op_BitwiseAnd(object lhs, TypeCode lhsTypeCode,
										  object rhs, TypeCode rhsTypeCode)
		{
			IConvertible lhsConvertible = (IConvertible)lhs;
			IConvertible rhsConvertible = (IConvertible)rhs;

			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					throw new ArgumentException(lhsTypeCode + " & " + rhsTypeCode);
				case TypeCode.UInt64:
					return lhsConvertible.ToUInt64(null) & rhsConvertible.ToUInt64(null);
				case TypeCode.Int64:
					return lhsConvertible.ToInt64(null) & rhsConvertible.ToInt64(null);
				case TypeCode.UInt32:
					return lhsConvertible.ToUInt32(null) & rhsConvertible.ToUInt32(null);
				case TypeCode.Int32:
				default:
					return lhsConvertible.ToInt32(null) & rhsConvertible.ToInt32(null);
			}
		}

		private static object op_BitwiseOr(object lhs, TypeCode lhsTypeCode,
										  object rhs, TypeCode rhsTypeCode)
		{
			IConvertible lhsConvertible = (IConvertible)lhs;
			IConvertible rhsConvertible = (IConvertible)rhs;

			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					throw new ArgumentException(lhsTypeCode + " | " + rhsTypeCode);
				case TypeCode.UInt64:
					return lhsConvertible.ToUInt64(null) | rhsConvertible.ToUInt64(null);
				case TypeCode.Int64:
					return lhsConvertible.ToInt64(null) | rhsConvertible.ToInt64(null);
				case TypeCode.UInt32:
					return lhsConvertible.ToUInt32(null) | rhsConvertible.ToUInt32(null);
				case TypeCode.Int32:
				default:
					return lhsConvertible.ToInt32(null) | rhsConvertible.ToInt32(null);
			}
		}

		private static object op_ExclusiveOr(object lhs, TypeCode lhsTypeCode,
										  object rhs, TypeCode rhsTypeCode)
		{
			IConvertible lhsConvertible = (IConvertible)lhs;
			IConvertible rhsConvertible = (IConvertible)rhs;

			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					throw new ArgumentException(lhsTypeCode + " ^ " + rhsTypeCode);
				case TypeCode.UInt64:
					return lhsConvertible.ToUInt64(null) ^ rhsConvertible.ToUInt64(null);
				case TypeCode.Int64:
					return lhsConvertible.ToInt64(null) ^ rhsConvertible.ToInt64(null);
				case TypeCode.UInt32:
					return lhsConvertible.ToUInt32(null) ^ rhsConvertible.ToUInt32(null);
				case TypeCode.Int32:
				default:
					return lhsConvertible.ToInt32(null) ^ rhsConvertible.ToInt32(null);
			}
		}

		private static object op_ShiftLeft(object lhs, TypeCode lhsTypeCode,
										  object rhs, TypeCode rhsTypeCode)
		{
			IConvertible lhsConvertible = (IConvertible)lhs;
			IConvertible rhsConvertible = (IConvertible)rhs;

			switch(rhsTypeCode)
			{
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					throw new ArgumentException(lhsTypeCode + " << " + rhsTypeCode);
				default:
					break;
			}

			switch (lhsTypeCode)
			{
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					throw new ArgumentException(lhsTypeCode + " << " + rhsTypeCode);
				case TypeCode.UInt64:
					return lhsConvertible.ToUInt64(null) << rhsConvertible.ToInt32(null);
				case TypeCode.Int64:
					return lhsConvertible.ToInt64(null) << rhsConvertible.ToInt32(null);
				case TypeCode.UInt32:
					return lhsConvertible.ToUInt32(null) << rhsConvertible.ToInt32(null);
				case TypeCode.Int32:
				default:
					return lhsConvertible.ToInt32(null) << rhsConvertible.ToInt32(null);
			}
		}
		
		private static object op_ShiftRight(object lhs, TypeCode lhsTypeCode,
										  object rhs, TypeCode rhsTypeCode)
		{
			IConvertible lhsConvertible = (IConvertible)lhs;
			IConvertible rhsConvertible = (IConvertible)rhs;
			
			switch(rhsTypeCode)
			{
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					throw new ArgumentException(lhsTypeCode + " >> " + rhsTypeCode);
				default:
					break;
			}

			switch (lhsTypeCode)
			{
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					throw new ArgumentException(lhsTypeCode + " >> " + rhsTypeCode);
				case TypeCode.UInt64:
					return lhsConvertible.ToUInt64(null) >> rhsConvertible.ToInt32(null);
				case TypeCode.Int64:
					return lhsConvertible.ToInt64(null) >> rhsConvertible.ToInt32(null);
				case TypeCode.UInt32:
					return lhsConvertible.ToUInt32(null) >> rhsConvertible.ToInt32(null);
				case TypeCode.Int32:
				default:
					return lhsConvertible.ToInt32(null) >> rhsConvertible.ToInt32(null);
			}
		}
		
		private static object op_UnaryNegation(object operand, TypeCode operandTypeCode)
		{
			IConvertible operandConvertible = (IConvertible)operand;

			switch (operandTypeCode)
			{
				case TypeCode.Decimal:
					return -operandConvertible.ToDecimal(null);
				case TypeCode.Double:
					return -operandConvertible.ToDouble(null);
				case TypeCode.Single:
					return -operandConvertible.ToSingle(null);
				case TypeCode.UInt64:
					return -operandConvertible.ToInt64(null);
				case TypeCode.Int64:
					return -operandConvertible.ToInt64(null);
				case TypeCode.UInt32:
					return -operandConvertible.ToInt64(null);
				case TypeCode.Int32:
				default:
					return -operandConvertible.ToInt32(null);
			}
		}

		#endregion

		internal static bool IsPromotableNumeric(TypeCode code)
		{
			switch (code)
			{
				case TypeCode.Byte: return true;
				case TypeCode.SByte: return true;
				case TypeCode.Int16: return true;
				case TypeCode.Int32: return true;
				case TypeCode.Int64: return true;
				case TypeCode.UInt16: return true;
				case TypeCode.UInt32: return true;
				case TypeCode.UInt64: return true;
				case TypeCode.Single: return true;
				case TypeCode.Double: return true;
				case TypeCode.Boolean: return true;
				case TypeCode.Decimal: return true;
				case TypeCode.Char: return true;
			}
			return false;
		}

		public static IConvertible CheckNumericPromotion(object value)
		{
			IConvertible convertible = (IConvertible)value;
			return CheckNumericPromotion(convertible);
		}

		public static IConvertible CheckNumericPromotion(IConvertible convertible)
		{
			if (IsPromotableNumeric(convertible.GetTypeCode()))
			{
				return convertible;
			}
			throw new InvalidCastException();
		}

		public static Byte UnboxByte(object value)
		{
			if (value is Byte)
			{
				return (Byte)value;
			}
			return CheckNumericPromotion(value).ToByte(null);
		}

		public static SByte UnboxSByte(object value)
		{
			if (value is SByte)
			{
				return (SByte)value;
			}
			return CheckNumericPromotion(value).ToSByte(null);
		}

		public static char UnboxChar(object value)
		{
			if (value is char)
			{
				return (char)value;
			}
			return CheckNumericPromotion(value).ToChar(null);
		}

		public static Int16 UnboxInt16(object value)
		{
			if (value is Int16)
			{
				return (Int16)value;
			}
			return CheckNumericPromotion(value).ToInt16(null);
		}

		public static UInt16 UnboxUInt16(object value)
		{
			if (value is UInt16)
			{
				return (UInt16)value;
			}
			return CheckNumericPromotion(value).ToUInt16(null);
		}

		public static Int32 UnboxInt32(object value)
		{
			if (value is Int32)
			{
				return (Int32)value;
			}
			return CheckNumericPromotion(value).ToInt32(null);
		}

		public static UInt32 UnboxUInt32(object value)
		{
			if (value is UInt32)
			{
				return (UInt32)value;
			}
			return CheckNumericPromotion(value).ToUInt32(null);
		}

		public static Int64 UnboxInt64(object value)
		{
			if (value is Int64)
			{
				return (Int64)value;
			}
			return CheckNumericPromotion(value).ToInt64(null);
		}

		public static UInt64 UnboxUInt64(object value)
		{
			if (value is UInt64)
			{
				return (UInt64)value;
			}
			return CheckNumericPromotion(value).ToUInt64(null);
		}

		public static Single UnboxSingle(object value)
		{
			if (value is Single)
			{
				return (Single)value;
			}
			return CheckNumericPromotion(value).ToSingle(null);
		}

		public static Double UnboxDouble(object value)
		{
			if (value is Double)
			{
				return (Double)value;
			}
			return CheckNumericPromotion(value).ToDouble(null);
		}

		public static Decimal UnboxDecimal(object value)
		{
			if (value is Decimal)
			{
				return (Decimal)value;
			}
			return CheckNumericPromotion(value).ToDecimal(null);
		}

		public static Boolean UnboxBoolean(object value)
		{
			if (value is Boolean)
			{
				return (Boolean)value;
			}
			return CheckNumericPromotion(value).ToBoolean(null);
		}

		#region bool conversion
		public static bool ToBool(object value)
		{
			if (null == value) return false;
			if (value is bool) return (bool) value;
			if (value is string)
				return !string.IsNullOrEmpty((string) value);

			Type type = value.GetType();
			return
				(bool)
				Dispatch(value, "$ToBool$", new Type[] {type}, new object[] {value}, delegate { return CreateBoolConverter(type); });
		}

		public static bool ToBool(decimal value)
		{
			return 0 != value;
		}

		public static bool ToBool(System.Single value)
		{
			return 0 != value;
		}

		public static bool ToBool(System.Double value)
		{
			return 0 != value;
		}

		private static readonly object True = true;
		private static object ToBoolTrue(object value, object[] arguments)
		{
			return True;
		}

		private static object UnboxBooleanDispatcher(object value, object[] arguments)
		{
			return UnboxBoolean(value);
		}
		
		static Dispatcher CreateBoolConverter(Type type)
		{
			MethodInfo method = FindImplicitConversionOperator(type, typeof(bool));
			if (null != method) return EmitImplicitConversionDispatcher(method);
			if (type.IsValueType) return UnboxBooleanDispatcher;
			return ToBoolTrue;
		}
		#endregion

		#region conversion proxy helpers
		internal static MethodInfo FindImplicitConversionOperator(Type from, Type to)
		{
			const BindingFlags ConversionOperatorFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;
			MethodInfo found = FindImplicitConversionMethod(from.GetMethods(ConversionOperatorFlags), from, to);
			if (null != found) return found;
			found = FindImplicitConversionMethod(to.GetMethods(ConversionOperatorFlags), from, to);
			if (null != found) return found;
			return FindImplicitConversionMethod(GetExtensionMethods(), from, to);
		}

		private static IEnumerable<MethodInfo> GetExtensionMethods()
		{
			foreach (MemberInfo member in _extensions.Extensions)
			{
				if (member.MemberType == MemberTypes.Method) yield return (MethodInfo) member;
			}
		}

		private static MethodInfo FindImplicitConversionMethod(System.Collections.Generic.IEnumerable<MethodInfo> candidates, Type from, Type to)
		{
			foreach (MethodInfo m in candidates)
			{
				if (m.Name != "op_Implicit") continue;
				if (m.ReturnType != to) continue;
				ParameterInfo[] parameters = m.GetParameters();
				if (parameters.Length != 1) continue;
				if (!parameters[0].ParameterType.IsAssignableFrom(from)) continue;
				return m;
			}
			return null;
		}

		#endregion

		static void Error(string name, params object[] args)
		{
			throw new ApplicationException(Boo.Lang.ResourceManager.Format(name, args));
		}

		static void Error(string name)
		{
			throw new ApplicationException(Boo.Lang.ResourceManager.GetString(name));
		}

		public static string RuntimeDisplayName
		{
			get {
				Type runtime = Type.GetType("Mono.Runtime");
				return (null != runtime)
					? (string) runtime.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null)
					: string.Concat("CLR ", Environment.Version.ToString());
			}
		}
	}
}
