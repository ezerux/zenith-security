namespace ZenithSecurity.UI
{
    /// <summary>Returns the full single-file HTML dashboard (CSS + JS inlined).</summary>
    public static class HtmlLoader
    {
        public static string GetDashboardHtml() => DashboardHtml.Content;
    }
}
