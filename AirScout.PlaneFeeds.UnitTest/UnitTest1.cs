using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirScout.PlaneFeeds.Plugin.FlexJSON;

using System.Web.Script.Serialization;
using AirScout.PlaneFeeds.Plugin;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;

namespace AirScout.PlaneFeeds.UnitTest
{
    [TestClass]
    public class PlaneFeedsUnitTest
    {
        [TestMethod]
        public void TestFlexJSONGetAllPlaneInfos()
        {
            Assert.AreEqual(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, ",");  // not nice, but the hard coded strings below are using ","
            FlexJSONPlugin flexJSONPlugin = new FlexJSONPlugin();


            PlaneFeedPluginArgs args = new PlaneFeedPluginArgs();
            args.TmpDirectory = ""; // need to be set - GetAllPlaneInfos writes to this directory
            flexJSONPlugin.Start(args);

            string json = @"[{""hex"": ""40688F"", ""call"": null, ""lat"": 50.07652, ""lon"": 8.69768, ""alt"": 39000, ""track"": 106, ""speed"": 536, ""type"": null, ""category"": null, ""manufacturer"": null, ""model"": null, ""time"": 1672428147.068393}, 
                             {""hex"": ""407536"", ""call"": ""BAW705  "", ""lat"": 50.11353, ""lon"": 8.47551, ""alt"": 33975, ""track"": 293, ""speed"": 388, ""type"": null, ""category"": null, ""manufacturer"": null, ""model"": null, ""time"": 1672428161.365445}]";

            JavaScriptSerializer js = new JavaScriptSerializer();
            dynamic root = js.Deserialize<dynamic>(json);
            // get all planeinfos in an array - needs to be accessed through PrivateObject, as it is private
            PrivateObject obj = new PrivateObject(flexJSONPlugin);
            var retval = obj.Invoke("GetAllPlaneInfos", new Object[] { root });
            // now we have the result in private field flexJSONPlugin.PlaneList, so use reflection
            var pl = typeof(FlexJSONPlugin).GetField("PlaneList", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(flexJSONPlugin);
            List<string[]> planelist = (List<string[]>)pl;
            List<string[]> expectedVal = new List<string[]>();
            // need to copy the first value (=key), as it changes
            expectedVal.Add(new string[] { planelist[0][0], "40688F", "", "50,07652", "8,69768", "39000", "106", "536", "", "", "", "", "1672428147,068393" });
            expectedVal.Add(new string[] { planelist[1][0], "407536", "BAW705  ", "50,11353", "8,47551", "33975", "293", "388", "", "", "", "", "1672428161,365445" });

            CollectionAssert.AreEqual(expectedVal[0], planelist[0]);
            CollectionAssert.AreEqual(expectedVal[1], planelist[1]);

            Assert.AreEqual("40688F", flexJSONPlugin.To_Hex(planelist[0][1]));
            Assert.AreEqual("BAW705", flexJSONPlugin.To_Call(planelist[1][2])); // no trailing space!
            Assert.AreEqual(null, flexJSONPlugin.To_Call(planelist[0][2]));
            Assert.AreEqual(50.07652, flexJSONPlugin.To_Lat(planelist[0][3]));
            Assert.AreEqual(8.69768, flexJSONPlugin.To_Lon(planelist[0][4]));
            Assert.AreEqual(39000, flexJSONPlugin.To_Alt(planelist[0][5]));
            Assert.AreEqual(106, flexJSONPlugin.To_Track(planelist[0][6]));
            Assert.AreEqual(536, flexJSONPlugin.To_Speed(planelist[0][7]));

            // alternate feed
            json = @"{""full_count"":14780,""version"":4,""2ec8d821"":[""3CD803"",45.92,10.05,40,45025,475,"""",""F - LIPX4"",""LJ31"",""D - CJCL"",1673192326,""LEI"",""SZG"",""JCL104"",0,64,""UNI104"",0,""JCL""],
                ""2ec8ddac"":[""490D07"",41.56,-4.29,184,45000,461,"""",""F-LESO4"",""CL35"",""CS-CHG"",1673192325,""BQH"",""AGP"","""",0,-64,""NJE132D"",0,""NJE""]
                }";
            root = js.Deserialize<dynamic>(json);
            // get all planeinfos in an array - needs to be accessed through PrivateObject, as it is private
            obj = new PrivateObject(flexJSONPlugin);
            retval = obj.Invoke("GetAllPlaneInfos", new Object[] { root });
            pl = typeof(FlexJSONPlugin).GetField("PlaneList", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(flexJSONPlugin);
            planelist = (List<string[]>)pl;
            expectedVal.Clear();
            expectedVal.Add(new string[] { "2ec8d821", "3CD803", "45,92", "10,05", "40", "45025", "475", "", "F - LIPX4", "LJ31", "D - CJCL", "1673192326", "LEI", "SZG", "JCL104", "0", "64", "UNI104", "0", "JCL" });
            expectedVal.Add(new string[] { "2ec8ddac", "490D07", "41,56", "-4,29", "184", "45000", "461", "", "F-LESO4", "CL35", "CS-CHG", "1673192325", "BQH", "AGP", "", "0", "-64", "NJE132D", "0", "NJE" });


            Assert.AreEqual(new DateTime(2022, 12, 30, 19, 22, 27, 68), flexJSONPlugin.To_UTC("1672428147,068393")); // double
            Assert.AreEqual(new DateTime(2022, 12, 30, 19, 22, 27), flexJSONPlugin.To_UTC("2022-12-30 19:22:27") ); // DateTime
            Assert.AreEqual(new DateTime(2022, 12, 30, 19, 22, 27, 68), flexJSONPlugin.To_UTC("1672428147068")); // int64
            Assert.AreEqual(new DateTime(2022, 12, 30, 19, 22, 27), flexJSONPlugin.To_UTC("1672428147")); // int32

        }
    }
}
