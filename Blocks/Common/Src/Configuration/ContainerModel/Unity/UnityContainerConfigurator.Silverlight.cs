﻿//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.Unity;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.Unity
{
    /// <summary>
    /// The <see cref="UnityContainer"/> specific configurator for <see cref="TypeRegistration"/> entries.
    /// </summary>
    public partial class UnityContainerConfigurator : IContainerConfigurator
    {
        /// <summary>
        /// Initializer for the configurator.
        /// </summary>
        /// <param name="container">The <see cref="IUnityContainer"/> to configure.</param>
        public UnityContainerConfigurator(IUnityContainer container)
        {
            this.container = container;

            //this.container.AddNewExtensionIfNotPresent<Interception>();
            this.container.AddNewExtensionIfNotPresent<TransientPolicyBuildUpExtension>();
            //this.container.AddNewExtensionIfNotPresent<ReaderWriterLockExtension>();
            this.container.AddNewExtensionIfNotPresent<LifetimeInspector>();
            this.container.AddNewExtensionIfNotPresent<PolicyListAccessor>();
            AddValidationExtension();
        }

        /// <summary>
        /// Consume the set of <see cref="TypeRegistration"/> objects and
        /// configure the associated container.
        /// </summary>
        /// <param name="configurationSource">Configuration source to read registrations from.</param>
        /// <param name="rootProvider"><see cref="ITypeRegistrationsProvider"/> that knows how to
        /// read the <paramref name="configurationSource"/> and return all relevant type registrations.</param>
        public void RegisterAll(IConfigurationSource configurationSource, ITypeRegistrationsProvider rootProvider)
        {
            this.RegisterConfigurationSource(configurationSource);
            this.RegisterAllCore(configurationSource, rootProvider);
        }

        /// <summary>
        /// Consume the set of <see cref="TypeRegistration"/> objects and
        /// configure the associated container.
        /// </summary>
        /// <param name="configurationSource">Configuration source to read registrations from.</param>
        /// <param name="rootProvider"><see cref="ITypeRegistrationsProvider"/> that knows how to
        /// read the <paramref name="configurationSource"/> and return all relevant type registrations.</param>
        protected void RegisterAllCore(IConfigurationSource configurationSource, ITypeRegistrationsProvider rootProvider)
        {
            foreach (var registration in rootProvider.GetRegistrations(configurationSource))
            {
                Register(registration);
            }
        }

        private void AddValidationExtension()
        {
            // We load this by name so we don't have a hard dependency from common -> validation
            const string fullExtensionTypeName =
                "Microsoft.Practices.EnterpriseLibrary.Validation.Configuration.Unity.ValidationBlockExtension, Microsoft.Practices.EnterpriseLibrary.Validation.Silverlight, Culture=neutral, Version=5.0.414.0, PublicKeyToken=null";

            var extensionType = Type.GetType(fullExtensionTypeName);
            if (extensionType != null && container.Configure(extensionType) == null)
            {
                var vabExtension = (UnityContainerExtension)Activator.CreateInstance(extensionType);
                container.AddExtension(vabExtension);
            }
        }
    }
}