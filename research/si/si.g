options
{
	language="CSharp";
}
{
	using System.Collections;
}
class SIParser extends Parser;
options
{
	defaultErrorHandler=false;
	exportVocab = SI;
}
tokens
{
	RBRACE; // used by esi.g
	ESEPARATOR;
	PRINT="print";
}
{
	protected Hashtable _variables = new Hashtable();
	
	protected ArrayList _elist = new ArrayList();
	
	public static void Main(string[] args)
	{
		antlr.TokenStreamSelector selector = new antlr.TokenStreamSelector();
		SILexer lexer = new SILexer(Console.In);
		lexer.Initialize(selector);
		
		SIParser parser = new SIParser(selector);
		try
		{
			parser.start();
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}
	}

	int Eval(string op, int l, int r)
	{
		switch (op)
		{
			case "+": return l+r;
			case "-": return l-r;
			case "*": return l*r;
			case "/": return l/r;
		}
		throw new ArgumentException(string.Format("Unknown operator: {0}!", op));
	}

	int GetVar(string name)
	{
		object value = _variables[name];
		if (null != value)
		{
			return (int)value;
		}
		throw new ArgumentException(string.Format("Undeclared variable: {0}", name));
	}

	void SetVar(string name, int value)
	{
		_variables[name] = value;
	}
}

start: (
			(
				assign | print
			)
			(EOS)+
		)*;

assign { int value = -1; }:
		id:ID ASSIGN value=expression
		{ SetVar(id.getText(), value); }
		;

print { object value = null; }:
		PRINT (value=expression | value=string_interpolation)
		{ Console.WriteLine(value); }
		;

expression returns [int value] { value=-1; int rvalue; }:
	value=term
	(
		op:SUM_OPERATOR
		rvalue=term
		{ value = Eval(op.getText(), value, rvalue); }
	)*
	;

term returns [int value] { value=-1; int rvalue; }:
		value=atom
		(
			op:MULT_OPERATOR
			rvalue=atom
			{ value = Eval(op.getText(), value, rvalue); }
		)*
		;
		
atom returns [int value] { value=-1; }:
		i:INT { value = int.Parse(i.getText()); } |
		id:ID { value = GetVar(id.getText()); }
		;
		
string_interpolation returns [string value]
		{
			value = string.Empty;
			_elist.Clear();
			int evalue = 0;
		}:
		dqs:DOUBLE_QUOTED_STRING { value = dqs.getText(); }
		(
			ESEPARATOR
			evalue=expression { _elist.Add(evalue); }			
		)*
		{
			if (_elist.Count > 0)
			{
				value = string.Format(value, _elist.ToArray());
			}
		}		
		;

class SILexer extends Lexer;
options
{
	testLiterals = false;
	k = 2;
	charVocabulary='\u0003'..'\uFFFF';
	// without inlining some bitset tests, ANTLR couldn't do unicode;
	// They need to make ANTLR generate smaller bitsets;
	codeGenBitsetTestThreshold=20;
	defaultErrorHandler=false;
}
{
	antlr.Token _eseparator = new antlr.CommonToken(ESEPARATOR, "SEPARATOR");
	
	int _eindex = 0;
	
	ExpressionLexer _el;
	
	TokenStreamRecorder _erecorder;
	
	antlr.TokenStreamSelector _selector;
	
	internal void Initialize(antlr.TokenStreamSelector selector)
	{
		_selector = selector;
		_el = new ExpressionLexer(getInputState());
		_erecorder = new TokenStreamRecorder(selector);		
		selector.select(this);
	}
}
ID options { testLiterals = true; }: ID_LETTER (ID_LETTER | DIGIT)*;

INT: (DIGIT)+;

ASSIGN: '=';

SUM_OPERATOR : '+' | '-' ;

MULT_OPERATOR: '*' | '/' ;

WS: (' ' | '\t')+ { $setType(Token.SKIP); };

EOS: (
		('\r' | '\n') { newline(); } |
		';'
	 )
	;

DOUBLE_QUOTED_STRING :
		'"'! { _eindex = 0; } 
		(
			DQS_ESC |
			ESCAPED_EXPRESSION |
			~('"' | '\\' | '$')
		)*
		'"'!
		{
			if (_erecorder.Count > 0)
			{
				_selector.push(_erecorder);
			}
		}
		;
		
protected
ESCAPED_EXPRESSION : "${"!
		{			
			_erecorder.Enqueue(_eseparator);
			if (_erecorder.RecordUntil(_el, RBRACE) > 0)
			{
				$setText("{" + _eindex + "}");
				++_eindex;
			}
		}
		;

protected
DQS_ESC :
	'\\'
	(
		'$' | 'r' | 'n' | 't' | '\\' | '"' 
	)
	;
	
protected
ID_LETTER : ('_' | 'a'..'z' | 'A'..'Z' );

protected
DIGIT : '0'..'9';
