namespace web_tour.Filters
{
    public class FilterConfig
    {
        public static readonly HashSet<string> IgnoredControllers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ClientInfoController",
            "EmployeeController",
            "FileManagerController",
            "FileSystemController",
            "DocumentsController",
        };
    }
}