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
using Xamarin.Auth;
using Xamarin.Social.Services;
using System.Net.Http;
using System.Text.RegularExpressions;
using Conversive.PHPSerializationLibrary;
using System.Collections;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace PowerOfLove.Helpers
{
    class Facebook
    {
        static Facebook _instance;
        public static Facebook Instance
        {
            get { return (_instance ?? (_instance = new Facebook())); }
        }

        readonly FacebookService Service = new FacebookService
        {
#if DEV_KEYS
            // Dev-Keys
            ClientId = "405744166227759",
            ClientSecret = "d7a9c8d5cfdaf8e7916991e9754b90a9",
            RedirectUrl = new System.Uri("http://apps.facebook.com/dev-poweroflove/"),
#else
            ClientId = "242699782569520",
            ClientSecret = "c599315593f42e88d32481af36337ae1",
            RedirectUrl = new System.Uri("http://apps.facebook.com/ggjpoweroflove/"),
#endif
            Scope = "email, publish_actions"
        };

        Account _account;
        Task<string[]> _getFriendNames;

        public string UserName { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string UserId { get; private set; }
        public string AccessToken { get; private set; }

        private Facebook() { }

        public async Task<bool> IsLoggedInAsync(Activity context)
        {
            if (_account == null)
                _account = (await Service.GetAccountsAsync(context)).FirstOrDefault();

            return _account != null;
        }

        public async Task<bool> RefreshUserStatus(Activity context)
        {
            if (!await IsLoggedInAsync(context))
                return false;

            var facebookInfo = (JObject)await Service.Ajax(_account, "me");

            UserId = (string)facebookInfo["id"];
            UserName = (string)facebookInfo["name"];
            FirstName = (string)facebookInfo["first_name"];
            LastName = (string)facebookInfo["last_name"];
            AccessToken = _account.Properties["access_token"];
            return true;
        }

        public async Task LogInAsync(Activity context)
        {
            if (!await IsLoggedInAsync(context))
            {
                var auth = new OAuth2Authenticator(
                               clientId: Service.ClientId,
                               scope: Service.Scope,
                               authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                               redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html"));

                var userAccount = await context.LoginAsync(auth);
                Service.SaveAccount(context.BaseContext, userAccount);
            }
        }

        public Task<string[]> LoadFriendNamesAsync()
        {
            if (_getFriendNames == null)
            {
                _getFriendNames = DoLoadFriendNamesAsync();
                _getFriendNames.ContinueWith(t => _getFriendNames = null, TaskContinuationOptions.NotOnRanToCompletion);
            }
            return _getFriendNames;
        }

        async Task<string[]> DoLoadFriendNamesAsync()
        {
            var result = await Service.Ajax(_account, "me/friends");

            return (from JObject friendInfo in (JArray)((JObject)result)["data"]
                                select friendInfo["name"].ToString()).ToArray();
        }
    }
}