using DonationHub.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DonationHub.Infrastructure.Identity
{
    public sealed class TemporaryPasswordGenerator : ITemporaryPasswordGenerator
    {
        private const int PasswordLength = 12;

        // Character groups required by Identity password rules.:
        private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private const string Digits = "0123456789";
        private const string SpecialCharacters = "!@#$%^&*()-_=+";

        // All available characters used to fill the remaining positions.
        private const string AllCharacters =
        Uppercase + Lowercase + Digits + SpecialCharacters;
        public string Generate()
        {
            // Create an array to build the password character by character.
            var password = new char[PasswordLength];

            // Guarantee at least one character from each required group.
            password[0] = GetRandomCharacter(Uppercase);
            password[1] = GetRandomCharacter(Lowercase);
            password[2] = GetRandomCharacter(Digits);
            password[3] = GetRandomCharacter(SpecialCharacters);

            // Fill the remaining positions with random characters.
            for (var i = 4; i < PasswordLength; i++)
            {
                password[i] = GetRandomCharacter(AllCharacters);
            }

            // Randomize the positions of all characters.
            Shuffle(password);


            return new string(password);
        }

        /// <summary>
        ///Returns one cryptographically random character from the given group.
        /// </summary>
        /// <param name="characters"></param>
        /// <returns></returns>
        private static char GetRandomCharacter(string characters)
        {
            var index = RandomNumberGenerator.GetInt32(characters.Length);
            return characters[index];
        }

        /// <summary>
        ///  Randomly swaps character positions using the Fisher-Yates algorithm.
        /// </summary>
        /// <param name="characters"></param>
        private static void Shuffle(char[] characters)
        {
            for (var i = characters.Length - 1; i > 0; i--)
            {
                // Select a random position between 0 and the current index.
                var j = RandomNumberGenerator.GetInt32(i + 1);

                // Swap the characters at positions i and j.
                (characters[i], characters[j]) =
                    (characters[j], characters[i]);
            }
        }
    }
}
