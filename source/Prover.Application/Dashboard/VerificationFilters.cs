using System;
using Prover.Application.Models.EvcVerifications;

namespace Prover.Application.Dashboard
{
    public static class VerificationFilters
    {
        public static Func<EvcVerificationTest, bool> IsVerified { get; } = test => test.Verified;
        public static Func<EvcVerificationTest, bool> IsNotVerified { get; } = test => !test.Verified;
        public static Func<EvcVerificationTest, bool> IsNotExported { get; } = test => test.ExportedDateTime == null;
    }
}