using FluentAssertions;
using System.Reflection;
using TestAttributes;

namespace Compose.Tests
{
    public class ActivateTests
    {
        #region "Test Classes"
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
        public class PublicClassWithNoParameterlessConstructor
        {
            public PublicClassWithNoParameterlessConstructor(int a, string b) { }
        }
        #endregion

        [Unit]
        public void GivenAPublicClassWithPublicConstructorsWithNoConstructorArgumentsReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PublicClassWithPublicConstructors).GetTypeInfo());

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PublicClassWithPublicConstructors>();
        }

        [Unit]
        public void GivenAPublicClassWithPublicConstructorsWhenPassedArgumentsThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PublicClassWithPublicConstructors).GetTypeInfo(), 1);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PublicClassWithPublicConstructors>();
        }

        [Unit]
        public void GivenAPublicClassWithPublicConstructorsWhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PublicClassWithPublicConstructors).GetTypeInfo(), "123");

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PublicClassWithPublicConstructors>();
        }

        [Unit]
        public void GivenAPublicClassWithInternalConstructorsWithNoConstructorArgumentsReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PublicClassWithInternalConstructors).GetTypeInfo());

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PublicClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAPublicClassWithInternalConstructorsWhenPassedArgumentsThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PublicClassWithInternalConstructors).GetTypeInfo(), 1);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PublicClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAPublicClassWithInternalConstructorsWhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PublicClassWithInternalConstructors).GetTypeInfo(), "123");

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PublicClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAPublicClassWithInternalConstructorsGivenAPublicClassWithPrivateConstructorsWithNoConstructorArgumentsReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PublicClassWithInternalConstructors).GetTypeInfo());

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PublicClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAPublicClassWithPrivateConstructorsWhenPassedArgumentsThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PublicClassWithInternalConstructors).GetTypeInfo(), 1);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PublicClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAPublicClassWithPrivateConstructorsWhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PublicClassWithInternalConstructors).GetTypeInfo(), "123");

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PublicClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAnInternalClassWithPublicConstructorsWithNoConstructorArgumentsReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(InternalClassWithPublicConstructors).GetTypeInfo());

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<InternalClassWithPublicConstructors>();
        }

        [Unit]
        public void GivenAnInternalClassWithPublicConstructorsWhenPassedArgumentsThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(InternalClassWithPublicConstructors).GetTypeInfo(), 1);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<InternalClassWithPublicConstructors>();
        }

        [Unit]
        public void GivenAnInternalClassWithPublicConstructorsWhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(InternalClassWithPublicConstructors).GetTypeInfo(), "123");

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<InternalClassWithPublicConstructors>();
        }

        [Unit]
        public void GivenAnInternalClassWithInternalConstructorsWithNoConstructorArgumentsReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(InternalClassWithInternalConstructors).GetTypeInfo());

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<InternalClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAnInternalClassWithInternalConstructorsWhenPassedArgumentsThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(InternalClassWithInternalConstructors).GetTypeInfo(), 1);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<InternalClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAnInternalClassWithInternalConstructorsWhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(InternalClassWithInternalConstructors).GetTypeInfo(), "123");

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<InternalClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAnInternalClassWithPrivateConstructorsWithNoConstructorArgumentsReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(InternalClassWithInternalConstructors).GetTypeInfo());

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<InternalClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAnInternalClassWithPrivateConstructorsWhenPassedArgumentsThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(InternalClassWithInternalConstructors).GetTypeInfo(), 1);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<InternalClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAnInternalClassWithPrivateConstructorsWhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(InternalClassWithInternalConstructors).GetTypeInfo(), "123");

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<InternalClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAPrivateClassWithPublicConstructorsWithNoConstructorArgumentsReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PrivateClassWithPublicConstructors).GetTypeInfo());

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PrivateClassWithPublicConstructors>();
        }

        [Unit]
        public void GivenAPrivateClassWithPublicConstructorsWhenPassedArgumentsThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PrivateClassWithPublicConstructors).GetTypeInfo(), 1);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PrivateClassWithPublicConstructors>();
        }

        [Unit]
        public void GivenAPrivateClassWithPublicConstructorsWhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PrivateClassWithPublicConstructors).GetTypeInfo(), "123");

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PrivateClassWithPublicConstructors>();
        }

        [Unit]
        public void GivenAPrivateClassWithInternalConstructorsWithNoConstructorArgumentsReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PrivateClassWithInternalConstructors).GetTypeInfo());

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PrivateClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAPrivateClassWithInternalConstructorsWhenPassedArgumentsThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PrivateClassWithInternalConstructors).GetTypeInfo(), 1);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PrivateClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAPrivateClassWithInternalConstructorsWhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PrivateClassWithInternalConstructors).GetTypeInfo(), "123");

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PrivateClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAPrivateClassWithPrivateConstructorsWithNoConstructorArgumentsReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PrivateClassWithInternalConstructors).GetTypeInfo());

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PrivateClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAPrivateClassWithPrivateConstructorsWhenPassedArgumentsThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PrivateClassWithInternalConstructors).GetTypeInfo(), 1);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PrivateClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAPrivateClassWithPrivateConstructorsWhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(PrivateClassWithInternalConstructors).GetTypeInfo(), "123");

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PrivateClassWithInternalConstructors>();
        }

        [Unit]
        public void GivenAGenericClassWithPublicConstructorsWithNoConstructorArgumentsReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(GenericClassWithPublicConstructors<string>).GetTypeInfo());

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<GenericClassWithPublicConstructors<string>>();
        }

        [Unit]
        public void GivenAGenericClassWithPublicConstructorsWhenPassedArgumentsThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(GenericClassWithPublicConstructors<string>).GetTypeInfo(), 1);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<GenericClassWithPublicConstructors<string>>();
        }

        [Unit]
        public void GivenAGenericClassWithPublicConstructorsWhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(GenericClassWithPublicConstructors<string>).GetTypeInfo(), "123");

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<GenericClassWithPublicConstructors<string>>();
        }

        [Unit]
        public void GivenAGenericClassWithInternalConstructorsWithNoConstructorArgumentsReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(GenericClassWithInternalConstructors<string>).GetTypeInfo());

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<GenericClassWithInternalConstructors<string>>();
        }

        [Unit]
        public void GivenAGenericClassWithInternalConstructorsWhenPassedArgumentsThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(GenericClassWithInternalConstructors<string>).GetTypeInfo(), 1);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<GenericClassWithInternalConstructors<string>>();
        }

        [Unit]
        public void GivenAGenericClassWithInternalConstructorsWhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(GenericClassWithInternalConstructors<string>).GetTypeInfo(), "123");

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<GenericClassWithInternalConstructors<string>>();
        }

        [Unit]
        public void GivenAGenericClassWithPrivateConstructorsWithNoConstructorArgumentsReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(GenericClassWithInternalConstructors<string>).GetTypeInfo());

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<GenericClassWithInternalConstructors<string>>();
        }

        [Unit]
        public void GivenAGenericClassWithPrivateConstructorsWhenPassedArgumentsThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(GenericClassWithInternalConstructors<string>).GetTypeInfo(), 1);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<GenericClassWithInternalConstructors<string>>();
        }

        [Unit]
        public void GivenAGenericClassWithPrivateConstructorsWhenPassedArgumentsForAlternativeConstructorThenReturnsObjectOfType()
        {
            var result = Activate.Type(typeof(GenericClassWithInternalConstructors<string>).GetTypeInfo(), "123");

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<GenericClassWithInternalConstructors<string>>();
        }

        [Unit]
        public void GivenAPublicClassWithNoParameterlessConstructorThenReturnObjectOfType()
        {
            var result = Activate.Type(typeof(PublicClassWithNoParameterlessConstructor).GetTypeInfo(), 2, "abc");

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PublicClassWithNoParameterlessConstructor>();
        }
    }
}