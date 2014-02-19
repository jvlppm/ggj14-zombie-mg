using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.RegularExpressions;
using Conversive.PHPSerializationLibrary;
using System.Collections;
using System.Globalization;

namespace PowerOfLove.Helpers
{
    class PowerOfLoveServer
    {
        public class UserInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int HighScore { get; set; }
            public int TotalZombies { get; set; }
        }

        static async Task<object> Call(string method, object parameters)
        {
            string url = "https://www.diogomuller.com.br/files/games/zombie-social/database/database_access.php?call=" + Uri.EscapeDataString(method);

            foreach (var param in parameters.GetType().GetProperties())
                url += string.Format("&{0}={1}", Uri.EscapeDataString(param.Name), Uri.EscapeDataString(param.GetValue(parameters).ToString()));

            var htClient = new HttpClient();
            var resp = await htClient.GetStringAsync(url);
            resp = Regex.Replace(resp, "<script.*?>.*?</script>", "", RegexOptions.IgnoreCase);
            PHPSerializer serializer = new PHPSerializer();
            return serializer.Deserialize(resp);
        }

        public static async Task<UserInfo> GetUserInfoAsync(string userId)
        {
            var userData = (Hashtable)await Call("getUser", new { id = userId });

            return new UserInfo
            {
                Id = (string)userData["id"],
                HighScore = (int)int.Parse(userData["highscore"].ToString(), CultureInfo.InvariantCulture),
                TotalZombies = (int)int.Parse(userData["totalzombies"].ToString(), CultureInfo.InvariantCulture)
            };
        }

        public static async Task SetUserInfoAsync(string userId, int highScore, int totalZombies, int mapId)
        {
            await Call("saveUser", new
            {
                id = userId,
                highscore = highScore,
                totalzombies = totalZombies,
                mapid = mapId
            });
        }

        public static async Task<UserInfo[]> LoadRankingsAsync(string userId, string rankType, int count, string accessToken)
        {
            var rankData = (ArrayList)await Call("loadRankings", new
            {
                id = userId,
                type = rankType,
                count = count,
                access_token = accessToken
            });

            return (from Hashtable userData in rankData
                    select new UserInfo
                    {
                        Id = (string)userData["id"],
                        Name = (string)userData["name"],
                        HighScore = (int)userData["highscore"],
                        TotalZombies = (int)userData["totalzombies"]
                    }).ToArray();
        }
    }
}