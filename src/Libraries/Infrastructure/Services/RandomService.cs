using System;
using System.Linq;
using Application.Common.Interfaces;

namespace Infrastructure.Services
{
    public class RandomService : IRandomService
    {
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private Random random;

        public RandomService()
        {
            random = new Random();
        }
        public string RandomAlphanumericString(int length)
        {
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
