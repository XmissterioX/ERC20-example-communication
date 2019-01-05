using Nethereum.Hex.HexTypes;
using Nethereum.Signer;
using Nethereum.StandardTokenEIP20;
using Nethereum.StandardTokenEIP20.Functions;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MeaTokenNethereum

{
    class Program
        {


        static  void Main(string[] args)
        {   //instantiate    

            //ContractsServices service = new ContractsServices();

            //  service.ahla().Wait();   
            /*
                        //test unlock
                        Console.WriteLine("Unlocking account...");
                        var result1 =  service.UnlockAccount(web3, "0x0270a69465e4ad9fe6307c24945068839781f083", "accountA");
                        Console.WriteLine(result1.Result);
                        //test jsonRpc
                        Console.WriteLine("Checking transaction status...");
                        var result2 =  service.check_transaction_status(httpWebRequest, "0x384669ab3025e5561443e21413f457b7c6bf52308d7ad89d365f7449e9e9e13e");
                        Console.WriteLine(result2.Result);*/

            /*   var balance =  service.get_MeaToken_Account_Balance("0x0589B85f2F2dBdc629ca3ee956Bd4A90051811d5");
               var hash = service.Meacoin_Offline_Transaction("e67916692ee7421fa5121842e46e518cf5b71f73d16ec97884132496f80e692d", "0x0589B85f2F2dBdc629ca3ee956Bd4A90051811d5", "0xC6AcAddeEf953F9A65b2F16Acb3cF0082FE2dEc3",50);
              var etherAmount = Web3.Convert.FromWei(balance.Result);

              var status = service.check_transaction_status(hash.Result);

             Console.WriteLine("hash : " + hash.Result);
             Console.WriteLine("status " + status.Result);

              Console.WriteLine("balance : " + balance.Result);
              */

            //var x =  ContractsServices.get_MeaToken_Account_Balance("0xC6AcAddeEf953F9A65b2F16Acb3cF0082FE2dEc3").Result;
            // Console.WriteLine(x);

             string hash = ContractsServices.SendToken("e67916692ee7421fa5121842e46e518cf5b71f73d16ec97884132496f80e692d", "0xC6AcAddeEf953F9A65b2F16Acb3cF0082FE2dEc3", 50).Result;
              Console.WriteLine("Hash : " + hash);

            /* List<string> x = ContractsServices.Exchangetoken("72ef3762da381edb8a26f17da11593be15d9b8c71ffc632f09838c32c9537d1d", ContractsServices.CoinType.CoinB, 10).Result;

             foreach(string t in x)
             {
                 Console.WriteLine(t);
             }
             */


         /*   var t = ContractsServices.sendEth();
            Console.WriteLine(t);*/

            Console.WriteLine("finished!");
            Console.ReadKey();
        }
    }

    }

