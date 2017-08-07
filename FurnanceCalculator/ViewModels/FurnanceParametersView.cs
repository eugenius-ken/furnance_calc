using FurnanceCalculator.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FurnanceCalculator.ViewModels
{
    public class FurnanceParametersView
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Высота рабочего пространства в зоне 1")]
        [Range(0, 5, ErrorMessage = "В пределах 0-5 метров")]
        public double? FurnanceHeightZone1 { get; set; } 

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Высота рабочей поверхности в сечении 0")]
        [Range(0, 5, ErrorMessage = "В пределах 0-5 метров")]
        public double? WorkHeightSector0 { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Высота рабочей поверхности в сечении 1")]
        [Range(0, 5, ErrorMessage = "В пределах 0-5 метров")]
        public double? WorkHeightSector1 { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Высота рабочей поверхности в сечении 2")]
        [Range(0, 5, ErrorMessage = "В пределах 0-5 метров")]
        public double? WorkHeightSector2 { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Производительность печи")]
        [Range(0, 50, ErrorMessage = "В пределах 0-50 тонн в час")]
        public double? FurnanceProductivity { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Ширина печи")]
        [Range(0, 5, ErrorMessage = "В пределах 0-5 метров")]
        public double? FurnanceWidth { get; set; } 

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Тип горелок")]
        public TorchType TorchType { get; set; } = TorchType.LongFlame;
    }
}