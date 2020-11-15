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




