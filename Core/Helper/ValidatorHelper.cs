using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Core.Helper
{
    public class ValidatorHelper
    {
        private static readonly HashSet<string> ValidCategories = ["Junior", "Senior"];
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ValidatorHelper(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        // Authentication & Authorization Validators
        public (bool IsValid, string? ErrorMessage, Guid UserId) ValidateUser()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                return (false, "Invalid user ID", Guid.Empty);
            
            return (true, null, userId);
        }

        public async Task<(bool IsValid, string? ErrorMessage, Teacher? Teacher)> ValidateTeacherAsync(Guid userId)
        {
            var teacher = await _unitOfWork.Teacher.Get(t => t.UserId == userId && !t.IsDeleted);
            if (teacher is null)
                return (false, "Teacher not found", null);
            
            return (true, null, teacher);
        }

        public async Task<(bool IsValid, string? ErrorMessage, Student? Student)> ValidateStudentAsync(Guid userId)
        {
            var student = await _unitOfWork.Student.Get(s => s.UserId == userId);
            if (student is null)
                return (false, "Student not found", null);
            
            return (true, null, student);
        }

        public async Task<(bool IsValid, string? ErrorMessage, Admin? Admin)> ValidateAdminAsync(Guid userId)
        {
            var admin = await _unitOfWork.Admin.Get(a => a.UserId == userId && !a.IsDeleted);
            if (admin is null)
                return (false, "Admin not found", null);
            
            return (true, null, admin);
        }

        // Session & Term Validators
        public async Task<(bool IsValid, string? ErrorMessage, Session? Session, Term? Term)> ValidateCurrentSessionAndTermAsync()
        {
            var session = await _unitOfWork.Session.GetCurrentSession();
            if (session is null)
                return (false, "No session set", null, null);
            
            var term = session.Terms?.FirstOrDefault(t => t.CurrentTerm == true);
            if (term is null)
                return (false, "No current term set", session, null);
            
            return (true, null, session, term);
        }

        // Entity Validators
        public async Task<(bool IsValid, string? ErrorMessage, Subject? Subject)> ValidateSubjectAsync(Guid subjectId)
        {
            if (subjectId == Guid.Empty)
                return (false, "Subject is required", null);
            
            var subject = await _unitOfWork.Subject.Get(s => s.Id == subjectId);
            if (subject is null)
                return (false, "Subject not found", null);
            
            return (true, null, subject);
        }

        public async Task<(bool IsValid, string? ErrorMessage, Level? Level)> ValidateLevelAsync(Guid levelId)
        {
            if (levelId == Guid.Empty)
                return (false, "Level is required", null);
            
            var level = await _unitOfWork.Level.Get(l => l.Id == levelId);
            if (level is null)
                return (false, "Level not found", null);
            
            return (true, null, level);
        }

        public async Task<(bool IsValid, string? ErrorMessage, Assignment? Assignment)> ValidateAssignmentAsync(Guid assignmentId)
        {
            if (assignmentId == Guid.Empty)
                return (false, "Assignment ID is required", null);
            
            var assignment = await _unitOfWork.Assignment.Get(a => a.Id == assignmentId && !a.IsDeleted);
            if (assignment is null)
                return (false, "Assignment not found", null);
            
            return (true, null, assignment);
        }

        // Business Logic Validators
        public (bool IsValid, string? ErrorMessage) ValidateAssignmentOwnership(Assignment assignment, Teacher teacher)
        {
            if (assignment.TeacherId != teacher.Id)
                return (false, "You are not authorized to access this assignment");
            
            return (true, null);
        }

        public (bool IsValid, string? ErrorMessage) ValidateAssignmentDueDate(Assignment assignment)
        {
            if (assignment.DueDate < DateOnly.FromDateTime(DateTime.Now))
                return (false, "Can't update after due date");
            
            return (true, null);
        }

        // Existing validators...

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