using DonationHub.Domain.Common;
using DonationHub.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.ValueObjects
{
    public sealed class Money : ValueObject
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; } = string.Empty;
        /// <summary>
        /// Defines which properties determine Value Object equality.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<object?> GetEqualityComponents()
        {
             yield return Amount;
            yield return Currency;
        }

        private Money() => Currency = string.Empty;

        // Private constructor prevents creating invalid Money directly.
        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        /// <summary>
        /// Factory method to create a Money instance with valid amount and currency.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public static Result<Money> Create(decimal amount, string currency) 
        { 
            if(amount<=0)
                return Result.Failure<Money>(Error.Validation("Money.AmountMustBePositive", "Money amount must be greater than zero."));

            if(!TryNormalizeCurrency(currency, out var normalizedCurrency))
                return Result.Failure<Money>(Error.Validation("Money.InvalidCurrency", "Invalid currency code."));
            
            return Result<Money>.Success(new Money (amount, normalizedCurrency));
        }

        /// <summary>
        ///  Adds two Money values only when they use the same currency.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Result<Money> Add(Money? other) 
        { 
            if(other is null)
                return Result.Failure<Money>(Error.Validation("Money.NullOther", "Cannot add null Money value."));

            if(!IsSameCurrency(other))
                return Result.Failure<Money>(
                 Error.UnprocessableEntity("Money.CurrencyMismatch", "Money values must use the same currency."));

            var totalAmount = Amount + other.Amount;

            return Result<Money>.Success(new Money(totalAmount, Currency));

        }


        // Compares two Money values while ensuring their currencies match.
        public Result<bool> IsGreaterThanOrEqualTo(Money? other)
        {
            if (other is null)
            {
                return Result.Failure<bool>(
                    Error.Validation(
                        "Money.OtherRequired",
                        "The other money value is required."));
            }

            if (!IsSameCurrency(other))
            {
                return Result.Failure<bool>(
                Error.UnprocessableEntity(
                    "Money.CurrencyMismatch",
                    "Money values must use the same currency."));
            }

            return Result.Success(Amount >= other.Amount);
        }

        private bool IsSameCurrency(Money? other) {
            return other is not null 
                    && string.Equals(Currency, other.Currency, StringComparison.Ordinal);
        }

        /// <summary>
        /// Creates a zero amount using the currency of an existing Money instance.
        /// RaisedAmount must start with Zero.
        /// </summary>
        /// <param name="currencyBasis"></param>
        /// <returns></returns>
        internal static Money Zero(Money currencyBasis)
        {
            ArgumentNullException.ThrowIfNull(currencyBasis);

            return new Money(0, currencyBasis.Currency);
        }

        /// <summary>
        /// Validates the currency code and normalizes it to uppercase
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="normalized"></param>
        /// <returns></returns>
        private static bool TryNormalizeCurrency(string? currency, out string normalized)
        {
           if(string.IsNullOrWhiteSpace(currency))
            {
                normalized = string.Empty;
                return false;
            }

           normalized=currency.Trim().ToUpperInvariant();

            //validate code length and characters:

            if (normalized.Length != 3 || !System.Text.RegularExpressions.Regex.IsMatch(normalized, "^[A-Z]{3}$"))
            {
                normalized = string.Empty;
                return false;
            }

            return true;

        }

    }
    }

