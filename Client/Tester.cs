using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using Salaxy.Client.Api;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Salaxy.Client
{
    /// <summary>
    /// Quick smoke test methods for the Salaxy API.
    /// These can be called to get quickly started with some basic tasks assuring
    /// </summary>
    public class Tester
    {
        /// <summary>
        /// Does a quick anonymous test connecting to the demo server.
        /// </summary>
        /// <returns>A text with simple calculation calculted on the server.</returns>
        public static async Task<string> QuickAnonTest()
        {
            var result = await TestAnonCalculate(20, 30, CalculationRowType.HourlySalary);
            return $"Salaxy calculation 20h * 30eur/h = {result.Result.Totals.TotalGrossSalary}eur results to net salary of {result.Result.WorkerCalc.TotalWorkerPayment}eur after side costs (0 tax).";
        }

        /// <summary>
        /// Does a small anonymous calculation in the Demo environment.
        /// </summary>
        /// <param name="count">Count, e.g. number of hours.</param>
        /// <param name="price">Price, e.g. salary per hour.</param>
        /// <param name="type">
        /// Change the the row type for different types of calculations.
        /// Default is basic salary, meaning provision based salary (no hours / months defined).
        /// </param>
        /// <returns>The salary as calculated on server anonymously. See the result for different values to show.</returns>
        public static async Task<Calculation> TestAnonCalculate(double count, double price, CalculationRowType type = CalculationRowType.Salary)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://demo-secure.salaxy.com/v03/api/calculator/new");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var calc  = new Calculation()
            {
                Rows = new List<UserDefinedRow>()
                {
                    new UserDefinedRow()
                    {
                        Count = count,
                        Price = price,
                        RowType = type,
                    }
                }
            };
            return await new Salaxy.Client.Api.CalculatorClient(httpClient).RecalculateAsync(calc);
        }

        /// <summary>
        /// Tests the connection to the Import API (anonymous).
        /// </summary>
        /// <returns>The client cofiguration.</returns>
        public static async Task<Import.Response> TestImport()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://test-import-staging.salaxy.com/");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var client = new Salaxy.Client.Import.Client(httpClient);
            var versions = await client.GetVersionAsync();
            return versions;
        }

    }
}
