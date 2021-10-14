using System;
using System.Threading.Tasks;
using IntegreNet.Exceptions;

namespace IntegreNet
{
    internal static class Try
    {
        internal static async Task TryHandle(Func<Task> work)
        {
            try
            {
                await work();
            }
            catch (IntegreException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new IntegreException("Unexpected error occurred", e);
            }
        }

        internal static async Task<T> TryHandle<T>(Func<Task<T>> work)
        {
            try
            {
                return await work();
            }
            catch (IntegreException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new IntegreException("Unexpected error occurred", e);
            }
        }
    }
}