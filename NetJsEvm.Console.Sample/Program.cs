using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EdgeJs;

namespace NetJsEvm.Console.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run((Action)Start);
            new ManualResetEvent(false).WaitOne();
        }

        public static async void Start()
        {
            // Define an event handler to be called for every message from the client

            var onMessage = (Func<object, Task<object>>)(async (message) =>
            {
                System.Console.WriteLine(message.ToString());
                return true;
            });

            
            var executeCodeOnEVM = Edge.Func(@"
            var EthJs = require('./../ethereumjs.js');

            return function(options, cb) {
                
                var code = '7f4e616d65526567000000000000000000000000000000000000000000000000003055307f4e616d6552656700000000000000000000000000000000000000000000000000557f436f6e666967000000000000000000000000000000000000000000000000000073661005d2720d855f1d9976f88bb10c1a3398c77f5573661005d2720d855f1d9976f88bb10c1a3398c77f7f436f6e6669670000000000000000000000000000000000000000000000000000553360455560df806100c56000396000f3007f726567697374657200000000000000000000000000000000000000000000000060003514156053576020355415603257005b335415603e5760003354555b6020353360006000a233602035556020353355005b60007f756e72656769737465720000000000000000000000000000000000000000000060003514156082575033545b1560995733335460006000a2600033545560003355005b60007f6b696c6c00000000000000000000000000000000000000000000000000000000600035141560cb575060455433145b1560d25733ff5b6000355460005260206000f3'
                var vm = new EthJs.VM();
            

                vm.on('step', function (data) {
                    options.onMessage(data.opcode.name);
                });

                code = new Buffer(code, 'hex');


                vm.runCode({
                    code: code,
                    gasLimit: new Buffer('ffffffff', 'hex')
                }, function(err, results) {
                    options.onMessage('returned: ' + results.return.toString('hex'));
                    options.onMessage('gasUsed: ' + results.gasUsed.toString());
                    options.onMessage(err);
                });

                cb();
            };
           ");

            await executeCodeOnEVM(new
            {
                onMessage = onMessage
            });
        }


    }

}
