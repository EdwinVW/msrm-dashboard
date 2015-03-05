using System;
using System.Text.RegularExpressions;

namespace RMDashboard.Validators
{
    internal static class IncludedReleasePathIdsValidator
    {
        public static bool IsValidIncludedReleasePathIds(string includedReleasePathIds)
        {
            return (string.IsNullOrEmpty(includedReleasePathIds) ||
                Regex.IsMatch(includedReleasePathIds, @"^\d+(,\d+)*$"));
        }
    }
}