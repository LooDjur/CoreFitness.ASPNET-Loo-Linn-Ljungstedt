using Domain.Abstractions;
using Domain.Courses.Entities;
using Domain.Courses.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Courses.Repository;

public interface ICourseRepository : IRepositoryBase<CourseEntity>
{
    Task<IEnumerable<CourseEntity>> GetUpcomingCoursesAsync(CancellationToken ct = default);
    Task<IEnumerable<CourseEntity>> GetByCategoryAsync(CourseCategory category, CancellationToken ct = default);
}
