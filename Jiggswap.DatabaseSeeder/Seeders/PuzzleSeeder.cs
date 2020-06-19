using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Jiggswap.DatabaseSeeder.Seeders
{
    internal class PuzzleSeeder
    {
        private NpgsqlConnection _db;
        private Random _rand;

        public class SeededPuzzle
        {
            public int Id { get; set; }

            public int OwnerId { get; set; }

            public int ImageId { get; set; }

            public string Title { get; set; }

            public int NumPieces { get; set; }

            public int NumPiecesMissing { get; set; }

            public string AdditionalNotes { get; set; }

            public string Tags { get; set; }

            public string Brand { get; set; }

            public override string ToString()
            {
                return $"Title {Title}\tOwner:{OwnerId}";
            }
        }

        public PuzzleSeeder(NpgsqlConnection db)
        {
            _db = db;
            _rand = new Random();
        }

        private List<string> Brands = new List<string> { "Buffalo Games", "Ravensburger", "Springbok", "White Mountain Puzzles", "Cobble Hill" };

        private List<string> Tags = new List<string>
        {
            "condescension",
            "winterization",
            "secularity",
            "accredited",
            "cotylosaur",
            "multipresent",
            "deteriorative",
            "dentinocemental",
            "coracomorphae",
            "unphonographed",
            "pelagothuria",
            "vituperate",
            "azerbaijanian",
            "tredecillionth"
        };

        private List<int> NumPieces = new List<int> { 100, 150, 200, 250, 500, 1000, 2000 };

        private List<int> NumPiecesMissing = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 10, 30 };

        private List<string> S3DummyImages = new List<string> { "dummy_images/1.png", "dummy_images/2.png", "dummy_images/3.png", "dummy_images/4.png", "dummy_images/5.png", "dummy_images/6.png", "dummy_images/7.png", "dummy_images/8.png" };

        private int GetRandomS3Image()
        {
            var s3Filename = S3DummyImages.OrderBy(i => Guid.NewGuid()).First();

            var url = "https://" + "dx30g48l30v8i.cloudfront.net" + "/" + s3Filename;

            return _db.QuerySingle<int>("insert into images (image_data, image_url, s3_filename) values (@ImageData, @S3Url, @S3Filename) returning id", new { ImageData = new byte[] { }, S3Url = url, S3Filename = s3Filename });
        }

        private int CreateRandomImage()
        {
            var color = Color.FromArgb(_rand.Next(255), _rand.Next(255), _rand.Next(255));
            var img = new Bitmap(400, 300);

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    img.SetPixel(x, y, color);
                }
            }

            using (var stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Png);
                var bytes = stream.ToArray();

                return _db.QuerySingle<int>("insert into images (image_data) values (@bytes) returning id", new { bytes });
            }
        }

        internal SeededPuzzle CreateRandomPuzzleForUser(UserSeeder.SeededUser user)
        {
            var imageId = GetRandomS3Image();

            var puzz = new SeededPuzzle
            {
                Title = $"{user.Username}-{Path.GetRandomFileName().Substring(0, 3)}",
                Brand = Brands.OrderBy(b => Guid.NewGuid()).First(),
                AdditionalNotes = $"{Path.GetRandomFileName()}{Path.GetRandomFileName()}",
                Tags = string.Join(",", Tags.OrderBy(t => Guid.NewGuid()).Take(3)),
                NumPieces = NumPieces.OrderBy(n => Guid.NewGuid()).First(),
                NumPiecesMissing = NumPiecesMissing.OrderBy(m => Guid.NewGuid()).First(),
                ImageId = imageId,
                OwnerId = user.Id,
            };

            puzz.Id = _db.QuerySingle<int>(@"insert into puzzles
                (owner_id, title,
                num_pieces, num_pieces_missing,
                additional_notes, image_id,
                tags, brand)
                values
                (
                    @OwnerId, @Title,
                    @NumPieces, @NumPiecesMissing,
                    @AdditionalNotes, @ImageId,
                    @Tags, @Brand
                ) returning id", puzz);

            Console.WriteLine($"Puzzle Seeded: {puzz}");

            user.Puzzles.Add(puzz);

            return puzz;
        }
    }
}