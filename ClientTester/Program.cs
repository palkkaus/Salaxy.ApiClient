using Salaxy.Tools.ClientTester;

Console.WriteLine("Testing the client...");
// var result = await Tester.QuickAnonTest();
// var result = await Tester.TestImport();

// Token needs to be obtained from the SSO process (this token has already expired).
string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkZJODRQWVlTMDAyNTIxODk2MiIsIm5hbWVpZCI6IkZJODRQWVlTMDAyNTIxODk2MiIsImFjY291bnRfb3duZXJfaWQiOiJGSTg0UFlZUzAwMjUyMTg5NjIiLCJhY2NvdW50X3Jvd19pZCI6IkZJODRQWVlTMDAyNTIxODk2MiIsImNyZWRlbnRpYWxfaWQiOiJhdXRoMHw2NWVlOWU4NzE4YTkyNDRlZDExNmIxMWUiLCJzYWxheHlfcGFydG5lciI6IiIsImVtYWlsIjoic2lnbmljYXQtdGVzdGVyMkBwYWxra2F1cy5maSIsImZpcnN0X25hbWUiOiJTaWduaWNhdCBBUyIsImxhc3RfbmFtZSI6IiIsInBpY3R1cmUiOiJodHRwczovL3MuZ3JhdmF0YXIuY29tL2F2YXRhci9kMGM1YTM3OGEyYWI0Nzg3MzI2MjZlYWQ1NzJhNGM1Yz9zPTQ4MCZyPXBnJmQ9aHR0cHMlM0ElMkYlMkZjZG4uYXV0aDAuY29tJTJGYXZhdGFycyUyRnNpLnBuZyIsImNvbG9yIjoiIiwiaW5pdGlhbHMiOiJTIiwibmJmIjoxNzMxMzEwMTAwLCJleHAiOjE3MzE0ODI5MDAsImlhdCI6MTczMTMxMDEwMCwiaXNzIjoiaHR0cHM6Ly90ZXN0LXNlY3VyZS5zYWxheHkuY29tIiwiYXVkIjoiaHR0cHM6Ly90ZXN0LXNlY3VyZS5zYWxheHkuY29tIn0.IfGng0Xh4usa5XmgepYw7x7bYcfKJgpGBRZEuM0GbyI";
// Identifier for a Payroll **template** that must be created in Palkkaus.fi. The template is used as the bases for the Payroll.
// string payrollTemplateId = "ef48df49-dec5-4886-a845-af59758bbf86";
// Any unique string that defines the period (Payroll in Palkkaus) that you are sending
string uniqueIdForPeriod = "xyz1234";

var sender = new SendDataToImportStaging(token, uniqueIdForPeriod, "090687-943H");
// var result = await sender.CreateUpdatePayroll(payrollTemplateId);
// var result = await sender.AssureEmployment();
var result = await sender.UpdateSomeRows();
Console.WriteLine("Update (1): " + result);

result = await sender.Commit();
Console.WriteLine("Commit (2): " + result);

result = await sender.ChangeSomeRows();
Console.WriteLine("Change (3): " + result);

Console.WriteLine("DONE");
