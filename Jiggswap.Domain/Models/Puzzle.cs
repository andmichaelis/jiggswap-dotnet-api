using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jiggswap.Domain.Models
{
    [Table("puzzles")]
    public class Puzzle
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("public_id")]
        public Guid PublicId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("owner_id")]
        public int OwnerId { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("num_pieces")]
        public int NumPieces { get; set; }

        [Column("num_pieces_missing")]
        public int NumPiecesMissing { get; set; }

        [Column("additional_notes")]
        public string AdditionalNotes { get; set; }

        [Column("image_id")]
        public int ImageId { get; set; }

        [Column("tags")]
        public string Tags { get; set; }

        [Column("brand")]
        public string Brand { get; set; }
    }
}