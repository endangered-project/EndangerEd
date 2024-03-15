using EndangerEd.Game.Objects;

namespace EndangerEd.Game.API;

public class APIUtility
{
    public static QuestionMode ConvertToQuestionMode(string questionMode)
    {
        return questionMode switch
        {
            "FourChoice" => QuestionMode.FourChoice,
            _ => QuestionMode.FourChoice
        };
    }
}
