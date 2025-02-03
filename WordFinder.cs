using System.Text;

namespace WordFinder
{
    /// <summary>
    /// The WordFinder class preprocesses a character matrix to extract all possible horizontal and vertical substrings,
    /// storing their frequencies in a dictionary. It then provides a method to search for words in the matrix efficiently.
    /// </summary>
    public class WordFinder
    {
        // Array storing the matrix for indexed access and consistency.
        private readonly string[] _matrix;

        // Dictionary to store each substring and its total frequency in the matrix.
        private readonly Dictionary<string, int> _substringsFrequency;

        /// <summary>
        /// Constructor that receives the matrix as IEnumerable<string>.
        /// It converts the sequence to an array and pre-processes it to extract all horizontal and vertical substrings.
        /// </summary>
        /// <param name="matrix">Sequence of strings representing the rows of the matrix.</param>
        public WordFinder(IEnumerable<string> matrix)
        {
            // Materialize the matrix to guarantee indexed access and avoid re-evaluation.
            _matrix = matrix.ToArray();

            if (_matrix.Length > 0)
            {
                int expectedLength = _matrix[0].Length;
                foreach (var row in _matrix)
                {
                    if (row.Length != expectedLength)
                    {
                        throw new ArgumentException("All rows in the matrix must have the same length.", nameof(matrix));
                    }
                }
            }

            // Initialize the dictionary.
            _substringsFrequency = new Dictionary<string, int>();

            // Preprocess horizontal substrings.
            PreprocessHorizontal();

            // Preprocess vertical substrings.
            PreprocessVertical();
        }

        /// <summary>
        /// Preprocess each row of the matrix by generating all possible substrings (left-to-right)
        /// and updating their frequency in the dictionary.
        /// </summary>
        private void PreprocessHorizontal()
        {
            foreach (var row in _matrix)
            {
                int length = row.Length;
                // Generate all possible substrings in the row.
                for (int start = 0; start < length; start++)
                {
                    for (int end = start; end < length; end++)
                    {
                        // Get the substring from 'start' to 'end' (inclusive).
                        string sub = row.Substring(start, end - start + 1);
                        // Increment its frequency in the dictionary.
                        if (_substringsFrequency.TryGetValue(sub, out int count))
                        {
                            _substringsFrequency[sub] = count + 1;
                        }
                        else
                        {
                            _substringsFrequency[sub] = 1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Preprocess each column of the matrix by generating all possible substrings (top-to-bottom)
        /// and updating their frequency in the dictionary.
        /// </summary>
        private void PreprocessVertical()
        {
            if (_matrix.Length == 0) return;

            int numRows = _matrix.Length;
            int numCols = _matrix[0].Length;

            for (int col = 0; col < numCols; col++)
            {
                // Build the vertical string corresponding to column 'col'.
                StringBuilder sb = new StringBuilder();
                for (int row = 0; row < numRows; row++)
                {
                    sb.Append(_matrix[row][col]);
                }
                string columnString = sb.ToString();

                int len = columnString.Length;
                // Generate all possible substrings in the vertical string.
                for (int start = 0; start < len; start++)
                {
                    for (int end = start; end < len; end++)
                    {
                        string sub = columnString.Substring(start, end - start + 1);
                        if (_substringsFrequency.TryGetValue(sub, out int count))
                        {
                            _substringsFrequency[sub] = count + 1;
                        }
                        else
                        {
                            _substringsFrequency[sub] = 1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Searches for the words from the word stream in the matrix.
        /// Only unique words from the stream are considered, and up to 10 words found are returned,
        /// ordered by descending frequency (and lexicographically in case of ties).
        /// </summary>
        /// <param name="wordstream">Sequence of words to search for.</param>
        /// <returns>Collection of found words, ordered by descending frequency.</returns>
        public IEnumerable<string> Find(IEnumerable<string> wordstream)
        {
            // Remove duplicates: each word is processed only once.
            var uniqueWords = new HashSet<string>(wordstream);

            // List to store the found words along with their frequency in the matrix.
            List<(string word, int frequency)> foundWords = new List<(string, int)>();

            foreach (var word in uniqueWords)
            {
                if (_substringsFrequency.TryGetValue(word, out int frequency))
                {
                    foundWords.Add((word, frequency));
                }
            }

            // Sort the list of found words:
            // Primary criterion: frequency in descending order (i.e., most occurrences first).
            // Secondary criterion: word in ascending alphabetical order (used as a tie-breaker).
            var topWords = foundWords
                .OrderByDescending(pair => pair.frequency)
                .ThenBy(pair => pair.word)
                .Take(10)
                .Select(pair => pair.word)
                .ToList();

            return topWords;
        }
    }
}
