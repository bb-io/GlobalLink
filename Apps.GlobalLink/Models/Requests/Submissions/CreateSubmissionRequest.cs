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
     [Display("Project ID"), DataSource(typeof(ProjectDataHandler))]
     public override string ProjectId { get; set; } = string.Empty;

     [Display("Name")]
     public string Name { get; set; } = string.Empty;

     [Display("Source language"), DataSource(typeof(SourceLanguageDataHandler))]
     public string SourceLanguage { get; set; } = string.Empty;

     [Display("Target languages"), DataSource(typeof(TargetLanguageDataHandler))]
     public IEnumerable<string> TargetLanguages { get; set; } = new List<string>();

     [Display("Instructions")]
     public string? Instructions { get; set; }

     public string? Background { get; set; }

     [Display("Workflow ID"), DataSource(typeof(WorkflowDataHandler))]
     public string? WorkflowId { get; set; }

     [Display("Target format", Description = "By default we will use 'TXLF' parameter"), StaticDataSource(typeof(TargetFormatStaticDataHandler))]
     public string? TargetFormat { get; set; } = "TXLF";

     [Display("Batch name", Description = "By default we will use 'Batch1' parameter")]
     public string? BatchName { get; set; } = "Batch1";

     [Display("Claim scope", Description = "By default we will use 'Language' parameter"), StaticDataSource(typeof(ClaimScopeStaticDataHandler))]
     public string? ClaimScope { get; set; } = "LANGUAGE";

     [Display("Due date")]
     public DateTime DueDate { get; set; }

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
     }
}
