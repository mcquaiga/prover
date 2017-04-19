using System;
using System.Configuration;
using System.Xml;
using NLog.Targets;

namespace Prover.Client.Framework.Configuration
{
    public class ClientConfig
    {
        public bool IgnoreStartupTasks { get; set; }
        
        public DatabaseInfo Database { get; set; }
        public class DatabaseInfo
        {
            public string Provider { get; set; }
            public string ConnectionString { get; set; }
        }
    }
}