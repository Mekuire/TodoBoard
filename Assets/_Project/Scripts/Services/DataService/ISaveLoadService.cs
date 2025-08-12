namespace TodoBoard
{
    public interface ISaveLoadService
    {
        bool SaveData<T>(T data, string fileName);
        bool LoadData<T>(string fileName, out T data);
    }
}