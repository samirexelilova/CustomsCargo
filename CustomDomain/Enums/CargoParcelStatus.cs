using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDomain.Enums
{
    public enum CargoParcelStatus
    {
        Arrived = 0,    // Bağlama kargo anbarına çatıb, bəyan gözlənilir
        Declared = 1    // Bəyannamə (Draft) yaradılıb
    }
}
