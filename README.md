# FluentBuilder

Library for dynamic creation of objects, implemented with a focus on writing more readable and less fragile unit tests.

### Available at NuGet:
Install-Package Nosbor.FluentBuilder

### Features

####1. With

Allows setting values for writable properties (ie. properties must have 'set' with any kind of modifier)

Sample:
```
var newAddress = FluentBuilder<Address>
                  .New()
                  .With(a => a.Street, "1st Street")
                  .With(a => a.ZipCode, "1011")
                  .Build();
```

