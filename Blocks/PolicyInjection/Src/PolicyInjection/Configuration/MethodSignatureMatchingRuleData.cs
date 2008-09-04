//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Policy Injection Application Block
//===============================================================================
// Copyright � Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection.MatchingRules;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.PolicyInjection.Configuration
{
    /// <summary>
    /// Configuration element that stores the configuration information for an instance
    /// of <see cref="MethodSignatureMatchingRule"/>.
    /// </summary>
    [Assembler(typeof(MethodSignatureMatchingRuleAssembler))]
    public class MethodSignatureMatchingRuleData : StringBasedMatchingRuleData
    {
        private const string ParametersPropertyName = "parameters";

        /// <summary>
        /// Constructs a new <see cref="MethodSignatureMatchingRuleData"/> instance.
        /// </summary>
        public MethodSignatureMatchingRuleData()
            : base()
        {
        }

        /// <summary>
        /// Constructs a new <see cref="MethodSignatureMatchingRuleData"/> instance.
        /// </summary>
        /// <param name="matchingRuleName">Name of matching rule in config.</param>
        /// <param name="memberName">Method name pattern to match.</param>
        public MethodSignatureMatchingRuleData(string matchingRuleName, string memberName)
            : base(matchingRuleName, memberName, typeof(MethodSignatureMatchingRule))
        {
        }

        /// <summary>
        /// The collection of parameters that make up the matching method signature.
        /// </summary>
        /// <value>The "parameters" child element.</value>
        [ConfigurationProperty(ParametersPropertyName)]
        public ParameterTypeElementDataCollection Parameters
        {
            get { return (ParameterTypeElementDataCollection)base[ParametersPropertyName]; }
            set { base[ParametersPropertyName] = value; }
        }
    }

    /// <summary>
    /// Class used by ObjectBuilder to construct a <see cref="MethodSignatureMatchingRule"/> instance
    /// from a <see cref="MethodSignatureMatchingRuleData"/> instance.
    /// </summary>
    public class MethodSignatureMatchingRuleAssembler : IAssembler<IMatchingRule, MatchingRuleData>
    {
        /// <summary>
        /// Builds an instance of the subtype of IMatchingRule type that the receiver knows how to build, based on 
        /// an a configuration object.
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> that represents the current building process.</param>
        /// <param name="objectConfiguration">The configuration object that describes the object to build.</param>
        /// <param name="configurationSource">The source for configuration objects.</param>
        /// <param name="reflectionCache">The cache to use retrieving reflection information.</param>
        /// <returns>A fully initialized instance of the IMatchingRule subtype.</returns>
        public IMatchingRule Assemble(IBuilderContext context, MatchingRuleData objectConfiguration, IConfigurationSource configurationSource, ConfigurationReflectionCache reflectionCache)
        {
            MethodSignatureMatchingRuleData castedRuleData = (MethodSignatureMatchingRuleData)objectConfiguration;

            List<string> parameterTypes = new List<string>();
            foreach (ParameterTypeElement parameterType in castedRuleData.Parameters)
            {
                parameterTypes.Add(parameterType.ParameterTypeName);
            }

            MethodSignatureMatchingRule matchingRule = new MethodSignatureMatchingRule(castedRuleData.Match, parameterTypes, castedRuleData.IgnoreCase);

            return matchingRule;
        }
    }

    /// <summary>
    /// A configuration element that stores a collection of <see cref="ParameterTypeElement"/> objects.
    /// </summary>
    [ConfigurationCollection(typeof(ParameterTypeElement), AddItemName="parameter")]
    public class ParameterTypeElementDataCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new element to store in the collection.
        /// </summary>
        /// <returns>The new element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ParameterTypeElement();
        }

        /// <summary>
        /// Gets the element key from the element.
        /// </summary>
        /// <param name="element">Element to retrieve key from.</param>
        /// <returns>The key.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            ParameterTypeElement paramTypeAttribute = element as ParameterTypeElement;
            if (paramTypeAttribute != null)
            {
                return paramTypeAttribute.Name;
            }
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Adds a <see cref="ParameterTypeElement"/> to the collection.
        /// </summary>
        /// <param name="parameterTypeElement">The element to add.</param>
        public void Add(ParameterTypeElement parameterTypeElement)
        {
            base.BaseAdd(parameterTypeElement, true);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            base.BaseClear();
        }

        /// <summary>
        /// Gets the element at the given index.
        /// </summary>
        /// <param name="index">Index of desired element.</param>
        /// <returns>The element at that index.</returns>
        public ParameterTypeElement Get(int index)
        {
            return base.BaseGet(index) as ParameterTypeElement;
        }

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="index">Index of element to remove.</param>
        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

    }

    /// <summary>
    /// A configuration element representing a single parameter in a method signature.
    /// </summary>
    public class ParameterTypeElement : ConfigurationElement
    {
        private const string NamePropertyName = "name";
        private const string ParameterTypeNamePropertyName = "typeName";

        /// <summary>
        /// Constructs a new <see cref="ParameterTypeElement"/> instance.
        /// </summary>
        /// <param name="name">unique identifier for this parameter. The name does
        /// NOT need to match the target's parameter name.</param>
        /// <param name="parameterType">Expected type of parameter</param>
        public ParameterTypeElement(string name, string parameterType)
        {
            Name = name;
            ParameterTypeName = parameterType;
        }

        /// <summary>
        /// Constructs a new <see cref="ParameterTypeElement"/> instance.
        /// </summary>
        public ParameterTypeElement()
        {
        }

        /// <summary>
        /// A unique ID for this parameter. This name does not need to match
        /// the corresponding parameter in the target types; only the type is
        /// used.
        /// </summary>
        /// <value>A name for this property that is unique in this rule's configuration.</value>
        [ConfigurationProperty(NamePropertyName, IsKey=true, IsRequired=true)]
        public string Name
        {
            get { return (string)base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }

        /// <summary>
        /// The parameter type required.
        /// </summary>
        /// <value>The "typeName" config attribute.</value>
        [ConfigurationProperty(ParameterTypeNamePropertyName, IsKey=true, IsRequired=true, DefaultValue="")]
        public string ParameterTypeName
        {
            get{return (string) base[ParameterTypeNamePropertyName];}
            set{base[ParameterTypeNamePropertyName] =value;}
        }
    }
}