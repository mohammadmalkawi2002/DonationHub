using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Common
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        // Derived classes must implement this method to provide the components that define equality for the value object.
        protected abstract IEnumerable<object?> GetEqualityComponents();

        
        public bool Equals(ValueObject? other)
        {
            return other is not null
           && GetType() == other.GetType()
           && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override bool Equals(object? obj)
        {
            return obj is ValueObject other && Equals(other);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(GetType());
            foreach (var component in GetEqualityComponents())
            {
                hash.Add(component);
            }

            return hash.ToHashCode();
        }

        public static bool operator ==(ValueObject? left, ValueObject? right)
        {
            return ReferenceEquals(left, right) || (left is not null && left.Equals(right));
        }

        public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);

    }
}
