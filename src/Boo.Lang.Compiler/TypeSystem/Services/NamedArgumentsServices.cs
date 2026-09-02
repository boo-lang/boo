#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem.Services
{
	/// <summary>
	/// An argument written 'name = value' is meant for the parameter it names,
	/// wherever it sits in the list.
	/// </summary>
	public static class NamedArgumentsServices
	{
		public const string NamedArgument = "namedargument";

		/// <summary>
		/// Marks an argument the source wrote as 'name = value'. Only the
		/// parser calls this, so what a macro or a 'case' pattern expands to
		/// never looks named however much it resembles one.
		/// </summary>
		public static void MarkNamedArgument(Expression argument)
		{
			var assignment = argument as BinaryExpression;
			if (assignment == null || assignment.Operator != BinaryOperatorType.Assign)
				return;

			// MemberReferenceExpression derives from ReferenceExpression, and
			// 'a.b = c' names no parameter.
			if (assignment.Left.NodeType == NodeType.ReferenceExpression)
				assignment[NamedArgument] = true;
		}

		// A named argument passes its right hand side; whatever local the
		// assignment declared along the way is dead.
		public static bool IsNamedArgument(Node node)
		{
			return node[NamedArgument] is bool;
		}

		/// <summary>
		/// The name an argument was written with, or null when it was not
		/// written as a named argument at all.
		/// </summary>
		public static string NameOf(Expression argument)
		{
			if (!IsNamedArgument(argument))
				return null;
			return ((ReferenceExpression)((BinaryExpression)argument).Left).Name;
		}

		/// <summary>
		/// The parameter an argument names, or null when it names none.
		/// </summary>
		public static string NamedParameter(Expression argument, IParameter[] parameters)
		{
			var name = NameOf(argument);
			if (name == null)
				return null;

			foreach (var parameter in parameters)
				if (parameter.Name == name)
					return parameter.Name;

			return null;
		}

		/// <summary>
		/// The parameter a misspelling was probably reaching for, allowing one
		/// edit per four characters of the name.
		/// </summary>
		public static string ClosestParameter(string name, IParameter[] parameters)
		{
			var closestDistance = 1 + name.Length / 4;
			string closest = null;
			foreach (var parameter in parameters)
			{
				var distance = StringUtilities.EditDistance(name, parameter.Name);
				if (distance >= closestDistance)
					continue;

				closestDistance = distance;
				closest = parameter.Name;
			}
			return closest;
		}

		private static readonly object DeclaredLocalKey = new object();

		public static void RememberDeclaredLocal(Node argument, InternalLocal local)
		{
			argument[DeclaredLocalKey] = local;
		}

		public static InternalLocal DeclaredLocal(Node argument)
		{
			return argument[DeclaredLocalKey] as InternalLocal;
		}

		public static Expression ValueOf(Expression argument)
		{
			return ((BinaryExpression)argument).Right;
		}

		public static bool HasNamedArgument(ExpressionCollection arguments, IParameter[] parameters)
		{
			foreach (var argument in arguments)
				if (NamedParameter(argument, parameters) != null)
					return true;
			return false;
		}

		public static int SuppliedCount(Expression[] positional)
		{
			var count = 0;
			foreach (var slot in positional)
				if (slot != null)
					++count;
			return count;
		}

		/// <summary>
		/// Lays the call out against a candidate's parameters. False when an
		/// argument names a parameter already spoken for, or names one twice.
		/// </summary>
		public static bool LayOut(ExpressionCollection arguments, IParameter[] parameters, out Expression[] positional)
		{
			string conflict;
			return LayOut(arguments, parameters, out positional, out conflict);
		}

		public static bool LayOut(ExpressionCollection arguments, IParameter[] parameters, out Expression[] positional, out string conflict)
		{
			positional = new Expression[parameters.Length];
			conflict = null;

			var next = 0;
			foreach (var argument in arguments)
			{
				var name = NamedParameter(argument, parameters);
				if (name == null)
				{
					// A positional argument fills the next unclaimed place.
					while (next < positional.Length && positional[next] != null)
						++next;
					if (next >= positional.Length)
						return false;
					positional[next++] = argument;
					continue;
				}

				var index = IndexOf(parameters, name);
				if (positional[index] != null)
				{
					conflict = name;
					return false;
				}
				positional[index] = ValueOf(argument);
			}

			return true;
		}

		private static int IndexOf(IParameter[] parameters, string name)
		{
			for (var i = 0; i < parameters.Length; ++i)
				if (parameters[i].Name == name)
					return i;
			return -1;
		}
	}
}
