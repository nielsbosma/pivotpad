namespace PivotPad;

public interface IPivotRenderer
{
    public string Render(PivotReport report, string csvHost);
}