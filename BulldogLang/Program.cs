using Antlr4.Runtime;
using System;

namespace AntlrConsole2
{


    public class Program
    {
        /*
        private string TokenToString(IToken token, Lexer lexer)
        {
            string str = string.Empty;
            if (token.Channel > 0)
                str = ",channel=" + (object)token.Channel;
            string text = token.Text;
            string typeName = token.Type > 0 && token.Type < this.tokenSymbolicNames.Length ? this.tokenSymbolicNames[token.Type] : token.Type.ToString();

            return "[@" + (object)token.TokenIndex + "," + (object)token.StartIndex + ":" + (object)token.StopIndex +
                   "='" + (text == null
                       ? "<no text>"
                       : text.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t")) + "',<" + typeName + ">" + str + "," + (object)token.Line + ":" + (object)token.Column + "]";
        }
        */


        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }

        void Run()
        {
            //            var input = "DECLARE";
            var input =
                "DECLARE junk AS SOURCE BEGIN\n" +
                "   READ FROM SQLServer\n" +
                "   COLUMNS [This, that, other1, other2]\n" +
                "   USING CONNECT STRING \"Trusted_Connection=TRUE;Server=Lake;Database=Bulldog;\"\n" +
                "END\n" +
                "DECLARE fooey AS DESTINATION BEGIN\n" +
                "   WITH INPUT FROM junk\n" +
                "   COLUMNS [*] \n" +
                "   WRITE TO SQLServer\n" +
                "   USING CONNECT STRING \"Trusted_Connection=TRUE;Server=Lake;Database=Bulldog;\"\n" +
                "END\n";
            var str = new AntlrInputStream(input);
            Console.WriteLine(str.ToString());
            var lexer = new bulldogLexer(str);
            Console.WriteLine(lexer.ToString());
            var tokens = new CommonTokenStream(lexer);
            tokens.Fill();

            foreach (var token in tokens.GetTokens())
            {
                /*
                if (token is CommonToken)
                {
                    Console.WriteLine(this.TokenToString(token, lexer));
                }
                else
                */
                {
                    Console.WriteLine(token.ToString());
                }
            }


            var parser = new bulldogParser(tokens);
            var listener = new ErrorListener<IToken>();
            parser.AddErrorListener(listener);
            var tree = parser.file();
            if (listener.had_error)
            {
                Console.WriteLine("error in parse.");
            }
            else
            {
                Console.WriteLine("parse completed.");
                Console.WriteLine(tokens.OutputTokens());
                Console.WriteLine(tree.OutputTree(tokens));
            }
            var visitor = new BulldogVisitor();
            visitor.Visit(tree);
        }
    }
}
