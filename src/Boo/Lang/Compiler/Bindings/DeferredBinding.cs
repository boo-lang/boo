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

namespace Boo.Lang.Compiler.Bindings
{
	using System;
	using Boo.Lang.Ast;	 	
	
	public class DeferredBindingResolvedArgs : EventArgs
	{
		public IBinding Binding;
		public Node Node;
		
		public DeferredBindingResolvedArgs(IBinding binding, Node node)
		{
			Binding = binding;
			Node = node;
		}
	}
	
	public delegate void DeferredBindingResolvedHandler(object sender, DeferredBindingResolvedArgs args);

	/// <summary>
	/// A binding for nodes that depend on other nodes or
	/// external conditions before they can be bound/resolved.
	/// </summary>
	public class DeferredBinding : AbstractInternalBinding, IBinding
	{
		protected Node _node;
		
		protected DeferredBindingResolvedHandler _handler;
		
		public DeferredBinding(Node node, DeferredBindingResolvedHandler handler)
		{			
			if (null == node)
			{
				throw new ArgumentNullException("node");
			}
			
			if (null == handler)
			{
				throw new ArgumentNullException("handler");
			}			
			
			_node = node;
			_handler = handler;
		}
		
		public virtual BindingType BindingType
		{
			get
			{
				return BindingType.Deferred;
			}
		}
		
		public string Name
		{
			get
			{
				return "Deferred";
			}
		}		
		
		public void OnDependencyResolved(object sender, EventArgs args)
		{
			_handler(this, new DeferredBindingResolvedArgs((IBinding)sender, _node));
			base.OnResolved();
		}
	}
}
