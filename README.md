# Microsoft Release Management Dashboard
This repo contains a simple dashboard for Microsoft Release Management (RM). It's used primarily for giving teams a quick insight into their release pipeline and all releases that are triggered. 

## Dashboard
The dashboard shows the last 5 releases that were started on the RM server (the number of releases shown is configurable). See a screenshot of a dashboard below:

![Dashboard](dashboard.png "Dashboard")

Per release, the stages are shown that are defined in the releasepath that was chosen when the release was started.
For every stage a block is shown with the name of the stage and the name of the corresponding environment. 
Within each stage block, the steps that are executed within that stage are shown. 
The stage that was chosen as target stage for the release is tagged with a bulls-eye icon in the top-left corner.

## Configuration Panel
By clicking the 'Configure' link in the upper-right corner of the dashboard, the configuration panel is shown: 

![Configuration Panel](configpanel.png "Configuration Panel")

The following settings are available:

- Title : the title shown on the dashboard
- Theme : the color theme to use
- Auto refresh : indication whether or not the page is automatically refreshed
- Refresh interval : the interval to wait between every page refresh
- Neumer of releases : the number of releases to show on the dashboard
- Release paths : the release paths to show on the dashboard (none selected means no filter)

If one or more release paths are selected and a new release path is added in RM, this new release path is not shown on the dashboard until it is selected on the configuration panel.

The settings are persisted in a cookie on the client machine when the 'Save' button is clicked.
The cancel button will discard any configuration changes and return to the dashboard.

## Installation
The dashboard is built as an ASP.NET web-application that must be deployed to an IIS webserver.
The application consists of an HTML5/Angular SPA "front-end" and an ASP.NET WebAPI back-end. 
The WebAPI collects the necessary data by querying the RM database.
The web.config contains the connection-string to connect to the database. 
The user used to connect to the database must have read-access to the following tables:

- ReleaseV2
- ReleaseStatus
- ReleaseV2StageWorkflow
- Stage
- StageType
- Environment
- ReleaseV2Step
- StageStepType
- ReleaseStepStatus

The WebAPI exposes 2 resources:

- /api/releases : the data that is presented on the dashboard
- /api/releasepaths : the list of releasepaths defined in RM (used on the configuration panel)

When doing a GET request on /api/releases, two optional headers can be included in the request:

- includedReleasePathIds : a comma-seperated list (without spaces )of release path Ids to include in the dashboard
- releaseCount : the number of releases to show in the dashboard

## Compatibility
The dasboard is currently only tested with Release Management 2013 Update 4 and is only implemented for the "vNext" functionality in RM. 

