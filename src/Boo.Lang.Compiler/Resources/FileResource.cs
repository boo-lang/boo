#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
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

namespace Boo.Lang.Compiler.Resources
{
	using System;
	using System.Resources;
	using System.Collections;
	using Boo.Lang.Compiler;
	
	public class FileResource : ICompilerResource
	{
		string _fname;
		
		public FileResource(string fname)
		{
			if (null == fname)
			{
				throw new ArgumentNullException("fname");
			}
			_fname = fname;
		}
		
		public string Name
		{
			get
			{
				return System.IO.Path.GetFileName(_fname);
			}
		}
		
		public string Description
		{
			get
			{
				return null;
			}
		}
		
		public void WriteResources(System.Resources.IResourceWriter writer)
		{
			using (ResourceReader reader = new ResourceReader(_fname))
			{
				IDictionaryEnumerator e = reader.GetEnumerator();
				while (e.MoveNext())
				{
					writer.AddResource((string)e.Key, e.Value);
				}
			}
		}	
	}
}
