namespace Prover.Core.Startup
{
    internal interface ISettings
    {
        string CommName { get; set; }
        string BaudRate { get; set; }
    }
}