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
    public QuestionMode QuestionMode { get; set; }
}

public enum ContentType
{
    Text,
    Image
}

public enum QuestionMode
{
    FourChoice,
    Cannon,
    Bucket,
    TakePicture,
    Conveyor,
    Traffic
}
