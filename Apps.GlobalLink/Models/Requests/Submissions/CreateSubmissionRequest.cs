using Apps.GlobalLink.Handlers;
using Apps.GlobalLink.Handlers.Static;
using Apps.GlobalLink.Models.Requests.Projects;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class CreateSubmissionRequest : ProjectRequest
{
     [Display("Name")]
     public string Name { get; set; } = string.Empty;

     [Display("Source language"), DataSource(typeof(SourceLanguageDataHandler))]
     public string SourceLanguage { get; set; } = string.Empty;

     [Display("Targaet languages"), DataSource(typeof(TargetLanguageDataHandler))]
     public IEnumerable<string> TargetLanguages { get; set; } = new List<string>();

     public string? Instructions { get; set; }

     public string? Background { get; set; }

     [Display("Workflow ID"), DataSource(typeof(WorkflowDataHandler))]
     public string? WorkflowId { get; set; }

     [Display("Target format", Description = "By default we will use 'NON_PARSABLE' parameter"), StaticDataSource(typeof(TargetFormatStaticDataHandler))]
     public string? TargetFormat { get; set; } = "NON_PARSABLE";

     [Display("Batch name", Description = "By default we will use 'Batch1' parameter")]
     public string? BatchName { get; set; } = "Batch1";

     [Display("Claim scope", Description = "By deault we will use 'LANGUAGE' parameter"), StaticDataSource(typeof(ClaimScopeStaticDataHandler))]
     public string? ClaimScope { get; set; } = "LANGUAGE";

     [Display("Due date")]
     public DateTime DueDate { get; set; }

     [Display("Webhook URL", Description = "Webhook URL for callback notifications")]
     public string? WebhookUrl { get; set; }

     [Display("Webhook scopes"), StaticDataSource(typeof(WebhookScopeStaticDataHandler))]
     public IEnumerable<string>? WebhookScopes { get; set; }

     public void ThrowIfInvalid()
     {
          if (string.IsNullOrEmpty(Name))
          {
               throw new PluginMisconfigurationException("'Name' input is either empty or null. Please provide a valid name.");
          }

          if (string.IsNullOrEmpty(ProjectId))
          {
               throw new PluginMisconfigurationException("'Project ID' input is either empty or null. Please provide a valid project ID.");
          }

          if (string.IsNullOrEmpty(SourceLanguage))
          {
               throw new PluginMisconfigurationException("'Source language' input is either empty or null. Please provide a valid source language.");
          }

          if (TargetLanguages == null || !TargetLanguages.Any())
          {
               throw new PluginMisconfigurationException("'Target languages' input is either empty or null. Please provide a valid target language.");
          }

          if (!string.IsNullOrEmpty(WebhookUrl) && WebhookScopes == null)
          {
               throw new PluginMisconfigurationException("If 'Webhook URL' is provided, 'Webhook scopes' must also be provided, but it is null. Please provide a valid Webhook scopes.");
          }
     }
}
