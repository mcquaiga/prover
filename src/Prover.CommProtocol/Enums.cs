using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol
{
    public enum InstrumentTypeCode
    {
        Mini = 2,
        ECAT = 0,
        MiniAT = 3,
        MiniMax = 4,
        AccuTest = 21,
        TurboCor = 20,
        TurboMon = 6,
        PulseAcc = 8,
        TCI = 12,
        EC300 = 13,
        ER = 1,
        ERX = 10,
        PTBoard = 5,
        AutoDetect = 99
    }

    public enum BaudRate
    {
        b300 = 300,
        b600 = 600,
        b1200 = 1200,
        b2400 = 2400,
        b4800 = 4800,
        b9600 = 9600,
        b19200 = 19200,
        b38400 = 38400
    }

    public enum CommStates
    {
        UnlinkedIdle,
        IncomingAwaitingSITE,
        IncomingConnected,
        WakingItUp,
        SigningOn,
        LinkedIdle,
        ReadingItem,
        ReadingGroup,
        ReadingFirstLog,
        ReadingMoreLogs,
        WritingItem,
        SigningOff,
        InitializingModem,
        Listening,
        SendingClearAlarmsMessage,
        Dialing,
        HangingUp
    }

    internal enum MessageState
    {
        OK_Idle,
        WaitingForReceiveEvent,
        ReceivedAPacket,
        ProtocolError,
        FramingError,
        ReceivedERROR,
        OtherError
    }

    internal enum CommChar
    {
        SOH = 1,
        STX = 2,
        ETX = 3,
        EOT = 4,
        ENQ = 5,
        ACK = 6,
        CR = 13,
        NAK = 21,
        RS = 30
    }

    internal enum InstrumentErrors
    {
        NoData = -1,
        NoError = 0,
        MessageFormatError = 1,
        SignOnError = 20,  //Wrong Instrument Type
        TimeoutError = 21, //Timeout Error
        FramingError = 22, //Framing Error
        CheckByteError = 23, //Check byte error
        InvalidAccessCodeError = 27, //Invalid Access Code Error
        InvalidCommandError = 28, //Invalid Command Error
        InvalidItemNumberError = 29, //Invalid Item Number
        InvalidEnquiryError = 30, //Invalid Enquiry
        TooManyRetransmissionsError = 31, //Too many retransmissions
        ReadOnlyError = 32, //Attempted access is disabled. item is read-only
        NoAuditTrailError = 33, //No audit trail
        EventLoggerFullError = 34, //Event logger full
        InvalidAGA8Condition = 35,
        InvalidDataError = 38,
        InvalidChangeAttempted = 39,
        InsufficientPower = 40,
        REIError = 41
    }

    internal enum AlarmValues
    {
        Alarm = 11111111,
        NoAlarm = 0
    }
}
