using EndangerEd.Game.Objects;

namespace EndangerEd.Game.Screens.Games;

/// <summary>
/// Big class for micro game screen.
/// </summary>
public partial class MicroGameScreen(Question question) : EndangerEdScreen
{
    public Question CurrentQuestion { get; set; } = question;
}
