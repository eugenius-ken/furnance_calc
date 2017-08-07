using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FurnanceCalculator.ViewModels
{
    public class FuelParametersView
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Теплота сгорания природного газа")]
        [Range(0, 40000, ErrorMessage = "В пределах 0-40000 кДЖ")]
        public double? HeatNatural { get; set; } 

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Теплота сгорания доменного газа")]
        [Range(0, 40000, ErrorMessage = "В пределах 0-40000 кДЖ")]
        public double? HeatBlast { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DisplayName("Теплота сгорания всей смеси")]
        [Range(0, 40000, ErrorMessage = "В пределах 0-40000 кДЖ")]
        public double? HeatFull { get; set; }
    }
}