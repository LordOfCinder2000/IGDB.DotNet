namespace IGDB
{
    public static class TagNumberHelper
    {
        public static long Generate(TagType tagType, long targetId)
        {
            var tagNumber = ((long)tagType) << 28;
            return tagNumber |= targetId;
        }
    }

    public enum TagType
    {
        Theme = 0,
        Genre = 1,
        Keyword = 2,
        Game = 3,
        PlayerPerspective = 4
    }
}
