declare @instrId uniqueidentifier = newid()
declare @data varchar(max) = '{"0":" 0000000","2":" 0000002","5":" 0000000","6":" 0000000","8":"    0.03","10":"99999.98","11":"   -1.00","12":"  0.0000","13":" 14.9500","14":" 14.7300","26":"  -40.00","27":"  -35.00","28":"  145.00","34":"   60.00","35":"  0.0893","44":"  0.0021","45":"  1.2383","47":"  1.0000","53":"  0.5300","54":"  1.7500","55":"  0.7230","56":"  2.0000","57":"  2.0000","58":"  3.4013","62":"01084240","87":"       1","89":"       0","90":"       7","92":"       4","93":"       0","94":"       2","95":"! Unsupported","98":"      14","109":"       1","110":"       1","111":"       0","112":"       1","113":"  0.0000","122":"  2.9032","127":"       4","137":"  100.00","140":" 0000000","141":"       0","142":" 1000.00","147":"       0","200":"01084240","201":"02547086","203":"15 40 01","204":"17-12-07","262":"       2","432":"       2","439":".0222222","892":"  2.0000"}'

declare @clientId uniqueidentifier = '564B2281-B217-40AA-8C94-1DDCEAB43D70'

INSERT [Instruments]([Id], [TestDateTime], [ArchivedDateTime], [Type], [CertificateId], [ClientId], [EmployeeId], [JobId], [ExportedDateTime], [EventLogPassed], [CommPortsPassed], [InstrumentData])
VALUES (@instrId, GETDATE(), NULL, 4, NULL, @clientId, NULL, NULL, NULL, NULL, NULL, @data)


declare @vtId uniqueidentifier = newid()
INSERT [VerificationTests]([Id], [TestNumber], [InstrumentId])
VALUES (@vtId, 0, @instrId)

INSERT [TemperatureTests]([Id], [Gauge], [VerificationTestId], [InstrumentData])
VALUES (@vtId, 32, @vtId, '{"26":"  -40.00","35":"  0.0893","45":"  1.2383"}' )

INSERT [VolumeTests]([Id], [PulseACount], [PulseBCount], [AppliedInput], [TestInstrumentData], [DriveTypeDiscriminator], [VerificationTestId], [InstrumentData])
VALUES (@vtId, 10, 10, 100, '{"0":" 0000050","2":" 0000052","113":"  0.0000","892":"  2.0000"}', 'Rotary', @vtId, '{"0":" 0000000","2":" 0000002","113":"  0.0000","892":"  2.0000"}' )


set @vtId = newid();

INSERT [VerificationTests]([Id], [TestNumber], [InstrumentId])
VALUES (@vtId, 1, @instrId)

INSERT [TemperatureTests]([Id], [Gauge], [VerificationTestId], [InstrumentData])
VALUES (@vtId, 60, @vtId, '{"26":"  -40.00","35":"  0.0893","45":"  1.2383"}')


set @vtId = newid()
INSERT [VerificationTests]([Id], [TestNumber], [InstrumentId])
VALUES (@vtId, 2, @instrId)


INSERT [TemperatureTests]([Id], [Gauge], [VerificationTestId], [InstrumentData])
VALUES (@vtId, 90, @vtId, '{"26":"  -40.00","35":"  0.0893","45":"  1.2383"}')



