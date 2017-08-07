using FurnanceCalculator.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FurnanceCalculator.Models
{
    public class InputData
    {
        [Key]
        [ForeignKey("Variant")]
        public Guid Id { get; set; }
        public virtual Variant Variant { get; set; }

        //Характеристики заготовок
        public double BarHeight { get; set; }
        public double BarThickness { get; set; }
        public double BarLength { get; set; }
        public double EndTopSteelTemperature { get; set; }
        public double EndBottomSteelTemperature { get; set; }
        public double StartSteelTemperature { get; set; } 
        public double TopSteelTemperatureSector1 { get; set; }
        public SteelTypeForDensity SteelTypeForDensity { get; set; }
        public SteelTypeProperty SteelTypeProperty { get; set; }
        public Int32 BarNumber { get; set; }

        //Характеристики печи
        public double FurnanceHeightZone1 { get; set; }
        public double WorkHeightSector0 { get; set; }
        public double WorkHeightSector1 { get; set; }
        public double WorkHeightSector2 { get; set; }
        public double FurnanceProductivity { get; set; }
        public double FurnanceWidth { get; set; }
        public TorchType TorchType { get; set; }

        //Характеристики среды
        public double AirTemperature { get; set; }
        public double GasTemperatureSector0 { get; set; }
        public double GasTemperatureSector1 { get; set; }
        public double GasTemperatureSector2 { get; set; }

        //Характеристики топлива
        public double HeatNatural { get; set; }
        public double HeatBlast { get; set; }
        public double HeatFull { get; set; }
    }
}
