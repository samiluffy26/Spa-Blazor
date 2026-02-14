# RelaxSpa - Blazor Web App

A modern, responsive Single Page Application (SPA) for a Spa & Wellness center, built with **.NET 9 Blazor Web App** and **Tailwind CSS v4**.

This project is a migration from an existing React application, leveraging the power of Blazor's `InteractiveAuto` render mode for optimal performance and SEO.

## 🚀 Features

*   **Modern UI/UX**: Clean, responsive design using Tailwind CSS v4.
*   **Blazor Interactive Auto**: Combines server-side prerendering with client-side interactivity (WebAssembly) for fast load times and rich user experience.
*   **Booking System**: Multi-step wizard for scheduling appointments (Service selection, Date/Time, Customer Info).
*   **Service Catalog**: Browse services with filtering by category, search, and sorting.
*   **My Reservations**: tailored dashboard for users to view and manage their bookings.
*   **responsive Navigation**: Mobile-friendly menu and sticky header.

## 🛠 Technology Stack

*   **Framework**: [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
*   **Platform**: Blazor Web App (Server + Client)
*   **Styling**: [Tailwind CSS v4](https://tailwindcss.com/)
*   **Icons**: [Lucide](https://lucide.dev/) (via SVG integration)
*   **Language**: C# 13

## 📋 Prerequisites

Ensure you have the following installed:

1.  **[.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)** (or later)
2.  **[Node.js](https://nodejs.org/)** (v18 or later) - *Required for building Tailwind CSS*

## ⚙️ Setup & Run

### 1. Clone the repository
```bash
git clone https://github.com/samiluffy26/Spa-Blazor.git
cd Spa-Blazor
```

### 2. Build Styles (Crucial)
The application uses Tailwind CSS, which must be generated before running the app.

```bash
# Install dependencies
npm install

# Build CSS (Run this in a separate terminal to watch for changes, or just once)
npm run build:css
```

### 3. Run the Application
Since this is a hosted Blazor Web App solution, you must run the **Server** project:

```bash
dotnet run --project spa-reservas-blazor/spa-reservas-blazor.csproj
```

The application will launch at `http://localhost:5199` (check your console output for the exact port).

## 📂 Project Structure

*   `spa-reservas-blazor`: **Server Project**. Hosts the application, handles API requests (if any), and prerenders pages.
    *   `Components/Pages`: Server-side pages (Home, Error).
    *   `Components/Layout`: Main layouts (Header, Footer).
*   `spa-reservas-blazor.Client`: **Client Project**. Contains interactive UI components that can run on WebAssembly.
    *   `Components/Pages`: Interactive pages (BookingPage, Services, MyReservations).
    *   `Models`: Shared data models (`Booking`, `Service`).
    *   `Services`: Client-side services (`BookingService`).
*   `Styles`: Source CSS files (Tailwind input).
*   `wwwroot`: Static assets (images, generated CSS).

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License.
