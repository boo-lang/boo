#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Collections;

namespace Boo.Lang
{
	/// <summary>
	/// boo language builtin functions.
	/// </summary>
	public class Builtins
	{	
		public static void print(string s)
		{
			Console.WriteLine(s);
		}

		public static string gets()
		{
			return Console.ReadLine();
		}

		public static string prompt(string message)
		{
			Console.Write(message);
			return Console.ReadLine();
		}
		
		//[EnumeratorItemType(Type.GetType("System.Object[]"))]
		public static IEnumerable enumerate(object enumerable)
		{			
			return new EnumerateEnumerator(RuntimeServices.GetEnumerable(enumerable).GetEnumerator());
		}
		
		public static void assert(string message, bool condition)
		{
			throw new System.NotImplementedException();
		}
		
		public static void assert(bool condition)
		{
			throw new System.NotImplementedException();
		}
		
		private class EnumerateEnumerator : IEnumerator, IEnumerable
		{
			int _index = -1;
			
			IEnumerator _enumerator;
			
			public EnumerateEnumerator(IEnumerator enumerator)
			{
				if (null == enumerator)
				{
					throw new ArgumentNullException("enumerator");
				}
				_enumerator = enumerator;
			}
			
			public void Reset()
			{
				_index = -1;
				_enumerator.Reset();
			}
			
			public bool MoveNext()
			{				
				if (_enumerator.MoveNext())
				{
					++_index;
					return true;
				}
				return false;
			}
			
			public object Current
			{
				get
				{
					return new object[2] { _index, _enumerator.Current };
				}
			}
			
			public IEnumerator GetEnumerator()
			{
				return this;
			}
		}
		
		private static string GetString(string name)
		{
			return Boo.ResourceManager.GetString(name);
		}
	}
}
