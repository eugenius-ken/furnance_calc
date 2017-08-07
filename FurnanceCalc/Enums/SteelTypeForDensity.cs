using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FurnanceCalc.Enums
{
    public enum SteelTypeForDensity
    {
        [Description("Литая кипящая")]
        CastBoiling,
        [Description("Литая спокойная")]
        CastQuiet,
        [Description("Кованая или прокатанная")]
        ForgedLaminated
    }
}
