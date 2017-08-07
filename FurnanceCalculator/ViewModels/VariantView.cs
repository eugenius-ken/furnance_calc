using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FurnanceCalculator.ViewModels
{
    public class VariantView
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Введите название")]
        [DisplayName("Название")]
        public string Name { get; set; }

        [DisplayName("Описание")]
        public string Description { get; set; }

        public bool ResultIsExists { get; set; } = false;
    }
}