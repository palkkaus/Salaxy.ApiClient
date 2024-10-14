using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salaxy.Client
{
    /// <summary>
    /// Converts an enum to a string, but if the conversion fails, returns the default value of the enum instaead failing the deserialization process.
    /// </summary>
    public class SafeEnumConverter : StringEnumConverter
    {
        /// <summary>
        /// First tries the standard enum conversion, and if it fails, returns the default value of the enum (taking into account that it may be nullable).
        /// </summary>
        /// <param name="reader">The Newtonsoft.Json.JsonReader to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            // Alternative implementation here: https://filx.medium.com/how-to-fix-nswag-api-client-unknown-enum-value-error-a7faa0320012
            try
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch (Exception)
            {
                if (Nullable.GetUnderlyingType(objectType) != null)
                {
                    objectType = Nullable.GetUnderlyingType(objectType)!;
                }
                var result = Activator.CreateInstance(objectType);
                return result;
            }
        }
    }
}
