using System;
using System.Xml;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            // var sMsg = "<xml><ToUserName><![CDATA[wwa9fab5c9fd8b9fec]]></ToUserName><Encrypt><![CDATA[6IDR6YMPQs39WYguaclsGbNvLNFphYHHxzg31HtCNAAA+bg+1JQ6T3BLuUeWC3Rr7bAE7AY0P1hTGNJbNLi/JQn/aY4+0w67yTf0tCCkJO0U13+YcWOqpAZLV13d32vf53vkL3eR4yxqu8q6lygxkQTwXh9w/Xgwy5jK4wX9Vc1ngO+d6Dl6Z6UmoXv7KUjCQEyLcOCCCq5miQ/BKxAqO+jjX6CU5GLJGJna0F11lI4WJ/i0FfUaHRE2YjjrzBs7pC8E7AKPsqmOWTq9MJdy0DOAVDAtHCdr0l8vhJWDExUEBclUCtMU1Xw6YwdOYNr2y/Vfr4HGbR7/Xp0JY7mhK878VetclgCs3sGNW5mHsQXj3HT/Xt2QH3PgSTFlbHJw]]></Encrypt><AgentID><![CDATA[]]></AgentID></xml>";
            var sMsg = "<xml><SuiteId><![CDATA[wwa9fab5c9fd8b9fec]]></SuiteId><InfoType><![CDATA[suite_ticket]]></InfoType><TimeStamp>1610613825</TimeStamp><SuiteTicket><![CDATA[d0ASIeOFSzvETxkrW_wVpdwhJe7_SKqwH7TtuifCeNN0CpUiOo_Ki_CszKb65Nfv]]></SuiteTicket></xml>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sMsg);
            var root = doc.FirstChild;
            var suiteTicket = root["SuiteTicket"].InnerText.Replace("<![CDATA[", "").Replace("]]", "");
        }
    }
}
