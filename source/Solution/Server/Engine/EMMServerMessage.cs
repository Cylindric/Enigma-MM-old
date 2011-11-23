using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EnigmaMM.Engine
{
    public class EMMServerMessage
    {
        public string Message { private set; get; }
        public MessageTypes Type { private set; get; }
        public Dictionary<string, string> Data { private set; get; }
        private Data.User mUser;
        private static List<MessagePattern> sPatterns;

        public enum MatchTypes
        {
            Regex,
            EndsWith,
        }

        public enum MessageTypes
        {
            // Standard Minecraft types
            ErrorPortBusy,
            MinecraftBanner,
            SaveComplete,
            SaveStarted,
            AutoSaveEnabled,
            AutoSaveDisabled,
            StartupComplete,
            UserCount,
            UserList,
            UserLoggedIn,
            UserLoggedOut,
            UserFloating,
            ServerCommand,
            TriedServerCommand,

            // Anything else is an 'other'
            Other
        }

        private class MessagePattern
        {
            public MessageTypes MessageType;
            public MatchTypes MatchType;
            public string Pattern;

            public MessagePattern(string type, string matchType, string matchPattern)
            {
                if (Enum.IsDefined(typeof(MessageTypes), type))
                {
                    MessageType = (MessageTypes)Enum.Parse(typeof(MessageTypes), type);
                }
                else
                {
                    MessageType = MessageTypes.Other;
                }

                if (Enum.IsDefined(typeof(MatchTypes), matchType))
                {
                    MatchType = (MatchTypes)Enum.Parse(typeof(MatchTypes), matchType);
                }
                else
                {
                    MatchType = MatchTypes.EndsWith;
                }

                Pattern = matchPattern;
            }

            public MessagePattern(MessageTypes type, MatchTypes matchType, string matchPattern)
            {
                MessageType = type;
                MatchType = matchType;
                Pattern = matchPattern;
            }
        }

        public EMMServerMessage(string msg)
        {
            Message = msg;
            DetermineType();
        }

        public Data.User User
        {
            set { mUser = value; }
            get { return mUser; }
        }
        
        public void SetUser(string username)
        {
            mUser = Manager.GetContext.Users.SingleOrDefault(u => u.Username == username);
        }

        /// <summary>
        /// Iterates over all the defined Patterns to determine the type of message and if applicable
        /// extracts any pertinent data.
        /// </summary>
        private void DetermineType()
        {
            if (sPatterns == null)
            {
                PopulateRules();
            }
            Type = MessageTypes.Other;
            foreach (MessagePattern pattern in sPatterns)
            {
                if (ProcessMatch(pattern.MatchType, pattern.Pattern, pattern.MessageType)) { return; }
            }
        }

        private void PopulateRules()
        {
            sPatterns = new List<MessagePattern>();
            using (Data.EMMDataContext db = Manager.GetContext)
            {
                foreach (Data.MessageType message in db.MessageTypes)
                {
                    MessagePattern p = new MessagePattern(message.Name, message.MatchType, message.Expression);
                    sPatterns.Add(p);
                }
            }
        }

        /// <summary>
        /// Tries to match the message with a MessageType, using the pattern passed.
        /// If the message matches the pattern, and the pattern contains the ?&lt;data&gt; placeholder,
        /// any strings in that placeholder are stored into mData.
        /// </summary>
        /// <param name="rule">The regex to look for</param>
        /// <param name="matchType">The type of match to perform, i.e. Regex or EndsWith</param>
        /// <param name="type">The type to set this object to if a match is found</param>
        /// <returns>True if the message matches, else False.</returns>
        private bool ProcessMatch(MatchTypes matchType, string rule, MessageTypes type)
        {
            bool retval = false;
            Data = new Dictionary<string, string>();

            switch (matchType)
            {
                case (MatchTypes.EndsWith):
                    if (Message.EndsWith(rule))
                    {
                        Type = type;
                        retval = true;
                    }
                    break;

                case (MatchTypes.Regex):
                    Regex RE = new Regex(rule);
                    if (RE.IsMatch(Message))
                    {
                        Type = type;
                        MatchCollection matches = Regex.Matches(Message, rule);
                        foreach (Match match in matches)
                        {
                            for (int i = 1; i < match.Groups.Count; i++)
                            {
                                Data.Add(RE.GroupNameFromNumber(i), match.Groups[i].Value);
                            }
                        }

                        retval = true;
                    }
                    break;
            }

            if (Data.ContainsKey("username"))
            {
                SetUser(Data["username"]);
            }

            return retval;
        }

    }
}
