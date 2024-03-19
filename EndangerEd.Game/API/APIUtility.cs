using EndangerEd.Game.Objects;

namespace EndangerEd.Game.API;

public class APIUtility
{
    public static QuestionMode ConvertToQuestionMode(string questionMode)
    {
        return questionMode switch
        {
            "Four Choice" => QuestionMode.FourChoice,
            "Cannon" => QuestionMode.Cannon,
            "Bucket" => QuestionMode.Bucket,
            "Take Picture" => QuestionMode.TakePicture,
            _ => QuestionMode.FourChoice
        };
    }
}
