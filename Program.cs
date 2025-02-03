using WordFinder;

namespace WordFinder
{
    class Program
    {
        static void Main(string[] args)
        {           
            var matrix = new List<string>
            {
                "lebron23",
                "michael ",  
                "kobexxxx",  
                "durant  "   
            };

            // Word stream
            var wordStream = new List<string> { "lebron", "kobe", "michael", "lebron", "warm" };

            WordFinder wordFinder = new WordFinder(matrix);

            // Find the words in the matrix
            IEnumerable<string> result = wordFinder.Find(wordStream);

            Console.WriteLine("Found words:");
            foreach (var word in result)
            {
                Console.WriteLine(word);
            }
        }
    }
}
