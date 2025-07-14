using App.Repositories.Base;
using App.Repositories.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Repositories.Meters
{
    public class MeterRepository : GenericRepository<Meter>, IMeterRepository
    {
        public MeterRepository(List<Meter> meters) : base(meters)
        {
        }

        // Sayaç numarasına göre getir
        public async Task<Meter?> GetByMeterNumberAsync(string meterNumber)
        {
            return await Task.FromResult(_data.FirstOrDefault(m => m.MeterNumber == meterNumber));
        }

        // Belediyeye göre getir
        public async Task<IEnumerable<Meter>> GetByMunicipalityAsync(string municipality)
        {
            return await Task.FromResult(_data.Where(m =>
                m.Municipality.Equals(municipality, StringComparison.OrdinalIgnoreCase)));
        }

        // Satış yöntemine göre getir
        public async Task<IEnumerable<Meter>> GetBySalesMethodAsync(SalesMethod salesMethod)
        {
            return await Task.FromResult(_data.Where(m => m.SalesMethod == salesMethod));
        }

        // Tarife türüne göre getir
        public async Task<IEnumerable<Meter>> GetByTariffTypeAsync(TariffType tariffType)
        {
            return await Task.FromResult(_data.Where(m => m.TariffType == tariffType));
        }

        // Override GetByIdAsync
        public override async Task<Meter?> GetByIdAsync(object id)
        {
            if (id is string meterNumber)
                return await GetByMeterNumberAsync(meterNumber);

            return null;
        }
    }
}
