using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Repositories.PriceInfos
{
    //Fiyat bilgilerinde ptf model hali

    public class PriceInfo
    {
        // Tarih ve Saat Bilgisi
        public DateTime DateTime { get; set; }

        // PTF Fiyatı
        public decimal PtfPrice { get; set; }
    }
}
