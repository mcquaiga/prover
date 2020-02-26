using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Application.ExternalDevices.DInOutBoards.Tests;
using Devices.Core.Items.ItemGroups;
using DynamicData;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Application.Hardware;
using Prover.Application.Services;
using Prover.Shared;
using Prover.Shared.Interfaces;
using Tests.Application;

namespace Prover.Application.ExternalDevices.DInOutBoards.Tests
{
    [TestClass]
    public class PulseInputsListenerServiceTests
    {
        private PulseOutputItems.ChannelItems _channelAItems;
        private Mock<IInputChannel> _channelAMock;
        private PulseOutputItems.ChannelItems _channelBItems;
        private Mock<IInputChannel> _channelBMock;
        private Mock<IInputChannelFactory> _channelMockFactory;
        private PulseOutputItems _pulseOutputItems;
        private TestSchedulers _schedulers;

        [TestInitialize]
        public void Init()
        {
            _pulseOutputItems = new PulseOutputItems();

            _channelAItems = new PulseOutputItems.ChannelItems
            {
                Name = PulseOutputChannel.Channel_A,
                Units = PulseOutputUnitType.UncVol,
                Count = 0,
                Scaling = 62.5m
            };

            _channelBItems = new PulseOutputItems.ChannelItems
            {
                Name = PulseOutputChannel.Channel_B,
                Units = PulseOutputUnitType.CorVol,
                Count = 0,
                Scaling = 62.5m
            };

            _channelAMock = new Mock<IInputChannel>();
            _channelBMock = new Mock<IInputChannel>();

            _channelMockFactory = new Mock<IInputChannelFactory>();

            _channelMockFactory.Setup(factory => factory.CreateInputChannel(PulseOutputChannel.Channel_A))
                .Returns(_channelAMock.Object);

            _channelMockFactory.Setup(factory => factory.CreateInputChannel(PulseOutputChannel.Channel_B))
                .Returns(_channelBMock.Object);

            _schedulers = new TestSchedulers();
        }

        [TestMethod]
        public void ListenForPulsesOnOneChannel()
        {
            _pulseOutputItems.Channels.Add(_channelAItems);

            StartPulseGenerator(_channelAMock, _schedulers.TaskPool);

            var service = new PulseInputsListenerService(null, _channelMockFactory.Object, _schedulers.TaskPool);

            var listener = service.PulseListener.Connect()
                .Bind(out var pulses)
                .Subscribe();

            service.StartListening();

            /*            
             *   off      on        off        on         off         on
             *        __________           __________            ___________
             *       |          |         |          |          |           |    
             *  _____|          |_________|          |__________|           |
             *
             *  0   62.5       125      187.5       250        312.5       375   
             */


            _schedulers.TaskPool.AdvanceByMilliSeconds(1);
            Assert.IsTrue(pulses.Count == 1);

            _schedulers.TaskPool.AdvanceByMilliSeconds(60.5);
            Assert.IsTrue(pulses[0].TotalPulses == 0); // 61.5 ms  Off

            _schedulers.TaskPool.AdvanceByMilliSeconds(61.5);
            Assert.IsTrue(pulses[0].TotalPulses == 1); // 123 ms  On

            _schedulers.TaskPool.AdvanceByMilliSeconds(3);
            Assert.IsTrue(pulses[0].TotalPulses == 1); // 126 ms Off

            _schedulers.TaskPool.AdvanceByMilliSeconds(62.5);
            Assert.IsTrue(pulses[0].TotalPulses == 2); // 188.5 On

            _schedulers.TaskPool.AdvanceByMilliSeconds(186.5);

            _schedulers.Dispatcher.AdvanceBySeconds(1);

            Assert.IsTrue(pulses[0].TotalPulses == 3);
        }

        [TestMethod] 
        public void RandomIntervalSimulatorInputChannelTest()
        {
            var simulator = new SimulatedInputChannel(PulseOutputChannel.Channel_A, _schedulers.TaskPool);

            var value = 0;
            var changes = 0;
            var pulses = 0;

            simulator.GetRandomSimulator()
                //.Do(v => Debug.WriteLine($"Pulse Received {v}"))
                .ObserveOn(_schedulers.Dispatcher)
                .Subscribe(v =>
                {
                    value = v;
                    changes++;
                    if (value != simulator.OffValue)
                    {
                        pulses++;
                        Debug.WriteLine($"Total = {pulses}");
                    }

                    Assert.IsTrue(simulator.GetValue() == value);
                });

            //_schedulers.TaskPool.AdvanceBySeconds(5);
            _schedulers.TaskPool.Start();
            
            do
            {
                _schedulers.Dispatcher.AdvanceByMilliSeconds(60);
            } while (pulses < 10);
            Assert.IsTrue(changes > 0);
            Assert.IsTrue(pulses == 10);
        }

