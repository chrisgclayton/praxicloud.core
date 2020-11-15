# PraxiCloud Core
PraxiCloud Libraries are a set of common utilities and tools for general software development that simplify common development efforts for software development. The core library contains basic utilities and functions with the minimum required additional dependencies. These libraries target the .NET Core frameworks to enable cross operating system usage.



# Installing via NuGet

Install-Package PraxiCloud-Core



# Buffer Management

In developing high throughput protocols and messaging systems the creation and deletion of byte arrays can consume significant memory in the form of buffers. The buffer pool types are meant to offer a solution for this issue.

## Key Types and Interfaces

|Class| Description | Notes |
| ------------- | ------------- | ------------- |
|**BufferPoolConfiguration**|Describes the configuration of a BufferPool and how it operates. Key properties include:<br><br>***Capacity*** The number of buffers in the pool<br>***Size*** The size of the buffers in the pool<br>***CleanOnReturn*** true if the buffer contents should be cleared when returned to the pool|  |
|**BufferPool**|A pool of byte arrays ("buffers") that can be "checked out" and later returned to reduce the number of allocations required.<br /><br />***Take*** checks out a buffer from the pool. If no buffers are available one will be instantiated.<br />***Return*** returns a previously checked out (through the Take method) buffer to the pool for future use.| It is recommended to keep buffers checked for the minimal time possible, to reduce the number of times it must resort to  new allocations. |

## Sample Usage

### Buffer Pool Creation, Take and Return

```csharp
var configuration = new BufferPoolConfiguration
{
	Capacity = capacity,
	Size = size,
	CleanOnReturn = cleanOnReturn
};

var pool = new BufferPool(configuration);
var buffer = pool.Take();

// Do something with the buffer here

var returnResult = pool.Return(buffer);
```

## Additional Information

For additional information the Visual Studio generated documentation found [here](./documents/praxicloud.core/praxicloud.core.xml), can be viewed using your favorite documentation viewer.



# Configuration

Configuration is a key piece of any application and the use of these utilities speed the development of a layered configuration approach, making the solution easily leveraged for situations where there are multiple deployment strategies such as console vs. Kubernetes. These utilities leverage the most common layering types to provide a consistent experience.

## Key Types and Interfaces

|Class| Description | Notes |
| ------------- | ------------- | ------------- |
|**ConfigurationUtilities**|Loads configuration from environment variables, command line parameters, JSON files, JSON strings and in-memory dictionaries into POCO objects.<br><br>***ExpandConfigFileName*** returns the filename if available at the specified location and if not tries relative to the current directory.<br>***CreateFrom<T>*** loads the configuration object of type T in layers of the provided information based on the following order: in-memory dictionary, environment variables, JSON file name, command line arguments, JSON string. The final defined value is assigned to the POCO objects property.<br>***GetFromDictionary*** are a set of helper methods used to retrieve values from a dictionary of string keys and values, using the typed default value specified if they cannot be property converted or are not found.| While this type offers many methods for loading configurations based on the instantiated type, using the static CreateFrom methods is expected to be used in most situations. |
|**PropertyDump**|A helper type that uses reflection to output all of the properties of the provided object that have a public getter.<br /><br />***WriteConfiguration*** writes all of the property values that have public getters by invoking the WriteProperty delegate provided. A common way to  use this is to write to the core logger or console.| It is recommended to keep buffers checked for the minimal time possible, to reduce the number of times it must resort to  new allocations.<br />An attribute is included called DoNotOutputAttribute that when applied to a property will indicate it should be skipped (to stop from outputting security materials etc.). |

## Sample Usage

### Retrieving a Boolean from a Dictionary with Default Value of true

```csharp
var dictionary = new Dictionary<string, string>
{
    { "Item1", "true" },
    { "Item2", "false" },
    { "Item3", "true" },
    { "Item4", "false" },
    { "Item5", "true" },
    { "Item6", "false" },
    { "Item7", "true" },
    { "Item8", "false" }
};

var value = ConfigurationUtilities.GetFromDictionary(dictionary, "Item4", false);
```
### File Expansion

