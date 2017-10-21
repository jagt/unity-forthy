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
        var outcome = TestContext.CurrentContext.Result.Outcome;
        if (outcome == ResultState.Error || outcome == ResultState.Failure)
            Console.WriteLine(_forthy.runtime.ToString());
    }

    [Test]
    public void Arithmetic()
    {
        var stack = _forthy.runtime.dataStack;

        _forthy.LoadAndRun("1 2 + 3 +");
        Assert.AreEqual(1, stack.Count);
        Assert.AreEqual(1 + 2 + 3, stack[0].AsInt);
        _forthy.runtime.ClearStack();

        _forthy.LoadAndRun("1.2 2.3 + 3.4 +");
        Assert.AreEqual(1, stack.Count);
        Assert.AreEqual(1.2 + 2.3 + 3.4, stack[0].AsFloat, 0.01f);
        _forthy.runtime.ClearStack();

        _forthy.LoadAndRun("1 2 - 3 -");
        Assert.AreEqual(1, stack.Count);
        Assert.AreEqual(1 - 2 - 3, stack[0].AsInt);
        _forthy.runtime.ClearStack();

        _forthy.LoadAndRun("1.2 2.3 - 3.4 -");
        Assert.AreEqual(1, stack.Count);
        Assert.AreEqual(1.2 - 2.3 - 3.4, stack[0].AsFloat, 0.01f);
        _forthy.runtime.ClearStack();

        _forthy.LoadAndRun("1 2 * 3 *");
        Assert.AreEqual(1, stack.Count);
        Assert.AreEqual(1 * 2 * 3, stack[0].AsInt);
        _forthy.runtime.ClearStack();

        _forthy.LoadAndRun("1.2 2.3 * 3.4 *");
        Assert.AreEqual(1, stack.Count);
        Assert.AreEqual(1.2 * 2.3 * 3.4, stack[0].AsFloat, 0.01f);
        _forthy.runtime.ClearStack();

        _forthy.LoadAndRun("1 2 / 3 /");
        Assert.AreEqual(1, stack.Count);
        Assert.AreEqual(1 / 2 / 3, stack[0].AsInt);
        _forthy.runtime.ClearStack();

        _forthy.LoadAndRun("1.2 2.3 / 3.4 /");
        Assert.AreEqual(1, stack.Count);
        Assert.AreEqual(1.2 / 2.3 / 3.4, stack[0].AsFloat, 0.01f);
        _forthy.runtime.ClearStack();

        _forthy.LoadAndRun("1 2 + 3 =");
        Assert.AreEqual(1, stack.Count);
        Assert.AreEqual(true, stack[0].AsBool);
        _forthy.runtime.ClearStack();

        _forthy.LoadAndRun("1 2 - 3 =");
        Assert.AreEqual(1, stack.Count);
        Assert.AreEqual(false, stack[0].AsBool);

        _forthy.LoadAndRun("1 3 <");
        Assert.AreEqual(1, stack.Count);
        Assert.AreEqual(true, stack[0].AsBool);

        _forthy.LoadAndRun("3.1 1.2 <");
        Assert.AreEqual(1, stack.Count);
        Assert.AreEqual(false, stack[0].AsBool);

        _forthy.LoadAndRun("3 1 >");
        Assert.AreEqual(1, stack.Count);
        Assert.AreEqual(true, stack[0].AsBool);

        _forthy.LoadAndRun("1 3 >");
        Assert.AreEqual(1, stack.Count);
        Assert.AreEqual(false, stack[0].AsBool);
    }

    [Test]
    public void CoreWords()
    {
        var stack = _forthy.runtime.dataStack;

        _forthy.LoadAndRun("1 2 swap drop");
        Assert.AreEqual(1, stack.Count);
        Assert.AreEqual(2, stack[0].AsInt);

        _forthy.LoadAndRun("1 dup");
        Assert.AreEqual(2, stack.Count);
        Assert.AreEqual(1, stack[1].AsInt);

        _forthy.LoadAndRun("1 2 3 over");
        Assert.AreEqual(4, stack.Count);
        Assert.AreEqual(2, stack[3].AsInt);

        string lastOutput = null;
        _forthy.runtime.stdout = (str) =>
        {
            lastOutput = str;
        };

        _forthy.LoadAndRun("\"forthy\" .");
        Assert.AreEqual(lastOutput, "forthy");

        //  TODO handle single quote and white spaced string
        //_forthy.LoadAndRun("'single quote' .");
        //Assert.AreEqual(lastOutput, "forthy");

        _forthy.LoadAndRun("1 2 3 4 5 dump");
        Assert.True(lastOutput.StartsWith("stack:\t[1,2,3,4,5]"));
    }

}
