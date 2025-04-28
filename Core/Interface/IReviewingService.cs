using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface IReviewingService
    {
        Task AddReviewAsync(Reviewing review);
        Task<bool> IsVisitAlreadyReviewedAsync(int visitId);
        Task<List<Reviewing>> GetReviewsByNurseIdAsync(string nurseId);
        Task<double> GetAverageRatingForNurseAsync(string nurseId);

    }
}
