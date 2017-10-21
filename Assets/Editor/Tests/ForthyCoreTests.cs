using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

[Category("Forthy Core")]
public class ForthyCoreTests
{
    private Forthy _forthy;

    [SetUp]
    public void SetUp()
    {
        _forthy = new Forthy();
    }

    [TearDown]
    public void TearDown()
    {
        if (TestContext.CurrentContext.Result.Outcome == ResultState.Failure)
            Console.WriteLine(_forthy.runtime.ToString());
    }


    [Test]
    public void Basic()
    {
        _forthy.LoadAndRun("1 2 +");
        Assert.AreEqual(1, _forthy.runtime.dataStack.Count);
        Assert.AreEqual(3, _forthy.runtime.dataStack[0].AsInt);
        _forthy.runtime.ClearStack();

        _forthy.LoadAndRun("1.2 2.3 + 3.4 +");
        Assert.AreEqual(1, _forthy.runtime.dataStack.Count);
        Assert.AreEqual(6.9f, _forthy.runtime.dataStack[0].AsFloat, 0.01f);
    } 


}
