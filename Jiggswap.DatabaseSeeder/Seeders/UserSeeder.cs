using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using static Jiggswap.DatabaseSeeder.Seeders.PuzzleSeeder;

namespace Jiggswap.DatabaseSeeder.Seeders
{
    internal class UserSeeder
    {
        private NpgsqlConnection _dbConn;

        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 10000;

        private static string HashPassword(string password)
        {
            using var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithmName.SHA512);
            var salt = Convert.ToBase64String(algorithm.Salt);
            var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));

            return $"{Iterations}.{salt}.{key}";
        }

        public class SeededUser
        {
            public int Id { get; set; }

            public string Username { get; set; }

            public string Email { get; set; }

            public string PasswordHash => HashPassword("password");

            public SeededUserProfile Profile { get; set; }

            public List<SeededPuzzle> Puzzles { get; set; }

            public override string ToString()
            {
                return $"{Username} <{Email}> ({Id})";
            }

            public SeededUser(string username, string email, SeededUserProfile? profile)
            {
                Username = username;
                Email = email;
                Profile = profile;
                Puzzles = new List<SeededPuzzle>();
            }
        }

        public class SeededUserProfile
        {
            public int UserId { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string StreetAddress { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public string Zip { get; set; }
        }

        public UserSeeder(NpgsqlConnection dbConn)
        {
            _dbConn = dbConn;
        }

        public void CreateUser(SeededUser user)
        {
            var findSql = "select id from users where username = @Username";

            var id = _dbConn.QuerySingleOrDefault<int>(findSql, user);

            if (id == 0)
            {
                var createSql = "insert into users (username, email, password_hash) values (@Username, @Email, @PasswordHash) returning id";

                id = _dbConn.QuerySingle<int>(createSql, user);

                user.Profile.UserId = id;
                var profileSql = @"insert into user_profiles
                    (user_id, firstname, lastname,
                     streetaddress, city, state, zip)
                     values (@UserId, @FirstName, @LastName,
                     @StreetAddress, @City, @State, @Zip)";
                _dbConn.Query(profileSql, user.Profile);

                Console.WriteLine($"Created User: {user}");
            }
            else
            {
                Console.WriteLine($"User already exists: {user}");
            }

            user.Id = id;
        }
    }
}