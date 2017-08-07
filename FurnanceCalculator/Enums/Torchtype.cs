using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FurnanceCalculator.Enums
{
    public enum TorchType
    {
        [Display(Name = "Короткопламенные")]
        ShortFlame,
        [Display(Name = "Длиннопламенные")]
        LongFlame
    }
}
