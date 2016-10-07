[![Build status](https://ci.appveyor.com/api/projects/status/vmkcqc500wbj3krr?svg=true)](https://ci.appveyor.com/project/robsoncastilho/fluentbuilder)
[![Coverage Status](https://coveralls.io/repos/robsoncastilho/FluentBuilder/badge.svg?branch=master&service=github)](https://coveralls.io/github/robsoncastilho/FluentBuilder?branch=master)
[![NuGet Badge](https://buildstats.info/nuget/Nosbor.FluentBuilder)](https://www.nuget.org/packages/Nosbor.FluentBuilder/)
# FluentBuilder

Library for dynamic creation of objects, implemented with a focus on writing more readable and less fragile unit tests.

### Available at NuGet
```powershell
Install-Package Nosbor.FluentBuilder
```

### Compatibility

.Net Framework 4.5 or higher.
.Net Core 1.0

### Features

####1. Public API

#####1.1. New

Creates a new FluentBuilder instance and starts the fluent construction of the destination object (see next sections).

#####1.2. With

Allows setting values for writable properties (ie. properties must have 'set' with any kind of modifier):

```csharp
var newAddress = FluentBuilder<Address>
                .New()
                .With(a => a.Street, "1st Street")
                .With(a => a.ZipCode, "1011")
                .Build();
```

#####1.3. WithValue

Allows setting a value (reference type) just passing its instance. FluentBuilder will find the corresponding member to receive the instance based on naming convention:

```csharp
var newCustomer = FluentBuilder<Customer>
                  .New()
                  .WithValue(FluentBuilder<Address>.New().Build())
                  .Build();
```

For code above succeed, Customer class must have a property or field named 'Address' or 'address' and being of the same type of Address (or a base class of Address).

#####1.4. WithDependency

Allows setting values for a injected dependency stored in a private field.
This option allows creating a builder for a service that has dependencies and passing a test double object to the SUT.

Sample 1 (Using a concrete dependency for integration tests):

```csharp
var concreteDependency = new SampleConcreteDependency();
var service = FluentBuilder<SampleServiceWithDependency>
              .New()
              .WithDependency<IDependency, SampleConcreteDependency>(concreteDependency)
              .Build();
    
service.DoSomething();
......
```

Sample 2 (Using a mock for unit tests):
Using Moq, but you can use another mocking library or manually implement your mock object.

```csharp
var dependencyMock = new Mock<IDependency>();
var service = FluentBuilder<SampleServiceWithDependency>
              .New()
              .WithDependency<IDependency, IDependency>(dependencyMock.Object)
              .Build();
    
service.DoSomething();

dependencyMock.Verify(dependency => dependency.Do(), Times.Once);
......
```

#####1.5. AddingTo

Allows setting elements in a collection, one by one:

```csharp
var customer = FluentBuilder<Customer>
              .New()
              .AddingTo(c => c.Addresses, new Address("1st Street"))
              .AddingTo(c => c.Addresses, new Address("2nd Street"))
              .Build();
```

Or:

```csharp
var customer = FluentBuilder<Customer>
              .New()
              .AddingTo(c => c.Addresses, FluentBuilder<Address>.New().With(a => a.Street, "1st Street").Build())
              .AddingTo(c => c.Addresses, FluentBuilder<Address>.New().With(a => a.Street, "2nd Street").Build())
              .Build();
```

The collection must be a read-only collection with a backing private field. If the collection is a writable property then you can set the whole collection using "With()".

#####1.6. Build

Constructs and returns the expected object.

#####1.7. AsList

Like Build() method, with the difference that AsList() builds and returns the object within a list:

```csharp
IEnumerable<Address> addresses = FluentBuilder<Address>
                                .New()
                                .With(a => a.Street, "1st Street")
                                .AsList();
```

It's helpful when just one object is necessary for the test but the method/construtor of the SUT requires a collection of objects of that type.

####2. Implicit conversion

Allows calling Build() to be avoided since you use the returned type explicitly instead of 'var':

```csharp
Address address = FluentBuilder<Address>.New().With(a => a.Street, "1st Street");
```

####3. Extensions
(coming soon)
