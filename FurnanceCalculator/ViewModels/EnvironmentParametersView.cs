using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FurnanceCalculator.ViewModels
{
    public class EnvironmentParametersView
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Температура воздуха в рекуператоре")]
        [Range(100, 500, ErrorMessage = "В пределах 100-500 градусов")]
        public double? AirTemperature { get; set; } 

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Температура газов в сечении 0")]
        [Range(500, 1000, ErrorMessage = "В пределах 500-1000 градусов")]
        public double? GasTemperatureSector0 { get; set; } 

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Температура газов в сечении 1")]
        [Range(1000, 1500, ErrorMessage = "В пределах 1000-1500 метров")]
        public double? GasTemperatureSector1 { get; set; } 

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Температура газов в сечении 2")]
        [Range(1000, 1500, ErrorMessage = "В пределах 1000-1500 метров")]
        public double? GasTemperatureSector2 { get; set; }
    }
}