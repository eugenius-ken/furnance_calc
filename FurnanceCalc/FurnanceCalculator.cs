using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FurnanceCalc.Enums;

namespace FurnanceCalc
{
    public class FurnanceCalculator
    {
        public FurnanceCalculator()
        {
        }

        /// <summary>
        /// Вычисляет температуру горения топлива
        /// </summary>
        /// <param name="isLongFlameTorch">Является ли горелка длиннопламенной</param>
        /// <param name="combustionHeatBlast">Теплота сгорания доменного газа</param>
        /// <param name="combustionHeatNatural">Теплота сгорания природного газа</param>
        /// <param name="combustionHeatFull">Теплота сгорания всей смеси топлива</param>
        /// <param name="airTemperature">Температура воздуха, подогреваемого в рекуператоре</param>
        /// <returns></returns>
        public Double GetConsumptionTemperature(Boolean isLongFlameTorch, Double combustionHeatBlast, Double combustionHeatNatural, Double combustionHeatFull, Double airTemperature)
        {
            //Необходимое количество воздуха для сжигания смеси
            Double amountAirForFullBurning = GetAirAmountForFullBurning(combustionHeatBlast, combustionHeatNatural, combustionHeatFull);
            //Разность между объемом продуктов сгорания всей смеси и газа, израсходованного на горение воздуха
            Double diffBetweenFullAndAir = GetDiffBetweenNaturalAndAir(combustionHeatBlast, combustionHeatNatural, combustionHeatFull);
            //Действительный расход воздуха для сжигания кубического метра смеси
            Double airConsumptionForBurningFuel = GetAirConsumptionForBurningFuel(isLongFlameTorch, amountAirForFullBurning);
            //Выход продуктов сгорания
            Double combustionProducts = airConsumptionForBurningFuel + diffBetweenFullAndAir;
            //Содержание избыточного воздуха в продуктах горения
            Double airInCombustionProducts = (airConsumptionForBurningFuel - amountAirForFullBurning) / combustionProducts * 100;
            //химический недожог топлива
            Double q3 = isLongFlameTorch ? 0.03 * combustionHeatFull : 0.015 * combustionHeatFull;
            //Теплосодержание продуктов горения в соответствии с балансовой температурой
            Double combustionProductsEnthalpy = (combustionHeatFull + (0.0001 * airTemperature + 1.2864) * airTemperature * amountAirForFullBurning - q3) / combustionProducts;

            Double tempWithoutCorrection;
            if (combustionHeatFull > 12000)
                tempWithoutCorrection = (combustionProductsEnthalpy + 145.64) / 1.7378;
            else if (combustionHeatFull < 8000)
                tempWithoutCorrection = (combustionProductsEnthalpy + 154.32) / 1.8007;
            else
                tempWithoutCorrection = (combustionProductsEnthalpy + 133.14) / 1.743;

            Double result = 0.8 * (tempWithoutCorrection + airInCombustionProducts * (0.0042 * tempWithoutCorrection - 3.8614));

            if (Double.IsNaN(result))
                throw new Exception("Характеристики топлива заданы неверно");

            return result;
        }

        /// <summary>
        /// Проверяет, осуществим ли технологический процесс
        /// </summary>
        /// <param name="steelFinishtemperature">Конечная температура верхней поверхности заготовки</param>
        /// <param name="consumptionTemperature">Температура горения топлива</param>
        /// <returns></returns>
        public Boolean IsProcessAvailable(Double steelFinishtemperature, Double consumptionTemperature)
        {
            return consumptionTemperature > (steelFinishtemperature + 100) ? true : false;
        }

