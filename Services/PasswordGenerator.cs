using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BlazorWASMValidPasswordGenerator.Services
{
    public class PasswordGenerator
    {
        private static readonly string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string NumberChars = "0123456789";
        private static readonly string SpecialChars = "!@#$%^&*()_-+=[{]};:'\",<.>/?";

        // Blacklist of common character strings to avoid.
        private static readonly List<string> Blacklist = new()
        {
            "1234",
            "aaaa",
            "würth", // Add other company-related terms here
            // ... add other blacklisted strings ...
        };

        // Example of forbidden patterns (can be expanded)
        private static readonly List<string> ForbiddenPatterns = new()
        {
            @"herbst\d{4}", // Example: herbst2018
            @"PW4WGS\d{4}"  // Example: PW4WGS1234
            // ... add more forbidden patterns as regex strings
        };

        public static string GenerateValidPassword(bool isPrivileged = false)
        {
            int minLength = isPrivileged ? 15 : 12;
            string password;

            do
            {
                password = GeneratePassword(minLength);
            } while (!IsValidPassword(password, isPrivileged));

            return password;
        }

        private static string GeneratePassword(int minLength)
        {
            StringBuilder passwordBuilder = new();
            RandomNumberGenerator rng = RandomNumberGenerator.Create();

            // Ensure at least one character from each required category
            passwordBuilder.Append(GetRandomChar(UppercaseChars, rng));
            passwordBuilder.Append(GetRandomChar(LowercaseChars, rng));
            passwordBuilder.Append(GetRandomChar(NumberChars, rng));
            passwordBuilder.Append(GetRandomChar(SpecialChars, rng));

            // Fill the rest of the password with random characters
            for (int i = passwordBuilder.Length; i < minLength; i++)
            {
                string charSet = GetRandomCharSet(rng);
                passwordBuilder.Append(GetRandomChar(charSet, rng));
            }

            // Shuffle the password to mix the characters
            return ShuffleString(passwordBuilder.ToString(), rng);
        }

        private static bool IsValidPassword(string password, bool isPrivileged)
        {
            int minLength = isPrivileged ? 15 : 12;

            // Check length
            if (password.Length < minLength) return false;

            // Check character categories
            int categories = 0;
            if (password.Any(char.IsUpper)) categories++;
            if (password.Any(char.IsLower)) categories++;
            if (password.Any(char.IsDigit)) categories++;
            if (password.IndexOfAny(SpecialChars.ToCharArray()) >= 0) categories++;
            if (categories < 3) return false;

            // Check blacklist
            if (Blacklist.Any(blacklisted => password.Contains(blacklisted, StringComparison.OrdinalIgnoreCase))) return false;

            // Check forbidden patterns
            foreach (var pattern in ForbiddenPatterns)
            {
                if (Regex.IsMatch(password, pattern, RegexOptions.IgnoreCase))
                {
                    return false;
                }
            }

            // Check for personal information (this is a basic example and would need to be expanded)
            // You would ideally have this information provided as arguments to the method.
            // Consider using libraries like Levenshtein distance for fuzzy matching.
            // Example with hardcoded values (replace with actual user data):
            string[] forbiddenStrings = { "JohnDoe", "CompanyName", "19900101", "1234567890" };
            if (forbiddenStrings.Any(s => password.Contains(s, StringComparison.OrdinalIgnoreCase))) return false;

            return true;
        }

        private static string GetRandomCharSet(RandomNumberGenerator rng)
        {
            byte[] randomBytes = new byte[1];
            rng.GetBytes(randomBytes);
            int randomValue = randomBytes[0] % 4;

            switch (randomValue)
            {
                case 0: return UppercaseChars;
                case 1: return LowercaseChars;
                case 2: return NumberChars;
                default: return SpecialChars;
            }
        }

        private static char GetRandomChar(string charSet, RandomNumberGenerator rng)
        {
            byte[] randomBytes = new byte[4];
            rng.GetBytes(randomBytes);
            uint randomIndex = BitConverter.ToUInt32(randomBytes, 0) % (uint)charSet.Length;
            return charSet[(int)randomIndex];
        }

        private static string ShuffleString(string str, RandomNumberGenerator rng)
        {
            char[] array = str.ToCharArray();
            int n = array.Length;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do rng.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                char value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return new string(array);
        }
    }
}
