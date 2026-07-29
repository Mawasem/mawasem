import { Outlet } from "react-router-dom";
import { GalleryVerticalEnd } from "lucide-react";

export default function AuthLayout() {
  return (
    <main className="grid min-h-svh lg:grid-cols-2">
      <section className="flex flex-col gap-4 p-6 md:p-10">
        <div className="flex justify-center md:justify-start">
          <a
            href="/"
            className="flex items-center gap-2 font-medium"
          >
            <div className="flex size-8 items-center justify-center rounded-md bg-primary text-primary-foreground">
              <GalleryVerticalEnd className="size-4" />
            </div>

            <span>Mawasem</span>
          </a>
        </div>

        <div className="flex flex-1 items-center justify-center">
          <div className="w-full max-w-sm">
            <Outlet />
          </div>
        </div>
      </section>

      <section className="relative hidden overflow-hidden bg-muted lg:block">
        <img
          src="/images/auth-cover.jpg"
          alt=""
          className="absolute inset-0 h-full w-full object-cover dark:brightness-[0.35]"
        />

        <div className="absolute inset-0 bg-gradient-to-t from-background/60 via-transparent to-transparent" />
      </section>
    </main>
  );
}