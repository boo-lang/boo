#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.Pipelines
{
	public class Quack : Run
	{
		public Quack()
		{
			MakeItQuack(this);
		}
		
		public static CompilerPipeline MakeItQuack(CompilerPipeline pipeline)
		{
			int index = pipeline.Find(typeof(Boo.Lang.Compiler.Steps.ProcessMethodBodies));
			pipeline[index] = new Boo.Lang.Compiler.Steps.HuntDucks();
			return pipeline;
		}
	}
}
