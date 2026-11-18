namespace si730pc2u201621873.API.Sale.Domain.Model.ValueObjects;

/// <summary>
///     Enumeration of the fabrics offered by Hialpesa.
/// </summary>
/// <remarks>
///     The numeric value is the persisted <c>fabric_id</c>; the member name is the fabric name.
///     Author: NOMBRES Y APELLIDOS DEL ESTUDIANTE
/// </remarks>
public enum EFabric
{
    /// <summary>Cotton (Id 1). Default fabric when none is specified.</summary>
    Algodon = 1,

    /// <summary>Modal (Id 2).</summary>
    Modal = 2,

    /// <summary>Elastane (Id 3).</summary>
    Elastano = 3,

    /// <summary>Polyester (Id 4).</summary>
    Poliester = 4,

    /// <summary>Nylon (Id 5).</summary>
    Nailon = 5,

    /// <summary>Acrylic (Id 6).</summary>
    Acrilico = 6,

    /// <summary>Rayon (Id 7).</summary>
    Rayon = 7,

    /// <summary>Lyocell (Id 8).</summary>
    Lyocell = 8
}
