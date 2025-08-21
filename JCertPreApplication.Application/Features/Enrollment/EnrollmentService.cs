using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Enrollment;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.Payment;
using Microsoft.Extensions.Logging;

namespace JCertPreApplication.Application.Features.Enrollment
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<EnrollmentService> _logger;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            IUserRepository userRepository,
            ICourseRepository courseRepository,
            IPaymentService paymentService,
            ILogger<EnrollmentService> logger)
        {
            _enrollmentRepository = enrollmentRepository ?? throw new ArgumentNullException(nameof(enrollmentRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<EnrollmentResponseDto> EnrollUserAsync(Guid userId, Guid courseId)
        {
            _logger.LogInformation("Starting enrollment process for user {UserId} and course {CourseId}", userId, courseId);

            // 1. Validate input
            if (userId == Guid.Empty)
                throw ApiException.BadRequest("INVALID_USER_ID", "User ID cannot be empty");
            
            if (courseId == Guid.Empty)
                throw ApiException.BadRequest("INVALID_COURSE_ID", "Course ID cannot be empty");

            // 2. Get user and course
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw ApiException.NotFound("USER_NOT_FOUND", "User not found");

            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw ApiException.NotFound("COURSE_NOT_FOUND", "Course not found");

            // 3. Check if user is already enrolled
            var isAlreadyEnrolled = await _enrollmentRepository.IsUserEnrolledAsync(userId, courseId);
            if (isAlreadyEnrolled)
                throw ApiException.BadRequest("ALREADY_ENROLLED", "User is already enrolled in this course");

            // 4. Check if course is available for enrollment
            if (course.status != Domain.Enums.CourseStatus.Published)
                throw ApiException.BadRequest("COURSE_NOT_AVAILABLE", "Course is not available for enrollment");

            // 5. Check sufficient credit using PaymentService
            var hasSufficientCredit = await _paymentService.HasSufficientCreditAsync(userId, course.price);
            if (!hasSufficientCredit)
            {
                var currentCredit = user.credit;
                var requiredCredit = (int)Math.Ceiling(course.price);
                throw ApiException.BadRequest("INSUFFICIENT_CREDIT", 
                    $"Insufficient credit. Required: {requiredCredit}, Available: {currentCredit}");
            }

            try
            {
                // 6. Process payment using PaymentService
                var paymentResult = await _paymentService.ProcessCreditPaymentAsync(
                    userId, 
                    courseId, 
                    course.price, 
                    $"Course enrollment: {course.title}");

                if (!paymentResult.IsSuccess)
                {
                    throw ApiException.BadRequest("PAYMENT_FAILED", paymentResult.Message);
                }

                // 7. Create enrollment after successful payment
                var enrollment = new Domain.Entities.Enrollment
                {
                    enrollmentId = Guid.NewGuid(),
                    userId = userId,
                    courseId = courseId,
                    enrollDate = DateTime.UtcNow,
                    price = course.price
                };

                await _enrollmentRepository.InsertAsync(enrollment);
                await _enrollmentRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully enrolled user {UserId} in course {CourseId} with payment {PaymentId}", 
                    userId, courseId, paymentResult.PaymentId);

                // 8. Return response
                return new EnrollmentResponseDto
                {
                    EnrollmentId = enrollment.enrollmentId,
                    UserId = userId,
                    CourseId = courseId,
                    CourseTitle = course.title,
                    PricePaid = course.price,
                    EnrollDate = enrollment.enrollDate,
                    RemainingCredit = paymentResult.RemainingCredit,
                    Message = "Successfully enrolled in the course"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during enrollment process for user {UserId} and course {CourseId}", userId, courseId);
                throw ApiException.InternalServerError("ENROLLMENT_FAILED", "An error occurred during enrollment process");
            }
        }

        public async Task<bool> IsUserEnrolledAsync(Guid userId, Guid courseId)
        {
            return await _enrollmentRepository.IsUserEnrolledAsync(userId, courseId);
        }

        public async Task<IEnumerable<EnrollmentResponseDto>> GetUserEnrollmentsAsync(Guid userId)
        {
            var enrollments = await _enrollmentRepository.GetUserEnrollmentsAsync(userId);
            
            return enrollments.Select(e => new EnrollmentResponseDto
            {
                EnrollmentId = e.enrollmentId,
                UserId = e.userId,
                CourseId = e.courseId,
                CourseTitle = e.Course?.title ?? "Unknown Course",
                PricePaid = e.price,
                EnrollDate = e.enrollDate,
                RemainingCredit = 0, // This would need to be calculated separately
                Message = "Enrollment record"
            });
        }

        public async Task<bool> UnenrollUserAsync(Guid userId, Guid courseId)
        {
            var enrollment = await _enrollmentRepository.GetFirstOrDefaultAsync(
                e => e.userId == userId && e.courseId == courseId);

            if (enrollment == null)
                return false;

            // Note: You might want to refund credit here
            // var user = await _userRepository.GetByIdAsync(userId);
            // user.credit += (int)enrollment.price;
            // await _userRepository.UpdateAsync(user);

            await _enrollmentRepository.DeleteAsync(enrollment);
            await _enrollmentRepository.SaveChangesAsync();

            return true;
        }
    }
} 