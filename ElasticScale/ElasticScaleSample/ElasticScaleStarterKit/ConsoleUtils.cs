/*
    Copyright 2014 Microsoft, Corp.

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using System;

namespace ElasticScaleStarterKit
{
    internal static class ConsoleUtils
    {
        /// <summary>
        /// Writes detailed information to the console.
        /// </summary>
        public static void WriteInfo(string format, params object[] args)
        {
            WriteColor(ConsoleColor.DarkGray, "\t" + format, args);
        }

        /// <summary>
        /// Writes warning text to the console.
        /// </summary>
        public static void WriteWarning(string format, params object[] args)
        {
            WriteColor(ConsoleColor.Yellow, format, args);
        }

        /// <summary>
        /// Writes colored text to the console.
        /// </summary>
        public static void WriteColor(ConsoleColor color, string format, params object[] args)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(format, args);
            Console.ForegroundColor = oldColor;
        }

        /// <summary>
        /// Reads an integer from the console.
        /// </summary>
        public static int ReadIntegerInput(string prompt)
        {
            return ReadIntegerInput(prompt, allowNull: false).Value;
        }

        /// <summary>
        /// Reads an integer from the console, or returns null if the user enters nothing and allowNull is true.
        /// </summary>
        public static int? ReadIntegerInput(string prompt, bool allowNull)
        {
            while (true)
            {
                Console.Write(prompt);
                string line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line) && allowNull)
                {
                    return null;
                }

                int inputValue;
                if (int.TryParse(line, out inputValue))
                {
                    return inputValue;
                }
            }
        }

        /// <summary>
        /// Reads an integer from the console.
        /// </summary>
        public static int ReadIntegerInput(string prompt, int defaultValue, Func<int, bool> validator)
        {
            while (true)
            {
                int? input = ReadIntegerInput(prompt, allowNull: true);

                if (!input.HasValue)
                {
                    // No input, so return default
                    return defaultValue;
                }
                else
                {
                    // Input was provided, so validate it
                    if (validator(input.Value))
                    {
                        // Validation passed, so return
                        return input.Value;
                    }
                }
            }
        }
    }
}
