using Salaxy.Client;

Console.WriteLine("Testing the client...");
var result = await Tester.QuickAnonTest();
Console.WriteLine(result);

Console.WriteLine("DONE");
