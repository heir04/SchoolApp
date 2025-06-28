using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApp.Core.Helper
{
    public class ValidatorHelper
    {
        private static readonly HashSet<string> ValidCategories = ["Junior", "Senior"];

        public bool ValidateScores(double examScore, double continuousAssessment)
        {

            if (examScore < 0 || examScore > 60)
            {
                return false;
            }

            if (continuousAssessment < 0 || continuousAssessment > 40)
            {
                return false;
            }

            return true;
        }

        public bool ValidateCategory(string category)
        {
            return ValidCategories.Contains(category);
        }

        public string ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return "Email cannot be empty.";
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                return "Invalid email format.";
            }

            return string.Empty; // Valid email
        }
    }
}