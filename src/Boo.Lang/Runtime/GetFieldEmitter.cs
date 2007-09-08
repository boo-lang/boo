using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Boo.Lang.Runtime
{
	internal class GetFieldEmitter : DispatcherEmitter
	{
		protected readonly FieldInfo _field;

		public GetFieldEmitter(FieldInfo field) : base(field.DeclaringType, field.Name)
		{
			_field = field;
		}

		protected override void EmitMethodBody()
		{	
			if (_field.IsStatic)
			{
				// make sure type is initialized before
				// accessing any static fields
				RuntimeHelpers.RunClassConstructor(_field.DeclaringType.TypeHandle);
				_il.Emit(OpCodes.Ldsfld, _field);
			}
			else
			{
				EmitLoadTargetObject(_field.DeclaringType);
				_il.Emit(OpCodes.Ldfld, _field);
			}

			EmitReturn(_field.FieldType);
		}
	}
}