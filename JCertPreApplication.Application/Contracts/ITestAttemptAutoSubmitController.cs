using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
