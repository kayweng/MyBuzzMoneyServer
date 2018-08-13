using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace MyBuzzMoney.Library.Enums
{
    public enum VerifiedType
    {
        [Description("Verified")]
        Verified,

        [Description("Pending")]
        Pending,

        [Description("Expired")]
        Expired,
    }
}
