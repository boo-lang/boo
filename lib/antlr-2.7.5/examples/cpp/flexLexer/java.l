%{
#include <stdio.h>
#include "JavaTokenTypes.hpp"
extern void newLine();

char string_buf[1024];
char* string_buf_ptr = 0;

%}

HexDigit			[0-9a-fA-F]
Digit				[0-9]
OctalDigit		[0-7]
NonZeroDigit	[1-9]
AnyButSlash		[^\/]
AnyButAstr		[^\*]

Unicode			[\\][u]{HexDigit}{HexDigit}{HexDigit}{HexDigit}
Octal				[\\]{OctalDigit}({Digit}({Digit})?)?
Escape			[\\]([r]|[n]|[b]|[f]|[t]|[\\]|[']|["])|{Unicode}|{Octal}

IntSuffix		([l]|[L])
DecimalNum		{NonZeroDigit}{Digit}*{IntSuffix}?
OctalNum			[0]{OctalDigit}*{IntSuffix}?
HexNum			[0]([x]|[X]){HexDigit}{HexDigit}*{IntSuffix}?
IntegerLiteral	({DecimalNum}|{OctalNum}|{HexNum})

Sign				([\+]|[\-])
FlSuffix			([f]|[F]|[d][D])
SignedInt		{Sign}?{Digit}+
Expo				([e]|[E])
ExponentPart	{Expo}{SignedInt}?
Float1			{Digit}+[\.]{Digit}+?{ExponentPart}?{FlSuffix}?
Float2			[\.]{Digit}+{ExponentPart}?{FlSuffix}?
Float3			{Digit}+{ExponentPart}{FlSuffix}?
Float4			{Digit}+{FlSuffix}
FloatingPoint	({Float1}|{Float2}|{Float3}|{Float4})

%x string comment
%%
"?"				return QUESTION;
"("				return LPAREN;
")"				return RPAREN;
"["				return LBRACK;
"]"				return RBRACK;
"{"				return LCURLY;
"}"				return RCURLY;
":"				return COLON;
","				return COMMA;
"."				return DOT;
"=="				return EQUAL;
"="				return ASSIGN;
"!"				return LNOT;
"~"				return BNOT;
"!="				return NOT_EQUAL;
"/="				return DIV_ASSIGN;
"/"				return DIV;
"+="				return PLUS_ASSIGN;
"++"				return INC;
"+"				return PLUS;
"-="				return MINUS_ASSIGN;
"--"				return DEC;
"-"				return MINUS;
"*="				return STAR_ASSIGN;
"*"				return STAR;
"%="				return MOD_ASSIGN;
"%"				return MOD;
">>>="			return BSR_ASSIGN;
">>>"				return BSR;
">>="				return SR_ASSIGN;
">>"				return SR;
">="				return GE;
">"				return GT;
"<<="				return SL_ASSIGN;
"<<"				return SL;
"<="				return LE;
"<"				return LT_;
"^="				return BXOR_ASSIGN;
"^"				return BXOR;
"|="				return BOR_ASSIGN;
"||"				return LOR;
"|"				return BOR;
"&="				return BAND_ASSIGN;
"&&"				return LAND;
"&"				return BAND;
";"				return SEMI;

[ \f\t]			;
"\r\n"|\r|\n	{ newLine(); }

"//".*			;

[\']({Escape}|[^\'])[\']	return CHAR_LITERAL;

"/*" 	BEGIN(comment);
<comment>{
[^*\n]*        /* eat anything that's not a '*' */
"*"+[^*/\n]*   /* eat up '*'s not followed by '/'s */
\n             { newLine(); }
"*"+"/"        BEGIN(INITIAL);
}

[\"] {
	BEGIN(string);
	string_buf_ptr = string_buf;
}
<string>{
\" {
	BEGIN(INITIAL);
	*string_buf_ptr = '\0';
	return STRING_LITERAL;
}
{Octal} {
	int result;
	sscanf( yytext + 1, "%o", &result );
	*string_buf_ptr++ = result;
}
{Unicode} {
	int result;
	sscanf( yytext + 1, "%x", &result );
	*string_buf_ptr++ = result;
}
\\n  		*string_buf_ptr++ = '\n';
\\t  		*string_buf_ptr++ = '\t';
\\r  		*string_buf_ptr++ = '\r';
\\b  		*string_buf_ptr++ = '\b';
\\f		*string_buf_ptr++ = '\f';
\\(.|\n)	*string_buf_ptr++ = yytext[1];
[^\\\n\"]+ {
	char *yptr = yytext;

	while ( *yptr )
		*string_buf_ptr++ = *yptr++;
	}
}	// end string states

while				return LITERAL_while;
volatile			return LITERAL_volatile;
void				return LITERAL_void;
try				return LITERAL_try;
true				return LITERAL_true;
transient		return LITERAL_transient;
throws			return LITERAL_throws;
throw				return LITERAL_throw;
threadsafe		return LITERAL_threadsafe;
this				return LITERAL_this;
synchronized	return LITERAL_synchronized;
switch			return LITERAL_switch;
super				return LITERAL_super;
static			return LITERAL_static;
short				return LITERAL_short;
return			return LITERAL_return;
public			return LITERAL_public;
protected		return LITERAL_protected;
private			return LITERAL_private;
package			return LITERAL_package;
null				return LITERAL_null;
new				return LITERAL_new;
native			return LITERAL_native;
long				return LITERAL_long;
interface		return LITERAL_interface;
int				return LITERAL_int;
instanceof		return LITERAL_instanceof;
import			return LITERAL_import;
implements		return LITERAL_implements;
if					return LITERAL_if;
for				return LITERAL_for;
float				return LITERAL_float;
finally			return LITERAL_finally;
final				return FINAL;
false				return LITERAL_false;
extends			return LITERAL_extends;
else				return LITERAL_else;
double			return LITERAL_double;
do					return LITERAL_do;
default			return LITERAL_default;
continue			return LITERAL_continue;
class				return LITERAL_class;
char				return LITERAL_char;
catch				return LITERAL_catch;
case				return LITERAL_case;
byte				return LITERAL_byte;
break				return LITERAL_break;
boolean			return LITERAL_boolean;
abstract			return ABSTRACT;

[a-zA-Z_$][a-zA-Z_0-9$]*	return IDENT;

{IntegerLiteral}	return NUM_INT;
{FloatingPoint}	return NUM_FLOAT;

%%