        /// <summary>
        /// Вычисляет суммарный тепловой поток в сечении 2
        /// </summary>
        /// <param name="d">Ширина печи</param>
        /// <param name="hz">Высота заготовки</param>
        /// <param name="hp">Высота рабочего пространства печи в сечении 2</param>
        /// <param name="combustionHeatBlast">Теплота сгорания доменного газа</param>
        /// <param name="combustionHeatNatural">Теплота сгорания природного газа</param>
        /// <param name="combustionHeatFull">Теплота сгорания всей смеси топлива</param>
        /// <param name="isLongFlameTorch">Является ли горелка длиннопламенной</param>
        /// <param name="l">Длина заготовки</param>
        /// <param name="n">Число рядов заготовок по ширине печи</param>
        /// <param name="surfaceT">Температура верхней поверхности заготовки в сечении 2 (равна конечной температуре поверхности заготовки)</param>
        /// <param name="gasT">Температура газов в сечении 2 (1280-1380 градусов)</param>
        /// <returns></returns>
        public Double GetSummaryHeatFlowOfSection2(Boolean isLongFlameTorch, Double d, Double hz, Double hp, Double combustionHeatBlast, Double combustionHeatNatural, Double combustionHeatFull, Double gasT, Double l, Int32 n, Double surfaceT)
        {
            //Высота рабочего пространства над заготовками
            Double h2 = hp - hz;

            //Эффективная толщина излучающего слоя печных газов
            Double Seff = 1.8 * d * h2 / (d + h2);
            //Температура газов в зависимости от размера заготовок
            //Int32 Tg = isSteelMassive ? 1280 : 1380;

            //Содержание H20 и СО2 в продуктах горения природно-доменной смеси
            Double deltaV = GetDiffBetweenNaturalAndAir(combustionHeatBlast, combustionHeatNatural, combustionHeatFull);
            Double L0 = GetAirAmountForFullBurning(combustionHeatBlast, combustionHeatNatural, combustionHeatFull);
            Double Lalpha = GetAirConsumptionForBurningFuel(isLongFlameTorch, L0);
            Double H20 = (0.0006 * combustionHeatFull - 1.0069) * ((L0 + deltaV) / (Lalpha + deltaV));
            Double CO2 = (-0.0008 * combustionHeatFull + 33.062) * ((L0 + deltaV) / (Lalpha + deltaV));

            Double tmpForEH2O = 0.01 * Seff * H20;
            Double tmpForECO2 = 0.01 * Seff * CO2;
            //Double pressureH2O = 

            Double Eco2 = GetEmissivityCO2(tmpForECO2, gasT);
            Double Eh2oTemp = GetEmissivityTempH2O(tmpForEH2O, gasT);
            if(Eco2 == Double.NaN || Eh2oTemp == Double.NaN)
                throw new Exception("Температура газов в сечении 2 вне допустимых значений. Допустимое значение от 600 до 1500 градусов.");
            //поправочный множитель
            Double b = GetKoefForEmissivityH2O(Eh2oTemp, 0.01 * H20);
            //Степень черноты водяного пара
            Double Eh20 = b * Eh2oTemp;
            //Степень черноты горящих газов
            Double Eg = (Eh20 + Eco2) * 1.8; //1.8 - коэффициент, зависит от смеси и от рассчитываемого сечения
            //Степень развития кладки
            Double w2 = (2 * h2 + d) / (l * n);
            //Приведенный коэффициент излучения для системы "газ-кладка-металл"
            Double Cgkm = 5.7 * 0.8 * (1 + w2 - Eg) / ((0.8 + Eg * (1 - 0.8)) * ((1 - Eg) / Eg) + w2);
            //Удельный лучистый тепловой поток
            Double ql = Cgkm * 1 * (Math.Pow((gasT + 273) / 100, 4) - Math.Pow((surfaceT + 273) / 100, 4)); // 1 - угловой коэффициент металла на кладку
            //Конвективная составляющая общего теплового потока
            Double qk = 55 * (gasT - surfaceT); //55 - коэффициент конвективного теплообмена, зависящий от угла наклона торцовых горелок
            return qk + ql;
        }

        /// <summary>
        /// Вычисляет температуру массы верхней части заготовки в сечении 2
        /// </summary>
        /// <param name="surfaceT">Температура верхней поверхности заготовки</param>
        /// <param name="tc">Минимальная температура заготовки в сечении 2</param>
        /// <returns></returns>
        public Double GetMetalTemperatureInSection2(Double surfaceT, Double tc)
        {
            return surfaceT - 2 * (surfaceT - tc) / 3;
        }

        /// <summary>
        /// Вычисляет минимальную температуру заготовки перед заходом на сплошной под (в сечении 2)
        /// </summary>
        /// <param name="S">Высота заготовки</param>
        /// <param name="surfaceT">Температура верхней поверхности заготовки</param>
        /// <param name="qm">Суммарный тепловой поток в сечении 2</param>
        /// <returns></returns>
        public Double GetMinimumMetalTemperatureInSection2(Double S, Double surfaceT, Double qm, SteelTypeProperty steelType)
        {
            //Верхняя прогреваемая толщина заготовки
            Double Sv = (-0.92 * 0.5 + 1.052) * S; //0.5 - тепловая мощность низа печи

            return surfaceT - ((qm * Sv) / (2 * (GetThermalConductivityBySteelType(surfaceT - 75, steelType))));
        }

        /// <summary>
        /// Вычисляет температуру массы верхней части заготовки в сечении 1
        /// </summary>
        /// <param name="S">Высота заготовки</param>
        /// <param name="surfaceT">Температуры верхней поверхности заготовки</param>
        /// <param name="qm">Суммарный тепловой поток в сечении 1</param>
        /// <returns></returns>
        public Double GetMetalTemperatureInSection1(Double S, Double surfaceT, Double qm, SteelTypeProperty steeltype)
        {
            //Верхняя прогреваемая толщина заготовки
            //Double Sv = (-0.92 * 0.5 + 1.052) * S; //0.5 - тепловая мощность низа печи
            ////Минимальная температура перед его заходом на сплошной под
            //Double tc = surfaceT - ((qm * Sv) / (2 * (268.55 * Math.Pow((surfaceT - 75), -0.315))));
            Double tc = GetMinimumMetalTemperatureInSection2(S, surfaceT, qm, steeltype);

            return surfaceT - 2 * (surfaceT - tc) / 3;
        }

