using System;

namespace ABPCodeGenerator.Services
{
    public interface IService
    {
    }

    public interface IGenerateService<T>
    {

    }

    public class GenerateService<T> : IGenerateService<T>
    {
        public T Data { get; set; }

        public GenerateService(T Data)
        {
            this.Data = Data;
        }
    }


    public interface IOrderService
    {

    }

    public class OrderService : IOrderService, IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine($"{this.GetHashCode()} Is Disposabled");
        }
    }
}
