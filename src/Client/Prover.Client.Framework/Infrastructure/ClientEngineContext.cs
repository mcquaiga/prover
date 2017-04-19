//using System.Configuration;
//using Prover.Client.Framework.Configuration;
//using Prover.Shared.Infrastructure;

//namespace Prover.Client.Framework.Infrastructure
//{
//    /// <summary>
//    ///     Provides access to the singleton instance of the Nop engine.
//    /// </summary>
//    public class ClientEngineContext
//    {
//        #region Methods

//        /// <summary>
//        ///     Initializes a static instance of the Nop factory.
//        /// </summary>
//        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
       
//        public static IEngine<ClientConfig> Initialize(bool forceRecreate)
//        {
//            if (Singleton<IEngine<ClientConfig>>.Instance == null || forceRecreate)
//            {
//                Singleton<IEngine<ClientConfig>>.Instance = new ClientEngine();

//                var config = ConfigurationManager.GetSection("ClientConfig") as ClientConfig;;
//                Singleton<IEngine<ClientConfig>>.Instance.Initialize(config);
//            }
//            return Singleton<IEngine<ClientConfig>>.Instance;
//        }

//        /// <summary>
//        ///     Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.
//        /// </summary>
//        /// <param name="engine">The engine to use.</param>
//        /// <remarks>Only use this method if you know what you're doing.</remarks>
//        public static void Replace(IEngine<ClientConfig> engine)
//        {
//            Singleton<IEngine<ClientConfig>>.Instance = engine;
//        }

//        #endregion

//        #region Properties

//        /// <summary>
//        ///     Gets the singleton Nop engine used to access Nop services.
//        /// </summary>
//        public static IEngine<ClientConfig> Current
//        {
//            get
//            {
//                if (Singleton<IEngine<ClientConfig>>.Instance == null)
//                    Initialize(false);
//                return Singleton<IEngine<ClientConfig>>.Instance;
//            }
//        }

//        #endregion
//    }
//}