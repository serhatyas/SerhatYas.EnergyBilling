using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Repositories.Constants
{
    //excel fiyat bilgileri d ve e kısmı

    public static class PriceConstants
    {
        // YEK Fiyatı (TL/MWh)
        public const decimal YekPrice = 300m;

        // Sanayi Enerji Tarifesi (TL/MWh)
        public const decimal IndustrialEnergyTariff = 2800m;

        // Ticarethane Enerji Tarifesi (TL/MWh)
        public const decimal CommercialEnergyTariff = 3000m;

        // Sanayi Dağıtım Tarifesi (TL/MWh)
        public const decimal IndustrialDistributionTariff = 500m;

        // Ticarethane Dağıtım Tarifesi (TL/MWh)
        public const decimal CommercialDistributionTariff = 600m;
    }
}
