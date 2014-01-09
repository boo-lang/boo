// MemberID.cs
// Copyright (C) 2005  Kevin Downs
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
using System;
using System.Reflection;
using System.Text;

namespace NDoc.Core
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class MemberID
	{
		private MemberID()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetMemberID(Type type)
		{
			return "T:" + GetTypeNamespaceName(type);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static string GetMemberID(FieldInfo field)
		{
			return "F:" + GetFullNamespaceName(field) + "." + field.Name;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public static string GetMemberID(PropertyInfo property)
		{
			string memberName;

			memberName = "P:" + GetFullNamespaceName(property) +
				"." + property.Name.Replace('.', '#').Replace('+', '#');

			try
			{
				if (property.GetIndexParameters().Length > 0)
				{
					memberName += "(";

					int i = 0;

					foreach (ParameterInfo parameter in property.GetIndexParameters())
					{
						if (i > 0)
						{
							memberName += ",";
						}

						Type type = parameter.ParameterType;

#if NET_2_0
                        if (type.ContainsGenericParameters)
                        {
                            memberName += "`" + type.GenericParameterPosition.ToString();
                        }
                        else
                        {
                            memberName += type.FullName;
                        }
#else
						memberName += type.FullName;
#endif

						++i;
					}

					memberName += ")";
				}
			}
			catch (System.Security.SecurityException) { }

			return memberName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public static string GetMemberID(MethodBase method)
		{
			string memberName;

			memberName =
				"M:" +
				GetFullNamespaceName(method) +
				"." +
				method.Name.Replace('.', '#').Replace('+', '#');

#if NET_2_0
            if (method.IsGenericMethod)
                memberName = memberName + "``" + method.GetGenericArguments().Length;
#endif

			memberName += GetParameterList(method);

			if (method is MethodInfo)
			{
				MethodInfo mi = (MethodInfo)method;
				if (mi.Name == "op_Implicit" || mi.Name == "op_Explicit")
				{
					memberName += "~" + mi.ReturnType;
				}
			}

			return memberName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventInfo"></param>
		/// <returns></returns>
		public static string GetMemberID(EventInfo eventInfo)
		{
			return "E:" + GetFullNamespaceName(eventInfo) +
				"." + eventInfo.Name.Replace('.', '#').Replace('+', '#');
		}

		private static string GetTypeNamespaceName(Type type)
		{
#if NET_2_0
            if (type.IsGenericType)
            {
                return type.GetGenericTypeDefinition().FullName.Replace('+', '.');
            }
            else
            {
                return type.FullName.Replace('+', '.');
            }
#else
			return type.FullName.Replace('+', '.');
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		public static string GetDeclaringTypeName(MemberInfo member)
		{
			return GetTypeNamespaceName(member.DeclaringType);
		}

		private static string GetFullNamespaceName(MemberInfo member)
		{
			return GetTypeNamespaceName(member.ReflectedType);
		}

#if NET_2_0
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
		public static string GetTypeName(Type type)
        {
            return GetTypeName(type, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="UsePositionalNumber"></param>
        /// <returns></returns>
		public static string GetTypeName(Type type, bool UsePositionalNumber)
        {
            string result = "";
            if (type.IsGenericType)
            {
                // HACK: bug in reflection - namespace sometimes returns null
                string typeNamespace = null;
                try
                {
                    typeNamespace = type.Namespace + ".";
                }
                catch (System.NullReferenceException) { }

                if (typeNamespace == null)
                {
                    int lastDot = type.FullName.LastIndexOf(".");
                    if (lastDot > -1)
                        typeNamespace = type.FullName.Substring(0, lastDot + 1);
                    else
                        typeNamespace = string.Empty;
                }
                //************ end of hack *************************

                string typeName = String.Empty;
                string typeBounds = String.Empty;
                int lastSquareBracket = type.Name.LastIndexOf("[");
                if (lastSquareBracket > -1)
                {
                    typeName = type.Name.Substring(0, lastSquareBracket);
                    typeBounds = type.Name.Substring(lastSquareBracket);
                    typeBounds = typeBounds.Replace(",", ",0:").Replace("[,", "[0:,");
                }
                else
                {
                    typeName = type.Name;
                }

                int genParmCountPos = typeName.IndexOf("`");
                if (genParmCountPos > -1)
                    typeName = typeName.Substring(0, genParmCountPos);

                result = String.Concat(typeNamespace, typeName, GetTypeArgumentsList(type), typeBounds);
            }
            else
            {
                if (type.ContainsGenericParameters)
                {
                    if (type.HasElementType)
                    {
                        Type eleType = type.GetElementType();
                        System.Diagnostics.Debug.Write(eleType.GenericParameterPosition.ToString());
                        if (UsePositionalNumber)
                        {
                            result = "`" + eleType.GenericParameterPosition.ToString();
                        }
                        else
                        {
                            result = eleType.Name;
                        }

                        if (type.IsArray)
                        {
                            int rank = type.GetArrayRank();
                            result += "[";
                            if (rank > 1)
                            {
                                int i = 0;
                                while (i < rank)
                                {
                                    if (i > 0) result += ",";
                                    result += "0:";
                                    i++;
                                }
                            }
                            result += "]";
                        }
                        else if (type.IsByRef)
                        {
                            result += "@";
                        }
                        else if (type.IsPointer)
                        {
                            result += "*";
                        }
                    }
                    else
                    {
                        if (UsePositionalNumber)
                        {
                            result = "`" + type.GenericParameterPosition.ToString();
                        }
                        else
                        {
                            result = type.Name;
                        }
                    }
                }
                else
                {
                    result = type.FullName.Replace("&", "").Replace('+', '#');
                }
            }
            return result;
        }
#else
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetTypeName(Type type)
		{
			// XML Documentation file appends a "@" to reference and out types, not a "&"
			string result = type.FullName.Replace("&", "@").Replace('+', '#');
			return result;
		}

#endif

#if NET_2_0
        private static string GetTypeArgumentsList(Type type)
        {
            StringBuilder argList = new StringBuilder();
            int i = 0;

            foreach (Type argType in type.GetGenericArguments())
            {
                if (i == 0)
                {
                    argList.Append('{');
                }
                else
                {
                    argList.Append(',');
                }

                if (argType.IsGenericType | argType.HasElementType)
                {
                    argList.Append(GetTypeName(argType));
                }
                else if (argType.ContainsGenericParameters)
                {
                    argList.Append('`');
                    argList.Append(argType.GenericParameterPosition.ToString());
                }
                else
                {
                    argList.Append(argType.FullName);
                }

                ++i;
            }

            if (i > 0)
            {
                argList.Append('}');
            }

            // XML Documentation file appends a "@" to reference and out types, not a "&"
            argList.Replace('&', '@');
            argList.Replace('+', '.');

            return argList.ToString();
        }
#endif

		private static string GetParameterList(MethodBase method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			StringBuilder paramList = new StringBuilder();

			int i = 0;

			foreach (ParameterInfo parameter in parameters)
			{
				if (i == 0)
				{
					paramList.Append('(');
				}
				else
				{
					paramList.Append(',');
				}

				Type paramType = parameter.ParameterType;
				paramList.Append(GetTypeName(paramType));

				++i;
			}

			if (i > 0)
			{
				paramList.Append(')');
			}

#if NET_2_0
			if (method.ContainsGenericParameters)
                paramList.Replace("`", "``");
#endif

			return paramList.ToString();
		}

	}
}
