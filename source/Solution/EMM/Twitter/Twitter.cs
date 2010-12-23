using Twitterizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM
{
    public class Twitter
    {
        private const string mCallback = "oob";
        private string mRequestToken = "";
        private OAuthTokenResponse mAccessToken;
        private string mPin = "";
        private OAuthTokens mTokens;


        public Twitter()
        {
            mTokens = new OAuthTokens();
            mAccessToken = new OAuthTokenResponse();
            mTokens.ConsumerKey = TwitterKeys.CONSUMER_KEY;
            mTokens.ConsumerSecret = TwitterKeys.CONSUMER_SECRET;
        }

        public string AccessToken
        {
            get { return mAccessToken.Token; }
            set {
                mAccessToken.Token = value;
                mTokens.AccessToken = value;
            }
        }

        public string AccessTokenSecret
        {
            get { return mAccessToken.TokenSecret; }
            set
            {
                mAccessToken.TokenSecret = value;
                mTokens.AccessTokenSecret = value;
            }
        }

        public Uri GetAuthUri()
        {
            OAuthTokenResponse TR = Twitterizer.OAuthUtility.GetRequestToken(TwitterKeys.CONSUMER_KEY, TwitterKeys.CONSUMER_SECRET, mCallback);
            mRequestToken = TR.Token;
            return Twitterizer.OAuthUtility.BuildAuthorizationUri(mRequestToken);
        }

        public void GetAccessToken(string Pin)
        {
            mPin = Pin;
            mAccessToken = Twitterizer.OAuthUtility.GetAccessToken(TwitterKeys.CONSUMER_KEY, TwitterKeys.CONSUMER_SECRET, mRequestToken, mPin);
            mTokens.AccessToken = mAccessToken.Token;
            mTokens.AccessTokenSecret = mAccessToken.TokenSecret;
        }

        public void Tweet(string Message)
        {
            TwitterResponse<TwitterStatus> response = TwitterStatus.Update(mTokens, Message);
        }

    }
}
