using System.Collections.Generic;
using System.Linq;

namespace VerySimpleTwitchChat
{
    [System.Serializable]
    public struct ChatterEmote
    {
        [System.Serializable]
        public struct Index
        {
            public int startIndex, endIndex;
        }

        public string id;
        public Index[] indexes;
    }

    [System.Serializable]
    public struct ChatterBadge
    {
        public string id;
        public string version;
    }

    [System.Serializable]
    public class IRCTags
    {
        public string colorHex = string.Empty;
        public string displayName = string.Empty;
        public string channelId = string.Empty;
        public string userId = string.Empty;

        public ChatterBadge[] badges = new ChatterBadge[0];
        public ChatterEmote[] emotes = new ChatterEmote[0];

        public bool ContainsEmote(string emoteId)
        {
            return emotes.Any(b => b.id == emoteId);
        }

        public bool HasBadge(string badge)
        {
            return badges.Any(b => b.id == badge);
        }
    }
}