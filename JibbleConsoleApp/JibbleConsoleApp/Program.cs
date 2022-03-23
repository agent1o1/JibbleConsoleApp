using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trippin;

namespace JibbleConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            string? GetUserInput()
            {
                Console.WriteLine("Please input a command");
                return Console.ReadLine();
            }

            void PrintPersonsUserName(IEnumerable<Person> query)
            {
                string? s;
                foreach (var person in query)
                {
                    Console.WriteLine($"{person.UserName}");
                }
            }

            var container = new Container(new Uri("https://services.odata.org/TripPinRESTierService/"));
            var people = container.People;

            var command = GetUserInput();
            while (command != "exit")
            {
                switch (command)
                {
                    case "list":
                        {
                            var query = (await people
                                .AddQueryOption("$select", $"{nameof(Person.UserName)}")
                                .ExecuteAsync())
                                .ToList();

                            PrintPersonsUserName(query);
                            command = GetUserInput();
                        }
                        break;
                    case "filter":
                        {
                            Console.WriteLine("Input search string");
                            var searchStr = Console.ReadLine();
                            var query = (await people
                                .AddQueryOption("$filter",
                                    $"contains({nameof(Person.UserName)}, '{searchStr}')")
                                .AddQueryOption("$select", $"{nameof(Person.UserName)}")
                                .ExecuteAsync())
                                .ToList();

                            PrintPersonsUserName(query);
                            command = GetUserInput();
                        }
                        break;
                    case "detail":
                        {
                            Console.WriteLine("Please input Person username");
                            var input = Console.ReadLine();

                            var query = (await people
                                .AddQueryOption("$filter", $"{nameof(Person.UserName)} eq '{input}'")
                                .ExecuteAsync())
                                .ToList();

                            if (!query.Any())
                                Console.WriteLine("Person with provided username was not found");
                            else
                                foreach (var person in query)
                                {
                                    var sb = new StringBuilder();
                                    sb.AppendLine($"UserName: {person.UserName} \n");
                                    sb.AppendLine($"FirstName: {person.FirstName} \n");
                                    sb.AppendLine($"LastName: {person.LastName} \n");
                                    sb.AppendLine($"Gender: {person.Gender} \n");
                                    sb.AppendLine($"Age: {person.Age} \n");
                                    sb.AppendLine($"AddressInfo: {string.Join('\n', person.AddressInfo.Select(x => $"City: {x.City?.Name ?? "unknown"} Address: {x.Address} \n"))}");
                                    sb.AppendLine($"Emails: {string.Join(", ", person.Emails)} \n");
                                    sb.AppendLine($"Features: {string.Join(", ", string.Join(',', person.Features.Select(x => x.ToString())))} \n");
                                    Console.WriteLine(sb.ToString());
                                }

                            command = GetUserInput();
                        }
                        break;

                    case "help":
                        Console.WriteLine("Available commands: list, filter, detail, help");
                        command = GetUserInput();
                        break;
                    default:
                        Console.WriteLine("unknown command");
                        command = GetUserInput();
                        break;
                }
            }
        }
    }
}
