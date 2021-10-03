public interface IDataIndex<T> {

    T GetData(string key);
    T GetDataOrNull(string tag);
}
