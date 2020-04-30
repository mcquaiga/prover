namespace Devices.Communications
{
    //public static class DeviceConnection
    //{
    //    public static Task<ICommunicationsClient> ConnectAsync<T>(this T deviceType, ICommPort commPort, int retryAttempts = 10, TimeSpan? timeout = null, IObserver<string> statusObserver = null)
    //        where T : DeviceType
    //    {
    //        var ass = Assembly.Load(deviceType.GetType().Assembly.ToString());

    //        var factory = ass.DefinedTypes.FirstOrDefault(t => t.ImplementedInterfaces.Contains(typeof(IDeviceTypeCommClientFactory<>)) && !t.IsInterface && !t.IsAbstract);

    //        if (factory == null)
    //            throw new ArgumentNullException($"Could not locate factory method for device type {typeof(T)}.");
            
    //        var clientFactory = (IDeviceTypeCommClientFactory<T>)Activator.CreateInstance(factory);
    //        var method = factory.GetMethod("Create");

    //        return (Task<ICommunicationsClient>)method?.Invoke(clientFactory, new object[] { deviceType, commPort, retryAttempts, timeout, statusObserver });
    //    }
    //}
}