using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ThreeBandContinuousFurnanceCalculating.EF.Models
{
    public class ResultData
    {
        public Double ConsumptionTemperature { get; set; }

        public Double SummaryHeatFlowSector0 { get; set; }
        public Double SummaryHeatFlowSector1Zone1 { get; set; }
        public Double SummaryHeatFlowSector1Zone2 { get; set; }
        public Double SummaryHeatFlowSector2 { get; set; }

        public Double BarTemperatureSection1 { get; set; }
        public Double BarTemperatureSection2 { get; set; }
        public Double BarTemperatureSection3 { get; set; }

        public Double AverageHeatFlowZone1 { get; set; }
        public Double AverageHeatFlowZone2 { get; set; }

        public Double HeatingTimeZone1 { get; set; }
        public Double HeatingTimeZone2 { get; set; }
        public Double HeatingTimeZone3 { get; set; }
        public Double HeatingTimeFull { get; set; }

        public Double LengthZone1 { get; set; }
        public Double LengthZone2 { get; set; }
        public Double LengthZone3 { get; set; }
        public Double LengthFull { get; set; }
    }
}
