using Devices.Core.Items.ItemGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Shared.Interfaces;
using Tests.Application;

namespace Application.ExternalDevices.DInOutBoards.Tests
{

    [TestClass]
    public class PulseChannelListenerTests
    {
        private Mock<IInputChannel> _channelMock;
        private Mock<PulseOutputItems.ChannelItems> _itemsMock;
        private TestSchedulers _schedulers;


        //[TestMethod()]
        //public void PulseChannelListenerTest()
        //{
        //    var scheduler = new TestSchedulers();

        //    var pulseValue = 255;
        //    var random = Observable
        //        .Generate(255, i => true, i => i == 255 ? 0 : 255, i => i)
        //        .Subscribe(x => pulseValue = x);

        //    _channelMock.Setup(x => x.GetValue())
        //        .Callback(() => pulseValue = 255 - pulseValue)
        //        .Returns(() => pulseValue);

        //    var listener = new PulseChannelListener(_itemsMock.Object, _channelMock.Object);

        //    listener.PulseListener.Connect()
        //        .Bind(out var pulses)
        //        .Subscribe();

        //    scheduler.ThreadPool.Schedule(() => listener.Start().Subscribe());
        //    scheduler.ThreadPool.AdvanceBySeconds(4);
        //    //var result = scheduler.ThreadPool.Start(() => listener.Start(), 0, 0, TimeSpan.FromSeconds(2).Ticks);

        //    scheduler.Dispatcher.AdvanceByMilliSeconds(600);
        //    Assert.IsTrue(pulses.Count > 0);
        //}

 
        [TestInitialize]
        public void Init()
        {
            _itemsMock = new Mock<PulseOutputItems.ChannelItems>();
            _channelMock = new Mock<IInputChannel>();
            _schedulers = new TestSchedulers();

            //_channelMock.SetupSequence(i => i.GetValue())
            //               .Returns(255).Returns(255)
            //               .Returns(0).Returns(0) // 1 Pulse
            //               .Returns(255)
            //               .Returns(0) // 2
            //               .Returns(255).Returns(255)
            //               .Returns(0).Returns(0) //3
            //               .Returns(255).Returns(255);
        }

        //[TestMethod]
        //public void PulseChannelListenerTest()
        //{
        //    var scheduler = new TestSchedulers();

        //    var pulseValue = 255;
        //    var random = Observable
        //        .Generate(255, i => true, i => i == 255 ? 0 : 255, i => i)
        //        .Subscribe(x => pulseValue = x);

        //    _channelMock.Setup(x => x.GetValue())
        //        .Callback(() => pulseValue = 255 - pulseValue)
        //        .Returns(() => pulseValue);

        //    var listener = new PulseChannelListener(_itemsMock.Object, _channelMock.Object);

        //    listener.PulseListener.Connect()
        //        .Bind(out var pulses)
        //        .Subscribe();

        //    scheduler.ThreadPool.Schedule(() => listener.Start().Subscribe());
        //    scheduler.ThreadPool.AdvanceBySeconds(4);
        //    //var result = scheduler.ThreadPool.Start(() => listener.Start(), 0, 0, TimeSpan.FromSeconds(2).Ticks);

        //    scheduler.Dispatcher.AdvanceByMilliSeconds(600);
        //    Assert.IsTrue(pulses.Count > 0);
        //}
    }
}