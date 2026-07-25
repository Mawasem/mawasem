import mawasem from "@/assets/coming soon.jpeg";

export default function HomePage() {
  return (
    <div className="h-screen w-screen overflow-hidden">
      <img
        src={mawasem}
        alt="Coming Soon"
        className="h-full w-full object-contain"
      />
    </div>
  );
}