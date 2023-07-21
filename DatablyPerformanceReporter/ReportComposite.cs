using System.Timers;

namespace DatablyPerformanceReporter;

public class ReportComposite
{
  
    public ReportComposite(ReportComposite? parent, string description)
    {
        Parent = parent;
        Description = description;
        Elapsed = TimeSpan.Zero;
    }

    public void SetElapsed(TimeSpan elapsed)
    {
        Elapsed = elapsed;
    }
    public void AddChild(ReportComposite child)
    {
        child.Parent = this;
        Children = Children.Append(child).ToArray();
    }
    public ReportComposite? Parent { get; set; }
    public string Description { get; }
    public TimeSpan Elapsed { get; private set; }

    public decimal TimeSlicePercentage => CalculateTimeSlicePercentage();

    private decimal CalculateTimeSlicePercentage()
    {
        if (Parent is null)
        {
            return 100M;
        }

        if (Parent.Elapsed.TotalMilliseconds <= 0)
        {
            return 0;
        }
        return (decimal)(Elapsed.TotalMilliseconds / Parent.Elapsed.TotalMilliseconds * 100);
    }

    public IEnumerable<ReportComposite> Children { get; private set; } 
        = Array.Empty<ReportComposite>();

    public string GenerateReport()
    {
        var result = Environment.NewLine + FormatRow(Description, Elapsed, TimeSlicePercentage);
        foreach (var c in Children)
        {
            result += Environment.NewLine+ "-" + c.GenerateReport();
        }
        /*
         * --------------------------------------------------
         * - Request 1                1000 ms.    23 %   
         * - Request 2                1850  ms.   15 %   
         * -- Task 1                   850  ms.   15 %    
         * -- Task 2                   850  ms.   15 %  
         * __________________________________________________
         * Request 3                  900 ms.     13 %
         * --------------------------------------------------
         */
        return result;
    }


    private string FormatRow(
        string description, TimeSpan elapsed, decimal percentage)
    {
        var d = description.PadRight(90);
        var e = (elapsed.TotalMilliseconds.ToString() + " ms").PadRight(10);
        var p = (percentage.ToString() + " %").PadRight(10);
        return d + "| " + e + "| " + p;
    }
}