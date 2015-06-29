[![Build status](https://ci.appveyor.com/api/projects/status/vmkcqc500wbj3krr?svg=true)](https://ci.appveyor.com/project/robsoncastilho/fluentbuilder)

# FluentBuilder

Library for dynamic creation of objects, implemented with a focus on writing more readable and less fragile unit tests.

### Available at NuGet:
```powershell
Install-Package Nosbor.FluentBuilder
```

### Features

####1. Public API

#####1.1. With

Allows setting values for writable properties (ie. properties must have 'set' with any kind of modifier):

```csharp
var newAddress = FluentBuilder<Address>
                  .New()
                  .With(a => a.Street, "1st Street")
                  .With(a => a.ZipCode, "1011")
                  .Build();
```

#####1.2. WithDependency

Allows setting values for a injected dependency stored in a private field.
This option allows creating a builder for a SUT (a service that has dependencies) and passing a test double object to the SUT.

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

#####1.3. AddingTo

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

**** IN CONSTRUCTION....
