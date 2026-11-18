namespace si730pc2u201621873.API.Sale.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the vendor of a purchase order.
/// </summary>
public sealed record Vendor
{
    private const int MinLength = 4;
    private const int MaxLength = 50;

    /// <summary>
    ///     Initializes a new <see cref="Vendor"/> with the given raw string value.
    /// </summary>
    /// <param name="value">The raw string. Must be non-null, non-whitespace, and between 4 and 50 characters.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> breaks the validation rule.</exception>
    public Vendor(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Vendor cannot be null, empty, or whitespace.");

        if (value.Length < MinLength || value.Length > MaxLength)
            throw new ArgumentException($"Vendor must be between {MinLength} and {MaxLength} characters.");

        Value = value;
    }

    /// <summary>
    ///     Gets the underlying primitive value.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;
}
