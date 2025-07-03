public class ChoiceUpdateDto
{
    public Guid ChoiceId { get; set; }
    public string ChoiceText { get; set; } = null!;
    public bool IsCorrect { get; set; }
}