using System.Text.Json.Serialization;

namespace IGDB.Models
{
    public class MultiplayerMode : IIdentifier, IHasChecksum
    {
        [JsonPropertyName("campaigncoop")]
        public bool? CampaignCoop { get; set; }
        public string Checksum { get; set; }
        [JsonPropertyName("dropin")]
        public bool? DropIn { get; set; }
        public IdentityOrValue<Game> Game { get; set; }
        public long? Id { get; set; }
        [JsonPropertyName("lancoop")]
        public bool? LanCoop { get; set; }
        [JsonPropertyName("offlinecoop")]
        public bool? OfflineCoop { get; set; }
        [JsonPropertyName("offlinecoopmax")]
        public int? OfflineCoopMax { get; set; }
        [JsonPropertyName("offlinemax")]
        public int? OfflineMax { get; set; }
        [JsonPropertyName("onlinecoop")]
        public bool? OnlineCoop { get; set; }
        [JsonPropertyName("onlinecoopmax")]
        public int? OnlineCoopMax { get; set; }
        [JsonPropertyName("onlinemax")]
        public int? OnlineMax { get; set; }
        public IdentityOrValue<Platform> Platform { get; set; }
        [JsonPropertyName("splitscreen")]
        public bool? SplitScreen { get; set; }
        [JsonPropertyName("splitscreenonline")]
        public bool? SplitScreenOnline { get; set; }
    }
}