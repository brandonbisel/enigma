using Bunit;
using Enigma.App;
using Enigma.Extensions.DependencyInjection;
using Enigma.Web.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Web.Tests;

/// <summary>
/// The Kriegsmarine indicator procedure, driven through the panel rather than the
/// library. The message that pins it in the library pins it here too: if the panel
/// wires anything up wrongly, U-534's rotors land somewhere other than ODFF.
/// </summary>
public class NavalIndicatorPanelTests : BunitContext
{
    public NavalIndicatorPanelTests()
    {
        Services.AddEnigmaServices();
        Services.AddSingleton<IKeySheetCatalogue>(_ => new KeySheetCatalogue());
    }

    [Fact]
    public void U534sMessageIsReadRightThroughThePanel()
    {
        // P1030690, 1 May 1945. The indicator as transmitted, the table the
        // Tauschtafelplan names for that day, and the rotor positions the operator
        // wrote on his sheet.
        var page = Naval("u534", IndicatorMode.NavalReceiving);

        page.Find("[data-testid=kennziffer]").Change("6");
        page.Find("[data-testid=monatstag]").Change("1");
        page.Find("[data-testid=indicator-sent]").Change("FNHC GVET");

        Assert.Equal("ODFF", Windows(page));
    }

    [Fact]
    public void TheWorkingIsShownAsAnOperatorWouldWriteIt()
    {
        var page = Naval("u534", IndicatorMode.NavalReceiving);

        page.Find("[data-testid=kennziffer]").Change("6");
        page.Find("[data-testid=indicator-sent]").Change("FNHC GVET");

        var worked = page.Find("[data-testid=worked]").TextContent;

        Assert.Contains("DUZ", worked);   // Schlüsselkenngruppe
        Assert.Contains("YMU", worked);   // Verfahrenkenngruppe
        Assert.Contains("ODFF", worked);  // where the rotors go
    }

    [Fact]
    public void TheCalendarPicksTheTableSoTheOperatorDoesNot()
    {
        var page = Naval("u534", IndicatorMode.NavalReceiving);

        page.Find("[data-testid=kennziffer]").Change("6");
        page.Find("[data-testid=monatstag]").Change("1");

        Assert.Contains("Tafel A", page.Find("[data-testid=plan-says]").TextContent);
        Assert.True(page.Find("[data-testid=tafel]").HasAttribute("disabled"));
    }

    [Fact]
    public void AnotherDayIsAnotherTable()
    {
        var page = Naval("u534", IndicatorMode.NavalReceiving);

        page.Find("[data-testid=kennziffer]").Change("6");
        page.Find("[data-testid=monatstag]").Change("1");

        var first = page.Find("[data-testid=plan-says]").TextContent;

        page.Find("[data-testid=monatstag]").Change("2");

        Assert.NotEqual(first, page.Find("[data-testid=plan-says]").TextContent);
    }

    [Fact]
    public void ADayCallingForTheTableThatWasNeverRecoveredSaysSo()
    {
        // Kennziffer two, the seventh: the plan reads J, and Tafel J is in no
        // surviving source. An empty panel would be a lie; a message is the truth.
        var page = Naval("u534", IndicatorMode.NavalReceiving);

        page.Find("[data-testid=kennziffer]").Change("2");
        page.Find("[data-testid=monatstag]").Change("3");

        Assert.Contains("Tafel J", page.Find("[data-testid=tafel-missing]").TextContent);
        Assert.NotEmpty(page.FindAll("[data-testid=indicator-error]"));
    }

    [Fact]
    public void WithoutAKennzifferTheTableIsChosenByHand()
    {
        var page = Naval("u534", IndicatorMode.NavalReceiving);

        Assert.Empty(page.FindAll("[data-testid=plan-says]"));
        Assert.Empty(page.FindAll("[data-testid=monatstag]"));
        Assert.False(page.Find("[data-testid=tafel]").HasAttribute("disabled"));

        page.Find("[data-testid=tafel]").Change("A");
        page.Find("[data-testid=indicator-sent]").Change("FNHC GVET");

        Assert.Equal("ODFF", Windows(page));
    }

