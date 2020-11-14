// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests.reflection
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using praxicloud.core.reflection;
    using System.Collections.Generic;
    using System;
    using System.Diagnostics.CodeAnalysis;
    #endregion

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ReflectionTests
    {
        #region Shredding
        /// <summary>
        /// Tests shredding capabilities
        /// </summary>
        [TestMethod]
        public void Shredding()
        {
            var getPropertyList = new Dictionary<string, bool>();
            var setPropertyList = new Dictionary<string, bool>();

            var shredder = new TypeShredder(typeof(Person));

            foreach(var item in shredder.ExpressionList)
            {
                getPropertyList.Add(item.PropertyName, item.GetExpression != null);
                setPropertyList.Add(item.PropertyName, item.SetExpression != null);
            }

            Assert.IsTrue(string.Equals(shredder.Name, "praxicloud.core.tests.reflection.ReflectionTests+Person", System.StringComparison.Ordinal), "The name of the type is unexpected");
            Assert.IsTrue(shredder.TypeRepresented == typeof(Person), "The type is unexpected");

            Assert.IsTrue(getPropertyList["FirstName"], "First name get property not expected");
            Assert.IsFalse(setPropertyList["FirstName"], "First name set property not expected");
            Assert.IsTrue(getPropertyList["LastName"], "Last name get property not expected");
            Assert.IsFalse(setPropertyList["LastName"], "Last name set property not expected");
            Assert.IsTrue(getPropertyList["Age"], "Age get property not expected");
            Assert.IsTrue(setPropertyList["Age"], "Age set property not expected");
            Assert.IsTrue(getPropertyList["BillingAddress"], "Billing address get property not expected");
            Assert.IsTrue(setPropertyList["BillingAddress"], "Billing address set property not expected");
        }

        /// <summary>
        /// Populates an instance using the shredder
        /// </summary>
        [TestMethod]
        public void PopulateAndRead()
        {
            var personShredder = new TypeShredder(typeof(Person));
            var addressShredder = new TypeShredder(typeof(Address));

            var address = new Address();
            var person = new Person("Joe", "Smith", 23);

            var firstNameExpression = personShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "FirstName", StringComparison.Ordinal)).First();
            var lastNameExpression = personShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "LastName", StringComparison.Ordinal)).First();
            var ageExpression = personShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "Age", StringComparison.Ordinal)).First();
            var billingAddressExpression = personShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "BillingAddress", StringComparison.Ordinal)).First();
            var streetExpression = addressShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "Street", StringComparison.Ordinal)).First();
            var cityExpression = addressShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "City", StringComparison.Ordinal)).First();
            var provinceExpression = addressShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "Province", StringComparison.Ordinal)).First();
            var countryExpression = addressShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "Country", StringComparison.Ordinal)).First();

            ageExpression.SetValue(person, 32);
            billingAddressExpression.SetValue(person, address);
            streetExpression.SetValue(address, "Street2");
            cityExpression.SetValue(address, "City2");
            provinceExpression.SetValue(address, "Province2");
            countryExpression.SetValue(address, "Country2");

            Assert.IsTrue(personShredder.TypeRepresented == typeof(Person), "Person type shredder should represent a person");
            Assert.IsTrue(addressShredder.TypeRepresented == typeof(Address), "Address type shredder should represent a address");

            var firstName = firstNameExpression.GetValue(person);
            Assert.IsTrue(string.Equals((string)firstName, "Joe", StringComparison.Ordinal), "First name is not expected");

            var lastName = lastNameExpression.GetValue(person);
            Assert.IsTrue(string.Equals((string)lastName, "Smith", StringComparison.Ordinal), "Last name is not expected");

            var age = ageExpression.GetValue(person);
            Assert.IsTrue((int)age == 32, "Age is not expected");

            var billingAddress = billingAddressExpression.GetValue(person);
            Assert.IsNotNull(billingAddress != null, "Billing address should not be null");
            Assert.IsTrue(billingAddressExpression.PropertyType == typeof(Address), "Billing address not of expected type");


            var street = streetExpression.GetValue((Address)billingAddress);
            Assert.IsTrue(string.Equals((string)street, "Street2", StringComparison.Ordinal), "Street is not expected");

            var city = cityExpression.GetValue((Address)billingAddress);
            Assert.IsTrue(string.Equals((string)city, "City2", StringComparison.Ordinal), "City is not expected");

            var province = provinceExpression.GetValue((Address)billingAddress);
            Assert.IsTrue(string.Equals((string)province, "Province2", StringComparison.Ordinal), "Province is not expected");

            var country = countryExpression.GetValue((Address)billingAddress);
            Assert.IsTrue(string.Equals((string)country, "Country2", StringComparison.Ordinal), "Country is not expected");

        }

        /// <summary>
        /// Populates an instance using the shredder without exposing the age
        /// </summary>
        [TestMethod]
        public void PopulateAndReadWithoutAge()
        {
            var personShredder = new TypeShredder(typeof(Person), new List<string> { "Age" });
            var addressShredder = new TypeShredder(typeof(Address));

            var address = new Address();
            var person = new Person("Joe", "Smith", 23);

            var firstNameExpression = personShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "FirstName", StringComparison.Ordinal)).First();
            var lastNameExpression = personShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "LastName", StringComparison.Ordinal)).First();
            var ageExpression = personShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "Age", StringComparison.Ordinal)).FirstOrDefault();
            var billingAddressExpression = personShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "BillingAddress", StringComparison.Ordinal)).First();
            var streetExpression = addressShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "Street", StringComparison.Ordinal)).First();
            var cityExpression = addressShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "City", StringComparison.Ordinal)).First();
            var provinceExpression = addressShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "Province", StringComparison.Ordinal)).First();
            var countryExpression = addressShredder.ExpressionList.Where(item => string.Equals(item.PropertyName, "Country", StringComparison.Ordinal)).First();

            billingAddressExpression.SetValue(person, address);
            streetExpression.SetValue(address, "Street2");
            cityExpression.SetValue(address, "City2");
            provinceExpression.SetValue(address, "Province2");
            countryExpression.SetValue(address, "Country2");

            Assert.IsTrue(string.Equals(person.FirstName, "Joe", StringComparison.Ordinal), "First name is not expected");
            Assert.IsTrue(string.Equals(person.LastName, "Smith", StringComparison.Ordinal), "Last name is not expected");
            Assert.IsNull(ageExpression, "Age was to be excluded");
            Assert.IsNotNull(person.BillingAddress != null, "Billing address should not be null");
            Assert.IsTrue(string.Equals(person.BillingAddress.Street, "Street2", StringComparison.Ordinal), "Street is not expected");
            Assert.IsTrue(string.Equals(person.BillingAddress.City, "City2", StringComparison.Ordinal), "City is not expected");
            Assert.IsTrue(string.Equals(person.BillingAddress.Province, "Province2", StringComparison.Ordinal), "Province is not expected");
            Assert.IsTrue(string.Equals(person.BillingAddress.Country, "Country2", StringComparison.Ordinal), "Country is not expected");

        }
        #endregion
        #region Classes
        /// <summary>
        /// A person
        /// </summary>
        [ExcludeFromCodeCoverage]
        public class Person
        {
            #region Constructors
            /// <summary>
            /// Initializes a new instance of the type
            /// </summary>
            /// <param name="firstName">The first name</param>
            /// <param name="lastName">The last name</param>
            /// <param name="age">The age in years</param>
            public Person(string firstName, string lastName, int age)
            {
                FirstName = firstName;
                LastName = lastName;
                Age = age;
            }
            #endregion
            #region Variables
            /// <summary>
            /// holds the write only property
            /// </summary>
            private string _value;
            #endregion
            #region Properties
            /// <summary>
            /// The first name
            /// </summary>
            public string FirstName { get; }

            /// <summary>
            /// The last name
            /// </summary>
            public string LastName { get; }

            /// <summary>
            /// The age in years
            /// </summary>
            public int Age { get; set; }

            /// <summary>
            /// A value to only be writeable
            /// </summary>
            public string NotWritable { get; }

            /// <summary>
            /// A value to only be readable
            /// </summary>
            public string NotReadable 
            { 
                set
                {
                    _value = value;
                }
            }

            /// <summary>
            /// The billing address
            /// </summary>
            public Address BillingAddress { get; set; }
            #endregion
        }

        /// <summary>
        /// Address container
        /// </summary>
        [ExcludeFromCodeCoverage]
        public class Address
        {
            #region Properties
            /// <summary>
            /// The street address
            /// </summary>
            public string Street { get; set; }

            /// <summary>
            /// The city
            /// </summary>
            public string City { get; set; }

            /// <summary>
            /// The province
            /// </summary>
            public string Province { get; set; }

            /// <summary>
            /// The country
            /// </summary>
            public string Country { get; set; }
            #endregion
        }
        #endregion
    }
}
