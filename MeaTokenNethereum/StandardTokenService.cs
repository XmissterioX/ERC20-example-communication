using Nethereum.Contracts;
using Nethereum.Contracts.CQS;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.StandardTokenEIP20.Functions;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeaTokenNethereum
{
    public class StandardTokenService
    {
        protected Web3 Web3 { get; set; }

        private string abi =
            @"
[
	{
		""anonymous"": false,
		""inputs"": [
			{
				""indexed"": true,
				""name"": ""_owner"",
				""type"": ""address""
			},
			{
				""indexed"": true,
				""name"": ""_spender"",
				""type"": ""address""
			},
			{
				""indexed"": false,
				""name"": ""_value"",
				""type"": ""uint256""
			}
		],
		""name"": ""Approval"",
		""type"": ""event""
	},
	{
		""anonymous"": false,
		""inputs"": [
			{
				""indexed"": true,
				""name"": ""_from"",
				""type"": ""address""
			},
			{
				""indexed"": true,
				""name"": ""_to"",
				""type"": ""address""
			},
			{
				""indexed"": false,
				""name"": ""_value"",
				""type"": ""uint256""
			}
		],
		""name"": ""Transfer"",
		""type"": ""event""
	},
	{
		""constant"": false,
		""inputs"": [
			{
				""name"": ""_spender"",
				""type"": ""address""
			},
			{
				""name"": ""_value"",
				""type"": ""uint256""
			}
		],
		""name"": ""approve"",
		""outputs"": [
			{
				""name"": ""success"",
				""type"": ""bool""
			}
		],
		""payable"": false,
		""stateMutability"": ""nonpayable"",
		""type"": ""function""
	},
	{
		""constant"": false,
		""inputs"": [
			{
				""name"": ""_to"",
				""type"": ""address""
			},
			{
				""name"": ""_value"",
				""type"": ""uint256""
			}
		],
		""name"": ""transfer"",
		""outputs"": [
			{
				""name"": ""success"",
				""type"": ""bool""
			}
		],
		""payable"": false,
		""stateMutability"": ""nonpayable"",
		""type"": ""function""
	},
	{
		""constant"": false,
		""inputs"": [
			{
				""name"": ""_from"",
				""type"": ""address""
			},
			{
				""name"": ""_to"",
				""type"": ""address""
			},
			{
				""name"": ""_value"",
				""type"": ""uint256""
			}
		],
		""name"": ""transferFrom"",
		""outputs"": [
			{
				""name"": ""success"",
				""type"": ""bool""
			}
		],
		""payable"": false,
		""stateMutability"": ""nonpayable"",
		""type"": ""function""
	},
	{
		""inputs"": [],
		""payable"": false,
		""stateMutability"": ""nonpayable"",
		""type"": ""constructor""
	},
	{
		""constant"": true,
		""inputs"": [],
		""name"": ""_totalSupply"",
		""outputs"": [
			{
				""name"": """",
				""type"": ""uint256""
			}
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""constant"": true,
		""inputs"": [
			{
				""name"": ""_owner"",
				""type"": ""address""
			},
			{
				""name"": ""_spender"",
				""type"": ""address""
			}
		],
		""name"": ""allowance"",
		""outputs"": [
			{
				""name"": ""remaining"",
				""type"": ""uint256""
			}
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""constant"": true,
		""inputs"": [
			{
				""name"": ""_owner"",
				""type"": ""address""
			}
		],
		""name"": ""balanceOf"",
		""outputs"": [
			{
				""name"": ""balance"",
				""type"": ""uint256""
			}
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""constant"": true,
		""inputs"": [
			{
				""name"": """",
				""type"": ""address""
			}
		],
		""name"": ""balances"",
		""outputs"": [
			{
				""name"": """",
				""type"": ""uint256""
			}
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""constant"": true,
		""inputs"": [],
		""name"": ""decimals"",
		""outputs"": [
			{
				""name"": """",
				""type"": ""uint8""
			}
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""constant"": true,
		""inputs"": [],
		""name"": ""name"",
		""outputs"": [
			{
				""name"": """",
				""type"": ""string""
			}
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""constant"": true,
		""inputs"": [],
		""name"": ""symbol"",
		""outputs"": [
			{
				""name"": """",
				""type"": ""string""
			}
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""constant"": true,
		""inputs"": [],
		""name"": ""totalSupply"",
		""outputs"": [
			{
				""name"": ""totalSupply"",
				""type"": ""uint256""
			}
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	}
]
";

        public Contract Contract { get; set; }

        public StandardTokenService(Web3 web3, string address)
        {
            this.Web3 = web3;
            this.Contract = web3.Eth.GetContract(abi, address);
            this.ContractHandler = web3.Eth.GetContractHandler(address);
        }

        protected ContractHandler ContractHandler { get; set; }

        public async Task<TNumber> GetTotalSupplyAsync<TNumber>()
        {
            var function = GetTotalSupplyFunction();
            return await function.CallAsync<TNumber>();
        }

        protected Function GetTotalSupplyFunction()
        {
            return Contract.GetFunction("totalSupply");
        }

        public async Task<T> GetBalanceOfAsync<T>(string address)
        {
            var function = GetBalanceOfFunction();
            return await function.CallAsync<T>(address);
        }

        protected Function GetBalanceOfFunction()
        {
            return Contract.GetFunction("balanceOf");
        }

        public async Task<T> GetAllowanceAsync<T>(string addressOwner, string addressSpender)
        {
            var function = GetAllowanceFunction();
            return await function.CallAsync<T>(addressOwner, addressSpender);
        }

        protected Function GetAllowanceFunction()
        {
            return Contract.GetFunction("allowance");
        }

        public async Task<string> TransferAsync<T>(string addressFrom, string addressTo, T value, HexBigInteger gas)
        {
            var function = GetTransferFunction();
            return await function.SendTransactionAsync(addressFrom, gas, null, addressTo, value);
        }

        public async Task<string> TransferAsync(TransferFunction transferMessage)
        {
            return await ContractHandler.SendRequestAsync(transferMessage).ConfigureAwait(false);
        }

        public async Task<TransactionReceipt> TransferAndWaitForReceiptAsync(TransferFunction transferMessage)
        {
            return await ContractHandler.SendRequestAndWaitForReceiptAsync(transferMessage).ConfigureAwait(false);
        }

        protected Function GetTransferFunction()
        {
            return Contract.GetFunction("transfer");
        }

        public async Task<string> TransferFromAsync<T>(string addressFrom, string addressTransferedFrom, string addressTransferedTo,
            T value, HexBigInteger gas)
        {
            var function = GetTransferFromFunction();
            return await function.SendTransactionAsync(addressFrom, gas, null, addressTransferedFrom, addressTransferedTo, value);
        }

        protected Function GetTransferFromFunction()
        {
            return Contract.GetFunction("transferFrom");
        }

        public async Task ApproveAsync<T>(string addressFrom, string addressSpender, T value, HexBigInteger gas = null)
        {
            var function = GetApproveFunction();
            await function.SendTransactionAsync(addressFrom, gas, null, addressSpender, value);
        }

        protected Function GetApproveFunction()
        {
            return Contract.GetFunction("approve");
        }

        public Event GetApprovalEvent()
        {
            return Contract.GetEvent("Approval");
        }

        public Event GetTransferEvent()
        {
            return Contract.GetEvent("Transfer");
        }

    }
}