    [Fact]
    public void TheWrongTableGivesTheWrongRotors()
    {
        // The table is not decoration: reading a message under the wrong day's
        // table has to land somewhere else.
        var page = Naval("u534", IndicatorMode.NavalReceiving);

        page.Find("[data-testid=tafel]").Change("A");
        page.Find("[data-testid=indicator-sent]").Change("FNHC GVET");

        var right = Windows(page);

        page.Find("[data-testid=tafel]").Change("B");

        Assert.NotEqual(right, Windows(page));
    }

    [Fact]
    public void WhatOneStationSendsAnotherReads()
    {
        var sender = Naval("u534", IndicatorMode.NavalSending);

        sender.Find("[data-testid=tafel]").Change("A");
        sender.Find("[data-testid=key-group]").Change("DUZ");
        sender.Find("[data-testid=message-group]").Change("YMU");
        sender.Find("[data-testid=first-filler]").Change("K");
        sender.Find("[data-testid=last-filler]").Change("Z");

        var sent = sender.Find("[data-testid=worked]").TextContent;
        var rotors = Windows(sender);

        // The trigrams and fillers of the real message, so this is the transmitted
        // indicator itself rather than any eight letters that round-trip.
        Assert.Contains("FNHCGVET", sent.Replace(" ", string.Empty));

        var receiver = Naval("u534", IndicatorMode.NavalReceiving);

        receiver.Find("[data-testid=tafel]").Change("A");
        receiver.Find("[data-testid=indicator-sent]").Change("FNHCGVET");

        Assert.Equal(rotors, Windows(receiver));
    }

    [Fact]
    public void TheNavalFieldsAppearOnlyForTheNavalProcedure()
    {
        var page = Page();

        Assert.Empty(page.FindAll("[data-testid=tafel-picker]"));

        page.Find("[data-testid=procedure]").Change(nameof(IndicatorMode.Sending));

        Assert.Empty(page.FindAll("[data-testid=tafel-picker]"));
        Assert.Empty(page.FindAll("[data-testid=key-group]"));

        page.Find("[data-testid=procedure]").Change(nameof(IndicatorMode.NavalSending));

        Assert.NotEmpty(page.FindAll("[data-testid=tafel-picker]"));
        Assert.NotEmpty(page.FindAll("[data-testid=key-group]"));
        Assert.NotEmpty(page.FindAll("[data-testid=message-group]"));

        // Receiving is given eight letters, not two trigrams to choose.
        page.Find("[data-testid=procedure]").Change(nameof(IndicatorMode.NavalReceiving));

        Assert.Empty(page.FindAll("[data-testid=key-group]"));
        Assert.NotEmpty(page.FindAll("[data-testid=indicator-sent]"));
    }

    [Fact]
    public void AnIndicatorThatIsNotEightLettersIsReportedNotThrown()
    {
        var page = Naval("u534", IndicatorMode.NavalReceiving);

        page.Find("[data-testid=tafel]").Change("A");
        page.Find("[data-testid=indicator-sent]").Change("FNHC");

        Assert.NotEmpty(page.FindAll("[data-testid=indicator-error]"));

        // And the machine is still there to correct it on.
        Assert.NotEmpty(page.FindAll("[data-testid=panel]"));
    }

    [Fact]
    public void LeavingTheProcedureReturnsTheRotorsToTheGroundSetting()
    {
        var page = Naval("u534", IndicatorMode.NavalReceiving);

        page.Find("[data-testid=tafel]").Change("A");
        page.Find("[data-testid=indicator-sent]").Change("FNHCGVET");

        Assert.Equal("ODFF", Windows(page));

        page.Find("[data-testid=procedure]").Change(nameof(IndicatorMode.None));

        Assert.Equal("IBFK", Windows(page));
    }

    private IRenderedComponent<Home> Naval(string sheet, IndicatorMode mode)
    {
        var page = Page();

        page.Find("[data-testid=sheet]").Change(sheet);
        page.Find("[data-testid=procedure]").Change(mode.ToString());

        return page;
    }

    private IRenderedComponent<Home> Page() => Render<Home>();

    private static string Windows(IRenderedComponent<Home> page) =>
        string.Concat(page.FindAll("[data-testid=window] .window-letter")
            .Select(window => window.TextContent));
}
