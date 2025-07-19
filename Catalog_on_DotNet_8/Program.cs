using Catalog_on_DotNet;

using System.Text;


Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Console.OutputEncoding = Encoding.GetEncoding("windows-1251");
Console.InputEncoding = Encoding.GetEncoding("windows-1251");

Storage storage = new StorageFromFile();
//Storage storage = new StorageFromDB();
Catalog catalog = new Catalog(storage);
ConsoleUI consoleUI = new ConsoleUI(catalog);

consoleUI.RunMainMenu();
