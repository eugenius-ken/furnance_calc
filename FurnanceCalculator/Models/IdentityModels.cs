using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using FurnanceCalculator.Enums;

namespace FurnanceCalculator.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public string Position { get; set; }
        public string FIO { get; set; }

        public virtual ICollection<Variant> Variants { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("FurnanceCalcConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        static ApplicationDbContext()
        {
            Database.SetInitializer(new Dbinitializer());
        }

        public DbSet<Variant> Variants { get; set; }
        public DbSet<InputData> InputData { get; set; }
        public DbSet<ResultData> ResultData { get; set; }

    }

    public class Dbinitializer: DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            RoleManager.Create(new IdentityRole("Administrator"));
            RoleManager.Create(new IdentityRole("User"));

            var admin = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Administrator@urfu.ru",
                FIO = "Иванов Иван Иванович",
                Position = "Руководитель проектов"
            };

            var user = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "User@urfu.ru",
                FIO = "Петров Петр Петрович",
                Position = "Инженер-проектировщик"
            };

            UserManager.Create(admin, "7753191Aa-");
            UserManager.Create(user, "7753191Aa-");

            UserManager.AddToRole(admin.Id, "Administrator");
            UserManager.AddToRole(user.Id, "User");

            Variant variant = new Variant()
            {
                Id = Guid.NewGuid(),
                Name = "Вариант по умолчанию",
                Description = "Для демонстрации работы расчета",
                User = user,
                IsBarParametersExist = true,
                IsEnvironmentParametersExist = true,
                IsFuelParametersExist = true,
                IsFurnanceParametersExist = true,
                InputData = new InputData()
                {
                    BarHeight = 0.35,
                    BarThickness = 0.35,
                    BarLength = 2,
                    EndTopSteelTemperature = 1200,
                    EndBottomSteelTemperature = 1100,
                    StartSteelTemperature = 20,
                    TopSteelTemperatureSector1 = 750,
                    SteelTypeForDensity = SteelTypeForDensity.CastQuiet,
                    SteelTypeProperty = SteelTypeProperty.LowCarbon,
                    BarNumber = 1,
                    FurnanceHeightZone1 = 1.06,
                    WorkHeightSector0 = 1.51,
                    WorkHeightSector1 = 0.98,
                    WorkHeightSector2 = 2.54,
                    FurnanceProductivity = 20,
                    FurnanceWidth = 2.6,
                    TorchType = TorchType.LongFlame,
                    AirTemperature = 350,
                    GasTemperatureSector0 = 800,
                    GasTemperatureSector1 = 1230,
                    GasTemperatureSector2 = 1280,
                    HeatNatural = 35588,
                    HeatBlast = 3350,
                    HeatFull = 16750
                }
            };

            Calculate(variant);

            context.Variants.Add(variant);
            context.SaveChanges();
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