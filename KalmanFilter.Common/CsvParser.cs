using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.Utility
{
    using Microsoft.VisualBasic.FileIO;
    using System.IO;

    public class CSVParser
    {

        public static List<Tuple<DateTime, Double>> Parse()
        {
            string fileName = "AAPL200609.csv";
            var path = Path.Combine(PathHelper.GetParentPathName(),"Resources",fileName);

            List<Tuple<DateTime,Double>> tuplelist = new List<Tuple<DateTime, Double>>();

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
                    double open= double.Parse(fields[1]);
                    var t= Tuple.Create(date, open);
                    tuplelist.Add(t);
                }
            }

            return tuplelist;

        }


  

    }
}