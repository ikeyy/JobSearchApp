using System.Runtime.Serialization;

namespace JobSearchApp.Domain.Enums
{
    public enum JobOfferStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,

        [EnumMember(Value = "Accepted")]
        Accepted,

        [EnumMember(Value = "Rejected")]
        Rejected
    }
}
