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
using Jv.Games.Xna;

namespace PowerOfLove.Helpers
{
    class PowerOfLoveServer
    {
        static PowerOfLoveServer _instance;
        public static PowerOfLoveServer Instance
        {
            get { return _instance ?? (_instance = new PowerOfLoveServer()); }
        }

        public class UserInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int HighScore { get; set; }
            public int TotalZombies { get; set; }
        }

        string _getUserInfoId;
        Task<UserInfo> _getUserInfoAsync;
        MutexAsync _requestMutex;
        readonly IDisposable EmptyDisposable = Disposable.Create(() => { });

        PowerOfLoveServer()
        {
            _requestMutex = new MutexAsync();
        }

        async Task<object> Call(string method, object parameters)
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

        public Task<UserInfo> GetUserInfoAsync(string userId, bool waitRequests)
        {
            if (_getUserInfoAsync == null || _getUserInfoId != userId)
            {
                _getUserInfoId = userId;
                _getUserInfoAsync = DoGetUserInfoAsync(userId, waitRequests);
            }
            return _getUserInfoAsync;
        }

        async Task<UserInfo> DoGetUserInfoAsync(string userId, bool waitRequests)
        {
            using (await WaitRequestAvailability(waitRequests))
            {
                var userData = (Hashtable)await Call("getUser", new { id = userId });

                return new UserInfo
                {
                    Id = (string)userData["id"],
                    HighScore = (int)int.Parse(userData["highscore"].ToString(), CultureInfo.InvariantCulture),
                    TotalZombies = (int)int.Parse(userData["totalzombies"].ToString(), CultureInfo.InvariantCulture)
                };
            }
        }

        public async Task SetUserInfoAsync(string userId, int highScore, int totalZombies, int mapId, bool waitRequests)
        {
            using (await WaitRequestAvailability(waitRequests))
            {
                await Call("saveUser", new
                {
                    id = userId,
                    highscore = highScore,
                    totalzombies = totalZombies,
                    mapid = mapId
                });
            }
        }

        async Task<IDisposable> WaitRequestAvailability(bool waitRequests)
        {
            if (!waitRequests)
                return EmptyDisposable;

            return await _requestMutex.WaitAsync();
        }

        public async Task<UserInfo[]> LoadRankingsAsync(string userId, string rankType, int count, string accessToken, bool waitRequests = true)
        {
            using (await WaitRequestAvailability(waitRequests))
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

        public async Task PostResultToServerAsync(int gameResult)
        {
            using (await _requestMutex.WaitAsync())
            {
                if (_getUserInfoAsync != null && _getUserInfoAsync.IsCompleted)
                {
                    var userInfo = _getUserInfoAsync.Result;
                    var newInfo = new UserInfo
                    {
                        Id = userInfo.Id,
                        Name = userInfo.Name,
                        TotalZombies = userInfo.TotalZombies + gameResult,
                        HighScore = Math.Max(userInfo.HighScore, gameResult)
                    };
                    _getUserInfoAsync = Task.FromResult(newInfo);
                }
                
                const int AndroidMapId = 3;
                try
                {
                    var facebookId = Facebook.Instance.UserId;
                    if (facebookId == null)
                        return;
                    var userInfo = await GetUserInfoAsync(facebookId, false);
                    var highScore = Math.Max(userInfo.HighScore, gameResult);
                    await SetUserInfoAsync(facebookId, highScore, userInfo.TotalZombies + gameResult, AndroidMapId, false);
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}