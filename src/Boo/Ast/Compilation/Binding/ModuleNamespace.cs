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

namespace Boo.Ast.Compilation.Binding
{
	class ModuleNamespace : INamespace
	{
		Module _module;
		
		BindingManager _bindingManager;
		
		INamespace[] _using;
		
		public ModuleNamespace(BindingManager bindingManager, Module module)
		{
			_bindingManager = bindingManager;
			_module = module;
			_using = new INamespace[_module.Using.Count];
			for (int i=0; i<_using.Length; ++i)
			{
				_using[i] = (INamespace)bindingManager.GetBinding(_module.Using[i]);
			}
		}
		
		public IBinding Resolve(string name)
		{			
			foreach (TypeMember member in _module.Members)
			{
				if (name == member.Name)
				{
					IBinding binding = _bindingManager.GetBinding(member);
					if (BindingType.Type == binding.BindingType)
					{
						binding = _bindingManager.ToTypeReference((ITypeBinding)binding);
					}
					return binding;
				}
			}			
			
			foreach (INamespace ns in _using)
			{
				// todo: resolve name in all namespaces...
				IBinding binding = ns.Resolve(name);
				if (null != binding)
				{					
					return binding;
				}
			}
			return null;
		}
	}
}
