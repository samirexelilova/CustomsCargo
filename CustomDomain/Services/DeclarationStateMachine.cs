using CustomDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDomain.Services
{
    public static class DeclarationStateMachine
    {
        private sealed record Rule(DeclarationStatus From, DeclarationStatus To, UserRole[] Roles);

        private static readonly Rule[] Rules = new Rule[]
        {
        new Rule(DeclarationStatus.Draft, DeclarationStatus.Submitted,
            new[] { UserRole.Declarant }),

        new Rule(DeclarationStatus.Submitted, DeclarationStatus.UnderReview,
            new[] { UserRole.CustomsOfficer, UserRole.Admin }),

        new Rule(DeclarationStatus.UnderReview, DeclarationStatus.Approved,
            new[] { UserRole.CustomsOfficer, UserRole.Admin }),

        new Rule(DeclarationStatus.UnderReview, DeclarationStatus.Rejected,
            new[] { UserRole.CustomsOfficer, UserRole.Admin }),

        new Rule(DeclarationStatus.Approved, DeclarationStatus.Released,
            new[] { UserRole.WarehouseOperator, UserRole.Admin }),

        new Rule(DeclarationStatus.Rejected, DeclarationStatus.Draft,
            new[] { UserRole.Declarant }),
        };

        public static bool IsValidTransition(DeclarationStatus from, DeclarationStatus to)
            => Rules.Any(r => r.From == from && r.To == to);

        public static bool IsAllowed(DeclarationStatus from, DeclarationStatus to, UserRole role)
            => Rules.Any(r => r.From == from && r.To == to && r.Roles.Contains(role));

        public static IReadOnlyList<DeclarationStatus> AllowedTargets(DeclarationStatus from, UserRole role)
            => Rules.Where(r => r.From == from && r.Roles.Contains(role)).Select(r => r.To).ToList();
    }
}