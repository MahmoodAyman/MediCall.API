using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Core.Models
{
    [Owned]
    public class Location
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
