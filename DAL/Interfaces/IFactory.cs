namespace DAL.Interfaces
{
    public interface IFactory<T>
    {
        public T Get();
    }
}
