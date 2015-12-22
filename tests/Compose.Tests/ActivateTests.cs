using Compose;
using FluentAssertions;
using System.Reflection;
using Xunit;

namespace Compose.Tests
{
    public class ActivateTests
    {
        public class GivenAPublicClass
        {
            public class PublicClass
            { }

            public class PublicClassWithPrivateConstructors
            {
                private PublicClassWithPrivateConstructors() { }
                private PublicClassWithPrivateConstructors(int a) { }
                private PublicClassWithPrivateConstructors(string a) { }
            }

            [Fact]
            public void WithNoConstructorArgumentsThenReturnsObjectOfType()
            {
                var result = Activate.Type(typeof(PublicClass).GetTypeInfo());

                result.Should().NotBeNull();
                result.Should().BeAssignableTo<PublicClass>();
            }

            [Fact]
            public void WithPrivateConstructorThenReturnsObjectOfType()
            {
                var result = Activate.Type(typeof(PublicClassWithPrivateConstructors).GetTypeInfo());

                result.Should().NotBeNull();
                result.Should().BeAssignableTo<PublicClassWithPrivateConstructors>();
            }

            [Fact]
            public void WithPrivateConstructorWhenPassedArgumentsThenReturnsObjectOfType()
            {
                var result = Activate.Type(typeof(PublicClassWithPrivateConstructors).GetTypeInfo(), 1);

                result.Should().NotBeNull();
                result.Should().BeAssignableTo<PublicClassWithPrivateConstructors>();
            }
        }

        public class GivenAnInternalClass
        {
            internal class InternalClass
            { }

            [Fact]
            public void WithNoConstructorArgumentsThenReturnsObjectOfType()
            {
                var result = Activate.Type(typeof(InternalClass).GetTypeInfo());

                result.Should().NotBeNull();
                result.Should().BeAssignableTo<InternalClass>();
            }
        }

        public class GivenAPrivateClass
        {
            private class PrivateClass
            { }

            [Fact]
            public void WithNoConstructorArgumentsThenReturnsObjectOfType()
            {
                var result = Activate.Type(typeof(PrivateClass).GetTypeInfo());

                result.Should().NotBeNull();
                result.Should().BeAssignableTo<PrivateClass>();
            }
        }
    }
}
