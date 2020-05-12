using Dapper;
using FluentValidation;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Puzzles.Commands
{
    public abstract class PuzzleCommandBase
    {
        public string Title { get; set; }

        public int NumPieces { get; set; }

        public int NumPiecesMissing { get; set; }

        public string AdditionalNotes { get; set; }

        public string Tags { get; set; }

        public IFormFile ImageBlob { get; set; }
    }

    public class PuzzleCommandBaseValidator : AbstractValidator<PuzzleCommandBase>
    {
        public PuzzleCommandBaseValidator()
        {
            RuleFor(v => v.Title)
                .NotNull()
                .Length(5, 100);

            RuleFor(v => v.AdditionalNotes)
                .MaximumLength(200);

            RuleFor(v => v.NumPieces)
                .GreaterThan(0);

            RuleFor(v => v.NumPiecesMissing)
                .GreaterThanOrEqualTo(0)
                .LessThan(v => v.NumPieces)
                .WithMessage(v => $"Must be lower than # Pieces ({v.NumPieces})");

            RuleFor(v => v.Tags)
                .MaximumLength(310); // (10 tags * 30 chars) + 10 delimiters
        }
    }
}