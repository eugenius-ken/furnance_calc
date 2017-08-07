using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FurnanceCalculator.Enums
{
    public enum SteelTypeForDensity
    {
        [Display(Name = "Литая кипящая")]
        CastBoiling,
        [Display(Name = "Литая спокойная")]
        CastQuiet,
        [Display(Name = "Кованая или прокатанная")]
        ForgedLaminated
    }
}
