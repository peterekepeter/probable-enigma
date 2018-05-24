using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace ProbableEnigma
{
    public static class ProbableEnigmaExtensions
    {
        public static IApplicationBuilder UseProbableEnigma(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ProbableEnigmaMiddleware>();
            return builder;
        }
    }
}
