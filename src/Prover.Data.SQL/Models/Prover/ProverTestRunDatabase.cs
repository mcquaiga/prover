using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prover.Domain.Instrument;
using Prover.Domain.Models.Prover;

namespace Prover.Data.EF.Models.Prover
{
    internal class ProverTestRunDatabase : ProverTestRun
    {
        [NotMapped]
        public override IInstrument Instrument { get; set; }

        public string InstrumentType => JsonConvert.SerializeObject(Instrument.ItemData);

    }
}
