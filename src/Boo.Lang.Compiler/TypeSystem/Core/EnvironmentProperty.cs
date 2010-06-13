using System;

namespace Boo.Lang.Compiler.TypeSystem.Core
{
	public class EnvironmentProperty<T>
	{
		private T _value;

		public T Value
		{
			get { return _value;  }
			set
			{
				_value = value;
				OnChange();
			}
		}

		public event System.EventHandler Changed;

		private void OnChange()
		{
			if (Changed == null)
				return;
			Changed(this, EventArgs.Empty);
		}
	}
}