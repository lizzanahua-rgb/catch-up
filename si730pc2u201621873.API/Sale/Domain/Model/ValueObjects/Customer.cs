namespace si730pc2u201621873.API.Sale.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the customer (brand) that issues a purchase order.
/// </summary>
public sealed record Customer
{
    private const int MinLength = 4;
    private const int MaxLength = 50;

    /// <summary>
    ///     Initializes a new <see cref="Customer"/> with the given raw string value.
    /// </summary>
    /// <param name="value">The raw string. Must be non-null, non-whitespace, and between 4 and 50 characters.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> breaks the validation rule.</exception>
    public Customer(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Customer cannot be null, empty, or whitespace.");

        if (value.Length < MinLength || value.Length > MaxLength)
            throw new ArgumentException($"Customer must be between {MinLength} and {MaxLength} characters.");

        Value = value;
    }

    /// <summary>
    ///     Gets the underlying primitive value.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;
}
