using Enigma.Models;

namespace Enigma.App;

/// <summary>
/// The key sheets a front end can offer, under the names it knows them by. As with
/// <see cref="IPartsCatalogue"/>, an implementation may draw on the sheets the
/// library ships with, on sheets the operator supplied, or on both.
/// </summary>
public interface IKeySheetCatalogue
{
    /// <summary>Every sheet on offer, packaged and supplied alike.</summary>
    IReadOnlyDictionary<string, KeySheet> All { get; }

    IReadOnlyList<string> Names { get; }

    bool TryGet(string name, out KeySheet keySheet);
}
