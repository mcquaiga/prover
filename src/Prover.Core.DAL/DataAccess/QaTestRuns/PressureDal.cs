using System.ComponentModel.DataAnnotations.Schema;
using Prover.Core.DAL.Common;

namespace Prover.Core.DAL.DataAccess.QaTestRuns
{
    [Table("qa_pressure")]
    public class PressureDal : Entity
    {
        public virtual TestPointDal TestPoint { get; protected set; }
    }
}