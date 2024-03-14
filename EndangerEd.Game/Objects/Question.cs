namespace EndangerEd.Game.Objects;

/// <summary>
/// Question using in the game.
/// </summary>
public class Question
{
    public string QuestionText { get; set; }
    public string Answer { get; set; }
    public string[] Choices { get; set; }
    public ContentType ContentType { get; set; }
    public string QuestionMode { get; set; }
}

public enum ContentType
{
    Text,
    Image
}
