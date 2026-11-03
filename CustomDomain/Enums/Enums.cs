using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDomain.Enums
{
    public enum UserRole
    {
        Declarant = 0,          // Deklarant
        CustomsOfficer = 1,     // Gömrük əməkdaşı
        WarehouseOperator = 2,  // Anbar işçisi
        Admin = 3
    }

    public enum DeclarantType
    {
        Individual = 0,   // Fiziki şəxs
        LegalEntity = 1   // Hüquqi şəxs
    }

    public enum DeclarationStatus
    {
        Draft = 0,
        Submitted = 1,
        UnderReview = 2,
        Approved = 3,
        Rejected = 4,
        Released = 5
    }

}
