using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABPCodeGenerator.Interceptors
{
    public class MyInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"Before:{invocation.Method.Name}");
            invocation.Proceed();
            Console.WriteLine($"After:{invocation.Method.Name}");
        }
    }
}
