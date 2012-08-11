<a name="HOLTop" />
# Building Windows Azure Cloud Services with Cache Service #

---

<a name="Overview" />
## Overview ##

Windows Azure Cache Service provides a distributed, cost-effective in-memory cache for your Cloud Services. With Cache Service enabled on your Cloud Services roles, you can utilize spare memory on your service hosts as high performance cache to improve response time and system throughput. And because the cache hosts are collocated with your Cloud Service roles, you get optimal access time by avoiding external service calls. In this lab, you will learn how easy it is to enable Cache Service on your Cloud Services roles, and how to use Cache Service to provide high performance in-memory caching to your Cloud Services.

<a name="Objectives" />
### Objectives ###
In this hands-on lab, you will learn how to:

- Easily and quickly enable Cache service.
- Use Cache Service for your Asp.Net session state.
- Cache reference data from Windows Azure SQL Database in Cache Service.
- Create a reusable and extensible caching layer for your Cloud Services.

During this lab, you will explore how to use these features in a simple Asp.Net MVC4 application.

<a name="Prerequisites" />
### Prerequisites ###

The following is required to complete this hands-on lab:

- [Microsoft Visual Studio 2012][1]
- [Windows Azure Tools for Microsoft Visual Studio 1.7][2]
- A Windows Azure subscription - [sign up for a free trial](http://aka.ms/WATK-FreeTrial)

[1]: http://www.microsoft.com/visualstudio/11/en-us
[2]: http://www.windowsazure.com/en-us/develop/downloads/
[3]: http://aka.ms/WATK-FreeTrial

>**Note:** This lab was designed to use Windows 8 Operating System.  This hands-on lab has been designed to use the latest release of the Windows Azure Tools for Visual Studio 2012 (version 1.7).

<a name="Setup" />
### Setup ###
In order to run the exercises in this hands-on lab you need to set up your environment first.

1. Open a Windows Explorer window and browse to the lab’s **Source** folder.
1. Right-click on **Setup.cmd** and select Run as Administrator to launch the setup process that will configure your environment and install the Visual Studio code snippets for this lab.
1. If the User Account Control dialog is shown, confirm the action to proceed.

>**Note:** Make sure you have checked all the dependencies for this lab before running the setup.

>This lab requires a Windows Azure SQL Database to start. To build the Northwind2 database automatically, the **Setup.cmd** file will prompt to you with your Windows Azure SQL Database account information. Remember to update the NorthwingEntities connection string in the application’s configuration file to point to your database for each solution.

>Remember to configure the firewall setting your Windows Azure SQL Database account to allow you to specify a list of IP addresses that can access your Windows Azure SQL Database Server. The firewall will deny all connections by default, so **be sure to configure your allow list** so you can connect to the database. Changes to your firewall settings can take a few moments to become effective. For additional information on how to prepare your Windows Azure SQL Database account, refer to the exercise 1 of the Introduction to Windows Azure SQL Database lab in the training kit.

>![SQL Azure database setup](images/sql-azure-database-setup.png?raw=true "Windows Azure SQL Database setup")

>_Windows Azure SQL Database setup_

<a name="CodeSnippets" />
### Using the Code Snippets ###

Throughout the lab document, you will be instructed to insert code blocks. For your convenience, most of that code is provided as Visual Studio Code Snippets, which you can use from within Visual Studio 2012 to avoid having to add it manually. 

>**Note**: Each exercise is accompanied by a starting solution located in the Begin folder of the exercise that allows you to follow each exercise independently of the others. Please be aware that the code snippets that are added during an exercise are missing from these starting solutions and that they will not necessarily work until you complete the exercise. Inside the source code for an exercise, you will also find an End folder containing a Visual Studio solution with the code that results from completing the steps in the corresponding exercise. You can use these solutions as guidance if you need additional help as you work through this hands-on lab.

---

<a name="Exercises" />
## Exercises ##
This hands-on lab includes the following exercises:

1. [Enable Cache service for Session State](#Exercise1)
1. [Caching Data with Cache service](#Exercise2)
1. [Creating a Reusable and Extensible Caching Layer](#Exercise3)

Estimated time to complete this lab: **60 minutes**.

>**Note:** When you first start Visual Studio, you must select one of the predefined settings collections. Every predefined collection is designed to match a particular development style and determines window layouts, editor behavior, IntelliSense code snippets, and dialog box options. The procedures in this lab describe the actions necessary to accomplish a given task in Visual Studio when using the **General Development Settings** collection. If you choose a different settings collection for your development environment, there may be differences in these procedures that you need to take into account.

<a name="Exercise1" />
### Exercise 1: Enable Cache service for Session State ###

In this exercise, you will explore the use of the session state provider for Cache service as the mechanism for out-of-process storage of session state data. For this purpose, you will use the Azure Store-a sample shopping cart application implemented with Asp.Net MVC4. You will run this application in the compute emulator and then modify it to take advantage of the Windows Azure Cache service as the back-end store for the Asp.Net session state. You will start with a begin solution and explore the sample using the default Asp.Net in-proc session state provider. Next, you will add references to the Cache assemblies and configure the session state provider to store the contents of the shopping cart in the distributed cache cluster provided by Cache service.

<a name="Ex1Task1" />
#### Task 1 – Running the Azure Store Sample Site in the Compute Emulator ####

In this task, you will run the Azure Store application in the compute emulator using the default session state provider; you will change that provider to take advantage of the Windows Azure Cache service later on.

1. Start **Microsoft Visual Studio 2012** as administrator.
1. Open the **Begin** solution located at **Source\\Ex1-CacheSessionState\\Begin**.

	>**Important:** 	Before you execute the solution, make sure that the start-up project is set. For MVC projects, the start page must be left blank.

	>To set the start-up project, in **Solution Explorer**, right-click the **MvcAzureStore.Azure** project and select **Set as StartUp Project**.
	
	>To set the start page, in **Solution Explorer**, right-click the **MvcAzureStore** project and select **Properties**. In the **Properties** window, select the **Web** tab and in the **Start Action**, select **Specific Page**. Leave the value of this field blank.

1. In the **Web.config** file, update the _NorthwindEntities_ connection string to point to your database. Replace **[YOUR-SQL-DATABASE-SERVER-ADDRESS]**, **[SQL-DATABASE-USERNAME]**, and **[SQL-DATABASE-PASSWORD]** in the connectionStrings section with the Windows Azure SQL Database server name, Administrator Username and Administrator password that you registered at the portal and used for creating the database during setup.

	>**Note:** Make sure that you follow the instructions of the setup section to create a copy of the Northwind2 database in your own Windows Azure SQL Database account and configure your Windows Azure SQL Database firewall settings.

1. Press **F5** to build and run the application in the compute emulator. 
1. Explore the main page of the application, the **Products** page, which displays a list of products obtained from a Windows Azure SQL Database.

	![Azure Store products page](images/azure-store-products-page.png?raw=true "Azure Store products page")

	_Azure Store products page_

1. Select a product from the list and click **Add item to cart**. You may repeat the process to store additional items in the shopping cart.
1. Click the **Checkout** link to view the contents of the cart. Verify that the items you selected appear on the list. These items are stored in the current session.

	![Checkout page showing the contents of the shopping cart](images/checkout-page-showing-the-contents-of-the-sho.png?raw=true "Checkout page showing the contents of the shopping cart")

	_Checkout page showing the contents of the shopping cart_

1. Navigate back to **Products** page.
1. Click on **Recycle** link. This link forces the web role to be recycled. Once you click on the link, the Products page will turn blank.
1. In the **Compute Emulator**, observe how the web role is recycled by the emulator:

	![Suspending the service role instance](images/suspending-the-service-role-instance.png?raw=true "Suspending the service role instance")

	_Web role recycled_

1. Got back to browser, remove */Home/Recylce* from address bar, and then press Enter to reload the site. The **Products** page should come back normal after a short delay.

1. Navigate to **Checkout** page. Notice that the order now appears empty.

	>**Note:** The application is currently using in-proc session state, which maintains the session state in-memory. When you stop the service instance, it discards all session state including the contents of the shopping cart. In the following task, you will configure the application to store session state using Windows Azure Caching as the storage mechanism, which allows the application to maintain the session state in the presence of restarts and across multiple role instances hosting the application.

1. Close the browser window to stop the application.

<a name="Ex1Task2" />
#### Task 2 – Adding a dedicated caching role ####
In this task, you will add a new worker role that serves as a dedicated cache host. All other web roles and worker roles in the Cloud Service will be able to access the Cache service hosted by this role. You can set up multiple such dedicated work roles within your Cloud Service. In addition, you can also enable Cache service on any of the existing roles and allocate certain percentage of virtual machine memory to be used as cache. 

1. In solution explorer, expand **MvcAzureStore.Azure** node, and then right-click on **Roles**. Then, select **Add**->**New Worker Role Project...***.
2. In **Add New Role Project** dialog, select **Cache Worker Role** template. Name the role as **CacheWorkerRole**, and then click "Add".

  >**Note:** All Cache hosts in your Cloud Service share their runtime states via a Windows Azure Blog Storage. By default, a cache work role is configured to use development storage. You can change this setting in **Caching** tab on the role property page. 

<a name="Ex1Task3" />
#### Task 3 – Configuring Session State Using Windows Azure Cache service ####

In this task, you will change the Session State provider to take advantage of the Windows Azure Cache as the storage mechanism. This requires adding the appropriate assemblies to the **MvcAzureStore** project and then updating the corresponding configuration in the **Web.config** file. 

1. In Visual Studio 2012, open **Package manager Console** from **Tools**->**Library package Manager**->**Package Manager Console** menu.

1. Make sure that **MvcAzureStore** is selected in the **Default project** drop-down list. Issue the following command to install the Nuget package for Cache service:  
 
	````PowerShell
	Install-package Microsoft.WindowsAzure.Caching 
	````
   
1. Open the **Web.config** file located in the root folder of the **MvcAzureStore** project.
1. Change **[cache cluster role name]** to **CacheWorkerRole**.

	<!--mark: 4-->
	```` XML
	<dataCacheClients>
     <tracing sinkType="DiagnosticSink" traceLevel="Error" />
      <dataCacheClient name="default">
       <autoDiscover isEnabled="true" identifier="CacheWorkerRole" />
      <!--<localCache isEnabled="true" sync="TimeoutBased" objectCount="100000" ttlValue="300" />-->
      </dataCacheClient>
  </dataCacheClients>
	  ...
	````

1. Add a new session state provider configuration under System.Web tag:  

   ````XML
	<system.Web>
	...
	<sessionState mode="Custom" customProvider="NamedCacheBProvider">
      <providers>
        <add cacheName="default" name="NamedCacheBProvider" 
             dataCacheClientName="default" applicationName="MyApp" 
             type="Microsoft.Web.DistributedCache.DistributedCacheSessionStateStoreProvider, Microsoft.Web.DistributedCache" />
      </providers>
    </sessionState>
	...
	</system.web>
   ````
1. Press **CTRL + S** to save your changes to the **Web.config** file.

<a name="Ex1Task4"></a>
#### Task 4 – Verification ####

1. Press **Ctrl + F5** to build and run the application. Wait for the browser to launch and show the **Products** page. 
1. Select one product from the list and click Add item to cart. Repeat the process to store additional items in the cart.
1. Click the **Checkout** link to view the contents of the shopping cart. Verify that the items you selected appear on the list.
1. Navigate back to **Products** page and click on "Recycle" link.
1. Observe the web role getting recycled in **Show Compute Emulator UI**. 
1. Go back to browser, remove */Home/Recycle* from address, and then press Enter to reload the site.
1. **Products** page should load correctly. Navigate to **Checkout** page. Notice that the order is intact. This confirms that with the Windows Azure Caching provider, the session state is stored outside the role instance and can persist through application restarts.

	> **Note:** You should infer from the verification that for an application hosted in multiple servers or Windows Azure role instances where a load balancer distributes requests to the application, clients would continue to have access to their session data regardless of which instance responds to the request.

1. Close the browser window to stop the application.

<a name="Exercise2" />
### Exercise 2: Caching Data with Windows Azure Caching ###

This exercise will show you how to use the Windows Azure Caching to cache results from queries to Windows Azure SQL Database. You will continue with a solution based on the one used for the previous exercise the only difference is in the home page, which has been updated to show the elapsed time to retrieve the list of products in the catalog, and now has a link to enable or disable the use of the cache.
During the exercise, you will update the data access code with a trivial implementation of caching. It uses the canonical pattern, in which the code checks the cache first to retrieve the results of a query and, if there is no data available, executes the query against the database to cache the results.

<a name="Ex2Task1" />
#### Task 1 – Caching Data Retrieved from the SQL Reporting ####

To make use of Windows Azure Caching, you first need to create a **DataCacheFactory** object. This object determines the cache cluster connection information, which is set programmatically or by reading settings from the configuration file. Typically, you create an instance of the factory class and use it for the lifetime of the application. To store data in the cache, you request a **DataCache** instance from the **DataCacheFactory** and then use it to add or retrieve items from the cache.
In this task, you update the data access code to cache the result of queries to Windows Azure SQL Database using the Windows Azure Caching. 

1. Start **Microsoft Visual Studio 2012** as an administrator.
1. Open the **Begin** solution located at **Source\\Ex2-CachingData\\Begin**.

	>**Important:** Before you execute the solution, make sure that the start-up project is set. For MVC projects, the start page must be left blank. 
	> To set the startup project, in **Solution Explorer**, right-click the **MvcAzureStore.Azure** project and then select **Set as StartUp Project**. 
	> To set the start page, in **Solution Explorer**, right-click the **MvcAzureStore** project and select **Properties**. In the **Properties** window, select the **Web** tab and in the **Start Action**, select **Specific Page**. Leave the value of this field blank.

1. In the **Web.config** file, update the _NorthwindEntities_ connection string to point to your database. Replace **[YOUR-SQL-DATABASE-SERVER-ADDRESS]**, **[SQL-DATABASE-USERNAME]**, and **[SQL-DATABASE-PASSWORD]** with the Windows Azure SQL Database server name, Administrator Username and Administrator password that you registered at the portal and used for creating the database during setup.

	> **Note:** 	Make sure that you follow the instructions of the setup section to create a copy of the Northwind2 database in your own Windows Azure SQL Database account and configure your Windows Azure SQL Database firewall settings.

1. Open the **ProductsRepository.cs** file in the **Services** folder of the **MvcAzureStore** project.
1. Add a namespace directive for **Microsoft.ApplicationServer.Caching**.

	<!--mark: 5-->
	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using MvcAzureStore.Models;
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

1. Open the **HomeController.cs** file in the **Controllers** folder and add the **System.Diagnostics** using directive at the top of the file.
	
	<!-- mark:1 -->
	````C#
	using System.Diagnostics;
	````

1. Find the **Index** action, locate the lines that instantiate a new **ProductsRepository** and call its **GetProducts** method, and replace them with the highlighted code, as shown below.

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

1. Press **F5** to build and launch the application in the compute emulator.

	>**Note:** Ideally, you should test the code in Windows Azure. When you execute the application in the compute emulator, consider that accessing the Windows Azure SQL Database data source and the Windows Azure Caching require executing requests to resources located outside the bounds of your own network. Depending on your geographic location, both requests may exhibit a relatively high latency, which may overshadow the difference between the cached and non-cached scenarios. Once you deploy the application to Windows Azure, it is co-located in the same data center as the Windows Azure Caching service in Windows Azure SQL Database. As the latency is much lower, the results should be more significant.

1. When you start the application, the cache is initially disabled. Refresh the page and notice the elapsed time displayed at the bottom of the page that indicates the time required to retrieve the product catalog. Note that the first item in the list indicates that the application retrieved the product catalog from the data source.

	>**Note:** You may need to refresh the page several times to obtain a stable reading. The value shown for the first request may be greater because ASP.NET needs to compile the page.

	![Running the application without the cache](images/running-the-application-without-the-cache.png?raw=true "Running the application without the cache")

	_Running the application without the cache_

1. Observe the **Object ID** indicator shown above the product catalog and notice how it changes every time you refresh the page indicating that the repository returns a different object for each call.

1. Now, click **Yes** in **Enable Cache** and wait for the page to refresh. Notice that the first item in the list indicates that it was still necessary for the application to retrieve the product catalog from the data source because the information has yet to be cached.

1. Click **Products**, or refresh the page in the browser. This time, the application retrieves the product data from the Windows Azure Caching and the elapsed time should be lower. Confirm that the first item in the list indicates that the source of the information is the cache.

	![Running the application with the cache enabled](images/running-the-application-with-the-cache-enable.png?raw=true "Running the application with the cache enabled")

	_Running the application with the cache enabled_

1. Close the browser.

<a name="Ex2Task3"></a>
#### Task 3 – Enabling the Local Cache ####

When using Windows Azure Caching, you have the option of using a local cache that allows objects to be cached in-memory at the client, as well as being stored in the cache cluster. In this task, you will enable the local cache and then compare the access time with the remote case.

1. Open the **ProductsRepository.cs** file in the **Services** folder of the **MvcAzureStore** project.

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

1. Open **Index.cshtml** file in the **Views\Home** folder and add the following highlighted code above the **elapsedTime** div.

	(Code Snippet - _BuildingAppsWithCachingService-Ex2-EnableLocalCache Option-HTML_)
	<!--mark: 12-23-->
	````HTML
	<fieldset>
		 <legend>Cache settings for product data</legend>Enable Cache:
		 @if (Model.IsCacheEnabled)
		 {
			  <span>Yes |</span><span>@Html.ActionLink("No", "EnableCache", new { enabled = false })</span>
		 }
		 else
		 {
			  <span>@Html.ActionLink("Yes", "EnableCache", new { enabled = false })</span><span> | No</span>
		 }
		 <br />
		 @if(Model.IsCacheEnabled)
		 {
			  <span>Use Local Cache:</span>
			  if (Model.IsLocalCacheEnabled)
			  {
					<span>Yes |</span><span>@Html.ActionLink("No", "EnableLocalCache", new { enabled = false })</span>
			  }
			  else
			  {
					<span>@Html.ActionLink("Yes", "EnableLocalCache", new { enabled = false })</span><span> | No</span>
			  }
		 }
		 <div id="elapsedTime">Elapsed time: @Model.ElapsedTime.ToString() milliseconds.</div>
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

To take advantage of this caching implementation in the Azure Store application, you will create a caching counterpart of the **ProductsRepository** class. The application uses this class, which implements an **IProductsRepository** contract with a single **GetProducts** operation, to retrieve catalog information from Windows Azure SQL Database. To create a caching products catalog source, you need to perform the following steps:

- Create a new **CachingProductsReposity** class that inherits from **CachedDataSource**.

- Add a constructor to the new class that receives an **IProductRepository** parameter with an instance of the non-caching data source class as well as an **ObjectCache** parameter with an instance of the caching provider to use.

- Implement each method in the **IProductRepository** interface by calling the **RetrievedCachedData** method in the base class and supplying a delegate that calls the original data source class.

<a name="Ex3Task1" />
#### Task 1 – Implementing a Caching Data Source Base Class ####

In this task, you will create the abstract class that you will use as the base class for your caching data source classes. You can take advantage of this general-purpose class in any project that requires a caching layer.

1. Start **Microsoft Visual Studio 2012** as administrator.

1. Open the **Begin** solution located at **Source\\Ex3-ReusableCachingImplementation**.

	>**Important:** 	Before you execute the solution, make sure that the start-up project is set. For MVC projects, the start page must be left blank.  
	
	> To set the start up project, in **Solution Explorer**, right-click the **MvcAzureStore.Azure** project and then select **Set as StartUp Project**. 

	> To set the start page, in **Solution Explorer**, right-click the **MvcAzureStore** project and select **Properties**. In the **Properties** window, select the **Web** tab and in the **Start Action**, select **Specific Page**. Leave the value of this field blank.

1. In the **Web.config** file, update the _NorthwindEntities_ connection string to point to your database. Replace **[YOUR-SQL-DATABASE-SERVER-ADDRESS]**, **[SQL-DATABASE-USERNAME]**, and **[SQL-DATABASE-PASSWORD]** with the Windows Azure SQL Database server name, Administrator Username and Administrator password that you registered at the portal and used for creating the database during setup.

	>**Note:** 	Make sure that you follow the instructions of the setup section to create a copy of the Northwind2 database in your own Windows Azure SQL Database account and configure your Windows Azure SQL Database firewall settings.

1. Add a reference to the **System.Runtime.Caching** assembly in the **MvcAzureStore** project.

1. In the **Services** folder of the **MvcAzureStore** project, add a new folder named **Caching**.

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

1. Inside the **Services\Caching** folder of the **MvcAzureStore** project, add a new class file named **CachedProductsRepository.cs**.

1. In the new class file, append a namespace directive for **System.Runtime.Caching** and **MvcAzureStore.Services**.

	<!-- mark:5-6 -->
	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using MvcAzureStore.Services
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

1. Add a copy of the **AzureCacheProvider.cs** file located in the **\\Source\\Assets** folder to the **MvcAzureStore** project and place it in its **Services\Caching** folder.

	>**Note:** The **AzureCacheProvider** class implements an **ObjectCache** that wraps the services provided by the Windows Azure Cache Service.

1. Inside the **Services** folder of the **MvcAzureStore** project, add a new class file named **DataSourceFactory.cs**.

1. In the new class file, insert namespace directives for **System.Configuration**, **System.Runtime.Caching**, **MvcAzureStore.Services** and **MvcAzureStore.Services.Caching**.

	(Code Snippet - _BuildingAppsWithCachingService-Ex3-DataSourceFactory namespaces-CS_)
	<!--mark: 5-7-->
	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Configuration;
	using System.Runtime.Caching;
	using MvcAzureStore.Services;
	using MvcAzureStore.Services.Caching;
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
		MvcAzureStore.Services.DataSourceFactory.GetProductsRepository(enableCache);
		Stopwatch stopWatch = new Stopwatch();
		stopWatch.Start();
		...
	  }
	  ...
	}
	````

1. To configure the **DataSourceFactory**, open the **Web.config** file and add the following (highlighted) setting to the **appSettings** section.

	(Code Snippet - _BuildingAppsWithCachingService-Ex3-Web.config appSettings section-CS_)
	<!--mark: 3-->
	````XML
	  <appSettings>
		 ...
	    <add key="CacheService.Provider" value="InMemory" />
	  </appSettings>
	````

	>**Note:** If you host the application in a single node, the in-memory cache provider would be a good choice.

1. Press **CTRL+F5** to build and test the enhanced caching implementation in the compute emulator.

1. When you start the application, the cache is initially disabled. Click **Yes** in **Use cache for product data** and wait for the page to refresh. Remember that the initial request after you enable the cache includes the overhead required to retrieve the data and insert it into the cache.

1. Click **Products**, or refresh the page in the browser once again. This time, the application retrieves the product data from the cache and the elapsed time should be lower, most likely under a millisecond given that you have currently configured it to use the in-memory cache provided by the .NET Framework.

1. Now, in the **Web.config** file, locate the **appSettings** section and set the value of the **CacheService.Provider** setting to _Azure_.

	<!--mark: 3-->
	````XML
	  <appSettings>
			  ...
	        <add key="CacheService.Provider" value="Azure" />
	  </appSettings>
	````

	>**Note:** If you host the application in multiple nodes, the in-memory cache provider is no longer a good choice. Instead, you can take advantage of the distributed cache offered by the Windows Azure Caching.

1. Save the **Web.config** file.

1. Click the **Recycle** link to recycle the role and reload the configuration. Once you click on the link, the Products page will turn blank.

1. Got back to browser, remove _/Home/Recylce_ from address bar, and then press Enter to reload the site. The **Products** page should come back normal after a short delay.

1. Make sure that the cache is still enabled and then refresh the page in the browser **twice** to prime the cache with data. Notice that the elapsed times for the cached scenario have increased indicating that the application is now using the Windows Azure Caching provider instead of the in-memory provider.

---

<a name="Summary" />
## Summary ##

In this hands-on lab, you explored the use of the Windows Azure Caching. You saw how to configure session state to be cached across a cache cluster, allowing sessions to be preserved in the presence of restarts and across multiple role instances hosting the application. In addition, you learnt the basics of data caching with Windows Azure and in particular, how to cache the results of queries to a Windows Azure SQL Database. Finally, you looked at a reusable caching layer implementation that will allow you to add a caching layer to your applications in a very simple manner.