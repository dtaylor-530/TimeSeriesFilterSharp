using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp.Utility
{
    using Microsoft.VisualBasic.FileIO;
    using System.IO;

    public class CSVParser
    {

        public static List<Tuple<DateTime, Decimal>> Parse()
        {
            string fileName = "AAPL200609.csv";
            var path = Path.Combine(GetParentPathName(),"Resources",fileName);

            List<Tuple<DateTime, Decimal>> tuplelist = new List<Tuple<DateTime, decimal>>();

            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                //csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = false;

                // Skip the row with the column names
                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] fields = csvParser.ReadFields();
                    DateTime date =DateTime.Parse( fields[0]);
                    decimal open= decimal.Parse(fields[1]);
                    var t= Tuple.Create(date, open);
                    tuplelist.Add(t);
                }
            }

            return tuplelist;

        }


        public static string GetParentPathName()
        {

            return Directory.GetParent(
                     Directory.GetCurrentDirectory()).Parent.FullName;
        }

    }
}