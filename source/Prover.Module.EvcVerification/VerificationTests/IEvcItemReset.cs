using System.Threading.Tasks;

namespace Module.EvcVerification.VerificationTests
{
    public interface IPostTestCommand
    {
        Task Execute(EvcCommunicationClient commClient);
    }

    public interface IPreTestCommand
    {
        Task Execute(EvcCommunicationClient commClient);
    }
}