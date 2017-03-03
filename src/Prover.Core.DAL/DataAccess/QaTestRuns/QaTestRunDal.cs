using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.InstrumentTypes;
using Prover.CommProtocol.Common.Items;
using Prover.Core.DAL.Common;
using Prover.Core.Domain.Enums;
using Prover.Core.Domain.Models.QaTestRuns;
using Prover.Core.Extensions;

namespace Prover.Core.DAL.DataAccess.QaTestRuns
{
    public class QaTestRunRepository : DataRepository<QaTestRunDal, QaTestRunDto>
    {
        
    }

    [Table("qa_testruns")]
    public class QaTestRunDal : AggregateRoot
    {
        [NotMapped]
        public EvcCorrectorType CorrectorType { get; set; }

        [Column("CorrectorType")]
        protected string CorrectorTypeString
        {
            get { return CorrectorType.ToString(); }
            set { CorrectorType = value.ParseEnum<EvcCorrectorType>(); }
        }

        [NotMapped]
        public InstrumentType InstrumentType { get; protected set; }

        [Column("InstrumentType")]
        protected string InstrumentTypeString
        {
            get { return InstrumentType.Name; }
            set { InstrumentType = Instruments.GetAll().FirstOrDefault(x => string.Equals(x.Name, value)); }
        }

        public DateTime TestDateTime { get; set; }
        public DateTime? ExportedDateTime { get; set; }
        public DateTime? ArchivedDateTime { get; set; }
        public bool? EventLogPassed { get; set; }
        public bool? CommPortsPassed { get; set; }
        public string EmployeeId { get; set; }
        public string JobId { get; set; }

        /// <summary>
        /// Gets a Json value from the constructor
        /// </summary>
        public string ItemValues { get; protected set; }

        public virtual ICollection<TestPointDal> TestPoints { get; set; }
    }
}


//public QaTestRunDal(EvcCorrectorType correctorType,
//    InstrumentType instrumentType,
//    DateTime testDateTime, DateTime? exportedDateTime, DateTime? archivedDateTime, bool? eventLogPassed,
//    bool? commPortsPassed, string employeeId, string jobId, ICollection<ItemValue> instrumentItems,
//    ICollection<TestPointDal> testPoints)
//{
//    CorrectorType = correctorType;
//    CorrectorTypeString = CorrectorType.ToString();

//    InstrumentType = instrumentType;
//    InstrumentTypeString = InstrumentType.Name;

//    TestDateTime = testDateTime;
//    ExportedDateTime = exportedDateTime;
//    ArchivedDateTime = archivedDateTime;

//    EventLogPassed = eventLogPassed;
//    CommPortsPassed = commPortsPassed;
//    EmployeeId = employeeId;
//    JobId = jobId;

//    ItemValues = instrumentItems.Serialize();
//    TestPoints = testPoints;
//}