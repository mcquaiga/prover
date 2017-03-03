using System.Diagnostics.CodeAnalysis;
using Prover.Core.Domain.Models.QaTestRuns;

namespace Prover.Core.Domain.Enums
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum EvcCorrectorType
    {
        T,
        P,
        PTZ
    }

    public enum TestLevel
    {
        High = 0,
        Medium = 1,
        Low = 2
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum TemperatureUnits
    {
        C,
        F,
        R,
        K
    }
}