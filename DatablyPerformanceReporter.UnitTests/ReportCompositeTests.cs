namespace DatablyPerformanceReporter.UnitTests;

public class ReportCompositeTests
{
    private ReportComposite CreateReportCompositeLeaf(
        string description, int ellapsedMs)
    {
        var ellapsed = TimeSpan.FromMilliseconds(ellapsedMs);

        return new(null, description, ellapsed);
    }

    private ReportComposite CreateReportCompositeLeaf(int ellapsedMs)
    {
        var ellapsed = TimeSpan.FromMilliseconds(ellapsedMs);

        return new(null, Guid.NewGuid().ToString(), ellapsed);
    }

    [Fact]
    public void Constructor_SetRootData()
    {
        // ************ ARRANGE ************
        
        // ************ ACT ************

        var sut = CreateReportCompositeLeaf("some request", 500);
        
        // ************ ASSERT ************
        
        Assert.Equal("some request", sut.Description);
        Assert.Equal(500, sut.Elapsed.TotalMilliseconds);
    }
    
    [Fact]
    public void CanAddChild_SettingChildParent()
    {
        // ************ ARRANGE ************

        var parent = CreateReportCompositeLeaf("some request", 100);

        var child = CreateReportCompositeLeaf("another request", 500);
        
        // ************ ACT ************

        parent.AddChild(child);
        
        // ************ ASSERT ************

        Assert.Contains(parent.Children, c =>c.Equals(child));
        Assert.Equal(parent, child.Parent);
    }


    [Fact]
    public void TimeSlicePercentage_NoParent_Returns100Percent()
    {
        // ************ ARRANGE ************

        var sut = CreateReportCompositeLeaf("some request", 500);

        // ************ ACT ************

        var result = sut.TimeSlicePercentage;

        // ************ ASSERT ************
        
        Assert.Equal(100M, result);
    }

    [Fact]
    public void TimeSlicePercentage_HasParent_ReturnsEllapsedPercengateRespectToParentEllapsed()
    {
        // ************ ARRANGE ************

        var parent = CreateReportCompositeLeaf("some request", 1000);

        var child = CreateReportCompositeLeaf("another request", 100);
        
        parent.AddChild(child);

        // ************ ACT ************

        var result = child.TimeSlicePercentage;

        // ************ ASSERT ************
        
        Assert.Equal(10M, result);
    }

    [Fact]
    public void GenerateReport_NoChildren_ReportsItself()
    {
        // ************ ARRANGE ************

        var sut = CreateReportCompositeLeaf(1000);

        // ************ ACT ************

        var result = sut.GenerateReport();

        // ************ ASSERT ************
        
        VerifyLeafDataInString(result, sut);
        
    }

    [Fact]
    public void GenerateReport_ReportsItself_Plus_EachChild()
    {
        // ************ ARRANGE ************
        
        var parent = CreateReportCompositeLeaf(1000);
        var child1 = CreateReportCompositeLeaf(400);
        var child2 = CreateReportCompositeLeaf(400);
        
        parent.AddChild(child1);
        parent.AddChild(child2);

        // ************ ACT ************
        
        var result = parent.GenerateReport();

        // ************ ASSERT ************
        
        VerifyLeafDataInString(result, child1);
        VerifyLeafDataInString(result, child2);
    }

    private void VerifyLeafDataInString(string searchString, ReportComposite leaf)
    {
        Assert.True(searchString.IndexOf(leaf.Description)>=0);
        var elapsed = ((decimal)leaf.Elapsed.TotalMilliseconds).ToString();
        Assert.True(searchString.IndexOf(elapsed)>=0);
        Assert.True(searchString.IndexOf(leaf.TimeSlicePercentage.ToString())>=0);
    }

}