        /// <summary>
        /// Вычисляет суммарный тепловой поток в сечении 0
        /// </summary>
        /// <param name="d">Ширина печи</param>
        /// <param name="hz">Высота заготовки</param>
        /// <param name="hp">Высота рабочего пространства печи в сечении 0</param>
        /// <param name="combustionHeatBlast">Теплота сгорания доменного газа</param>
        /// <param name="combustionHeatNatural">Теплота сгорания природного газа</param>
        /// <param name="combustionHeatFull">Теплота сгорания всей смеси топлива</param>
        /// <param name="isLongFlameTorch">Является ли горелка длиннопламенной</param>
        /// <returns></returns>
        public Double GetSummaryHeatFlowOfSection0(Boolean isLongFlameTorch, Double d, Double hz, Double hp, Double combustionHeatBlast, Double combustionHeatNatural, Double combustionHeatFull, Double gasT, Double l, Double n, Double surfaceT)
        {
            //Температура газов
            //Double Tg = 625+0.68*(isSteelMassive ? 400 : 700)*n*l/d*(1+d/n/l)-300*n*l/d;
            //Высота рабочего пространства над заготовками
            Double h = hp - hz;
            //Эффективная толщина излучающего слоя печных газов
            Double Seff = 1.8 * d * h / (d + h);
            //Содержание H20 и СО2 в продуктах горения природно-доменной смеси
            Double deltaV = GetDiffBetweenNaturalAndAir(combustionHeatBlast, combustionHeatNatural, combustionHeatFull);
            Double L0 = GetAirAmountForFullBurning(combustionHeatBlast, combustionHeatNatural, combustionHeatFull);
            Double Lalpha = GetAirConsumptionForBurningFuel(isLongFlameTorch, L0);
            Double H20 = (0.0006 * combustionHeatFull - 1.0069) * (L0 + deltaV) / (Lalpha + deltaV);
            Double CO2 = (-0.0008 * combustionHeatFull + 33.062) * (L0 + deltaV) / (Lalpha + deltaV);

            Double tmpForEH2O = 0.01 * Seff * H20;
            Double tmpForECO2 = 0.01 * Seff * CO2;

            Double Eco2 = GetEmissivityCO2(tmpForECO2, gasT);
            Double Eh2oTemp = GetEmissivityTempH2O(tmpForEH2O, gasT);
            if (Eco2 == Double.NaN || Eh2oTemp == Double.NaN)
                throw new Exception("Температура газов в сечении 2 вне допустимых значений. Допустимое значение от 600 до 1500 градусов.");

            Double b = GetKoefForEmissivityH2O(Eh2oTemp, 0.01 * H20);

            Double Eh20 = b * Eh2oTemp;

            Double Eg = Eh20 + Eco2;//когда сделаю, вынести в отдельную функцию

            //Степень развития кладки
            Double w = (2 * h + d) / (l * n);
            //Приведенный коэффициент излучения для системы "газ-кладка-металл"
            Double Cgkm = 5.7 * 0.8 * (1 + w - Eg) / ((0.8 + Eg * (1 - 0.8)) * ((1 - Eg) / Eg) + w);

            return 1.03 * Cgkm * (Math.Pow(((gasT + 273) / 100), 4) - Math.Pow(((surfaceT + 273) / 100), 4));
        }

        /// <summary>
        /// Вычисляет суммарный тепловой поток в сечении 1 со стороны зоны 1
        /// </summary>
        /// <param name="hz">Высота заготовки</param>
        /// <param name="hp">>Высота рабочего пространства печи в зоне 1</param>
        /// <param name="d">Ширина печи</param>
        /// <param name="combustionHeatBlast">Теплота сгорания доменного газа</param>
        /// <param name="combustionHeatNatural">Теплота сгорания природного газа</param>
        /// <param name="combustionHeatFull">Теплота сгорания всей смеси топлива</param>
        /// <param name="isLongFlameTorch">Является ли горелка длиннопламенной</param>
        /// <returns></returns>
        public Double GetSummaryHeatFlowOfSection1ForZone1(Boolean isLongFlameTorch, Double d, Double hz, Double hp, Double combustionHeatBlast, Double combustionHeatNatural, Double combustionHeatFull, Double gasT, Double l, Double n, Double surfaceT)
        {
            //Высота рабочего пространства над заготовками
            Double h = hp - hz;

            //Эффективная толщина излучающего слоя печных газов
            Double Seff = 1.8 * d * h / (d + h);
            //Содержание H20 и СО2 в продуктах горения природно-доменной смеси
            Double deltaV = GetDiffBetweenNaturalAndAir(combustionHeatBlast, combustionHeatNatural, combustionHeatFull);
            Double L0 = GetAirAmountForFullBurning(combustionHeatBlast, combustionHeatNatural, combustionHeatFull);
            Double Lalpha = GetAirConsumptionForBurningFuel(isLongFlameTorch, L0);
            Double H20 = (0.0006 * combustionHeatFull - 1.0069) * (L0 + deltaV) / (Lalpha + deltaV);
            Double CO2 = (-0.0008 * combustionHeatFull + 33.062) * (L0 + deltaV) / (Lalpha + deltaV);

            Double tmpForEH2O = 0.01 * Seff * H20;
            Double tmpForECO2 = 0.01 * Seff * CO2;

            Double Eco2 = GetEmissivityCO2(tmpForECO2, gasT);
            Double Eh2oTemp = GetEmissivityTempH2O(tmpForEH2O, gasT);
            if (Eco2 == Double.NaN || Eh2oTemp == Double.NaN)
                throw new Exception("Температура газов в сечении 2 вне допустимых значений. Допустимое значение от 600 до 1500 градусов.");

            Double b = GetKoefForEmissivityH2O(Eh2oTemp, 0.01 * H20);

            Double Eh2o = b * Eh2oTemp;

            Double Eg = Eh2o + Eco2;

            //Степень развития кладки
            Double w = (2 * h + d) / (l * n);
            //Приведенный коэффициент излучения для системы "газ-кладка-металл"
            Double Cgkm = 5.7 * 0.8 * (1 + w - Eg) / ((0.8 + Eg * (1 - 0.8)) * ((1 - Eg) / Eg) + w);

            return 1.05 * Cgkm * (Math.Pow(((gasT + 273) / 100), 4) - Math.Pow(((surfaceT + 273) / 100), 4));
        }

