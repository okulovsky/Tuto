using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace Books.ListMyLibrary
{
    /// <summary>
    /// Sample which demonstrates how to use the Books API.
    /// https://code.google.com/apis/books/docs/v1/getting_started.html
    /// <summary>
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Books API Sample: List MyLibrary");
            Console.WriteLine("================================");
            try
            {
                new Program().Run().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private async Task Run()
        {
            UserCredential credential;
           
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                      new ClientSecrets
                      {
                          ClientId = "230938529802-a0if8hku9tptjltqihfpj3bv041lstcq.apps.googleusercontent.com",
                          ClientSecret = "lycpjRfHvG8K0mlbAewruYES"
                      },
                      new[] { BooksService.Scope.Books },
                      "user",
                      CancellationToken.None,
                      new FileDataStore("Books.ListMyLibrary"));


            // Create the service.
            var service = new BooksService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Books API Sample",
                });

            var bookshelves = await service.Mylibrary.Bookshelves.List().ExecuteAsync();

        }
    }
}