using Elsa.Activities.Workflows.Extensions;
using Elsa.Models;
using Elsa.Samples.UserRegistration.Web.Models;
using Elsa.Services;
using Elsa.Sample.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Elsa.Sample.Business
{

    public interface IUserBusiness
    {
        Task UserRegistration(RegistrationModel request);
    }
    public class UserBusiness : IUserBusiness
    {
        private readonly ILogger<UserBusiness> _logger;
        private readonly IWorkflowInvoker _workflowInvoker;

        public UserBusiness(
            ILogger<UserBusiness> logger,
            IWorkflowInvoker workflowInvoker)
        {
            _logger = logger;
            _workflowInvoker = workflowInvoker;
        }

        public async Task UserRegistration(RegistrationModel request)
        {
            _logger.LogInformation("Triggering RegisterUser signal...");

            
            var input = new Variables();
            input.SetVariable("RegistrationModel", request);

            
            await _workflowInvoker.TriggerSignalAsync("RegisterUser", input);
        }
    }
}