        /// <summary>
        /// Вычисляет суммарный тепловой поток в сечении 1 со стороны зоны 2
        /// </summary>
        /// <param name="hz"></param>
        /// <param name="hp"></param>
        /// <param name="d">Ширина печи</param>
        /// <param name="combustionHeatBlast">Теплота сгорания доменного газа</param>
        /// <param name="combustionHeatNatural">Теплота сгорания природного газа</param>
        /// <param name="combustionHeatFull">Теплота сгорания всей смеси топлива</param>
        /// <param name="isLongFlameTorch">Является ли горелка длиннопламенной</param>
        /// <returns></returns>
        public Double GetSummaryHeatFlowOfSection1ForZone2(Boolean isLongFlameTorch, Double d, Double hz, Double hp, Double combustionHeatBlast, Double combustionHeatNatural, Double combustionHeatFull, Double gasT, Double l, Double n, Double surfaceT)
        {
            //Высота рабочего пространства над заготовками
            Double h = hp - hz;

            //Эффективная толщина излучающего слоя печных газов
            Double Seff = 1.8 * d * h / (d + h);
            //Содержание H20 и СО2 в продуктах горения природно-доменной смеси
            Double deltaV = GetDiffBetweenNaturalAndAir(combustionHeatBlast, combustionHeatNatural, combustionHeatFull);
            Double L0 = GetAirAmountForFullBurning(combustionHeatBlast, combustionHeatNatural, combustionHeatFull);
            Double Lalpha = GetAirConsumptionForBurningFuel(isLongFlameTorch, L0);
            Double H20 = (0.0006 * combustionHeatFull - 1.0069) * (L0 + deltaV) / (Lalpha + deltaV);
            Double CO2 = (-0.0008 * combustionHeatFull + 33.062) * (L0 + deltaV) / (Lalpha + deltaV);

            Double tmpForEH2O = 0.01 * Seff * H20;
            Double tmpForECO2 = 0.01 * Seff * CO2;

            Double Eco2 = GetEmissivityCO2(tmpForECO2, gasT);
            Double Eh2oTemp = GetEmissivityTempH2O(tmpForEH2O, gasT);
            if (Eco2 == Double.NaN || Eh2oTemp == Double.NaN)
                throw new Exception("Температура газов в сечении 2 вне допустимых значений. Допустимое значение от 600 до 1500 градусов.");

            Double b = GetKoefForEmissivityH2O(Eh2oTemp, 0.01 * H20);

            Double Eh20 = b * Eh2oTemp;

            //Степень черноты пламени            
            Double Epl = 1.2 * (Eh20 + Eco2);

            //Степень развития кладки
            Double w = (2 * h + d) / (l * n);

            //Приведенный коэффициент излучения для системы "газ-кладка-металл"
            Double Cgkm = 5.7 * 0.8 * (1 + w - Epl) / ((0.8 + Epl * (1 - 0.8)) * ((1 - Epl) / Epl) + w);

            return 1.1 * Cgkm * (Math.Pow(((gasT + 273) / 100), 4) - Math.Pow(((surfaceT + 273) / 100), 4));
        }

        /// <summary>
        /// Вычисляет средний тепловой поток в зоне 1
        /// </summary>
        /// <param name="q1">Тепловой поток в сечении 1</param>
        /// <param name="q0">Тепловой поток в сечении 0</param>
        /// <returns></returns>
        public Double GetAverageHeatFlowInZone1(Double q1, Double q0)
        {
            return (q1 - q0) / Math.Log(q1 / q0);
        }

        /// <summary>
        /// Вычисляет средний тепловой поток в зоне 2
        /// </summary>
        /// <param name="q1">Тепловой поток в сечении 1</param>
        /// <param name="q2">Тепловой поток в сечении 2</param>
        /// <returns></returns>
        public Double GetAverageHeatFlowInZone2(Double q1, Double q2)
        {
            return (q1 - q2) / Math.Log(q1 / q2);
        }

