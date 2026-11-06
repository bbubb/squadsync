import { CreateTeamForm } from "@/features/teams/create-team-form";

export default function CreateTeamPage() {
  return (
    <div className="mx-auto w-full max-w-6xl">
      <div className="page-heading">
        <p className="eyebrow">Local workspace</p>
        <h1>Create Team</h1>
        <p>Start with a name. Your new Team starts without members or roster entries.</p>
      </div>
      <section className="team-panel" aria-label="New Team">
        <CreateTeamForm />
      </section>
    </div>
  );
}
