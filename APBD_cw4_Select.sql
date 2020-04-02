select distinct s.IndexNumber, s.FirstName, s.LastName, st.Name, e.Semester from student as s
join Enrollment as e on s.IdEnrollment=e.IdEnrollment
join Studies as st on st.IdStudy=e.IdStudy
order by s.IndexNumber