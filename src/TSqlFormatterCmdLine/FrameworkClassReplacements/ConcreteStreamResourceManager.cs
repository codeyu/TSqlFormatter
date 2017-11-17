using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Resources;
namespace FrameworkClassReplacements
{
    public class ConcreteStreamResourceManager : StreamResourceManager
        {
            public static ResourceWriter GenerateResourceStream(Dictionary<string, string> inp_dict, MemoryStream ms)
            {
                ResourceWriter rw = new ResourceWriter(ms);
                foreach (KeyValuePair<string, string> e in inp_dict)
                {
                    string name = e.Key;
                    string values = e.Value;
                    rw.AddResource(name, values);
                }
                rw.Generate();
                ms.Seek(0L, SeekOrigin.Begin);
                return rw;
            }
            protected override Stream GetResourceStream(CultureInfo culture)
            {
                MemoryStream ms = null;
                if (culture.Name == CultureInfo.InvariantCulture.Name)
                {
                    ms = new MemoryStream();
                    ConcreteStreamResourceManager.GenerateResourceStream(null, ms);//TODO:It's not null.
                }
                return ms;
            }
        }
} 
 
 