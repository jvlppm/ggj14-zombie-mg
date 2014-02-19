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
        string _userName;

        public string UserName { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string UserId { get; private set; }
        public string AccessToken { get; private set; }

        private Facebook()
        {
            
        }

        public async Task<bool> IsLoggedInAsync(Activity context)
        {
            if (_account == null)
                _account = (await Service.GetAccountsAsync(context)).FirstOrDefault();

            return _account != null;
        }

        public async Task RefreshUserStatus(Activity context)
        {
            if (!await IsLoggedInAsync(context))
                return;

            var facebookInfo = await Service.Ajax(_account, "me");

            UserId = (string)facebookInfo["id"];
            UserName = (string)facebookInfo["name"];
            FirstName = (string)facebookInfo["first_name"];
            LastName = (string)facebookInfo["last_name"];
            AccessToken = _account.Properties["access_token"];
        }
    }
}