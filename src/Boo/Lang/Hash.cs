#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Boo.Lang
{
	/// <summary>
	/// Hash.
	/// </summary>
	[Serializable]
	public class Hash : Hashtable
	{
		public Hash()
		{
		}
		
		public Hash(IEnumerable enumerable)
		{
			if (null == enumerable)
			{
				throw new ArgumentNullException("enumerable");
			}
			
			foreach (Array tuple in enumerable)
			{
				Add(tuple.GetValue(0), tuple.GetValue(1));
			}
		}
		
		public Hash(bool caseInsensitive) : base(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default)
		{
		}
		
		public Hash(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