        [TestMethod] 
        public void ConstantIntervalSimulatorInputChannelTest()
        {
            var myThread = _schedulers.TaskPool;

            var simulator = new SimulatedInputChannel(PulseOutputChannel.Channel_A, myThread);

            var value = 0;
            var changes = 0;
            var pulses = 0;

            simulator.GetRandomSimulator(1000)
                //.Do(v => Debug.WriteLine($"Pulse Received {v}")
                .ObserveOn(_schedulers.Dispatcher)
                //.SubscribeOn(_schedulers.TaskPool)
                .Subscribe(v =>
                {
                    value = v;
                    changes++;
                    if (value != simulator.OffValue)
                    {
                        pulses++;
                        Debug.WriteLine($"Total = {pulses}");
                    }

                    Assert.IsTrue(simulator.GetValue() == value);
                });

            //_schedulers.TaskPool.AdvanceBySeconds(5);
            myThread.Start();
            do
            {
                _schedulers.Dispatcher.AdvanceByMilliSeconds(60);
            } while (pulses < 10);
            Assert.IsTrue(changes > 0);
            Assert.IsTrue(pulses == 10);
        }

        [TestMethod]
        public void TwoInputChannelsStartedByOutputChannel()
        {

        }

        [TestMethod]
        public void ListenToTwoPulseOutChannels()
        {
            var testTimeMs = 5000;
            var expected = testTimeMs / 125;

            StartPulseGenerator(_channelAMock, _schedulers.TaskPool);
            StartPulseGenerator(_channelBMock, _schedulers.TaskPool);

            _pulseOutputItems.Channels.Add(_channelAItems);
            _pulseOutputItems.Channels.Add(_channelBItems);

            var service = new PulseInputsListenerService(null, _channelMockFactory.Object, _schedulers.TaskPool);

            var listener = service.PulseListener.Connect()
                .Bind(out var pulses)
                .Subscribe();

            service.StartListening();
            Assert.IsTrue(pulses.Count == 2);
            /*            
             *   off      on        off        on         off         on
             *        __________           __________            ___________
             *       |          |         |          |          |           |    
             *  _____|          |_________|          |__________|           |
             *
             *  0   62.5       125      187.5       250        312.5       375   
             */
            _schedulers.TaskPool.AdvanceByMilliSeconds(testTimeMs);
            _schedulers.Dispatcher.AdvanceBySeconds(1);

            Assert.IsTrue(pulses[0].TotalPulses == expected);
            Assert.IsTrue(pulses[1].TotalPulses == expected);
        }


        private void StartPulseGenerator(Mock<IInputChannel> channelMock, TestScheduler scheduler,
            TimeSpan? delayTimeSpan = null)
        {
            var offValue = 255;
            var pulseValue = offValue;

            channelMock.Setup(x => x.GetValue())
                .Returns(() => pulseValue);

            Observable.Interval(TimeSpan.FromMilliseconds(62.5), scheduler)
                .Subscribe(x =>
                {
                    pulseValue = offValue - pulseValue;
                    Debug.WriteLine($"Generator {pulseValue} at {scheduler.Clock.Milliseconds().Milliseconds} ms");
                });
        }
    }
}

//[TestMethod]
//public void DaqPulseOutChannels()
//{
//    var testTimeMs = 10000;
//    var expected = testTimeMs / 125;

//    _pulseOutputItems.Channels.Add(_channelBItems);

//    var channelFactory = new DaqBoardChannelFactory(null);

//    var service = new PulseInputsListenerService(null, channelFactory, _schedulers.TaskPool);

//    var listener = service.PulseListener.Connect()
//        .Bind(out var pulses)
//        .Subscribe();

//    service.StartListening(_pulseOutputItems);
//    Assert.IsTrue(pulses.Count == 1);
//    /*            
//     *   off      on        off        on         off         on
//     *        __________           __________            ___________
//     *       |          |         |          |          |           |    
//     *  _____|          |_________|          |__________|           |
//     *
//     *  0   62.5       125      187.5       250        312.5       375   
//     */
//    _schedulers.TaskPool.AdvanceByMilliSeconds(testTimeMs);
//    _schedulers.Dispatcher.AdvanceBySeconds(1);

//    Assert.IsTrue(pulses[0].TotalPulses == expected);
//    Assert.IsTrue(pulses[1].TotalPulses == expected);
//}