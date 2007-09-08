using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Lang.Runtime
{
	class SetFieldEmitter : DispatcherEmitter
	{
		private readonly FieldInfo _field;

		public SetFieldEmitter(FieldInfo field)
			: base(field.DeclaringType)
		{
			_field = field;
		}

		protected override void EmitMethodBody()
		{
			if (_field.IsStatic)
			{
				EmitLoadValue();
				_il.Emit(OpCodes.Stsfld, _field);
			}
			else
			{
				EmitLoadTargetObject(_field.DeclaringType);
				EmitLoadValue();
				_il.Emit(OpCodes.Stfld, _field);
			}
			EmitLoadValue();
			EmitReturn(_field.FieldType);
		}

		private void EmitLoadValue()
		{
			EmitArgArrayElement(0);
			EmitCastOrUnbox(_field.FieldType);
		}
	}
}
