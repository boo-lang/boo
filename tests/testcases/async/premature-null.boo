"""
in FindReferencesInDocumentAsync
in GetTokensWithIdentifierAsync
in FindReferencesInTokensAsync
tokens were fine
document was fine
done!
"""

import System
import System.Collections.Generic
import System.Diagnostics
import System.Linq
import System.Text
import System.Threading
import System.Threading.Tasks

static class Program:
    [async] internal def GetTokensWithIdentifierAsync() as Task[of string]:
        Console.WriteLine("in GetTokensWithIdentifierAsync")
        return "GetTokensWithIdentifierAsync"

    [async] protected def FindReferencesInTokensAsync(document as string, tokens as string) as Task[of string]:
        Console.WriteLine("in FindReferencesInTokensAsync")
        if tokens is null: raise NullReferenceException("tokens")
        Console.WriteLine("tokens were fine")
        if document is null: raise NullReferenceException("document")
        Console.WriteLine("document was fine")
        return "FindReferencesInTokensAsync"

    [async] public def FindReferencesInDocumentAsync(document as string) as Task[of string]:
        Console.WriteLine("in FindReferencesInDocumentAsync")
        if document is null: raise NullReferenceException("document")
        var nonAliasReferences = await(FindReferencesInTokensAsync(
            document,
            await(GetTokensWithIdentifierAsync())
            ).ConfigureAwait(true))
        return "done!"

public def Main(args as (string)):
    try:
        var ar = Program.FindReferencesInDocumentAsync("Document")
        ar.Wait(1000)
        Console.WriteLine(ar.Result)
    except ex as Exception:
        Console.WriteLine(ex)
