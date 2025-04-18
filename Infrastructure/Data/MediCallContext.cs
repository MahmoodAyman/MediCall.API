using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class MediCallContext(DbContextOptions options) : DbContext(options)
    {
        // TODO: Create DbSets
    }
}
