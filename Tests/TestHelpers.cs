using DiffLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tests
{
    public static class TestHelpers
    {
        public static void AreEqualWithDiff(string actualValue, string expectedValue)
        {
            actualValue = Regex.Replace(actualValue.Trim(), @"[\s]{2,}", "");
            expectedValue = Regex.Replace(expectedValue.Trim(), @"[\s]{2,}", "");

            if (actualValue != expectedValue)
            {
                var sections = Diff.CalculateSections(expectedValue.ToCharArray(), actualValue.ToCharArray());

                int i1 = 0;
                int i2 = 0;
                foreach (var section in sections)
                {
                    if (section.IsMatch)
                    {
                        string same = expectedValue.Substring(i1, section.LengthInCollection1).Trim();
                        if (!string.IsNullOrEmpty(same))
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine(EllipsisString(same));
                        }
                    }
                    else
                    {
                        string added = expectedValue.Substring(i1, section.LengthInCollection1);
                        if (!string.IsNullOrEmpty(added))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("+" + WhiteSpaceToHex(added));
                        }

                        string removed = actualValue.Substring(i2, section.LengthInCollection2);
                        if (!string.IsNullOrEmpty(removed))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("-" + WhiteSpaceToHex(removed));
                        }

                        Console.WriteLine();
                    }

                    i1 += section.LengthInCollection1;
                    i2 += section.LengthInCollection2;
                }

                throw new Exception("String is not the same as expected!");
            }
        }

        private static string WhiteSpaceToHex(string input)
        {
            return string.Join("", input.Select(c => {
                if (Char.IsWhiteSpace(c))
                    return "\\x" + ((int)c).ToString("X2");
                else
                    return c.ToString();
            }));
        }

        private static string EllipsisString(string input, int maxLength = 100)
        {
            if (input.Length <= maxLength)
            {
                return input;
            }

            var partLen = maxLength / 2;
            int index;
            var part1 = input.Substring(0, partLen);
            if ((index = part1.LastIndexOf(" ")) > -1)
                part1 = part1.Substring(0, index);

            var part2 = input.Substring(input.Length-partLen, partLen);
            if ((index = part2.IndexOf(" ")) > -1)
                part2 = part2.Substring(index+1);

            return part1 + " ... " + part2;
        }
    }
}
