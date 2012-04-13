<a name="HOLTop" />
# Building Windows Azure Apps with Caching service #

---

<a name="Overview" />
## Overview ##

The Windows Azure Caching service provides a distributed, in-memory cache for your applications. In this lab, you will learn how to use the Windows Azure Caching service for both your ASP.NET session state and to cache data from your data-tier. You will see how the Windows Azure Caching service provides your application with a cache that provides low latency and high throughput without having to configure, deploy, or manage the service.

<a name="Objectives" />
### Objectives ###
In this hands-on lab, you will learn how to:

- Easily and quickly provision your cache through the portal.
- Use the caching service for your ASP.NET session state.
- Cache reference data from SQL Azure in the caching service.
- Create a reusable and extensible caching layer for your applications.


During this lab, you will explore how to use these features in a simple ASP.NET MVC application.

<a name="Prerequisites" />
### Prerequisites ###

The following is required to complete this hands-on lab:

- [Microsoft .NET Framework 4.0][1]
- Microsoft IIS 7
- [Microsoft Visual Studio 2010][2]
- [Windows Azure SDK and Windows Azure Tools for Microsoft Visual Studio 1.6][3]
- [Windows Azure Libraries for .NET 1.6][4]


[1]: http://go.microsoft.com/fwlink/?linkid=186916
[2]: http://msdn.microsoft.com/vstudio/products/
[3]: http://www.microsoft.com/windowsazure/sdk/
[4]: http://www.microsoft.com/web/gallery/install.aspx?appid=WindowsAzureLibrariesNET

>**Note:** This hands-on lab has been designed to use the latest release of the Windows Azure Tools for Visual Studio 2010 (version 1.6) and the new Windows Azure Management Portal experience.

<a name="Setup" />
### Setup ###
In order to execute the exercises in this hands-on lab you need to set up your environment.

1. Open a Windows Explorer window and browse to the lab’s **Source** folder.
1. Double-click the **Setup.cmd** file in this folder to launch the setup process that will configure your environment and install the Visual Studio code snippets for this lab.
1. If the User Account Control dialog is shown, confirm the action to proceed.

>**Note:** Make sure you have checked all the dependencies for this lab before running the setup.

>This lab requires a SQL Azure database to start. To build the Northwind2 database automatically, the **Setup.cmd** file will prompt to you with your SQL Azure account information. Remember to update the NorthwingEntities connection string in the application’s configuration file to point to your database for each solution.

>Remember to configure the firewall setting your SQL Azure account to allow you to specify a list of IP addresses that can access your SQL Azure Server. The firewall will deny all connections by default, so **be sure to configure your allow list** so you can connect to the database. Changes to your firewall settings can take a few moments to become effective. For additional information on how to prepare your SQL Azure account, refer to the exercise 1 of the Introduction to SQL Azure lab in the training kit.

>![SQL Azure database setup](images/sql-azure-database-setup.png?raw=true "SQL Azure database setup")

>_SQL Azure database setup_

<a name="CodeSnippets" />
### Using the Code Snippets ###

Throughout the lab document, you will be instructed to insert code blocks. For your convenience, most of that code is provided as Visual Studio Code Snippets, which you can use from within Visual Studio 2010 to avoid having to add it manually. 

---

<a name="Exercises" />
## Exercises ##
This hands-on lab includes the following exercises:

