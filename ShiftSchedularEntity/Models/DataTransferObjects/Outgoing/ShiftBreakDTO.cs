using ShiftSchedularEntity.Converters;
using System.Text.Json.Serialization;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class ShiftBreakDTO
    {
        public Guid ShiftBreakId { get; set; }
        public Guid ShiftParentId { get; set; }
        public int ShiftBreakTypeId { get; set; }
        public string ShiftBreakTypeDisplay { get; set; }
        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan ShiftBreakStartTime { get; set; }
        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan ShiftBreakDuration { get; set; }
        public bool IncludedInShift { get; set; }
        public bool IsTimeFlexible { get; set; }
    }
}
