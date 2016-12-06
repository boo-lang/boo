using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Attribute = Boo.Lang.Compiler.Ast.Attribute;

namespace Boo.Lang.Compiler.Steps
{
    public class CheckAttributeTypes : AbstractTransformerCompilerStep
    {
        private BooCodeBuilder _cb;

        public override void Run()
        {
            _cb = Context.CodeBuilder;
            base.Run();
        }

        public override void OnAttribute(Attribute node)
        {
            foreach (var arg in node.Arguments)
            ;
            foreach (var pair in node.NamedArguments)
            {
                var propType = ((ITypedEntity) pair.First.Entity).Type;
                var valueType = pair.Second.ExpressionType;
                if (propType != valueType)
                    pair.Replace(pair.Second, CoerceType(pair.Second, propType));
            }
        }

        private Expression CoerceType(Expression value, IType type)
        {
            if (type == TypeSystemServices.SingleType)
                return CoerceToSingle(value);
            if (type == TypeSystemServices.DoubleType)
                return CoerceToDouble(value);
            if (type == TypeSystemServices.IntType)
                return CoerceToInt(value);
            if (type == TypeSystemServices.LongType)
                return CoerceToLong(value);
            throw new CompilerError(value, string.Format("Unsupported attribute coerce type: {0}", type));
        }

        private DoubleLiteralExpression CoerceToSingle(Expression value)
        {
            switch (value.NodeType)
            {
                case NodeType.IntegerLiteralExpression:
                    var ile = (IntegerLiteralExpression) value;
                    return new DoubleLiteralExpression(ile.LexicalInfo, ile.Value, true) { ExpressionType = TypeSystemServices.SingleType };
                case NodeType.DoubleLiteralExpression:
                    var dle = (DoubleLiteralExpression)value;
                    return new DoubleLiteralExpression(dle.LexicalInfo, dle.Value, true) { ExpressionType = TypeSystemServices.SingleType };
                default:
                    throw new CompilerError(value, string.Format("Can't coerce type: {0} to single", value.NodeType));
            }
        }

        private DoubleLiteralExpression CoerceToDouble(Expression value)
        {
            switch (value.NodeType)
            {
                case NodeType.IntegerLiteralExpression:
                    var ile = (IntegerLiteralExpression)value;
                    return new DoubleLiteralExpression(ile.LexicalInfo, ile.Value, false) { ExpressionType = TypeSystemServices.DoubleType };
                case NodeType.DoubleLiteralExpression:
                    var dle = (DoubleLiteralExpression)value;
                    return new DoubleLiteralExpression(dle.LexicalInfo, dle.Value, false) { ExpressionType = TypeSystemServices.DoubleType };
                default:
                    throw new CompilerError(value, string.Format("Can't coerce type: {0} to double", value.NodeType));
            }
        }

        private IntegerLiteralExpression CoerceToInt(Expression value)
        {
            if (value.NodeType == NodeType.IntegerLiteralExpression)
            {
                var ile = (IntegerLiteralExpression) value;
                return new IntegerLiteralExpression(ile.LexicalInfo, ile.Value, false) { ExpressionType = TypeSystemServices.IntType };
            }
            throw new CompilerError(value, string.Format("Can't coerce type: {0} to int", value.NodeType));
        }

        private IntegerLiteralExpression CoerceToLong(Expression value)
        {
            if (value.NodeType == NodeType.IntegerLiteralExpression)
            {
                var ile = (IntegerLiteralExpression)value;
                return new IntegerLiteralExpression(ile.LexicalInfo, ile.Value, true) { ExpressionType = TypeSystemServices.LongType };
            }
            throw new CompilerError(value, string.Format("Can't coerce type: {0} to long", value.NodeType));
        }
    }
}
