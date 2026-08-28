using Bunit;
using Enigma;
using Enigma.App;
using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Enigma.Web.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Web.Tests;

public class HomeTests : BunitContext
{
    private IKeySheetCatalogue _catalogue = new KeySheetCatalogue();

    // bUnit will not take services once anything has been resolved, and several of
    // these render two pages.
    public HomeTests()
    {
        Services.AddEnigmaServices();
        Services.AddSingleton<IKeySheetCatalogue>(_ => _catalogue);
    }

    [Fact]
    public void EveryKeySheetOnOfferIsListed()
    {
        Assert.Equal(KeySheets.All.Count, Page().FindAll("[data-testid=sheet] option").Count);
    }

    [Fact]
    public void APanelIsShownForTheSheetInUse()
    {
        var page = Page();

        Assert.NotNull(page.Find("[data-testid=panel]"));
        Assert.Equal(26, page.FindAll("[data-testid=key]").Count);
    }

    [Fact]
    public void ChoosingAnotherSheetKeysAnotherMachine()
    {
        var page = Page();

        page.Find("[data-testid=sheet]").Change("u264");

        // The M4 has four wheels where the Enigma I has three.
        Assert.Equal(4, page.FindAll("[data-testid=window]").Count);
    }

    // -- the message ------------------------------------------------------------

    [Fact]
    public void TypingOnTheKeyboardWritesTheMessageAndTheCipher()
    {
        var page = Page();

        Type(page, "ATTACK");

        Assert.Equal("ATTACK", Plaintext(page));
        Assert.Equal(Session("default").Type("ATTACK"), Ciphertext(page));
    }

    [Fact]
    public void TheKeyboardAndTheTextAgree()
    {
        // Two ways of entering one message. If they could differ, one of the two
        // views would be lying about what the machine did.
        var typed = Page();
        var written = Page();

        Type(typed, "ATTACKATDAWN");
        written.Find("[data-testid=plaintext]").Change("ATTACKATDAWN");

        Assert.Equal(Ciphertext(typed), Ciphertext(written));
    }

    [Fact]
    public void WritingTheMessageKeysItFromTheStart()
    {
        var page = Page();

        Type(page, "ZZZZZ");
        page.Find("[data-testid=plaintext]").Change("ATTACK");

        // Keyed from the start, not carried on from the five letters before it, so
        // the fast wheel stands six on from A -- the length of the message.
        Assert.Equal(Session("default").Type("ATTACK"), Ciphertext(page));
        Assert.Equal("AAG", Windows(page));
    }

    [Fact]
    public void AKeyTheMachineHasNotGotChangesNothing()
    {
        var page = Page();

        page.Find("[data-testid=panel]").KeyDown(
            new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "7" });

