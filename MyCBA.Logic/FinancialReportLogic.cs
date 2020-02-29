using MyCBA.Core.Models;
using MyCBA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Logic
{
    public class FinancialReportLogic
    {
        //public void CreateTransaction(CustomerAccount account, decimal amount, TransactionType trnType)
        //{
        //    if (account.AccountType == AccountType.Loan)
        //    {
        //        //Record this transaction for Trial Balance generation
        //        Transaction transaction = new Transaction();
        //        transaction.Amount = amount;
        //        transaction.Date = DateTime.Now;
        //        transaction.AccountName = account.AccountName;
        //        transaction.SubCategory = "Customer's Loan Account";
        //        transaction.MainCategory = MainGlCategory.Asset;
        //        transaction.TransactionType = trnType;
        //        db.Transactions.Add(transaction);
        //    }
        //    else
        //    {
        //        //Record this transaction for Trial Balance generation
        //        Transaction transaction = new Transaction();
        //        transaction.Amount = amount;
        //        transaction.Date = DateTime.Now;
        //        transaction.AccountName = account.AccountName;
        //        transaction.SubCategory = "Customer Account";
        //        transaction.MainCategory = MainGlCategory.Liability;
        //        transaction.TransactionType = trnType;
        //        db.Transactions.Add(transaction);
        //    }
        //}
        public void CreateTransaction(GlAccount account, decimal amount, TransactionType trnType)
        {
            //Record this transaction for Trial Balance generation
            Transaction transaction = new Transaction();
            transaction.Amount = amount;
            transaction.Date = DateTime.Now;
            transaction.AccountName = account.AccountName;
            transaction.SubCategory = account.GlCategory.Name;
            transaction.MainCategory = account.GlCategory.MainCategory;
            transaction.TransactionType = trnType;

            new TransactionRepository().Insert(transaction);
        }

        public void CreateTransaction(CustomerAccount account, decimal amount, TransactionType trnType)
        {
            if (account.AccountType == AccountType.Loan)
            {
                //Record this transaction for Trial Balance generation
                Transaction transaction = new Transaction();
                transaction.Amount = amount;
                transaction.Date = DateTime.Now;
                transaction.AccountName = account.AccountName;
                transaction.SubCategory = "Customer's Loan Account";
                transaction.MainCategory = MainGlCategory.Asset;
                transaction.TransactionType = trnType;
                new TransactionRepository().Insert(transaction);
            }
            else
            {
                //Record this transaction for Trial Balance generation
                Transaction transaction = new Transaction();
                transaction.Amount = amount;
                transaction.Date = DateTime.Now;
                transaction.AccountName = account.AccountName;
                transaction.SubCategory = "Customer Account";
                transaction.MainCategory = MainGlCategory.Liability;
                transaction.TransactionType = trnType;
                new TransactionRepository().Insert(transaction);
            }
        }
    }
}
