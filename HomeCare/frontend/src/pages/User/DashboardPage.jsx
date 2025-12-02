import React, { useEffect, useState } from "react";
import { getBookingPage } from "../../api/bookingApi";

import AppointmentList from "../../components/AppointmentList";
import ReminderList from "../../components/ReminderList";
import Calendar from "../../components/Calendar";

const DashboardPage = () => {
  const [appointments, setAppointments] = useState([]);
  const [reminders, setReminders] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function loadData() {
      try {
        const page = await getBookingPage();

        const allAppointments = page.bookings || [];

        // Reminders = de neste 3 kommende avtalene
        const upcoming = allAppointments
          .filter((a) => new Date(a.date) >= new Date())
          .sort((a, b) => new Date(a.date) - new Date(b.date))
          .slice(0, 3);

        setAppointments(allAppointments);
        setReminders(upcoming);
      } catch (err) {
        console.error("Failed to load dashboard data:", err);
      } finally {
        setLoading(false);
      }
    }

    loadData();
  }, []);

  if (loading) return <p className="text-center mt-5">Laster...</p>;

  return (
    <div className="container py-4">

      <div className="dashboard-header text-green text-center mb-4">
        <h2>Velkommen til din side</h2>
        {/* Tilpass hvis dere har auth-løsning */}
        <h4>Bruker</h4>
      </div>

      <div className="dashboard-main d-flex flex-wrap">

        {/* LEFT SIDE */}
        <div className="dashboard-left flex-fill p-3" style={{ minWidth: "300px" }}>
          <div className="reminder-section mb-4">
            <h3>Påminnelser</h3>
            <ReminderList reminders={reminders} />
          </div>

          <div className="appointment-section">
            <h3>Dine timer</h3>
            <AppointmentList appointments={appointments} />

            <div className="action-button mt-3">
              <a href="/booking" className="btn btn-primary">
                Book time her
              </a>
            </div>
          </div>
        </div>

        {/* RIGHT SIDE */}
        <div className="dashboard-right flex-fill p-3" style={{ minWidth: "300px" }}>
          <h3>Kalender</h3>
          <Calendar appointments={appointments} />
        </div>
      </div>
    </div>
  );
};

export default DashboardPage;
