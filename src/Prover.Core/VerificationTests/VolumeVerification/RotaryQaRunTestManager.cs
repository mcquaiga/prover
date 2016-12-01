namespace Prover.Core.VerificationTests.VolumeVerification
{
    //public sealed class RotaryQaRunTestManager : QaRunTestManager
    //{
    //    private RotaryQaRunTestManager(IContainer container, Instrument instrument,
    //        EvcCommunicationClient instrumentCommunicator, VolumeTestManagerBase volumeTestManager, IValidator verifier)
    //        : base(container, instrument, instrumentCommunicator, verifier)
    //    {
    //        VolumeTestManagerBase = volumeTestManager;
    //    }

    //    public static async Task<RotaryQaRunTestManager> CreateRotaryTest(IContainer container, EvcCommunicationClient instrumentCommClient, string tachometerPortName, IValidator verifier)
    //    {
    //        TachometerCommunicator tachComm = null;
    //        if (!string.IsNullOrEmpty(tachometerPortName))
    //        {
    //            tachComm = new TachometerCommunicator(tachometerPortName);
    //        }

    //        await instrumentCommClient.Connect();
    //        var itemValues = await instrumentCommClient.GetItemValues(instrumentCommClient.ItemDetails.GetAllItemNumbers());
    //        await instrumentCommClient.Disconnect();

    //        var instrument = new Instrument(InstrumentType.MiniMax, itemValues);
    //        var driveType = new RotaryDrive(instrument);
    //        CreateVerificationTests(instrument, driveType);

    //        var volumeTest = instrument.VolumeTest;
    //        var rotaryVolumeTest = new AutoVolumeTestManagerBase(container.Resolve<IEventAggregator>(), volumeTest, instrumentCommClient, tachComm);

    //        var manager = new RotaryQaRunTestManager(container, instrument, instrumentCommClient, rotaryVolumeTest, verifier);
    //        await manager.SaveAsync();
    //        container.RegisterInstance<QaRunTestManager>(manager);

    //        return manager;
    //    }
    //}
}