using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FurnanceCalculator.Models
{
    public class ResultData
    {
        [Key]
        [ForeignKey("Variant")]
        public Guid Id { get; set; }
        public virtual Variant Variant { get; set; }

        //Время нагрева заготовок
        public double HeatingTimeZone1 { get; set; }
        public double HeatingTimeZone2 { get; set; }
        public double HeatingTimeZone3 { get; set; }
        public double HeatingTimeFull { get; set; }

        //Длины зон
        public double LengthZone1 { get; set; }
        public double LengthZone2 { get; set; }
        public double LengthZone3 { get; set; }
        public double LengthFull { get; set; }

        //Расчет горения топлива
        public double ConsumptionTemperature { get; set; }

        //Средний тепловой поток
        public double AverageHeatFlowZone1 { get; set; }
        public double AverageHeatFlowZone2 { get; set; }

        //Температура массы слитка
        public double BarTemperatureSection1 { get; set; }
        public double BarTemperatureSection2 { get; set; }
        public double BarTemperatureSection3 { get; set; }

        //Удельный тепловой поток
        public double SummaryHeatFlowSector0 { get; set; }
        public double SummaryHeatFlowSector1Zone1 { get; set; }
        public double SummaryHeatFlowSector1Zone2 { get; set; }
        public double SummaryHeatFlowSector2 { get; set; }

        public void Round()
        {
            this.AverageHeatFlowZone1 = Math.Round(this.AverageHeatFlowZone1, 2);
            this.AverageHeatFlowZone2 = Math.Round(this.AverageHeatFlowZone2, 2);
            this.BarTemperatureSection1 = Math.Round(this.BarTemperatureSection1, 2);
            this.BarTemperatureSection2 = Math.Round(this.BarTemperatureSection2, 2);
            this.BarTemperatureSection3 = Math.Round(this.BarTemperatureSection3, 2);
            this.ConsumptionTemperature = Math.Round(this.ConsumptionTemperature, 2);
            this.HeatingTimeFull = Math.Round(this.HeatingTimeFull, 2);
            this.HeatingTimeZone1 = Math.Round(this.HeatingTimeZone1, 2);
            this.HeatingTimeZone2 = Math.Round(this.HeatingTimeZone2, 2);
            this.HeatingTimeZone3 = Math.Round(this.HeatingTimeZone3, 2);
            this.LengthFull = Math.Round(this.LengthFull, 2);
            this.LengthZone1 = Math.Round(this.LengthZone1, 2);
            this.LengthZone2 = Math.Round(this.LengthZone2, 2);
            this.LengthZone3 = Math.Round(this.LengthZone3, 2);
            this.SummaryHeatFlowSector0 = Math.Round(this.SummaryHeatFlowSector0, 2);
            this.SummaryHeatFlowSector1Zone1 = Math.Round(this.SummaryHeatFlowSector1Zone2, 2);
            this.SummaryHeatFlowSector1Zone2 = Math.Round(this.SummaryHeatFlowSector1Zone2, 2);
            this.SummaryHeatFlowSector2 = Math.Round(this.SummaryHeatFlowSector2, 2);
        }
    }
}
