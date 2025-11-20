using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using HomeCare.Controllers;
using HomeCare.Data;
using HomeCare.Models;

namespace HomeCare.Tests.Controllers
{
    public class UserControllerTests
    {
        // Helper: create fresh in-memory context for each test
        private AppDbContext CreateContextWithData(bool seedAppointments = true)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            if (seedAppointments)
            {
                // Seed some appointments (past + future)
                var today = DateTime.Today;

                var cat1 = new Category { Id = 1, Name = "Category 1" };
                var cat2 = new Category { Id = 2, Name = "Category 2" };
                var cat3 = new Category { Id = 3, Name = "Category 3" };

                context.Categories.AddRange(cat1, cat2, cat3);

                context.Appointments.AddRange(
                    new Appointment
                    {
                        Id = 1,
                        DateTime = today.AddHours(9),   // today
                        Category = cat1
                    },
                    new Appointment
                    {
                        Id = 2,
                        DateTime = today.AddDays(1).AddHours(10), // tomorrow
                        Category = cat2
                    },
                    new Appointment
                    {
                        Id = 3,
                        DateTime = today.AddDays(-1).AddHours(8), // yesterday (should NOT be included)
                        Category = cat3
                    }
                );

                context.SaveChanges();
            }

            return context;
        }

        
        /* Testing -  Dashboard with no year/month => uses today's year/month and 
        returns reminders + future appointments */

        [Fact]
        public void Dashboard_NoParameters_ReturnsView_WithRemindersAndFutureAppointments()
        {
            // Arrange
            using var context = CreateContextWithData();
            var controller = new UserController(context);
            var today = DateTime.Today;

            // Act
            var result = controller.Dashboard(null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            // Check ViewBag calendar values
            Assert.Equal(today.Year, (int)controller.ViewBag.CalendarYear);
            Assert.Equal(today.Month, (int)controller.ViewBag.CalendarMonth);

            // Check model type and content
            var model = Assert.IsType<(List<Reminder> reminders, List<Appointment> appointments)>(viewResult.Model);

            // Reminders come from GetTodayReminders() (2 dummy reminders)
            Assert.Equal(2, model.reminders.Count);

            // Appointments: only today and future, ordered by DateTime
            Assert.All(model.appointments, a => Assert.True(a.DateTime.Date >= today));
            var ordered = model.appointments.OrderBy(a => a.DateTime).ToList();
            Assert.Equal(ordered, model.appointments);
        }

        // Testing - Dashboard with specific year/month
        [Fact]
        public void Dashboard_WithYearAndMonth_SetsCalendarToProvidedValues()
        {
            // Arrange
            using var context = CreateContextWithData(seedAppointments: false);
            var controller = new UserController(context);

            int year = 2030;
            int month = 12;

            // Act
            var result = controller.Dashboard(year, month);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal(year, (int)controller.ViewBag.CalendarYear);
            Assert.Equal(month, (int)controller.ViewBag.CalendarMonth);

            // Model still has the correct tuple type
            var model = Assert.IsType<(List<Reminder> reminders, List<Appointment> appointments)>(viewResult.Model);
            Assert.Equal(2, model.reminders.Count);         // from GetTodayReminders()
            Assert.Empty(model.appointments);               // no seeded appointments in this test
        }

        
        // Testing - Dashboard when there are no appointments in DB
        [Fact]
        public void Dashboard_NoAppointments_ReturnsEmptyAppointmentsList()
        {
            // Arrange
            using var context = CreateContextWithData(seedAppointments: false);
            var controller = new UserController(context);

            // Act
            var result = controller.Dashboard(null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<(List<Reminder> reminders, List<Appointment> appointments)>(viewResult.Model);

            Assert.Equal(2, model.reminders.Count); // default reminders
            Assert.Empty(model.appointments);       // nothing in DB
        }
    }
}