```csharp
var expandedPath = ConfigurationUtilities.ExpandConfigFileName("samplefile.txt", Environment.CurrentDirectory);
```

### Loading Configuration

```csharp
var memory = new Dictionary<string, string>
            {
                { "StringValue", "StringValueMemory" },
                { "ObjectValue:StringValue", "SubstringMemory" }
            };

var commandLine = new string[] 
            {
                "--DateTimeValue=2020-09-01 14:00:00.0000"
            };
var json = "{ \"BooleanValue\": true, \"IntegerValue\": 72 }";

DemoConfiguration typedConfiguration = ConfigurationUtilities.CreateFrom<DemoConfiguration>(json, "SparseConfiguration.json", "configuration", commandLine, memory);

/// <summary>
/// A configuration test object
/// </summary>
public sealed class DemoConfiguration
{
    /// <summary>
    /// A value with boolean data type
    /// </summary>
    public bool BooleanValue { get; set; }
    
    /// <summary>
    /// A value with integer data type
    /// </summary>
    public int IntegerValue { get; set; }
    
    /// <summary>
    /// A value with a string data type
    /// </summary>
    public string StringValue { get; set; }
    
    /// <summary>
    /// A value with an enumeration data type
    /// </summary>
    public DemoType EnumerationValue { get; set; }
    
    /// <summary>
    /// A value with a date time data type
    /// </summary>
    public DateTime DateTimeValue { get; set; }
    
    /// <summary>
    /// A value with a time span data type
    /// </summary>
    public TimeSpan TimeSpanValue { get; set; }
    
    /// <summary>
    /// A value with an object data type
    /// </summary>
    public SubConfiguration ObjectValue { get; set; }
}

/// <summary>
/// Demo type definition
/// </summary>
public enum DemoType
{
    Simple = 1,
    Complex = 2
}

/// <summary>
/// A configuration object used as a sub object
/// </summary>
public sealed class SubConfiguration
{
    /// <summary>
    /// A value with a string data type
    /// </summary>
    public string StringValue { get; set; }
}

/*
Sample (SparseConfiguration.json:
{
  "BooleanValue": true,
  "DateTimeValue": "2020-10-17 17:05:00.0000",
  "EnumerationValue": 2,
  "IntegerValue": 88,
  "TimeSpanValue": "17:17:17.1717",
  "StringValue": "StringFile2",
  "ObjectValue": {
    "StringValue": "SubstringFile2"
  }
}
*/
```

### Dumping Object Properties

```csharp
var container = new ConfigurationContainer
{
    Description = "Description of the container",
    FileName = "C:\\upload\\file.txt",
    Retries = 6,
    UploadTimeout = TimeSpan.FromMinutes(10)
};

PropertyDump.WriteConfiguration(container, (string propertyName, object value, Type type) => Console.WriteLine($"{propertyName} :: {value}"));

/// <summary>
/// A sample configuration container (POCO)
/// </summary>
public class ConfigurationContainer
{
    /// <summary>
    /// The file to upload
    /// </summary>
    public string FileName { get; set; }
		
    /// <summary>
    /// The description of the file
    /// </summary>
    [DoNotOutput]
    public string Description { get; set; }

    /// <summary>
    /// The number of retries
    /// </summary>
    public int Retries { get; set; }

    /// <summary>
    /// The timeout
    /// </summary>
    public TimeSpan UploadTimeout { get; set; }
}
```

## Additional Information

For additional information the Visual Studio generated documentation found [here](./documents/praxicloud.core/praxicloud.core.xml), can be viewed using your favorite documentation viewer.



# Containers

When constructing containers regardless of the platform there are typically a series of features and functions that are required. In many cases these same features and functions may be applied in a console application but the surfacing of the container movement has made them a top of mind construct. 

