using App.Repositories.Consumptions;
using App.Repositories.Data;
using App.Repositories.Meters;
using App.Repositories.PriceInfos;
using App.Services.Data;
using App.Services.Invoices;
using App.Services.Invoices.SerhatYas.EnergyBilling.App.Services.Invoices;
using App.Services.Municipalitie;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Controllers ekle
        builder.Services.AddControllers();

        // Swagger ekle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Excel dosya yolu
        var excelFilePath = builder.Configuration.GetValue<string>("ExcelFilePath") ?? "Data/Sayax_Task_Veri.xlsx";

        // Data Services
        builder.Services.AddSingleton<IExcelDataReader>(provider => new ExcelDataReader(excelFilePath));

        // Repositories (Singleton - memory'de tutuyoruz)
        builder.Services.AddSingleton<IMeterRepository>(provider =>
        {
            var excelReader = provider.GetRequiredService<IExcelDataReader>();
            var meters = excelReader.ReadMetersAsync().Result;
            return new MeterRepository(meters);
        });

        builder.Services.AddSingleton<IConsumptionRepository>(provider =>
        {
            var excelReader = provider.GetRequiredService<IExcelDataReader>();
            var consumptions = excelReader.ReadConsumptionsAsync().Result;
            return new ConsumptionRepository(consumptions);
        });

        builder.Services.AddSingleton<IPriceInfoRepository>(provider =>
        {
            var excelReader = provider.GetRequiredService<IExcelDataReader>();
            var priceInfos = excelReader.ReadPriceInfosAsync().Result;
            return new PriceInfoRepository(priceInfos);
        });

        // Business Services
        builder.Services.AddScoped<IInvoiceCalculationService, InvoiceCalculationService>();
        builder.Services.AddScoped<IMunicipalityPaymentService, MunicipalityPaymentService>();
        builder.Services.AddScoped<IDataService, DataService>();

        var app = builder.Build();

        app.MapFallbackToFile("index.html");

        // Development environment
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAuthorization();
        app.MapControllers();

        // Root endpoint
        app.MapGet("/", () => Results.Redirect("/swagger"));

        Console.WriteLine("Enerji Fatura Hesaplama API baþlatýldý!");
        Console.WriteLine("Swagger UI: https://localhost:7000/swagger");

        app.Run();
    }
}