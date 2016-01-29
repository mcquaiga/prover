using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol
{
    public enum InstrumentType
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

    public enum CommState
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

    public enum CommChar
    {
        SOH = (char)1,
        STX = (char)2,
        ETX = (char)3,
        EOT = (char)4,
        ENQ = (char)5,
        ACK = (char)6,
        CR = (char)13,
        NAK = (char)21,
        RS = (char)30
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
