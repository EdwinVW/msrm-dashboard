# Microsoft Release Management Dashboard
This repo contains a simple dashboard for Microsoft Release Management (RM). It's used primarily for giving teams a quick insight into their release pipeline and all releases that are triggered. 

## Dashboard
The dashboard shows the last 5 releases that were started on the RM server (the number of releases shown is configurable). See a screenshot of a dashboard below:

![Dashboard](dashboard.png "Dashboard")

Per release, the stages are shown that are defined in the releasepath that was chosen when the released was started.
For every stage a block is shown with the name of the stage and the name of the corresponding environment. 
Within each stage block, the steps that are executed are shown for that stage are shown. 

## Configuration Panel
By clicking the 'Configure' link in the upper-right corner, the configuration panel is shown: 

![Configuration Panel](configpanel.png "Configuration Panel")

The settings are persisted in a cookie on the client machine when the 'Save' button is clicked.

## Compatibility
The dasboard is currently only tested with Release Management 2013 Update 4.

