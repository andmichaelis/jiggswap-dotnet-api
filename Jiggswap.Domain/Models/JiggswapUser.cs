using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jiggswap.Domain.Models
{
    [Table("users")]
    public class JiggswapUser
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("public_id")]
        public Guid PublicId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("password_hash")]
        public string PasswordHash { get; set; }
    }
}