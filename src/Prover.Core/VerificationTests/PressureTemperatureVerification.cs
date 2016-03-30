using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests
{
    class PressureTemperatureVerification : VerificationBase
    {
        protected PressureTemperatureVerification(Instrument instrument, InstrumentCommunicator instrumentComm) 
            : base(instrument, instrumentComm)
        {
        }

        //private void BuildCorrectorTypes()
        //{
        //    if (CorrectorType == CorrectorType.PressureOnly)
        //    {
        //        Pressure = new Pressure(this);
        //        Pressure.AddTest();
        //        Pressure.AddTest();
        //        Pressure.AddTest();

        //        VerificationTests.Add(new VerificationTest(0, this, null, Pressure.Tests[0]));
        //        VerificationTests.Add(new VerificationTest(1, this, null, Pressure.Tests[1]));
        //        VerificationTests.Add(new VerificationTest(2, this, null, Pressure.Tests[2]));
        //    }

        //    if (CorrectorType == CorrectorType.TemperatureOnly)
        //    {
        //        Temperature = new Temperature(this);
        //        Temperature.AddTemperatureTest();
        //        Temperature.AddTemperatureTest();
        //        Temperature.AddTemperatureTest();

        //        VerificationTests.Add(new VerificationTest(0, this, Temperature.Tests[0], null));
        //        VerificationTests.Add(new VerificationTest(1, this, Temperature.Tests[1], null));
        //        VerificationTests.Add(new VerificationTest(2, this, Temperature.Tests[2], null));
        //    }

        //    if (CorrectorType == CorrectorType.PressureTemperature)
        //    {
        //        Temperature = new Temperature(this);
        //        Temperature.AddTemperatureTest();
        //        Temperature.AddTemperatureTest();
        //        Temperature.AddTemperatureTest();

        //        Pressure = new Pressure(this);
        //        Pressure.AddTest();
        //        Pressure.AddTest();
        //        Pressure.AddTest();

        //        VerificationTests.Add(new VerificationTest(0, this, Temperature.Tests[0], Pressure.Tests[0]));
        //        VerificationTests.Add(new VerificationTest(1, this, Temperature.Tests[1], Pressure.Tests[1]));
        //        VerificationTests.Add(new VerificationTest(2, this, Temperature.Tests[2], Pressure.Tests[2]));
        //    }

        //    Volume = new Volume(this);
        //}
    }
}
