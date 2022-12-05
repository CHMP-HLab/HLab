using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace HLab.Compiler.Wpf
{
    public class CompilerCodeParser
    {
        public CompilerCodeParser(string code)
        {
            Code = code;
        }

        const string Alpha = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
        const string Space = " \t\n\r";

        public string Code { get; private set; }

        public int Index { get; set; }

        public string NextClass()
        {
            Index = Code.IndexOf("class", Index, StringComparison.Ordinal);
            return NextWord();
        }

        public string BaseClass()
        {
            var colonPos = Code .IndexOf(':',Index);
            var bracePos = Code.IndexOf('{',Index);
            var semicolonPos = Code.IndexOf(';',Index);
            if (colonPos<0 || bracePos < colonPos || semicolonPos <= colonPos) return "";

            Index = colonPos;
            return NextWord();
        }
        public bool BeforeClassContent()
        { 
            var bracePos = Code.IndexOf('{', Index);
            var semicolonPos = Code.IndexOf(';', Index);

            if(bracePos >= 0 && (bracePos < semicolonPos || semicolonPos < 0))
            {
                Index = bracePos;
                return true;
            }

            if (semicolonPos >= 0 && semicolonPos < bracePos )
            {
                Index = semicolonPos;
                return true;
            }

            return false;
        }

        public bool ClassContent()
        {
            var bracePos = Code.IndexOf('{', Index);
            var semicolonPos = Code.IndexOf(';', Index);
            if (bracePos >= 0 && (bracePos < semicolonPos || semicolonPos < 0))
            {
                Index = ++bracePos;
                //var word = NextWord();
                return true;
            }

            return false;
        }

        public void Insert(string value)
        {
            Code = Code.Insert(Index, value);
            Index += value.Length;
        }

        public void Insert(Action<StringBuilder> build)
        {
            StringBuilder sb = new();
            build(sb);
            Insert(sb.ToString());
        }

        public string NextWord()
        {
            while (!Space.Contains(Code[Index])) Index++;
            while (Space.Contains(Code[Index])) Index++;
            var start = Index;
            while (Alpha.Contains(Code[Index])) Index++;
            var end = Index;
            return Code.Substring(start, end - start);
        }

        public void FormatCode()
        {
            var tree = CSharpSyntaxTree.ParseText(Code);
            var root = tree.GetRoot().NormalizeWhitespace();
            Code = root.ToFullString();
        }

    }
}
