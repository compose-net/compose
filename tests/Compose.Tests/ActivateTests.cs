using FluentAssertions;
using System;
using System.Diagnostics;
using System.Reflection;
using Xunit;

namespace Compose.Tests
{
    public class ActivateTests
    {
        public class GivenAPublicClass
        {
            public class PublicClassWithPublicConstructors
            {
                internal PublicClassWithPublicConstructors() { }
                internal PublicClassWithPublicConstructors(int a) { }
                internal PublicClassWithPublicConstructors(string a) { }
            }
            public class PublicClassWithInternalConstructors
            {
                internal PublicClassWithInternalConstructors() { }
                internal PublicClassWithInternalConstructors(int a) { }
                internal PublicClassWithInternalConstructors(string a) { }
            }
            public class PublicClassWithPrivateConstructors
            {
                private PublicClassWithPrivateConstructors() { }
                private PublicClassWithPrivateConstructors(int a) { }
                private PublicClassWithPrivateConstructors(string a) { }
            }


            public class WithPublicConstructors
            {
                [Fact]
                public void WithNoConstructorArgumentsReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PublicClassWithPublicConstructors).GetTypeInfo());

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PublicClassWithPublicConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PublicClassWithPublicConstructors).GetTypeInfo(), 1);

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PublicClassWithPublicConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PublicClassWithPublicConstructors).GetTypeInfo(), "123");

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PublicClassWithPublicConstructors>();
                }
            }
            public class WithInternalConstructors
            {
                [Fact]
                public void WithNoConstructorArgumentsReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PublicClassWithInternalConstructors).GetTypeInfo());

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PublicClassWithInternalConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PublicClassWithInternalConstructors).GetTypeInfo(), 1);

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PublicClassWithInternalConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PublicClassWithInternalConstructors).GetTypeInfo(), "123");

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PublicClassWithInternalConstructors>();
                }
            }
            public class WithPrivateConstructors
            {
                [Fact]
                public void WithNoConstructorArgumentsReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PublicClassWithInternalConstructors).GetTypeInfo());

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PublicClassWithInternalConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PublicClassWithInternalConstructors).GetTypeInfo(), 1);

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PublicClassWithInternalConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PublicClassWithInternalConstructors).GetTypeInfo(), "123");

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PublicClassWithInternalConstructors>();
                }
            }
        }

        public class GivenAnInternalClass
        {
            public class InternalClassWithPublicConstructors
            {
                internal InternalClassWithPublicConstructors() { }
                internal InternalClassWithPublicConstructors(int a) { }
                internal InternalClassWithPublicConstructors(string a) { }
            }
            public class InternalClassWithInternalConstructors
            {
                internal InternalClassWithInternalConstructors() { }
                internal InternalClassWithInternalConstructors(int a) { }
                internal InternalClassWithInternalConstructors(string a) { }
            }
            public class InternalClassWithPrivateConstructors
            {
                private InternalClassWithPrivateConstructors() { }
                private InternalClassWithPrivateConstructors(int a) { }
                private InternalClassWithPrivateConstructors(string a) { }
            }


            public class WithPublicConstructors
            {
                [Fact]
                public void WithNoConstructorArgumentsReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(InternalClassWithPublicConstructors).GetTypeInfo());

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<InternalClassWithPublicConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(InternalClassWithPublicConstructors).GetTypeInfo(), 1);

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<InternalClassWithPublicConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(InternalClassWithPublicConstructors).GetTypeInfo(), "123");

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<InternalClassWithPublicConstructors>();
                }
            }
            public class WithInternalConstructors
            {
                [Fact]
                public void WithNoConstructorArgumentsReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(InternalClassWithInternalConstructors).GetTypeInfo());

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<InternalClassWithInternalConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(InternalClassWithInternalConstructors).GetTypeInfo(), 1);

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<InternalClassWithInternalConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(InternalClassWithInternalConstructors).GetTypeInfo(), "123");

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<InternalClassWithInternalConstructors>();
                }
            }
            public class WithPrivateConstructors
            {
                [Fact]
                public void WithNoConstructorArgumentsReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(InternalClassWithInternalConstructors).GetTypeInfo());

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<InternalClassWithInternalConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(InternalClassWithInternalConstructors).GetTypeInfo(), 1);

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<InternalClassWithInternalConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(InternalClassWithInternalConstructors).GetTypeInfo(), "123");

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<InternalClassWithInternalConstructors>();
                }
            }
        }

        public class GivenAPrivateClass
        {
            public class PrivateClassWithPublicConstructors
            {
                internal PrivateClassWithPublicConstructors() { }
                internal PrivateClassWithPublicConstructors(int a) { }
                internal PrivateClassWithPublicConstructors(string a) { }
            }
            public class PrivateClassWithInternalConstructors
            {
                internal PrivateClassWithInternalConstructors() { }
                internal PrivateClassWithInternalConstructors(int a) { }
                internal PrivateClassWithInternalConstructors(string a) { }
            }
            public class PrivateClassWithPrivateConstructors
            {
                private PrivateClassWithPrivateConstructors() { }
                private PrivateClassWithPrivateConstructors(int a) { }
                private PrivateClassWithPrivateConstructors(string a) { }
            }


            public class WithPublicConstructors
            {
                [Fact]
                public void WithNoConstructorArgumentsReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PrivateClassWithPublicConstructors).GetTypeInfo());

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PrivateClassWithPublicConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PrivateClassWithPublicConstructors).GetTypeInfo(), 1);

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PrivateClassWithPublicConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PrivateClassWithPublicConstructors).GetTypeInfo(), "123");

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PrivateClassWithPublicConstructors>();
                }
            }
            public class WithInternalConstructors
            {
                [Fact]
                public void WithNoConstructorArgumentsReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PrivateClassWithInternalConstructors).GetTypeInfo());

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PrivateClassWithInternalConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PrivateClassWithInternalConstructors).GetTypeInfo(), 1);

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PrivateClassWithInternalConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PrivateClassWithInternalConstructors).GetTypeInfo(), "123");

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PrivateClassWithInternalConstructors>();
                }
            }
            public class WithPrivateConstructors
            {
                [Fact]
                public void WithNoConstructorArgumentsReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PrivateClassWithInternalConstructors).GetTypeInfo());

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PrivateClassWithInternalConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PrivateClassWithInternalConstructors).GetTypeInfo(), 1);

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PrivateClassWithInternalConstructors>();
                }

                [Fact]
                public void WhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(PrivateClassWithInternalConstructors).GetTypeInfo(), "123");

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<PrivateClassWithInternalConstructors>();
                }
            }
        }

        public class GivenAGenericClass
        {
            public class GenericClassWithPublicConstructors<T>
            {
                internal GenericClassWithPublicConstructors() { }
                internal GenericClassWithPublicConstructors(int a) { }
                internal GenericClassWithPublicConstructors(string a) { }
            }
            public class GenericClassWithInternalConstructors<T>
            {
                internal GenericClassWithInternalConstructors() { }
                internal GenericClassWithInternalConstructors(int a) { }
                internal GenericClassWithInternalConstructors(string a) { }
            }
            public class GenericClassWithPrivateConstructors<T>
            {
                private GenericClassWithPrivateConstructors() { }
                private GenericClassWithPrivateConstructors(int a) { }
                private GenericClassWithPrivateConstructors(string a) { }
            }


            public class WithPublicConstructors
            {
                [Fact]
                public void WithNoConstructorArgumentsReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(GenericClassWithPublicConstructors<string>).GetTypeInfo());

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<GenericClassWithPublicConstructors<string>>();
                }

                [Fact]
                public void WhenPassedArgumentsThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(GenericClassWithPublicConstructors<string>).GetTypeInfo(), 1);

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<GenericClassWithPublicConstructors<string>>();
                }

                [Fact]
                public void WhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(GenericClassWithPublicConstructors<string>).GetTypeInfo(), "123");

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<GenericClassWithPublicConstructors<string>>();
                }
            }
            public class WithInternalConstructors
            {
                [Fact]
                public void WithNoConstructorArgumentsReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(GenericClassWithInternalConstructors<string>).GetTypeInfo());

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<GenericClassWithInternalConstructors<string>>();
                }

                [Fact]
                public void WhenPassedArgumentsThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(GenericClassWithInternalConstructors<string>).GetTypeInfo(), 1);

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<GenericClassWithInternalConstructors<string>>();
                }

                [Fact]
                public void WhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(GenericClassWithInternalConstructors<string>).GetTypeInfo(), "123");

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<GenericClassWithInternalConstructors<string>>();
                }
            }
            public class WithPrivateConstructors
            {
                [Fact]
                public void WithNoConstructorArgumentsReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(GenericClassWithInternalConstructors<string>).GetTypeInfo());

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<GenericClassWithInternalConstructors<string>>();
                }

                [Fact]
                public void WhenPassedArgumentsThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(GenericClassWithInternalConstructors<string>).GetTypeInfo(), 1);

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<GenericClassWithInternalConstructors<string>>();
                }

                [Fact]
                public void WhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
                {
                    var result = Activate.Type(typeof(GenericClassWithInternalConstructors<string>).GetTypeInfo(), "123");

                    result.Should().NotBeNull();
                    result.Should().BeAssignableTo<GenericClassWithInternalConstructors<string>>();
                }
            }
        }
    }
}