using Enigma.Models;

namespace Enigma.Analysis.Tests;

/// <summary>
/// Pinned by the three cases the library already carries published answers for. Two
/// of the historical messages are issued with a second setting that is the same
/// machine written another way; a third is issued with one that merely reads the
/// message. Telling those apart is the whole job here, and these are the only three
/// worked examples of it anyone has published for this traffic.
/// </summary>
public class SettingsEquivalenceTests
{
    private static readonly SettingsEquivalence Equivalence = new(Machinery.Factory);

    private const string RaschCiphertext =
        "HCEYZTCSOPUPPZDICQRDLWXXFACTTJMBRDVCJJMMZRPYIKHZAWGLYXWTMJPQUEFSZBOTVRLALZXWVXTSLFFFAUDQFB" +
        "WRRYAPSBOWJMKLDUYUPFUQDOWVHAHCDWAUARSWTKOFVOYFPUFHVZFDGGPOOVGRMBPXXZCANKMONFHXPCKHJZBUMXJW" +
        "XKAUODXZUCVCXPFT";

    private static KeySheet Sheet(string name)
    {
        Assert.True(KeySheets.TryGet(name, out var sheet));

        return sheet;
    }

    [Fact]
    public void TheTwoGrafSpeeSettingsAreOneMachine()
    {
        // Rings AHX at EKD and rings AUX at EXD. The shift is thirteen on the middle
        // wheel, and that wheel is VI, whose two notches are themselves thirteen
        // apart -- so the shift maps the notch set onto itself and the stepping is
        // untouched as well as the wiring.
        var sheet = Sheet("graf-spee");
        var alternate = sheet.Copy();

        alternate.RingSettings = "AUX";
        alternate.Positions = "EXD";

        Assert.True(Equivalence.AreOneMachine(sheet, alternate));
        Assert.Null(Equivalence.PartsAt(sheet, alternate));
    }

    [Fact]
    public void ThePublishedDoenitzSettingIsTheSameMachineWrittenAnotherWay()
    {
        // Rings EPEL at CDSZ and rings AAEL at YOSZ. Shifted on the Greek wheel,
        // which never turns, and on the left wheel, whose notch drives nothing.
        var sheet = Sheet("doenitz");
        var published = sheet.Copy();

        published.RingSettings = "AAEL";
        published.Positions = "YOSZ";

        Assert.True(Equivalence.AreOneMachine(sheet, published));
    }

    [Fact]
    public void TheAlternateRaschSettingReadsTheMessageWithoutBeingTheSameMachine()
    {
        // The distinction this class exists for. These two read all 196 letters of
        // the signal alike and are not one machine: a longer message would part them,
        // which is the traffic pinning the wheels only as far as it runs.
        var sheet = Sheet("rasch");
        var alternate = sheet.Copy();

        alternate.RingSettings = "ZZTG";
        alternate.Positions = "NBHL";

        Assert.True(Equivalence.ReadTheSame(sheet, alternate, RaschCiphertext));
        Assert.False(Equivalence.AreOneMachine(sheet, alternate));

        var parted = Equivalence.PartsAt(sheet, alternate);

        Assert.NotNull(parted);
        Assert.True(parted > RaschCiphertext.Length,
            $"They part at {parted}, which is inside the {RaschCiphertext.Length} letters they are supposed to read alike.");
    }

    [Fact]
    public void ARingShiftedWithItsPositionIsOneMachineWhereTheNotchDrivesNothing()
    {
        // The general rule behind both of the cases above, stated on its own: the
        // leftmost wheel of a three wheel machine has nothing to its left, so its
        // notch drives nothing and its Ringstellung is not determined by anything the
        // machine does.
        var sheet = Sheet("barbarossa");
        var shifted = sheet.Copy();

        shifted.RingSettings = "GUL";
        shifted.Positions = "GLA";

        Assert.True(Equivalence.AreOneMachine(sheet, shifted));
    }

    [Fact]
    public void TheSameShiftOnAWheelWhoseNotchDrivesSomethingIsNotOneMachine()
    {
        // The other half of the rule, without which the first would be a claim that
        // ring settings never matter. Shift the fast wheel and the middle wheel is
        // carried at a different moment, so the machines part.
        var sheet = Sheet("barbarossa");
        var shifted = sheet.Copy();

        shifted.RingSettings = "BUQ";
        shifted.Positions = "BLF";

        Assert.False(Equivalence.AreOneMachine(sheet, shifted));
    }

    [Fact]
    public void ASettingOneLetterOutReadsMostOfTheMessageAndNotAllOfIt()
    {
        // Why the count exists rather than only the yes or no. Shifting the fast
        // wheel's ring by one moves the first turnover by one keypress, so the two
        // machines differ over a short opening stretch and agree everywhere after it.
        var sheet = Sheet("barbarossa");
        var near = sheet.Copy();

        near.RingSettings = "BUM";
        near.Positions = "BLB";

        var message = new string('A', 300);
        var (agreed, of) = Equivalence.AgreeOn(sheet, near, message);

        Assert.Equal(300, of);
        Assert.False(Equivalence.ReadTheSame(sheet, near, message));
        Assert.InRange(agreed, 200, 299);
    }

    [Fact]
    public void MachinesInDifferentAlphabetsAreNotOneMachine()
    {
        Assert.False(Equivalence.AreOneMachine(Sheet("barbarossa"), Sheet("z30")));
    }
}
