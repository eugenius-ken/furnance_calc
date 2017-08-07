using FurnanceCalculator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace FurnanceCalculator.ViewModels
{
    public class ResultView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        //Время нагрева заготовок
        [DisplayName("Время нагрева заготовок в зоне 1")]
        public double HeatingTimeZone1 { get; set; }
        [DisplayName("Время нагрева заготовок в зоне 2")]
        public double HeatingTimeZone2 { get; set; }
        [DisplayName("Время нагрева заготовок в зоне 3")]
        public double HeatingTimeZone3 { get; set; }
        [DisplayName("Общее время нагрева")]
        public double HeatingTimeFull { get; set; }

        //Длины зон
        [DisplayName("Зона 1")]
        public double LengthZone1 { get; set; }
        [DisplayName("Зона 2")]
        public double LengthZone2 { get; set; }
        [DisplayName("Зона 3")]
        public double LengthZone3 { get; set; }
        [DisplayName("Активная часть пода печи")]
        public double LengthFull { get; set; }

        //Расчет горения топлива
        [DisplayName("Температура газовой среды печи")]
        public double ConsumptionTemperature { get; set; }

        //Средний тепловой поток
        [DisplayName("В зоне 1")]
        public double AverageHeatFlowZone1 { get; set; }
        [DisplayName("В зоне 2")]
        public double AverageHeatFlowZone2 { get; set; }

        //Температура массы слитка
        [DisplayName("В сечении 1")]
        public double BarTemperatureSection1 { get; set; }
        [DisplayName("В сечении 2")]
        public double BarTemperatureSection2 { get; set; }
        [DisplayName("В сечении 3")]
        public double BarTemperatureSection3 { get; set; }

        //Удельный тепловой поток
        [DisplayName("В сечении 0")]
        public double SummaryHeatFlowSector0 { get; set; }
        [DisplayName("В сечении 1 для зоны 1")]
        public double SummaryHeatFlowSector1Zone1 { get; set; }
        [DisplayName("В сечении 1 для зоны 2")]
        public double SummaryHeatFlowSector1Zone2 { get; set; }
        [DisplayName("В сечении 2")]
        public double SummaryHeatFlowSector2 { get; set; }

        //Время нагрева заготовок
        public string StyleHeatingTimeZone1 { get; set; }
        public string StyleHeatingTimeZone2 { get; set; }
        public string StyleHeatingTimeZone3 { get; set; }
        public string StyleHeatingTimeFull { get; set; }

        //Длины зон
        public string StyleLengthZone1 { get; set; }
        public string StyleLengthZone2 { get; set; }
        public string StyleLengthZone3 { get; set; }
        public string StyleLengthFull { get; set; }

        //Расчет горения топлива
        public string StyleConsumptionTemperature { get; set; }

        //Средний тепловой поток
        public string StyleAverageHeatFlowZone1 { get; set; }
        public string StyleAverageHeatFlowZone2 { get; set; }

        //Температура массы слитка
        public string StyleBarTemperatureSection1 { get; set; }
        public string StyleBarTemperatureSection2 { get; set; }
        public string StyleBarTemperatureSection3 { get; set; }

        //Удельный тепловой поток
        public string StyleSummaryHeatFlowSector0 { get; set; }
        public string StyleSummaryHeatFlowSector1Zone1 { get; set; }
        public string StyleSummaryHeatFlowSector1Zone2 { get; set; }
        public string StyleSummaryHeatFlowSector2 { get; set; }


        public static ResultView CopyFrom(ResultData results)
        {
            return new ResultView()
            {
                AverageHeatFlowZone1 = results.AverageHeatFlowZone1,
                AverageHeatFlowZone2 = results.AverageHeatFlowZone2,
                BarTemperatureSection1 = results.BarTemperatureSection1,
                BarTemperatureSection2 = results.BarTemperatureSection2,
                BarTemperatureSection3 = results.BarTemperatureSection3,
                ConsumptionTemperature = results.ConsumptionTemperature,
                HeatingTimeFull = results.HeatingTimeFull,
                HeatingTimeZone1 = results.HeatingTimeZone1,
                HeatingTimeZone2 = results.HeatingTimeZone2,
                HeatingTimeZone3 = results.HeatingTimeZone3,
                LengthFull = results.LengthFull,
                LengthZone1 = results.LengthZone1,
                LengthZone2 = results.LengthZone2,
                LengthZone3 = results.LengthZone3,
                SummaryHeatFlowSector0 = results.SummaryHeatFlowSector0,
                SummaryHeatFlowSector1Zone1 = results.SummaryHeatFlowSector1Zone1,
                SummaryHeatFlowSector1Zone2 = results.SummaryHeatFlowSector1Zone2,
                SummaryHeatFlowSector2 = results.SummaryHeatFlowSector2
            };
        }
    }
}