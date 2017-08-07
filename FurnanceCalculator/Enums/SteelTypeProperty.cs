using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FurnanceCalculator.Enums
{
    public enum SteelTypeProperty
    {
        [Display(Name = "Малоугледродистая")]
        LowCarbon,
        [Display(Name = "Среднеугледродистая")]
        MiddleCarbon,
        [Display(Name = "Высокоугледродистая")]
        HighCarbon,
        [Display(Name = "Нержавеющая")]
        Stainless,
        [Display(Name = "Жаропрочная")]
        Heatproof
    }
}
