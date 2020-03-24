using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Shared.Data;

namespace Prover.Storage.EF
{
    class SqliteDataProvider : IDataProvider
    {
        public void InitConnectionFactory()
        {
            throw new NotImplementedException();
        }

        public void SetDatabaseInitializer()
        {
            throw new NotImplementedException();
        }

        public void InitializeProvider()
        {
            throw new NotImplementedException();
        }

        public bool StoredProceduredSupported => false;
        public bool BackupSupported => false;

        public int SupportedLengthOfBinaryHash()
        {
            throw new NotImplementedException();
        }
    }
}
