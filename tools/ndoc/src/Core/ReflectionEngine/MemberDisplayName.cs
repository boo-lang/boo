// MemberDisplayName.cs
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
	public sealed class MemberDisplayName
	{
		private MemberDisplayName()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetMemberDisplayName(Type type)
		{
			string result = string.Empty;
			if (type.DeclaringType != null) //IsNested?
			{
				result = GetTypeDisplayName(type);
				Type declaringType = type.DeclaringType;
				while (declaringType != null)
				{
					result = GetTypeDisplayName(declaringType) + "." + result;
					declaringType = declaringType.DeclaringType;
				}
			}
			else
			{
				result = GetTypeDisplayName(type);
			}
			return result;
		}


#if NET_2_0
        private static string GetTypeDisplayName(Type type)
        {
            //if (type.HasGenericArguments) type = type.GetGenericTypeDefinition();
            if (type.ContainsGenericParameters)
            {
                String result;
                int i = type.Name.IndexOf('`');
                if (i > -1)
                {
                    result = type.Name.Substring(0, type.Name.IndexOf('`'));
                }
                else
                {
                    result = type.Name;
                }
                result += GetTypeArgumentsList(type);
                return result;
            }
            else
            {
                return type.Name;
            }
        }
#else
		private static string GetTypeDisplayName(Type type)
		{
			return type.Name;
		}
#endif


#if NET_2_0
        private static string GetTypeArgumentsList(Type type)
        {
            StringBuilder argList = new StringBuilder();

            int genArgLowerBound = 0;
            if (type.IsNested)
            {
                Type parent = type.DeclaringType;
                Type[] parentGenArgs = parent.GetGenericArguments();
                genArgLowerBound = parentGenArgs.Length;
            }

            Type[] genArgs = type.GetGenericArguments();
            int i = 0;
            for (int k = genArgLowerBound; k < genArgs.Length; k++)
            {
                Type argType = genArgs[k];
                if (i == 0)
                {
                    argList.Append('<');
                }
                else
                {
                    argList.Append(',');
                }
                if (argType.FullName == null)
                {
                    argList.Append(argType.Name);
                }
                else
                {
                    argList.Append(GetMemberDisplayName(argType));
                }

                ++i;
            }

            if (i > 0)
            {
                argList.Append('>');
            }

            return argList.ToString();
        }
#endif
	}
}
