using App.Repositories.Base;
using App.Repositories.Enums;

namespace App.Repositories.Meters
{
    public interface IMeterRepository:IGenericRepository<Meter>
    {
        // Sayaç numarasına göre getir
        Task<Meter?> GetByMeterNumberAsync(string meterNumber);

        // Belediyeye göre getir
        Task<IEnumerable<Meter>> GetByMunicipalityAsync(string municipality);

        // Satış yöntemine göre getir
        Task<IEnumerable<Meter>> GetBySalesMethodAsync(SalesMethod salesMethod);

        // Tarife türüne göre getir
        Task<IEnumerable<Meter>> GetByTariffTypeAsync(TariffType tariffType);
    }
}
