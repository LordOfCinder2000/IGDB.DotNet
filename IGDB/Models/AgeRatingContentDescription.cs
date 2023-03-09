namespace IGDB.Models
{
    public class AgeRatingContentDescription : IIdentifier, IHasChecksum
    {
        public AgeRatingContentDescriptionCategory? Category { get; set; }
        public string Checksum { get; set; }
        public string Description { get; set; }
        public long? Id { get; set; }
    }

    public enum AgeRatingContentDescriptionCategory
    {
        PEGI = 1,
        ESRB = 2,
        CERO = 3,
        USK = 4,
        GRAC = 5,
        CLASS_IND = 6,
        ACB = 7
    }
}