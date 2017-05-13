using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Java.Security;
using Xamarin.Facebook;
using Xamarin.Facebook.Login.Widget;
using Java.Lang;
using Xamarin.Facebook.Login;
using System.Collections.Generic;
using Xamarin.Facebook.Share.Model;
using Facebook;

namespace Lippz.Droid
{
    [Activity(Label = "Lippz.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IFacebookCallback
    {
        private ICallbackManager mCallBackManager;
        const string FacebookAppId = "1390101657726597";
        string userToken = "";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Window.RequestFeature(WindowFeatures.NoTitle);

#pragma warning disable CS0618 // Type or member is obsolete
            FacebookSdk.SdkInitialize(this.ApplicationContext);
#pragma warning restore CS0618 // Type or member is obsolete


            SetContentView(Resource.Layout.Main);

            //LoginButton button = FindViewById<LoginButton>(Resource.Id.login_button);

            //button.SetReadPermissions("user_friends");
            mCallBackManager = CallbackManagerFactory.Create();

            //button.RegisterCallback(mCallBackManager, this);

            LoginManager.Instance.RegisterCallback(mCallBackManager, this);

            FindViewById<Button>(Resource.Id.button).Click += (o, e) =>
            {
                LoginManager.Instance.LogInWithReadPermissions(this, new List<string> { "public_profile", "user_friends" });
            };





        }

        public void OnCancel() // wanneer je het wegklikt
        {
            //throw new NotImplementedException();
        }

        public void OnError(FacebookException error) // bij bijvoorbeeld geen internet error komt 
        {
            //throw new NotImplementedException();
        }

        public void OnSuccess(Java.Lang.Object result) // wanneer je ingelogd bent
        {
            LoginResult loginResult = result as LoginResult;
            userToken = loginResult.AccessToken.UserId;
            Console.WriteLine(loginResult.AccessToken.UserId);
            PrintFriendsNames();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            mCallBackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        void PrintFriendsNames()
        {
            // This uses Facebook Query Language
            // See https://developers.facebook.com/docs/technical-guides/fql/ for more information.
            var query = string.Format("SELECT uid,name,pic_square FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1={0}) ORDER BY name ASC", "me()");
            FacebookClient fb = new FacebookClient(userToken);

            fb.GetTaskAsync("fql", new { q = query }).ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var result = (IDictionary<string, object>)t.Result;
                    var data = (IList<object>)result["data"];
                    var count = data.Count;
                    var message = string.Format("You have {0} friends", count);
                    Console.WriteLine(message);

                    foreach (IDictionary<string, object> friend in data)
                        Console.WriteLine((string)friend["name"]);
                }
            });

        }
    }
}


