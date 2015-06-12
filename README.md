# FluentBuilder

Library for dynamic creation of objects, implemented with a focus on writing more readable and less fragile unit tests.

### Available at NuGet:
Install-Package Nosbor.FluentBuilder

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

Sample 1 : using a concrete dependency for integration tests

```
var concreteService = new SampleConcreteDependency();
var createdObject = FluentBuilder<SampleServiceWithDependency>
    .New()
    .WithDependency<IDependency, SampleConcreteDependency>(concreteService)
    .Build();
```
**** IN CONSTRUCTION....
