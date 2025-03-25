# Blackbird.io GlobalLink Enterprise

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

GlobalLink is a cloud-based, vendor-agnostic technology platform that optimizes translation management for Enterprise and SMB companies. It helps users in delivering and maintaining multilingual websites quickly and affordably. GlobalLink provides a comprehensive solution for creating, managing, and delivering multilingual content across various platforms.

With the `GlobalLink Enterprise` app for Blackbird, you can automate your translation workflows by creating submissions, uploading files for translation, monitoring submission statuses, downloading translated content, and integrating these operations with other business processes.

## Before setting up

Before you can connect to `GlobalLink Enterprise` through Blackbird, you need to make sure that:

- You have a GlobalLink account with appropriate access permissions
- You know your GlobalLink API endpoint URL
- You have a valid username and password for authentication
- You have a Basic Auth Token for API access

## Connecting

1. Navigate to apps and search for **GlobalLink Enterprise**
2. Click _Add Connection_
3. Name your connection for future reference e.g., 'My GlobalLink'
4. Fill in the following fields:
   - **Base URL**: Your GlobalLink API endpoint (e.g., `https://api.globallink.com`)
   - **Username**: Your GlobalLink username
   - **Password**: Your GlobalLink password
   - **Basic auth token**: Your GlobalLink API Basic Auth token
5. Click _Connect_
6. Confirm that the connection has appeared and the status is _Connected_

![connection](image/README/connection.png)

## Actions

### Submission management

- **Create submission**: Creates a new translation submission with specified source and target languages, project details, due date, and optionally configures webhooks for status notifications.
- **Get submission**: Retrieves detailed information about a specific submission using its ID.
- **Start submission**: First analyzes and then starts a submission, initiating the translation workflow process.
- **Claim submission**: Claims a submission for processing at a specific phase.

> Note: When creating a submission, you can configure webhook notifications to receive updates when the submission is completed, cancelled, or analyzed. This is useful for triggering automated workflows when translations reach specific states.

### File management

- **Upload source file**: Uploads a source file to a submission for translation.
- **Upload reference file**: Uploads a reference file to a submission on submission level. Reference files provide context for translators.
- **Upload target file**: Uploads a translated file to a submission and waits for the process to finish successfully. 

  > Important: The file name must not be modified in any way. It is encoded in the filename and is essential for the upload process to work correctly.

- **Download source files**: Downloads source files from a submission at a specific phase.
- **Download target files**: Downloads translated files from a completed submission.

## Events

GlobalLink provides webhook-based event notifications that can be used to trigger workflows in Blackbird:

- **On submission callback received**: Triggered when a submission callback is received from GlobalLink. This event can be configured to listen for specific state changes:
  - `submission.completed`: Triggered when a submission is fully completed
  - `submission.cancelled`: Triggered when a submission is cancelled
  - `submission.analyzed`: Triggered when a submission analysis is completed

> Note: These webhook events may not be received immediately after the callback and could be slightly delayed.

## Translation workflow automation

A typical **GlobalLink Enterprise** translation workflow in Blackbird might include:

1. Creating a submission with appropriate source and target languages
2. Uploading source content files to the submission
3. Optionally adding reference materials to assist translators
4. Starting the submission to begin the translation process
5. Claiming the submission for processing
6. Using webhooks to monitor the submission status

For effective automation, you'll typically need to create two additional birds with the **On submission callback received** event:

- **First bird**: Triggered by `submission.analyzed`
  - Download source files
  - Translate them using your preferred translation app (e.g., DeepL)
  - Upload the translated files back to the submission

- **Second bird**: Triggered by `submission.completed`
  - Download the finalized translated files
  - Process them as needed (e.g., upload to a CMS, notify stakeholders)

This workflow allows for fully automated end-to-end translation processing with minimal manual intervention.

## Example

Here's a simple example of how to set up a translation workflow with `GlobalLink Enterprise` and `Contentful` CMS:

### Bird 1: Submission creation and source upload
![Submission creation and upload](image/README/bird_1-1.png)
![Uploading files](image/README/bird_1-2.png)

This bird handles creating a new submission and uploading the source files for translation.

### Bird 2: Translation processing
![Translation process with DeepL](image/README/bird_2-1.png)

This bird manages the translation process using DeepL and uploads the translated files back to GlobalLink.

### Bird 3: Delivery to CMS
![Downloading and pushing to Contentful](image/README/bird_3-1.png)

This bird downloads the finalized translated files and uploads them to Contentful for publishing.

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
