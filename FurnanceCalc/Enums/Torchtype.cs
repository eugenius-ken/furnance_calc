using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FurnanceCalc.Enums
{
    public enum TorchType
    {
        [Description("Короткопламенные")]
        ShortFlame,

        [Description("Длиннопламенные")]
        LongFlame
    }
}
