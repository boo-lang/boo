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

namespace Boo.Lang.Compiler.Bindings
{
	using System;
	using Boo.Lang.Compiler.Services;
	
	public class ExternalEventBinding : IEventBinding
	{
		DefaultBindingService _bindingService;
		
		System.Reflection.EventInfo _event;
		
		public ExternalEventBinding(DefaultBindingService bindingManager, System.Reflection.EventInfo event_)
		{
			_bindingService = bindingManager;
			_event = event_;
		}
		
		public ITypeBinding DeclaringType
		{
			get
			{
				return _bindingService.AsTypeBinding(_event.DeclaringType);
			}
		}
		
		public System.Reflection.EventInfo EventInfo
		{
			get
			{
				return _event;
			}
		}
		
		public bool IsPublic
		{
			get
			{
				return _event.GetAddMethod().IsPublic;
			}
		}
		
		public string Name
		{
			get
			{
				return _event.Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _event.DeclaringType.FullName + "." + _event.Name;
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Event;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return _bindingService.AsTypeBinding(_event.EventHandlerType);
			}
		}
		
		public bool IsStatic
		{
			get
			{
				return false;
			}
		}
	}
}