1. [Using the Windows Azure Caching for Session State](#Exercise1)
1. [Caching Data with Windows Azure Caching](#Exercise2)
1. [Creating a Reusable and Extensible Caching Layer](#Exercise3)


Estimated time to complete this lab: **60 minutes**.

>**Note:** When you first start Visual Studio, you must select one of the predefined settings collections. Every predefined collection is designed to match a particular development style and determines window layouts, editor behavior, IntelliSense code snippets, and dialog box options. The procedures in this lab describe the actions necessary to accomplish a given task in Visual Studio when using the **General Development Settings** collection. If you choose a different settings collection for your development environment, there may be differences in these procedures that you need to take into account.

<a name="GettingStarted" />
### Getting Started: Provisioning the Service ###

To complete the exercises in this lab, you will require a Windows Live ID Account. Once you have signed in, you will create a new subscription to use the Windows Azure Caching.

<a name="GSTask1" />
#### Task 1 - Provisioning the Cache ####

In this task, you will create a new subscription to use the Windows Azure Caching.

1. Navigate to the [Management Portal](http://windows.azure.com/) and, if necessary, sign in using your Windows Live ID credentials.

1. Go to **Service Bus, Access Control & Caching**, located under the navigation pane, and click **Caching**.

	![Windows Azure Cache](images/windows-azure-cache.png?raw=true "Windows Azure Cache")

	_Windows Azure Cache_

1. Click **New** to create a new instance of caching. Complete the Cache Service Namespace with a custom name, pick a Country/Region, Cache Size, and click **Create Namespace**.

	![Creating a new Cache Service Namespace](images/creating-a-new-cache-service-namespace.png?raw=true "Creating a new Cache Service Namespace")

	_Creating a new Cache Service Namespace_

	>**Note:** A service namespace defines a boundary for each application exposed through the Service Bus, allowing an application to uniquely address the service endpoints.

1. Locate the new entry in the list of configured service namespaces and then wait for its **Status** column to show the namespace as _Active_.

	![Summary list of available cache service namespaces](images/summary-list-of-available-cache-service-names.png?raw=true "Summary list of available cache service namespaces")

	_Summary list of available cache service namespaces_

1. Click on the namespace created in the previous steps, locate the **Properties** pane and record the value shown for the **Service URL** and the **Service Port**. 

	![Service namespace cache settings](images/service-namespace-cache-settings.png?raw=true "Service namespace cache settings")

	_Service namespace cache settings_

1. Now click the **View** button next to **Authentication Token** in the **Properties** pane. In the **Authentication Token** dialog, click **Copy to Clipboard** next to **Authentication Token.** For your convenience, save this value on notepad as you will use this value later on to configure the application.

	![Retrieving the Authentication Token](images/retrieving-the-authentication-token.png?raw=true "Retrieving the Authentication Token")

	_Retrieving the Authentication Token_

1. Click **View Client Configuration** on the upper ribbon and select all the text from the **Client Configuration** window. Press **CTRL + C** to copy, and paste it on a notepad for a later use. 
This XML contains the configuration required to leverage Windows Azure Caching Access Control.

	![Copying XML from Client Configuration](images/copying-xml-from-client-configuration.png?raw=true "Copying XML from Client Configuration")

	_Copying XML from Client Configuration_

<a name="Exercise1" />
### Exercise 1: Using the Windows Azure Caching for Session State ###

In this exercise, you will explore the use of the session state provider for Windows Azure Caching as the mechanism for out-of-process storage of session state data. For this purpose, you will use the Azure Store-a sample shopping cart application implemented with ASP.NET MVC. You will run this application in the compute emulator and then modify it to take advantage of the Windows Azure Caching as the back-end store for the ASP.NET session state. You will start with a begin solution and explore the sample using the default ASP.NET in-proc session state provider. Next, you will add references to the Windows Azure Caching assemblies and configure the session state provider to store the contents of the shopping cart in the distributed cache cluster provided by Window Azure.

<a name="Ex1Task1" />
#### Task 1 – Running the Azure Store Sample Site in the Compute Emulator ####

In this task, you will run the Azure Store application in the compute emulator using the default session state provider; you will change that provider to take advantage of the Windows Azure Caching service later on.

1. Start **Microsoft Visual Studio 2010** as administrator.
1. Open the **Begin** solution located at **Source\\Ex1-CacheSessionState\\Begin**.

	>**Important:** 	Before you execute the solution, make sure that the start-up project is set. For MVC projects, the start page must be left blank.

	>To set the start-up project, in **Solution Explorer**, right-click the **AzureStoreService** project and select **Set as StartUp Project**.
	
	>To set the start page, in **Solution Explorer**, right-click the **MVCAzureStore** project and select **Properties**. In the **Properties** window, select the **Web** tab and in the **Start Action**, select **Specific Page**. Leave the value of this field blank.

1. In the **Web.config** file, update the _NorthwindEntities_ connection string to point to your database. Replace **[YOUR-SQL-AZURE-SERVER-ADDRESS]**, **[SQL-AZURE-USERNAME]**, and **[SQL-AZURE-PASSWORD]** in the connectionStrings section with the SQL Azure database server name, Administrator Username and Administrator password that you registered at the portal and used for creating the database during setup.

	>**Note:** Make sure that you follow the instructions of the setup section to create a copy of the Northwind2 database in your own SQL Azure account and configure your SQL Azure firewall settings.

1. Press **F5** to build and run the application in the compute emulator. 
1. Explore the main page of the application, the **Products** page, which displays a list of products obtained from a SQL Azure database.

	![Azure Store products page](images/azure-store-products-page.png?raw=true "Azure Store products page")

	_Azure Store products page_

1. Select a product from the list and click **Add item to cart**. You may repeat the process to store additional items in the shopping cart.
1. Click the **Checkout** link to view the contents of the cart. Verify that the items you selected appear on the list. These items are stored in the current session.

	![Checkout page showing the contents of the shopping cart](images/checkout-page-showing-the-contents-of-the-sho.png?raw=true "Checkout page showing the contents of the shopping cart")

	_Checkout page showing the contents of the shopping cart_

1. Do not close the browser window or navigate away from the checkout page.
1. In the task bar, right-click the compute emulator icon and select **Show Compute Emulator UI**. 
1. In the **Compute Emulator**, right-click the **AzureStoreService** node and choose **Suspend**.

	![Suspending the service role instance](images/suspending-the-service-role-instance.png?raw=true "Suspending the service role instance")

	_Suspending the service role instance_

1. Open a command window in elevated administrator mode, from **Start | All Programs | Accessories | Command Prompt** by right clicking the **Command Prompt** shortcut and choosing **Run as Administrator** and type _iisreset._

	![Executing iisreset command](images/executing-iisreset-command.png?raw=true "Executing iisreset command")

	_Executing iisreset command_

	>**Note:** These two steps, recycling the role and restarting IIS, simulate what would happen in Windows Azure when a role instance is restarted.

1. Go back to the **Compute Emulator** and wait until the service is destroyed as indicated by the instance icon turning purple. Now, restart the service instance once again. To do this, right-click the **AzureStoreService** node and choose **Run**, then wait for the service to start. 

	>**Note:** If you are using as default browser other than Internet Explorer, the suspended instance in the compute emulator may be removed and Visual Studio will stop debugging. If this happens, go back to Visual Studio and press **F5** to run the application in the compute emulator again and wait for the service to start.

1. Switch back to the browser window showing the checkout page and click **Refresh**. Notice that the order now appears empty.

	>**Note:** The application is currently using in-proc session state, which maintains the session state in-memory. When you stop the service instance, it discards all session state including the contents of the shopping cart. In the following task, you will configure the application to store session state using Windows Azure Caching as the storage mechanism, which allows the application to maintain the session state in the presence of restarts and across multiple role instances hosting the application.

1. Close the browser window to stop the application.

<a name="Ex1Task2" />
#### Task 2 – Configuring Session State Using Windows Azure Caching ####

In this task, you will change the Session State provider to take advantage of the Windows Azure Cache as the storage mechanism. This requires adding the appropriate assemblies to the **MVCAzureStore** project and then updating the corresponding configuration in the **Web.config** file. 

1. Go back to Visual Studio 2010 and add a reference to the **Microsoft.ApplicationServer.Caching.Client**, **Microsoft.ApplicationServer.Caching.Core**, and **Microsoft.Web.DistributedCache** assemblies. In **Solution Explorer**, right-click the **MVCAzureStore** project, select **Add Reference**, click the **.NET** tab, select the assemblies and click **OK**.
1. Make sure to include these assemblies as part of the service package. To do this, right-click the **Microsoft.ApplicationServer.Caching.Client** assembly and select **Properties**. In the **Project Properties** window, verify that the value of the **Copy Local** setting is set to _True_. Do the same for the other assemblies added in the previous step.

	>**Note:** In general, you need to set **Copy Local** = _True_ for any assembly that is not installed by default in the Windows Azure VMs to ensure that it is deployed with your application.

1. Open the **Web.config** file located in the root folder of the **MVCAzureStore** project.
1. Go back to the notepad window where you saved the Client Configuration data and copy the text from the **configSections** tag.

	![Copying configSections](images/copying-configsections.png?raw=true "Copying configSections")

	_Copying configSections_

1. Go back to Visual Studio 2010, and paste the **configSections** block inside the **configuration** tag in **web.config** file. Make sure that this element is the first element inside the **configuration** tag.

	<!--mark: 2-5-->
	```` XML
	<configuration>
	  <configSections>
	    <section name="dataCacheClients" type="Microsoft.ApplicationServer.Caching.DataCacheClientsSection, Microsoft.ApplicationServer.Caching.Core"
	             allowLocation="true" allowDefinition="Everywhere"/>
	  </configSections>
 
	  <connectionStrings>
	  ...
	````

1. Go back to the notepad window again and copy the **dataCacheClients** configuration.

	![Copying dataCacheClients section](images/copying-datacacheclients-section.png?raw=true "Copying dataCacheClients section")

	_Copying dataCacheClients section_

1. Go back to Visual Studio 2010 and paste the configuration you have copied, under the closing **configSections** tag, as shown in the image below.

	<!--mark: 4-16--->
	````XML
	...
	</configSections>
	
	<dataCacheClients>
	  <dataCacheClient name="default">
	    <hosts>
	      <host name="[SERVICE-HOST-NAME]" cachePort="22233" />
	    </hosts>
	
	    <securityProperties mode="Message">
	      <messageSecurity
	        authorizationInfo="[AUTHORIZATION INFO]">
	      </messageSecurity>
	    </securityProperties>
	  </dataCacheClient>
	</dataCacheClients>
   
	<connectionStrings>
	````

	>**Note:** When copying the **dataCacheClients** section from the Management Portal, you will find two endpoints declared: one simple and other ssl endpoint. For this lab, we will be using the simple one, so the ssl should not be included in the **web.config** file.

1. The last setting you need to configure is the sessionState provider, to do so, go back to the notepad window and copy **sessionState** element.

	![Copying sessionState](images/copying-sessionstate.png?raw=true "Copying sessionState")

	_Copying sessionState_

1. Go back to Visual Studio 2010 and paste the element you copied inside the **system.web** configuration section.

	<!--mark: 4-12-->
	````XML
	...
	<system.web>
	
	  <sessionState mode="Custom" customProvider="AzureCacheSessionStoreProvider">
	    <providers>
	      <add name="AzureCacheSessionStoreProvider"
	            type="Microsoft.Web.DistributedCache.DistributedCacheSessionStateStoreProvider, Microsoft.Web.DistributedCache"
	            cacheName="default"
	            useBlobMode="true"
	            dataCacheClientName="default" />
	    </providers>
	  </sessionState>
 
	  <compilation debug="true" targetFramework="4.0">
	  ...
	````

	>**Note:** The **DistributedCacheSessionStateStoreProvider** session state provider enables out-of-process storage of session state data using Windows Azure Cache as the storage mechanism.

1. Press **CTRL + S** to save your changes to the **Web.config** file.

<a name="Ex1Task3"></a>
#### Task 3 – Verification ####

1. Press **F5** to build and run the application. Wait for the browser to launch and show the Products page. 
1. Select one product from the list and click Add item to cart. Repeat the process to store additional items in the cart.
1. Click the **Checkout** link to view the contents of the shopping cart. Verify that the items you selected appear on the list.
1. Do not close the browser window or navigate away from the checkout page.
1. In the task bar, right-click the compute emulator icon and select **Show Compute Emulator UI**. 
1. In the **Compute Emulator**, right-click the **AzureStoreService** node and choose **Suspend**. Wait until the service stops as indicated by the instance icon turning purple. 
1. Open a command window in elevated administrator mode and type **iisreset**.
1. Now, restart the service instance once again. To do this, right-click the **AzureStoreService** node and choose **Run**, then wait for the service to start. 

	> **Note:** If you are using as default browser other than Internet Explorer, the suspended instance in the compute emulator may be removed and Visual Studio will stop debugging. If this happens, go back to Visual Studio and press **F5** to run the application in the compute emulator again and wait for the service to start.

1. Switch back to the browser window showing the checkout page and click **Refresh**. Notice that the order is intact. This confirms that with the Windows Azure Caching provider, the session state is stored outside the role instance and can persist through application restarts.

	> **Note:** You should infer from the verification that for an application hosted in multiple servers or Windows Azure role instances where a load balancer distributes requests to the application, clients would continue to have access to their session data regardless of which instance responds to the request.

1. Close the browser window to stop the application.

<a name="Exercise2" />
### Exercise 2: Caching Data with Windows Azure Caching ###

This exercise will show you how to use the Windows Azure Caching to cache results from queries to SQL Azure. You will continue with a solution based on the one used for the previous exercise-the only difference is in the home page, which has been updated to show the elapsed time to retrieve the list of products in the catalog, and now has a link to enable or disable the use of the cache.
During the exercise, you will update the data access code with a trivial implementation of caching. It uses the canonical pattern, in which the code checks the cache first to retrieve the results of a query and, if there is no data available, executes the query against the database to cache the results.

<a name="Ex2Task1" />
#### Task 1 – Caching Data Retrieved from the SQL Azure Reporting ####

To make use of the Windows Azure Caching service, you first need to create a **DataCacheFactory** object. This object determines the cache cluster connection information, which is set programmatically or by reading settings from the configuration file. Typically, you create an instance of the factory class and use it for the lifetime of the application. To store data in the cache, you request a **DataCache** instance from the **DataCacheFactory** and then use it to add or retrieve items from the cache.
In this task, you update the data access code to cache the result of queries to SQL Azure using the Windows Azure Caching. 

1. Start **Microsoft Visual Studio 2010** as an administrator.
1. Open the **Begin** solution located at **Source\\Ex2-CachingData\\Begin**.

	>**Important:** Before you execute the solution, make sure that the start-up project is set. For MVC projects, the start page must be left blank. 
	> To set the startup project, in **Solution Explorer**, right-click the **AzureStoreService** project and then select **Set as StartUp Project**. 
	> To set the start page, in **Solution Explorer**, right-click the **MVCAzureStore** project and select **Properties**. In the **Properties** window, select the **Web** tab and in the **Start Action**, select **Specific Page**. Leave the value of this field blank.

1. In the **Web.config** file, update the _NorthwindEntities_ connection string to point to your database. Replace **[YOUR-SQL-AZURE-SERVER-ADDRESS]**, **[SQL-AZURE-USERNAME]**, and **[SQL-AZURE-PASSWORD]** with the SQL Azure database server name, Administrator Username and Administrator password that you registered at the portal and used for creating the database during setup.

	> **Note:** 	Make sure that you follow the instructions of the setup section to create a copy of the Northwind2 database in your own SQL Azure account and configure your SQL Azure firewall settings.

1. Open the **ProductsRepository.cs** file in the **Services** folder of the **MVCAzureStore** project.
1. Add a namespace directive for **Microsoft.ApplicationServer.Caching**.

	<!--mark: 5-->
	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using MVCAzureStore.Models;
	using Microsoft.ApplicationServer.Caching;
	...
	````

1. In the **ProductsRepository** class, add the following highlighted code to define a constructor and declare a static member variable for a **DataCacheFactory** object instance, in addition to a boolean instance variable to control the use of the cache.

	(Code Snippet - _BuildingAppsWithCachingService-Ex2-ProductsRepository constructor-CS_)
	<!--mark: 3-9-->
	````C#
	public class ProductsRepository : IProductRepository	
	{
	  private static DataCacheFactory cacheFactory = new DataCacheFactory();
	  private bool enableCache = false;
 
	  public ProductsRepository(bool enableCache)
	  {
	    this.enableCache = enableCache;
	  }
 
	  public List<string> GetProducts()
	  {
	    ...
	  }
	}
	````

	> **Note:** The **DataCacheFactory** member is declared as static and is used throughout the lifetime of the application.

1. Locate the **GetProducts** method and insert the following (highlighted) code immediately after the line that declares the **products** local variable.
	
	(Code Snippet - _BuildingAppsWithCachingService-Ex2-GetProducts read cache-CS_)
	<!--mark: 8-30-->
	````C#
	public class ProductsRepository : IProductRepository
	{
	  ...
	  public List<string> GetProducts()
	  {
	    List<string> products = null;
	
	    DataCache dataCache = null;
	    if (this.enableCache)
	    {
	      try
	      {
	        dataCache = cacheFactory.GetDefaultCache();
	        products = dataCache.Get("products") as List<string>;
	        if (products != null)
	        {
	          products[0] = "(from cache)";
	          return products;
	        }
	      }
	      catch (DataCacheException ex)
	      {
	        if (ex.ErrorCode != DataCacheErrorCode.RetryLater)
	        {
	          throw;
	        }

	        // ignore temporary failures
	      }
	    }
 	    
	    NorthwindEntities context = new NorthwindEntities();
	    
	    try
	    {
	      var query = from product in context.Products
	                  select product.ProductName;
	      products = query.ToList();
	    }
	    finally
	    {
	      if (context != null)
	      {
	        context.Dispose();
	      }
	    }
	    
	    return products;
	  }
	}
	````

	>**Note:** The inserted code uses the **DataCacheFactory** object to return an instance of the default cache object and then attempts to retrieve an item from this cache using a key with the value "_products_". If the cache contains an object with the requested key, it sets the text of the first entry to indicate that the list was retrieved from the cache and then returns it. The code treats temporary failures from the Windows Azure Caching service as a cache miss so that it can retrieve the item from its data source instead.

1. Next, add the following (highlighted) code block to the **GetProducts** method, immediately before the line that returns the **products** list at the end of the method.

	(Code Snippet - _BuildingAppsWithCachingService-Ex2-GetProducts write cache-CS_)
	<!--mark: 30-35-->
	````C#
	public class ProductsRepository : IProductRepository
	{
		...
		public List<string> GetProducts()
		{
			List<string> products = null;
		
			DataCache dataCache = null;
			if (this.enableCache)
			{
			  ...
			}
		
			NorthwindEntities context = new NorthwindEntities();
		
			try
			{
			  var query = from product in context.Products
			             select product.ProductName;
			  products = query.ToList();
			}
			finally
			{
			  if (context != null)
			  {
			    context.Dispose();
			  }
			}
		
			products.Insert(0, "(from data source)");
		
			if (this.enableCache && dataCache != null)
			{
			  dataCache.Add("products", products, TimeSpan.FromSeconds(30));
			}
			
			return products;
		}
	}
	````

	>**Note:** The inserted code stores the result of the query against the data source into the cache and sets its expiration policy to purge the item from the cache after 30 seconds.

<a name="Ex2Task2" />
#### Task 2 – Measuring the Data Access Latency ####

In this task, you will update the application to allow control of the use of the cache from the UI and to display the time required to retrieve catalog data, allowing you to compare the latency of retrieving data from the cache against the time required to access the data source.

1. Open the **HomeController.cs** file in the **Controllers** folder and find the **Index** action. Locate the lines that instantiate a new **ProductsRepository** and call its **GetProducts** method, and replace them with the highlighted code, as shown below.

	(Code Snippet - _BuildingAppsWithCachingService-Ex2-GetProducts latency-CS_)
	<!--mark: 9-17; strike:6-8-->
	````C#
	public class HomeController : Controller
	{
	  ...                       
	  public ActionResult Index()
	  {
	    Services.IProductRepository productRepository =
	        new Services.ProductsRepository();
	    var products = productRepository.GetProducts();
	    bool enableCache = (bool)this.Session["EnableCache"];
	
	    // retrieve product catalog from repository and measure the elapsed time
	    Services.IProductRepository productRepository =
	        new Services.ProductsRepository(enableCache);
	    Stopwatch stopWatch = new Stopwatch();
	    stopWatch.Start();
	    var products = productRepository.GetProducts();
	    stopWatch.Stop();
 
	    // add all products currently not in session
	    var itemsInSession = this.Session["Cart"] as List<string> ?? new List<string>();
	    var filteredProducts = products.Where(item => !itemsInSession.Contains(item));
	
	    IndexViewModel model = new IndexViewModel()
	    {
	      Products = filteredProducts
	    };
	
	    return View(model);
	  }
	  ...
	}
	````

1. In the same method, locate the code that creates a new **IndexViewModel** instance and replace its initialization with the following (highlighted) code block.

	(Code Snippet - _BuildingAppsWithCachingService-Ex2-IndexViewModel initialization-CS_)
	<!--mark: 22-25-->
	````C#
	public class HomeController : Controller
	{
	  ...                       
	  public ActionResult Index()
	  {
	    bool enableCache = (bool)this.Session["EnableCache"];
	
	    // retrieve product catalog from repository and measure the elapsed time
	    Services.IProductRepository productRepository =
	        new Services.ProductsRepository(enableCache);
	    Stopwatch stopWatch = new Stopwatch();
	    stopWatch.Start();
	    var products = productRepository.GetProducts();
	    stopWatch.Stop();
	
	    // add all products currently not in session
	    var itemsInSession = this.Session["Cart"] as List<string> ?? new List<string>();
	    var filteredProducts = products.Where(item => !itemsInSession.Contains(item));
	
	    IndexViewModel model = new IndexViewModel()
	    {
	      Products = filteredProducts,
	      ElapsedTime = stopWatch.ElapsedMilliseconds,
	      IsCacheEnabled = enableCache,
	      ObjectId = products.GetHashCode().ToString()
		};
	
	    return View(model);
	  }
	  ...
	}
	````

	>**Note:** The elements added to the view model provide the time taken to load the product catalog from the repository, a flag to indicate whether the cache is enabled, and an identifier for the catalog object returned by the call to **GetProducts**. The view displays the object ID to allow you to determine whether the instance returned by the call to the repository has changed. This feature will be used later in the exercise, when you enable the local cache.


1. Add a new action method to the **HomeController** to enable or disable the cache from the UI of the application.

	(Code Snippet - _BuildingAppsWithCachingService-Ex2-EnableCache method-CS_)
	<!--mark: 4-8-->
	````C#
	public class HomeController : Controller
	{
	  ...
	  public ActionResult EnableCache(bool enabled)
	  {
	    this.Session["EnableCache"] = !((bool)this.Session["EnableCache"]);
	    return RedirectToAction("Index");
	  }
	 }
	````

1. Open the **Web.config** file and locate the **dataCacheClient** section with name “default”. Replace the **[SERVICE-HOST-NAME]** placeholder with the name of the host for the Windows Azure Caching service endpoint that you provisioned earlier. For example, _your-namespace.cache.windows.net_. Replace the **[AUTHORIZATION_INFO]** placeholder with the authentication token that you copied from the **Cache Settings** page.

1. Press **F5** to build and launch the application in the compute emulator.

	>**Note:** Ideally, you should test the code in Windows Azure. When you execute the application in the compute emulator, consider that accessing the SQL Azure data source and the Windows Azure Caching require executing requests to resources located outside the bounds of your own network. Depending on your geographic location, both requests may exhibit a relatively high latency, which may overshadow the difference between the cached and non-cached scenarios. Once you deploy the application to Windows Azure, it is co-located in the same data center as the Windows Azure Caching service in SQL Azure. As the latency is much lower, the results should be more significant.

1. When you start the application, the cache is initially disabled. Refresh the page and notice the elapsed time displayed at the bottom of the page that indicates the time required to retrieve the product catalog. Note that the first item in the list indicates that the application retrieved the product catalog from the data source.

	>**Note:** You may need to refresh the page several times to obtain a stable reading. The value shown for the first request may be greater because ASP.NET needs to compile the page.

	![Running the application without the cache](images/running-the-application-without-the-cache.png?raw=true "Running the application without the cache")

	_Running the application without the cache_

1. Observe the **Object ID** indicator shown above the product catalog and notice how it changes every time you refresh the page indicating that the repository returns a different object for each call.

1. Now, click **Yes** in **Enable Cache** and wait for the page to refresh. Notice that the first item in the list indicates that it was still necessary for the application to retrieve the product catalog from the data source because the information has yet to be cached.

1. Click **Products**, or refresh the page in the browser. This time, the application retrieves the product data from the Windows Azure Caching and the elapsed time should be lower. Confirm that the first item in the list indicates that the source of the information is the cache.

	![Running the application with the cache enabled](images/running-the-application-with-the-cache-enable.png?raw=true "Running the application with the cache enabled")

	_Running the application with the cache enabled_

<a name="Ex2Task3"></a>
#### Task 3 – Enabling the Local Cache ####

When using the Windows Azure Caching service, you have the option of using a local cache that allows objects to be cached in-memory at the client, as well as being stored in the cache cluster. In this task, you will enable the local cache and then compare the access time with the remote case.

1. Open the **ProductsRepository.cs** file in the **Services** folder of the **MVCAzureStore** project.

	>**Note:** Make sure your solution is not running before editing the files.

1. In the **ProductsRepository** class, replace the current fields and constructor with the following code, to add the logic of managing the localCache configuration.

	(Code Snippet - _BuildingAppsWithCachingService-Ex2-ProductsRepository with local cache-CS_)
	<!--mark: 2-34-->
	````C#
	...	
	private static DataCacheFactory cacheFactory;
	private static DataCacheFactoryConfiguration factoryConfig;
	private bool enableCache = false;
	private bool enableLocalCache = false;
	
	public ProductsRepository(bool enableCache, bool enableLocalCache)
	{
	    this.enableCache = enableCache;
	    this.enableLocalCache = enableLocalCache;
	
	    if (enableCache)
	    {
	        if (enableLocalCache && (factoryConfig == null || !factoryConfig.LocalCacheProperties.IsEnabled))
	        {
	            TimeSpan localTimeout = new TimeSpan(0, 0, 30);
	            DataCacheLocalCacheProperties localCacheConfig = new DataCacheLocalCacheProperties(10000, localTimeout, DataCacheLocalCacheInvalidationPolicy.TimeoutBased);
	            factoryConfig = new DataCacheFactoryConfiguration();
	
	            factoryConfig.LocalCacheProperties = localCacheConfig;
	            cacheFactory = new DataCacheFactory(factoryConfig);
	        }
	        else if (!enableLocalCache && (factoryConfig == null || factoryConfig.LocalCacheProperties.IsEnabled))
	        {
	            cacheFactory = null;
	        }
	    }
	
	    if (cacheFactory == null)
	    {
	        factoryConfig = new DataCacheFactoryConfiguration();
	        cacheFactory = new DataCacheFactory(factoryConfig);
	    }
	} 
	...
	````

1. Open the **HomeController.cs** file in the **Controllers** folder and find the **Index** action. Locate the line that instantiates a new **ProductsRepository** and Replace those lines with the following highlighted code:

	(Code Snippet - _BuildingAppsWithCachingService-Ex2-GetProducts LocalCache-CS_)
	<!--mark: 7-10; strike: 11-13-->
	````C#
	public class HomeController : Controller
	{
	  ...                       
	  public ActionResult Index()
	  {
		bool enableCache = (bool)this.Session["EnableCache"];
		bool enableLocalCache = (bool)this.Session["EnableLocalCache"];

		// retrieve product catalog from repository and measure the elapsed time
		Services.IProductRepository productRepository = new Services.ProductsRepository(enableCache, enableLocalCache);
		// retrieve product catalog from repository and measure the elapsed time
		Services.IProductRepository productRepository =
		new Services.ProductsRepository(enableCache);
		Stopwatch stopwatch = new Stopwatch();
		stopWatch.Start();
		var products = productRepository.GetProducts();
	    ...
	}
	````

1. In the same method, locate the code that creates a new **IndexViewModel** and add the following highlighted property.

	<!--mark: 25-->
	````C#
	public class HomeController : Controller
	{
	  ...                       
	  public ActionResult Index()
	  {
	      bool enableCache = (bool)this.Session["EnableCache"];
	      bool enableLocalCache = (bool)this.Session["EnableLocalCache"];
	      // retrieve product catalog from repository and measure the elapsed time
	      Services.IProductRepository productRepository =
	      new Services.ProductsRepository(enableCache, enableLocalCache);
	      Stopwatch stopwatch = new Stopwatch();
	      stopWatch.Start();
	      var products = productRepository.GetProducts();
	      stopWatch.Stop();
	
	      // add all products currently not in session
	      var itemsInSession = this.Session["Cart"] as List<string> ?? new List<string>();
	      var filteredProducts = products.Where(item => !itemsInSession.Contains(item));
	
	      IndexViewModel model = new IndexViewModel()
	      {
	          Products = filteredProducts,
	          ElapsedTime = stopWatch.ElapsedMilliseconds,
	          IsCacheEnabled = enableCache,
	          IsLocalCacheEnabled = enableLocalCache,
	          ObjectId = products.GetHashCode().ToString()
	      };
	      return View(model);
	  }
	}
	````

1. Add a new action method to the **HomeController** to enable or disable the local cache from the UI of the application.

	(Code Snippet - _BuildingAppsWithCachingService-Ex2-EnableLocalCache method-CS_)
	<!--mark: 4-8-->
	````C#
	public class HomeController : Controller
	{
	  ...
	  public ActionResult EnableLocalCache(bool enabled)
	  {
	    this.Session["EnableLocalCache"] = !((bool)this.Session["EnableLocalCache"]);
	    return RedirectToAction("Index");
	  }
 }
	````

1. Open **Index.aspx** file in the **Views\Home** folder and add the following highlighted code above the **elapsedTime** div.

	(Code Snippet - _BuildingAppsWithCachingService-Ex2-EnableLocalCache Option-HTML_)
	<!--mark: 11-25-->
	````HTML
	<fieldset>
	    <legend>Cache settings for product data</legend>Enable Cache:
	    <%if (Model.IsCacheEnabled)
	      { %>
	        Yes | <%=Html.ActionLink("No", "EnableCache", new { enabled = false }).ToString()%>
	    <%}
	      else
	      { %>
	        <%=Html.ActionLink("Yes", "EnableCache", new { enabled = true }).ToString()%> | No
	    <%} %>
	    <br />
	    <%if (Model.IsCacheEnabled)
	    { %>
	      Use Local Cache:
	      <%if (Model.IsLocalCacheEnabled)
	      { %>
	    Yes |
	        <%=Html.ActionLink("No", "EnableLocalCache", new { enabled = false }).ToString()%>
	      <%}
	      else
	      { %>
	        <%=Html.ActionLink("Yes", "EnableLocalCache", new { enabled = true }).ToString()%>
	    | No
	      <%} %>
	    <%} %>
	    <div id="elapsedTime">Elapsed time: <%:Model.ElapsedTime.ToString()%> milliseconds.</div>
	</fieldset>
	````

1. Press **F5** to build and launch the application in the compute emulator.
 
1. When you start the application, the cache option is initially disabled and the local cache option is hidden (it will be shown once you enable cache). Enable cache and then the local cache. 

1. Refresh the page several times until the elapsed time stabilizes. Notice that the reading is now significantly lower, possibly under a millisecond, showing that the application now retrieves the data from the local in-memory cache. 

	![Using the local cache](images/using-the-local-cache.png?raw=true "Using the local cache")

	_Using the local cache_

1. Observe that, each time you refresh the page, the **Object ID** shown above the product catalog remains constant indicating that the repository now returns the same object each time.

	>**Note:** 	This is an important aspect to consider. Previously, with the local cache disabled, changing an object retrieved from the cache had no effect on the cached data and subsequent fetches always returned a fresh copy. Once you enable the local cache, it stores references to in-memory objects and any changes to the object directly affect the cached data. 
You should be aware of this when using the cache in your own applications and consider that, after changing a cached object and later retrieving the same object from the cache, it may or may not include these changes depending on whether it is returned by the local or remote cache.

1. Wait for at least 30 seconds and then refresh the page one more time. Notice that the elapsed time is back to its original value and that the object ID has changed, showing that the cached item has expired and been purged from the cache due to the expiration policy set on the object when it was stored.

<a name="Exercise3" />
### Exercise 3: Creating a Reusable and Extensible Caching Layer ###

In the previous exercise, you explored the fundamental aspects of using the Windows Azure Caching by directly updating the methods in the data access class to cache data retrieved from the repository. While this approach can yield significant benefits, it requires you to change each one of your data access methods to enable caching. An alternative approach that does not require changes to your existing data access classes would be advantageous. 

In this exercise, you will explore building a caching layer on top of your existing data access classes that will allow you to plug in different caching providers, or even remove them altogether, through simple configuration changes.

To build this layer, you will implement an abstract caching class named **CachedDataSource** that will provide support for storing and removing data in the cache. You will then derive from this class to create a caching equivalent for any data source in your application. The only requirement is that your data source implements a contract to define its data access operations. The caching class encapsulates a caching provider, which you need to provide in its constructor, and provides methods to retrieve and remove data from the cache.

The data retrieval method in the caching class receives a cache key that uniquely identifies a cached item, a delegate that retrieves data from the data source, and a cache expiration policy that determines when to purge the item from the cache. This method implements the classic caching pattern where it first attempts to retrieve an item from the cache and, if it does not find a copy, uses the supplied delegate to retrieve the data from the source and then stores it in the cache.

The implementation of the **CachedDataSource** class is completely reusable, allowing you to use any caching provider that fits your requirements. To specify a caching provider, you supply an [ObjectCache](http://msdn.microsoft.com/en-us/library/system.runtime.caching.objectcache.aspx) instance to its constructor. The **ObjectCache** class, part of the **System.Runtime.Caching** namespace, was introduced in the .NET Framework 4 to make caching available for all applications. This abstract class represents an object cache and provides base methods and properties for accessing an underlying cache provider. The .NET Framework already offers a concrete implementation of this class that provides an in-memory cache, the [MemoryCache](http://msdn.microsoft.com/en-us/library/system.runtime.caching.memorycache.aspx). 

To use a given cache service with the **CachedDataSource** derived class, you need to supply an **ObjectCache** implementation specific to the caching provider. A good approach is to create a data source factory that allows you to choose a suitable caching implementation based on your needs. Replacing the caching provider is then simply a matter of changing a setting in the configuration file.

Currently, the Windows Azure Caching does not supply its own **ObjectCache** implementation. Nevertheless, you can create one that provides a wrapper around its services. You will find an example of such an implementation, the **AzureCacheProvider**, in the **BuildingAppsWithCacheService\\Source\\Assets** folder. This class derives from **ObjectCache** to expose the services in the Windows Azure Caching.

To take advantage of this caching implementation in the Azure Store application, you will create a caching counterpart of the **ProductsRepository** class. The application uses this class, which implements an **IProductsRepository** contract with a single **GetProducts** operation, to retrieve catalog information from SQL Azure. To create a caching products catalog source, you need to perform the following steps:

- Create a new **CachingProductsReposity** class that inherits from **CachedDataSource**.

- Add a constructor to the new class that receives an **IProductRepository** parameter with an instance of the non-caching data source class as well as an **ObjectCache** parameter with an instance of the caching provider to use.

- Implement each method in the **IProductRepository** interface by calling the **RetrievedCachedData** method in the base class and supplying a delegate that calls the original data source class.

<a name="Ex3Task1" />
#### Task 1 – Implementing a Caching Data Source Base Class ####

In this task, you will create the abstract class that you will use as the base class for your caching data source classes. You can take advantage of this general-purpose class in any project that requires a caching layer.

1. Start **Microsoft Visual Studio 2010** as administrator.

1. Open the **Begin** solution located at **Source\\Ex3-ReusableCachingImplementation**.

	>**Important:** 	Before you execute the solution, make sure that the start-up project is set. For MVC projects, the start page must be left blank.  
	
	> To set the start up project, in **Solution Explorer**, right-click the **AzureStoreService** project and then select **Set as StartUp Project**. 

	> To set the start page, in **Solution Explorer**, right-click the **MVCAzureStore** project and select **Properties**. In the **Properties** window, select the **Web** tab and in the **Start Action**, select **Specific Page**. Leave the value of this field blank.

1. In the **Web.config** file, update the _NorthwindEntities_ connection string to point to your database. Replace **[YOUR-SQL-AZURE-SERVER-ADDRESS]**, **[SQL-AZURE-USERNAME]**, and **[SQL-AZURE-PASSWORD]** with the SQL Azure database server name, Administrator Username and Administrator password that you registered at the portal and used for creating the database during setup.

	>**Note:** 	Make sure that you follow the instructions of the setup section to create a copy of the Northwind2 database in your own SQL Azure account and configure your SQL Azure firewall settings.

1. Add a reference to the **System.Runtime.Caching** assembly in the **MVCAzureStore** project.

1. In the **Services** folder of the **MVCAzureStore** project, add a new folder named **Caching**.

1. Inside the **Caching** folder created in the previous step, add a new class file named **CachedDataSource.cs**.

1. In the new class file, add a namespace directive for **System.Runtime.Caching**.

	<!--mark: 5-->
	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Runtime.Caching;
	...
	````

1. Specify an **abstract** modifier for the **CachedDataSource** class.

	<!--mark: 1-3-->
	````C#
	public abstract class CachedDataSource
	{
	}
	````

1. Add the following (highlighted) member fields to the class.

	(Code Snippet - _BuildingAppsWithCachingService-Ex3-CachedDataSource member fields-CS_)
	<!--mark: 3,4-->
	````C#
	public abstract class CachedDataSource
	{
	  private readonly ObjectCache cacheProvider;
	  private readonly string regionName;
	}
	````

1. Now, define a constructor that receives an object cache and a region name as parameters, as shown (highlighted) below.

	(Code Snippet - _BuildingAppsWithCachingService-Ex3-CachedDataSource constructor-CS_)
	<!--mark: 4-18-->
	````C#
	public abstract class CachedDataSource
	{
	  ...
	  public CachedDataSource(ObjectCache cacheProvider, string regionName)
	  {
	    if (cacheProvider == null)
	    {
	      throw new ArgumentNullException("cacheProvider");
	    }
	
	    if (cacheProvider is MemoryCache)
	    {
	      regionName = null;
	    }
	
	    this.cacheProvider = cacheProvider;
	    this.regionName = regionName;
	  }
	}
	````

	>**Note:** The **CachedDataSource** constructor receives an ObjectCache ([http://msdn.microsoft.com/en-us/library/system.runtime.caching.objectcache.aspx](http://msdn.microsoft.com/en-us/library/system.runtime.caching.objectcache.aspx)) instance as a parameter, which provides methods and properties for accessing an object cache, as well as a region name.  A cache region is a partition in the cache used to organize cache objects.

1. Next, add the following (highlighted) method to retrieve data from the cache.

	(Code Snippet - _BuildingAppsWithCachingService-Ex3-RetrieveCachedData method-CS_)
	<!--mark: 4-19-->
	````C#
	public abstract class CachedDataSource
	{
	  ...
	  protected T RetrieveCachedData<T>(string cacheKey, Func<T> fallbackFunction, CacheItemPolicy cachePolicy) where T : class
	  {
	    var data = this.cacheProvider.Get(cacheKey, this.regionName) as T;
	    if (data != null)
	    {
	      return data;
	    }
	
	    data = fallbackFunction();
	    if (data != null)
	    {
	      this.cacheProvider.Add(new CacheItem(cacheKey, data, this.regionName), cachePolicy);
	    }
	
	    return data;
	  }
	}
	````

	>**Note:** The **RetrieveCachedData** method uses the provided key to retrieve a copy of the requested item from the cache. If the data is available, it returns it; otherwise, it uses the provided fallback delegate to obtain the information from the data source and then caches the result using the supplied cache expiration policy.

1. Finally, add a method to delete items from the cache.

	(Code Snippet - _BuildingAppsWithCachingService-Ex3-RemoveCachedData method-CS_)
	<!--mark: 4-7-->
	````C#
	public abstract class CachedDataSource
	{
	  ...
	  protected void RemoveCachedData(string cacheKey)
	  {
	    this.cacheProvider.Remove(cacheKey, this.regionName);
	  }
	}
	````

1. Save the **CachedDataSource.cs** file.

<a name="Ex3Task2" />
#### Task 2 – Building a Caching Product Catalog Repository ####

Once you have created an abstract base class for caching data sources, you will now create a concrete implementation that will provide a caching alternative for the **ProductsRepository** class. This task represents the steps you would typically follow when creating a caching layer for your data access code using the **CachedDataSource** class.

1. Inside the **Services\Caching** folder of the **MVCAzureStore** project, add a new class file named **CachedProductsRepository.cs**.

1. In the new class file, append a namespace directive for **System.Runtime.Caching**.

	<!--mark: 5-->
	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Runtime.Caching;
	...
	````

1. Change the declaration for the **CachedProductsRepository** class to derive from both **CachedDataSource** and **IProductRepository**, as shown (highlighted) below.

	<!--mark: 2-->
	````C#
	public class CachedProductsRepository 
	  : CachedDataSource, IProductRepository
	{
	}
	````

	>**Note:** The caching data source class derives from **CachedDataSource** to provide the necessary caching behavior, as well as implementing the same contract used by the original data source class.

1. Add the following code to define a constructor and declare a member field that holds a reference to the underlying data source, as shown (highlighted) below.

	(Code Snippet - _BuildingAppsWithCachingService-Ex3-CachedProductsRepository constructor-CS_)
	<!--mark: 3-9-->
	````C#
	public class CachedProductsRepository : CachedDataSource, IProductRepository
	{
	  private readonly IProductRepository repository;
	 
	  public CachedProductsRepository(IProductRepository repository, ObjectCache cacheProvider) :
	    base(cacheProvider, "Products")
	  {
	    this.repository = repository;
	  }
	}
	````

	>**Note:** The **CachedProductsRepository** constructor initializes its base class using the supplied cache provider and saves a reference to the underlying data source in a member field. The class defines a "_Products_" cache region.

1. Finally, fulfill the **IProductRepository** contract by implementing the **GetProducts** method, as shown (highlighted) below.

	(Code Snippet - _BuildingAppsWithCachingService-Ex3-GetProducts method -CS_)
	<!--mark: 4-10-->
	````C#
	public class CachedProductsRepository : CachedDataSource, IProductRepository
	{
	  ...
	  public List<string> GetProducts()
	  {
		return RetrieveCachedData(
		"allproducts",
		() => this.repository.GetProducts(),
		new CacheItemPolicy { AbsoluteExpiration = DateTime.UtcNow.AddMinutes(1) });
	  }
	}
	````

	>**Note:** The **GetProducts** method calls **RetrieveCachedData** in the base class, passing in a key that identifies the cached item, in this case "_allproducts_", a fallback delegate in the form of a lambda expression that simply calls the **GetProducts** method in the original data source, and a [CacheItemPolicy](http://msdn.microsoft.com/en-us/library/system.runtime.caching.cacheitempolicy.aspx) to set the expiration of the item to 1 minute.

	Because the **IProductRepository** contract is so simple, this is all that is required to provide a caching implementation. Typically, your data sources will have more than one method, but the basic approach should not change, allowing you to implement every method by copying this same pattern.

<a name="Ex3Task3" />
#### Task 3 – Creating a Data Source Factory Class ####

In this task, you will create a factory class that can return data source instances. The factory determines the cache provider to use from the application configuration settings and returns a data source suitably configured to use the chosen cache provider.

1. Add a copy of the **AzureCacheProvider.cs** file located in the **\\Source\\Assets** folder to the **MVCAzureStore** project and place it in its **Services\Caching** folder.

	>**Note:** The **AzureCacheProvider** class implements an **ObjectCache** that wraps the services provided by the Windows Azure Cache Service.

1. Inside the **Services** folder of the **MVCAzureStore** project, add a new class file named **DataSourceFactory.cs**.

1. In the new class file, insert namespace directives for **System.Configuration**, **System.Runtime.Caching**, and **MVCAzureStore.Services.Caching**.

	(Code Snippet - _BuildingAppsWithCachingService-Ex3-DataSourceFactory namespaces-CS_)
	<!--mark: 5-7-->
	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Configuration;
	using System.Runtime.Caching;
	using MVCAzureStore.Services.Caching;
	````

1. Now, add the following code to define a type constructor for the **DataSourceFactory** class and declare a static field that holds a reference to the configured cache service provider, as shown (highlighted) below.

	(Code Snippet - _BuildingAppsWithCachingService-Ex3-DataSourceFactory class constructor-CS_)
	<!--mark: 3-20-->
	````C#
	public class DataSourceFactory
	{
	  private static readonly ObjectCache cacheProvider;
	
	  static DataSourceFactory()
	  {
	    string provider = ConfigurationManager.AppSettings["CacheService.Provider"];
	    if (provider != null)
	    {
	      switch (ConfigurationManager.AppSettings["CacheService.Provider"].ToUpperInvariant())
	      {
	        case "AZURE":
	          cacheProvider = new AzureCacheProvider();
	          break;
	        case "INMEMORY":
	          cacheProvider = MemoryCache.Default;
	          break;
	      }
	    }
	  }
	}
	````

	>**Note:** The class constructor reads the _CacheService.Provider_ setting from the configuration and initializes the cache provider for the application based on its value. In this example, two different values for the setting are recognized, one for the Windows Azure Caching and another one for the default in-memory cache provider offered by the .NET Framework 4.


1. Next, add the following property to return the configured cache service provider.

	(Code Snippet - _BuildingAppsWithCachingService-Ex3-CacheProvider property-CS_)
	<!--mark: 4-7-->
	````C#
	public class DataSourceFactory
	{
	  ...
	  public static ObjectCache CacheProvider
	  {
	    get { return cacheProvider; }
	  }
	}
	````

1. Finally, add a method to return an instance of the **IProductRepository** data source initialized with the configured cache service provider.

	(Code Snippet - _BuildingAppsWithCachingService-Ex3-GetProductsRepository method-CS_)
	<!--mark: 4-13-->
	````C#
	public class DataSourceFactory
	{
	  ...
	  public static IProductRepository GetProductsRepository(bool enableCache)
	  {
	    var dataSource = new ProductsRepository();
	    if (enableCache && CacheProvider != null)
	    {
	      return new CachedProductsRepository(dataSource, cacheProvider);
	    }
	
	    return dataSource;
	  }
	}
	````

<a name="Ex3Task4" />
#### Task 4 – Configuring the Application for Caching ####

In this task, you will update the application to take advantage of the data source factory to instantiate the product catalog data source. To complete the setup of the caching layer, you will define the necessary configuration settings to select a caching provider.

1. Open the **HomeController.cs** file in the **Controllers** folder and find the **Index** method. Inside this method, replace the line that initializes the **productRepository** local variable with the code shown (highlighted) below that uses the **DataSourceFactory** to retrieve an **IProductRepository** instance.

	<!--mark: 10-->
	````C#
	public class HomeController : Controller
	{
	  ...
	  public ActionResult Index()
	  {
		bool enableCache = (bool)this.Session["EnableCache"];
	
		// retrieve product catalog from repository and measure the elapsed time
		Services.IProductRepository productRepository =
		MVCAzureStore.Services.DataSourceFactory.GetProductsRepository(enableCache);
		Stopwatch stopWatch = new Stopwatch();
		stopWatch.Start();
		...
	  }
	  ...
	}
	````

1. To configure the **DataSourceFactory**, open the **Web.config** file and insert the following (highlighted) **appSettings** section, ensuring that you add the content after the **configSections** element and as a direct child of the **configuration** element.

	(Code Snippet - _BuildingAppsWithCachingService-Ex3-Web.config appSettings section-CS_)
	<!--mark: 6-8-->
	````XML
	<configuration>
	  <configSections>
	    ...
	  </configSections>
	  ...
	  <appSettings>
	    <add key="CacheService.Provider" value="InMemory" />
	  </appSettings>
   ...
	</configuration>
	````

	>**Note:** If you host the application in a single node, the in-memory cache provider would be a good choice.

1. Locate the **dataCacheClient** section with name “default” and replace the **[SERVICE-HOST-NAME]** placeholder with the name of the host for the Windows Azure Caching service endpoint that you provisioned earlier. For example, _your-namespace.cache.windows.net_. Replace the **[AUTHORIZATION_INFO]** placeholder with the authorization token that you copied from the **Cache Settings** page.

1. Press **F5** to build and test the enhanced caching implementation in the compute emulator.

1. When you start the application, the cache is initially disabled. Click **Yes** in **Use cache for product data** and wait for the page to refresh. Remember that the initial request after you enable the cache includes the overhead required to retrieve the data and insert it into the cache.

1. Click **Products**, or refresh the page in the browser once again. This time, the application retrieves the product data from the cache and the elapsed time should be lower, most likely under a millisecond given that you have currently configured it to use the in-memory cache provided by the .NET Framework.

1. Now, in the **Web.config** file, locate the **appSettings** section and set the value of the **CacheService.Provider** setting to _Azure_.

	<!--mark: 4-->
	````XML
	<configuration>
	  ...
	  <appSettings>
	        <add key="CacheService.Provider" value="Azure" />
	  </appSettings>
	  ...
	</configuration>
	````

	>**Note:** If you host the application in multiple nodes, the in-memory cache provider is no longer a good choice. Instead, you can take advantage of the distributed cache offered by the Windows Azure Caching.

1. Save the **Web.config** file.

1. Suspend and Run the compute emulator to restart and reload the configuration.
Make sure that the cache is still enabled and then refresh the page in the browser **twice** to prime the cache with data. Notice that the elapsed times for the cached scenario have increased indicating that the application is now using the Windows Azure Caching provider instead of the in-memory provider.

---

<a name="Summary" />
## Summary ##

In this hands-on lab, you explored the use of the Windows Azure Caching. You saw how to configure session state to be cached across a cache cluster, allowing sessions to be preserved in the presence of restarts and across multiple role instances hosting the application. In addition, you learnt the basics of data caching with Windows Azure and in particular, how to cache the results of queries to a SQL Azure database. Finally, you looked at a reusable caching layer implementation that will allow you to add a caching layer to your applications in a very simple manner.