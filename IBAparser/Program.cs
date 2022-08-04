using IBAparser;

var newDBCreator = new DatabaseCreator();
Directory.CreateDirectory("./RecipeImages");
await newDBCreator.CreateDBAsync();
Console.WriteLine("Press 'Enter' to exit");
Console.ReadLine();