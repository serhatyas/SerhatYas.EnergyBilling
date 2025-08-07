using App.Repositories.Consumptions;
using App.Repositories.Enums;
using App.Repositories.PriceInfos;
using ClosedXML.Excel;
using Meter = App.Repositories.Meters.Meter;

namespace App.Repositories.Data
{

    // Excel dosyasından sayaç, tüketim ve fiyat verilerini okur ve belleğe yükler.

    public class ExcelDataReader
        
        : IExcelDataReader
    {
        private string _filePath;

        public ExcelDataReader(string filePath)
        {
            _filePath = filePath;
            // Lisans ayarına gerek yok!
        }

        public void SetFilePath(string filePath)
        {
            _filePath = filePath;
        }

        public bool FileExists()
        {
            return File.Exists(_filePath);
        }

        public async Task<List<Meter>> ReadMetersAsync()
        {
            //sayaç bilgilerini çeker

            var meters = new List<Meter>();

            using var workbook = new XLWorkbook(_filePath);
            var worksheet = workbook.Worksheet("Sayac Bilgileri");

            if (worksheet == null) return meters;

            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

            for (int row = 2; row <= lastRow; row++)
            {
                var meterNumber = worksheet.Cell(row, 1).GetString();
                if (string.IsNullOrEmpty(meterNumber)) continue;

                var salesMethodText = worksheet.Cell(row, 2).GetString();
                var commissionValue = worksheet.Cell(row, 3).Value;
                var btvRate = Convert.ToDecimal(worksheet.Cell(row, 4).GetDouble());
                var kdvRate = Convert.ToDecimal(worksheet.Cell(row, 5).GetDouble());
                var tariffName = worksheet.Cell(row, 6).GetString();
                var municipality = worksheet.Cell(row, 7).GetString();

                var meter = new Meter
                {
                    MeterNumber = meterNumber,
                    SalesMethod = ParseSalesMethod(salesMethodText),
                    MunicipalityTaxRate = btvRate,
                    VatRate = kdvRate,
                    TariffType = ParseTariffType(tariffName),
                    Municipality = municipality
                };

                ParseCommissionDiscount(meter, commissionValue, salesMethodText);
                meters.Add(meter);
            }

            return meters;
        }

        public async Task<List<Consumption>> ReadConsumptionsAsync()
        {

            //s1, s2, s3 sayfalarındaki veriyi okur,  filtrelenebilir hale geliyor.

            var consumptions = new List<Consumption>();

            using var workbook = new XLWorkbook(_filePath);
            var sheetNames = new[] { "S1 Tuketim", "S2 Tuketim", "S3 Tuketim" };

            foreach (var sheetName in sheetNames)
            {
                var worksheet = workbook.Worksheet(sheetName);
                if (worksheet == null) continue;

                var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

                for (int row = 2; row <= lastRow; row++)
                {
                    var meterNumber = worksheet.Cell(row, 1).GetString();
                    var dateValue = worksheet.Cell(row, 2).Value;
                    var hour = Convert.ToInt32(worksheet.Cell(row, 3).GetDouble());
                    var amount = Convert.ToDecimal(worksheet.Cell(row, 4).GetDouble());

                    if (string.IsNullOrEmpty(meterNumber)) continue;

                    if (TryParseDate(dateValue, out DateTime date))
                    {
                        consumptions.Add(new Consumption
                        {
                            MeterNumber = meterNumber,
                            Date = date,
                            Hour = hour,
                            ConsumptionAmount = amount
                        });
                    }
                }
            }

            return consumptions;
        }

        public async Task<List<PriceInfo>> ReadPriceInfosAsync()
        {

            //fiyat bilgilerini çeker

            var priceInfos = new List<PriceInfo>();

            using var workbook = new XLWorkbook(_filePath);
            var worksheet = workbook.Worksheet("Fiyat Bilgileri");

            if (worksheet == null) return priceInfos;

            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

            for (int row = 2; row <= lastRow; row++)
            {
                var dateTimeValue = worksheet.Cell(row, 1);
                var ptfValue = worksheet.Cell(row, 2);

                if (dateTimeValue.IsEmpty() || ptfValue.IsEmpty()) continue;

                if (TryParseDate(dateTimeValue.Value, out DateTime dateTime))
                {
                    var ptfPrice = Convert.ToDecimal(ptfValue.GetDouble());

                    priceInfos.Add(new PriceInfo
                    {
                        DateTime = dateTime,
                        PtfPrice = ptfPrice
                    });
                }
            }

            return priceInfos;
        }

        // Helper metodlar (aynı kalacak)
        private static SalesMethod ParseSalesMethod(string salesMethodText)
        {
            var text = salesMethodText.ToLower();

            if (text.Contains("ptf") && text.Contains("yek") && text.Contains("%"))
                return SalesMethod.PtfYekWithPercentageCommission;

            if (text.Contains("ptf") && text.Contains("yek") && text.Contains("komisyon"))
                return SalesMethod.PtfYekWithFixedCommission;

            if (text.Contains("tarife") && text.Contains("indirim"))
                return SalesMethod.TariffWithPercentageDiscount;

            return SalesMethod.PtfYekWithPercentageCommission;
        }

        private static TariffType ParseTariffType(string tariffName)
        {
            return tariffName.ToLower() switch
            {
                "sanayi" => TariffType.Industrial,
                "ticarethane" => TariffType.Commercial,
                _ => TariffType.Industrial
            };
        }

        private static void ParseCommissionDiscount(Meter meter, object? commissionValue, string salesMethodText)
        {
            if (commissionValue == null) return;

            var valueStr = commissionValue.ToString();
            if (decimal.TryParse(valueStr?.Replace(" TL", ""), out var numericValue))
            {
                if (salesMethodText.Contains("TL"))
                {
                    meter.FixedCommissionAmount = numericValue;
                }
                else if (salesMethodText.Contains("İndirim"))
                {
                    meter.DiscountRate = numericValue;
                }
                else
                {
                    meter.CommissionRate = numericValue;
                }
            }
        }

        private static bool TryParseDate(object dateValue, out DateTime date)
        {
            date = default;

            if (dateValue is DateTime dt)
            {
                date = dt;
                return true;
            }

            if (DateTime.TryParse(dateValue.ToString(), out dt))
            {
                date = dt;
                return true;
            }

            return false;
        }
    }
}
