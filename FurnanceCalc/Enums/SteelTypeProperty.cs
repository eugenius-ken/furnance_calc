using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FurnanceCalc.Enums
{
    public enum SteelTypeProperty
    {
        [Description("Малоугледродистая")]
        LowCarbon,
        [Description("Среднеугледродистая")]
        MiddleCarbon,
        [Description("Высокоугледродистая")]
        HighCarbon,
        [Description("Нержавеющая")]
        Stainless,
        [Description("Жаропрочная")]
        Heatproof
    }
}
