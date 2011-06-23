using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChelasInjection.Tests
{
    using Exceptions;
    using SampleTypes;
    using ChelasInjection.SampleTypes.Attributes;

    [TestClass]
    public class UnitTest1
    {
        private Injector _injector = new Injector(new MyBinder());

        [TestMethod]
        public void TestMethod1()
        {
        }
        #region Instance creation

        [TestMethod]
        public void ShouldCreateASomeInterface1ImplInstanceForISomeInterface1Request()
        {
            // Arrange


            // Act
            ISomeInterface1 i1impl = _injector.GetInstance<ISomeInterface1>();

            // Assert

            Assert.AreEqual(i1impl.GetType(), typeof(SomeInterface1Impl));
        }

        [TestMethod]
        public void ShouldCreateASomeInterface2ImplInstanceForISomeInterface2RequestAndAlsoCreateItsDependencies()
        {

            ISomeInterface2 i2impl = _injector.GetInstance<ISomeInterface2>();

            Assert.AreEqual(i2impl.GetType(), typeof(SomeInterface2Impl));
            Assert.IsNotNull(i2impl.I1);
        }


        [TestMethod]
        public void ShouldCreateASomeInterface3ImplInstanceForISomeInterface2RequestAndAlsoCreateItsDependencies()
        {

            ISomeInterface3 i3impl = _injector.GetInstance<ISomeInterface3>();

            Assert.AreEqual(i3impl.GetType(), typeof(SomeInterface3Impl));
            Assert.IsNotNull(i3impl.I1);
            Assert.IsNotNull(i3impl.I2);
            Assert.IsNotNull(i3impl.I2.I1);
        }

        [TestMethod]
        public void ShouldCreateASomeClass4InstanceForSomeClass4Request()
        {
            // Arrange

            // Act
            SomeClass4 sc4 = _injector.GetInstance<SomeClass4>();

            // Assert
            Assert.AreEqual(sc4.GetType(), typeof(SomeClass4));
        }

        [TestMethod, ExpectedException(typeof(CircularDependencyException))]
        public void ShouldThrowCircularDependencyExceptionOnInstantiationForSomeClass1()
        {
            // Arrange

            // Act
            SomeClass1 sc1 = _injector.GetInstance<SomeClass1>();

            // Assert
            Assert.Fail("The Injector.GetInstance should have thrown a CircularDependencyException");
        }

        [TestMethod, ExpectedException(typeof(UnboundTypeException))]
        public void ShouldThrowUnboundTypeExceptionForAnUnboundAbstractType()
        {
            // Arrange

            // Act
            _injector.GetInstance<ISomeInterface6>();

            // Assert
            Assert.Fail("The Injector.GetInstance should have thrown a UnboundTypeException");
        }


        [TestMethod]
        public void ShouldCreateAnInstanceForAnUnboundTypeWithPublicConstructors()
        {
            // Arrange

            // Act
            SomeClass5 sc5 = _injector.GetInstance<SomeClass5>();

            // Assert
            Assert.IsNotNull(sc5);
        }


        [TestMethod]
        public void ShouldCreateASomeClass9InstanceForSomeClass9RequestWithSelfBindConfiguration()
        {
            // Arrange

            // Act
            SomeClass9 sc9 = _injector.GetInstance<SomeClass9>();

            // Assert
            Assert.IsNotNull(sc9);
        }

        #endregion Instance creation

        #region Activation tests
        [TestMethod]
        public void ShouldHavePerRequestAsDefaultActivation()
        {
            // Arrange

            // Act
            ISomeInterface1 i1impl = _injector.GetInstance<ISomeInterface1>();
            ISomeInterface1 i1impl1 = _injector.GetInstance<ISomeInterface1>();


            // Assert
            Assert.AreNotSame(i1impl, i1impl1);
        }


        [TestMethod]
        public void ShouldHaveDefautActivationOnDependencies()
        {
            // Arrange

            // Act
            ISomeInterface2 i2impl = _injector.GetInstance<ISomeInterface2>();
            ISomeInterface2 i2impl1 = _injector.GetInstance<ISomeInterface2>();


            // Assert
            Assert.AreNotSame(i2impl.I1, i2impl1.I1);
        }

        [TestMethod]
        public void ShouldHavePerRequestActivationForISomeInterface2Request()
        {
            // Arrange

            // Act
            ISomeInterface2 i2impl = _injector.GetInstance<ISomeInterface2>();
            ISomeInterface2 i2impl1 = _injector.GetInstance<ISomeInterface2>();


            // Assert
            Assert.AreNotSame(i2impl, i2impl1);

        }


        [TestMethod]
        public void ShouldHaveSingletonActivationForISomeInterface3Request()
        {
            // Arrange

            // Act
            ISomeInterface3 i3impl = _injector.GetInstance<ISomeInterface3>();
            ISomeInterface3 i3impl1 = _injector.GetInstance<ISomeInterface3>();


            // Assert
            Assert.AreSame(i3impl, i3impl1);
        }

        [TestMethod]
        public void ShouldHavePerCallActivationForDependenciesISomeInterface3Instances()
        {
            // Arrange

            // Act
            ISomeInterface3 i3impl = _injector.GetInstance<ISomeInterface3>();


            // Assert
            Assert.AreSame(i3impl.I1, i3impl.I2.I1);

        }

        #endregion Activation tests

        #region Instance Initialization & construction
        [TestMethod]
        public void ShouldExecuteInitializationExpressionForISomeInterface2Impl()
        {
            // Arrange

            // Act
            ISomeInterface2 i2impl = _injector.GetInstance<ISomeInterface2>();


            // Assert
            Assert.AreEqual(1, i2impl.SomeInt);
            Assert.AreEqual("Initialized", i2impl.SomeStr);

        }


        [TestMethod]
        public void ShouldChooseTheConstructorWithMoreBindedArgumenstByDefault()
        {
            // Arrange

            // Act
            ISomeInterface4 i4impl = _injector.GetInstance<ISomeInterface4>();


            // Assert
            Assert.IsNotNull(i4impl.I1);
            Assert.IsNotNull(i4impl.I2);

        }

        [TestMethod]
        public void ShouldChooseTheConstructorAnotatedWithDefaulAttributeForISomeInterface5()
        {
            // Arrange

            // Act
            ISomeInterface5 i5impl = _injector.GetInstance<ISomeInterface5>();


            // Assert
            Assert.IsNotNull(i5impl.I1);
            Assert.IsNull(i5impl.I2);

        }

        [TestMethod, ExpectedException(typeof(MultipleDefaultConstructorAttributesException))]
        public void ShouldThrowMultipleDefaultConstructorAttributesExceptionWhenForSomeClass10BecauseHasMultipleConstructorsWithDefaultConstructorAttribute()
        {
            // Arrange

            // Act
            _injector.GetInstance<SomeClass10>();


            // Assert
        }

        [TestMethod]
        public void ShouldChooseTheCorrectConstructorWhenWithConstructorOperatorIsUsedForISomeInterface2()
        {
            // Arrange

            // Act
            ISomeInterface3 i3impl = _injector.GetInstance<ISomeInterface3>();


            // Assert
            Assert.IsNotNull(i3impl.I1);
            Assert.IsNotNull(i3impl.I2);
            Assert.AreEqual(12, i3impl.P1);
            Assert.AreEqual("SLB", i3impl.P3);

        }

        [TestMethod]
        public void ShouldChooseTheNoArgumentsConstructorWhenNoArgumentsConstructorOperatorIsUsedForISomeInterface1()
        {
            // Arrange

            // Act
            ISomeInterface1 i1impl = _injector.GetInstance<ISomeInterface1>();


            // Assert
            // If no exception is thrown, the implementation has the correct behaviour 
            // (See public SomeInterface1Impl.SomeInterface1Impl(ISomeInterface5 i5) implementation) 
        }

        #endregion Instance Initialization & construction

        #region Multiple specifications

        [TestMethod]
        public void ShouldUseTheLastBindForISomeInterface4()
        {
            // Arrange

            // Act
            ISomeInterface4 i4impl = _injector.GetInstance<ISomeInterface4>();


            // Assert
            Assert.IsNotNull(i4impl.I1);
            Assert.IsNotNull(i4impl.I2);

        }


        [TestMethod]
        public void ShouldUseTheLastConstructorDefinitionInOneBindForISomeInterface3()
        {
            // Arrange

            // Act
            ISomeInterface3 i3impl = _injector.GetInstance<ISomeInterface3>();

            // Assert
            Assert.IsNotNull(i3impl.I1);
            Assert.IsNotNull(i3impl.I2);
            Assert.AreEqual(12, i3impl.P1);
            Assert.AreEqual("SLB", i3impl.P3);
        }

        #endregion Multiple specifications

        #region Custom Resolver tests
        [TestMethod]
        public void ShouldReturnSomeClass6NotBoundAndCreatedByTheCustomResolver()
        {
            // Arrange

            // Act
            SomeClass6 sc6 = _injector.GetInstance<SomeClass6>();

            // Assert
            Assert.IsNotNull(sc6);
        }


        [TestMethod]
        public void ShouldGivePriorirtyToCustomResolverAlthoughSomeClass7IsBound()
        {
            // Arrange

            // Act
            SomeClass7 sc7 = _injector.GetInstance<SomeClass7>();

            // Assert
            Assert.IsNotNull(sc7);
            Assert.IsNull(sc7.Sc6);
        }


        [TestMethod]
        public void ShouldMaintainActivationEvenForSomeClass7ProvidedByCustomResolver()
        {
            // Arrange

            // Act
            SomeClass7 sc7 = _injector.GetInstance<SomeClass7>();
            SomeClass7 sc71 = _injector.GetInstance<SomeClass7>();

            // Assert
            Assert.IsNotNull(sc7);
            Assert.AreSame(sc7, sc71);
        }

        [TestMethod]
        public void ShouldHavePerRequestActivationForNotBoundClass8ButWithCustomResolverProvidedInstance()
        {
            // Arrange

            // Act
            SomeClass8 sc8 = _injector.GetInstance<SomeClass8>();

            // Assert
            Assert.IsNotNull(sc8);
            Assert.IsNotNull(sc8.Sc61);
            Assert.AreSame(sc8.Sc61, sc8.Sc62);
        }

        [TestMethod]
        public void ShouldHaveSingletonActivationForSomeClass7CustomResolverProvidedInstances()
        {
            // Arrange

            // Act
            SomeClass7 sc7 = _injector.GetInstance<SomeClass7>();
            SomeClass7 sc71 = _injector.GetInstance<SomeClass7>();

            // Assert
            Assert.IsNotNull(sc7);
            Assert.AreSame(sc7, sc71);
        }
        #endregion Custom Resolver tests

        #region Multiple Binding

        [TestMethod]
        public void ShouldChooseSomeInterface10RedImplementationForDependencyOnSomeClass11()
        {
            // Arrange

            // Act
            SomeClass11 sc11 = _injector.GetInstance<SomeClass11>();


            // Assert
            Assert.IsNotNull(sc11);
            Assert.AreSame(typeof(SomeInterface7And8Red), sc11.Si7.GetType());

        }


        [TestMethod]
        public void ShouldChooseSomeInterface10YellowImplementationForDependencyOnSomeClass12()
        {
            // Arrange

            // Act
            SomeClass12 sc12 = _injector.GetInstance<SomeClass12>();


            // Assert
            Assert.IsNotNull(sc12);
            Assert.AreSame(typeof(SomeInterface7And8Yellow), sc12.Si7.GetType());

        }

        [TestMethod]
        public void ShouldReturnSomeInterface7DefaultForSomeInterface7DependencyOnSomeClass13WithNotConfiguredBlackAttribute()
        {
            // Arrange

            // Act
            SomeClass13 sc13 = _injector.GetInstance<SomeClass13>();

            // Assert
            Assert.AreSame(typeof(SomeInterface7Default), sc13.Si7.GetType());

        }


        [TestMethod]
        public void ShouldReturnSomeInterface7DefaultForSomeInterface7DependencyOnSomeClass14WithNoArgumentAttribute()
        {
            // Arrange

            // Act
            SomeClass14 sc14 = _injector.GetInstance<SomeClass14>();

            // Assert
            Assert.AreSame(typeof(SomeInterface7Default), sc14.Si7.GetType());

        }

        [TestMethod]
        public void ShouldReturnSomeInterface7DefaultForSomeInterface7RequestWhenNoSelectionAttributeIsdefined()
        {
            // Arrange

            // Act
            ISomeInterface7 isi7 = _injector.GetInstance<ISomeInterface7>();

            // Assert
            Assert.AreSame(typeof(SomeInterface7Default), isi7.GetType());

        }

        [TestMethod]
        public void ShouldReturnSomeInterface7And8RedForSomeInterface7RequestWhenSelectionAttributeIsdefined()
        {
            // Arrange

            // Act
            ISomeInterface7 isi7 = _injector.GetInstance<ISomeInterface7, RedAttribute>();

            // Assert
            Assert.AreSame(typeof(SomeInterface7And8Red), isi7.GetType());

        }


        [TestMethod, ExpectedException(typeof(UnboundTypeException))]
        public void ShouldThrowUnboundExceptionForISomeInterface8Request()
        {
            // Arrange

            // Act
            _injector.GetInstance<ISomeInterface8>();

            // Assert
        }


        [TestMethod]
        public void ShouldReturnSomeInterface7And8RedForSomeInterface8RequestWhenSelectionAttributeIsdefined()
        {
            // Arrange

            // Act
            ISomeInterface8 isi8 = _injector.GetInstance<ISomeInterface8, RedAttribute>();

            // Assert
            Assert.AreSame(typeof(SomeInterface7And8Red), isi8.GetType());

        }
        #endregion Multiple Binding

    }
}
