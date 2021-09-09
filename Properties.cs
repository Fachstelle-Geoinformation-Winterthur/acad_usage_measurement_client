// <copyright company="Vermessungsamt Winterthur">
// Author: Edgar Butwilowski
// Copyright (c) 2021 Vermessungsamt Winterthur. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.IO;

namespace win.acad_usage_measurement
{
    public class Properties
    {
        private Dictionary<string, string> properties = new Dictionary<string, string>();
        public Properties(string pathToFile)
        {
            foreach (string line in File.ReadAllLines(pathToFile))
            {
                string[] linesArray = line.Split('=');
                if (linesArray.Length >= 2)
                {
                    string key = linesArray[0];
                    string[] valuesArray = new string[linesArray.Length - 1];
                    for (int i = 1; i < linesArray.Length; i++)
                    {
                        valuesArray[i - 1] = linesArray[i];
                    }
                    string value = string.Join("=", valuesArray);
                    properties.Add(key, value);
                }
            }
        }

        public string getValue(string key)
        {
            string result = "";
            this.properties.TryGetValue(key, out result);
            return result;
        }
    }
}
