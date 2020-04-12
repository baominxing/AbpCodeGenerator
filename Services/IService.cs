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
        void Show();
    }

    public class OrderService : IOrderService, IDisposable
    {

        public OrderNameService OrderNameService { get; set; }

        public void Dispose()
        {
            Console.WriteLine($"{this.GetHashCode()} Is Disposabled");
        }

        public virtual void Show()
        {
            Console.WriteLine($"{this.GetType().Name}.Show:{GetHashCode()},OrderNameService IS NULL?{OrderNameService == null}");
        }
    }

    public class OrderService2 : IOrderService, IDisposable
    {

        public OrderNameService OrderNameService { get; set; }

        public void Dispose()
        {
            Console.WriteLine($"{this.GetHashCode()} Is Disposabled");
        }

        public void Show()
        {
            Console.WriteLine($"{this.GetType().Name}.Show:{GetHashCode()},OrderNameService IS NULL?{OrderNameService == null}");
        }
    }

    public class OrderNameService : IOrderService
    {
        public void Show()
        {
            Console.WriteLine($"{this.GetType().Name}.Show:{GetHashCode()},OrderNameService IS NULL?{this == null}");
        }
    }
}
