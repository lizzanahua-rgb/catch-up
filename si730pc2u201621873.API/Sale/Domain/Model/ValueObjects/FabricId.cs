namespace si730pc2u201621873.API.Sale.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing a valid fabric identifier of Hialpesa.
/// </summary>
/// <remarks>
///     Only the values defined in <see cref="EFabric"/> (1 to 8) are valid; any other value is rejected.
///     When no fabric is specified, <see cref="Default"/> (Algodon) must be used.
/// </remarks>
public sealed record FabricId
{
    /// <summary>
    ///     Initializes a new <see cref="FabricId"/> with the given raw integer value.
    /// </summary>
    /// <param name="value">The raw fabric identifier. Must match one of the <see cref="EFabric"/> values.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is not defined in <see cref="EFabric"/>.</exception>
    public FabricId(int value)
    {
        if (!Enum.IsDefined((EFabric)value))
        {
            var validValues = string.Join(", ", Enum.GetValues<EFabric>().Select(f => $"{(int)f}={f}"));
            throw new ArgumentException($"FabricId {value} is not valid. Valid values are: {validValues}.");
        }

        Value = value;
    }

    /// <summary>
    ///     Gets the default fabric (Algodon) used when no fabric is specified.
    /// </summary>
    public static FabricId Default => new((int)EFabric.Algodon);

    /// <summary>
    ///     Gets the underlying primitive value (the fabric Id).
    /// </summary>
    public int Value { get; }

    /// <summary>
    ///     Gets the fabric as an <see cref="EFabric"/> member.
    /// </summary>
    public EFabric Fabric => (EFabric)Value;

    /// <summary>
    ///     Gets the fabric name.
    /// </summary>
    public string Name => Fabric.ToString();

    /// <inheritdoc />
    public override string ToString() => Name;
}
