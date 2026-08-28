using Bunit;
using Enigma;
using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Enigma.Web.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Web.Tests;

public class KeySheetEditorTests : BunitContext
{
    // bUnit will not take services once anything has been resolved, and several of
    // these render twice.
    public KeySheetEditorTests() => Services.AddEnigmaServices();

    [Fact]
    public void ThePartsOfferedComeFromTheCatalogue()
    {
        // Not a hardcoded list: a parts file's wheels must appear here without the
        // editor knowing they exist.
        var editor = Render(Sheet());

        Assert.Equal(MachineParts.RotorNames.Count, editor.FindAll("[data-testid=wheel]:first-of-type option").Count);
        Assert.Equal(MachineParts.ReflectorNames.Count, editor.FindAll("[data-testid=reflector] option").Count);
        Assert.Equal(MachineParts.LayoutNames.Count, editor.FindAll("[data-testid=model] option").Count);
        Assert.Equal(MachineParts.CharacterMapNames.Count, editor.FindAll("[data-testid=alphabet] option").Count);
    }

    [Fact]
    public void TheEntryWheelOffersTheModelsOwnChoiceAsWellAsTheNamedOnes()
    {
        // Left unset the model decides, which is what makes a G-31 keyboard wired
        // without the key sheet having to say so.
        var editor = Render(Sheet());

        Assert.Equal(
            MachineParts.EntryWheelNames.Count + 1,
            editor.FindAll("[data-testid=entry] option").Count);
    }

    [Fact]
    public void OneDropdownPerWheelInTheMachine()
    {
        Assert.Equal(3, Render(Sheet()).FindAll("[data-testid=wheel]").Count);
    }

    [Fact]
    public void ChangingAWheelReportsANewKeySheet()
    {
        KeySheet? changed = null;
        var editor = Render(Sheet(), sheet => changed = sheet);

        editor.FindAll("[data-testid=wheel]")[2].Change("V");

        Assert.Equal("I II V", changed?.Rotors);
    }

    [Fact]
    public void ChangingTheModelReportsANewKeySheet()
    {
        KeySheet? changed = null;
        var editor = Render(Sheet(), sheet => changed = sheet);

        editor.Find("[data-testid=model]").Change("G-31");

        Assert.Equal("G-31", changed?.Model);
    }

    [Fact]
    public void AddingAWheelPutsAThinOneOnTheLeft()
    {
        // The wheels already fitted shift right, and the new one is thin, because a
        // fourth wheel only fits in the space a thin reflector frees. Offering a
        // full width wheel there would build a machine the layout refuses.
        var parts = new ServiceCollection()
            .AddEnigmaServices()
            .BuildServiceProvider()
            .GetRequiredService<IPartsCatalogue>();

        KeySheet? changed = null;
        var editor = Render(Sheet(), sheet => changed = sheet);

        editor.Find("[data-testid=add-wheel]").Click();

        Assert.Equal(4, changed?.Rotors.Split(' ').Length);
        Assert.EndsWith("I II III", changed?.Rotors);
        Assert.True(parts.CreateRotor(changed!.Rotors.Split(' ')[0]).IsThin);
    }

    [Fact]
    public void AddingAWheelKeepsTheSettingsAsLongAsTheWheels()
    {
        // A Ringstellung shorter than the wheels is a key sheet that cannot be read.
        KeySheet? changed = null;
        var editor = Render(Sheet(rings: "BUL", positions: "BLA"), sheet => changed = sheet);

        editor.Find("[data-testid=add-wheel]").Click();

        Assert.Equal(4, changed?.RingSettings.Length);
        Assert.Equal(4, changed?.Positions.Length);
        Assert.EndsWith("BUL", changed?.RingSettings);
    }

    [Fact]
    public void RemovingAWheelShortensTheSettingsToMatch()
    {
        KeySheet? changed = null;
        var editor = Render(Sheet(wheels: "BETA II IV I", rings: "AAAV", positions: "VJNA"),
            sheet => changed = sheet);

        editor.Find("[data-testid=remove-wheel]").Click();

        Assert.Equal("II IV I", changed?.Rotors);
        Assert.Equal("AAV", changed?.RingSettings);
        Assert.Equal("JNA", changed?.Positions);
    }

