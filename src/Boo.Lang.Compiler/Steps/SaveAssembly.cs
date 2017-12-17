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

namespace Boo.Lang.Compiler.Steps
{
	using System.IO;
	using System.Reflection;
	using System.Reflection.Emit;

	public class SaveAssembly : AbstractCompilerStep
	{
		override public void Run()
		{
			if (Errors.Count > 0)
				return;

			var builder = ContextAnnotations.GetAssemblyBuilder(Context);
			var filename = Path.GetFileName(Context.GeneratedAssemblyFileName);
			Save(builder, filename);

            var resFilename = (string)Context.Properties["ResFileName"];
            if (resFilename != null)
            {
                File.Delete(resFilename);
            }
        }

        void Save(AssemblyBuilder builder, string filename)
		{
			switch (Parameters.Platform)
			{
				case "x86":
					builder.Save(filename, PortableExecutableKinds.Required32Bit, ImageFileMachine.I386);
					break;
				case "x64":
					builder.Save(filename, PortableExecutableKinds.PE32Plus, ImageFileMachine.AMD64);
					break;
				case "itanium":
					builder.Save(filename, PortableExecutableKinds.PE32Plus, ImageFileMachine.IA64);
					break;
				default: //AnyCPU
					builder.Save(filename);
					break;
			}
		}
	}
}
