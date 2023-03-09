namespace IGDB
{
    public static class TagNumberHelper
    {
        public static int Generate(TagType tagType, int targetId)
        {
            var tagNumber = ((int)tagType) << 28;
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
