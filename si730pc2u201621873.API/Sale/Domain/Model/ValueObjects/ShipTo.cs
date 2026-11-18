namespace si730pc2u201621873.API.Sale.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the shipping destination of a purchase order.
/// </summary>
public sealed record ShipTo
{
    private const int MaxLength = 255;

    /// <summary>
    ///     Initializes a new <see cref="ShipTo"/> with the given raw string value.
    /// </summary>
    /// <param name="value">The raw string. Must be non-null, non-whitespace, and at most 255 characters.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> breaks the validation rule.</exception>
    public ShipTo(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("ShipTo cannot be null, empty, or whitespace.");

        if (value.Length > MaxLength)
            throw new ArgumentException($"ShipTo cannot be longer than {MaxLength} characters.");

        Value = value;
    }

    /// <summary>
    ///     Gets the underlying primitive value.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;
}
