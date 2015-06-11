# FluentBuilder

Library for dynamic creation of objects, implemented with a focus on writing more readable and less fragile unit tests.

### Available at NuGet:
Install-Package Nosbor.FluentBuilder

### Features

##### With

var newAddress = FluentBuilder<Address>
                .New()
                .With(a => a.Street)
                .With(a => a.ZipCode)
                .Build();

### WARNING !!
**** BETA VERSION / Some features not working as expected yet (see ignored tests) ****
