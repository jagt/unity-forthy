using System;
using System.Collections.Generic;
using System.Text;

/*
 *  Forthy
 * 
 *  A minimal Forth dialect in Unity.
 * 
 */

public partial class Forthy
{
    public class ForthyException : Exception
    {
        public ForthyException(string message) : base(message) { }
    }

    public class Chunk
    {
        public List<Variant> codes;

        public Chunk()
        {
            codes = new List<Variant>();
            return;
        }
    }

    public struct StoredFrame
    {
        public Chunk chunk;
        public int pc;
    }

    public class RuntimeContext
    {
        public List<Variant> dataStack;
        public List<Variant> heap;
        public int heapNext;
        public Dictionary<string, Variant> dictionary;
        public Stack<StoredFrame> storedFrames;
        public Chunk chunk;
        public int pc;


        //  optional IO
        public Action<string> stdout;

        public RuntimeContext()
        {
            const int DEFAULT_HEAP_SIZE = 20;
            dataStack = new List<Variant>();
            heap = new List<Variant>(DEFAULT_HEAP_SIZE);
            heapNext = 0;
            for (int ix = 0; ix < DEFAULT_HEAP_SIZE; ix++)
                heap.Add(null);

            storedFrames = new Stack<StoredFrame>();

            dictionary = new Dictionary<string, Variant>(_BULITIN_RUNTIME_DICTIONARY);

            return;
        }

        public void SetMain(Chunk newChunk)
        {
            chunk = newChunk;
            pc = 0;
            ClearStack();

            return;
        }

        public void ClearStack()
        {
            dataStack.Clear();

            return;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("stack:\t[{0}]\n", string.Join(",", dataStack));
            sb.AppendFormat("heap:\t[{0}]\n", string.Join(",", heap));

            return sb.ToString();
        }
    }

    public abstract class RuntimeAction
    {
        public string word;
        public abstract void Execute(RuntimeContext ctx);
    }

    public class CompileContext
    {
        public List<Variant> words;
        public Dictionary<string, CompileAction> dictionary; 

        public CompileContext()
        {
            //  TODO update from default
            dictionary = new Dictionary<string, CompileAction>();

            return;
        }

        public void Reset(List<Variant> newWords)
        {
            words = newWords;
            return;
        }
    }

    public abstract class CompileAction
    {
        public string word;
        public abstract void Execute(CompileContext ctx, Chunk chunk);
    }

    public RuntimeContext runtime;
    public CompileContext compile;
    private Tokenizer _tokenizer;

    public Forthy()
    {
        runtime = new RuntimeContext();
        compile = new CompileContext();
        _tokenizer = new Tokenizer(IsInRuntimeOrCompileDictionary);

        return;
    }

    public void LoadAndRun(string source)
    {
        var chunk = Compile(source);
        runtime.SetMain(chunk);
        RunToDry();

        return;
    }

    private void RunToDry()
    {
        while (runtime.pc < runtime.chunk.codes.Count)
        {
            var cur = runtime.chunk.codes[runtime.pc];
            ForthyUtils.Assert(cur.type == Variant.Type.Action, "pc points at non action");
            runtime.pc = runtime.pc + 1;
            cur.AsAction.Execute(runtime);
        }

        return;
    }

    private Chunk Compile(string source)
    {
        var chunk = new Chunk();
        var codes = chunk.codes;

        compile.Reset(Tokenize(source));
        foreach (var word in compile.words)
        {
            if (word.type != Variant.Type.Word)
            {
                codes.Add(rPush);
                codes.Add(word);
            }
            else
            {
                var wordValue = word.AsCSharpString;
                if (runtime.dictionary.ContainsKey(wordValue))
                {
                    var action = runtime.dictionary[wordValue];
                    codes.Add(action);
                }
                else if (compile.dictionary.ContainsKey(wordValue))
                {
                    var compileAction = compile.dictionary[wordValue];
                    compileAction.Execute(compile, chunk);
                }
            }
        }

        return chunk;
    }

    private bool IsInRuntimeOrCompileDictionary(string token)
    {
        return runtime.dictionary.ContainsKey(token)
            || compile.dictionary.ContainsKey(token);
    }

    private List<Variant> Tokenize(string source)
    {
        return _tokenizer.Tokenize(source);
    }
}


public static class ForthyUtils
{
    public static void Assert(bool cond, string msg)
    {
        if (!cond)
            Error(msg);
    }

    public static void Error(string msg)
    {
        throw new Forthy.ForthyException(msg);
    }

    public static T Pop<T>(this List<T> ls)
    {
        var value = ls[ls.Count - 1];
        ls.RemoveAt(ls.Count - 1);
        return value;
    }

    public static T Last<T>(this List<T> ls, int ix = -1)
    {
        return ls[ls.Count + ix];
    }
}
