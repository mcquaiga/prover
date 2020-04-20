using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Devices.Core.Items.ItemGroups;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Application.Hardware;
using Prover.Application.Services;
using Prover.Shared;
using Prover.Shared.Interfaces;
using Tests.Shared;

namespace Tests.Application.ExternalDevices.DInOutBoards
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
        private static TestSchedulers _schedulers;

        [TestMethod]
        public void ConstantIntervalSimulatorInputChannelTest()
        {
            var simulator = new SimulatedInputChannel(PulseOutputChannel.Channel_A, null, null, _schedulers.TaskPool);
            
            var changes = 0;
            var pulses = 0;

            simulator.GetRandomSimulator(1000)
                .ObserveOn(_schedulers.Dispatcher)
                .TimeInterval(_schedulers.Dispatcher)
                .Do(x => Debug.WriteLine($"{simulator.Channel} - {x.Value} - {x.Interval.Milliseconds} ms"))
                .Do(value =>
                {
                    //var simValue = simulator.GetValue();
                    //Assert.IsTrue(simValue == value);
                    changes++;
                    if (value.Value != simulator.OffValue)
                    {
                        pulses++;
                        Debug.WriteLine($"Total = {pulses}");
                    }
                })
                .Subscribe();

            //do
            //{
            //    _schedulers.TaskPool.AdvanceByMilliSeconds(1000);
            //    _schedulers.Dispatcher.AdvanceByMilliSeconds(1000);
            //    //changes++;
            //} while (changes < 40);

            _schedulers.TaskPool.AdvanceBySeconds(20);
            _schedulers.Dispatcher.AdvanceBySeconds(20);

            Assert.IsTrue(changes > 20);
            Assert.IsTrue(pulses == 20);
        }

        [TestInitialize]
        public void Init()
        {
            _pulseOutputItems = new PulseOutputItems();

            _channelAItems = new PulseOutputItems.ChannelItems
            {
                Name = PulseOutputChannel.Channel_A,
                ChannelType = PulseOutputType.UncVol,
                Count = 0,
                Scaling = 62.5m
            };

            _channelBItems = new PulseOutputItems.ChannelItems
            {
                Name = PulseOutputChannel.Channel_B,
                ChannelType = PulseOutputType.CorVol,
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

            var service = new PulseOutputsListenerService(null, _channelMockFactory.Object, _schedulers.TaskPool);
            service.Initialize(_pulseOutputItems);
            //var listener = service.PulseCountUpdates.Connect()
            //    .Bind(out var pulses)
            //    .Subscribe();

            var pulses = 0;
            service.PulseCountUpdates.Subscribe(p => pulses = p.PulseCount);



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
            //Assert.IsTrue(pulses.Count == 1);

            _schedulers.TaskPool.AdvanceByMilliSeconds(60.5);
            Assert.IsTrue(pulses == 0); // 61.5 ms  Off

            _schedulers.TaskPool.AdvanceByMilliSeconds(61.5);
            Assert.IsTrue(pulses== 1); // 123 ms  On

            _schedulers.TaskPool.AdvanceByMilliSeconds(3);
            Assert.IsTrue(pulses == 1); // 126 ms Off

            _schedulers.TaskPool.AdvanceByMilliSeconds(62.5);
            Assert.IsTrue(pulses == 2); // 188.5 On

            _schedulers.TaskPool.AdvanceByMilliSeconds(186.5);

            _schedulers.Dispatcher.AdvanceBySeconds(1);

            Assert.IsTrue(pulses == 3);
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

            var service = new PulseOutputsListenerService(null, _channelMockFactory.Object, _schedulers.TaskPool);
            service.Initialize(_pulseOutputItems);

            var countA = 0;
            service.PulseCountUpdates.Where(p => p.Channel == PulseOutputChannel.Channel_A).Subscribe(c => countA = c.PulseCount);

            var countB = 0;
            service.PulseCountUpdates.Where(p => p.Channel == PulseOutputChannel.Channel_B).Subscribe(c => countB = c.PulseCount);

            //service.PulseCountUpdates.Connect()
            //    .Bind(out var pulses)
            //    .Subscribe();

            service.StartListening();
            //Assert.IsTrue(pulses.Count == 2); // Assert Pulse Output channel objects are initialized
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

            Assert.IsTrue(countA == expected);
            Assert.IsTrue(countB == expected);
        }

        [TestMethod]
        public void RandomIntervalSimulatorInputChannelTest()
        {
            var simulator = new SimulatedInputChannel(PulseOutputChannel.Channel_A, null, null, _schedulers.TaskPool);
            
            var changes = 0;
            var pulses = 0;

            simulator.GetRandomSimulator()
                .ObserveOn(_schedulers.Dispatcher)
                .TimeInterval(_schedulers.Dispatcher)
                .Do(x => Debug.WriteLine($"{simulator.Channel} - {x.Value} - {x.Interval.Milliseconds} ms"))
                .Do(value =>
                {
                    changes++;
                    if (value.Value != simulator.OffValue)
                    {
                        pulses++;
                        Debug.WriteLine($"Total = {pulses}");
                    }
                })
                .Subscribe();

            //var result = _schedulers.TaskPool.Start(() => random);

            //_schedulers.TaskPool.AdvanceBySeconds(15);
            ////_schedulers.TaskPool.AdvanceBySeconds(5);
            //_schedulers.Dispatcher.AdvanceBySeconds(15);

            do
            {
                _schedulers.TaskPool.AdvanceByMilliSeconds(25);
                _schedulers.Dispatcher.AdvanceByMilliSeconds(25);
            } while (pulses < 10);

            //Assert.IsTrue(result.Messages.Count == 10);
            Assert.IsTrue(changes > 0);
            Assert.IsTrue(pulses > 0);
        }

        [TestMethod]
        public void TwoInputChannelsStartedByOutputChannel()
        {
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