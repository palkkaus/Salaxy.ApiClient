using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Salaxy.Client;
using Salaxy.Client.Import;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Salaxy.Tools.ClientTester
{
    /// <summary>
    /// Example on how to update data usingg the import API.
    /// </summary>
    public class SendDataToImportStaging
    {
        private readonly string token;
        private readonly string payrollSourceId;
        private readonly string personalId;

        /// <summary>
        /// Creates a new instance of the demo data sender.
        /// </summary>
        /// <param name="token">Token is required, you would typically obtain this using the SSO process.</param>
        /// <param name="payrollSourceId">
        /// A unique ID in the source system for the period that is being procesed.
        /// Payrolls are created / updated based on this ID and rows are directed to payroll based on this ID.
        /// </param>
        /// <param name="personalId">Personal ID for the employee that is used in the example.</param>
        public SendDataToImportStaging(string token, string payrollSourceId, string personalId)
        {
            this.token = token;
            this.payrollSourceId = payrollSourceId;
            this.personalId = personalId;
        }

        /// <summary>
        /// Creates or updates a Payroll in Salaxy. Payroll is used in grouping the salary data for a period and salary date (payment date).
        /// </summary>
        /// <param name="templateId">
        /// Identifier for template that is the bases for the Payroll.
        /// Template defines properties for salary payment such as whether the salary is monthly or hourly and what is the payment channel (method of payment.
        /// </param>
        /// <returns>String "OK" if the operation succeeded.</returns>
        public async Task<string> CreateUpdatePayroll(string templateId)
        {
            var client = ImportHelpers.GetImportClientWithToken(token, false);
            var result = await client.UpdatePayrollAsync(new PayrollDetails()
            {
                Input = new PayrollInput()
                {
                    SalaryDate = new DateTime(2024, 12, 24).ToIsoDate(),
                    Period = new DateTime(2024, 12, 1).GetDateRange(new DateTime(2024, 12, 31)),
                    Title = "Test payroll from .Net",
                    SourceId = payrollSourceId,
                    Template = templateId,
                },
            });
            result.Validations.ThrowIfValidationErrors();
            return "OK";
        }

        /// <summary>
        /// Assures that the employment with the given personal ID exists.
        /// </summary>
        /// <returns>OK if the operation succeeded</returns>
        public async Task<string> AssureEmployment()
        {
            var client = ImportHelpers.GetImportClientWithToken(token, false);
            var data = new WorktimeImport()
            {
                Header = new WorktimeImportHeader()
                {
                    Title = "Työsuhdetietoja",
                },
                Employments = new List<WorktimeImportEmployment> {
                    new WorktimeImportEmployment() {
                        Identity = new EmploymentIdentity() {
                            PersonalId = personalId,
                            FirstName = "Dotnet",
                            LastName = "Testeri",
                        },
                        Info = new EmploymentRelationInfo()
                        {
                            StartDate = "2024-01-01",
                        },
                        Work = new WorkDescription() {
                            OccupationCode = "32141",
                            Description = "Hampaidenhoitaja",
                        },
                    }
                }
            };
            var result = await client.UpdateWorktimeAsync(data);
            result.Employments.Select(x => x.Validation).ToList().ThrowIfValidationErrors();
            return "OK";
        }

        /// <summary>
        /// Updates some rows for a given personal ID - uses the base data from GetSomeRows().
        /// </summary>
        /// <returns>OK if the operation succeeded</returns>
        public async Task<string> UpdateSomeRows()
        {
            var client = ImportHelpers.GetImportClientWithToken(token, false);
            var data = new WorktimeImport()
            {
                Header = new WorktimeImportHeader() {
                    Title = "Palkka-aineisto 2024/11",
                    SourceId = payrollSourceId,
                    SalaryDate = "2024-11-29",
                    Period = new DateTime(2024, 11, 1).GetDateRange(new DateTime(2024, 12, 31)),
                },
                Rows = GetSomeRows(),
            };
            var result = await client.UpdateWorktimeAsync(data);
            result.Rows.Select(x => x.Validation).ToList().ThrowIfValidationErrors();
            return "OK";
        }

        /// <summary>
        /// Commits the data to the payroll system.
        /// </summary>
        /// <returns>OK if the operation succeeded</returns>
        public async Task<string> Commit() {
            var client = ImportHelpers.GetImportClientWithToken(token, false);
            // TODO: Figure out how to make the Body typename meaningful. Also how to be able to send undefined to the API.
            // TODO: Add support for commit by Source ID
            // await client.CommitPayrollAsync(payrollSourceId, new Body());
            await client.CommitPayrollAsync("f0c4f390-3ffe-4fe6-a959-9f9c065659e6", new Body());
            return "OK";
        }

        /// <summary>
        /// Sends the same data as UpdateSomeRows, but changes the number of hours in hourly salary.
        /// </summary>
        /// <returns>OK if the operation succeeded</returns>
        public async Task<string> ChangeSomeRows()
        {
            var client = ImportHelpers.GetImportClientWithToken(token, false);
            var rows = GetSomeRows();
            rows.Single(x => x.RowType == "hourlySalary").Count = 110;
            rows.Single(x => x.RowType == "hourlySalary").SourceId = "changes-110";
            var data = new WorktimeImport()
            {
                Header = new WorktimeImportHeader()
                {
                    Title = "Palkka-aineisto 2024/11",
                    SourceId = payrollSourceId,
                    SalaryDate = "2024-11-29",
                    Period = new DateTime(2024, 11, 1).GetDateRange(new DateTime(2024, 12, 31)),
                },
                Rows = rows,
            };
            var result = await client.UpdateWorktimeAsync(data);
            result.Rows.Select(x => x.Validation).ToList().ThrowIfValidationErrors();
            return "OK";
        }

        /// <summary>
        /// Gets a set of demo rows: One RowType=hourlySalary and one RowType=absencePeriod.
        /// </summary>
        /// <returns>Gets two rows in an array.</returns>
        public WorktimeImportRow[] GetSomeRows() {
            return new[] {
                    new WorktimeImportRow() {
                        RowType = "hourlySalary",
                        PersonalId = personalId,
                        Count = 107,
                        Price = 15,
                        SourceId = "107",
                        Accounting = ImportHelpers.GetAccountingWithDimensions(new[] { ("costCenter", "1001") }),
                        Period = new DateTime(2024, 11, 1).GetDateRange(new DateTime(2024, 11, 30)),
                        Data = Helpers.GetDict(new {
                            isApproved = true,
                            payrollPeriodId = payrollSourceId,
                        }),
                    },
                    new WorktimeImportRow() {
                        RowType = "absencePeriod",
                        PersonalId = personalId,
                        Count = 2,
                        Price = 120,
                        Accounting = ImportHelpers.GetAccountingWithDimensions(new[] { ("costCenter", "1001") }),
                        Period = new DateTime(2024, 11, 4).GetDateRange(new DateTime(2024, 11, 5)),
                        Data = Helpers.GetDict(new {
                            isApproved = true,
                            payrollPeriodId = payrollSourceId,
                            kind = "illness",
                            isPaid = true,
                        }),
                    },
                };
        }

        /// <summary>
        /// Deletes the Payroll (based on the source ID) from Salaxy (as long as the payment process has not started).
        /// Also deletes all the related calculations and data in staging etc.
        /// Use this for clean-up.
        /// </summary>
        /// <returns></returns>
        public async Task<string> DeletePayroll()
        {
            var client = ImportHelpers.GetImportClientWithToken(token, false);
            var result = await client.UpdatePayrollAsync(new PayrollDetails()
            {
                Input = new PayrollInput()
                {
                    SalaryDate = new DateTime(2024, 12, 24).ToIsoDate(),
                    Period = new DateTime(2024, 12, 1).GetDateRange(new DateTime(2024, 12, 31)),
                    Title = "Test payroll from .Net",
                    SourceId = payrollSourceId,
                },
                Usecase = {
                    Data = new Dictionary<string, object>() { { "deletePayroll", true } },
                },
            });
            result.Validations.ThrowIfValidationErrors();
            return "OK";
        }
    }
}
