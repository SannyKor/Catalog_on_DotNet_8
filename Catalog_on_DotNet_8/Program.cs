using Catalog_on_DotNet;

using System.Text;


Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Console.OutputEncoding = Encoding.GetEncoding("windows-1251");
Console.InputEncoding = Encoding.GetEncoding("windows-1251");

//Storage storage = new StorageFromFile();
CatalogDbContext dbContext = new CatalogDbContext();
Storage storage = new StorageFromDbEf(dbContext);
UserService userService = new UserService(dbContext);
CategoryService categoryService = new CategoryService(dbContext);
//Storage storage = new StorageFromDB();
Catalog catalog = new Catalog(storage);
ConsoleUI consoleUI = new ConsoleUI(catalog, userService, categoryService);

consoleUI.RunMainMenu();
