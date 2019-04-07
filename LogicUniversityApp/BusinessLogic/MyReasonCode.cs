using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicUniversityApp
{
    /// <summary>
    /// Enumerator to distinguish the reason for adjustment voucher
    /// </summary>
    public enum MyReasonCode
    {
        InstoreDamaged = 0,
        InstoreMissing = 1,
        Else = 2,
        InstoreWrongInput = 4,
        FreeOfCharge = 3
    };
}