using FurnanceCalculator.Enums;
using FurnanceCalculator.Models;
using FurnanceCalculator.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FurnanceCalculator.Controllers
{
    [Authorize]
    public class CalculatorController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var query = User.IsInRole("Administrator") ?
                db.Variants.AsQueryable() :
                db.Variants.Where(v => v.UserId == userId);

            var variants = query.ToList();
            var model = new VariantIndexView()
            {
                Variants = variants.Select(v => new VariantView()
                {
                    Id = v.Id,
                    Name = v.Name,
                    Description = v.Description,
                    ResultIsExists = v.ResultData != null
                }).ToList()
            };

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VariantView model)
        {
            if (ModelState.IsValid)
            {
                var newVariant = new Variant()
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Description = model.Description,
                    UserId = User.Identity.GetUserId()
                };

                db.Variants.Add(newVariant);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Edit(Guid id)
        {
            var variant = db.Variants.Find(id);
            if (variant == null) return HttpNotFound();

            var model = new VariantView()
            {
                Id = variant.Id,
                Name = variant.Name,
                Description = variant.Description
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VariantView model)
        {
            if (ModelState.IsValid)
            {
                var variant = db.Variants.Find(model.Id);
                if (variant == null) return HttpNotFound();

                variant.Name = model.Name;
                variant.Description = model.Description;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Delete(Guid id)
        {
            var variant = db.Variants.Find(id);
            if (variant == null) return HttpNotFound();

            if (variant.InputData != null) db.InputData.Remove(variant.InputData);
            if (variant.ResultData != null) db.ResultData.Remove(variant.ResultData);

            db.Variants.Remove(variant);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult BarParameters(Guid id)
        {
            var variant = db.Variants.Find(id);
            if (variant == null) return HttpNotFound();

            var model = new BarParametersView();

            if (variant.IsBarParametersExist)
            {
                model.Id = Guid.NewGuid();
                model.BarHeight = variant.InputData.BarHeight;
                model.BarLength = variant.InputData.BarLength;
                model.BarNumber = variant.InputData.BarNumber;
                model.BarThickness = variant.InputData.BarThickness;
                model.EndBottomSteelTemperature = variant.InputData.EndBottomSteelTemperature;
                model.EndTopSteelTemperature = variant.InputData.EndTopSteelTemperature;
                model.StartSteelTemperature = variant.InputData.StartSteelTemperature;
                model.SteelTypeForDensity = variant.InputData.SteelTypeForDensity;
                model.SteelTypeProperty = variant.InputData.SteelTypeProperty;
                model.TopSteelTemperatureSector1 = variant.InputData.TopSteelTemperatureSector1;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BarParameters(BarParametersView model)
        {
            if (ModelState.IsValid)
            {
                var variant = db.Variants.Find(model.Id);
                if (variant == null) return HttpNotFound();

                if (variant.InputData == null) variant.InputData = new InputData();

                variant.InputData.BarHeight = model.BarHeight.Value;
                variant.InputData.BarLength = model.BarLength.Value;
                variant.InputData.BarNumber = model.BarNumber.Value;
                variant.InputData.BarThickness = model.BarThickness.Value;
                variant.InputData.EndBottomSteelTemperature = model.EndBottomSteelTemperature.Value;
                variant.InputData.EndTopSteelTemperature = model.EndTopSteelTemperature.Value;
                variant.InputData.StartSteelTemperature = model.StartSteelTemperature.Value;
                variant.InputData.SteelTypeForDensity = model.SteelTypeForDensity;
                variant.InputData.SteelTypeProperty = model.SteelTypeProperty;
                variant.InputData.TopSteelTemperatureSector1 = model.TopSteelTemperatureSector1.Value;
                variant.IsBarParametersExist = true;

                db.SaveChanges();
                return RedirectToAction("FurnanceParameters", new { id = model.Id });
            }

            return View(model);
        }

        public ActionResult FurnanceParameters(Guid id)
        {
            var variant = db.Variants.Find(id);
            if (variant == null) return HttpNotFound();

            var model = new FurnanceParametersView();

            if (variant.IsFurnanceParametersExist)
            {
                model.Id = variant.InputData.Id;
                model.FurnanceHeightZone1 = variant.InputData.FurnanceHeightZone1;
                model.FurnanceProductivity = variant.InputData.FurnanceProductivity;
                model.FurnanceWidth = variant.InputData.FurnanceWidth;
                model.TorchType = variant.InputData.TorchType;
                model.WorkHeightSector0 = variant.InputData.WorkHeightSector0;
                model.WorkHeightSector1 = variant.InputData.WorkHeightSector1;
                model.WorkHeightSector2 = variant.InputData.WorkHeightSector2;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FurnanceParameters(FurnanceParametersView model)
        {
            if (ModelState.IsValid)
            {
                var variant = db.Variants.Find(model.Id);
                if (variant == null) return HttpNotFound();

                if (variant.InputData == null) variant.InputData = new InputData();

                variant.InputData.FurnanceHeightZone1 = model.FurnanceHeightZone1.Value;
                variant.InputData.FurnanceProductivity = model.FurnanceProductivity.Value;
                variant.InputData.FurnanceWidth = model.FurnanceWidth.Value;
                variant.InputData.TorchType = model.TorchType;
                variant.InputData.WorkHeightSector0 = model.WorkHeightSector0.Value;
                variant.InputData.WorkHeightSector1 = model.WorkHeightSector1.Value;
                variant.InputData.WorkHeightSector2 = model.WorkHeightSector2.Value;
                variant.IsFurnanceParametersExist = true;

                db.SaveChanges();
                return RedirectToAction("EnvironmentParameters", new { id = model.Id });
            }

            return View(model);
        }

        public ActionResult EnvironmentParameters(Guid id)
        {
            var variant = db.Variants.Find(id);
            if (variant == null) return HttpNotFound();

            var model = new EnvironmentParametersView();

            if (variant.IsEnvironmentParametersExist)
            {
                model.Id = variant.InputData.Id;
                model.AirTemperature = variant.InputData.AirTemperature;
                model.GasTemperatureSector0 = variant.InputData.GasTemperatureSector0;
                model.GasTemperatureSector1 = variant.InputData.GasTemperatureSector1;
                model.GasTemperatureSector2 = variant.InputData.GasTemperatureSector2;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EnvironmentParameters(EnvironmentParametersView model)
        {
            if (ModelState.IsValid)
            {
                var variant = db.Variants.Find(model.Id);
                if (variant == null) return HttpNotFound();

                if (variant.InputData == null) variant.InputData = new InputData();

                variant.InputData.Id = model.Id;
                variant.InputData.AirTemperature = model.AirTemperature.Value;
                variant.InputData.GasTemperatureSector0 = model.GasTemperatureSector0.Value;
                variant.InputData.GasTemperatureSector1 = model.GasTemperatureSector1.Value;
                variant.InputData.GasTemperatureSector2 = model.GasTemperatureSector2.Value;
                variant.IsEnvironmentParametersExist = true;

                db.SaveChanges();
                return RedirectToAction("FuelParameters", new { id = model.Id });
            }

            return View(model);
        }

        public ActionResult FuelParameters(Guid id)
        {
            var variant = db.Variants.Find(id);
            if (variant == null) return HttpNotFound();

            var model = new FuelParametersView();

            if (variant.IsFuelParametersExist)
            {
                model.Id = variant.Id;
                model.HeatBlast = variant.InputData.HeatBlast;
                model.HeatFull = variant.InputData.HeatFull;
                model.HeatNatural = variant.InputData.HeatNatural;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FuelParameters(FuelParametersView model)
        {
            if (ModelState.IsValid)
            {
                var variant = db.Variants.Find(model.Id);
                if (variant == null) return HttpNotFound();

                if (variant.InputData == null) variant.InputData = new InputData();

                variant.InputData.Id = model.Id;
                variant.InputData.HeatBlast = model.HeatBlast.Value;
                variant.InputData.HeatFull = model.HeatFull.Value;
                variant.InputData.HeatNatural = model.HeatNatural.Value;
                variant.IsFuelParametersExist = true;

                if (variant.ResultData != null) Session["old_result"] = ResultView.CopyFrom(variant.ResultData);

                try
                {
                    Calculate(variant);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", "Не удалось выполнить расчет");
                    ModelState.AddModelError("", exc.Message);
                    return View(model);
                }

                db.SaveChanges();
                return RedirectToAction("Results", new { id = model.Id, checkOldResults = true });
            }

            return View(model);
        }

        public ActionResult Results(Guid id, bool checkOldResults = false)
        {
            var variant = db.Variants.Find(id);
            if (variant == null) return HttpNotFound();
            if (variant.ResultData == null) return HttpNotFound();

            var model = new ResultView()
            {
                Id = variant.Id,
                Name = variant.Name,
                AverageHeatFlowZone1 = variant.ResultData.AverageHeatFlowZone1,
                AverageHeatFlowZone2 = variant.ResultData.AverageHeatFlowZone2,
                BarTemperatureSection1 = variant.ResultData.BarTemperatureSection1,
                BarTemperatureSection2 = variant.ResultData.BarTemperatureSection2,
                BarTemperatureSection3 = variant.ResultData.BarTemperatureSection3,
                ConsumptionTemperature = variant.ResultData.ConsumptionTemperature,
                HeatingTimeFull = variant.ResultData.HeatingTimeFull,
                HeatingTimeZone1 = variant.ResultData.HeatingTimeZone1,
                HeatingTimeZone2 = variant.ResultData.HeatingTimeZone2,
                HeatingTimeZone3 = variant.ResultData.HeatingTimeZone3,
                LengthFull = variant.ResultData.LengthFull,
                LengthZone1 = variant.ResultData.LengthZone1,
                LengthZone2 = variant.ResultData.LengthZone2,
                LengthZone3 = variant.ResultData.LengthZone3,
                SummaryHeatFlowSector0 = variant.ResultData.SummaryHeatFlowSector0,
                SummaryHeatFlowSector1Zone1 = variant.ResultData.SummaryHeatFlowSector1Zone1,
                SummaryHeatFlowSector1Zone2 = variant.ResultData.SummaryHeatFlowSector1Zone2,
                SummaryHeatFlowSector2 = variant.ResultData.SummaryHeatFlowSector2
            };

            #region styles
            if (checkOldResults)
            {
                model.StyleHeatingTimeZone1 = (Session["old_result"] as ResultView).HeatingTimeZone1 == model.HeatingTimeZone1 ?
                "style=\"background-color: #eeeeee\"" :
                "style=\"background-color: #f5c9aa\"";
                model.StyleHeatingTimeZone2 = (Session["old_result"] as ResultView).HeatingTimeZone2 == model.HeatingTimeZone2 ?
                "style=\"background-color: #eeeeee\"" :
                "style=\"background-color: #f5c9aa\"";
                model.StyleHeatingTimeZone3 = (Session["old_result"] as ResultView).HeatingTimeZone3 == model.HeatingTimeZone3 ?
                "style=\"background-color: #eeeeee\"" :
                "style=\"background-color: #f5c9aa\"";
                model.StyleHeatingTimeFull = (Session["old_result"] as ResultView).HeatingTimeFull == model.HeatingTimeFull ?
                "style=\"background-color: #eeeeee\"" :
                "style=\"background-color: #f5c9aa\"";

                model.StyleLengthZone1 = (Session["old_result"] as ResultView).LengthZone1 == model.LengthZone1 ?
                "style=\"background-color: #eeeeee\"" :
                "style=\"background-color: #f5c9aa\"";
                model.StyleLengthZone2 = (Session["old_result"] as ResultView).LengthZone2 == model.LengthZone2 ?
                "style=\"background-color: #eeeeee\"" :
                "style=\"background-color: #f5c9aa\"";
                model.StyleLengthZone3 = (Session["old_result"] as ResultView).LengthZone3 == model.LengthZone3 ?
                "style=\"background-color: #eeeeee\"" :
                "style=\"background-color: #f5c9aa\"";
                model.StyleLengthFull = (Session["old_result"] as ResultView).LengthFull == model.LengthFull ?
                "style=\"background-color: #eeeeee\"" :
                "style=\"background-color: #f5c9aa\"";

                model.StyleConsumptionTemperature = (Session["old_result"] as ResultView).ConsumptionTemperature == model.ConsumptionTemperature ?
                "style=\"background-color: #eeeeee\"" :
                "style=\"background-color: #f5c9aa\"";

                model.StyleAverageHeatFlowZone1 = (Session["old_result"] as ResultView).AverageHeatFlowZone1 == model.AverageHeatFlowZone1 ?
               "style=\"background-color: #eeeeee\"" :
               "style=\"background-color: #f5c9aa\"";
                model.StyleAverageHeatFlowZone2 = (Session["old_result"] as ResultView).AverageHeatFlowZone2 == model.AverageHeatFlowZone2 ?
               "style=\"background-color: #eeeeee\"" :
               "style=\"background-color: #f5c9aa\"";

                model.StyleBarTemperatureSection1 = (Session["old_result"] as ResultView).BarTemperatureSection1 == model.BarTemperatureSection1 ?
               "style=\"background-color: #eeeeee\"" :
               "style=\"background-color: #f5c9aa\"";
                model.StyleBarTemperatureSection2 = (Session["old_result"] as ResultView).BarTemperatureSection2 == model.BarTemperatureSection2 ?
               "style=\"background-color: #eeeeee\"" :
               "style=\"background-color: #f5c9aa\"";
                model.StyleBarTemperatureSection3 = (Session["old_result"] as ResultView).BarTemperatureSection3 == model.BarTemperatureSection3 ?
               "style=\"background-color: #eeeeee\"" :
               "style=\"background-color: #f5c9aa\"";

                model.StyleSummaryHeatFlowSector0 = (Session["old_result"] as ResultView).SummaryHeatFlowSector0 == model.SummaryHeatFlowSector0 ?
               "style=\"background-color: #eeeeee\"" :
               "style=\"background-color: #f5c9aa\"";
                model.StyleSummaryHeatFlowSector1Zone1 = (Session["old_result"] as ResultView).SummaryHeatFlowSector1Zone1 == model.SummaryHeatFlowSector1Zone1 ?
               "style=\"background-color: #eeeeee\"" :
               "style=\"background-color: #f5c9aa\"";
                model.StyleSummaryHeatFlowSector1Zone2 = (Session["old_result"] as ResultView).SummaryHeatFlowSector1Zone2 == model.SummaryHeatFlowSector1Zone2 ?
               "style=\"background-color: #eeeeee\"" :
               "style=\"background-color: #f5c9aa\"";
                model.StyleSummaryHeatFlowSector2 = (Session["old_result"] as ResultView).SummaryHeatFlowSector2 == model.SummaryHeatFlowSector2 ?
               "style=\"background-color: #eeeeee\"" :
               "style=\"background-color: #f5c9aa\"";
            }
            #endregion

            return View(model);
        }

        public ActionResult GetPdf(Guid id)
        {
            var variant = db.Variants.Find(id);
            if (variant == null) return HttpNotFound();
            if (variant.ResultData == null) return HttpNotFound();

            var fileName = variant.Name + ".pdf";

            iTextSharp.text.Font supFont = new iTextSharp.text.Font(iTextSharp.text.Font.NORMAL, 6);
            Chunk o = new Chunk("o", supFont);
            o.SetTextRise(7);
            Chunk two = new Chunk("2", supFont);
            two.SetTextRise(7);

            BaseFont bf = BaseFont.CreateFont(Server.MapPath("~/Content/FreeSans.ttf"), "Cp1251", BaseFont.EMBEDDED);
            iTextSharp.text.Font russian12 = new iTextSharp.text.Font(bf, 12);
            iTextSharp.text.Font russian14 = new iTextSharp.text.Font(bf, 14);
            iTextSharp.text.Font russian16 = new iTextSharp.text.Font(bf, 16);

            var doc = new Document();
            MemoryStream stream = new MemoryStream();
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            var header = new Paragraph(new Phrase(variant.Name, russian16));
            header.Alignment = Element.ALIGN_CENTER;
            header.SpacingAfter = 20;
            doc.Add(header);

            var paragraph = new Paragraph(new Phrase("Исходные данные:", russian14));
            paragraph.Alignment = Element.ALIGN_CENTER;
            doc.Add(paragraph);

            PdfPTable table = GetTable();
            PdfPCell cell = GetCell("Характеристики заготовок", russian12);
            cell.BackgroundColor = new BaseColor(Color.LightBlue);
            cell.Padding = 5;
            cell.Colspan = 2;
            table.AddCell(cell);
            table.AddCell(GetCell("Высота заготовки", russian12));
            table.AddCell(GetCell(variant.InputData.BarHeight.ToString() + " м", russian12));
            table.AddCell(GetCell("Толщина заготовки", russian12));
            table.AddCell(GetCell(variant.InputData.BarThickness.ToString() + " м", russian12));
            table.AddCell(GetCell("Длина заготовки", russian12));
            table.AddCell(GetCell(variant.InputData.BarLength.ToString() + " м", russian12));
            table.AddCell(GetCell("Число рядов", russian12));
            table.AddCell(GetCell(variant.InputData.BarNumber.ToString(), russian12));
            table.AddCell(GetCell("Начальная температура", russian12));
            Phrase phrase = new Phrase(variant.InputData.StartSteelTemperature.ToString() + " ", russian12);
            phrase.Add(o);
            phrase.Add("C");
            table.AddCell(GetCell(phrase));
            table.AddCell(GetCell("Конечная температура верхней поверхности", russian12));
            phrase = new Phrase(variant.InputData.EndTopSteelTemperature.ToString() + " ", russian12);
            phrase.Add(o);
            phrase.Add("C");
            table.AddCell(GetCell(phrase));
            table.AddCell(GetCell("Конечная температура нижней поверхности", russian12));
            phrase = new Phrase(variant.InputData.EndBottomSteelTemperature.ToString() + " ", russian12);
            phrase.Add(o);
            phrase.Add("C");
            table.AddCell(GetCell(phrase));
            table.AddCell(GetCell("Температура поверхности в сечении 1", russian12));
            phrase = new Phrase(variant.InputData.TopSteelTemperatureSector1.ToString() + " ", russian12);
            phrase.Add(o);
            phrase.Add("C");
            table.AddCell(GetCell(phrase));
            table.AddCell(GetCell("Тип стали", russian12));
            table.AddCell(GetCell(EnumHelper<SteelTypeForDensity>.GetDisplayValue(variant.InputData.SteelTypeForDensity), russian12));
            table.AddCell(GetCell("Тип стали по содержанию элементов", russian12));
            table.AddCell(GetCell(EnumHelper<SteelTypeProperty>.GetDisplayValue(variant.InputData.SteelTypeProperty), russian12));
            doc.Add(table);

            table = GetTable();
            cell = GetCell("Характеристики печи", russian12);
            cell.BackgroundColor = new BaseColor(Color.LightBlue);
            cell.Padding = 5;
            cell.Colspan = 2;
            table.AddCell(cell);
            table.AddCell(GetCell("Высота рабочего пространства в зоне 1", russian12));
            table.AddCell(GetCell(variant.InputData.FurnanceHeightZone1.ToString() + " м", russian12));
            table.AddCell(GetCell("Высота рабочей поверхности в сечении 0", russian12));
            table.AddCell(GetCell(variant.InputData.WorkHeightSector0.ToString() + " м", russian12));
            table.AddCell(GetCell("Высота рабочей поверхности в сечении 1", russian12));
            table.AddCell(GetCell(variant.InputData.WorkHeightSector1.ToString() + " м", russian12));
            table.AddCell(GetCell("Высота рабочей поверхности в сечении 2", russian12));
            table.AddCell(GetCell(variant.InputData.WorkHeightSector2.ToString() + " м", russian12));
            table.AddCell(GetCell("Ширина печи", russian12));
            table.AddCell(GetCell(variant.InputData.FurnanceWidth.ToString() + " м", russian12));
            table.AddCell(GetCell("Производительность печи", russian12));
            table.AddCell(GetCell(variant.InputData.FurnanceProductivity.ToString() + " тонн/ч", russian12));
            table.AddCell(GetCell("Тип горелок", russian12));
            table.AddCell(GetCell(EnumHelper<TorchType>.GetDisplayValue((variant.InputData.TorchType)), russian12));
            doc.Add(table);

            table = GetTable();
            cell = GetCell("Характеристики среды", russian12);
            cell.BackgroundColor = new BaseColor(Color.LightBlue);
            cell.Padding = 5;
            cell.Colspan = 2;
            table.AddCell(cell);
            table.AddCell(GetCell("Температура воздуха в рекуператоре", russian12));
            phrase = new Phrase(variant.InputData.AirTemperature.ToString() + " ", russian12);
            phrase.Add(o);
            phrase.Add("C");
            table.AddCell(GetCell(phrase));
            table.AddCell(GetCell("Температура газов в сечении 0", russian12));
            phrase = new Phrase(variant.InputData.GasTemperatureSector0.ToString() + " ", russian12);
            phrase.Add(o);
            phrase.Add("C");
            table.AddCell(GetCell(phrase));
            table.AddCell(GetCell("Температура газов в сечении 1", russian12));
            phrase = new Phrase(variant.InputData.GasTemperatureSector1.ToString() + " ", russian12);
            phrase.Add(o);
            phrase.Add("C");
            table.AddCell(GetCell(phrase));
            table.AddCell(GetCell("Температура газов в сечении 2", russian12));
            phrase = new Phrase(variant.InputData.GasTemperatureSector2.ToString() + " ", russian12);
            phrase.Add(o);
            phrase.Add("C");
            table.AddCell(GetCell(phrase));
            doc.Add(table);

            table = GetTable();
            cell = GetCell("Характеристики топлива", russian12);
            cell.BackgroundColor = new BaseColor(Color.LightBlue);
            cell.Padding = 5;
            cell.Colspan = 2;
            table.AddCell(cell);
            table.AddCell(GetCell("Теплота сгорания всей смеси", russian12));
            phrase = new Phrase(variant.InputData.HeatFull.ToString() + " Вт/м", russian12);
            phrase.Add(two);
            table.AddCell(GetCell(phrase));
            table.AddCell(GetCell("Теплота сгорания природного газа", russian12));
            phrase = new Phrase(variant.InputData.HeatNatural.ToString() + " Вт/м", russian12);
            phrase.Add(two);
            table.AddCell(GetCell(phrase));
            table.AddCell(GetCell("Теплота сгорания доменного газа", russian12));
            phrase = new Phrase(variant.InputData.HeatBlast.ToString() + " Вт/м", russian12);
            phrase.Add(two);
            table.AddCell(GetCell(phrase));
            doc.Add(table);

            paragraph = new Paragraph(new Phrase("Результаты:", russian14));
            paragraph.Alignment = Element.ALIGN_CENTER;
            doc.Add(paragraph);

            table = GetTable();
            cell = GetCell("Время нагрева заготовок", russian12);
            cell.BackgroundColor = new BaseColor(Color.LightBlue);
            cell.Padding = 5;
            cell.Colspan = 2;
            table.AddCell(cell);
            table.AddCell(GetCell("Зона 1", russian12));
            table.AddCell(GetCell(variant.ResultData.HeatingTimeZone1.ToString() + " ч", russian12));
            table.AddCell(GetCell("Зона 2", russian12));
            table.AddCell(GetCell(variant.ResultData.HeatingTimeZone2.ToString() + " ч", russian12));
            table.AddCell(GetCell("Зона 3", russian12));
            table.AddCell(GetCell(variant.ResultData.HeatingTimeZone3.ToString() + " ч", russian12));
            table.AddCell(GetCell("Общее", russian12));
            table.AddCell(GetCell(variant.ResultData.HeatingTimeFull.ToString() + " ч", russian12));
            doc.Add(table);

            table = GetTable();
            cell = GetCell("Длины зон", russian12);
            cell.BackgroundColor = new BaseColor(Color.LightBlue);
            cell.Padding = 5;
            cell.Colspan = 2;
            table.AddCell(cell);
            table.AddCell(GetCell("Зона 1", russian12));
            table.AddCell(GetCell(variant.ResultData.LengthZone1.ToString() + " м", russian12));
            table.AddCell(GetCell("Зона 2", russian12));
            table.AddCell(GetCell(variant.ResultData.LengthZone2.ToString() + " м", russian12));
            table.AddCell(GetCell("Зона 3", russian12));
            table.AddCell(GetCell(variant.ResultData.LengthZone3.ToString() + " м", russian12));
            table.AddCell(GetCell("Общая", russian12));
            table.AddCell(GetCell(variant.ResultData.LengthFull.ToString() + " м", russian12));
            doc.Add(table);

            table = GetTable();
            cell = GetCell("Горение топлива", russian12);
            cell.BackgroundColor = new BaseColor(Color.LightBlue);
            cell.Padding = 5;
            cell.Colspan = 2;
            table.AddCell(cell);
            table.AddCell(GetCell("Температура газовой среды", russian12));
            phrase = new Phrase(variant.ResultData.ConsumptionTemperature.ToString() + " ", russian12);
            phrase.Add(o);
            phrase.Add("C");
            table.AddCell(GetCell(phrase));
            doc.Add(table);

            table = GetTable();
            cell = GetCell("Средний тепловой поток", russian12);
            cell.BackgroundColor = new BaseColor(Color.LightBlue);
            cell.Padding = 5;
            cell.Colspan = 2;
            table.AddCell(cell);
            table.AddCell(GetCell("В зоне 1", russian12));
            phrase = new Phrase(variant.ResultData.AverageHeatFlowZone1.ToString() + " Вт/м", russian12);
            phrase.Add(two);
            table.AddCell(GetCell(phrase));
            table.AddCell(GetCell("В зоне 2", russian12));
            phrase = new Phrase(variant.ResultData.AverageHeatFlowZone2.ToString() + " Вт/м", russian12);
            phrase.Add(two);
            table.AddCell(GetCell(phrase));
            doc.Add(table);

            table = GetTable();
            cell = GetCell("Температура массы слитка", russian12);
            cell.BackgroundColor = new BaseColor(Color.LightBlue);
            cell.Padding = 5;
            cell.Colspan = 2;
            table.AddCell(cell);
            table.AddCell(GetCell("В сечении 1", russian12));
            phrase = new Phrase(variant.ResultData.BarTemperatureSection1.ToString() + " ", russian12);
            phrase.Add(o);
            phrase.Add("C");
            table.AddCell(GetCell(phrase));
            table.AddCell(GetCell("В сечении 2", russian12));
            phrase = new Phrase(variant.ResultData.BarTemperatureSection2.ToString() + " ", russian12);
            phrase.Add(o);
            phrase.Add("C");
            table.AddCell(GetCell(phrase));
            table.AddCell(GetCell("В сечении 3", russian12));
            phrase = new Phrase(variant.ResultData.BarTemperatureSection3.ToString() + " ", russian12);
            phrase.Add(o);
            phrase.Add("C");
            table.AddCell(GetCell(phrase));
            doc.Add(table);

            table = GetTable();
            cell = GetCell("Удельный тепловой поток", russian12);
            cell.BackgroundColor = new BaseColor(Color.LightBlue);
            cell.Padding = 5;
            cell.Colspan = 2;
            table.AddCell(cell);
            table.AddCell(GetCell("В сечении 0", russian12));
            phrase = new Phrase(variant.ResultData.SummaryHeatFlowSector0.ToString() + " Вт/м", russian12);
            phrase.Add(two);
            table.AddCell(GetCell(phrase));
            table.AddCell(GetCell("В сечении 1 для зоны 1", russian12));
            phrase = new Phrase(variant.ResultData.SummaryHeatFlowSector1Zone1.ToString() + " Вт/м", russian12);
            phrase.Add(two);
            table.AddCell(GetCell(phrase));
            table.AddCell(GetCell("В сечении 1 для зоны 2", russian12));
            phrase = new Phrase(variant.ResultData.SummaryHeatFlowSector1Zone2.ToString() + " Вт/м", russian12);
            phrase.Add(two);
            table.AddCell(GetCell(phrase));
            table.AddCell(GetCell("В сечении 2", russian12));
            phrase = new Phrase(variant.ResultData.SummaryHeatFlowSector2.ToString() + " Вт/м", russian12);
            phrase.Add(two);
            table.AddCell(GetCell(phrase));
            doc.Add(table);

            var user = db.Users.Find(User.Identity.GetUserId());

            paragraph = new Paragraph(new Phrase("Выполнил:", russian14));
            paragraph.SpacingBefore = 20;
            doc.Add(paragraph);
            paragraph = new Paragraph(new Phrase(user.Position, russian12));
            doc.Add(paragraph);
            paragraph = new Paragraph(new Phrase(user.FIO, russian12));
            doc.Add(paragraph);

            paragraph = new Paragraph(new Phrase("Подпись:  ________" +
                "                                                                                                "
                + DateTime.Now.ToString("dd.MM.yyyyг."), russian12));
            paragraph.SpacingBefore = 20;
            doc.Add(paragraph);

            doc.Close();

            Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName);
            return File(stream.ToArray(), "application/pdf");
        }

        private PdfPCell GetCell(string text, iTextSharp.text.Font font)
        {
            var newCell = new PdfPCell(new Phrase(text, font));
            newCell.HorizontalAlignment = Element.ALIGN_CENTER;

            return newCell;
        }

        private PdfPCell GetCell(Phrase phrase)
        {
            var newCell = new PdfPCell(phrase);
            newCell.HorizontalAlignment = Element.ALIGN_CENTER;

            return newCell;
        }

        private PdfPTable GetTable()
        {
            var newTable = new PdfPTable(2);
            newTable.SpacingBefore = 30;

            return newTable;
        }

        private void Calculate(Variant variant)
        {
            if (variant.ResultData == null) variant.ResultData = new ResultData();

            var steelTypePropertyValue = variant.InputData.SteelTypeProperty;
            var steelTypeProperty = (FurnanceCalc.Enums.SteelTypeProperty)Enum.Parse(typeof(FurnanceCalc.Enums.SteelTypeProperty), steelTypePropertyValue.ToString());

            var steelTypeForDensityValue = (int)variant.InputData.SteelTypeProperty;
            var steelTypeForDensity = (FurnanceCalc.Enums.SteelTypeForDensity)Enum.Parse(typeof(FurnanceCalc.Enums.SteelTypeForDensity), steelTypeForDensityValue.ToString());

            var isLongFlameTorch = variant.InputData.TorchType == TorchType.LongFlame ? true : false;

            FurnanceCalc.FurnanceCalculator calc = new FurnanceCalc.FurnanceCalculator();

            variant.ResultData.ConsumptionTemperature = calc.GetConsumptionTemperature(isLongFlameTorch, variant.InputData.HeatBlast, variant.InputData.HeatNatural, variant.InputData.HeatFull, variant.InputData.AirTemperature);

            variant.ResultData.SummaryHeatFlowSector2 = calc.GetSummaryHeatFlowOfSection2(isLongFlameTorch, variant.InputData.FurnanceWidth, variant.InputData.BarHeight, variant.InputData.WorkHeightSector2,
                variant.InputData.HeatBlast, variant.InputData.HeatNatural, variant.InputData.HeatFull, variant.InputData.GasTemperatureSector2, variant.InputData.BarLength, variant.InputData.BarNumber, variant.InputData.EndTopSteelTemperature);

            variant.ResultData.SummaryHeatFlowSector0 = calc.GetSummaryHeatFlowOfSection0(isLongFlameTorch, variant.InputData.FurnanceWidth, variant.InputData.BarHeight, variant.InputData.WorkHeightSector0,
                variant.InputData.HeatBlast, variant.InputData.HeatNatural, variant.InputData.HeatFull, variant.InputData.GasTemperatureSector0, variant.InputData.BarLength, variant.InputData.BarNumber, variant.InputData.StartSteelTemperature);

            variant.ResultData.SummaryHeatFlowSector1Zone1 = calc.GetSummaryHeatFlowOfSection1ForZone1(isLongFlameTorch, variant.InputData.FurnanceWidth, variant.InputData.BarHeight, variant.InputData.FurnanceHeightZone1,
                variant.InputData.HeatBlast, variant.InputData.HeatNatural, variant.InputData.HeatFull, variant.InputData.GasTemperatureSector1, variant.InputData.BarLength, variant.InputData.BarNumber, variant.InputData.TopSteelTemperatureSector1);

            variant.ResultData.SummaryHeatFlowSector1Zone2 = calc.GetSummaryHeatFlowOfSection1ForZone2(isLongFlameTorch, variant.InputData.FurnanceWidth, variant.InputData.BarHeight, variant.InputData.WorkHeightSector1,
                variant.InputData.HeatBlast, variant.InputData.HeatNatural, variant.InputData.HeatFull, variant.InputData.GasTemperatureSector1, variant.InputData.BarLength, variant.InputData.BarNumber, variant.InputData.TopSteelTemperatureSector1);

            var minTempSection2 = calc.GetMinimumMetalTemperatureInSection2(variant.InputData.BarHeight, variant.InputData.EndTopSteelTemperature, variant.ResultData.SummaryHeatFlowSector2, steelTypeProperty);
            variant.ResultData.BarTemperatureSection2 = calc.GetMetalTemperatureInSection2(variant.InputData.EndTopSteelTemperature, minTempSection2);
            variant.ResultData.BarTemperatureSection1 = calc.GetMetalTemperatureInSection1(variant.InputData.BarHeight, variant.InputData.TopSteelTemperatureSector1, variant.ResultData.SummaryHeatFlowSector1Zone1, steelTypeProperty);
            variant.ResultData.BarTemperatureSection3 = calc.GetMetalTemperatureInSection3(variant.InputData.EndTopSteelTemperature, variant.InputData.EndBottomSteelTemperature);

            variant.ResultData.AverageHeatFlowZone1 = calc.GetAverageHeatFlowInZone1(variant.ResultData.SummaryHeatFlowSector1Zone1, variant.ResultData.SummaryHeatFlowSector2);
            variant.ResultData.AverageHeatFlowZone2 = calc.GetAverageHeatFlowInZone2(variant.ResultData.SummaryHeatFlowSector1Zone2, variant.ResultData.SummaryHeatFlowSector2);

            variant.ResultData.HeatingTimeZone1 = calc.GetHeatingTimeinZone1(variant.InputData.BarHeight, variant.InputData.StartSteelTemperature, variant.ResultData.BarTemperatureSection1, variant.ResultData.AverageHeatFlowZone1, steelTypeForDensity, steelTypeProperty);
            variant.ResultData.HeatingTimeZone2 = calc.GetHeatingTimeinZone2(variant.InputData.BarHeight, variant.ResultData.BarTemperatureSection1, variant.ResultData.BarTemperatureSection2, variant.ResultData.AverageHeatFlowZone2, steelTypeForDensity, steelTypeProperty);
            variant.ResultData.HeatingTimeZone3 = calc.GetTimeForZone3(variant.ResultData.BarTemperatureSection3, variant.ResultData.BarTemperatureSection2, variant.InputData.EndTopSteelTemperature, variant.InputData.EndBottomSteelTemperature, variant.InputData.EndTopSteelTemperature, minTempSection2, variant.InputData.BarHeight, steelTypeForDensity, steelTypeProperty);
            variant.ResultData.HeatingTimeFull = variant.ResultData.HeatingTimeZone1 + variant.ResultData.HeatingTimeZone2 + variant.ResultData.HeatingTimeZone3;

            variant.ResultData.LengthFull = calc.GetLengthActivePartFurnance(variant.ResultData.HeatingTimeFull, variant.InputData.FurnanceProductivity, variant.InputData.BarHeight, variant.InputData.BarLength, variant.InputData.BarNumber, steelTypeForDensity);
            variant.ResultData.LengthZone1 = calc.GetLengthInZone(variant.ResultData.LengthFull, variant.ResultData.HeatingTimeZone1, variant.ResultData.HeatingTimeFull);
            variant.ResultData.LengthZone2 = calc.GetLengthInZone(variant.ResultData.LengthFull, variant.ResultData.HeatingTimeZone2, variant.ResultData.HeatingTimeFull);
            variant.ResultData.LengthZone3 = calc.GetLengthInZone(variant.ResultData.LengthFull, variant.ResultData.HeatingTimeZone3, variant.ResultData.HeatingTimeFull);

            variant.ResultData.HeatingTimeZone1 = variant.ResultData.HeatingTimeZone1 / 3600;
            variant.ResultData.HeatingTimeZone2 = variant.ResultData.HeatingTimeZone2 / 3600;
            variant.ResultData.HeatingTimeZone3 = variant.ResultData.HeatingTimeZone3 / 3600;
            variant.ResultData.HeatingTimeFull = variant.ResultData.HeatingTimeFull / 3600;

            variant.ResultData.Round();
        }
    }
}