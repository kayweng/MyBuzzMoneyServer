using System.ComponentModel;

namespace MyBuzzMoney.Library.Enums
{
    public enum UserType
    {
        [Description("Guest")]
        Unconfirmed,

        [Description("General User")]
        Confirmed,

        [Description("Genuine User")]
        Genuine,

        [Description("VIP")]
        VIP,

        [Description("VVIP")]
        VVIP,

        [Description("Banned")]
        Banned
    }
}