        Assert.Empty(Plaintext(page));
        Assert.Empty(Ciphertext(page));
        Assert.Equal("AAA", Windows(page));
    }

    [Fact]
    public void PressingTheLitLetterGivesTheOriginalBack()
    {
        // Reciprocity, as an operator would check it.
        var page = Page();

        Type(page, "A");

        var lamp = Ciphertext(page)[0];

        page.Find("[data-testid=clear]").Click();
        Type(page, lamp.ToString());

        Assert.Equal("A", Ciphertext(page));
    }

    [Fact]
    public void ClearingPutsTheWheelsBack()
    {
        var page = Page();

        Type(page, "ATTACK");
        page.Find("[data-testid=clear]").Click();

        Assert.Empty(Plaintext(page));
        Assert.Empty(Ciphertext(page));
        Assert.Equal("AAA", Windows(page));
    }

    [Fact]
    public void FittingTheTextToTheKeyboardIsOffered()
    {
        var page = Page();

        page.Find("[data-testid=plaintext]").Change("ATTACK AT DAWN");
        page.Find("[data-testid=prepare]").Change(true);

        Assert.Equal(Session("default").Type("ATTACKXATXDAWN"), Ciphertext(page));
    }

    [Fact]
    public void WithoutFittingItTheSpacesAreSimplyNotKeyed()
    {
        var page = Page();

        page.Find("[data-testid=plaintext]").Change("ATTACK AT DAWN");

        Assert.Equal(Session("default").Type("ATTACKATDAWN"), Ciphertext(page));
    }

    [Fact]
    public void TheCipherIsWrittenInGroups()
    {
        var page = Page();

        page.Find("[data-testid=plaintext]").Change("ATTACKATDAWN");
        page.Find("[data-testid=grouped]").Change(true);

        Assert.Equal(MessageText.InGroups(Session("default").Type("ATTACKATDAWN")), Ciphertext(page));
    }

    [Fact]
    public void TheGroupSizeCanBeSet()
    {
        var page = Page();

        page.Find("[data-testid=plaintext]").Change("ATTACKATDAWN");
        page.Find("[data-testid=grouped]").Change(true);
        page.Find("[data-testid=group-size]").Change("4");

        Assert.Equal(MessageText.InGroups(Session("default").Type("ATTACKATDAWN"), 4), Ciphertext(page));
    }

    [Fact]
    public void GroupingChangesOnlyHowTheCipherIsWrittenOut()
    {
        // Writing the message out in groups is a convention of the signaller, not
        // an operation on the machine. Asserting the window alone cannot show that:
        // re-keying the same message lands on the same window. A lamp still held
        // down can, because re-keying would put it out.
        var page = Page();

        Type(page, "ATTACK");
        page.Find("[data-testid=key][data-letter=A]").MouseDown();

        var window = Windows(page);
        var lamp = page.Find("[data-testid=lamp].lamp-lit").TextContent;

        page.Find("[data-testid=grouped]").Change(true);

        Assert.Equal(window, Windows(page));
        Assert.Equal(lamp, page.Find("[data-testid=lamp].lamp-lit").TextContent);
        Assert.Equal(Session("default").Type("ATTACKA"), Ciphertext(page).Replace(" ", string.Empty));
    }

    [Fact]
    public void ChangingASettingKeysTheSameMessageAgain()
    {
        // The point of being able to change the settings: the same text, keyed a
        // different way.
        var page = Page();

        page.Find("[data-testid=plaintext]").Change("ATTACKATDAWN");

        var before = Ciphertext(page);

        page.Find("[data-testid=positions]").Change("XYZ");

        Assert.Equal("ATTACKATDAWN", Plaintext(page));
        Assert.NotEqual(before, Ciphertext(page));
        Assert.NotEmpty(Ciphertext(page));
    }

    [Fact]
    public void ChoosingAnotherSheetKeysTheSameMessageAgain()
    {
        var page = Page();

        page.Find("[data-testid=plaintext]").Change("ATTACKATDAWN");

        var before = Ciphertext(page);

        page.Find("[data-testid=sheet]").Change("barbarossa");

        Assert.Equal("ATTACKATDAWN", Plaintext(page));
        Assert.NotEqual(before, Ciphertext(page));
    }

    // -- following the current --------------------------------------------------

    [Fact]
    public void TheCurrentIsNotFollowedUnlessAskedFor()
    {
        var page = Page();

        Type(page, "A");

        Assert.NotEmpty(page.FindAll("[data-testid=signal-off]"));
        Assert.Empty(page.FindAll("[data-testid=step]"));
    }

    [Fact]
    public void FollowingTheCurrentShowsTheLastKeypress()
    {
        var page = Page();

        Follow(page);
        Type(page, "A");

        Assert.NotEmpty(page.FindAll("[data-testid=step]"));
        Assert.Contains("A", page.Find("[data-testid=signal-ends]").TextContent);
    }

    [Fact]
    public void TheLampShownIsTheLampTheTraceReports()
    {
        // Two views of one keypress. If they could differ, one of them would be
        // describing a machine that is not the one enciphering.
        var page = Page();

        Follow(page);
        page.Find("[data-testid=key][data-letter=A]").MouseDown();

        var lamp = page.Find("[data-testid=lamp].lamp-lit").TextContent;
        var steps = page.FindAll("[data-testid=step] .step-flow");

        Assert.EndsWith(lamp, steps[^1].TextContent.Trim());
    }

    [Fact]
    public void TheWindowShownIsTheWindowTheTraceReports()
    {
        var page = Page();

        Follow(page);
        Type(page, "ATTACK");

        Assert.Contains(Windows(page), page.Find("[data-testid=signal-ends]").TextContent);
    }

    [Fact]
    public void AskingToFollowFillsTheViewFromTheMessageAlreadyKeyed()
    {
        // Turning it on after typing should not leave it blank until the next key.
        var page = Page();

        Type(page, "ATTACK");
        Follow(page);

        Assert.NotEmpty(page.FindAll("[data-testid=step]"));
    }

    [Fact]
    public void TheViewFollowsTheMachineAcrossAChangeOfSettings()
    {
        // Changing the settings builds a new machine, and a watcher that quietly
        // stopped reporting would be worse than one that threw.
        var page = Page();

        Follow(page);
        page.Find("[data-testid=positions]").Change("XYZ");
        Type(page, "A");

        Assert.NotEmpty(page.FindAll("[data-testid=step]"));
    }

    [Fact]
    public void TheViewFollowsTheMachineAcrossAChangeOfKeySheet()
    {
        var page = Page();

        Follow(page);
        page.Find("[data-testid=sheet]").Change("u264");
        Type(page, "A");

        // The M4 has a fourth wheel, so the path is two steps longer.
        Assert.Equal(13, page.FindAll("[data-testid=step]").Count);
    }

    [Fact]
    public void GivingUpFollowingClearsTheView()
    {
        var page = Page();

        Follow(page);
        Type(page, "A");
        page.Find("[data-testid=watching]").Change(false);

        Assert.Empty(page.FindAll("[data-testid=step]"));
        Assert.NotEmpty(page.FindAll("[data-testid=signal-off]"));
    }

    [Fact]
    public void FollowingTheCurrentDoesNotChangeTheCipher()
    {
        var watched = Page();
        var quiet = Page();

        Follow(watched);
        watched.Find("[data-testid=plaintext]").Change("ATTACKATDAWN");
        quiet.Find("[data-testid=plaintext]").Change("ATTACKATDAWN");

        Assert.Equal(Ciphertext(quiet), Ciphertext(watched));
    }

    [Fact]
    public void ClearingTheMessageClearsTheView()
    {
        // The path shown belongs to a keypress in the message. Clear the message
        // and there is no such keypress any more.
        var page = Page();

        Follow(page);
        Type(page, "A");
        page.Find("[data-testid=clear]").Click();

        Assert.Empty(page.FindAll("[data-testid=step]"));
        Assert.NotEmpty(page.FindAll("[data-testid=signal-idle]"));
    }

    [Fact]
    public void FollowingAgainAfterClearingShowsNoStaleKeypress()
    {
        var page = Page();

        Follow(page);
        Type(page, "A");
        page.Find("[data-testid=watching]").Change(false);
        page.Find("[data-testid=clear]").Click();
        Follow(page);

        Assert.Empty(page.FindAll("[data-testid=step]"));
        Assert.NotEmpty(page.FindAll("[data-testid=signal-idle]"));
    }

    private static void Follow(IRenderedComponent<Home> page) =>
        page.Find("[data-testid=watching]").Change(true);

    [Theory]
    // The panel is labelled as the machine was; the English is there to be read
    // alongside, not instead.
    [InlineData("Stromweg", "the path of the current")]
    [InlineData("Spruch", "the message")]
    [InlineData("Klartext", "plain text")]
    [InlineData("Geheimtext", "cipher text")]
    [InlineData("Schlüsseltafel", "the key sheet")]
    [InlineData("Walzenlage", "wheel order")]
    [InlineData("Ringstellung", "ring settings")]
    [InlineData("Grundstellung", "starting positions")]
    [InlineData("Umkehrwalze", "reflector")]
    [InlineData("Eintrittswalze", "entry wheel")]
    [InlineData("Steckerbrett", "the plugboard")]
    [InlineData("Spruchschlüssel", "the message key")]
    public void EveryGermanTermIsGlossedInEnglish(string german, string english)
    {
        var text = Page().Markup;

        Assert.Contains(german, text);
        Assert.Contains(english, text);
    }

    // -- the indicator procedure ------------------------------------------------

    [Fact]
    public void WithNoProcedureTheRotorsStartWhereTheKeySheetSays()
    {
        var page = Page();

        page.Find("[data-testid=sheet]").Change("barbarossa");

        Assert.Equal("BLA", Windows(page));
        Assert.Empty(page.FindAll("[data-testid=worked]"));
    }

    [Fact]
    public void SendingStartsTheRotorsAtTheMessageKey()
    {
        // The key sheet's positions become the ground setting, and the message is
        // enciphered somewhere else entirely.
        var page = Page();

        page.Find("[data-testid=sheet]").Change("barbarossa");
        Send(page, "QWE");

        Assert.Equal("QWE", Windows(page));
    }

    [Fact]
    public void SendingReportsTheIndicatorToTransmit()
    {
        var page = Page();

        page.Find("[data-testid=sheet]").Change("barbarossa");
        Send(page, "QWE");

        var worked = page.Find("[data-testid=worked]").TextContent;

        Assert.Contains("BLA", worked);
        Assert.Contains("QWE", worked);
        Assert.Contains(Indicator("QWE"), worked);
    }

    [Fact]
    public void ReceivingWorksTheMessageKeyBack()
    {
        var page = Page();

        page.Find("[data-testid=sheet]").Change("barbarossa");
        page.Find("[data-testid=procedure]").Change(nameof(IndicatorMode.Receiving));
        page.Find("[data-testid=indicator-sent]").Change(Indicator("QWE"));

        Assert.Equal("QWE", Windows(page));
        Assert.Contains("QWE", page.Find("[data-testid=worked]").TextContent);
    }

    [Fact]
    public void WhatOneStationSendsAnotherCanRead()
    {
        // The procedure end to end, which is the only thing that really matters.
        var sender = Page();
        var receiver = Page();

        sender.Find("[data-testid=sheet]").Change("barbarossa");
        Send(sender, "QWE");
        sender.Find("[data-testid=plaintext]").Change("ATTACKATDAWN");

        var indicator = Indicator("QWE");
        var cipher = Ciphertext(sender);

        receiver.Find("[data-testid=sheet]").Change("barbarossa");
        receiver.Find("[data-testid=procedure]").Change(nameof(IndicatorMode.Receiving));
        receiver.Find("[data-testid=indicator-sent]").Change(indicator);
        receiver.Find("[data-testid=plaintext]").Change(cipher);

        Assert.Equal("ATTACKATDAWN", Ciphertext(receiver));
    }

    [Fact]
    public void AnIndicatorSentTwiceIsTwiceAsLong()
    {
        var page = Page();

        page.Find("[data-testid=sheet]").Change("barbarossa");
        Send(page, "QWE");
        page.Find("[data-testid=doubled]").Change(true);

        Assert.Contains(Indicator("QWE", doubled: true), page.Find("[data-testid=worked]").TextContent);
    }

    [Fact]
    public void AMessageKeyThatWillNotWorkIsReportedAndLeavesTheRotorsAlone()
    {
        var page = Page();

        page.Find("[data-testid=sheet]").Change("barbarossa");
        Send(page, "QW");

        Assert.NotEmpty(page.FindAll("[data-testid=indicator-error]"));
        Assert.Empty(page.FindAll("[data-testid=worked]"));
        Assert.Equal("BLA", Windows(page));
    }

    [Fact]
    public void AGarbledDoubledIndicatorIsReported()
    {
        // What the doubling was for: the halves disagree, so it shows itself.
        var page = Page();

        page.Find("[data-testid=sheet]").Change("barbarossa");
        page.Find("[data-testid=procedure]").Change(nameof(IndicatorMode.Receiving));
        page.Find("[data-testid=indicator-sent]").Change("ABCDEF");

        Assert.Contains("intact", page.Find("[data-testid=indicator-error]").TextContent);
    }

    [Fact]
    public void GivingUpTheProcedurePutsTheRotorsBackToTheGroundSetting()
    {
        var page = Page();

        page.Find("[data-testid=sheet]").Change("barbarossa");
        Send(page, "QWE");
        page.Find("[data-testid=procedure]").Change(nameof(IndicatorMode.None));

        Assert.Equal("BLA", Windows(page));
        Assert.Empty(page.FindAll("[data-testid=worked]"));
    }

    [Fact]
    public void TheMessageKeyIsOfferedOnlyWhenSending()
    {
        var page = Page();

        Assert.Empty(page.FindAll("[data-testid=message-key]"));
        Assert.Empty(page.FindAll("[data-testid=indicator-sent]"));

        page.Find("[data-testid=procedure]").Change(nameof(IndicatorMode.Sending));

        Assert.NotEmpty(page.FindAll("[data-testid=message-key]"));
        Assert.Empty(page.FindAll("[data-testid=indicator-sent]"));

        page.Find("[data-testid=procedure]").Change(nameof(IndicatorMode.Receiving));

        Assert.Empty(page.FindAll("[data-testid=message-key]"));
        Assert.NotEmpty(page.FindAll("[data-testid=indicator-sent]"));
    }

    [Fact]
    public void ChangingTheGroundSettingChangesTheIndicator()
    {
        // The indicator is the message key enciphered at the ground setting, so it
        // is a property of both.
        var page = Page();

        page.Find("[data-testid=sheet]").Change("barbarossa");
        Send(page, "QWE");

        var before = page.Find("[data-testid=worked]").TextContent;

        page.Find("[data-testid=positions]").Change("XYZ");

        Assert.NotEqual(before, page.Find("[data-testid=worked]").TextContent);
        Assert.Equal("QWE", Windows(page));
    }

    private static void Send(IRenderedComponent<Home> page, string messageKey)
    {
        page.Find("[data-testid=procedure]").Change(nameof(IndicatorMode.Sending));
        page.Find("[data-testid=message-key]").Change(messageKey);
    }

    private static string Indicator(string messageKey, bool doubled = false)
    {
        Assert.True(new KeySheetCatalogue().TryGet("barbarossa", out var sheet));

        var procedure = new ServiceCollection().AddEnigmaServices().BuildServiceProvider()
            .GetRequiredService<IIndicatorProcedure>();

        return procedure.EncipherMessageKey(sheet, sheet.Positions, messageKey, doubled);
    }

    // -- settings ---------------------------------------------------------------

    [Fact]
    public void ASettingTheMachineCannotBeBuiltFromLeavesTheLastWorkingOneInPlace()
    {
        // Mid-way through setting up is not the same as holding a broken machine.
        var page = Page();

        page.Find("[data-testid=rings]").Change("NOTASETTING");

        Assert.NotEmpty(page.FindAll("[data-testid=error]"));
        Assert.NotEmpty(page.FindAll("[data-testid=panel]"));
        Assert.Equal("AAA", Windows(page));
    }

    [Fact]
    public void TheErrorClearsOnceTheSettingsWorkAgain()
    {
        var page = Page();

        page.Find("[data-testid=rings]").Change("NOTASETTING");
        page.Find("[data-testid=rings]").Change("BUL");

        Assert.Empty(page.FindAll("[data-testid=error]"));
    }

    [Fact]
    public void AKeySheetTheMachineCannotBeBuiltFromShowsAMessage()
    {
        var page = Page(new KeySheetCatalogue([new("default", Broken)]));

        Assert.Contains("NOSUCHWHEEL", page.Find("[data-testid=error]").TextContent);
        Assert.Empty(page.FindAll("[data-testid=panel]"));
    }

    // -- the board --------------------------------------------------------------

    [Fact]
    public void AMachineWithNoPlugboardIsOfferedNone()
    {
        var page = Page(new KeySheetCatalogue([new("g31", Zaehlwerk)]));

        Assert.NotEmpty(page.FindAll("[data-testid=stecker]"));

        page.Find("[data-testid=sheet]").Change("g31");

        Assert.Empty(page.FindAll("[data-testid=stecker]"));
    }

    [Fact]
    public void CablingTwoLettersPatchesTheMachine()
    {
        var page = Page();

        page.Find("[data-testid=jack][data-letter=A]").Click();
        page.Find("[data-testid=jack][data-letter=V]").Click();

        Assert.Equal("AV", page.Find("[data-testid=cables]").TextContent.Trim());
        Assert.Contains("jack-cabled", page.Find("[data-testid=jack][data-letter=A]").ClassName);
    }

    [Fact]
    public void PullingACableOutUnpatchesTheMachine()
    {
        var page = Page();

        page.Find("[data-testid=sheet]").Change("barbarossa");

        Assert.Contains("AV", page.Find("[data-testid=cables]").TextContent);

        page.Find("[data-testid=jack][data-letter=A]").Click();

        Assert.DoesNotContain("AV", page.Find("[data-testid=cables]").TextContent);
    }

    [Fact]
    public void CablingChangesTheCipher()
    {
        var page = Page();

        page.Find("[data-testid=plaintext]").Change("AAAAA");

        var plain = Ciphertext(page);

        page.Find("[data-testid=jack][data-letter=A]").Click();
        page.Find("[data-testid=jack][data-letter=V]").Click();

        Assert.NotEqual(plain, Ciphertext(page));
    }

    // -- helpers ----------------------------------------------------------------

    private static void Type(IRenderedComponent<Home> page, string text)
    {
        foreach (var key in text)
        {
            page.Find($"[data-testid=key][data-letter={key}]").MouseDown();
            page.Find($"[data-testid=key][data-letter={key}]").MouseUp();
        }
    }

    private static string Plaintext(IRenderedComponent<Home> page) =>
        page.Find("[data-testid=plaintext]").GetAttribute("value") ?? string.Empty;

    private static string Ciphertext(IRenderedComponent<Home> page) =>
        page.Find("[data-testid=ciphertext]").TextContent;

    private static string Windows(IRenderedComponent<Home> page) =>
        string.Concat(page.FindAll("[data-testid=window] .window-letter")
            .Select(window => window.TextContent));

    private static EnigmaSession Session(string name)
    {
        Assert.True(new KeySheetCatalogue().TryGet(name, out var sheet));

        var result = EnigmaSession.Open(
            new ServiceCollection().AddEnigmaServices().BuildServiceProvider()
                .GetRequiredService<IEnigmaMachineFactory>(),
            sheet);

        return result.Session!;
    }

    private IRenderedComponent<Home> Page(IKeySheetCatalogue? catalogue = null)
    {
        _catalogue = catalogue ?? _catalogue;

        return Render<Home>();
    }

    private static readonly KeySheet Zaehlwerk = new()
    {
        Name = "Zählwerk G-31",
        Model = "G-31",
        Reflector = "G",
        Rotors = "G-I G-II G-III",
        RingSettings = "AAA",
        Positions = "AAA",
        ReflectorPosition = "A",
        ReflectorRingSetting = "A"
    };

    private static readonly KeySheet Broken = new()
    {
        Name = "Broken",
        Reflector = "B",
        Rotors = "I II NOSUCHWHEEL",
        RingSettings = "AAA",
        Positions = "AAA"
    };
}
