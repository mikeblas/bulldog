namespace Bulldog
{
    using System;
    using System.Collections.Generic;
    using Antlr4.Runtime.Misc;

    class BulldogVisitor : bulldogBaseVisitor<string>
    {
        /// <summary>
        /// our symbol table; maps a name to a specific DataComponent instance
        /// </summary>
        Dictionary<string, DataComponent> namesToComponents = new Dictionary<string, DataComponent>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// resolved list of input connnections; key is an object, value is the object from which the key takes input
        /// </summary>
        Dictionary<DataComponent, DataComponent> inputsToOutputs = new Dictionary<DataComponent, DataComponent>();

        /// <summary>
        /// unresolved list of input connections; key is the name of an object, value is the name
        /// of the object from which the key object takes input
        /// </summary>
        Dictionary<string, string> inputsToOutputsByName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        DataSource source = null;
        DataDestination dest = null;

        private static string NormalizeQuotedString(bulldogParser.QuotedStringContext c)
        {
            // string beginning and ending quotes
            string s = c.GetText();
            if (s[0] == '"')
                s = s.Substring(1);
            if (s[s.Length - 1] == '"')
                s = s.Substring(0, s.Length - 1);

            // replace escaped quotes; replace \" with "
            s = s.Replace("\\\"", "\"");

            return s;
        }

        public BulldogVisitor()
        {
        }

        public Dictionary<string, DataComponent> GetObjects()
        {
            return namesToComponents;
        }

        public Dictionary<DataComponent, DataComponent> GetConnections()
        {
            return inputsToOutputs;
        }

        public List<DataComponent> GetTopComponents()
        {
            // all components from the existing list
            Dictionary<string, DataComponent> allComponents = new Dictionary<string, DataComponent>(this.namesToComponents, StringComparer.OrdinalIgnoreCase);

            // get each of the components; if it has no input, add it to the list
            foreach (DataComponent c in namesToComponents.Values)
            {
                // if this component has an input, remove it from our list
                if (c.InputComponent == null)
                {
                    allComponents.Remove(c.Name);
                }
            }

            // make a list of the remaining ones
            List<DataComponent> list = new List<DataComponent>(allComponents.Values);
            return list;
        }

        private void BuildConnections()
        {
            foreach(KeyValuePair<string, string> entry in inputsToOutputsByName)
            {
                if (!namesToComponents.ContainsKey(entry.Key))
                {
                    // this might be hard to do ...
                    Console.Out.WriteLine($"component {entry.Key} does not exist");
                }
                else if (!namesToComponents.ContainsKey(entry.Value))
                {
                    // this is just the case of declaring WITH INPUT FROM <name>,
                    // when no component <name> exists
                    Console.Out.WriteLine($"component {entry.Key} takes input from {entry.Value}, but {entry.Value} does ont exist");
                }
                else
                {
                    DataComponent d1 = namesToComponents[entry.Key];
                    DataComponent d2 = namesToComponents[entry.Value];
                    d1.InputComponent = d2;
                }
            }
        }


        public override string VisitAggregate_declaration([NotNull] bulldogParser.Aggregate_declarationContext context)
        {
            return base.VisitAggregate_declaration(context);
        }

        public override string VisitFrom_table([NotNull] bulldogParser.From_tableContext context)
        {
            if (source as SQLServerDataSource != null)
            {
                SQLServerDataSource s = source as SQLServerDataSource;
                s.TableName = context.s.Text.ToString();
                Console.WriteLine($"   source from table {s.TableName}");
            }
            return base.VisitFrom_table(context);
        }

        public override string VisitInto_table([NotNull] bulldogParser.Into_tableContext context)
        {
            if (dest as SQLServerDataDestination != null)
            {
                SQLServerDataDestination d = dest as SQLServerDataDestination;
                d.TableName = context.s.Text.ToString();
                Console.WriteLine($"   dest into table {d.TableName}");
            }
            return base.VisitInto_table(context);
        }


        public override string VisitColumns([NotNull] bulldogParser.ColumnsContext context)
        {
            return base.VisitColumns(context);
        }

        public override string VisitColumn_list([NotNull] bulldogParser.Column_listContext context)
        {
            foreach (var x in context.column())
            {
                Console.WriteLine($"   column list entry: {x.GetText()}");
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
            this.namesToComponents.Add(context.w.Text.ToString(), dest);

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

            BuildConnections();
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
                throw new Exception($"must have READ FROM statement at ~~~~");
            }

            // we now have a source object, so populate it
            this.namesToComponents.Add(context.w.Text.ToString(), source);

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
            {
                source.ConnectionString = NormalizeQuotedString(context.quotedString());
                Console.WriteLine($"   Using Connect: {source.ConnectionString}");
            }
            else if (dest != null)
            {
                dest.ConnectionString = NormalizeQuotedString(context.quotedString());
                Console.WriteLine($"   Using Connect: {dest.ConnectionString}");
            }
            return base.VisitUsing_connect(context);
        }

        public override string VisitWith_input_from([NotNull] bulldogParser.With_input_fromContext context)
        {
            Console.WriteLine($"   input from: {context.s.Text.ToString()}");
            if (this.source != null)
            {
                inputsToOutputsByName.Add(this.source.Name, context.s.Text.ToString());
            }
            else if (this.dest != null)
            {
                inputsToOutputsByName.Add(this.dest.Name, context.s.Text.ToString());
            }
            return base.VisitWith_input_from(context);
        }

        public override string VisitWrite_to([NotNull] bulldogParser.Write_toContext context)
        {
            Console.WriteLine($"   Write To: {context.s.Text.ToString()}");
            return base.VisitWrite_to(context);
        }
    }
}

