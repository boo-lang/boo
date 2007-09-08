using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Lang.Runtime
{
	internal class GetFieldEmitter : DispatcherEmitter
	{
		private static readonly MethodInfo RunClassConstructor =
			typeof(System.Runtime.CompilerServices.RuntimeHelpers).GetMethod("RunClassConstructor");

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
				_il.Emit(OpCodes.Ldtoken, _field.DeclaringType);
				_il.Emit(OpCodes.Call, RunClassConstructor);
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