using Clockwork.GearScript.Tokenization;

DefaultTokenSet tokenSet = new DefaultTokenSet();

foreach(TokenDefinition tokenDef in tokenSet)
{
    Console.WriteLine(tokenDef.Name);
}