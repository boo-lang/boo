using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Lang.Runtime
{
	class SetFieldEmitter : DispatcherEmitter
	{
		private static readonly MethodInfo Type_GetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");
		private static readonly MethodInfo RuntimeServices_Coerce = typeof(RuntimeServices).GetMethod("Coerce");

		private readonly FieldInfo _field;

		public SetFieldEmitter(FieldInfo field)
			: base(field.DeclaringType, field.Name + "=")
		{
			_field = field;
		}

		protected override void EmitMethodBody()
		{
			LocalBuilder value = DeclareLocal(_field.FieldType);
			EmitLoadValue();
			StoreLocal(value);

			if (_field.IsStatic)
			{
				LoadLocal(value);
				_il.Emit(OpCodes.Stsfld, _field);
			}
			else
			{
				EmitLoadTargetObject(_field.DeclaringType);
				LoadLocal(value);
				_il.Emit(OpCodes.Stfld, _field);
			}

			LoadLocal(value);
			EmitReturn(_field.FieldType);
		}

		private void LoadLocal(LocalBuilder value)
		{
			_il.Emit(OpCodes.Ldloc, value);
		}

		private void StoreLocal(LocalBuilder value)
		{
			_il.Emit(OpCodes.Stloc, value);
		}

		private LocalBuilder DeclareLocal(Type type)
		{
			return _il.DeclareLocal(type);
		}

		private void EmitLoadValue()
		{
			EmitArgArrayElement(0);
			EmitLoadType(_field.FieldType);
			_il.Emit(OpCodes.Call, RuntimeServices_Coerce);
			EmitCastOrUnbox(_field.FieldType);
		}

		private void EmitLoadType(Type type)
		{
			_il.Emit(OpCodes.Ldtoken, type);
			_il.Emit(OpCodes.Call, Type_GetTypeFromHandle);
		}
	}
}
