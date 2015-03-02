using RMDashboard.Models;
using System.Collections.Generic;

namespace RMDashboard.Repositories
{
    public interface IReleaseRepository
    {
        List<ReleasePath> GetReleasePaths();

        DataModel GetReleaseData(string includedReleasePathIds, int releaseCount);
    }
}
