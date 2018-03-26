using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Regionalism
{
    class Program
    {
        static void Main(string[] args)
        {
            //provincetr  
            //citytr  
            //countytr  
            //towntr  
            //villagetr  
            try
            {
                string url = "index.html";
                string pattern = @"<tr class='provincetr'>.*?</tr>";
                string patternTag = @"<a .*?>.*?</a>";

                List<string> provinceList = GetTagList(url, pattern, patternTag);

                foreach (var province in provinceList)
                {
                    Write(province);
                    string[] provinceTagArr = province.Split(' ');
                    string cityPattern = @"<tr class='citytr'>.*?</tr>";
                    List<string> cityList = GetTagList(provinceTagArr[0], cityPattern, patternTag);
                    foreach (var city in cityList)
                    {
                        Write("----------" + city);
                        string[] cityTagArr = city.Split(' ');
                        string countryPattern = @"<tr class='countytr'>.*?</tr>";
                        List<string> countryList = GetTagList(cityTagArr[0], countryPattern, patternTag);
                        foreach (var country in countryList)
                        {
                            Write("----------" + "----------" + country);
                            string[] countryTagArr = country.Split(' ');
                            string townPattern = @"<tr class='towntr'>.*?</tr>";
                            string cityPath = cityTagArr[0].Split('/')[0] + "/";
                            List<string> townList = GetTagList(cityPath + countryTagArr[0], townPattern, patternTag);
                            foreach (var town in townList)
                            {
                                Write("----------" + "----------" + "----------" + town);
                                string[] townTagArr = town.Split(' ');
                                if (town.Contains("西长安街街道办事处"))
                                {
                                     
                                }
                                string villagePattern = @"<tr class='villagetr'>.*?</tr>";
                                string countryPath = countryTagArr[0].Split('.')[0].Substring(countryTagArr[0].Split('.')[0].Length - 2) + "/";
                                List<string> villageList = GetTagList(cityPath + countryPath + townTagArr[0], villagePattern, patternTag, true);
                                foreach (var village in villageList)
                                {
                                    Write("----------" + "----------" + "----------" + "----------" + village);
                                    string[] v = village.Split(' ');
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
            }

            //GetResult(url, pattern);
            Console.WriteLine("跑完了");
            Console.ReadKey();
        }

        private static void Write(string content)
        {
            Console.WriteLine(content);
            File.AppendAllText("e://a.txt", content + "\r\n");
        }

        private static List<string> Join(List<string> list)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < list.Count; i++)
            {
                List<string> aTagArr = list[i].Split(new string[] { "<", ">", "'" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (dict.ContainsKey(aTagArr[1]))
                {
                    dict[aTagArr[1]] += " " + aTagArr[2];
                }
                else
                {
                    dict.Add(aTagArr[1], aTagArr[1] + " " + aTagArr[2]);
                }
            }
            List<string> resList = new List<string>();
            foreach (var item in dict)
            {
                resList.Add(dict[item.Key]);
            }
            return resList;
        }

        private static List<string> GetTagList(string url, string pattern, string patternTag, bool isEnd = false)
        {
            MatchCollection MatchCollection = GetMatchCollection(url, pattern);
            List<Match> MatchList = GetMatchList(MatchCollection);
            List<string> tagList = new List<string>();
            foreach (Match match in MatchList)
            {
                if (isEnd)
                    patternTag = @"<td.*?/td>";
                var list = GetTag(match, patternTag);
                tagList.AddRange(list);
            }
            if (isEnd)
            {
                return tagList;
            }
            else
            {
                List<string> joinList = Join(tagList);
                return joinList;
            }
        }

        private static List<string> GetTag(Match match, string pattern)
        {
            List<string> tagist = new List<string>();
            foreach (Match match1 in Regex.Matches(match.Value, pattern))
            {
                string aTag = match1.Value.Replace("<br/>", "");
                tagist.Add(aTag);
            }
            return tagist;
        }

        private static List<Match> GetMatchList(MatchCollection matchCollection)
        {
            List<Match> list = new List<Match>();
            foreach (Match match in matchCollection)
            {
                list.Add(match);
            }
            return list;
        }
          
        private static MatchCollection GetMatchCollection(string absolutePath, string pattern)
        {
            CommonUtility.HttpClient client = new CommonUtility.HttpClient();
            client.Url = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/" + absolutePath;
            string result = "";
            try
            {
                result = client.GetString();
                Thread.Sleep(100);
            }
            catch (Exception e)
            {

            }
            return Regex.Matches(result, pattern);
        }
    }
}
