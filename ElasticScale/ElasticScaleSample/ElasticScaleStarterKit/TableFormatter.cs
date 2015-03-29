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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElasticScaleStarterKit
{
    /// <summary>
    /// Stores tabular data and formats it for writing to output.
    /// </summary>
    internal class TableFormatter
    {
        /// <summary>
        /// Table column names.
        /// </summary>
        private readonly string[] columnNames;

        /// <summary>
        /// Table rows.
        /// </summary>
        private readonly List<string[]> rows;

        public TableFormatter(string[] columnNames)
        {
            this.columnNames = columnNames;
            this.rows = new List<string[]>();
        }

        public void AddRow(object[] values)
        {
            if (values.Length != this.columnNames.Length)
            {
                throw new ArgumentException(string.Format("Incorrect number of fields. Expected {0}, actual {1}", this.columnNames.Length, values.Length));
            }

            string[] valueStrings = values.Select(o => o.ToString()).ToArray();

            this.rows.Add(valueStrings);
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();

            // Determine column widths
            int[] columnWidths = new int[this.columnNames.Length];
            for (int c = 0; c < this.columnNames.Length; c++)
            {
                int maxValueLength = 0;

                if (this.rows.Any())
                {
                    maxValueLength = this.rows.Select(r => r[c].Length).Max();
                }

                columnWidths[c] = Math.Max(maxValueLength, this.columnNames[c].Length);
            }

            // Build format strings that are used to format the column names and fields
            string[] formatStrings = new string[this.columnNames.Length];
            for (int c = 0; c < this.columnNames.Length; c++)
            {
                formatStrings[c] = string.Format(" {{0,-{0}}} ", columnWidths[c]);
            }

            // Write header
            for (int c = 0; c < this.columnNames.Length; c++)
            {
                output.AppendFormat(formatStrings[c], this.columnNames[c]);
            }

            output.AppendLine();

            // Write separator
            for (int c = 0; c < this.columnNames.Length; c++)
            {
                output.AppendFormat(formatStrings[c], new string('-', this.columnNames[c].Length));
            }

            output.AppendLine();

            // Write rows
            foreach (string[] row in this.rows)
            {
                for (int c = 0; c < this.columnNames.Length; c++)
                {
                    output.AppendFormat(formatStrings[c], row[c]);
                }

                output.AppendLine();
            }

            return output.ToString();
        }
    }
}
