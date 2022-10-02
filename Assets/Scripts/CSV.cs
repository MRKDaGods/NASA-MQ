using System.Collections.Generic;
using UnityEngine;

namespace MRK
{
    public class CSV
    {
        private readonly string _file;
        private readonly char[] _typePattern;
        private string[] _headers;

        public List<Dictionary<string, object>> Records
        {
            get; private set;
        }

        public CSV(string file, char[] typePattern)
        {
            _file = file;
            _typePattern = typePattern;
        }

        public void Parse()
        {
            Records ??= new List<Dictionary<string, object>>();

            Records.Clear();

            string[] content = _file.Split('\n');
            Debug.Log("content sz=" + content.Length);

            // first line = headers
            _headers = content[0].Trim().Split(',');

            //Debug.Log(_headers.Length);

            for (int i = 1; i < content.Length; i++)
            {
                var record = new Dictionary<string, object>();
                string[] vals = content[i].Split(',');
                for (int j = 0; j < vals.Length; j++)
                {
                    Debug.Log(vals.Length);
                    var v = vals[j];
                    record[_headers[j]] = _typePattern[j] == 'n' ? float.Parse(v.Length == 0 ? "0" : v) : v;
                }

                Records.Add(record);
            }

            Debug.Log("r=" + Records.Count);
        }
    }
}
