using Antlr4.Runtime;
using NUnit.Framework;
using Bulldog;
using System;

namespace BulldogUnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }

        bulldogParser makeParser(string input)
        {
            var str = new AntlrInputStream(input);
            var lexer = new bulldogLexer(str);
            var tokens = new CommonTokenStream(lexer);
            tokens.Fill();
            var parser = new bulldogParser(tokens);

            return parser;
        }

        /// <summary>
        /// Test that we parse a source-dest and get
        /// all the objects and attributes we expect
        /// </summary>
        [Test]
        public void TestSourceDest()
        {
            string input =
                "DECLARE junk AS SOURCE BEGIN\n" +
                "   READ FROM SQLServer\n" +
                "   FROM TABLE SimpleSource\n" +
                "   COLUMNS [This, that, other1, other2]\n" +
                "   USING CONNECT STRING \"Trusted_Connection=TRUE;Server=localhost;Database=Bulldog;\"\n" +
                "END\n" +
                "DECLARE fooey AS DESTINATION BEGIN\n" +
                "   WITH INPUT FROM junk\n" +
                "   COLUMNS [*] \n" +
                "   INTO TABLE SimpleDest\n" +
                "   WRITE TO SQLServer\n" +
                "   USING CONNECT STRING \"Trusted_Connection=TRUE;Server=localhost;Database=Bulldog;\"\n" +
                "END\n";

            var parser = makeParser(input);
            var listener = new ErrorListener<IToken>();
            parser.AddErrorListener(listener);
            var tree = parser.file();
            Assert.IsTrue(!listener.had_error);
            var visitor = new BulldogVisitor();
            visitor.Visit(tree);


            Assert.Pass();
        }

        /// <summary>
        /// Test that we get an exception when we try to reuse a name
        /// </summary>
        [Test]
        public void TestReusedNameError()
        {
            Exception ex = Assert.Throws<Exception>(delegate { TestReusedNameErrorDelegate(); } );
            Assert.That(ex.Message, Is.EqualTo("object named junk already exists"));
        }

        void TestReusedNameErrorDelegate()
        {
            string input =
                "DECLARE junk AS SOURCE BEGIN\n" +
                "   READ FROM SQLServer\n" +
                "   FROM TABLE SimpleSource\n" +
                "   COLUMNS [This, that, other1, other2]\n" +
                "   USING CONNECT STRING \"Trusted_Connection=TRUE;Server=localhost;Database=Bulldog;\"\n" +
                "END\n" +
                "DECLARE junk AS DESTINATION BEGIN\n" +
                "   WITH INPUT FROM junk\n" +
                "   COLUMNS [*] \n" +
                "   INTO TABLE SimpleDest\n" +
                "   WRITE TO SQLServer\n" +
                "   USING CONNECT STRING \"Trusted_Connection=TRUE;Server=localhost;Database=Bulldog;\"\n" +
                "END\n";

            var parser = makeParser(input);
            var listener = new ErrorListener<IToken>();
            parser.AddErrorListener(listener);
            var tree = parser.file();
            Assert.IsTrue(!listener.had_error);
            var visitor = new BulldogVisitor();
            visitor.Visit(tree);

            Assert.Pass();
        }
    }
}