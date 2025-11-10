# Sitecore Content Hub

## DAM
`DAM/Scripts` folder provides a set of useful scripts to implement some bespoke functionality in the Sitecore Content Hub DAM.

### asset-auto-approval
`DAM/Scripts/asset-auto-approval` is an example script demonstrates how to automatically approve assets upon upload into the DAM.

To incorporate this script into your asset workflow, you will need to create an **Action** to run the script and then configure a **Trigger** to execute that Action under specific conditions.
For the asset auto-approval use case, follow the steps below to create the Action and the Trigger.

#### Creating the Action
1. Navigate to the _Manage_ page and select the Actions tile.
1. Click the _New Action_ button in the upper-right corner.
1. Enter a descriptive _Name_, then select the script from the _Action script_ dropdown list. You may optionally specify a _Label_ and _Type_ if desired.
1. Once all configurations are complete, click _Save_.
