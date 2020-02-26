using Microsoft.VisualStudio.TestTools.UnitTesting;
using Application.ExternalDevices.DInOutBoards;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Devices.Core.Items.ItemGroups;
using DynamicData;
using Microsoft.Reactive.Testing;
using Moq;
using Prover.Application.ExternalDevices.DInOutBoards;

namespace Application.ExternalDevices.DInOutBoards.Tests
{

    public sealed class TestSchedulers : ISchedulerProvider
    {
        private readonly TestScheduler _currentThread = new TestScheduler();
        private readonly TestScheduler _dispatcher = new TestScheduler();
        private readonly TestScheduler _immediate = new TestScheduler();
        private readonly TestScheduler _newThread = new TestScheduler();
        private readonly TestScheduler _threadPool = new TestScheduler();
        #region Explicit implementation of ISchedulerService
        IScheduler ISchedulerProvider.CurrentThread { get { return _currentThread; } }
        IScheduler ISchedulerProvider.Dispatcher { get { return _dispatcher; } }
        IScheduler ISchedulerProvider.Immediate { get { return _immediate; } }
        IScheduler ISchedulerProvider.NewThread { get { return _newThread; } }
        IScheduler ISchedulerProvider.ThreadPool { get { return _threadPool; } }
        #endregion
        public TestScheduler CurrentThread { get { return _currentThread; } }
        public TestScheduler Dispatcher { get { return _dispatcher; } }
        public TestScheduler Immediate { get { return _immediate; } }
        public TestScheduler NewThread { get { return _newThread; } }
        public TestScheduler ThreadPool { get { return _threadPool; } }
    }

    public sealed class ImmediateSchedulers : ISchedulerProvider
    {
        public IScheduler CurrentThread { get { return Scheduler.Immediate; } }
        public IScheduler Dispatcher { get { return Scheduler.Immediate; } }
        public IScheduler Immediate { get { return Scheduler.Immediate; } }
        public IScheduler NewThread { get { return Scheduler.Immediate; } }
        public IScheduler ThreadPool { get { return Scheduler.Immediate; } }
    }

    [TestClass()]
    public class PulseChannelListenerTests
    {
        private Mock<PulseOutputItems.ChannelItems> _itemsMock;
        private Mock<IInputChannel> _channelMock;
        private TestSchedulers _schedulers;

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

        [TestMethod()]
        public void PulseChannelListenerTest()
        {
            var scheduler = new TestSchedulers();

            var pulseValue = 255;
            var random = Observable
                .Generate(255, i => true, i => i == 255 ? 0 : 255, i => i)
                .Subscribe(x => pulseValue = x);

            _channelMock.Setup(x => x.GetValue())
                .Callback(() => pulseValue = 255 - pulseValue)
                .Returns(() => pulseValue);

            var listener = new PulseChannelListener(_itemsMock.Object, _channelMock.Object, scheduler);
            
            listener.PulseListener.Connect()
                .Bind(out var pulses)
                .Subscribe();

            scheduler.ThreadPool.Schedule(() => listener.Start().Subscribe());
            scheduler.ThreadPool.AdvanceBySeconds(4);
            //var result = scheduler.ThreadPool.Start(() => listener.Start(), 0, 0, TimeSpan.FromSeconds(2).Ticks);
            
            scheduler.Dispatcher.AdvanceByMilliSeconds(600);
            Assert.IsTrue(pulses.Count > 0);
        }

        [TestMethod()]
        public void ConnectTest()
        {
            Assert.Fail();
        }
    }

    public static class TextEx
    {

        public static void AdvanceBySeconds(this TestScheduler source, int seconds)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            source.AdvanceBy(TimeSpan.FromSeconds(seconds).Ticks);
        }

        public static void AdvanceByMilliSeconds(this TestScheduler source, int seconds)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            source.AdvanceBy(TimeSpan.FromMilliseconds(seconds).Ticks);
        }

     
    }
}