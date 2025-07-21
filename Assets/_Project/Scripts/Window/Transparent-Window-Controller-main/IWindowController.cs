namespace Desdinova
{
    public interface IWindowController
    {
        public void ChangeMonitor();
        public void SetTransparent(bool enable);
        public void SetOpacity(int opacity);
        public void SetAlwaysOnTop(bool enable);
    }
}