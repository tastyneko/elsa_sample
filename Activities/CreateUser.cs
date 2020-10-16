using System.Threading;
using System.Threading.Tasks;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Extensions;
using Elsa.Results;
using Elsa.Samples.UserRegistration.Web.Models;
using Elsa.Services;
using Elsa.Services.Models;
using Elsa.Sample.Models;
using MongoDB.Driver;

namespace Elsa.Samples.UserRegistration.Web.Activities
{
    [ActivityDefinition(Category = "Users", Description = "Create a User", Icon = "fas fa-user-plus", Outcomes = new[] { OutcomeNames.Done })]
    public class CreateUser : Activity
    {
        private readonly IMongoCollection<User> _store;
        private readonly IIdGenerator _idGenerator;


        public CreateUser(
            IMongoCollection<User> store,
            IIdGenerator idGenerator)
        {
            _store = store;
            _idGenerator = idGenerator;
        }

        [ActivityProperty(Hint = "Enter an expression that evaluates to the name of the user to create.")]
        public WorkflowExpression<string> UserName
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }

        [ActivityProperty(Hint = "Enter an expression that evaluates to the email address of the user to create.")]
        public WorkflowExpression<string> Email
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }



        protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
        {


            var user = new User
            {
                Id = _idGenerator.Generate(),
                Name = await context.EvaluateAsync(UserName, cancellationToken),
                Email = await context.EvaluateAsync(Email, cancellationToken)
            };

            await _store.InsertOneAsync(user, cancellationToken: cancellationToken);

            Output.SetVariable("User", user);
            return Done();
        }
    }
}