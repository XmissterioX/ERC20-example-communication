using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.StandardTokenEIP20;
using Nethereum.StandardTokenEIP20.Functions;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MeaTokenNethereum
{
   public static class ContractsServices
    {
        public enum CoinType
        {
            MainCoin, CoinA, CoinB, CoinC, CoinD
        }

        //MeaToken Address--Must Be Unlocked
        public const string MeaToken_Contract_Address = "0xe1bd216413d0db43eab72a185fb704e3bed9666f";

        //OtherToken Address --Example -- Must be Unlocked
        public const string OtherToken_Contract_Address = "0x2d81ac7316111b0370589bdf561dd01c2ad18cf4";
        //tokenB contract address
        public const string TokenB_Contract_Address = "0x2d81ac7316111b0370589bdf561dd01c2ad18cf4";

        //web3
        private static Web3 web3 = new Web3("https://ropsten.infura.io/6jPJqzYES5Ar9eZVyYWV");
        //meaToken Contract 
        private static StandardTokenService meaToken = new StandardTokenService(web3, MeaToken_Contract_Address);
        //TokenB Contract
        private static StandardTokenService tokenB = new StandardTokenService(web3, TokenB_Contract_Address);
        //accounts
        public const string accountA = "0x0589B85f2F2dBdc629ca3ee956Bd4A90051811d5";
        public const string accountA_privateKey = "e67916692ee7421fa5121842e46e518cf5b71f73d16ec97884132496f80e692d";

        public const string accountB = "0xC6AcAddeEf953F9A65b2F16Acb3cF0082FE2dEc3";
        public const string accountB_privateKey = "2b79203e6c183de9c7363b76afd61a3e5d93ba786c2498c9ed24f41d3bf96af3";

        //MeaToken

        //
        private static TransactionSigner transactionSigner = new TransactionSigner();

        private static  HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://ropsten.infura.io/6jPJqzYES5Ar9eZVyYWV");
    
     
       

        //Unlock account
        public static async Task<bool> UnlockAccount(string senderAddress, string password)
        {          
            return await web3.Personal.UnlockAccount.SendRequestAsync(senderAddress, password, 120);
        }


        //create the contract instance
        public static StandardTokenService CreateContract(string contractAdress)
        {
            return new StandardTokenService(web3, contractAdress);
        }



        //return a list of the transactionHash of both contracts
        public static async Task<List<string>> buyOtherToken(StandardTokenService MeaToken, StandardTokenService OtherToken, string _addressFrom, string _addressTo, float amout)
        {
            var transactionHash_from_MeaToken = await MeaToken.TransferAsync(_addressFrom, _addressTo, amout, new HexBigInteger(3000000));
            var transactionHash_from_other_token = await OtherToken.TransferAsync(_addressTo, _addressFrom, amout, new HexBigInteger(3000000));

            List<string> Result = new List<string>() ;
            Result.Add(transactionHash_from_MeaToken);
            Result.Add(transactionHash_from_other_token);
            return Result;

        }


        public static async Task<string> Meacoin_Offline_Transaction(string privateKey,string _addressFrom, string _addressTo,int amount)
        {  
            //get the function
            var function = meaToken.Contract.GetFunction("transfer");
            //get the data
            var data = function.GetData(new object[] { _addressTo, new BigInteger(amount) });
            //nonce
            var txCount = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(_addressFrom, BlockParameter.CreatePending());
            //signing the transaction
            var encoded = transactionSigner.SignTransaction(privateKey, meaToken.Contract.Address, 0, txCount.Value, 1000000000000L, 900000, data);
            //transaction hash
            var txId = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync("0x" + encoded);
            //verify transaction
            var status = transactionSigner.VerifyTransaction(encoded);
            Console.WriteLine("Status : " + status);
            //returning transaction hash
            return txId;
        }

        public static async Task<bool>  Test()
        {

            var web3 = new Web3(new Account("2b79203e6c183de9c7363b76afd61a3e5d93ba786c2498c9ed24f41d3bf96af3"), "https://ropsten.infura.io/6jPJqzYES5Ar9eZVyYWV");

            var transactionMessage = new TransferFunction
            {
                FromAddress = "0xC6AcAddeEf953F9A65b2F16Acb3cF0082FE2dEc3",
                To = "0xEae6CB8937919baA39F4B5626037147919D6bb75",
                TokenAmount = 500
            };

            var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();

            var estimatedGas = await transferHandler.EstimateGasAsync(transactionMessage, "0xe1bd216413d0db43eab72a185fb704e3bed9666f");
           
            // for demo purpouses gas estimation it is done in the background so we don't set it
            transactionMessage.Gas = estimatedGas.Value;

            var transferReceipt =
                await transferHandler.SendRequestAndWaitForReceiptAsync(transactionMessage, "0xe1bd216413d0db43eab72a185fb704e3bed9666f");
            return true;
        }
        

        public static async Task<int> get_MeaToken_Account_Balance(string Account_Address)
        {
            var e = await meaToken.GetBalanceOfAsync<int>(Account_Address);
            return e;
        }

        public static async Task<BigInteger> balanceOf(Web3 web3 , string address)
        {
            var balance = await web3.Eth.GetBalance.SendRequestAsync(address);
            return balance.Value;

        }

        public static  bool isMined(string transactionHash)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://ropsten.infura.io/6jPJqzYES5Ar9eZVyYWV%22");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.AllowWriteStreamBuffering = true;




            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getTransactionByHash\",\"params\":["+"\""+transactionHash+"\"],\"id\":3}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse =  (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Console.WriteLine("result : " + result);
                dynamic dynamicRes = JsonConvert.DeserializeObject(result);

                string _blocknumer = dynamicRes.result.blockNumber;
               
                if (_blocknumer == null)
                 {
                     Console.WriteLine("The blockNumber is null -- Pending");

                    return false;
                 }
                 
                Console.WriteLine("The block : " + _blocknumer + " has been performed successfully"); 
                return true;
            }
     
        }


        ///////////////////////////////////////////////////////////////////////////////////////


        public static async Task<string> SendToken(string PrivateKeyFrom, string PublicKeyTo, float value)
        {
            
            //get the function
            var function = meaToken.Contract.GetFunction("transfer");
            //get the data
            var data = function.GetData(new object[] { PublicKeyTo, new BigInteger(value) });
           // var estimatedGaz = await function.EstimateGasAsync(new object[] { PublicKeyTo, new BigInteger(value) });

            //Console.WriteLine("Estimated Gaz : " + estimatedGaz);

            //nonce
            var txCount = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(EthECKey.GetPublicAddress(PrivateKeyFrom), BlockParameter.CreatePending());
            //signing the transaction
            var encoded = transactionSigner.SignTransaction(PrivateKeyFrom, meaToken.Contract.Address, 1, txCount.Value, 1000000000000L, 900000, data);

            //estimate gaz
            var GasEstimated = await web3.Eth.TransactionManager.EstimateGasAsync(new CallInput("0x" + encoded, PublicKeyTo));
            Console.WriteLine("Estimated Gas is : " + GasEstimated.Value.ToString());

            Console.WriteLine("My balance : " + web3.Eth.GetBalance.SendRequestAsync(EthECKey.GetPublicAddress(PrivateKeyFrom)).Result.Value.ToString());


            //transaction hash
        
           var bal = await  get_MeaToken_Account_Balance(EthECKey.GetPublicAddress(PrivateKeyFrom));

            Console.WriteLine("bal : " + bal);
      

            var txId = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync("0x" + encoded);
            //verify transaction
            var status = transactionSigner.VerifyTransaction(encoded);
            Console.WriteLine("Status : " + status);
            //returning transaction hash

            isMined(txId);
            return txId;

        }

        

        public static async Task<List<string>> Exchangetoken(string PrivateKey, CoinType TokenTo, float Value)
        {
            //get the function from  meaktoken
            var function_meakToken = meaToken.Contract.GetFunction("transfer");
            var dataM = function_meakToken.GetData(new object[] { accountA, new BigInteger(Value)});

         //  if (TokenTo.Equals(CoinType.CoinB))
          //  { //get fthe function from tokenB
                var function_tokenB = tokenB.Contract.GetFunction("transfer");
                var dataB = function_tokenB.GetData(new object[] { EthECKey.GetPublicAddress(PrivateKey), new BigInteger(Value) });

         //   }

            var txCountM = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(EthECKey.GetPublicAddress(PrivateKey), BlockParameter.CreatePending());
            var txCountB = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(accountB, BlockParameter.CreatePending());

            var encodedM = transactionSigner.SignTransaction(PrivateKey, meaToken.Contract.Address, 0, txCountM.Value, 1000000000000L, 900000, dataM);
            var encodedB = transactionSigner.SignTransaction(accountB_privateKey, TokenB_Contract_Address, 0, txCountB.Value, 1000000000000L, 900000, dataB);

            var txIdM = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync("0x" + encodedM);
            var txIdB = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync("0x" + encodedB);

            var statusM = transactionSigner.VerifyTransaction(encodedM);
            var statusB = transactionSigner.VerifyTransaction(encodedB);

            
            List<string> res = new List<string>();
            res.Add(txIdM);
            res.Add(txIdB);
            return res;


        }


        /*public static async Task<string> sendEth()
        {
            var txCount = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync("0x0589B85f2F2dBdc629ca3ee956Bd4A90051811d5");
            var encoded = transactionSigner.SignTransaction("e67916692ee7421fa5121842e46e518cf5b71f73d16ec97884132496f80e692d", "0xC6AcAddeEf953F9A65b2F16Acb3cF0082FE2dEc3", 1000000000000000000L, txCount.Value);
            var txId = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync("0x" + encoded);
            return txId.ToString();
        }*/


    }
}
