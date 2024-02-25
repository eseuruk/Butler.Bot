namespace Butler.Bot.Core;

public class ComponentFilter
{
    public string? FilterPathern { get; }

    public ComponentFilter(string? filterPathern)
    {
        FilterPathern = filterPathern;
    }

    public bool AcceptComponent(string componentId)
    {
        if(string.IsNullOrEmpty(FilterPathern)) return true;

        return FilterPathern.Equals(componentId, StringComparison.OrdinalIgnoreCase);
    }
}
