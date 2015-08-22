﻿
namespace DynamicSpecs.Core
{
    using System;

    /// <summary>
    /// Base class of all specifications, handling the basic workflow.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the system under test.
    /// </typeparam>
    public abstract class WorkflowSpecification<T> : ISpecify<T>
    {
        /// <summary>
        /// Gets or sets an Instance of the SUT.
        /// </summary>
        public T SUT { get; protected set; }

        /// <summary>
        /// Gets or sets the instance of the central type registration.
        /// </summary>
        private IRegisterTypes TypeRegistration { get; set; }

        /// <summary>
        /// Gets or sets the instance of the central type resolver.
        /// </summary>
        private IResolveTypes TypeResolver { get; set; }

        /// <summary>
        /// Method containing all code needed during the when phase.
        /// </summary>
        /// <param name="supporter">
        /// Class containing support code for a test run.
        /// </param>
        /// <returns>
        /// Instance of <see cref="ISupport"/>.
        /// </returns>
        public ISupport Given(ISupport supporter)
        {
            return this.InitializeSupportClass(supporter);
        }

        /// <summary>
        /// Method containing all code needed during the when phase.
        /// </summary>
        public virtual void Given()
        {
        }

        /// <summary>
        /// Method containing all code needed during the when phase.
        /// </summary>
        public virtual void When()
        {
        }

        /// <summary>
        /// Resolves the instance or mock instance of a given type.
        /// </summary>
        /// <typeparam name="TInstance">Type of the instance which shall be resolved.</typeparam>
        /// <returns>Instance of the given type.</returns>
        public TInstance GetInstance<TInstance>()
        {
            return this.TypeResolver.Resolve<TInstance>();
        }

        /// <summary>
        /// Executes the given support code after the SUT was instanciated and before the When phase.
        /// </summary>
        /// <typeparam name="TSupport">Type of the support class.</typeparam>
        /// <returns>Instance of the support class.</returns>
        public virtual TSupport Given<TSupport>() where TSupport : ISupport
        {
            var supporter = Activator.CreateInstance<TSupport>();
            this.InitializeSupportClass(supporter);

            return supporter;
        }

        /// <summary>
        /// This method is called by the child class to call <seealso cref="SetupEachSpec" /> when
        /// ever the testing framework starts a testrun for a particular spec.
        /// </summary>
        public abstract void Setup();

        /// <summary>
        /// Executes all needed code necessary for a test run of this instance in a particular order. 
        /// </summary>
        protected void SetupEachSpec()
        {
            this.Initialize();

            this.Given();

            this.When();
        }

        /// <summary>
        /// Gets the reference of the central type registration.
        /// </summary>
        /// <returns>
        /// The <see cref="IRegisterTypes"/>.
        /// </returns>
        protected abstract IRegisterTypes GetTypeRegistration();

        /// <summary>
        /// Gets the reference to the central instance with which types can be resolved as instances.
        /// </summary>
        /// <returns>
        /// The <see cref="IResolveTypes"/>.
        /// </returns>
        protected abstract IResolveTypes GetTypeResolver();

        /// <summary>
        /// Registers all types needed by the SUT at a central registration or container.
        /// </summary>
        /// <param name="typeRegistration">
        /// Instance which shall contain the registered types.
        /// </param>
        protected virtual void RegisterTypes(IRegisterTypes typeRegistration)
        {
        }

        /// <summary>
        /// Creates the system Under Test and resolves all it's dependencies.
        /// </summary>
        /// <returns>Instance of the SUT.</returns>
        protected virtual T CreateSut()
        {
            return this.TypeResolver.Resolve<T>();
        }

        /// <summary>
        /// Initializes the specification instance.
        /// </summary>
        private void Initialize()
        {
            this.TypeRegistration = this.GetTypeRegistration();

            this.RegisterTypes(this.TypeRegistration);

            this.TypeResolver = this.GetTypeResolver();

            this.SUT = this.CreateSut();
        }

        /// <summary>
        /// Initializes the given support class.
        /// </summary>
        /// <param name="supporter">Class which contains code to support a test run.</param>
        /// <returns>Instance of <see cref="ISupport"/></returns>
        private ISupport InitializeSupportClass(ISupport supporter)
        {
            supporter.Support(this);
            return supporter;
        }
    }
}