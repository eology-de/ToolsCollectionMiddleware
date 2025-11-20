using System;
namespace eology.ToolsCollection
{
	public class StringHelpers
	{
		public StringHelpers()
		{
		}

        public static string TrimWhitespace(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input; // Gibt den ursprünglichen String zurück, wenn er null oder leer ist.
            }

            return input.Trim();
        }
    }
}

