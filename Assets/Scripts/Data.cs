using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MRK
{
    public struct SMData
    {
        public int Year, Day, H, M, S;
        public float Lat, Long, Magnitude;
        public string Comments;
    }

    public struct DMData
    {
        public int A;
        public string Side;
        public float Lat, Lat_err, Long, Long_err, Depth, Depth_err;
        public string Assumed;
    }

    public class Data : MonoBehaviour
    {
        private CSV _sm;
        private CSV _dm;

        public bool Ready { get; private set; } = false;

        public static Data Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Debug.Log("preparing data");

            _sm = new CSV(Resources.Load<TextAsset>("sm").text, "nnnnnnnns".ToCharArray());
            _sm.Parse();

            _dm = new CSV(Resources.Load<TextAsset>("dm").text, "nsnnnnnns".ToCharArray());
            _dm.Parse();

            Ready = true;
        }

        public List<SMData> GetShallowMoonQuakesData(out float maxMag)
        {
            var list = new List<SMData>();

            maxMag = float.NegativeInfinity;

            foreach (var record in _sm.Records)
            {
                float mag = (float)record["Magnitude"];
                if (mag > maxMag)
                {
                    maxMag = mag;
                }


                list.Add(new SMData
                {
                    Year = (int)(float)record["Year"],
                    Day = (int)(float)record["Day"],
                    H = (int)(float)record["H"],
                    M = (int)(float)record["M"],
                    S = (int)(float)record["S"],
                    Lat = (float)record["Lat"],
                    Long = (float)record["Long"],
                    Magnitude = mag,
                    Comments = (string)record["Comments"]
                });
            }

            return list;
        }

        public List<DMData> GetDeepMoonQuakesData(out float maxDepth)
        {
            var list = new List<DMData>();

            maxDepth = float.NegativeInfinity;

            foreach (var record in _dm.Records)
            {
                float depth = (float)record["Depth"];
                if (depth > maxDepth)
                {
                    maxDepth = depth;
                }

                list.Add(new DMData
                {
                    A = (int)(float)record["A"],
                    Side = (string)record["Side"],
                    Lat = (float)record["Lat"],
                    Lat_err = (float)record["Lat_err"],
                    Long = (float)record["Long"],
                    Long_err = (float)record["Long_err"],
                    Depth = depth,
                    Depth_err = (float)record["Depth_err"],
                    Assumed = (string)record["Assumed"]
                });
            }

            return list;
        }
    }
}
