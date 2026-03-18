namespace ShiftSync.Mobile.ViewModels;

public sealed class DayOption
{
    public int Value { get; set; }
    public string Label { get; set; } = string.Empty;

    public override string ToString() => Label;
}
