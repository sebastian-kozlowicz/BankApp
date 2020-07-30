﻿using BankApp.Interfaces;
using BankApp.Models;

namespace BankApp.Helpers
{
    public class ExternalTransferService : ITransferService<ExternalTransferService>
    {
        /// <summary>
        /// Method that fakes real transfer order in external system like Elixir
        /// </summary>
        public void Create(BankAccount bankAccount, BankAccount targetBankAccount, decimal value)
        {
            throw new System.NotImplementedException();
        }
    }
}