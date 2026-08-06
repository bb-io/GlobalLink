using Apps.GlobalLink.Constants;
using Apps.GlobalLink.Models.Requests.Submissions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.GlobalLink.Events.Webhooks.Handlers;

public class SubmissionCompletedHandler(
    InvocationContext invocationContext,
    [WebhookParameter(true)] SubmissionRequest submissionRequest)
    : SubmissionWebhookHandler(invocationContext, ScopeConstants.SubmissionCompleted,
        SubmissionStatusConstants.Processed, submissionRequest)
{ }
