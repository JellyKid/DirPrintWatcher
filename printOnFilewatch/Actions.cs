using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DirPrintWatcher
{

    public enum PossibleActions { Default, MultiTranStart, MultiTranEnd, Print, PrintAndCut, Cut, Ignore, ValidatorPrint };

    class Actions
    {
        private readonly string LookupTableFile = System.AppDomain.CurrentDomain.BaseDirectory + "ActionTable.json";
        private static Dictionary<string, PossibleActions> lookup_table;

        public Actions()
        {
            InitLookupTable();
        }

        public PossibleActions MatchAction(string file)
        {
            PossibleActions match = PossibleActions.Default;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(file);

                var line = sr.ReadLine();

                //validator ESC POS command is always going to be on first line
                if (line.Contains(DirPrintWatcher.Settings.ValidatorString))
                    return PossibleActions.ValidatorPrint;

                while (!sr.EndOfStream && match == PossibleActions.Default)
                {
                    lookup_table.TryGetValue(line, out match);
                    line = sr.ReadLine();
                }
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
            
            return match;
        }

        private void InitLookupTable()
        {
            if (File.Exists(LookupTableFile))
            {
                StreamReader sr = null;
                try
                {
                    sr = new StreamReader(LookupTableFile);
                    string SerializedObject = sr.ReadToEnd();
                    lookup_table = JsonConvert.DeserializeObject<Dictionary<string, PossibleActions>>(SerializedObject);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Action lookup table init error: {0}\n{1}", e.Message,e.StackTrace);
                }
                finally
                {
                    if (sr != null)
                        sr.Close();
                }
            } else
            {
                lookup_table = new Dictionary<string, PossibleActions>();
            }
        }      

    }
}
