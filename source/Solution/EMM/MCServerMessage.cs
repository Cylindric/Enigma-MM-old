using System.Text.RegularExpressions;

namespace EnigmaMM
{
    class MCServerMessage
    {
        private string mMessage;
        private string mData;
        private MessageType mType;

        public enum MessageType
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

            // Hey0-specific types
            HModBanner,

            // Anything else is an 'other'
            Other
        }


        private class MessagePattern
        {
            public const byte REGEX = 0;
            public const byte ENDSWITH = 1;

            public string MatchPattern;
            public MessageType MessageType;
            public byte MatchType;
            public MessagePattern(MessageType type, byte matchType, string matchPattern)
            {
                MessageType = type;
                MatchType = matchType;
                MatchPattern = matchPattern;
            }
        }


        /// <summary>
        /// The array of server messages should ideally be defined with regex last, and in order of message frequency, so that
        /// more common messages are trapped first.
        /// </summary>
        private static MessagePattern[] sPatterns = 
        {
            // Standard Minecraft messages
            new MessagePattern(MessageType.StartupComplete, MessagePattern.ENDSWITH, @"[INFO] Done! For help, type ""help"" or ""?"""),
            new MessagePattern(MessageType.SaveComplete, MessagePattern.ENDSWITH, @"[INFO] CONSOLE: Save Complete"),
            new MessagePattern(MessageType.ErrorPortBusy, MessagePattern.ENDSWITH, @"[WARNING] **** FAILED TO BIND TO PORT!"),
            new MessagePattern(MessageType.SaveStarted, MessagePattern.ENDSWITH, @"[INFO] CONSOLE: Forcing save.."),
            new MessagePattern(MessageType.UserLoggedIn, MessagePattern.REGEX, @"^(?<timestamp>.+?)\[INFO]\ (?<data>\w+?)\ \[(?<data2>.+?)]\ logged\ in$"),
            new MessagePattern(MessageType.AutoSaveEnabled, MessagePattern.ENDSWITH, @"[INFO] CONSOLE: Enabling level saving.."),
            new MessagePattern(MessageType.AutoSaveDisabled, MessagePattern.ENDSWITH, @"[INFO] CONSOLE: Disabling level saving.."),
            new MessagePattern(MessageType.UserList, MessagePattern.REGEX, @"^(?<timestamp>.+?)\[INFO]\ Connected\ players:\ (?<data>.*?)$"),
            new MessagePattern(MessageType.UserCount, MessagePattern.REGEX, @"^Player\ count:\ (?<data>\d+)$"),
            new MessagePattern(MessageType.MinecraftBanner, MessagePattern.REGEX, @"^(?<timestamp>.+?)\[INFO]\ Starting\ minecraft\ server\ version\ (?<data>.*?)$"),

            // Hey0-specific messages
            new MessagePattern(MessageType.HModBanner, MessagePattern.REGEX, @"^(?<timestamp>.+?)\[INFO]\ Hey0\ Server\ Mod\ Build\ (?<data>.*?)$"),
        };



        public MessageType Type
        {
            get { return mType; }
        }

        public string Data
        {
            get { return mData; }
        }

        public MCServerMessage(string msg)
        {
            mMessage = msg;
            DetermineType();
        }
       
        /// <summary>
        /// Iterates over all the defined Patterns to determine the type of message and if applicable
        /// extracts any pertinent data.
        /// </summary>
        private void DetermineType()
        {
            mType = MessageType.Other;
            foreach (MessagePattern pattern in sPatterns)
            {
                if (ProcessMatch(pattern.MatchType, pattern.MatchPattern, pattern.MessageType)) { return; }
            }
        }
        
        /// <summary>
        /// Tries to match the message with a MessageType, using the pattern passed.
        /// If the message matches the pattern, and the pattern contains the ?&lt;data&gt; placeholder,
        /// any strings in that placeholder are stored into mData.
        /// </summary>
        /// <param name="regex">The regex to look for</param>
        /// <param name="matchType">The type of match to perform, i.e. Regex or EndsWith</param>
        /// <param name="type">The type to set this object to if a match is found</param>
        /// <returns>True if the message matches, else False.</returns>
        private bool ProcessMatch(byte matchType, string regex, MessageType type)
        {
            bool retval = false;

            switch (matchType)
            {
                case (MessagePattern.ENDSWITH):
                    if (mMessage.EndsWith(regex))
                    {
                        mType = type;
                        mData = "";
                        retval = true;
                    }
                    break;

                case (MessagePattern.REGEX):
                    if (Regex.IsMatch(mMessage, regex))
                    {
                        mType = type;
                        mData = "";
                        if (regex.Contains("?<data>"))
                        {
                            MatchCollection matches = Regex.Matches(mMessage, regex);
                            if (matches.Count > 0)
                            {
                                mData = matches[0].Groups["data"].Value;
                            }
                        }
                        retval = true;
                    }
                    break;
            }

            return retval;
        }

    }
}