        /// <summary>
        /// Вычисляет время нагрева заготовки в зоне 1
        /// </summary>
        /// <param name="h">Высота заготовки</param>
        /// <param name="t0">Температура заготовки в сечении 0</param>
        /// <param name="t1">Температура заготовки в сечении 1</param>
        /// <param name="q">Средний тепловой поток в зоне 1</param>
        /// <param name="steelType">Тип стали: 0 - литая кипящая, 1 - литая спокойная, 2 - кованая или прокатанная</param>
        /// <returns></returns>
        public Double GetHeatingTimeinZone1(Double h, Double t0, Double t1, Double q, SteelTypeForDensity steelType, SteelTypeProperty steel)
        {
            //Верхняя прогреваемая толщина заготовки
            Double Sv = (-0.92 * 0.5 + 1.052) * h; //0.5 - тепловая мощность низа печи
            Double Cm = GetHeatCapacityBySteelType(t1, steel);
            //Плотность нагреваемой стали
            Double p;
            switch (steelType)
            {
                case SteelTypeForDensity.CastBoiling: p = 6600; break;
                case SteelTypeForDensity.CastQuiet: p = 7600; break;
                case SteelTypeForDensity.ForgedLaminated: p = 7850; break;
                default: throw new InvalidOperationException("Неверно задан тип стали");
            }
            return Sv * Cm * p * (t1 - t0) / q;
        }

        /// <summary>
        /// Вычисляет время нагрева заготовки в зоне 2
        /// </summary>
        /// <param name="S">Высота заготовки</param>
        /// <param name="t1">Температура заготовки в сечении 1</param>
        /// <param name="t2">Температура заготовки в сечении 2</param>
        /// <param name="q">Средний тепловой поток в зоне 2</param>
        /// <param name="steelType">Тип стали: 0 - литая кипящая, 1 - литая спокойная, 2 - кованая или прокатанная</param>
        /// <returns></returns>
        public Double GetHeatingTimeinZone2(Double S, Double t1, Double t2, Double q, SteelTypeForDensity steelType, SteelTypeProperty steel)
        {
            //Верхняя прогреваемая толщина заготовки
            Double Sv = (-0.92 * 0.5 + 1.052) * S; //0.5 - тепловая мощность низа печи
            Double Cm2 = GetHeatCapacityBySteelType(t2, steel);
            Double Cm1 = GetHeatCapacityBySteelType(t1, steel);
            Double Cm = (Cm2 * t2 - Cm1 * t1) / (t2 - t1);
            //Плотность нагреваемой стали
            Double p;
            switch (steelType)
            {
                case SteelTypeForDensity.CastBoiling: p = 6600; break;
                case SteelTypeForDensity.CastQuiet: p = 7600; break;
                case SteelTypeForDensity.ForgedLaminated: p = 7850; break;
                default: throw new InvalidOperationException("Неверно задан тип стали");
            }
            return Sv * Cm * p * (t2 - t1) / q;
        }

        /// <summary>
        /// Вычисляет скорость нагрева заготовки. Генерит исключение, если скорость нагрева заготовки отличается от рекомендуемой
        /// </summary>
        /// <param name="t1">Температура заготовки в сечении 1</param>
        /// <param name="t0">Температура заготовки в сечении 0</param>
        /// <param name="time">Время нагрева заготовки в зоне 1</param>
        /// <param name="S">Толщина заготовки</param>
        /// <returns></returns>
        public Double GetIncreasingSpeedOfMetalTemperature(Double t1, Double t0, Double time, Double S)
        {
            Double result = 60 * (t1 - t0) / time;
            if (S <= 200)
            {
                if (result < 10 || result > 11)
                    throw new ArgumentOutOfRangeException("Скорость нагрева заготовки равна " + result + " град/мин, что отличается от рекомендуемой скорости 10-11 град/мин. Попробуйте изменить температуру газов или температуру поверхности заготовки");
            }
            else if (S > 200 && S < 300)
            {
                if (result < 6.5 || result > 8)
                    throw new ArgumentOutOfRangeException("Скорость нагрева заготовки равна " + result + " град/мин, что отличается от рекомендуемой скорости 6.5-8 град/мин. Попробуйте изменить температуру газов или температуру поверхности заготовки");
            }
            else if (S >= 300 && S < 400)
            {
                if (result < 4 || result > 5.5)
                    throw new ArgumentOutOfRangeException("Скорость нагрева заготовки равна " + result + " град/мин, что отличается от рекомендуемой скорости 4-5.5 град/мин. Попробуйте изменить температуру газов или температуру поверхности заготовки");
            }
            return 60 * (t1 - t0) / time;
        }