The key concepts approached are the easy implementation of health (is the container's logic perceiving it as healthy) and availability (is the container available to take requests) probes as well as monitoring for container shutdown, seeing most containers run until the container is terminated by an orchestrator of forms. The probes have been implemented using TCP but implement an interface for easy dependency injection cases to switch to HTTP or similar. 

## Key Types and Interfaces

|Class| Description | Notes |
| ------------- | ------------- | ------------- |
|**ContainerLifecycle**|Monitors for container shutdown, as identified by the executing assembly unloading or the cancellation key being pressed. <br><br>***CancellationToken*** is a cancellation token that triggers a cancellation request when the container is shutting down.<br>***Task*** is a Task that completes when the container is shutting down.<br>***End*** is a helpful static method used to artificially indicate a container shutdown. This is useful for unit testing and may also be used for secondary shutdown paths.| Many orchestrators notify the container through these means, which also aligns to console applications. |
|**ContainerEnvironment**|Provides a set of easy to use environment details in a single location.<br />***CommandLine*** the command line used to start the container.<br />***IsX86*** true if running with an OS architecture of X86.<br />***IsX64*** true if running with an OS architecture of X64.<br />***IsWindows*** true if the OS is Windows.<br />***IsLinux*** true if the OS is Linux.<br />***IsMacOS*** true if the OS is OSX.<br />| While many of these are available through other means, simplifying into a single type for readability and ease of access / awareness. |
|**IAvailabilityCheck**|This interface is implemented by the instances that perform the check to determine if the logic is available.<br />***IsAvailableAsync*** is the only property which is called by an availability probe to determine if the service is available.|  |
|**IHealthCheck**|This interface is implemented by the instances that perform the check to determine if the logic is healthy.<br />***IsHealthyAsync*** is the only property which is called by an health probe to determine if the service is healthy.|  |
|**AvailabilityContainerProbe**|This is a TCP availability probe used by containers to notify the coordinator if they are ready to receive traffic.| This type implements the IAvailabilityContainerProbe interface that can be used to enable dependency injection and the capability of changing probe protocols transparently. |
|**HealthContainerProbe**|This is a TCP health probe used by containers to notify the coordinator if they are healthy.| This type implements the IHealthContainerProbe interface that can be used to enable dependency injection and the capability of changing probe protocols transparently. |

## Sample Usage

### Wait for a Period or Container Shutdown

```csharp
await Task.Delay(5000, ContainerLifecycle.CancellationToken).ConfigureAwait(false);
```
### Wait for a Task to Complete or Container Shutdown

```csharp
await Task.WhenAny(Task.Delay(5000), ContainerLifecycle.Task).ConfigureAwait(false);
```

### Check if the Operating System is Linux

```csharp
if(ContainerEnvironment.IsLinux)
{
    Console.WriteLine("You are running on Linux");
}
```

### Expose an Availability Probe

```csharp
var probeCheck = new AvailabilityCheck();
var probe = new AvailabilityContainerProbe(10001, TimeSpan.FromSeconds(10), probeCheck);

await probe.StartAsync(ContainerLifecycle.CancellationToken).ConfigureAwait(false);
// Do things
await probe.StopAsync(CancellationToken.None).ConfigureAwait(false);

/// <summary>
/// Basic availability check interface
/// </summary>
public sealed class AvailabilityCheck : IAvailabilityCheck
{
    /// <inheritdoc />
    public Task<bool> IsAvailableAsync()
    {
        // Perform customer logic here to determine availabilty and return true if available.
        
        return Task.FromResult(true);
    }
}
```

### Expose an Health Probe

```csharp
var probeCheck = new HealthCheck();
var probe = new HealthContainerProbe(10001, TimeSpan.FromSeconds(10), probeCheck);

await probe.StartAsync(ContainerLifecycle.CancellationToken).ConfigureAwait(false);
// Do things
await probe.StopAsync(CancellationToken.None).ConfigureAwait(false);

/// <summary>
/// Basic health check interface
/// </summary>
public sealed class HealthCheck : IHealthCheck
{
    /// <inheritdoc />
    public Task<bool> IsHealthyAsync()
    {
        // Perform customer logic here to determine health and return true if available.
        
        return Task.FromResult(true);
    }
}
```
## Additional Information

For additional information the Visual Studio generated documentation found [here](./documents/praxicloud.core/praxicloud.core.xml), can be viewed using your favorite documentation viewer.



# Exceptions

This is a set of toos focused on working with exceptions, including flattening the messages across aggregates and inner exceptions to setting up the handled of unobserved exceptions raised by tasks and application domains. 

## Key Types and Interfaces

|Class| Description | Notes |
| ------------- | ------------- | ------------- |
|**UnobservedHandlers**|Exceptions that are not caught can lead to the application domain crashing and shutting down the application. This type sets up an easy handler set for these situations, allowing the code to perform the appropriate handling logic and then returning true if it was successful in handling it.| It is best practices to only resolve exceptions that the code base can reasonable handle in a safe way. |
|**ExceptionExtensions**|This type exposes as of common functions as extension methods.<br />***FlattenToString*** is an extension method that iterates over inner exceptions to build a fully description string whether a standard exception or aggregate.<br />***IsFatal*** is an extension method that returns true if the exception is any of the known "typically" fatal exceptions (e.g. DataException, OutOfMemoryException, InsufficientMemoryException, AccessViolationException or SEHException).| Is fatal may not be complete and does not infer any kind of application logic. |

## Sample Usage

### Write Unhandled Exceptions to the Console

```csharp
var handlers = new UnobservedHandlers((object sender, Exception exception, bool isTerminating, UnobservedType sourceType) => 
{
    Console.WriteLine($"An unobserved exception of type {sourceType} was raised with a message of {exception.Message}");

    // If possible do not fail as it is considered handled
    return true; 
});
```

### Save if not Fatal

```csharp
using System;
using praxicloud.core.exceptions.extensions;

public static class Program
{
    public void Main(string[] args)
    {
        try
        {
            // do something that may generate an exception
        }
        catch(Exception e)
        {
            if(!e.IsFatal())
            {
                Console.WriteLine("Good news the exception was not fatal");
            }
        }
    }
}
```
### Output the Complete Exception Message

```csharp
using System;
using praxicloud.core.exceptions.extensions;

public static class Program
{
    public void Main(string[] args)
    {
        try
        {
            // do something that may generate an exception
        }
        catch(Exception e)
        {
            var errorMessage = e.FlattenToString();
            
            Console.WriteLine($"The exception was: {errorMessage}");
        }
    }
}
```

## Additional Information

For additional information the Visual Studio generated documentation found [here](./documents/praxicloud.core/praxicloud.core.xml), can be viewed using your favorite documentation viewer.


# Performance

This is a set of tools used to configure common performance aspects of applications. 

## Key Types and Interfaces

|Class| Description | Notes |
| ------------- | ------------- | ------------- |
|**PerformanceManager**|A common set of configuration methods that configure performance characteristics of key application features based on the provided configuration.<br />***ConfigureThreadPool*** configures the .NET threadpool, including the worker and I/O completion threads. This configuration may be performed based on direct assignment of numbers or through multiplying the value by the core count.<br />***ConfigureHttp*** configures the ServicePointManager to optimize the communications with the various hosts. This may be overridden when using the HttpClient through the use of handlers.| The thread pools minimum thread count is an important consideration and before configuration it is best to review the meaning of the value (ThreadPool.SetMinThreads documentation) and base it on monitoring the application performance under load.<br /><br />HTTP configuration is extremely important when working with REST based systems or the polite web browser limits associated with maximum connections per host may greatly impede throughput. |

## Sample Usage

### Configure the ThreadPool

```csharp
var valuesSet = new PerformanceConfiguration
{
    UseCoreMultiplier = false,
    MinimumWorkerThreads = 40,
    MinimumIoCompletionThreads = 20,
    MaximumWorkerThreads = 80,
    MaximumIoCompletionThreads = 60
};

PerformanceManager.ConfigureThreadPool(valuesSet);
```

### Configure the ThreadPool Based on a Multiple of the Available Cores

```csharp
var valuesSet = new PerformanceConfiguration
{
    UseCoreMultiplier = true,
    MinimumWorkerThreads = 20,
    MinimumIoCompletionThreads = 10,
    MaximumWorkerThreads = 40,
    MaximumIoCompletionThreads = 30
};

PerformanceManager.ConfigureThreadPool(valuesSet);
```

### Configure the Http (ServicePointManager) 

```csharp
var configuration = new PerformanceConfiguration
{
    DefaultConnectionLimit = 26,
    UseNagle = false,
    Expect100Continue = false
};

PerformanceManager.ConfigureHttp(configuration);
```

## Additional Information

For additional information the Visual Studio generated documentation found [here](./documents/praxicloud.core/praxicloud.core.xml), can be viewed using your favorite documentation viewer.

# Reflection

Reflection is often the slowest part of an application forcing a balance of flexibility to performance. This set of features provides ways to use compiled expressions and delegates to increase performance and ease when using reflection for scenarios such as serializers. 

## Key Types and Interfaces

|Class| Description | Notes |
| ------------- | ------------- | ------------- |
|**TypeShredder**|The type shredder is used to interrogate an object for its public properties whether setters or getters. The properties setters and getters have a dictionary of compiled expressions it stores to improve execution performance of these properties.<br />***ExpressionList*** exposes a list of PropertyExpressions, which include both the setter and getter. If the setter or getter was not found or was not public it will be null.| The initial parsing of the expression lists and reflection to discover the properties is slow so it is recommended to use this judiciously. once the properties are retrieved it may be optimal to place the desired properties in an dictionary etc. for faster access. |

## Sample Usage

### Getting and Setting the First Name from a Person Through Reflection

```csharp
var personShredder = new TypeShredder(typeof(Person));

var person = new Person("Joe", "Smith", 23);
var firstNameExpression = personShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "FirstName", StringComparison.Ordinal)).First();

var firstName = firstNameExpression.GetValue(person);
Console.WriteLine($"The persons first name is { firstName }, changing to John");
firstNameExpression.SetValue(person, "John");


/// <summary>
/// A person
/// </summary>
public class Person
{
    /// <summary>
    /// Initializes a new instance of the type
    /// </summary>
    /// <param name="firstName">The first name</param>
    /// <param name="lastName">The last name</param>
    /// <param name="age">The age in years</param>
    public Person(string firstName, string lastName, int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }

    /// <summary>
    /// holds the write only property
    /// </summary>
    private string _value;

    /// <summary>
    /// The first name
    /// </summary>
    public string FirstName { get; }
    
    /// <summary>
    /// The last name
    /// </summary>
    public string LastName { get; }
    
    /// <summary>
    /// The age in years
    /// </summary>
    public int Age { get; set; }
    
    /// <summary>
    /// A value to only be writeable
    /// </summary>
    public string NotWritable { get; }
    
    /// <summary>
    /// A value to only be readable
    /// </summary>
    public string NotReadable 
    { 
        set
        {
            _value = value;
        }
    }
}
```

## Additional Information

For additional information the Visual Studio generated documentation found [here](./documents/praxicloud.core/praxicloud.core.xml), can be viewed using your favorite documentation viewer.

# Security

The security functions provided include functionality include tasks associated with items such as authentication, X509 certificate validation, secret storage, Shared Access Token construction and validation and method parameter validation.

## Key Types and Interfaces

|Class| Description | Notes |
| ------------- | ------------- | ------------- |
|**AzureOauthTokenAuthentication**|Helper utilities to use Azure Active Directory to retrieve Oauth tokens based on different authentication modes.<br />***GetOauthTokenCredentialFromManagedIdentity*** retrieves the token credential provider for scenarios that use Azure Managed Identity.<br />***GetOauthTokenCredentialFromClientSecret*** retrieves the token credential provider using authentication based on a service principal or application registration (tenant id, client id and client secret).| Ideal usage is to use managed identity when appropriate to reduce the requirements to store security credentials for the application to access. |
|**CertificateValidation**|Performs common certificate validation operations.<br />***CertificateValidationCallBack*** implements the remote server certificate callback to only allow certificates that pass all security validation tests.<br />***CertificateValidationCallBackAllowsSelfSigned*** implements the remote server certificate callback to only allow certificates that pass all security validation tests except it allows self signed.| Self signed certificates should only be used for development purposes if possible. |
|**Guard**|Offers a wide range of parameter fuzzing / validation methods. If a validation is failed a GuardException is raised, making consumers logic easier for differentiating between a parameter validation issue and other application logic error.<br />While the options are numerous a few very common validation methods include the following.<br />***NotNull*** ensures that the parameter value provided is not null.<br />***NotNullOrWhitespace*** ensures that the string parameter is not null, empty or white space.<br />***NotLessThanTimeSpanSuccessTest*** makes sure the provided numeric value is not less than the specified value.| The guard method exceptions have the error messages stored in a resource file making it possible to localize if desired. |
|**ISecretManager**|Types that implement this interface provide easy access to retrieve secrets and certificates.| While no concrete implementation exists in this library others do offer it. |
|**SecureStringUtilities**|Helpers when working with secure strings. <br />***GetSecureString*** an extension method that creates a secure string object based on the contents of the string.<br />***SecureStringToString*** an extension method that creates a CLR string based on the secure string provided.| Careful attention should be taken when working with secure strings that the in memory clear text representations are contained and do not exist when possible. |
|**SharedAccessTokens**|Helper to generate, decompose and validate shared access tokens.<br />***GenerateSasToken*** creates a SAS token for the URI based on the specified policy.<br />***DecomposeSasToken*** breaks a SAS token into its elements.<br />***IsSignatureValid*** Validates the SAS token with a signature based on the provided key.|  |

## Sample Usage

### Retrieve Token Credential Provider Using Client Secrets

```csharp
var credential = AzureOauthTokenAuthentication.GetOauthTokenCredentialFromClientSecret(tenantId, clientId, clientSecret);
```

### Retrieve Token Credential Provider Using Managed Service Provider

```csharp
var credential = AzureOauthTokenAuthentication.GetOauthTokenCredentialFromManagedIdentity();
```

### Person Parameter is Not Null and Age Parameter  is Not Less Than 5

```csharp
void SetPersonAge(Person person, int age)
{
    Guard.NotNull(nameof(person), person);
    Guard.NotLessThanTimeSpanSuccessTest(nameof(age), age, 5);
}
```

### Retrieve a CLR String Value, Convert it to a Secure String and Back

```csharp
using praxicloud.core.security;

...

void TestConversion(string clearText)
{
    var secure = clearText.GetSecureString();
    var plainText = secure.SecureStringToString();
}
```

### Generate and Decompose a SAS Token

```csharp
using praxicloud.core.security;

public void GenerateAndDecomposeToken(string resourceUri, string key, string policyName, int expiryInSeconds)
{
    var token = SharedAccessTokens.GenerateSasToken(resourceUri, key, policyName, expiryInSeconds);

    Console.WriteLine(token);
    
    if(SharedAccessTokens.DecomposeSasToken(token, out var outputResourceUri, out var outputPolicyName, out var expiresAtTime, out var stringToValidate, out var signature))
    {
	Console.WriteLine($"The token was decomposed for resource {outputResourceUri}");
    }
}
```

## Additional Information

For additional information the Visual Studio generated documentation found [here](./documents/praxicloud.core/praxicloud.core.xml), can be viewed using your favorite documentation viewer.
