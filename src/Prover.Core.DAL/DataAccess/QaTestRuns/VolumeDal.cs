using System.ComponentModel.DataAnnotations.Schema;
using Prover.Core.DAL.Common;

namespace Prover.Core.DAL.DataAccess.QaTestRuns
{
    [Table("qa_volume")]
    public class VolumeDal : Entity
    {
        public virtual TestPointDal TestPoint { get; protected set; }
    }
}