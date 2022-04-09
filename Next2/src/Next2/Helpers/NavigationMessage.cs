using Prism.Navigation;

namespace Next2.Helpers
{
    public class NavigationMessage
    {
        public NavigationMessage(string path, INavigationParameters parameters)
        {
            Path = path;
            Parameters = parameters;
        }

        public string Path { get; }

        public INavigationParameters Parameters { get; }
    }
}
