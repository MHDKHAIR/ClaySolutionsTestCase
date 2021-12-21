using System;
using System.Linq;
using Domain.Interfaces.Services;

namespace Application.Services
{
    public class RandomService : IRandomService
    {
        private string chars;
        private Random random;

        public RandomService()
        {
            chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            random = new Random();
        }
        public string RandomAlphanumericString(int length)
        {
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
