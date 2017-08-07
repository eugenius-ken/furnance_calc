using FurnanceCalculator.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FurnanceCalculator.ViewModels
{
    public class BarParametersView
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Высота заготовки")]
        [Range(0, 5, ErrorMessage = "В пределах 0-5 метров")]
        public double? BarHeight { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Толщина заготовки")]
        [Range(0, 5, ErrorMessage = "В пределах 0-5 метров")]
        public double? BarThickness { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Длина заготовки")]
        [Range(0, 5, ErrorMessage = "В пределах 0-5 метров")]
        public double? BarLength { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Конечная температура верхней поверхности")]
        [Range(1000, 1300, ErrorMessage = "В пределах 1000-1300 градусов")]
        public double? EndTopSteelTemperature { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Конечная температура нижней поверхности")]
        [Range(1000, 1300, ErrorMessage = "В пределах 1000-1300 градусов")]
        public double? EndBottomSteelTemperature { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Начальная температура заготовки")]
        [Range(0, 50, ErrorMessage = "В пределах 0-50 градусов")]
        public double? StartSteelTemperature { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Температура поверхности в сечении 1")]
        [Range(500, 1000, ErrorMessage = "В пределах 500-1000 градусов")]
        public double? TopSteelTemperatureSector1 { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Тип стали")]
        public SteelTypeForDensity SteelTypeForDensity { get; set; } = SteelTypeForDensity.CastQuiet;

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Тип стали по содержанию элементов")]
        public SteelTypeProperty SteelTypeProperty { get; set; } = SteelTypeProperty.LowCarbon;

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Число рядов заготовок")]
        [Range(1, 3, ErrorMessage = "В пределах 1-3 рядов")]
        public Int32? BarNumber { get; set; }
    }
}