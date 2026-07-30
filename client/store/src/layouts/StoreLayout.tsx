import Footer from "@/components/Footer";
import StoreNavbar from "@/components/Navbar";
import { Outlet } from "react-router-dom";

export default function StoreLayout() {
  return (
    <div className="flex min-h-screen flex-col bg-background">
      <StoreNavbar />

      <main className="flex-1">
        <div className="container mx-auto px-4 py-8 md:px-6 lg:px-8">
          <Outlet />
        </div>
      </main>

      <Footer />
    </div>
  );
}