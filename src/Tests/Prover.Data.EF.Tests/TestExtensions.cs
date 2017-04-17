using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Prover.Shared.Domain;

namespace Prover.Data.EF.Tests
{
    public static class TestExtensions
    {
        public static T PropertiesShouldEqual<T>(this T actual, T expected, params string[] filters)
        {
            var properties = typeof(T).GetProperties().ToList();

            var filterByEntities = new List<string>();
            var values = new Dictionary<string, object>();

            foreach (var propertyInfo in properties.ToList())
            {
                //skip by filter
                if (filters.Any(f => f == propertyInfo.Name || f == propertyInfo.Name))
                    continue;

                var value = propertyInfo.GetValue(actual);
                values.Add(propertyInfo.Name, value);

                if (value == null)
                    continue;

                //skip array and System.Collections.Generic types
                if (value.GetType().IsArray || value.GetType().Namespace == "System.Collections.Generic")
                {
                    properties.Remove(propertyInfo);
                    continue;
                }

                if (!(value is Entity) && !propertyInfo.Name.Contains("Id"))
                    continue;

                //skip BaseEntity types and entities Id
                filterByEntities.Add(propertyInfo.Name);
                properties.Remove(propertyInfo);
               
            }

            foreach (var propertyInfo in properties.Where(p => values.ContainsKey(p.Name)))
            {
                if (filterByEntities.Any(f => f == propertyInfo.Name))
                    continue;

                Assert.AreEqual(values[propertyInfo.Name], propertyInfo.GetValue(expected), string.Format("The property \"{0}.{1}\" of these objects is not equal", typeof(T).Name, propertyInfo.Name));
            }

            return actual;
        }
    }
    
}
