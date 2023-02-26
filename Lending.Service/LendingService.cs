using Lending.Enitity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;

namespace Lending.Service
{
    public class LendingService : ILendingService
    {
        private readonly IConfiguration _config;

        public LendingService(IConfiguration config)
        {
            _config = config; //Simulate if we need to pull hardcoded value from appsetting using dependency injection
        }

        private static Lender UserInput()
        {
            //Input (Act as the request model)
            Lender lender = new();
            Console.WriteLine("What is your loan amount?");
            lender.Loan = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("What is your asset value?");
            lender.AssetValue = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("What is your credit score?");
            lender.CreditScore = Convert.ToInt32(Console.ReadLine());

            return lender;
        }

        public void Run()
        {
            Console.WriteLine("Welcome to Lending!");

            //UserInputCommandLine();

            InputFromJson();

            Console.ReadLine();
        }

        private void InputFromJson()
        {
            using StreamReader stream = new StreamReader("data.json");
            var lenders = JsonConvert.DeserializeObject<List<Lender>>(stream.ReadToEnd());
            int totalLoanValue = default;

            foreach (var lenderInfo in lenders)
            {
                bool isValidate = ValidateApplication(lenderInfo);
                totalLoanValue = +lenderInfo.Loan;

                if (isValidate)
                {
                    Console.WriteLine($"Applicant {lenderInfo.Id} was successful");
                }
                else
                {
                    Console.WriteLine($"Applicant {lenderInfo.Id} was declined");
                }
            }
            Console.WriteLine($"Total Applicant: {lenders.Count}");
            Console.WriteLine($"Total value of loans : {totalLoanValue}");
            Console.WriteLine($"Total value of loans (mean average) : {totalLoanValue / lenders.Count}");
        }

            private void UserInputCommandLine()
        {
            Lender lenderInfo = LendingService.UserInput(); //uncomment to use your own inputs
            bool isValidate = ValidateApplication(lenderInfo);
            if (isValidate)
            {
                Console.WriteLine($"Applicant {lenderInfo.Id} was successful");
            }
            else
            {
                Console.WriteLine($"Applicant {lenderInfo.Id} was declined");
            }
        }

        //In real life these method would be include in interface/service so it can be called from other services/controller/unit test.

        private bool ValidateApplication(Lender lenderInfo)
        {
            bool isApplicationSuccessful = default;

            if (lenderInfo.Loan < _config.GetValue<int>("LoanMinValue") &&
                            lenderInfo.Loan < _config.GetValue<int>("LoanMaxValue"))
                isApplicationSuccessful = false;

            var sixtyPercentLoanToValue = GetLoadToValue(60, lenderInfo);
            var eightyPercentLoanToValue = GetLoadToValue(80, lenderInfo);
            var ninetyPercentLoanToValue = GetLoadToValue(90, lenderInfo);

            if (lenderInfo.Loan >= 1000000 &&
                sixtyPercentLoanToValue <= lenderInfo.AssetValue ||
                lenderInfo.CreditScore >= 950)
                isApplicationSuccessful = true;

            if (lenderInfo.Loan < 1000000)
            {
                if (lenderInfo.AssetValue < sixtyPercentLoanToValue &&
                    lenderInfo.CreditScore >= 750)
                    isApplicationSuccessful = true;

                if (lenderInfo.AssetValue < eightyPercentLoanToValue &&
                    lenderInfo.CreditScore >= 800)
                    isApplicationSuccessful = true;

                if (lenderInfo.AssetValue < ninetyPercentLoanToValue &&
                    lenderInfo.CreditScore >= 900)
                    isApplicationSuccessful = true;

                if (lenderInfo.AssetValue >=  ninetyPercentLoanToValue)
                    isApplicationSuccessful = false;
            }

            return isApplicationSuccessful;
        }

        private static int GetLoadToValue(int percentage, Lender lenderInfo) =>
            lenderInfo.Loan * percentage / 100;

    }
}
