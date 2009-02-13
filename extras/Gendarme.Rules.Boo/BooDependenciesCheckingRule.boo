"""
DSL-representation of disallowed dependencies within Boo project.
Checking is done by running permissions chains in order (iptables-like).
"""
namespace Gendarme.Rules.Boo

import Gendarme.Rules.Abstract

dependencies Boo:
	within Boo.Lang.Compiler.Ast:
		allow Boo.Lang.Compiler.TypeSystem, Interface, NonVisible
		deny Boo.Lang.Compiler.TypeSystem #deny anything else

