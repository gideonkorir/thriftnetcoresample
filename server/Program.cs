using System;
using tutorial;
using shared;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Thrift;
using Thrift.Transports;
using Thrift.Transports.Server;
using Thrift.Server;
using Thrift.Protocols;

namespace Server
{
    public class CalculatorHandler : Calculator.IAsync
    {
        Dictionary<int, SharedStruct> log;

        public CalculatorHandler()
        {
            log = new Dictionary<int, SharedStruct>();
        }

        public Task pingAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("ping()");
            return Task.CompletedTask;
        }

        public Task<int> addAsync(int n1, int n2, CancellationToken cancellationToken)
        {
            Console.WriteLine("add({0},{1})", n1, n2);
            return Task.FromResult(n1 + n2);
        }

        public Task<int> calculateAsync(int logid, Work work, CancellationToken cancellationToken)
        {
            Console.WriteLine("calculate({0}, [{1},{2},{3}])", logid, work.Op, work.Num1, work.Num2);
            int val = 0;
            switch (work.Op)
            {
                case Operation.ADD:
                    val = work.Num1 + work.Num2;
                    break;

                case Operation.SUBTRACT:
                    val = work.Num1 - work.Num2;
                    break;

                case Operation.MULTIPLY:
                    val = work.Num1 * work.Num2;
                    break;

                case Operation.DIVIDE:
                    if (work.Num2 == 0)
                    {
                        InvalidOperation io = new InvalidOperation();
                        io.WhatOp = (int)work.Op;
                        io.Why = "Cannot divide by 0";
                        throw io;
                    }
                    val = work.Num1 / work.Num2;
                    break;

                default:
                    {
                        InvalidOperation io = new InvalidOperation();
                        io.WhatOp = (int)work.Op;
                        io.Why = "Unknown operation";
                        throw io;
                    }
            }

            SharedStruct entry = new SharedStruct();
            entry.Key = logid;
            entry.Value = val.ToString();
            log[logid] = entry;

            return Task.FromResult(val);
        }

        public Task<SharedStruct> getStructAsync(int key, CancellationToken cancellationToken)
        {
            Console.WriteLine("getStruct({0})", key);
            return Task.FromResult(log[key]);
        }

        public Task zipAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("zip()");
            return Task.CompletedTask;
        }
    }

    public class CSharpServer
    {
        public static async Task Main()
        {
            var logger = new LoggerFactory();
            logger.AddConsole();
            var cts = new CancellationTokenSource();

            try
            {
                CalculatorHandler handler = new CalculatorHandler();
                Calculator.AsyncProcessor processor = new Calculator.AsyncProcessor(handler);
                TServerTransport serverTransport = new TServerSocketTransport(9090);
                var protocol = new TBinaryProtocol.Factory();

                TBaseServer server = new Thrift.Server.AsyncBaseServer(
                    processor, 
                    serverTransport, 
                    protocol,
                    protocol,
                    logger
                    );

                // Use this for a multithreaded server
                // server = new TThreadPoolServer(processor, serverTransport);

                Console.WriteLine("Starting the server...");
                await server.ServeAsync(cts.Token);
            }
            catch (Exception x)
            {
                Console.WriteLine(x.StackTrace);
            }
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            cts.Cancel();
            Console.WriteLine("Done.");

        }
    }
}