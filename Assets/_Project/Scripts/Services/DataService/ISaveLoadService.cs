namespace TodoBoard
{
    public interface ISaveLoadService
    {
        void SaveData<T>(T data, string fileName);
        T LoadData<T>(string fileName);
    }
}