namespace Devices.Communications.Messaging
{
    public abstract class ResponseCode
    {
        #region Public Properties

        public int Code { get; }

        #endregion Public Properties

        #region Protected Constructors

        protected ResponseCode(int code)
        {
            Code = code;
        }

        #endregion Protected Constructors
    }
}