using System;
using System.Collections.Generic;
using System.Text;

namespace Boo.Lang.Compiler.TypeSystem
{
	public abstract class TypeVisitor
	{
		public virtual void Visit(IType type)
		{
			IArrayType arrayType = type as IArrayType;
			if (arrayType != null) VisitArrayType(arrayType);

			if (type.IsByRef) VisitByRefType(type);

			if (type.ConstructedInfo != null) VisitConstructedType(type);

			ICallableType callableType = type as ICallableType;
			if (callableType != null) VisitCallableType(callableType);
		}

		public virtual void VisitArrayType(IArrayType arrayType)
		{
			Visit(arrayType.GetElementType());
		}

		public virtual void VisitByRefType(IType type)
		{
			Visit(type.GetElementType());
		}

		public virtual void VisitConstructedType(IType constructedType)
		{
			Visit(constructedType.ConstructedInfo.GenericDefinition);
			foreach (IType argumentType in constructedType.ConstructedInfo.GenericArguments)
			{
				Visit(argumentType);
			}
		}

		public virtual void VisitCallableType(ICallableType callableType)
		{
			CallableSignature sig = callableType.GetSignature();
			foreach (IParameter parameter in sig.Parameters)
			{
				Visit(parameter.Type);
			}
			Visit(sig.ReturnType);
		}
	}
}
