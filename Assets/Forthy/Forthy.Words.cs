using System;
using System.Collections.Generic;

/*
 *  Forthy.Words
 * 
 *  Builtin words
 */

public partial class Forthy
{
    public class SimpleAction : RuntimeAction
    {
        Action<RuntimeContext> _call;

        public SimpleAction(Action<RuntimeContext> call)
        {
            _call = call;
        }

        public override void Execute(RuntimeContext ctx)
        {
            _call(ctx);
            return;
        }
    }

    private static SimpleAction _rPush_action = new SimpleAction((ctx) => {
        ctx.dataStack.Add(ctx.chunk.codes[ctx.pc++]);
    });
    private static SimpleAction _rAdd_action = new SimpleAction((ctx) =>
    {
        var rhs = ctx.dataStack.Pop();
        var lhs = ctx.dataStack.Pop();
        ctx.dataStack.Add(lhs + rhs);
    });

    public static Variant rPush = Variant.MakeRuntimeAction(_rPush_action);

    static Dictionary<string, RuntimeAction> _BULITIN_RUNTIME_DICTIONARY = new Dictionary<string, RuntimeAction>()
    {
        { "+", _rAdd_action },
    };


}
