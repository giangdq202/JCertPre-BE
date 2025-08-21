
namespace JCertPreApplication.Application.Contracts
{
    public interface ITestAttemptAutoSubmitController
    {
        void StartMonitoring();
        void StopMonitoring();
        bool IsRunning { get; }
        void AddAttempt(Guid attemptId, DateTime endTime);
    }
}
