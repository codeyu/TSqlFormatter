using System;
using Xunit;
using System.Collections.Generic;
namespace TSqlFormatter.Tests
{
    public class TSqlFormatterCmdLineTest
    {
        [Fact]
        public void Test1()
        {

        }
         [Fact]
        public  void TestDefaultCulture()
        {
            CultureInfo jap = new CultureInfo("ja-jp");
            var s_dict = new Dictionary<string, string>(); //TODO: need init.
            ConcreteStreamResourceManager manager = new ConcreteStreamResourceManager();
            foreach (KeyValuePair<string, string> entry in s_dict)
            {
                string found = manager.GetString(entry.Key);
                Assert.True(string.Compare(entry.Value, found) == 0, "expected: " + entry.Value + ", but got : " + found);
            }
        }
    }
}
