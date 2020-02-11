using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items;
using Domain.EvcVerifications.CorrectionTests;
using Domain.Interfaces;

namespace Domain.EvcVerifications
{
    public class VerificationTestPoint : AggregateJoin<IVerificationTest>, IVerificationTest
    {
        protected VerificationTestPoint()
        {

        }

        protected internal VerificationTestPoint(int testNumber)
        {
            TestNumber = testNumber;
        }

        #region Public Properties

        public decimal? AppliedInput { get; set; }

        public virtual IEnumerable<ItemValue> BeforeValues { get; set; }

        public virtual IEnumerable<ItemValue> AfterValues { get; set; }

        public bool HasPassed()
        {
            return Tests.All(t => t.HasPassed());
        }

        public int TestNumber { get; set; }

        #endregion

        
    }

    

}