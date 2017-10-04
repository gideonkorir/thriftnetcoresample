using System;
using Thrift;
using Thrift.Protocols;
using Thrift.Server;
using Thrift.Transports;
using Thrift.Transports.Client;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using tutorial;

namespace CSharpTutorial
{
    public class CSharpClient
    {
        public static async Task Main()
        {
            try
            {
                TClientTransport transport = new TSocketClientTransport(IPAddress.Loopback, 9090);
                TProtocol protocol = new TBinaryProtocol(transport);
                Calculator.Client client = new Calculator.Client(protocol);

                await transport.OpenAsync();
                var cancel = CancellationToken.None;
                try
                {
                    await client.pingAsync(cancel);
                    Console.WriteLine("ping()");

                    int sum = await client.addAsync(1, 1, cancel);
                    Console.WriteLine("1+1={0}", sum);

                    Work work = new Work();

                    work.Op = Operation.DIVIDE;
                    work.Num1 = 1;
                    work.Num2 = 0;
                    try
                    {
                        int quotient = await client.calculateAsync(1, work, cancel);
                        Console.WriteLine("Whoa we can divide by 0");
                    }
                    catch (InvalidOperation io)
                    {
                        Console.WriteLine("Invalid operation: " + io.Why);
                    }

                    work.Op = Operation.SUBTRACT;
                    work.Num1 = 15;
                    work.Num2 = 10;
                    try
                    {
                        int diff = await client.calculateAsync(1, work, cancel);
                        Console.WriteLine("15-10={0}", diff);
                    }
                    catch (InvalidOperation io)
                    {
                        Console.WriteLine("Invalid operation: " + io.Why);
                    }

                    shared.SharedStruct log = await client.getStructAsync(1, cancel);
                    Console.WriteLine("Check log: {0}", log.Value);

                }
                finally
                {
                    transport.Close();
                }
            }
            catch (TApplicationException x)
            {
                Console.WriteLine(x.StackTrace);
            }

        }
    }
}
