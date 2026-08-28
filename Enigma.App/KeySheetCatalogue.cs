using Enigma.Models;

namespace Enigma.App;

/// <summary>
/// The packaged key sheets, with any the operator supplied laid over the top. A
/// supplied sheet under a packaged name replaces it, which is what lets someone
/// try their own Barbarossa settings without renaming them first.
/// </summary>
public sealed class KeySheetCatalogue : IKeySheetCatalogue
{
    public KeySheetCatalogue(IEnumerable<KeyValuePair<string, KeySheet>>? supplied = null)
    {
        var sheets = new Dictionary<string, KeySheet>(KeySheets.All, StringComparer.OrdinalIgnoreCase);
        var shadowed = new List<string>();

        foreach (var (name, sheet) in supplied ?? [])
        {
            if (KeySheets.All.ContainsKey(name))
            {
                shadowed.Add(name);
            }

            sheets[name] = sheet;
        }

        All = sheets;
        Names = [.. sheets.Keys];
        Shadowed = shadowed;
    }

    public IReadOnlyDictionary<string, KeySheet> All { get; }

    public IReadOnlyList<string> Names { get; }

    /// <summary>
    /// Packaged names a supplied sheet has taken over. The packaged sheets are each
    /// verified against a published message, so a front end may reasonably want to
    /// say when one is no longer the sheet it claims to be.
    /// </summary>
    public IReadOnlyList<string> Shadowed { get; }

    public bool TryGet(string name, out KeySheet keySheet) =>
        All.TryGetValue(name ?? string.Empty, out keySheet!);
}
