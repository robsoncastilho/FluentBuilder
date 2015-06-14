# FluentBuilder

Library for dynamic creation of objects, implemented with a focus on writing more readable and less fragile unit tests.

### Available at NuGet:
```
Install-Package Nosbor.FluentBuilder
```

### Features

####1. With

Allows setting values for writable properties (ie. properties must have 'set' with any kind of modifier):
```
var newAddress = FluentBuilder<Address>
                  .New()
                  .With(a => a.Street, "1st Street")
                  .With(a => a.ZipCode, "1011")
                  .Build();
```

####2. WithDependency

Allows setting values for a injected dependency stored in a private field.
This option allows creating a builder for a SUT (a service that has dependencies) and passing a test double object to the SUT.

Sample 1 (Using a concrete dependency for integration tests):

```
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

```
var dependencyMock = new Mock<IDependency>();
var service = FluentBuilder<SampleServiceWithDependency>
    .New()
    .WithDependency<IDependency, IDependency>(dependencyMock.Object)
    .Build();
    
service.DoSomething();

dependencyMock.Verify(dependency => dependency.Do(), Times.Once);
......
```

####3. AddingTo

**** IN CONSTRUCTION....
