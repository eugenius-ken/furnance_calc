using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
namespace ThreeBandContinuousFurnanceCalculating.EF.Models
{
    public class InputData
    {
        public Double HeatNatural { get; set; }
        public Double HeatFull { get; set; }
        public Double AirTemperature { get; set; }
        public Double StartSteelTemperature { get; set; }
        public Double FurnanceHeightZone1 { get; set; }
        public Double HeatBlast { get; set; }
        public Double EndTopSteelTemperature { get; set; }
        public Double EndBottomSteelTemperature { get; set; }
        public Double FurnanceWidth { get; set; }
        public Double BarHeight { get; set; }
        public Double BarThickness { get; set; }
        public Double BarLength { get; set; }
        public Double FurnanceProductivity { get; set; }

        public Int32 BarNumber { get; set; }

        //enums
        public Int32 TorchType { get; set; }
        public Int32 SteelTypeForDensity { get; set; }
        public Int32 SteelTypeProperty { get; set; }

        //Sector0
        public Double GasTemperatureSector0 { get; set; }
        public Double WorkHeightSector0 { get; set; }

        //Sector1
        public Double GasTemperatureSector1 { get; set; }
        public Double WorkHeightSector1 { get; set; }
        public Double TopSteelTemperatureSector1 { get; set; }

        //Sector2
        public Double GasTemperatureSector2 { get; set; }
        public Double WorkHeightSector2 { get; set; }
    }
}
