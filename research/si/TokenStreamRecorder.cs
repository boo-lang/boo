using System;
using System.Collections;
using antlr;

public class TokenStreamRecorder : TokenStream
{
	TokenStreamSelector _selector;
	Queue _queue = new Queue();
	
	public TokenStreamRecorder(TokenStreamSelector selector)
	{
		_selector = selector;
	}
	
	public int Count
	{
		get
		{
			return _queue.Count;
		}
	}
	
	public void Enqueue(Token token)
	{
		_queue.Enqueue(token);
	}
	
	public int RecordUntil(TokenStream stream, int ttype)
	{
		int cTokens = 0;
		
		ods("> RecordUntil");
		Token token = stream.nextToken();
		while (ttype != token.Type)
		{			
			if (token.Type < Token.MIN_USER_TYPE)
			{
				break;
			}
			
			ods("  > {0}", token);
			_queue.Enqueue(token);			
			
			++cTokens;			
			token = stream.nextToken();			
		}
		ods("< RecordUntil");
		return cTokens;
	}
	
	public Token nextToken()
	{
		if (_queue.Count > 0)
		{
			return (Token)_queue.Dequeue();
		}
		return _selector.pop().nextToken();
	}
	
	void ods(string s, params object[] args)
	{
		//Console.WriteLine(s, args);
	}
}
