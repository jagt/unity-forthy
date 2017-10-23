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

    private static Variant rPush = Variant.Make(new SimpleAction(ctx =>
    {
        ctx.dataStack.Add(ctx.chunk.codes[ctx.pc++]);
    }));

    private static Variant rJmp = Variant.Make(new SimpleAction(ctx =>
    {
        ctx.pc = ctx.chunk.codes[ctx.pc].AsInt;
    }));

    private static Variant rJnz = Variant.Make(new SimpleAction(ctx =>
    {
        bool isZero = ctx.dataStack.Pop().AsInt == 0;
        if (isZero)
            ctx.pc += 1;
        else
            ctx.pc = ctx.chunk.codes[ctx.pc].AsInt;
    }));

    private static Variant rJz = Variant.Make(new SimpleAction(ctx =>
    {
        bool isZero = ctx.dataStack.Pop().AsInt == 0;
        if (isZero)
            ctx.pc = ctx.chunk.codes[ctx.pc].AsInt;
        else
            ctx.pc += 1;
    }));

    private static Variant rRun = Variant.Make(new SimpleAction(ctx =>
    {
        var word = ctx.chunk.codes[ctx.pc].AsCSharpString;
        var chunk = ctx.dictionary[word].AsChunk;

        //  store current execution context
        var frame = new StoredFrame()
        {
            chunk = ctx.chunk,
            pc = ctx.pc + 1,
        };
        ctx.storedFrames.Push(frame);

        ctx.chunk = chunk;
        ctx.pc = 0;
    }));

    private static Variant rCreate = Variant.Make(new SimpleAction(ctx =>
    {


    }));

    private static Variant rAdd = Variant.Make(new SimpleAction(ctx =>
    {
        var rhs = ctx.dataStack.Pop();
        var lhs = ctx.dataStack.Pop();
        ctx.dataStack.Add(lhs + rhs);
    }));

    private static Variant rSub = Variant.Make(new SimpleAction(ctx =>
    {
        var rhs = ctx.dataStack.Pop();
        var lhs = ctx.dataStack.Pop();
        ctx.dataStack.Add(lhs - rhs);
    }));
    private static Variant rMul = Variant.Make(new SimpleAction(ctx =>
    {
        var rhs = ctx.dataStack.Pop();
        var lhs = ctx.dataStack.Pop();
        ctx.dataStack.Add(lhs * rhs);
    }));
    private static Variant rDiv = Variant.Make(new SimpleAction(ctx =>
    {
        var rhs = ctx.dataStack.Pop();
        var lhs = ctx.dataStack.Pop();
        ctx.dataStack.Add(lhs / rhs);
    }));

    private static Variant rEq = Variant.Make(new SimpleAction(ctx =>
    {
        var rhs = ctx.dataStack.Pop();
        var lhs = ctx.dataStack.Pop();
        ctx.dataStack.Add(Variant.Make(lhs == rhs));
    }));

    private static Variant rGt = Variant.Make(new SimpleAction(ctx =>
    {
        var rhs = ctx.dataStack.Pop();
        var lhs = ctx.dataStack.Pop();
        ctx.dataStack.Add(Variant.Make(lhs > rhs));
    }));

    private static Variant rLt = Variant.Make(new SimpleAction(ctx =>
    {
        var rhs = ctx.dataStack.Pop();
        var lhs = ctx.dataStack.Pop();
        ctx.dataStack.Add(Variant.Make(lhs < rhs));
    }));

    private static Variant rSwap = Variant.Make(new SimpleAction(ctx =>
    {
        var rhs = ctx.dataStack.Pop();
        var lhs = ctx.dataStack.Pop();
        ctx.dataStack.Add(rhs);
        ctx.dataStack.Add(lhs);
    }));

    private static Variant rDup = Variant.Make(new SimpleAction(ctx =>
    {
        //  Variant is immutable so there's no need to clone
        ctx.dataStack.Add(ctx.dataStack.Last());
    }));

    private static Variant rDrop = Variant.Make(new SimpleAction(ctx =>
    {
        ctx.dataStack.Pop();
    }));

    private static Variant rOver = Variant.Make(new SimpleAction(ctx =>
    {
        ctx.dataStack.Add(ctx.dataStack.Last(-2));
    }));

    private static Variant rDump = Variant.Make(new SimpleAction(ctx =>
    {
        if (ctx.stdout != null)
            ctx.stdout(ctx.ToString());
    }));

    private static Variant rDot = Variant.Make(new SimpleAction(ctx =>
    {
        if (ctx.stdout != null)
            ctx.stdout(ctx.dataStack.Pop().ToString());
    }));

    //  here's a code ordering issue, as static field initialize order depend on declare order
    private static Dictionary<string, Variant> _BULITIN_RUNTIME_DICTIONARY = new Dictionary<string, Variant>()
    {
        //  Arithmetic
        { "+",  rAdd },
        { "-",  rSub },
        { "*",  rMul },
        { "/",  rDiv },
        { "=",  rEq  },
        { ">",  rGt  },
        { "<",  rLt  },

        //  Forth Core Words
        { "swap",   rSwap   },
        { "dup",    rDup    },
        { "over",   rOver   },
        { "drop",   rDrop   },
        { "dump",   rDump   },
        { ".",      rDot    },
    };

}
