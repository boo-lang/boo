
// DO NOT EDIT! File automatically generated from parser error examples

namespace Boo.Lang.Parser
{
    static class GeneratedErrorPatterns
    {
        public static readonly ErrorPattern[] Patterns = {

            new Boo.Lang.Parser.NoViableAltErrorPattern(
                "Unbalanced expression, closing paren not found",
                "paren_expression"
            ) { Token = 22 },
            
            new Boo.Lang.Parser.NoViableAltErrorPattern(
                "Unbalanced expression, closing paren not found",
                "paren_expression"
            ) { Token = 9 },
            
            new Boo.Lang.Parser.NoViableAltErrorPattern(
                "Unbalanced expression, closing paren not found",
                "stmt"
            ) { Token = 77 },
            
            new Boo.Lang.Parser.MismatchedErrorPattern(
                "Unbalanced expression, closing paren not found",
                "slicing_expression"
            ) { Token = 78 },
            
            new Boo.Lang.Parser.MismatchedErrorPattern(
                "Unbalanced expression, closing paren not found",
                "base_types"
            ) { Token = 78 },
            
            new Boo.Lang.Parser.NoViableAltErrorPattern(
                "Unbalanced expression, opening paren not found",
                "parse_module"
            ) { Token = 81 },
            
            new Boo.Lang.Parser.NoViableAltErrorPattern(
                "Unbalanced expression, opening paren not found",
                "assignment_or_method_invocation_with_block_stmt"
            ) { Token = 78 },
            
            new Boo.Lang.Parser.NoViableAltErrorPattern(
                "Unbalanced expression, opening paren not found",
                "stmt"
            ) { Token = 81 },
            
            new Boo.Lang.Parser.MismatchedErrorPattern(
                "Either separate expressions with commas or make sure your parens are balanced",
                "slicing_expression"
            ) { Token = 78 },
            
            new Boo.Lang.Parser.MismatchedErrorPattern(
                "Expressions must be separated by commas",
                "slicing_expression"
            ) { Token = 78 },
            
            new Boo.Lang.Parser.NoViableAltErrorPattern(
                "Expressions must be separated by commas",
                "assignment_or_method_invocation_with_block_stmt"
            ) { Token = 81 },
            
            new Boo.Lang.Parser.MismatchedErrorPattern(
                "Block must be indented",
                "begin_block_with_doc"
            ) { Token = 4 },
            
            new Boo.Lang.Parser.MismatchedErrorPattern(
                "Block must be indented",
                "begin_with_doc"
            ) { Token = 4 },
            
            new Boo.Lang.Parser.NoViableAltErrorPattern(
                "Block must be indented",
                "compound_stmt"
            ) { Token = 89 },
            
            new Boo.Lang.Parser.NoViableAltErrorPattern(
                "Block must be indented",
                "class_definition"
            ) { Token = 4 },
            
            new Boo.Lang.Parser.MismatchedErrorPattern(
                "Block must be indented",
                "begin_with_doc"
            ) { Token = 89 }
   
        };
    }
}

