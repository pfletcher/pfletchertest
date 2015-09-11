# Hello World Node.js

New users should check out the resources available at [HP Helion Docs](http://docs.hpcloud.com/helion/devplatform/workbook/helloworld/node/). 
The site includes more detail and has instructions on how to create an HP
Helion Development Platform Application Lifecycle Services Cluster.

To create a Cloud Foundry app in Node.js, the only mandatory files are:
* Your main .js file (e.g. Server.js)
* manifest.yml
* package.json
* Procfile 

Manifest.yml is a config file used to specify settings that would otherwise be
specified at the command line when using the Helion Client.

The Procfile tells the Helion Development Platform how to run your Node.js 
application. An extremely simple one is included with this sample that just 
tells us how to execute the program. 

## Prerequisites
- If you do not have an HP Helion Development Platform Application Lifecycle 
  Services Cluster available, please create one before continuing. You will also
  need to install the Helion CLI, which can be installed from the cluster's
  Management Console. Please refer to [HP Helion Docs](http://docs.hpcloud.com/helion/devplatform/workbook/helloworld/node/)
  for further details.  
    
## Deploy the Application

Execute the following commands:

- Open the terminal
- If you are not already there, *cd* to the root directory of the sample. The 
  root directory contains the manifest.yml file which helps automate deployment. 
- If you have not logged in to your target environment yet, execute the following:

    `helion target https://api.example.com`
    
    `helion login`
    
    Enter your Management Console credentials
    
    `helion push`

    Hit enter to accept any default values that you may be prompted for. 
    Note: By default, ALS clusters are configured with two domains (private and
    public). In some situations, the Helion CLI may prompt you to select a target
    domain. If prompted, select the public domain from the given list (e.g. https://api.example.com)

## View and run the app
- Go to the management console (e.g. https://api.example.com)
- Check the applications link to see a list of your apps.
- Click on the name of the app you just deployed. The app name is specified in 
  manifest.yml.
- Click "View App" to see your app in action.

The result when visiting the application page and clicking 'View App' should be
a page with the text "Hello World".