        /// <summary>
        /// Вычисляет температуру заготовки в сечении 3
        /// </summary>
        /// <param name="t3v">Температура верхней части слитка в сечении 3</param>
        /// <param name="t3n">Температура нижней части слитка в сечении 3</param>
        /// <returns></returns>
        public Double GetMetalTemperatureInSection3(Double t3v, Double t3n)
        {
            return t3v - 2 * (t3v - t3n) / 3;
        }
        /// <summary>
        /// Вычисляет продолжительность выдержки в сечении 3
        /// </summary>
        /// <param name="t3m">Температура массы заготовки в сечении 3</param>
        /// <param name="t2m">Температура массы заготовки в сечении 2</param>
        /// <param name="t3pv">Температура верхней поверхности заготовки в сечении 3</param>
        /// <param name="t3pn">Температура нижней поверхности заготовки в сечении 3</param>
        /// <param name="t2p">Температура поверхности заготовки в сечении 2</param>
        /// <param name="t2c">Минимальная температура заготовки перед заходом на сплошной под (в сечении 2)</param>
        /// <param name="S">Толщина заготовки</param>
        /// <param name="steelType">Тип стали: 0 - литая кипящая, 1 - литая спокойная, 2 - кованая или прокатанная</param>
        /// <returns></returns>
        public Double GetTimeForZone3(Double t3m, Double t2m, Double t3pv, Double t3pn, Double t2p, Double t2c, Double S, SteelTypeForDensity steelType, SteelTypeProperty steel)
        {
            //ПОКА ТОЛЬКО МАЛОУГЛЕРОДИСТАЯ СТАЛЬ
            //Средняя теплоемкость заготовки
            Double Cm3 = GetHeatCapacityBySteelType(t3m, steel);
            Double Cm2 = GetHeatCapacityBySteelType(t2m, steel);
            Double Cm = (Cm3 * t3m - Cm2 * t2m) / (t3m - t2m);
            //Средняя теплопроводность заготовки
            Double lambda = (GetThermalConductivityBySteelType(t3m - 100, steel) + GetThermalConductivityBySteelType(t2m - 100, steel)) / 2;
            //Средняя температуропроводность
            Int32 p;
            switch (steelType)
            {
                case SteelTypeForDensity.CastBoiling: p = 6600; break;
                case SteelTypeForDensity.CastQuiet: p = 7600; break;
                case SteelTypeForDensity.ForgedLaminated: p = 7850; break;
                default: throw new InvalidOperationException("Неверно задан тип стали");
            }
            Double Am = lambda / Cm / p;
            //Прогреваемая сверху толщина заготовки
            Double Sv = (-0.92 * 0.5 + 1.052) * S;
            Double Sn = S - Sv;
            return (Math.Pow(S, 2) / 2.467 / Am) * Math.Log(1.621 * (Math.Pow(S / Sv, 2) * 0.637 - S * Sn / Math.Pow(Sv, 2)) * ((t2p - t2c) / (t3pv - t3pn)));
        }

        /// <summary>
        /// Вычисляет длину активной части пода печи
        /// </summary>
        /// <param name="fullTime">Полное время нагрева изделий</param>
        /// <param name="furnanceProductivity">Производительность печи, тонн в час</param>
        /// <param name="S">Толщина заготовки</param>
        /// <param name="l">Длина заготовки</param>
        /// <param name="n">Число рядов заготовок по ширине печи</param>
        /// <param name="steelType">Тип стали: 0 - литая кипящая, 1 - литая спокойная, 2 - кованая или прокатанная</param>
        /// <returns></returns>
        public Double GetLengthActivePartFurnance(Double fullTime, Double furnanceProductivity, Double S, Double l, Double n, SteelTypeForDensity steelType)
        {
            Int32 p;
            switch (steelType)
            {
                case SteelTypeForDensity.CastBoiling: p = 6600; break;
                case SteelTypeForDensity.CastQuiet: p = 7600; break;
                case SteelTypeForDensity.ForgedLaminated: p = 7850; break;
                default: throw new InvalidOperationException("Неверно задан тип стали");
            }
            return (furnanceProductivity * 1000 / 3600) * fullTime / S / l / p / n / 0.92; //0.92 - коэффициент заполнения
        }

        /// <summary>
        /// Вычисляет длину отдельной зоны печи
        /// </summary>
        /// <param name="Lap">Длина активной части пода печи</param>
        /// <param name="zoneTime">Время нагрева заготовки в зоне</param>
        /// <param name="fullTime">Полное время нагрева заготовки</param>
        /// <returns></returns>
        public Double GetLengthInZone(Double Lap, Double zoneTime, Double fullTime)
        {
            return Lap * zoneTime / fullTime;
        }

        #region Additional

        private Double GetAirAmountForFullBurning(Double combustionHeatBlast, Double combustionHeatNatural, Double combustionHeatFull)
        {
            //Теоретически необходимое количество воздуха для полного сжигания 1 кубического метра доменного газа
            Double airAmountForBurningBlast = 0.001 * 0.191 * combustionHeatBlast;
            //Теоретически необходимое количество воздуха для полного сжигания 1 кубического метра природного газа
            Double airAmountForBurningNatural = 0.001 * 0.264 * combustionHeatNatural - (combustionHeatNatural > 35800 ? 0 : 0.05);
            //Доля доменного газа в смеси
            Double portionBlast = (combustionHeatNatural - combustionHeatFull) / (combustionHeatNatural - combustionHeatBlast);
            //Доля природного газа в смеси
            Double portionNatural = 1 - portionBlast;

            return portionBlast * airAmountForBurningBlast + portionNatural * airAmountForBurningNatural;
        }

