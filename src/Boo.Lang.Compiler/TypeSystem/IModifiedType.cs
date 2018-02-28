using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boo.Lang.Compiler.TypeSystem
{
	interface IModifiedType : IType
	{
		IType[] ModReqs { get;  }

		IType[] ModOpts { get; }
	}
}
