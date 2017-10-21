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
            ForthyUtils.Assert(_call != null, "missing call");
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
    private static SimpleAction _rSub_action = new SimpleAction((ctx) =>
    {
        var rhs = ctx.dataStack.Pop();
        var lhs = ctx.dataStack.Pop();
        ctx.dataStack.Add(lhs - rhs);
    });
    private static SimpleAction _rMul_action = new SimpleAction((ctx) =>
    {
        var rhs = ctx.dataStack.Pop();
        var lhs = ctx.dataStack.Pop();
        ctx.dataStack.Add(lhs * rhs);
    });
    private static SimpleAction _rDiv_action = new SimpleAction((ctx) =>
    {
        var rhs = ctx.dataStack.Pop();
        var lhs = ctx.dataStack.Pop();
        ctx.dataStack.Add(lhs / rhs);
    });

    private static SimpleAction _rEq_action = new SimpleAction((ctx) =>
    {
        var rhs = ctx.dataStack.Pop();
        var lhs = ctx.dataStack.Pop();
        ctx.dataStack.Add(Variant.Make(lhs == rhs));
    });

    private static SimpleAction _rGt_action = new SimpleAction((ctx) =>
    {
        var rhs = ctx.dataStack.Pop();
        var lhs = ctx.dataStack.Pop();
        ctx.dataStack.Add(Variant.Make(lhs > rhs));
    });

    private static SimpleAction _rLt_action = new SimpleAction((ctx) =>
    {
        var rhs = ctx.dataStack.Pop();
        var lhs = ctx.dataStack.Pop();
        ctx.dataStack.Add(Variant.Make(lhs < rhs));
    });

    private static SimpleAction _rSwap_action = new SimpleAction((ctx) =>
    {
        var rhs = ctx.dataStack.Pop();
        var lhs = ctx.dataStack.Pop();
        ctx.dataStack.Add(rhs);
        ctx.dataStack.Add(lhs);
    });

    private static SimpleAction _rDup_action = new SimpleAction((ctx) =>
    {
        //  Variant is immutable so there's no need to clone
        ctx.dataStack.Add(ctx.dataStack.Last());
    });

    private static SimpleAction _rDrop_action = new SimpleAction((ctx) =>
    {
        ctx.dataStack.Pop();
    });

    private static SimpleAction _rOver_action = new SimpleAction((ctx) =>
    {
        ctx.dataStack.Add(ctx.dataStack.Last(-2));
    });

    private static SimpleAction _rDump_action = new SimpleAction((ctx) =>
    {
        if (ctx.stdout != null)
            ctx.stdout(ctx.ToString());
    });

    private static SimpleAction _rDot_action = new SimpleAction((ctx) =>
    {
        if (ctx.stdout != null)
            ctx.stdout(ctx.dataStack.Pop().ToString());
    });

    public static Variant rPush = Variant.Make(_rPush_action);

    //  here's a code ordering issue, as static field initialize order depend on declare order
    private static Dictionary<string, RuntimeAction> _BULITIN_RUNTIME_DICTIONARY = new Dictionary<string, RuntimeAction>()
    {
        //  Arithmetic
        { "+",  _rAdd_action },
        { "-",  _rSub_action },
        { "*",  _rMul_action },
        { "/",  _rDiv_action },
        { "=",  _rEq_action  },
        { ">",  _rGt_action  },
        { "<",  _rLt_action  },

        //  Forth Core Words
        { "swap",   _rSwap_action   },
        { "dup",    _rDup_action    },
        { "over",   _rOver_action   },
        { "drop",   _rDrop_action   },
        { "dump",   _rDump_action   },
        { ".",      _rDot_action    },
    };

}