        private Double GetDiffBetweenNaturalAndAir(Double combustionHeatBlast, Double combustionHeatNatural, Double combustionHeatFull)
        {
            Double diffBetweenBlastAndAir = 0.97 - 0.001 * 0.031 * combustionHeatBlast;
            //Разность между объемом продуктов сгорания 1 кубического метра природного газа и израсходованного на горение воздуха
            Double diffBetweenNaturalAndAir = (combustionHeatNatural > 35800 ? 0.38 : 1) - 0.001 * (combustionHeatNatural > 35800 ? -0.018 : 0) * combustionHeatNatural;
            //Доля доменного газа в смеси
            Double portionBlast = (combustionHeatNatural - combustionHeatFull) / (combustionHeatNatural - combustionHeatBlast);
            //Доля природного газа в смеси
            Double portionNatural = 1 - portionBlast;

            return portionBlast * diffBetweenBlastAndAir + portionNatural * diffBetweenNaturalAndAir;
        }

        private Double GetAirConsumptionForBurningFuel(Boolean isLongFlameTorch, Double amountAirForFullBurning)
        {
            return amountAirForFullBurning * (isLongFlameTorch ? 1.2 : 1.07);
        }

        private Double GetKoefForEmissivityH2O(Double emissWithoutb, Double pressure)
        {
            if (pressure <= 0.08)
                return -0.0511 * Math.Pow(emissWithoutb, 3) + 0.1444 * Math.Pow(emissWithoutb, 2) - 0.1213 * emissWithoutb + 1.0688;
            else if (pressure > 0.08 && pressure <= 0.09)
                return GetValueBetween(0.08, 0.09, -0.0511 * Math.Pow(emissWithoutb, 3) + 0.1444 * Math.Pow(emissWithoutb, 2) - 0.1213 * emissWithoutb + 1.0688,
                    -0.0537 * Math.Pow(emissWithoutb, 3) + 0.1454 * Math.Pow(emissWithoutb, 2) - 0.1133 * emissWithoutb + 1.0732, pressure);
            else if (pressure > 0.09 && pressure <= 0.1)
                return GetValueBetween(0.09, 0.1, -0.0537 * Math.Pow(emissWithoutb, 3) + 0.1454 * Math.Pow(emissWithoutb, 2) - 0.1133 * emissWithoutb + 1.0732,
                    -0.0588 * Math.Pow(emissWithoutb, 3) + 0.1552 * Math.Pow(emissWithoutb, 2) - 0.1227 * emissWithoutb + 1.0826, pressure);
            else if (pressure > 0.1 && pressure <= 0.125)
                return GetValueBetween(0.01, 0.125, -0.0588 * Math.Pow(emissWithoutb, 3) + 0.1552 * Math.Pow(emissWithoutb, 2) - 0.1227 * emissWithoutb + 1.0826,
                    1.0634 * Math.Pow(emissWithoutb, -0.009), pressure);
            else if (pressure > 0.125 && pressure <= 0.15)
                return GetValueBetween(0.125, 0.15, 1.0634 * Math.Pow(emissWithoutb, -0.009), 1.0701 * Math.Pow(emissWithoutb, -0.012), pressure);
            else if (pressure > 0.15 && pressure <= 0.175)
                return GetValueBetween(0.15, 0.175, 1.0701 * Math.Pow(emissWithoutb, -0.012), 1.076 * Math.Pow(emissWithoutb, -0.015), pressure);
            else if (pressure > 0.175 && pressure <= 0.2)
                return GetValueBetween(0.175, 0.2, 1.076 * Math.Pow(emissWithoutb, -0.015), 1.0858 * Math.Pow(emissWithoutb, -0.016), pressure);
            else if (pressure > 0.2 && pressure <= 0.25)
                return GetValueBetween(0.2, 0.25, 1.0858 * Math.Pow(emissWithoutb, -0.016), 1.1076 * Math.Pow(emissWithoutb, -0.015), pressure);
            else
                return 1.1076 * Math.Pow(emissWithoutb, -0.015);
        }

        private Double GetValueBetween(Double bottom, Double top, Double bottomVal, Double topVal, Double val)
        {
            Double prop = val / (top - bottom);
            return bottomVal + ((topVal - bottomVal) * prop);
        }

