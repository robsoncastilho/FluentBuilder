using Nosbor.FluentBuilder.Lib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nosbor.FluentBuilder.Mock.Moq;

namespace Nosbor.FluentBuilder.Tests.Mock.Moq
{
    [TestFixture]
    public class FluentBuilderMoqExtensionsTest
    {
        [Test]
        public void Should_configure_stub()
        {
            var stubReturnValue = 1;

            var createdObject = FluentBuilder<SampleTypeWithDependency>
                .New()
                .WithStubFor<ISomeDependency>(stub => stub.FindById(1))
                .Build();
        }
    }

    internal class SampleTypeWithDependency
    {
        public SampleTypeWithDependency(ISomeDependency someDependency)
        {

        }
    }

    internal interface ISomeDependency
    {
        int FindById(int id);
    }
}
