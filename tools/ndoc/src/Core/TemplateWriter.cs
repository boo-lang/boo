#region Copyright © 2002 Jean-Claude Manoli [jc@manoli.net]
/*
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the author(s) be held liable for any damages arising from
 * the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 *   1. The origin of this software must not be misrepresented; you must not
 *      claim that you wrote the original software. If you use this software
 *      in a product, an acknowledgment in the product documentation would be
 *      appreciated but is not required.
 * 
 *   2. Altered source versions must be plainly marked as such, and must not
 *      be misrepresented as being the original software.
 * 
 *   3. This notice may not be removed or altered from any source distribution.
 */ 
#endregion

using System;
using System.IO;

namespace NDoc.Core
{
	/// <summary>
	/// Stream writer that parses a template file to write a new file.
	/// </summary>
	public class TemplateWriter : StreamWriter
	{
		private TextReader template;

		/// <summary>
		/// Initializes a new instance of the <see cref="TemplateWriter"/> class for the specified 
		/// file on the specified path, using the specified template stream and using 
		/// the default encoding and buffer size.
		/// </summary>
		/// <param name="outPath">The complete file path to write to.</param>
		/// <param name="template">The template's stream reader.</param>
		public TemplateWriter(string outPath, TextReader template) : base(outPath)
		{
			this.template = template;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TemplateWriter"/> class for the specified 
		/// stream, using the specified template stream and using the default encoding 
		/// and buffer size.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="template">The template's stream reader.</param>
		public TemplateWriter(Stream stream, TextReader template) : base(stream)
		{
			this.template = template;
		}

		//TODO: write more contructors to handle other text encodings.


		/// <summary>
		/// Copies the text lines form the template to the output stream 
		/// until a specific line is found.
		/// </summary>
		/// <param name="toLine">The line text to search for.  
		/// Must match exactly.</param>
		/// <returns><b>true</b> if the line was found, <b>false</b> if the 
		/// end of the template stream was reached.</returns>
		public bool CopyToLine(string toLine)
		{
			String line;
			while (((line=template.ReadLine()) != null) && (line != toLine)) 
			{
				this.WriteLine(line);
			}
			return (line != null);
		}

		/// <summary>
		/// Copies the text lines form the template to the output stream 
		/// until the end of the template stream.
		/// </summary>
		public void CopyToEnd()
		{
			String line;
			while ((line=template.ReadLine()) != null) 
			{
				this.WriteLine(line);
			}
		}


		/// <summary>
		/// Closes the current StreamWriter and StreamReader.
		/// </summary>
		public override void Close()
		{
			template.Close();
			base.Close();
		}

		/// <summary>
		/// Releases the unmanaged resources used by the TemplateWriter 
		/// and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing"><b>true</b> to release both managed 
		/// and unmanaged resources; <b>false</b> to release only 
		/// unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) 
			{
				template.Close();
			}
			base.Dispose(disposing);
		}
	}
}
