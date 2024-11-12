using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Salaxy.Client.Import
{
    /// <summary>
    /// Helpers for the import API. These function in the Salaxy.Client.Import namespace.
    /// </summary>
    public static class ImportHelpers
    {
        /// <summary>
        /// Gets a Salaxy improt date range from two .Net DateTimes.
        /// </summary>
        /// <param name="start">Start date</param>
        /// <param name="end">
        /// End date. NOTE: If left undefined, the end date is the same as start (typical in import dates).
        /// If you want an open ended date range, use DateRange constructor instead.
        /// </param>
        /// <returns>ISO standard based date range object that Salaxy uses.</returns>
        public static DateRange GetDateRange(this DateTime start, DateTime? end)
        {
            return new DateRange()
            {
                Start = start.ToIsoDate(),
                End = end?.ToIsoDate() ?? start.ToIsoDate(),
            };
        }

        /// <summary>
        /// Gets a Salaxy import / staging client with a given token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="isProduction"></param>
        /// <returns></returns>
        public static Import.Client GetImportClientWithToken(string token, bool isProduction)
        {
            string baseAddress = isProduction ? "https://import-staging.salaxy.com/" : "http://localhost:3000/"; // "https://test-import-staging.salaxy.com/";
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return new Salaxy.Client.Import.Client(httpClient);
        }

        /// <summary>
        /// Throws an exception if there are validation errors.
        /// </summary>
        /// <param name="validations">Colelction of validatation objects that may or may not have errors.</param>
        /// <param name="message">Optional error message.</param>
        /// <exception cref="Exception"></exception>
        public static void ThrowIfValidationErrors(this ICollection<ApiListItemValidation> validations, string? message = null)
        {
            if (validations.Any(x => x.IsValid != true))
            {
                foreach (var validation in validations.Where(x => x.IsValid != true))
                {
                    Console.Error.WriteLine($"- {validation.Errors.First().Msg}");
                }
                throw new Exception(message ?? "Validation errors, see console for details.");
            }
        }

        /// <summary>
        /// Throws an exception if there are validation errors.
        /// </summary>
        /// <param name="validations">Colelction of validatation objects that may or may not have errors.</param>
        /// <param name="message">Optional error message.</param>
        /// <exception cref="Exception"></exception>
        public static void ThrowIfValidationErrors(this List<ApiValidation> validations, string? message = null)
        {
            if (validations.Any(x => x.IsValid != true))
            {
                foreach (var validation in validations.Where(x => x.IsValid != true))
                {
                    Console.Error.WriteLine($"- {validation.Errors.First().Msg}");
                }
                throw new Exception(message ?? "Validation errors, see console for details.");
            }
        }

        /// <summary>
        /// Helper for creating the RowAccounting object with dimensions values (which is the most common usecsase):
        /// </summary>
        /// <param name="dimensions">
        /// Provide the dimensions as a tuple array. Example: '("costCenter", "1234"), ( "project", "FOO" )'
        /// </param>
        /// <example>
        /// new WorktimeImportRow() {
        ///   // Other rows...
        ///   Accounting = ImportHelpers.GetAccountingWithDimensions(new[] { ("costCenter", "1234"), ( "project", "FOO" ) }),
        /// }
        /// </example>
        /// <returns></returns>
        public static RowAccounting GetAccountingWithDimensions((string Id, string Value)[] dimensions)
        {
            return new RowAccounting()
            {
                Dimensions = dimensions.Select(x => new CostAccountingDimension()
                {
                    Id = x.Id,
                    Value = x.Value,
                }).ToList(),
            };
        }
    }
}