    [Fact]
    public void ASettingWrittenAsNumbersIsLeftForTheKeySheetToParse()
    {
        // "01 01 01" means the same as "AAA", and guessing how to lengthen it would
        // be worse than letting the parser say what it makes of it.
        KeySheet? changed = null;
        var editor = Render(Sheet(rings: "01 01 01"), sheet => changed = sheet);

        editor.Find("[data-testid=add-wheel]").Click();

        Assert.Equal("01 01 01", changed?.RingSettings);
    }

    [Fact]
    public void TheReflectorSettingIsOfferedOnlyWhenTheReflectorTurns()
    {
        Assert.Empty(Render(Sheet()).FindAll("[data-testid=ukw-position]"));
        Assert.NotEmpty(Render(Sheet(), turning: true).FindAll("[data-testid=ukw-position]"));
    }

    [Fact]
    public void TheEditorShowsTheSettingsInUse()
    {
        // An editor that opens on blank fields is offering to replace the settings
        // rather than to adjust them.
        var editor = Render(Sheet(wheels: "II IV V", rings: "BUL", positions: "BLA"));

        Assert.Equal("BUL", editor.Find("[data-testid=rings]").GetAttribute("value"));
        Assert.Equal("BLA", editor.Find("[data-testid=positions]").GetAttribute("value"));
        Assert.Equal(
            ["II", "IV", "V"],
            editor.FindAll("[data-testid=wheel]").Select(wheel => wheel.GetAttribute("value")));
        Assert.Equal("B", editor.Find("[data-testid=reflector]").GetAttribute("value"));
    }

    [Fact]
    public void EveryControlShowsAValueItActuallyOffers()
    {
        // A key sheet names its parts however the writer wrote them -- "Service",
        // "Latin", "B-Thin" -- and the lists hold the normalised names. Matched any
        // other way the control shows nothing at all, which reads as "unset" when
        // the machine is in fact set.
        var editor = Render(new KeySheet
        {
            Reflector = "B-Thin",
            Rotors = "Beta II IV I",
            RingSettings = "AAAV",
            Positions = "VJNA",
            Model = "Service",
            CharacterMap = "Latin",
            EntryWheel = "Standard"
        });

        Assert.All(
            new[] { "model", "reflector", "entry", "alphabet" },
            control => Assert.Contains(
                editor.Find($"[data-testid={control}]").GetAttribute("value"),
                editor.FindAll($"[data-testid={control}] option").Select(o => o.GetAttribute("value"))));

        Assert.All(
            editor.FindAll("[data-testid=wheel]"),
            wheel => Assert.Contains(
                wheel.GetAttribute("value"),
                wheel.Children.Select(option => option.GetAttribute("value"))));
    }

    [Fact]
    public void LeavingTheEntryWheelToTheModelShowsAsSuch()
    {
        var editor = Render(Sheet());

        Assert.Equal(string.Empty, editor.Find("[data-testid=entry]").GetAttribute("value"));
    }

    [Fact]
    public void EditingDoesNotDisturbTheSheetInUse()
    {
        // The machine was keyed from this sheet; an editor writing into it would
        // change the settings out from under a machine already running.
        var sheet = Sheet();
        var editor = Render(sheet);

        editor.Find("[data-testid=rings]").Change("XYZ");

        Assert.Equal("AAA", sheet.RingSettings);
    }

    private IRenderedComponent<KeySheetEditor> Render(
        KeySheet sheet, Action<KeySheet>? changed = null, bool turning = false)
    {
        return Render<KeySheetEditor>(parameters => parameters
            .Add(editor => editor.Sheet, sheet)
            .Add(editor => editor.ShowsReflectorSetting, turning)
            .Add(editor => editor.OnChanged, s => changed?.Invoke(s)));
    }

    private static KeySheet Sheet(
        string wheels = "I II III", string rings = "AAA", string positions = "AAA") => new()
    {
        Name = "Test",
        Reflector = "B",
        Rotors = wheels,
        RingSettings = rings,
        Positions = positions
    };
}
