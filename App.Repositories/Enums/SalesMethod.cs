using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Repositories.Enums
{
    //Satış Yöntemi Türleri
    public enum SalesMethod
    {
        // (PTF+YEK) %Komisyon
        PtfYekWithPercentageCommission = 1,

        // PTF + YEK + Komisyon (sabit tutar)
        PtfYekWithFixedCommission = 2,

        // Tarife - %İndirim  
        TariffWithPercentageDiscount = 3
    }
}
