namespace AntlrConsole2
{
    using System;
    using System.Collections.Generic;
    using Antlr4.Runtime.Misc;

    class BulldogVisitor : bulldogBaseVisitor<string>
    {
        Dictionary<string, DataComponent> mapNamesToComponents = new Dictionary<string, DataComponent>(StringComparer.OrdinalIgnoreCase);
        DataSource source = null;
        DataDestination dest = null;

        public BulldogVisitor()
        {
        }

        public override string VisitAggregate_declaration([NotNull] bulldogParser.Aggregate_declarationContext context)
        {
            return base.VisitAggregate_declaration(context);
        }

        public override string VisitColumns([NotNull] bulldogParser.ColumnsContext context)
        {
            return base.VisitColumns(context);
        }

        public override string VisitColumn_list([NotNull] bulldogParser.Column_listContext context)
        {
            foreach (var x in context.column())
            {
                Console.WriteLine($" column list entry: {x.GetText()}");
                if (source != null)
                    source.AddColumn(x.GetText());
            }

            return base.VisitColumn_list(context);
        }

        public override string VisitDeclaration([NotNull] bulldogParser.DeclarationContext context)
        {
            return base.VisitDeclaration(context);
        }

        public override string VisitDest_declaration([NotNull] bulldogParser.Dest_declarationContext context)
        {
            Console.WriteLine($"Dest_Declaration: {context.w.Text.ToString()}");

            // see if we can find a write to here
            this.dest = null;
            for (int i = 0; i < context.ChildCount; i++)
            {
                var c = context.GetChild(i) as bulldogParser.Write_toContext;
                if (c != null)
                {
                    // already have one?
                    if (this.dest != null)
                    {
                        throw new Exception($"can't have multiple WRITE TO statements at ~~~");
                    }

                    switch (c.s.Text.ToString().ToUpper())
                    {
                        case "SQLSERVER":
                            this.dest = new SQLServerDataDestination(context.w.Text.ToString());
                            break;

                        default:
                            break;
                    }
                }
            }

            if (this.dest == null)
            {
                throw new Exception($"must have WRITE TO statement at ~~~~");
            }

            // we now have a source object, so populate it
            this.mapNamesToComponents.Add(context.w.Text.ToString(), dest);

            string s = base.VisitDest_declaration(context);

            this.dest = null;

            return base.VisitDest_declaration(context);
        }

        public override string VisitFile([NotNull] bulldogParser.FileContext context)
        {
            for (int i = 0; i < context.ChildCount; ++i)
            {
                var c = context.GetChild(i);
                if (c as bulldogParser.DeclarationContext != null)
                {
                    var v = Visit(c);
                    System.Console.WriteLine(v);
                }
            }
            return null;
       }

        public override string VisitFrom_file([NotNull] bulldogParser.From_fileContext context)
        {
            Console.WriteLine($"   From file: {context.s.ToString()}");
            return base.VisitFrom_file(context);
        }

        public override string VisitRead_from([NotNull] bulldogParser.Read_fromContext context)
        {
            Console.WriteLine($"   Read From: {context.s.Text.ToString()}");
            return base.VisitRead_from(context);
        }

        public override string VisitSource_declaration([NotNull] bulldogParser.Source_declarationContext context)
        {
            Console.WriteLine($"Source_Declaration: {context.w.Text.ToString()}");

            // see if we can find a read from here
            this.source = null;
            for (int i = 0; i < context.ChildCount; i++)
            {
                var c = context.GetChild(i) as bulldogParser.Read_fromContext;
                if (c != null)
                {
                    // already have one?
                    if (this.source != null)
                    {
                        throw new Exception($"can't have multiple READ FROM statements at ~~~");
                    }

                    switch(c.s.Text.ToString().ToUpper())
                    {
                        case "SQLSERVER":
                            this.source = new SQLServerDataSource(context.w.Text.ToString());
                            break;

                        default:
                            break;
                    }
                }
            }

            if (this.source == null)
            {
                throw new Exception($"muts have READ FROM statement at ~~~~");
            }

            // we now have a source object, so populate it
            this.mapNamesToComponents.Add(context.w.Text.ToString(), source);

            string s = base.VisitSource_declaration(context);

            this.source = null;

            return s;
        }

        public string Visit([NotNull] DataSource source, [NotNull] bulldogParser.Star_column_listContext context)
        {
            if (source != null)
                source.IsAllColumns = true;
            Console.WriteLine("   All columns");
            return base.VisitStar_column_list(context);
        }


        public override string VisitStar_column_list([NotNull] bulldogParser.Star_column_listContext context)
        {
            Console.WriteLine("   All columns");
            return base.VisitStar_column_list(context);
        }

        public override string VisitUsing_connect([NotNull] bulldogParser.Using_connectContext context)
        {
            if (source != null)
                source.ConnectionString = context.quotedString().GetText();
            Console.WriteLine($"   Using Connect: {context.quotedString().GetText()}");
            return base.VisitUsing_connect(context);
        }

        public override string VisitWith_input_from([NotNull] bulldogParser.With_input_fromContext context)
        {
            Console.WriteLine($"   input from: {context.s.Text.ToString()}");
            return base.VisitWith_input_from(context);
        }

        public override string VisitWrite_to([NotNull] bulldogParser.Write_toContext context)
        {
            Console.WriteLine($"   Write To: {context.s.Text.ToString()}");
            return base.VisitWrite_to(context);
        }
    }
}

