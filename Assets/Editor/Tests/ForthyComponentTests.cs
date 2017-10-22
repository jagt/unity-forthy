using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

[Category("Forty Component")]
public class ForthyComponentTests
{
    [Test]
    public void Tokenizer()
    {
        var words = new HashSet<string>();
        var tokenizer = new Forthy.Tokenizer(x => words.Contains(x));

        List<Forthy.Variant> tokens;
        tokens = tokenizer.Tokenize("  'single quoted string'  ");
        Assert.AreEqual(1, tokens.Count);
        Assert.AreEqual(Forthy.Variant.Type.String, tokens[0].type);
        Assert.AreEqual("single quoted string", tokens[0].AsCSharpString);

        tokens = tokenizer.Tokenize("  \"double quoted string\"  ");
        Assert.AreEqual(1, tokens.Count);
        Assert.AreEqual(Forthy.Variant.Type.String, tokens[0].type);
        Assert.AreEqual("double quoted string", tokens[0].AsCSharpString);

        words.Add("word");
        tokens = tokenizer.Tokenize(@" word 1 # comments
            'string' 2.3
        ");
        Assert.AreEqual(4, tokens.Count);
        Assert.AreEqual(Forthy.Variant.Type.Word, tokens[0].type);
        Assert.AreEqual("word", tokens[0].AsCSharpString);
        Assert.AreEqual(1, tokens[1].AsInt);
        Assert.AreEqual(Forthy.Variant.Type.String, tokens[2].type);
        Assert.AreEqual("string", tokens[2].AsCSharpString);
        Assert.AreEqual(2.3f, tokens[3].AsFloat, 0.01f);

        return;
    }
}
