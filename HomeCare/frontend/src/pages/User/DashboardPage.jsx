import React, { useEffect, useState } from "react";
import ReminderList from "../../components/ReminderList";
import AppointmentList from "../../components/AppointmentList";
import Calendar from "../../components/Calendar";
import { getUserAppointments, getUserReminders } from "../../api/bookingApi"; 
import { Link } from "react-router-dom";

export default function DashboardPage() {
  const [reminders, setReminders] = useState([]);
  const [appointments, setAppointments] = useState([]);
  const [userName, setUserName] = useState("");

  // Hent innlogget bruker (fra localStorage, JWT, eller API)
  useEffect(() => {
    const storedName = localStorage.getItem("userName");
    if (storedName) setUserName(storedName);
  }, []);

  // Hente påminnelser + avtaler
  useEffect(() => {
    async function fetchData() {
      try {
        const reminderData = await getUserReminders();
        const appointmentData = await getUserAppointments();

        setReminders(reminderData || []);
        setAppointments(appointmentData || []);
      } catch (error) {
        console.error("Feil ved henting av dashboard-data:", error);
      }
    }

    fetchData();
  }, []);

  return (
    <div className="container py-4">
      {/* HEADER */}
      <div className="dashboard-header text-center py-3">
        <h2 className="text-green">Velkommen til din side</h2>
        <h2>{userName ? `${userName}!` : "Bruker"}</h2>
      </div>

      {/* MAIN CONTENT */}
      <div className="dashboard-main d-flex flex-wrap gap-4 mt-4">

        {/* LEFT SIDE */}
        <div className="dashboard-left flex-fill p-3 shadow-sm bg-white rounded-4" style={{ minWidth: "320px" }}>
          
          {/* Reminders */}
          <div className="reminder-section mb-4">
            <h3 className="mb-3">Påminnelser</h3>
            <ReminderList reminders={reminders} />
          </div>

          {/* Appointments */}
          <div className="appointment-section">
            <h3 className="mb-3">Dine timer</h3>
            <AppointmentList appointments={appointments} />

            <div className="mt-3">
              <Link to="/booking" className="btn btn-main">
                Book time her
              </Link>
            </div>
          </div>
        </div>

        {/* RIGHT SIDE – Calendar */}
        <div className="dashboard-right flex-fill p-3 shadow-sm bg-white rounded-4" style={{ minWidth: "320px" }}>
          <h3 className="mb-3">Kalender</h3>
          <Calendar appointments={appointments} />
        </div>

      </div>
    </div>
  );
}
