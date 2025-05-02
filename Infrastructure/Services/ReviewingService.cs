using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ReviewingingService(MediCallContext _context)
    {

        public async Task AddReviewAsync(Reviewing review)
        {
            _context.Reviewings.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsVisitAlreadyReviewedAsync(int visitId)
        {
            return await _context.Reviewings.AnyAsync(r => r.VisitId == visitId);
        }

        public async Task<List<Reviewing>> GetReviewsByNurseIdAsync(string nurseId)
        {
            return await _context.Reviewings
                .Include(r => r.Visit)
                .Where(r => r.Visit.NurseId == nurseId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingForNurseAsync(string nurseId)
        {
            return await _context.Reviewings
                .Include(r => r.Visit)
                .Where(r => r.Visit.NurseId == nurseId)
                .AverageAsync(r => (double?)r.Rating) ?? 0.0;
        }
    }
}
