using BankApp.Enumerators;
using BankApp.Helpers.Builders;
using BankApp.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BankApp.Helpers.Factories
{
    public class PaymentCardNumberFactory : IPaymentCardNumberFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentCardNumberFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPaymentCardNumberBuilder GetPaymentCardNumberBuilder(IssuingNetwork issuingNetwork)
        {
            if (issuingNetwork == IssuingNetwork.Mastercard)
                using (var scope = _serviceProvider.CreateScope())
                    return scope.ServiceProvider.GetService<MastercardPaymentCardNumberBuilder>();

            using (var scope = _serviceProvider.CreateScope())
                return scope.ServiceProvider.GetService<VisaPaymentCardNumberBuilder>();
        }
    }
}
