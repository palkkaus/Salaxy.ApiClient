using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Salaxy.Client
{
    /// <summary>
    /// Generic salaxy helpers: These are not specific to any namespace.
    /// </summary>
    public static class Helpers
    {

        /// <summary>
        /// Converts a date to ISO format.
        /// </summary>
        /// <param name="date">DateTime to convert</param>
        /// <returns>The date as ISO date used by Salaxy.</returns>
        public static string ToIsoDate(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Helper to convert a dynamic object to a dictionary.
        /// Useful for entring worktime import and calculation rows data objects and usecase data objects.
        /// </summary>
        /// <param name="dynObj"></param>
        /// <returns></returns>
        public static Dictionary<String, Object> GetDict(dynamic dynObj)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(dynObj))
            {
                object obj = propertyDescriptor.GetValue(dynObj);
                dictionary.Add(propertyDescriptor.Name, obj);
            }
            return dictionary;
        }
    }
}
