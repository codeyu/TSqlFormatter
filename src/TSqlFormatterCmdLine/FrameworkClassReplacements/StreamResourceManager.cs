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
 public abstract class StreamResourceManager : ResourceManager
        {

            private Dictionary<string, ReadOnlyDictionary<string, string>> _resourceCollection;

            protected StreamResourceManager()
                : base(typeof(StreamResourceManager))
            {
                _resourceCollection = new Dictionary<string, ReadOnlyDictionary<string, string>>();
            }
            protected abstract Stream GetResourceStream(CultureInfo culture);
            protected virtual void Add(CultureInfo culture)
            {
                ReadOnlyDictionary<string, string> aggregatedCollection = null;
                lock (_resourceCollection)
                {
                    if (!TryGetResourceSet(culture, out aggregatedCollection))
                    {
                        Stream resourceStream = GetResourceStream(culture);
                        if (resourceStream != null)
                        {
                            using (var reader = new ResourceReader(resourceStream))
                            {
                                Dictionary<string, string> aggregator = new Dictionary<string, string>();
                                foreach (DictionaryEntry entry in reader)
                                {
                                    aggregator.Add((string)entry.Key, (string)entry.Value);
                                }
                                aggregatedCollection = new ReadOnlyDictionary<string, string>(aggregator);
                            }
                        }

                        _resourceCollection.Add(culture.Name, aggregatedCollection);
                    }
                }
            }
            public virtual IReadOnlyDictionary<string, string> GetAllStrings(CultureInfo culture)
            {
                Dictionary<string, string> aggregator = new Dictionary<string, string>();
                foreach (var dictionaryentry in walkCultureParentChain(culture))
                {
                    if (dictionaryentry != null)
                    {
                        foreach (var entry in dictionaryentry)
                        {
                            if (!aggregator.ContainsKey(entry.Key))
                            {
                                aggregator.Add(entry.Key, entry.Value);
                            }
                        }
                    }
                }
                return new ReadOnlyDictionary<string, string>(aggregator);
            }
            private IEnumerable<ReadOnlyDictionary<string, string>> walkCultureParentChain(CultureInfo culture)
            {
                ReadOnlyDictionary<string, string> currentresourceset = null;
                CultureInfo cultureInfo = culture;
                CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                if (cultureInfo == null)
                {
                    cultureInfo = CultureInfo.CurrentUICulture;
                }
                bool stopwalkingCultureParentChain;
                do
                {
                    stopwalkingCultureParentChain = (cultureInfo.Name == invariantCulture.Name);
                    if (!TryGetResourceSet(cultureInfo, out currentresourceset))
                    {
                        Add(cultureInfo);
                        TryGetResourceSet(cultureInfo, out currentresourceset);
                    }
                    yield return currentresourceset;
                    cultureInfo = cultureInfo.Parent;
                }
                while (!stopwalkingCultureParentChain);
                yield break;
            }
            protected virtual bool TryGetResourceSet(CultureInfo culture, out ReadOnlyDictionary<string, string> collection)
            {

                lock (_resourceCollection)
                {
                    return _resourceCollection.TryGetValue(culture.Name, out collection);
                }

            }
            // Looks up a resource value for a particular name.  Looks in the 
            // specified CultureInfo, and if not found, all parent CultureInfos.
            // Returns null if the resource wasn't found.
            // 
            public override String GetString(String name, CultureInfo culture)
            {
                if (null == name)
                {
                    throw new ArgumentNullException("name");
                }


                foreach (var CollectionLookedAt in walkCultureParentChain(culture))
                {
                    if (CollectionLookedAt != null && CollectionLookedAt.ContainsKey(name))
                    {
                        return CollectionLookedAt[name];

                    }
                }

                return null;

            }


        }
       

       
}