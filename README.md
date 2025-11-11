# Sitecore Content Hub

## DAM
`DAM/Scripts` folder provides a set of useful scripts to implement some bespoke functionality in the Sitecore Content Hub DAM.

### asset-auto-approval
`DAM/Scripts/asset-auto-approval` is an example script demonstrates how to automatically approve assets upon upload into the DAM.

To incorporate this script into your asset workflow, you will need to create an **Action** to run the script and then configure a **Trigger** to execute that Action under specific conditions.
For the asset auto-approval use case, follow the steps below to create the Action and the Trigger.

#### Creating the Action
1. Navigate to the _Manage_ page and select the **Actions** tile.
1. Click the _New Action_ button in the upper-right corner.
1. Enter a descriptive _Name_, then select the script from the _Action script_ dropdown list. You may optionally specify a _Label_ and _Type_ if desired.
1. Once all configurations are complete, click _Save_.

#### Creating the Trigger
A Trigger consists of three key elements: Events that initiate the trigger, Conditions as entity definitions and Actions that they execute either in the process or in the background. I recommend running this script **In Background** mode as opposed to In Process to avoid slowing down asset upload performance.
Here are simple steps to create a Trigger:
1.	Navigate to the _Manage_ page and select the **Triggers** tile.
1.	Click the _New Trigger_ button in the upper-corner.
1.	Enter a self-explanatory _Name_, then select _“Entity creation”_ from the _Objective_ list and set the _Execution type_ to _“In background”_.
1.	Next go to the _Conditions_ tab and add a definition for the **M.Asset** entity.
1.	Then proceed to the _Actions_ tab and add an action selecting the appropriate action that you created earlier from the dropdown list.
1.	When finished, click _Save and close_ and enable your newly created trigger.

