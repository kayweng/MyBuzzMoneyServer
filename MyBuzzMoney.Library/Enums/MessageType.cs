using System.ComponentModel;

namespace MyBuzzMoney.Library.Enums
{
    public enum MessageType
    {
        [Description("Information")]
        Info,

        [Description("Warning")]
        Warning,

        [Description("Alert")]
        Alert
    }
}
