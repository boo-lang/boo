
// DO NOT EDIT! File automatically generated from parser error examples

namespace Boo.Lang.Parser
{
    static class GeneratedErrorPatterns
    {
        public static List<ErrorPattern> Patterns
        {
            get
            {
                List<ErrorPattern> list = new List<ErrorPattern>();

                list.Add(
                    new Boo.Lang.Parser.NoViableAltErrorPattern(
                        "Unbalanced expression, closing paren not found",
                        "array_or_expression"
                    )
                    
                );
                list.Add(
                    new Boo.Lang.Parser.NoViableAltErrorPattern(
                        "Unbalanced expression, closing paren not found",
                        "array_or_expression"
                    )
                    
                );
                list.Add(
                    new Boo.Lang.Parser.MismatchedErrorPattern(
                        "Unbalanced expression, closing paren not found",
                        "argument"
                    )
                    
                );
                list.Add(
                    new Boo.Lang.Parser.NoViableAltErrorPattern(
                        "Unbalanced expression, opening paren not found",
                        "array_or_expression"
                    )
                    
                );
                list.Add(
                    new Boo.Lang.Parser.MismatchedErrorPattern(
                        "Either separate expressions with commas or make sure your parens are balanced",
                        "argument"
                    )
                    
                );
                list.Add(
                    new Boo.Lang.Parser.MismatchedErrorPattern(
                        "Expressions must be separated by commas",
                        "argument"
                    )
                    
                );
                list.Add(
                    new Boo.Lang.Parser.NoViableAltErrorPattern(
                        "Expressions must be separated by commas",
                        "array_or_expression"
                    )
                    
                );
                list.Add(
                    new Boo.Lang.Parser.MismatchedErrorPattern(
                        "Block must be indented",
                        "docstring"
                    )
                    
                );
                list.Add(
                    new Boo.Lang.Parser.MismatchedErrorPattern(
                        "Block must be indented",
                        "docstring"
                    )
                    
                );
                list.Add(
                    new Boo.Lang.Parser.NoViableAltErrorPattern(
                        "Block must be indented",
                        "expression"
                    )
                    
                );
   
                return list;
            }
        }
    }
}