        private Double GetEmissivityCO2(Double val, Double gasT)
        {
            if (gasT >= 600 && gasT < 700)
                return GetValueBetween(0.2139 * Math.Pow(val, 0.2571), 0.2118 * Math.Pow(val, 0.2713), val);
            else if (gasT >= 700 && gasT < 800)
                return GetValueBetween(0.2118 * Math.Pow(val, 0.2713), 0.2066 * Math.Pow(val, 0.2769), val);
            else if (gasT >= 800 && gasT < 900)
                return GetValueBetween(0.2066 * Math.Pow(val, 0.2769), 0.2051 * Math.Pow(val, 0.2893), val);
            else if (gasT >= 900 && gasT < 1000)
                return GetValueBetween(0.2051 * Math.Pow(val, 0.2893), 0.2015 * Math.Pow(val, 0.3055), val);
            else if (gasT >= 1000 && gasT < 1100)
                return GetValueBetween(0.2015 * Math.Pow(val, 0.3055), 0.194 * Math.Pow(val, 0.3154), val);
            else if (gasT >= 1100 && gasT < 1200)
                return GetValueBetween(0.194 * Math.Pow(val, 0.3154), 0.1851 * Math.Pow(val, 0.3255), val);
            else if (gasT >= 1200 && gasT < 1300)
                return GetValueBetween(0.1851 * Math.Pow(val, 0.3255), 0.1751 * Math.Pow(val, 0.3358), val);
            else if (gasT >= 1300 && gasT < 1400)
                return GetValueBetween(0.1751 * Math.Pow(val, 0.3358), 0.1654 * Math.Pow(val, 0.3403), val);
            else if (gasT >= 1400 && gasT <= 1500)
                return GetValueBetween(0.1654 * Math.Pow(val, 0.3403), 0.1556 * Math.Pow(val, 0.3522), val);

            return Double.NaN;
        }

        private Double GetEmissivityTempH2O(Double val, Double gasT)
        {
            if (gasT >= 600 && gasT < 700)
                return GetValueBetween(0.0927 * Math.Log(val) + 0.3609, 0.0893 * Math.Log(val) + 0.3448, val);
            else if (gasT >= 700 && gasT < 800)
                return GetValueBetween(0.0893 * Math.Log(val) + 0.3448, 0.0866 * Math.Log(val) + 0.3293, val);
            else if (gasT >= 800 && gasT < 900)
                return GetValueBetween(0.0866 * Math.Log(val) + 0.3293, 0.0829 * Math.Log(val) + 0.3101, val);
            else if (gasT >= 900 && gasT < 1000)
                return GetValueBetween(0.0829 * Math.Log(val) + 0.3101, 0.0792 * Math.Log(val) + 0.2921, val);
            else if (gasT >= 1000 && gasT < 1100)
                return GetValueBetween(0.0792 * Math.Log(val) + 0.2921, 0.0759 * Math.Log(val) + 0.2757, val);
            else if (gasT >= 1100 && gasT < 1200)
                return GetValueBetween(0.0759 * Math.Log(val) + 0.2757, 0.072 * Math.Log(val) + 0.2586, val);
            else if (gasT >= 1200 && gasT < 1300)
                return GetValueBetween(0.072 * Math.Log(val) + 0.2586, 0.0696 * Math.Log(val) + 0.2451, val);
            else if (gasT >= 1300 && gasT < 1400)
                return GetValueBetween(0.0696 * Math.Log(val) + 0.2451, 0.0653 * Math.Log(val) + 0.2287, val);
            else if (gasT >= 1400 && gasT <= 1500)
                return GetValueBetween(0.0653 * Math.Log(val) + 0.2287, 0.0625 * Math.Log(val) + 0.2157, val);

            return Double.NaN;
        }

        /// <summary>
        /// Чтобы получить значение точнее (разница нижней и верхней границы должна быть равна 100)
        /// </summary>
        /// <param name="bottomVal">Нижняя граница</param>
        /// <param name="topVal">Верхняя граница</param>
        /// <param name="val">Должно быть трехзначным</param>
        /// <returns></returns>
        private Double GetValueBetween(Double bottomVal, Double topVal, Double val)
        {
            Double prop = Double.Parse(val.ToString().Substring(1, val.ToString().Length - 1)) / 100;
            return bottomVal - (bottomVal - topVal) * prop;
        }

        private Double GetThermalConductivityBySteelType(Double temp, SteelTypeProperty steelType)
        {
            switch (steelType)
            {
                case SteelTypeProperty.LowCarbon: return 268.55 * Math.Pow((temp), -0.315);
                case SteelTypeProperty.MiddleCarbon: return 0.00000005 * Math.Pow(temp, 3) - 0.00007 * Math.Pow(temp, 2) + 0.0036 * temp + 49.748;
                case SteelTypeProperty.HighCarbon: return 0.00003 * Math.Pow(temp, 2) - 0.0578 * temp + 54.196;
                case SteelTypeProperty.Stainless: return 0.000000008 * Math.Pow(temp, 3) - 0.000009 * Math.Pow(temp, 2) - 0.0007 * temp + 28.189;
                case SteelTypeProperty.Heatproof: return 0.0161 * temp + 13.269;
                default: return Double.NaN;
            }
        }

        private Double GetHeatCapacityBySteelType(Double temp, SteelTypeProperty steelType)
        {
            if (steelType == SteelTypeProperty.HighCarbon || steelType == SteelTypeProperty.MiddleCarbon || steelType == SteelTypeProperty.LowCarbon)
                return temp < 800 ? (0.2679 * temp + 444.71) : (-0.0286 * temp + 719);
            else if (steelType == SteelTypeProperty.Stainless)
                return -0.0002 * Math.Pow(temp, 2) + 0.4275 * temp + 405.57;
            else if (steelType == SteelTypeProperty.Heatproof)
                return 0.0808 * temp + 500.19;

            return Double.NaN;
        }

        #endregion
    }
}
