using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FurnanceCalculator.Models
{
    public class Variant
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual InputData InputData { get; set; }
        public virtual ResultData ResultData { get; set; }

        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public bool IsBarParametersExist { get; set; } = false;
        public bool IsFurnanceParametersExist { get; set; } = false;
        public bool IsEnvironmentParametersExist { get; set; } = false;
        public bool IsFuelParametersExist { get; set; } = false;
    }
}